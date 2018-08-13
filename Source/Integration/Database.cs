using LarsOfTheStars.Source.Integration.REST;
using System;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;

namespace LarsOfTheStars.Source.Integration
{
    public class Database
    {
        private static WebClient Browser = new WebClient();
        private static Stopwatch DeliverTimer = new Stopwatch();
        public static string IP = "http://162.243.134.234/";
        public static int TryAgain = 0;
        public static void SetScore(int score)
        {
            if (TryAgain < 12)
            {
                try
                {
                    if (Game.Configs.DiscordRPC && Discord.IsReady)
                    {
                        if (DeliverTimer.ElapsedMilliseconds > 100 || !DeliverTimer.IsRunning)
                        {
                            Discord.You.Score = score;
                            string json = JsonConvert.SerializeObject(Discord.You);
                            Browser.UploadStringAsync(new Uri(IP + "score"), json);
                            if (DeliverTimer.IsRunning)
                            {
                                DeliverTimer.Restart();
                            }
                            else
                            {
                                DeliverTimer.Start();
                            }
                        }
                    }
                    TryAgain = 0;
                }
                catch
                {
                    TryAgain += 1;
                }
            }
        }
    }
}
