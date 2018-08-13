using System.IO;
using Newtonsoft.Json;

namespace LarsOfTheStars.Source
{
    public class Configs
    {
        public int Update { get; set; }         //      System Specified
        public int MaxParticles { get; set; }   //    80
        public int MaxStars { get; set; }       //    40
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
                this.Update = Game.Version;
                this.MaxParticles = 80;
                this.MaxStars = 40;
                this.FadeOut = true;
                this.SoundVolume = 100;
                this.MusicVolume = 50;
                this.FrameRate = 120;
                this.VSync = true;
                this.Fullscreen = false;
                this.ScreenHeight = 576;
                this.ScreenWidth = 768;
                this.Difficulty = 1;
                this.AutoStart = -1;
                this.DiscordRPC = true;
            }
        }
        public static void Load()
        {
            if (File.Exists("LarsOfTheStars.json"))
            {
                Game.Configs = JsonConvert.DeserializeObject<Configs>(File.ReadAllText("LarsOfTheStars.json"));
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
