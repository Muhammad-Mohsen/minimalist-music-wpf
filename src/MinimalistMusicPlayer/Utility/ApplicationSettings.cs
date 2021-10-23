using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace MinimalistMusicPlayer.Utility
{
    // thanks - https://stackoverflow.com/questions/6115721/how-to-save-restore-serializable-object-to-from-file
    // used to store the volume level and current directory.
    // the built-in Settings class uses different contexts (users) for launching the app directly and launching it with a file arg!! so that's why I have my own implementation
    public class ApplicationSettings
    {
        private static readonly string SettingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Minimalist";
        private static readonly string SettingsFile = SettingsDirectory + "\\MinimalistSettings.bin";

        public static Data Instance = new Data();

        public static bool Save()
        {
            Directory.CreateDirectory(SettingsDirectory); // make sure the directory exists
            using (Stream stream = File.Open(SettingsFile, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, new Data
                {
                    Volume = Instance.Volume,
                    ExplorerDirectory = Instance.ExplorerDirectory
                }); ;
            }

            return true;
        }
        public static bool Load()
        {
            if (!File.Exists(SettingsFile)) return false;

            using (Stream stream = File.Open(SettingsFile, FileMode.Open))
            {
                if (stream.Length == 0) return false;

                var binaryFormatter = new BinaryFormatter();
                var data = (Data)binaryFormatter.Deserialize(stream);

                Instance.Volume = data.Volume;
                Instance.ExplorerDirectory = data.ExplorerDirectory;
            }

            return true;
        }

        [Serializable]
        public class Data
        {
            public float Volume = 0.4F;
            public string ExplorerDirectory;
        }
    }
}
