using System;
using System.Windows;

namespace ChanisterWpf
{
    public class SkinResourceDictionary : ResourceDictionary
    {
        private Uri redSource;
        private Uri blueSource;
        private Uri purpleSource;
        private Uri whiteSource;

        public Uri RedSource
        {
            get { return redSource; }
            set
            {
                redSource = value;
                UpdateSource();
            }
        }
        public Uri BlueSource
        {
            get { return blueSource; }
            set
            {
                blueSource = value;
                UpdateSource();
            }
        }
        public Uri PurpleSource
        {
            get { return purpleSource; }
            set
            {
                purpleSource = value;
                UpdateSource();
            }
        }
        public Uri WhiteSource
        {
            get { return whiteSource; }
            set
            {
                whiteSource = value;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            Uri newSource = null;
            switch (App.CurrentSkin)
            {
                case Skin.Classic_red:
                    newSource = RedSource;
                    break;
                case Skin.Classic_blue:
                    newSource = BlueSource;
                    break;
                case Skin.White_is_tight:
                    newSource = WhiteSource;
                    break;
                case Skin.Sakura_purple:
                    newSource = PurpleSource;
                    break;
                default:
                    break;
            }
            if (newSource != null && Source != newSource)
                Source = newSource;
        }
    }
}
