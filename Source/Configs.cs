using System.IO;
using Newtonsoft.Json;

namespace LarsOfTheStars.Source
{
    public class Configs
    {
        public int Update { get; set; }         //      System Specified
        public int MaxParticles { get; set; }   //    80
        public int MaxStars { get; set; }       //    40
        public int MaxSounds { get; set; }      //     8
        public bool FadeOut { get; set; }       //  true
        public float SoundVolume { get; set; }  //   100
        public float MusicVolume { get; set; }  //    75
        public uint FrameRate { get; set; }     //   120
        public bool VSync { get; set; }         //  true
        public bool Fullscreen { get; set; }    // false
        public uint ScreenHeight { get; set; }  //   576
        public uint ScreenWidth { get; set; }   //   768
        public float Difficulty { get; set; }   //     1
        public int AutoStart { get; set; }      //    -1
        public bool DiscordRPC { get; set; }     //  true

        public Configs(bool reset = false)
        {
            if (reset)
            {
                Update = Game.Version;
                MaxParticles = 80;
                MaxStars = 40;
                MaxSounds = 8;
                FadeOut = true;
                SoundVolume = 100;
                MusicVolume = 50;
                FrameRate = 120;
                VSync = true;
                Fullscreen = false;
                ScreenHeight = 576;
                ScreenWidth = 768;
                Difficulty = 1;
                AutoStart = -1;
                DiscordRPC = true;
            }
        }
        public static void Load()
        {
            if (File.Exists("LarsOfTheStars.json"))
            {
                Game.Configs = JsonConvert.DeserializeObject<Configs>(File.ReadAllText("LarsOfTheStars.json"));
                if (Game.Configs.MaxSounds == 0)
                {
                    Game.Configs.MaxSounds = 8;
                }
                if (Game.Configs.MaxSounds <= 3)
                {
                    Game.Configs.MaxSounds = 3;
                }
            }
            else
            {
                StreamWriter writer = File.CreateText("LarsOfTheStars.json");
                writer.WriteLine(JsonConvert.SerializeObject(new Configs(true), Formatting.Indented));
                writer.Flush(); writer.Close();
                Game.Configs = new Configs(true);
            }
        }
    }
}
