﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChanisterWpf
{
    public static class TagParser
    {
        private enum TagTypes
        {
            quote,
            prettyprint,
            quotelink,
            deadlink,
            spoiler,
            link,
            threadlink,
        }
        private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline;
        private static readonly Regex[] tags = {
            new Regex(@"^<span class=""quote"">(.*?)</span>$", Options),
            new Regex(@"<pre class=""prettyprint"">(.*?)</pre>", Options),
            new Regex(@"<a href=""#p(\d*)"" class=""quotelink"">>>\d*</a>", Options),
            new Regex(@"<span class=""deadlink"">(.*?)</span>", Options),
            new Regex(@"<s>(.*?)</s>", Options),
            new Regex(@"(https?://(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?://(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,}[^</span>])", Options),
            new Regex(@"<a href=""/(.+)/thread/(\d+)#p(\d+)"" class=""quotelink"">>>\d*</a>"),
        };
        public static string DecodeText(string text)
        {
            if (text == null) return null;
            text = System.Web.HttpUtility.HtmlDecode(text);
            text = text.Replace("<br>", "\n");
            text = text.Replace("<wbr>", "");
            return text;
        }
        private static int CompareIndex((int, int, int, string, GroupCollection) x, (int, int, int, string, GroupCollection) y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
        public static List<(int tagType, string text, GroupCollection)> FindFormattedText(string text)
        {
            List<(int tagType, string text, GroupCollection groups)> tagList = new();
            List<MatchCollection> matchCollections = new();
            foreach (Regex rx in tags)
            {
                matchCollections.Add(rx.Matches(text));
            }
            List<(int index, int count, int regIndex, string match, GroupCollection groups)> indexCountAndMatch = new();
            int reg = 0;
            foreach (MatchCollection matchCollection in matchCollections)
            {
                foreach (Match match in matchCollection)
                {
                    indexCountAndMatch.Add((match.Index, match.Length, reg, match.Groups[1].ToString(), match.Groups));
                }
                reg++;
            }
            indexCountAndMatch.Sort(CompareIndex);
            int i = 0;
            foreach ((int index, int count, int regIndex, string match, GroupCollection groups) in indexCountAndMatch)
            {
                if (i + 1 < indexCountAndMatch.Count && indexCountAndMatch[i + 1].index < index + count) //Handle regex match overlap
                {
                    int tempcount = index + count - indexCountAndMatch[i + 1].index - indexCountAndMatch[i + 1].count + 1;
                    tagList.Add((regIndex, text[index..(index + tempcount)], groups));
                }
                else if (index > i + 1)
                {
                    tagList.Add((-1, text[i..index], groups));
                    tagList.Add((regIndex, match, groups));
                }
                else
                {
                    tagList.Add((regIndex, match, groups));
                }
                i = index + count;
            }
            tagList.Add((-1, text[i..], null));
            return tagList;
        }
        public static void AddInlines(List<(int tagType, string text, GroupCollection groups)> tagList, InlineCollection inlines, int postNumber, int opNumber)
        {
            foreach ((int tagType, string text, GroupCollection groups) in tagList)
            {
                switch ((TagTypes)tagType)
                {
                    case TagTypes.quote:
                        inlines.Add(new Run("\n" + text) { Foreground = MainWindow.solidGreen });
                        break;
                    case TagTypes.prettyprint:
                        inlines.Add(new Run("\n" + text) { FontFamily = new FontFamily("Consolas") });
                        break;
                    case TagTypes.quotelink:
                        try
                        {
                            int postQuotedNumber = Convert.ToInt32(text);
                            string identifier = "";
                            if (postQuotedNumber == opNumber)
                            {
                                identifier = "(OP)";
                            }
                            inlines.Add("\n");
                            inlines.Add(new QuoteLink(postQuotedNumber, postNumber, identifier, true));
                        }
                        catch (Exception)
                        {
                            inlines.Add(text);
                        }
                        break;
                    case TagTypes.deadlink:
                        inlines.Add(new Run("\n" + text) { TextDecorations = TextDecorations.Strikethrough });
                        break;
                    case TagTypes.spoiler:
                        Run spoiler = new(text)
                        {
                            Background = new SolidColorBrush(Colors.Black)
                        };
                        spoiler.MouseEnter += new System.Windows.Input.MouseEventHandler(ToggleSpoiler);
                        spoiler.MouseLeave += new System.Windows.Input.MouseEventHandler(ToggleSpoiler);
                        inlines.Add(spoiler);
                        break;
                    case TagTypes.link:
                        Hyperlink hyperlink = new(new Run(text));
                        try
                        {
                            Uri uri = new(text);
                            hyperlink.NavigateUri = uri;
                            hyperlink.RequestNavigate += NavigateHyperlink;
                            inlines.Add("\n");
                            inlines.Add(hyperlink);
                        }
                        catch (Exception)
                        {
                            Debug.WriteLine("Error on hyperlink creation. " + hyperlink);
                            inlines.Add(new Run(text));
                        }
                        break;
                    case TagTypes.threadlink:
                        if (groups.Count < 4) break;
                        inlines.Add("\n");
                        inlines.Add(new ThreadLink(groups[1].ToString(), Convert.ToInt32(groups[2].ToString()), Convert.ToInt32(groups[3].ToString())));
                        break;
                    default:
                        inlines.Add(new Run(text));
                        break;
                }
            }
        }
        private static void NavigateHyperlink(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
        private static void ToggleSpoiler(object sender, RoutedEventArgs e)
        {
            Run spoiler = e.Source as Run;
            if (spoiler.Background == null)
            {
                spoiler.Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                spoiler.Background = null;
            }
        }
    }
}
