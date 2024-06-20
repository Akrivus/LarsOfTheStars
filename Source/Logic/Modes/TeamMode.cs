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
        // Player 1 GUIs.
        private Sprite FacePlate1 = new Sprite(Textures.Load("plate", "p1", "face_0.png"));
        private Sprite HealthBar1 = new Sprite(Textures.Load("plate", "p1", "bar_health.png"));
        private Sprite BarPlate1 = new Sprite(Textures.Load("plate", "p1", "bars_1.png"));
        private Text Score1HudText = new Text("", Fonts.Load("gameover.ttf"), 75);

        // Player 2 GUIs.
        private Sprite FacePlate2 = new Sprite(Textures.Load("plate", "p2", "face_0.png"));
        private Sprite HealthBar2 = new Sprite(Textures.Load("plate", "p2", "bar_health.png"));
        private Sprite BarPlate2 = new Sprite(Textures.Load("plate", "p2", "bars.png"));
        private Text Score2HudText = new Text("", Fonts.Load("gameover.ttf"), 75);

        // Boss GUIs.
        private Sprite BossPlate = new Sprite(Textures.Load("plate", "boss", "bars.png"));
        private Sprite BossBar = new Sprite(Textures.Load("plate", "boss", "bar_health.png"));
        
        // Fader.
        private Sprite Fader = new Sprite(Textures.Load("fader.png"));
        private float FaderAlpha = 255;

        // Game over GUIs.
        private Text GameOverText = new Text("Game Over", Fonts.Load("start2p.ttf"), 75);
        private Text FinalScoreText = new Text("Player 1 Wins!", Fonts.Load("start2p.ttf"), 75);
        private Text ContinueText = new Text("Press [X] to go to menu.", Fonts.Load("start2p.ttf"), 75);

        // GUI timers.
        public Stopwatch Blink1Timer = new Stopwatch();
        public Stopwatch Blink2Timer = new Stopwatch();

        // Game timers.
        public Stopwatch Combo1Timer = new Stopwatch();
        public Stopwatch Combo2Timer = new Stopwatch();
        public Stopwatch UpdateTimer = new Stopwatch();
        public Stopwatch LevelTimer = new Stopwatch();

        // Injected models.
        public ModelBoss Boss = new ModelBoss();

        // Game values.
        public int ComboStreak1 = 0;
        public int HighestScore1InRound = 0;
        public int Score1 = 0;

        // Player 2.
        public int ComboStreak2 = 0;
        public int HighestScore2InRound = 0;
        public int Score2 = 0;

        // Global game values.
        public float PassesUntilNextLevel = 15;
        public float Passes = 0;
        public float PassAttempts = 0;

        public override void Start()
        {
            // Position HUD.
            FacePlate1.Position = new Vector2f(1, 1);
            BarPlate1.Position = new Vector2f(-65, 1);
            HealthBar1.Position = new Vector2f(-41, 5);
            Score1HudText.Scale = new Vector2f(0.1F, 0.1F);
            FacePlate2.Position = new Vector2f(191, 1);
            BarPlate2.Position = new Vector2f(321, 1);
            HealthBar2.Position = new Vector2f(256 - BarPlate2.Position.X - 24, BarPlate2.Position.Y + 12);
            Score2HudText.Scale = new Vector2f(0.1F, 0.1F);

            // Position boss HUD.
            BossPlate.Position = new Vector2f(96, -17);
            BossBar.Position = new Vector2f(101, 25);

            // Position game over screen.
            GameOverText.Position = new Vector2f(128, 96); GameOverText.Origin = new Vector2f(GameOverText.GetLocalBounds().Width / 2, GameOverText.GetLocalBounds().Height / 2);
            GameOverText.Scale = new Vector2f(0.25F, 0.25F);
            ContinueText.Position = new Vector2f(128, GameOverText.GetGlobalBounds().Height + GameOverText.GetGlobalBounds().Top + 10); ContinueText.Origin = new Vector2f(ContinueText.GetLocalBounds().Width / 2, ContinueText.GetLocalBounds().Height / 2);
            ContinueText.Scale = new Vector2f(0.1F, 0.1F);
            FinalScoreText.Scale = new Vector2f(0.1F, 0.1F);

            // Start timers.
            Blink1Timer.Start();
            Blink2Timer.Start();
            Combo1Timer.Start();
            Combo2Timer.Start();
            UpdateTimer.Start();
            LevelTimer.Start();

            // Set up scores.
            Score1 = 0;
            HighestScore1InRound = 0;
            Score2 = 0;
            HighestScore2InRound = 0;

            // Set up levels.
            PassesUntilNextLevel = 40;
            Passes = 0;
            PassAttempts = 0;
            Level = 1;

            // Reset entities.
            Game.ClientEntities.Clear();
            Game.ServerEntities.Clear();

            // Spawn player 1.
            Game.ServerPlayer1 = new ModelPlayer(1);
            Game.ServerPlayer1.Position = new Vector2f(64, 144);
            Game.ClientPlayer1 = new RenderPlayer(Game.ServerPlayer1);

            // Spawn player 2.
            Game.ServerPlayer2 = new ModelPlayer(2);
            Game.ServerPlayer2.Position = new Vector2f(192, 144);
            Game.ClientPlayer2 = new RenderPlayer(Game.ServerPlayer2);
        }
        public override void PreRender(Display target)
        {

        }
        public override void Update(Display target)
        {
            if (UpdateTimer.ElapsedMilliseconds % 2 == 0)
            {
                HealthBar1.TextureRect = new IntRect(0, 0, (int)(((10.0F - Game.ServerPlayer1.Damage) / 10.0F) * 37F), 8);
                if (BarPlate1.Position.X < 0)
                {
                    BarPlate1.Position = new Vector2f(BarPlate1.Position.X + target.FrameDelta, 1);
                    HealthBar1.Position = new Vector2f(BarPlate1.Position.X + 24, BarPlate1.Position.Y + 4);
                }
                else
                {
                    BarPlate1.Position = new Vector2f(1, 1);
                    HealthBar1.Position = new Vector2f(BarPlate1.Position.X + 24, BarPlate1.Position.Y + 4);
                    Score1HudText.DisplayedString = "Score: " + Score1;
                    if (Score1 < HighestScore1InRound)
                    {
                        Score1HudText.Color = Color.Red;
                    }
                    else if (Score1 < Score2)
                    {
                        Score1HudText.Color = Color.Red;
                    }
                    else
                    {
                        Score1HudText.Color = Color.White;
                    }
                    if (Score1HudText.Position.Y < 23)
                    {
                        Score1HudText.Position = new Vector2f(1, Score1HudText.Position.Y + target.FrameDelta);
                    }
                    else
                    {
                        Score1HudText.Position = new Vector2f(1, 23);
                    }
                }
                if (Blink1Timer.ElapsedMilliseconds > 4000)
                {
                    long ms = Blink1Timer.ElapsedMilliseconds;
                    if (ms > 4050)
                    {
                        if (ms > 4060)
                        {
                            if (ms > 4260)
                            {
                                Blink1Timer.Restart();
                            }
                            else
                            {
                                FacePlate1.Texture = Textures.Load("plate", "p1", "face_3.png");
                            }
                        }
                        else
                        {
                            FacePlate1.Texture = Textures.Load("plate", "p1", "face_2.png");
                        }
                    }
                    else
                    {
                        FacePlate1.Texture = Textures.Load("plate", "p1", "face_1.png");
                    }
                }
                else
                {
                    long ms = Blink1Timer.ElapsedMilliseconds;
                    if (ms > 50)
                    {
                        if (ms > 100)
                        {
                            FacePlate1.Texture = Textures.Load("plate", "p1", "face_0.png");
                        }
                        else
                        {
                            FacePlate1.Texture = Textures.Load("plate", "p1", "face_1.png");
                        }
                    }
                    else
                    {
                        FacePlate1.Texture = Textures.Load("plate", "p1", "face_2.png");
                    }
                }
                HealthBar2.TextureRect = new IntRect(0, 0, (int)(((10.0F - Game.ServerPlayer2.Damage) / 10.0F) * 37F), 8);
                if (BarPlate2.Position.X > 191)
                {
                    BarPlate2.Position = new Vector2f(BarPlate2.Position.X - target.FrameDelta, 1);
                    HealthBar2.Position = new Vector2f(BarPlate2.Position.X + 3, BarPlate2.Position.Y + 12);
                }
                else
                {
                    BarPlate2.Position = new Vector2f(191, 1);
                    HealthBar2.Position = new Vector2f(BarPlate2.Position.X + 3, BarPlate2.Position.Y + 12);
                    Score2HudText.DisplayedString = "Score: " + Score2;
                    if (Score2 < HighestScore2InRound)
                    {
                        Score2HudText.Color = Color.Red;
                    }
                    else if (Score2 < Score1)
                    {
                        Score2HudText.Color = Color.Red;
                    }
                    else
                    {
                        Score2HudText.Color = Color.White;
                    }
                    if (Score2HudText.Position.Y < 23)
                    {
                        Score2HudText.Position = new Vector2f(256 - (1 + Score2HudText.GetGlobalBounds().Width), Score2HudText.Position.Y + target.FrameDelta);
                    }
                    else
                    {
                        Score2HudText.Position = new Vector2f(256 - (1 + Score2HudText.GetGlobalBounds().Width), 23);
                    }
                }
                if (Blink2Timer.ElapsedMilliseconds > 4000)
                {
                    long ms = Blink2Timer.ElapsedMilliseconds;
                    if (ms > 4050)
                    {
                        if (ms > 4060)
                        {
                            if (ms > 4260)
                            {
                                Blink2Timer.Restart();
                            }
                            else
                            {
                                FacePlate2.Texture = Textures.Load("plate", "p2", "face_3.png");
                            }
                        }
                        else
                        {
                            FacePlate2.Texture = Textures.Load("plate", "p2", "face_2.png");
                        }
                    }
                    else
                    {
                        FacePlate2.Texture = Textures.Load("plate", "p2", "face_1.png");
                    }
                }
                else
                {
                    long ms = Blink2Timer.ElapsedMilliseconds;
                    if (ms > 50)
                    {
                        if (ms > 100)
                        {
                            FacePlate2.Texture = Textures.Load("plate", "p2", "face_0.png");
                        }
                        else
                        {
                            FacePlate2.Texture = Textures.Load("plate", "p2", "face_1.png");
                        }
                    }
                    else
                    {
                        FacePlate2.Texture = Textures.Load("plate", "p2", "face_2.png");
                    }
                }
                BossBar.TextureRect = new IntRect(0, 0, (int)(((100.0F - Boss.Damage) / 100.0F) * 54F), 6);
                if (Level == 10)
                {
                    if (BossPlate.Position.Y < 2)
                    {
                        BossPlate.Position = new Vector2f(96, BossPlate.Position.Y + target.FrameDelta);
                        BossBar.Position = new Vector2f(BossPlate.Position.X + 5, BossPlate.Position.Y + 23);
                    }
                    else
                    {
                        BossPlate.Position = new Vector2f(96, 2);
                        BossBar.Position = new Vector2f(101, 25);
                    }
                }
                else
                {
                    BossPlate.Position = new Vector2f(96, -17);
                }
            }
            else
            {
                if (Game.ServerPlayer1.IsNotDead() && Game.ServerPlayer2.IsNotDead())
                {
                    if (Level == 10)
                    {
                        if (!Game.ServerEntities.Contains(Boss))
                        {
                            Game.AddEntity(new RenderBoss(Boss));
                        }
                        if (Boss.IsDead())
                        {
                            LevelUp();
                        }
                    }
                    else
                    {
                        if (UpdateTimer.ElapsedMilliseconds > 20)
                        {
                            float delay = -(2.1F * (float)(10 * Math.Ceiling((Level % 10 + 1) / 10.0)) * Level) + (10 * Game.Configs.Difficulty) * (float)(Math.Sin(Level * Level)) / (float)(Math.Ceiling(Level / 10.0F)) + 1000;
                            delay *= Math.Max(0.25F, (1.0F - PassAttempts / PassesUntilNextLevel) * Game.Configs.Difficulty);
                            if (LevelTimer.ElapsedMilliseconds > Math.Max(20, delay))
                            {
                                if (Level % 2 == 0)
                                {
                                    Game.AddEntity(new RenderDiver(new ModelDiver()));
                                    PassAttempts += 1;
                                }
                                else if (Level % 3 == 0)
                                {
                                    if (Game.RNG.Next(3) > 0)
                                    {
                                        Game.AddEntity(new RenderSinker(new ModelSinker()));
                                        PassAttempts += 1;
                                    }
                                    else
                                    {
                                        Game.AddEntity(new RenderDiver(new ModelDiver()));
                                        PassAttempts += 1;
                                    }
                                }
                                else if (Level == 5 || Level == 7)
                                {
                                    // These are prime intro rounds.
                                }
                                else
                                {
                                    Game.AddEntity(new RenderSinker(new ModelSinker()));
                                    PassAttempts += 1;
                                }
                                if (Level % 5 == 0 || Level % 11 == 0)
                                {
                                    Game.AddEntity(new RenderSpinner(new ModelSpinner()));
                                    PassAttempts += 1;
                                }
                                if (Level % 7 == 0 || Level % 13 == 0)
                                {
                                    Game.AddEntity(new RenderArcher(new ModelArcher()));
                                    PassAttempts += 1;
                                }
                                LevelTimer.Restart();
                            }
                            UpdateTimer.Restart();
                        }
                    }
                }
            }
            Game.ClientPlayer1.Update(target);
            if (Game.ServerPlayer1.DamageTimer.ElapsedMilliseconds < 500)
            {
                ComboStreak1 = 0;
            }
            Game.ClientPlayer2.Update(target);
            if (Game.ServerPlayer2.DamageTimer.ElapsedMilliseconds < 500)
            {
                ComboStreak2 = 0;
            }
            if (Game.ServerPlayer1.IsDead() || Game.ServerPlayer2.IsDead())
            {
                if (Joystick.IsButtonPressed(0, 2) || Keyboard.IsKeyPressed(Keyboard.Key.X))
                {
                    Game.IsOnStartScreen = true;
                }
            }
            if (Passes >= PassesUntilNextLevel)
            {
                LevelUp();
            }
            Sounds.SetPitch(target.SpeedFactor);
        }
        public override void PostRender(Display target)
        {
            if (Game.ServerPlayer1.IsNotDead() && Game.ServerPlayer2.IsNotDead())
            {
                Score1HudText.Draw(target, RenderStates.Default);
                BarPlate1.Draw(target, RenderStates.Default);
                HealthBar1.Draw(target, RenderStates.Default);
                FacePlate1.Draw(target, RenderStates.Default);
                Score2HudText.Draw(target, RenderStates.Default);
                BarPlate2.Draw(target, RenderStates.Default);
                HealthBar2.Draw(target, RenderStates.Default);
                FacePlate2.Draw(target, RenderStates.Default);
                if (Level == 10)
                {
                    BossPlate.Draw(target, RenderStates.Default);
                    BossBar.Draw(target, RenderStates.Default);
                }
                if (FaderAlpha > 0)
                {
                    Fader.Color = new Color(255, 255, 255, (byte)(FaderAlpha));
                    FaderAlpha -= target.FrameDelta * 4;
                    Fader.Draw(target, RenderStates.Default);
                }
            }
            else
            {
                if (FaderAlpha < 255 && Game.Configs.FadeOut)
                {
                    Fader.Color = new Color(0, 0, 0, (byte)(FaderAlpha));
                    FaderAlpha += target.FrameDelta * 4;
                    Fader.Draw(target, RenderStates.Default);
                }
                else
                {
                    Fader.Draw(target, RenderStates.Default);
                    ContinueText.Draw(target, RenderStates.Default);
                    GameOverText.Draw(target, RenderStates.Default);
                    if (Game.ServerPlayer1.IsDead() && Game.ServerPlayer2.IsDead())
                    {
                        FinalScoreText.DisplayedString = "You both lost!";
                    }
                    else if (Game.ServerPlayer2.IsDead())
                    {
                        FinalScoreText.DisplayedString = "Player 1 wins!";
                    }
                    else
                    {
                        FinalScoreText.DisplayedString = "Player 2 wins!";
                    }
                    FinalScoreText.Position = new Vector2f(128, GameOverText.GetGlobalBounds().Top + FinalScoreText.GetGlobalBounds().Height - 20);
                    FinalScoreText.Origin = new Vector2f(FinalScoreText.GetLocalBounds().Width / 2, FinalScoreText.GetLocalBounds().Height / 2);
                    FinalScoreText.Draw(target, RenderStates.Default);
                }
            }
        }
        public override void Interact(int rank, int type)
        {
            if (Game.ServerPlayer1.IsNotDead() && Game.ServerPlayer2.IsNotDead())
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
                        ComboStreak1 += 1;
                        Passes += 1;
                    }
                    else
                    {
                        if (ComboStreak1 > 1 && Combo1Timer.ElapsedMilliseconds > 250)
                        {
                            Sounds.Play("fail.ogg");
                            Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, breaker[Game.RNG.Next(breaker.Length)], Color.Red)));
                            Combo1Timer.Restart();
                        }
                        ComboStreak1 = 0;
                    }
                    if (ComboStreak1 % 3 == 0 && ComboStreak1 > 0)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, combo[Game.RNG.Next(combo.Length)], Color.Magenta)));
                        for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Magenta)));
                        }
                        Game.ServerPlayer1.Damage -= ComboStreak1 / 3;
                        if (Game.ServerPlayer1.Damage < 0)
                        {
                            Game.ServerPlayer1.Damage = 0;
                        }
                        Score1 += ComboStreak1 / 3;
                        if (ComboStreak1 >= 12 && Game.ServerPlayer1.BuffTimer.ElapsedMilliseconds > 2000)
                        {
                            Game.ServerPlayer1.BuffTimer.Restart();
                            Game.ServerPlayer1.Buff = ModelPlayer.PowerUp.HYPERLASER;
                        }
                    }
                    else if (ComboStreak1 > 2)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, triples[Game.RNG.Next(triples.Length)], Color.Magenta)));
                    }
                    else if (ComboStreak1 > 1)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, doubles[Game.RNG.Next(doubles.Length)], Color.Magenta)));
                    }
                    Score1 += type;
                    if (Score1 >= HighestScore1InRound && HighestScore1InRound % 50 * Level > Score1 % 50 * Level)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, boost[Game.RNG.Next(boost.Length)], Color.Green)));
                        for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Green)));
                        }
                    }
                    if (Score1 > HighestScore1InRound)
                    {
                        HighestScore1InRound = Score1;
                    }
                }
                else
                {
                    if (type > 0)
                    {
                        ComboStreak2 += 1;
                        Passes += 1;
                    }
                    else
                    {
                        if (ComboStreak2 > 1 && Combo2Timer.ElapsedMilliseconds > 250)
                        {
                            Sounds.Play("fail.ogg");
                            Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, breaker[Game.RNG.Next(breaker.Length)], Color.Red)));
                            Combo2Timer.Restart();
                        }
                        ComboStreak2 = 0;
                    }
                    if (ComboStreak2 % 3 == 0 && ComboStreak2 > 0)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, combo[Game.RNG.Next(combo.Length)], Color.Magenta)));
                        for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer2.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer2.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Magenta)));
                        }
                        Game.ServerPlayer2.Damage -= ComboStreak2 / 3;
                        if (Game.ServerPlayer2.Damage < 0)
                        {
                            Game.ServerPlayer2.Damage = 0;
                        }
                        Score2 += ComboStreak2 / 3;
                        if (ComboStreak2 >= 12 && Game.ServerPlayer2.BuffTimer.ElapsedMilliseconds > 2000)
                        {
                            Game.ServerPlayer2.BuffTimer.Restart();
                            Game.ServerPlayer2.Buff = ModelPlayer.PowerUp.HYPERLASER;
                        }
                    }
                    else if (ComboStreak2 > 2)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, triples[Game.RNG.Next(triples.Length)], Color.Magenta)));
                    }
                    else if (ComboStreak2 > 1)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, doubles[Game.RNG.Next(doubles.Length)], Color.Magenta)));
                    }
                    Score2 += type;
                    if (Score2 >= HighestScore2InRound && HighestScore2InRound % 50 * Level > Score2 % 50 * Level)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y, boost[Game.RNG.Next(boost.Length)], Color.Green)));
                        for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer2.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer2.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Green)));
                        }
                        Game.ServerPlayer2.Damage = 0;
                    }
                    if (Score2 > HighestScore1InRound)
                    {
                        HighestScore1InRound = Score2;
                    }
                }
            }
        }
        public void LevelUp()
        {
            Level += 1;
            Passes = 0;
            PassAttempts = 0;
            PassesUntilNextLevel *= 0.75F / (Game.Configs.Difficulty / 2);
            Game.ServerPlayer1.Damage = 0;
            Game.ServerPlayer2.Damage = 0;
            Sounds.Play("start.ogg");
        }
        public override bool IsMultiUser()
        {
            return true;
        }
        public override void GetRPC()
        {
            Discord.RP.Assets.SmallImageText = "Playing Team Mode";
            Discord.RP.Assets.SmallImageKey = "p2";
            Discord.RP.Details = "Team Mode | Level " + Level;
            Discord.RP.State = "Score: " + Score1 + " | " + Score2;
            Discord.PushUpdate = true;
        }
    }
}
