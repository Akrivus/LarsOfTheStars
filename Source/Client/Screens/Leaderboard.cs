using LarsOfTheStars.Source.Files;
using LarsOfTheStars.Source.Integration;
using LarsOfTheStars.Source.Integration.REST;
using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using SFML.Graphics;
using SFML.Window;
using Newtonsoft.Json;

namespace LarsOfTheStars.Source.Client.Screens
{
    public class Leaderboard
    {
        private bool Visible = false;
        private float Buffer = -84;
        private WebClient Browser = new WebClient();
        private Stopwatch UpdateTimer = new Stopwatch();
        private Sprite Board;
        private Text[] Ranks = new Text[10];
        private Text[] Names = new Text[10];
        private Text[] Score = new Text[10];
        public Leaderboard()
        {
            this.Board = new Sprite(Textures.Load("board.png"));
            this.Board.Position = new Vector2f(256 - (this.Board.Texture.Size.X + 1), 1);
            for (int i = 0; i < 10; ++i)
            {
                this.Ranks[i] = new Text("", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/gameover.ttf"), 75);
                this.Ranks[i].Scale = new Vector2f(0.1F, 0.1F);
                this.Ranks[i].Position = new Vector2f(256 - (this.Board.Texture.Size.X + 1 - 3), 6 * i + 10);
                this.Names[i] = new Text("", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/gameover.ttf"), 75);
                this.Names[i].Scale = new Vector2f(0.1F, 0.1F);
                this.Names[i].Position = new Vector2f(256 - (this.Board.Texture.Size.X + 1 - 9), 6 * i + 10);
                this.Score[i] = new Text("", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/gameover.ttf"), 75);
                this.Score[i].Scale = new Vector2f(0.1F, 0.1F);
            }
            this.UpdateTimer.Start();
            this.Browser.DownloadStringCompleted += Update;
        }
        private void Update(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                Rank rank = JsonConvert.DeserializeObject<Rank>(e.Result);
                bool ranked = false;
                if (Discord.RP.Party == null)
                {
                    Discord.RP.Party = new DiscordRPC.Party();
                }
                Discord.RP.Party = new DiscordRPC.Party();
                Discord.RP.Party.ID = rank.GetHashCode().ToString();
                Discord.RP.Party.Size = rank.YourRank;
                Discord.RP.Party.Max = rank.Total;
                for (int i = 0; i < rank.Leaderboard.Length; ++i)
                {
                    this.Ranks[i].DisplayedString = (i + 1) + ".";
                    this.Names[i].DisplayedString = rank.Leaderboard[i].Username;
                    this.Score[i].DisplayedString = rank.Leaderboard[i].Score.ToString();
                    if (Discord.You.ID == rank.Leaderboard[i].ID)
                    {
                        this.Ranks[i].Color = Color.Yellow;
                        this.Names[i].Color = Color.Yellow;
                        this.Score[i].Color = Color.Yellow;
                        ranked = true;
                    }
                    else
                    {
                        this.Ranks[i].Color = Color.White;
                        this.Names[i].Color = Color.White;
                        this.Score[i].Color = Color.White;
                    }
                }
                if (ranked)
                {
                    this.Ranks[9].Color = Color.White;
                    this.Names[9].Color = Color.White;
                    this.Score[9].Color = Color.White;
                }
                else
                {
                    this.Ranks[9].Color = Color.Yellow;
                    this.Names[9].Color = Color.Yellow;
                    this.Score[9].Color = Color.Yellow;
                }
                this.Ranks[9].DisplayedString = (rank.YourRank + 1) + ".";
                this.Names[9].DisplayedString = Discord.You.Username;
                this.Score[9].DisplayedString = File.ReadAllText("LarsOfTheStars.hsc1");
                this.UpdateTimer.Start();
                this.Visible = true;
            }
        }
        public void Draw(Display target)
        {
            if (this.Visible)
            {
                if (this.Buffer < 0)
                {
                    this.Buffer += target.FrameDelta;
                }
                else
                {
                    this.Buffer = 0;
                }
                this.Board.Position = new Vector2f(256 - (this.Board.Texture.Size.X + 1) - this.Buffer, 1);
                this.Board.Draw(target, RenderStates.Default);
                for (int i = 0; i < 10; ++i)
                {
                    this.Ranks[i].Position = new Vector2f(256 - (this.Board.Texture.Size.X + 1 - 3) - this.Buffer, 6 * i + 10);
                    this.Names[i].Position = new Vector2f(256 - (this.Board.Texture.Size.X + 1 - 9) - this.Buffer, 6 * i + 10);
                    this.Ranks[i].Draw(target, RenderStates.Default);
                    if (this.Names[i].GetGlobalBounds().Width > 40)
                    {
                        this.Names[i].DisplayedString = this.Names[i].DisplayedString.Substring(0, 9) + "...";
                    }
                    this.Names[i].Draw(target, RenderStates.Default);
                    this.Score[i].Position = new Vector2f(256 - (this.Score[i].GetGlobalBounds().Width + 3) - this.Buffer, 6 * i + 10);
                    this.Score[i].Draw(target, RenderStates.Default);
                }
            }
            if (Database.TryAgain < 12 && Discord.IsReady)
            {
                if (this.UpdateTimer.ElapsedMilliseconds > 100)
                {
                    this.Browser.DownloadStringAsync(new Uri(Database.IP + "rank?id=" + Discord.You.ID));
                    this.UpdateTimer.Reset();
                }
            }
        }
    }
}
