using System.Collections.Generic;
using System.IO;

namespace ChanisterWpf.Singletons
{
    public sealed class MediaSingleton
    {
        private static MediaSingleton instance = null;

        public Dictionary<long, Stream> mediaDictionary;
        public int dictSizeBytes;

        MediaSingleton()
        {
            mediaDictionary = new();
        }

        public static MediaSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MediaSingleton();
                }
                return instance;
            }
        }

    }
}