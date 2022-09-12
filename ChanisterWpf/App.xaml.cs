using FlyleafLib;
using FlyleafLib.Controls.WPF;
using FlyleafLib.MediaPlayer;
using System.Windows;

namespace ChanisterWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static new App Current => Application.Current as App;
        private VideoView CurrentVideoView = null;
        public static Skin CurrentSkin
        { get; internal set; } = Skin.Classic_blue;
        public void ChangeSkin(Skin newSkin)
        {
            CurrentSkin = newSkin;
            foreach (ResourceDictionary dict in Resources.MergedDictionaries)
            {
                if (dict is SkinResourceDictionary skinDict)
                {
                    skinDict.UpdateSource();
                }
                else
                {
                    dict.Source = dict.Source;
                }
            }
        }
        public VideoView VideoView
        {
            get
            {
                if (Engine.Started)
                {
                    return CurrentVideoView;
                }
                else
                {
                    Engine.Start(new()
                    {
                        FFmpegPath = ":FFmpeg",
                        HighPerformaceTimers = false,
                        UIRefresh = true,
                        LogOutput = "Flyleaf.FirstRun.log",
                        LogLevel = LogLevel.Debug,
                        FFmpegDevices = false
                    });
                    Config config = new();
                    config.Player.Usage = Usage.AVS;
                    CurrentVideoView = new()
                    {
                        Player = new(config)
                    };
                    CurrentVideoView.Player.Audio.Volume = 50;
                    return CurrentVideoView;
                }
            }
            set
            {
                CurrentVideoView = value;
            }
        }
        public string VideoUriString { get; set; }
    }
}
