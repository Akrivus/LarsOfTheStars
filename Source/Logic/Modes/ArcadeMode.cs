using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Client.Screens;
using LarsOfTheStars.Source.Integration;
using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Modes
{
    public class ArcadeMode : Mode
    {
        private Sprite HealthBar = new Sprite(Textures.Load("plate", "p1", "bar_health.png"));
        private Sprite LevelBar = new Sprite(Textures.Load("plate", "p1", "bar_level.png"));
        private Sprite Fader = new Sprite(Textures.Load("fader.png"));
        private float FaderAlpha = 255;
        private Sprite BarPlate = new Sprite(Textures.Load("plate", "p1", "bars.png"));
        private Sprite FacePlate = new Sprite(Textures.Load("plate", "p1", "face_0.png"));
        private Text ScoreIndicator = new Text("", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/gameover.ttf"), 75);
        private Text HighScoreIndicator = new Text("", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/gameover.ttf"), 75);
        private Text ContinueText = new Text("Press [X] to go to menu.", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 75);
        private Text ScoreText = new Text("Final Score: 0", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 75);
        private Text GameOver = new Text("Game Over", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 75);
        private Text LevelName = new Text("Level 1", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 75);
        private Leaderboard Leaderboard = new Leaderboard();
        private WebClient Browser = new WebClient();
        public Stopwatch UpdateTimer = new Stopwatch();
        public Stopwatch BlinkTimer = new Stopwatch();
        public Stopwatch LevelTimer = new Stopwatch();
        public Stopwatch ComboTimer = new Stopwatch();
        public Stopwatch TitleTimer = new Stopwatch();
        public Stopwatch GameTimer = new Stopwatch();
        public int ComboStreak = 0;
        public int HighestScoreInRound = 0;
        public int HighScore = 0;
        public int Score = 0;
        public float PassesUntilNextLevel = 15;
        public float Passes = 0;
        public float PassAttempts = 0;
        public int Level = 1;
        public float LevelAlpha = 0;
        public override void Start()
        {
            this.FacePlate.Position = new Vector2f(1, 1);
            this.BarPlate.Position = new Vector2f(-65, 1);
            this.HealthBar.Position = new Vector2f(this.BarPlate.Position.X + 24, this.BarPlate.Position.Y + 4);
            this.LevelBar.Position = new Vector2f(this.BarPlate.Position.X + 24, this.BarPlate.Position.Y + 13);
            this.ScoreIndicator.Scale = new Vector2f(0.1F, 0.1F);
            this.ScoreIndicator.Position = new Vector2f(1, 7);
            this.HighScoreIndicator.Scale = new Vector2f(0.1F, 0.1F);
            this.HighScoreIndicator.Position = new Vector2f(1, 7);
            this.GameOver.Scale = new Vector2f(0.25F, 0.25F);
            this.GameOver.Origin = new Vector2f(this.GameOver.GetLocalBounds().Width / 2, this.GameOver.GetLocalBounds().Height / 2);
            this.GameOver.Position = new Vector2f(128, 96);
            this.ContinueText.Scale = new Vector2f(0.1F, 0.1F);
            this.ContinueText.Origin = new Vector2f(this.ContinueText.GetLocalBounds().Width / 2, this.ContinueText.GetLocalBounds().Height / 2);
            this.ContinueText.Position = new Vector2f(128, this.GameOver.GetGlobalBounds().Height + this.GameOver.GetGlobalBounds().Top + 10);
            this.ScoreText.Scale = new Vector2f(0.1F, 0.1F);
            this.LevelName.Scale = new Vector2f(0.25F, 0.25F);
            this.LevelName.Origin = new Vector2f(this.LevelName.GetLocalBounds().Width / 2, this.LevelName.GetLocalBounds().Height / 2);
            this.LevelName.Position = new Vector2f(128, 96);
            this.UpdateTimer.Start();
            this.BlinkTimer.Start();
            this.LevelTimer.Start();
            this.ComboTimer.Start();
            this.TitleTimer.Start();
            this.GameTimer.Start();
            if (File.Exists("LarsOfTheStars.hsc1"))
            {
                this.HighScore = int.Parse(File.ReadAllText("LarsOfTheStars.hsc1"));
            }
            else
            {
                this.HighScore = 0;
            }
            this.Score = 0;
            this.HighestScoreInRound = 0;
            this.PassesUntilNextLevel = 20;
            this.Passes = 0;
            this.PassAttempts = 0;
            this.Level = 0;
            if (this.Level < 400)
            {
                for (int i = 0; i < this.Level; ++i)
                {
                    if (this.Level > 1)
                    {
                        this.PassesUntilNextLevel *= 1.25F;
                    }
                }
            }
            else
            {
                this.PassesUntilNextLevel = Single.PositiveInfinity;
            }
            this.Browser.DownloadStringCompleted += FixScore;
            Game.ClientEntities.Clear();
            Game.ServerEntities.Clear();
            Game.ServerPlayer1 = new ModelPlayer(1);
            Game.ClientPlayer1 = new RenderPlayer(Game.ServerPlayer1);
            Discord.RP.Details = "Arcade Mode | Level 1";
            Discord.RP.State = "Score: 0";
        }
        private void FixScore(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                this.HighScore = int.Parse(e.Result);
                this.UpdateTimer.Start();
            }
            else
            {
                if (File.Exists("LarsOfTheStars.hsc1"))
                {
                    this.HighScore = int.Parse(File.ReadAllText("LarsOfTheStars.hsc1"));
                }
            }
        }
        public override void PreRender(Display target)
        {
            
        }
        public override void Update(Display target)
        {
            if (Game.ServerPlayer1.Buff == ModelPlayer.PowerUp.SLOW_MOTION && target.SpeedFactor != 0.5)
            {
                target.SpeedFactor = 0.5F;
            }
            else if (target.SpeedFactor == 0.5F)
            {
                target.SpeedFactor = 1.0F;
            }

            // Game logic for levels go here.
            if (Game.ServerPlayer1.IsNotDead() && this.GameTimer.ElapsedMilliseconds > 20 && this.TitleTimer.ElapsedMilliseconds > 2000)
            {
                float delay = -(4.2F * (float)(10 * Math.Ceiling((this.Level + 1) / 10.0)) * this.Level) + (10 * Game.Configs.Difficulty) * (float)(Math.Sin(this.Level * this.Level)) + 1000;
                if (this.LevelTimer.ElapsedMilliseconds > delay)
                {
                    if (this.Level % 2 == 0)
                    {
                        Game.AddEntity(new RenderDiver(new ModelDiver()));
                        this.PassAttempts += 1;
                    }
                    else if (this.Level % 3 == 0)
                    {
                        if (Game.RNG.Next(3) > 0)
                        {
                            Game.AddEntity(new RenderSinker(new ModelSinker()));
                            this.PassAttempts += 1;
                        }
                        else
                        {
                            Game.AddEntity(new RenderDiver(new ModelDiver()));
                            this.PassAttempts += 1;
                        }
                    }
                    else if (this.Level == 5 || this.Level == 7)
                    {
                        // These are prime intro rounds.
                    }
                    else
                    {
                        Game.AddEntity(new RenderSinker(new ModelSinker()));
                        this.PassAttempts += 1;
                    }
                    if (this.Level % 5 == 0 || this.Level % 11 == 0)
                    {
                        Game.AddEntity(new RenderSpinner(new ModelSpinner()));
                        this.PassAttempts += 1;
                    }
                    if (this.Level % 7 == 0 || this.Level % 13 == 0)
                    {
                        Game.AddEntity(new RenderArcher(new ModelArcher()));
                        this.PassAttempts += 1;
                    }
                    if (this.Level > 3)
                    {
                        if (Enumerable.Range(1, this.Level).Where(x => this.Level % x == 0).SequenceEqual(new[] { 1, this.Level }))
                        {
                            char[] chars = new char[] { '3', '7' };
                            if (chars.Contains<Char>(this.Level.ToString().Last<Char>()))
                            {
                                Game.AddEntity(new RenderArcher(new ModelArcher()));
                                this.PassAttempts += 1;
                            }
                            else
                            {
                                Game.AddEntity(new RenderSpinner(new ModelSpinner()));
                                this.PassAttempts += 1;
                            }
                        }
                    }
                    this.LevelTimer.Restart();
                }
                this.GameTimer.Restart();
            }
            Game.ClientPlayer1.Update(target);
            if (Game.ServerPlayer1.IsDead())
            {
                if (Joystick.IsButtonPressed(0, 2) || Keyboard.IsKeyPressed(Keyboard.Key.X))
                {
                    Game.IsOnStartScreen = true;
                }
            }
            if (this.Passes >= this.PassesUntilNextLevel)
            {
                this.LevelUp();
            }

            // Render stuff goes here for organization.
            this.HealthBar.TextureRect = new IntRect(0, 0, (int)(((10.0F - Game.ServerPlayer1.Damage) / 10.0F) * 37F), 8);
            this.LevelBar.TextureRect = new IntRect(0, 0, (int)((this.Passes / this.PassesUntilNextLevel) * 30F), 7);
            if (this.BarPlate.Position.X < 0)
            {
                this.BarPlate.Position = new Vector2f(this.BarPlate.Position.X + target.FrameDelta, 1);
                this.HealthBar.Position = new Vector2f(this.BarPlate.Position.X + 24, this.BarPlate.Position.Y + 4);
                this.LevelBar.Position = new Vector2f(this.BarPlate.Position.X + 24, this.BarPlate.Position.Y + 13);
            }
            else
            {
                this.BarPlate.Position = new Vector2f(1, 1);
                this.HealthBar.Position = new Vector2f(this.BarPlate.Position.X + 24, this.BarPlate.Position.Y + 4);
                this.LevelBar.Position = new Vector2f(this.BarPlate.Position.X + 24, this.BarPlate.Position.Y + 13);
                this.ScoreIndicator.DisplayedString = "Score: " + this.Score;
                if (this.Score >= this.HighScore)
                {
                    this.ScoreIndicator.Color = Color.Yellow;
                }
                else if (this.Score < this.HighestScoreInRound)
                {
                    this.ScoreIndicator.Color = Color.Red;
                }
                else
                {
                    this.ScoreIndicator.Color = Color.White;
                }
                if (this.ScoreIndicator.Position.Y < 23)
                {
                    this.ScoreIndicator.Position = new Vector2f(1, this.ScoreIndicator.Position.Y + target.FrameDelta);
                }
                else
                {
                    this.ScoreIndicator.Position = new Vector2f(1, 23);
                }
                this.HighScoreIndicator.DisplayedString = "High: " + this.HighScore;
                if (this.HighScoreIndicator.Position.Y < 29)
                {
                    this.HighScoreIndicator.Position = new Vector2f(1, this.HighScoreIndicator.Position.Y + target.FrameDelta);
                }
                else
                {
                    this.HighScoreIndicator.Position = new Vector2f(1, 29);
                }
            }
            if (this.BlinkTimer.ElapsedMilliseconds > 4000)
            {
                long ms = this.BlinkTimer.ElapsedMilliseconds;
                if (ms > 4050)
                {
                    if (ms > 4060)
                    {
                        if (ms > 4260)
                        {
                            this.BlinkTimer.Restart();
                        }
                        else
                        {
                            this.FacePlate.Texture = Textures.Load("plate", "p1", "face_3.png");
                        }
                    }
                    else
                    {
                        this.FacePlate.Texture = Textures.Load("plate", "p1", "face_2.png");
                    }
                }
                else
                {
                    this.FacePlate.Texture = Textures.Load("plate", "p1", "face_1.png");
                }
            }
            else
            {
                long ms = this.BlinkTimer.ElapsedMilliseconds;
                if (ms > 50)
                {
                    if (ms > 100)
                    {
                        this.FacePlate.Texture = Textures.Load("plate", "p1", "face_0.png");
                    }
                    else
                    {
                        this.FacePlate.Texture = Textures.Load("plate", "p1", "face_1.png");
                    }
                }
                else
                {
                    this.FacePlate.Texture = Textures.Load("plate", "p1", "face_2.png");
                }
            }
            Sounds.SetPitch(target.SpeedFactor);
            Discord.RP.Assets.SmallImageText = "Playing Arcade Mode";
            Discord.RP.Assets.SmallImageKey = "p1";
        }
        public override void PostRender(Display target)
        {
            if (Game.ServerPlayer1.IsNotDead())
            {
                this.ScoreIndicator.Draw(target, RenderStates.Default);
                this.HighScoreIndicator.Draw(target, RenderStates.Default);
                this.BarPlate.Draw(target, RenderStates.Default);
                this.HealthBar.Draw(target, RenderStates.Default);
                this.LevelBar.Draw(target, RenderStates.Default);
                this.FacePlate.Draw(target, RenderStates.Default);
                if (this.FaderAlpha > 0)
                {
                    this.Fader.Color = new Color(255, 255, 255, (byte)(this.FaderAlpha));
                    this.FaderAlpha -= target.FrameDelta * 4;
                    this.Fader.Draw(target, RenderStates.Default);
                }
                if (this.TitleTimer.ElapsedMilliseconds < 3000)
                {
                    this.LevelName.DisplayedString = "Level " + this.Level;
                    this.LevelName.Origin = new Vector2f(this.LevelName.GetLocalBounds().Width / 2, this.LevelName.GetLocalBounds().Height / 2);
                    this.LevelName.Draw(target, RenderStates.Default);
                    this.LevelAlpha = 255;
                }
                else if (this.LevelAlpha > 0 && Game.Configs.FadeOut)
                {
                    this.LevelName.Color = new Color(255, 255, 255, (byte)(this.LevelAlpha));
                    this.LevelAlpha -= target.FrameDelta * 4;
                    this.LevelName.Draw(target, RenderStates.Default);
                }
                this.Leaderboard.Draw(target);
            }
            else
            {
                if (this.FaderAlpha < 255 && Game.Configs.FadeOut)
                {
                    this.Fader.Color = new Color(0, 0, 0, (byte)(this.FaderAlpha));
                    this.FaderAlpha += target.FrameDelta * 4;
                    this.Fader.Draw(target, RenderStates.Default);
                }
                else
                {
                    this.Fader.Draw(target, RenderStates.Default);
                    this.ContinueText.Draw(target, RenderStates.Default);
                    this.GameOver.Draw(target, RenderStates.Default);
                    this.ScoreText.Origin = new Vector2f(this.ScoreText.GetLocalBounds().Width / 2, this.ScoreText.GetLocalBounds().Height / 2);
                    this.ScoreText.Position = new Vector2f(128, this.GameOver.GetGlobalBounds().Top + this.ScoreText.GetGlobalBounds().Height - 20);
                    this.ScoreText.Draw(target, RenderStates.Default);
                }
            }
        }
        public override void Interact(int rank, int type)
        {
            string[] doubles = new string[] { "Nice shot!", "Double shot!", "Double trouble!", "Dueces!"};
            string[] triples = new string[] { "Neato burrito!", "Oh baby a triple!", "Aces!" };
            string[] breaker = new string[] { "Combo breaker!", "Ouch!" };
            string[] combo = new string[] { "Bingo bongo!", "Nova thrusters!" };
            string[] boost = new string[] { "Feels good!", "Boost!" };
            if (type > 0)
            {
                this.ComboStreak += 1;
                this.Passes += 1;
            }
            else
            {
                if (this.ComboStreak > 1 && this.ComboTimer.ElapsedMilliseconds > 500)
                {
                    Sounds.PlayRandom("p1", "end");
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, breaker[Game.RNG.Next(breaker.Length)], Color.Red)));
                    this.ComboTimer.Restart();
                }
                this.ComboStreak = 0;
                this.Passes -= 2;
                if (this.Passes < 0)
                {
                    this.Passes = 0;
                }
            }
            if (this.ComboStreak % 3 == 0 && this.ComboStreak > 0)
            {
                if (this.ComboTimer.ElapsedMilliseconds > 1250)
                {
                    Sounds.PlayRandom("p1", "buff");
                }
                Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, combo[Game.RNG.Next(combo.Length)], Color.Magenta)));
                for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                {
                    Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Magenta)));
                }
                this.Passes += this.ComboStreak / 3;
                this.Score += this.ComboStreak / 3;
            }
            else if (this.ComboStreak > 2)
            {
                Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, triples[Game.RNG.Next(triples.Length)], Color.Magenta)));
            }
            else if (this.ComboStreak > 1)
            {
                Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, doubles[Game.RNG.Next(doubles.Length)], Color.Magenta)));
            }
            this.Score += type;
            if (Game.ServerPlayer1.Buff == ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                this.Score += type;
            }
            if (this.Score >= this.HighestScoreInRound && this.HighestScoreInRound % 50 > this.Score % 50)
            {
                Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, boost[Game.RNG.Next(boost.Length)], Color.Green)));
                for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                {
                    Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Green)));
                }
                Game.ServerPlayer1.Damage = 0;
                if (this.Score > 100)
                {
                    if (this.HighestScoreInRound % 100 > this.Score % 100)
                    {
                        Game.ServerPlayer1.BuffTimer.Restart();
                        if (this.HighestScoreInRound % 500 > this.Score % 500)
                        {
                            Game.ServerPlayer1.Buff = ModelPlayer.PowerUp.SLOW_MOTION;
                        }
                        else
                        {
                            Game.ServerPlayer1.Buff = ModelPlayer.PowerUp.HYPERLASER;
                        }
                    }
                    else if (this.HighestScoreInRound % 50 > this.Score % 50)
                    {
                        Game.ServerPlayer1.Buff = ModelPlayer.PowerUp.MULTILASER;
                    }
                }
                Sounds.PlayRandom("p1", "buff");
            }
            if (this.Score > this.HighestScoreInRound)
            {
                this.HighestScoreInRound = this.Score;
            }
            Discord.RP.State = "Score: " + this.Score;
            Database.SetScore(this.Score);
            if (this.Score > this.HighScore)
            {
                try { File.WriteAllText("LarsOfTheStars.hsc1", this.Score.ToString()); } catch { }
                this.HighScore = this.Score;
            }
        }
        public void LevelUp()
        {
            this.Passes = 0;
            this.PassAttempts = 0;
            this.PassesUntilNextLevel *= 1.25F;
            this.Level += 1;
            this.TitleTimer.Restart();
            if (Game.ServerPlayer1.Damage == 0)
            {
                for (int i = 0; i < Game.ServerEntities.Count; ++i)
                {
                    Model model = Game.ServerEntities[i];
                    if (model.CanBeSwept())
                    {
                        this.Score += 1;
                        model.OnDeath();
                        model.Kill();
                    }
                }
            }
            else if (this.Score > 0)
            {
                Game.ServerPlayer1.Damage = 0;
            }
            Discord.RP.Details = "Arcade Mode | Level " + this.Level;
            Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, "Level up!", Color.Yellow)));
            for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
            {
                Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Yellow)));
            }
            if (Game.RNG.Next(2) == 0)
            {
                Sounds.PlayRandom("p1", "level");
            }
            else
            {
                Sounds.PlayRandom("emerald");
            }
            if (Database.TryAgain < 12 && Discord.IsReady)
            {
                if (this.UpdateTimer.ElapsedMilliseconds > 100)
                {
                    this.Browser.DownloadStringAsync(new Uri(Database.IP + "high?id=" + Discord.You.ID));
                    this.UpdateTimer.Reset();
                }
            }
        }
    }
}
