using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using LarsOfTheStars.Source.Integration;
using LarsOfTheStars.Source.Logic.Objects;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;
using System.IO;

namespace LarsOfTheStars.Source.Logic.Modes
{
    public class DefenseMode : Mode
    {
        // Top-left-corner GUIs.
        private Sprite FacePlate = new Sprite(Textures.Load("plate", "p1", "face_0.png"));
        private Sprite BarPlate = new Sprite(Textures.Load("plate", "p1", "bars.png"));
        private Sprite HealthBar = new Sprite(Textures.Load("plate", "p1", "bar_health.png"));
        private Sprite LevelBar = new Sprite(Textures.Load("plate", "p1", "bar_level.png"));
        private Text ScoreHudText = new Text("", Fonts.Load("gameover.ttf"), 75);

        // Game over GUIs.
        private Text GameOverText = new Text("Game Over", Fonts.Load("start2p.ttf"), 75);
        private Text FinalScoreText = new Text("Final Score: 0", Fonts.Load("start2p.ttf"), 75);
        private Text ContinueText = new Text("Press [X] to go to menu.", Fonts.Load("start2p.ttf"), 75);

        // New level GUIs.
        private Text LevelNameText = new Text("Level 1", Fonts.Load("start2p.ttf"), 75);
        public float LevelAlpha = 0;

        // Fader.
        private Sprite Fader = new Sprite(Textures.Load("fader.png"));
        private float FaderAlpha = 255;

        // GUI timers.
        public Stopwatch BlinkTimer = new Stopwatch();
        public Stopwatch TitleTimer = new Stopwatch();

        // Game timers.
        public Stopwatch ComboTimer = new Stopwatch();
        public Stopwatch UpdateTimer = new Stopwatch();
        public Stopwatch LevelTimer = new Stopwatch();

        // Game values.
        public float PassesUntilNextLevel = 0;
        public float Passes = 0;
        public float PassAttempts = 0;
        public int ComboStreak = 0;
        public int HighestScoreInRound = 0;
        public int Score = 0;

        // Trigger on start of mode.
        public override void Start()
        {
            // Position HUD.
            FacePlate.Position = new Vector2f(1, 1);
            BarPlate.Position = new Vector2f(-65, 1);
            HealthBar.Position = new Vector2f(-41, 5);
            LevelBar.Position = new Vector2f(-41, 14);

            // Score elements.
            ScoreHudText.Position = new Vector2f(1, 7);
            ScoreHudText.Scale = new Vector2f(0.1F, 0.1F);

            // Position game over screen.
            GameOverText.Position = new Vector2f(128, 96); GameOverText.Origin = new Vector2f(GameOverText.GetLocalBounds().Width / 2, GameOverText.GetLocalBounds().Height / 2);
            GameOverText.Scale = new Vector2f(0.25F, 0.25F);
            ContinueText.Position = new Vector2f(128, GameOverText.GetGlobalBounds().Height + GameOverText.GetGlobalBounds().Top + 10); ContinueText.Origin = new Vector2f(ContinueText.GetLocalBounds().Width / 2, ContinueText.GetLocalBounds().Height / 2);
            ContinueText.Scale = new Vector2f(0.1F, 0.1F);
            FinalScoreText.Scale = new Vector2f(0.1F, 0.1F);

            // Position level screen.
            LevelNameText.Scale = new Vector2f(0.25F, 0.25F);
            LevelNameText.Origin = new Vector2f(LevelNameText.GetLocalBounds().Width / 2, LevelNameText.GetLocalBounds().Height / 2);
            LevelNameText.Position = new Vector2f(128, 96);

            // Start timers.
            BlinkTimer.Start();
            TitleTimer.Start();
            ComboTimer.Start();
            UpdateTimer.Start();
            LevelTimer.Start();

            // Reset entities and spawn player.
            Game.ClientEntities.Clear();
            Game.ServerEntities.Clear();
            Game.ServerPlayer1 = new ModelPlayer(1);
            Game.ClientPlayer1 = new RenderPlayer(Game.ServerPlayer1);
        }
        public override void Update(Display target)
        {
            if (UpdateTimer.ElapsedMilliseconds % 2 == 0)
            {
                HealthBar.TextureRect = new IntRect(0, 0, (int)(((10.0F - Game.ServerPlayer1.Damage) / 10.0F) * 37F), 8);
                LevelBar.TextureRect = new IntRect(0, 0, (int)((Passes / PassesUntilNextLevel) * 30F), 7);
                if (BarPlate.Position.X < 0)
                {
                    BarPlate.Position = new Vector2f(BarPlate.Position.X + target.FrameDelta, 1);
                    HealthBar.Position = new Vector2f(BarPlate.Position.X + 24, BarPlate.Position.Y + 4);
                    LevelBar.Position = new Vector2f(BarPlate.Position.X + 24, BarPlate.Position.Y + 13);
                }
                else
                {
                    BarPlate.Position = new Vector2f(1, 1);
                    HealthBar.Position = new Vector2f(25, 5);
                    LevelBar.Position = new Vector2f(25, 14);
                    ScoreHudText.DisplayedString = "Score: " + Score;
                    if (Score < HighestScoreInRound)
                    {
                        ScoreHudText.Color = Color.Red;
                    }
                    else
                    {
                        ScoreHudText.Color = Color.White;
                    }
                    if (ScoreHudText.Position.Y < 23)
                    {
                        ScoreHudText.Position = new Vector2f(1, ScoreHudText.Position.Y + target.FrameDelta);
                    }
                    else
                    {
                        ScoreHudText.Position = new Vector2f(1, 23);
                    }
                }
                if (BlinkTimer.ElapsedMilliseconds > 4000)
                {
                    long ms = BlinkTimer.ElapsedMilliseconds;
                    if (ms > 4050)
                    {
                        if (ms > 4060)
                        {
                            if (ms > 4260)
                            {
                                BlinkTimer.Restart();
                            }
                            else
                            {
                                FacePlate.Texture = Textures.Load("plate", "p1", "face_3.png");
                            }
                        }
                        else
                        {
                            FacePlate.Texture = Textures.Load("plate", "p1", "face_2.png");
                        }
                    }
                    else
                    {
                        FacePlate.Texture = Textures.Load("plate", "p1", "face_1.png");
                    }
                }
                else
                {
                    long ms = BlinkTimer.ElapsedMilliseconds;
                    if (ms > 50)
                    {
                        if (ms > 100)
                        {
                            FacePlate.Texture = Textures.Load("plate", "p1", "face_0.png");
                        }
                        else
                        {
                            FacePlate.Texture = Textures.Load("plate", "p1", "face_1.png");
                        }
                    }
                    else
                    {
                        FacePlate.Texture = Textures.Load("plate", "p1", "face_2.png");
                    }
                }
            }
            else
            {
                if (Game.ServerPlayer1.IsNotDead())
                {
                    
                }
            }
            Game.ClientPlayer1.Update(target);
            if (Game.ServerPlayer1.DamageTimer.ElapsedMilliseconds < 500)
            {
                ComboStreak = 0;
            }
            if (Game.ServerPlayer1.IsDead())
            {
                if (Joystick.IsButtonPressed(0, 2) || Keyboard.IsKeyPressed(Keyboard.Key.X))
                {
                    Game.IsOnStartScreen = true;
                }
            }
            Sounds.SetPitch(target.SpeedFactor);
        }
        public override void PostRender(Display target)
        {
            if (Game.ServerPlayer1.IsNotDead())
            {
                ScoreHudText.Draw(target, RenderStates.Default);
                BarPlate.Draw(target, RenderStates.Default);
                HealthBar.Draw(target, RenderStates.Default);
                LevelBar.Draw(target, RenderStates.Default);
                FacePlate.Draw(target, RenderStates.Default);
                if (FaderAlpha > 0)
                {
                    Fader.Color = new Color(255, 255, 255, (byte)(FaderAlpha));
                    FaderAlpha -= target.FrameDelta * 4;
                    Fader.Draw(target, RenderStates.Default);
                }
                if (TitleTimer.ElapsedMilliseconds < 3000)
                {
                    LevelNameText.DisplayedString = "Level " + Level;
                    LevelNameText.Color = new Color(255, 255, 255, 255);
                    LevelNameText.Origin = new Vector2f(LevelNameText.GetLocalBounds().Width / 2, LevelNameText.GetLocalBounds().Height / 2);
                    LevelNameText.Draw(target, RenderStates.Default);
                    LevelAlpha = 255;
                }
                else if (LevelAlpha > 0 && Game.Configs.FadeOut)
                {
                    LevelNameText.Color = new Color(255, 255, 255, (byte)(LevelAlpha));
                    LevelAlpha -= target.FrameDelta * 4;
                    LevelNameText.Draw(target, RenderStates.Default);
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
                    FinalScoreText.DisplayedString = "Final Score: " + Score;
                    FinalScoreText.Position = new Vector2f(128, GameOverText.GetGlobalBounds().Top + FinalScoreText.GetGlobalBounds().Height - 20);
                    FinalScoreText.Origin = new Vector2f(FinalScoreText.GetLocalBounds().Width / 2, FinalScoreText.GetLocalBounds().Height / 2);
                    FinalScoreText.Draw(target, RenderStates.Default);
                    ContinueText.Draw(target, RenderStates.Default);
                    GameOverText.Draw(target, RenderStates.Default);
                }
            }
        }
        public override void Interact(int rank, int type)
        {
            if (Game.ServerPlayer1.IsNotDead())
            {
                string[] doubles = new string[] { "Nice shot!", "Double shot!", "Double trouble!", "Dueces!" };
                string[] triples = new string[] { "Neato burrito!", "Oh baby a triple!", "Aces!" };
                string[] breaker = new string[] { "Combo breaker!", "Ouch!" };
                string[] combo = new string[] { "Bingo bongo!", "Nova thrusters!" };
                string[] boost = new string[] { "Feels good!", "Boost!" };
                if (type > 0)
                {
                    ComboStreak += 1;
                    Passes += 1;
                }
                else
                {
                    if (ComboStreak > 1 && ComboTimer.ElapsedMilliseconds > 500)
                    {
                        Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, breaker[Game.RNG.Next(breaker.Length)], Color.Red)));
                        Sounds.Play("fail.ogg");
                        ComboTimer.Restart();
                    }
                    ComboStreak = 0;
                }
                if (ComboStreak % 3 == 0 && ComboStreak > 0)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, combo[Game.RNG.Next(combo.Length)], Color.Magenta)));
                    for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Magenta)));
                    }
                    Game.ServerPlayer1.Damage -= ComboStreak / 3;
                    if (Game.ServerPlayer1.Damage < 0)
                    {
                        Game.ServerPlayer1.Damage = 0;
                    }
                    Score += ComboStreak / 3;
                    Passes += 1;
                    if (ComboStreak >= 12 && Game.ServerPlayer1.BuffTimer.ElapsedMilliseconds > 2000)
                    {
                        Game.ServerPlayer1.BuffTimer.Restart();
                        Game.ServerPlayer1.Buff = ModelPlayer.PowerUp.HYPERLASER;
                    }
                }
                else if (ComboStreak > 2)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, triples[Game.RNG.Next(triples.Length)], Color.Magenta)));
                }
                else if (ComboStreak > 1)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, doubles[Game.RNG.Next(doubles.Length)], Color.Magenta)));
                }
                Score += type;
                if (Game.ServerPlayer1.Buff == ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
                {
                    Score += type;
                }
                if (Score >= HighestScoreInRound && HighestScoreInRound % 50 * Level > Score % 50 * Level)
                {
                    Game.AddEntity(new RenderIndicator(new ModelIndicator(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y, boost[Game.RNG.Next(boost.Length)], Color.Green)));
                    for (int i = 0; i < Game.Configs.MaxParticles / 3; ++i)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(Game.ServerPlayer1.Position.X + (Game.RNG.Next(16) - 8), Game.ServerPlayer1.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.Green)));
                    }
                    Game.ServerPlayer1.Damage = 0;
                }
                if (Score > HighestScoreInRound)
                {
                    HighestScoreInRound = Score;
                }
            }
        }
        public override void GetRPC()
        {
            Discord.RP.Assets.SmallImageText = "Playing Defense Mode";
            Discord.RP.Assets.SmallImageKey = "p3";
            Discord.RP.Details = "Defense Mode | Level " + Level;
            Discord.RP.State = "Score: " + Score;
            Discord.PushUpdate = true;
        }
    }
}
