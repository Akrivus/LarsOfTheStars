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
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Modes
{
    public class TeamMode : Mode
    {
        private Sprite HealthBar1 = new Sprite(Textures.Load("plate", "p1", "bar_health.png"));
        private Sprite LevelBar1 = new Sprite(Textures.Load("plate", "p1", "bar_level.png"));
        private Sprite HealthBar2 = new Sprite(Textures.Load("plate", "p2", "bar_health.png"));
        private Sprite LevelBar2 = new Sprite(Textures.Load("plate", "p2", "bar_level.png"));
        private Sprite Fader = new Sprite(Textures.Load("fader.png"));
        private float FaderAlpha = 255;
        private Sprite BarPlate1 = new Sprite(Textures.Load("plate", "p1", "bars.png"));
        private Sprite FacePlate1 = new Sprite(Textures.Load("plate", "p1", "face_0.png"));
        private Sprite BarPlate2 = new Sprite(Textures.Load("plate", "p2", "bars.png"));
        private Sprite FacePlate2 = new Sprite(Textures.Load("plate", "p2", "face_0.png"));
        private Text Score1Indicator = new Text("", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/gameover.ttf"), 75);
        private Text Score2Indicator = new Text("", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/gameover.ttf"), 75);
        private Text ContinueText = new Text("Press [Q] to go to menu.", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 75);
        private Text GameOver = new Text("Game Over", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 75);
        private Text LevelName = new Text("Level 1", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 75);
        public Stopwatch Blink1Timer = new Stopwatch();
        public Stopwatch Blink2Timer = new Stopwatch();
        public Stopwatch LevelTimer = new Stopwatch();
        public Stopwatch Combo1Timer = new Stopwatch();
        public Stopwatch Combo2Timer = new Stopwatch();
        public Stopwatch TitleTimer = new Stopwatch();
        public Stopwatch GameTimer = new Stopwatch();
        public int ComboStreak1 = 0;
        public int HighestScore1InRound = 0;
        public int Score1 = 0;
        public int ComboStreak2 = 0;
        public int HighestScore2InRound = 0;
        public int Score2 = 0;
        public float PassesUntilNextLevel = 15;
        public float Passes = 0;
        public float PassAttempts = 0;
        public int Level = 1;
        public float LevelAlpha = 0;
        public override void Start()
        {
            this.FacePlate1.Position = new Vector2f(1, 1);
            this.FacePlate2.Position = new Vector2f(191, 1);
            this.BarPlate1.Position = new Vector2f(-65, 1);
            this.BarPlate2.Position = new Vector2f(321, 1);
            this.HealthBar1.Position = new Vector2f(256 - this.BarPlate1.Position.X + 24, this.BarPlate1.Position.Y + 4);
            this.LevelBar1.Position = new Vector2f(256 - this.BarPlate1.Position.X + 24, this.BarPlate1.Position.Y + 13);
            this.HealthBar2.Position = new Vector2f(256 - this.BarPlate2.Position.X - 24, this.BarPlate2.Position.Y + 4);
            this.LevelBar2.Position = new Vector2f(256 - this.BarPlate2.Position.X - 24, this.BarPlate2.Position.Y + 13);
            this.Score1Indicator.Scale = new Vector2f(0.1F, 0.1F);
            this.Score2Indicator.Scale = new Vector2f(0.1F, 0.1F);
            this.GameOver.Scale = new Vector2f(0.25F, 0.25F);
            this.GameOver.Origin = new Vector2f(this.GameOver.GetLocalBounds().Width / 2, this.GameOver.GetLocalBounds().Height / 2);
            this.GameOver.Position = new Vector2f(128, 96);
            this.ContinueText.Scale = new Vector2f(0.1F, 0.1F);
            this.ContinueText.Origin = new Vector2f(this.ContinueText.GetLocalBounds().Width / 2, this.ContinueText.GetLocalBounds().Height / 2);
            this.ContinueText.Position = new Vector2f(128, this.GameOver.GetGlobalBounds().Height + this.GameOver.GetGlobalBounds().Top + 10);
            this.LevelName.Scale = new Vector2f(0.25F, 0.25F);
            this.LevelName.Origin = new Vector2f(this.LevelName.GetLocalBounds().Width / 2, this.LevelName.GetLocalBounds().Height / 2);
            this.LevelName.Position = new Vector2f(128, 96);
            this.Blink1Timer.Start();
            this.Blink2Timer.Start();
            this.LevelTimer.Start();
            this.Combo1Timer.Start();
            this.Combo2Timer.Start();
            this.TitleTimer.Start();
            this.GameTimer.Start();
            this.Score1 = 0;
            this.HighestScore1InRound = 0;
            this.Score2 = 0;
            this.HighestScore2InRound = 0;
            this.PassesUntilNextLevel = 20;
            this.Passes = 0;
            this.PassAttempts = 0;
            this.Level = 1;
            Game.ClientEntities.Clear();
            Game.ServerEntities.Clear();
            Game.ServerPlayer1 = new ModelPlayer(1);
            Game.ClientPlayer1 = new RenderPlayer(Game.ServerPlayer1);
            Game.ServerPlayer2 = new ModelPlayer(2);
            Game.ClientPlayer2 = new RenderPlayer(Game.ServerPlayer2);
            Discord.RP.Assets.SmallImageText = "Playing Team Mode";
            Discord.RP.Assets.SmallImageKey = "p2";
            Discord.RP.Details = "Team Mode | Level 1";
            Discord.RP.State = "Score: 0 | 0";
        }
        public override void PreRender(Display target)
        {

        }
        public override void Update(Display target)
        {
            // Game logic for levels go here.
            if (this.GameTimer.ElapsedMilliseconds > 20 && this.TitleTimer.ElapsedMilliseconds > 2000)
            {
                float delay = (((float)(Math.Ceiling((this.Level + 1) / 10.0) * 10) - ((this.Level + this.PassAttempts) / this.PassesUntilNextLevel)) * (60 * Game.Configs.Difficulty)) * ((1.0F - this.Passes / this.PassesUntilNextLevel) + 1);
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
            Game.ClientPlayer2.Update(target);
            if (this.Passes >= this.PassesUntilNextLevel)
            {
                this.LevelUp();
            }

            // Render stuff goes here for organization.
            this.HealthBar1.TextureRect = new IntRect(0, 0, (int)(((10.0F - Game.ServerPlayer1.Damage) / 10.0F) * 37F), 8);
            this.LevelBar1.TextureRect = new IntRect(0, 0, (int)((this.Passes / this.PassesUntilNextLevel) * 30F), 7);
            if (this.BarPlate1.Position.X < 0)
            {
                this.BarPlate1.Position = new Vector2f(this.BarPlate1.Position.X + target.FrameDelta, 1);
                this.HealthBar1.Position = new Vector2f(this.BarPlate1.Position.X + 24, this.BarPlate1.Position.Y + 4);
                this.LevelBar1.Position = new Vector2f(this.BarPlate1.Position.X + 24, this.BarPlate1.Position.Y + 13);
            }
            else
            {
                this.BarPlate1.Position = new Vector2f(1, 1);
                this.HealthBar1.Position = new Vector2f(this.BarPlate1.Position.X + 24, this.BarPlate1.Position.Y + 4);
                this.LevelBar1.Position = new Vector2f(this.BarPlate1.Position.X + 24, this.BarPlate1.Position.Y + 13);
                this.Score1Indicator.DisplayedString = "Score: " + this.Score1;
                if (this.Score1 < this.HighestScore1InRound)
                {
                    this.Score1Indicator.Color = Color.Red;
                }
                else if (this.Score1 < this.Score2)
                {
                    this.Score1Indicator.Color = Color.Magenta;
                }
                else
                {
                    this.Score1Indicator.Color = Color.White;
                }
                if (this.Score1Indicator.Position.Y < 23)
                {
                    this.Score1Indicator.Position = new Vector2f(1, this.Score1Indicator.Position.Y + target.FrameDelta);
                }
                else
                {
                    this.Score1Indicator.Position = new Vector2f(1, 23);
                }
            }
            if (this.Blink1Timer.ElapsedMilliseconds > 4000)
            {
                long ms = this.Blink1Timer.ElapsedMilliseconds;
                if (ms > 4050)
                {
                    if (ms > 4060)
                    {
                        if (ms > 4260)
                        {
                            this.Blink1Timer.Restart();
                        }
                        else
                        {
                            this.FacePlate1.Texture = Textures.Load("plate", "p1", "face_3.png");
                        }
                    }
                    else
                    {
                        this.FacePlate1.Texture = Textures.Load("plate", "p1", "face_2.png");
                    }
                }
                else
                {
                    this.FacePlate1.Texture = Textures.Load("plate", "p1", "face_1.png");
                }
            }
            else
            {
                long ms = this.Blink1Timer.ElapsedMilliseconds;
                if (ms > 50)
                {
                    if (ms > 100)
                    {
                        this.FacePlate1.Texture = Textures.Load("plate", "p1", "face_0.png");
                    }
                    else
                    {
                        this.FacePlate1.Texture = Textures.Load("plate", "p1", "face_1.png");
                    }
                }
                else
                {
                    this.FacePlate1.Texture = Textures.Load("plate", "p1", "face_2.png");
                }
            }
            this.HealthBar2.TextureRect = new IntRect(0, 0, (int)(((10.0F - Game.ServerPlayer2.Damage) / 10.0F) * 37F), 8);
            this.LevelBar2.TextureRect = new IntRect(0, 0, (int)((this.Passes / this.PassesUntilNextLevel) * 30F), 7);
            if (this.BarPlate2.Position.X > 191)
            {
                this.BarPlate2.Position = new Vector2f(this.BarPlate2.Position.X - target.FrameDelta, 1);
                this.HealthBar2.Position = new Vector2f(this.BarPlate2.Position.X + 3, this.BarPlate2.Position.Y + 4);
                this.LevelBar2.Position = new Vector2f(this.BarPlate2.Position.X + 10, this.BarPlate2.Position.Y + 13);
            }
            else
            {
                this.BarPlate2.Position = new Vector2f(191, 1);
                this.HealthBar2.Position = new Vector2f(this.BarPlate2.Position.X + 3, this.BarPlate2.Position.Y + 4);
                this.LevelBar2.Position = new Vector2f(this.BarPlate2.Position.X + 10, this.BarPlate2.Position.Y + 13);
                this.Score2Indicator.DisplayedString = "Score: " + this.Score2;
                if (this.Score2 < this.HighestScore2InRound)
                {
                    this.Score2Indicator.Color = Color.Red;
                }
                else if (this.Score2 < this.Score1)
                {
                    this.Score2Indicator.Color = Color.Magenta;
                }
                else
                {
                    this.Score2Indicator.Color = Color.White;
                }
                if (this.Score2Indicator.Position.Y < 23)
                {
                    this.Score2Indicator.Position = new Vector2f(256 - (1 + this.Score2Indicator.GetGlobalBounds().Width), this.Score2Indicator.Position.Y + target.FrameDelta);
                }
                else
                {
                    this.Score2Indicator.Position = new Vector2f(256 - (1 + this.Score2Indicator.GetGlobalBounds().Width), 23);
                }
            }
            if (this.Blink2Timer.ElapsedMilliseconds > 4000)
            {
                long ms = this.Blink2Timer.ElapsedMilliseconds;
                if (ms > 4050)
                {
                    if (ms > 4060)
                    {
                        if (ms > 4260)
                        {
                            this.Blink2Timer.Restart();
                        }
                        else
                        {
                            this.FacePlate2.Texture = Textures.Load("plate", "p2", "face_3.png");
                        }
                    }
                    else
                    {
                        this.FacePlate2.Texture = Textures.Load("plate", "p2", "face_2.png");
                    }
                }
                else
                {
                    this.FacePlate2.Texture = Textures.Load("plate", "p2", "face_1.png");
                }
            }
            else
            {
                long ms = this.Blink2Timer.ElapsedMilliseconds;
                if (ms > 50)
                {
                    if (ms > 100)
                    {
                        this.FacePlate2.Texture = Textures.Load("plate", "p2", "face_0.png");
                    }
                    else
                    {
                        this.FacePlate2.Texture = Textures.Load("plate", "p2", "face_1.png");
                    }
                }
                else
                {
                    this.FacePlate2.Texture = Textures.Load("plate", "p2", "face_2.png");
                }
            }
            Sounds.SetPitch(target.SpeedFactor);
        }
        public override void PostRender(Display target)
        {
            this.Score1Indicator.Draw(target, RenderStates.Default);
            this.BarPlate1.Draw(target, RenderStates.Default);
            this.HealthBar1.Draw(target, RenderStates.Default);
            this.LevelBar1.Draw(target, RenderStates.Default);
            this.FacePlate1.Draw(target, RenderStates.Default);
            this.Score2Indicator.Draw(target, RenderStates.Default);
            this.BarPlate2.Draw(target, RenderStates.Default);
            this.HealthBar2.Draw(target, RenderStates.Default);
            this.LevelBar2.Draw(target, RenderStates.Default);
            this.FacePlate2.Draw(target, RenderStates.Default);
            if (this.FaderAlpha > 0)
            {
                this.Fader.Color = new Color(255, 255, 255, (byte)(this.FaderAlpha));
                this.FaderAlpha -= target.FrameDelta * 4;
                this.Fader.Draw(target, RenderStates.Default);
            }
            if (this.TitleTimer.ElapsedMilliseconds < 2000)
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
        }
        public override void Interact(int rank, int type)
        {
            string[] doubles = new string[] { "Nice shot!", "Double shot!", "Double trouble!", "Dueces!" };
            string[] triples = new string[] { "Neato burrito!", "Oh baby a triple!", "Aces!" };
            string[] breaker = new string[] { "Combo breaker!", "Ouch!" };
            string[] combo = new string[] { "Bingo bongo!", "Nova thrusters!" };
            string[] boost = new string[] { "Feels good!", "Boost!" };
            if (rank == 1)
            {
                if (type > 0)
                {
                    this.ComboStreak1 += 1;
                    this.Passes += 1;
                }
                else
                {
                    if (this.ComboStreak1 > 1 && this.Combo1Timer.ElapsedMilliseconds > 250)
                    {
                        Sounds.PlayRandom("p1", "end");
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, breaker[Game.RNG.Next(breaker.Length)], Color.Red)));
                        this.Combo1Timer.Restart();
                    }
                    this.ComboStreak1 = 0;
                    this.Passes -= 2;
                    if (this.Passes < 0)
                    {
                        this.Passes = 0;
                    }
                }
                if (this.ComboStreak1 % 3 == 0 && this.ComboStreak1 > 0)
                {
                    if (this.Combo1Timer.ElapsedMilliseconds > 500)
                    {
                        Sounds.PlayRandom("p1", "buff");
                    }
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, combo[Game.RNG.Next(combo.Length)], Color.Magenta)));
                    for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Magenta)));
                    }
                    this.Passes += this.ComboStreak1 / 3;
                    this.Score1 += this.ComboStreak1 / 3;
                }
                else if (this.ComboStreak1 > 2)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, triples[Game.RNG.Next(triples.Length)], Color.Magenta)));
                }
                else if (this.ComboStreak1 > 1)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, doubles[Game.RNG.Next(doubles.Length)], Color.Magenta)));
                }
                this.Score1 += type;
                if (this.Score1 >= this.HighestScore1InRound && this.HighestScore1InRound % 50 > this.Score1 % 50)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, boost[Game.RNG.Next(boost.Length)], Color.Green)));
                    for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Green)));
                    }
                    Game.ServerPlayer1.Damage = 0;
                    if (this.Score1 > 100)
                    {
                        if (this.HighestScore1InRound % 100 > this.Score1 % 100)
                        {
                            Game.ServerPlayer1.BuffTimer.Restart();
                            Game.ServerPlayer1.Buff = ModelPlayer.PowerUp.HYPERLASER;
                        }
                    }
                    Sounds.PlayRandom("p1", "buff");
                }
                if (this.Score1 > this.HighestScore1InRound)
                {
                    this.HighestScore1InRound = this.Score1;
                }
            }
            else
            {
                if (type > 0)
                {
                    this.ComboStreak2 += 1;
                    this.Passes += 1;
                }
                else
                {
                    if (this.ComboStreak2 > 1 && this.Combo2Timer.ElapsedMilliseconds > 250)
                    {
                        Sounds.PlayRandom("p2", "end");
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, breaker[Game.RNG.Next(breaker.Length)], Color.Red)));
                        this.Combo2Timer.Restart();
                    }
                    this.ComboStreak2 = 0;
                    this.Passes -= 2;
                    if (this.Passes < 0)
                    {
                        this.Passes = 0;
                    }
                }
                if (this.ComboStreak2 % 3 == 0 && this.ComboStreak2 > 0)
                {
                    if (this.Combo2Timer.ElapsedMilliseconds > 500)
                    {
                        Sounds.PlayRandom("p1", "buff");
                    }
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, combo[Game.RNG.Next(combo.Length)], Color.Magenta)));
                    for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer2.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer2.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Magenta)));
                    }
                    this.Passes += this.ComboStreak2 / 3;
                    this.Score2 += this.ComboStreak2 / 3;
                }
                else if (this.ComboStreak2 > 2)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, triples[Game.RNG.Next(triples.Length)], Color.Magenta)));
                }
                else if (this.ComboStreak2 > 1)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, doubles[Game.RNG.Next(doubles.Length)], Color.Magenta)));
                }
                this.Score2 += type;
                if (this.Score2 >= this.HighestScore2InRound && this.HighestScore2InRound % 50 > this.Score2 % 50)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, boost[Game.RNG.Next(boost.Length)], Color.Green)));
                    for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer2.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer2.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Green)));
                    }
                    Game.ServerPlayer2.Damage = 0;
                    if (this.Score2 > 100)
                    {
                        if (this.HighestScore1InRound % 100 > this.Score1 % 100)
                        {
                            Game.ServerPlayer2.BuffTimer.Restart();
                            Game.ServerPlayer2.Buff = ModelPlayer.PowerUp.HYPERLASER;
                        }
                    }
                    Sounds.PlayRandom("p2", "buff");
                }
                if (this.Score2 > this.HighestScore1InRound)
                {
                    this.HighestScore1InRound = this.Score2;
                }
            }
            Discord.RP.State = "Score: " + this.Score1 + " | " + this.Score2;
        }
        public void LevelUp()
        {
            this.Passes = 0;
            this.PassAttempts = 0;
            this.PassesUntilNextLevel *= 1.5F;
            this.Level += 1;
            this.TitleTimer.Restart();
            Game.ServerPlayer1.Damage = 0;
            Game.ServerPlayer2.Damage = 0;
            Discord.RP.Details = "Arcade Mode | Level " + this.Level;
            if (Game.RNG.Next(2) == 0)
            {
                Sounds.PlayRandom("p1", "level");
            }
            else if (Game.RNG.Next(2) == 0)
            {
                Sounds.PlayRandom("p2", "level");
            }
            else
            {
                Sounds.PlayRandom("emerald");
            }
        }
        public override bool IsMultiUser()
        {
            return true;
        }
    }
}
