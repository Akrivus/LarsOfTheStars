using LarsOfTheStars.Source.Files;
using LarsOfTheStars.Source.Integration;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;

namespace LarsOfTheStars.Source.Client.Screens
{
    public class Menu
    {
        private static int MAX_STARS = Game.Configs.MaxStars;
        private Sprite[] Stars = new Sprite[MAX_STARS];
        private Sprite Logo;
        private Sprite ArcadeModeButton;
        private Sprite TeamModeButton;
        private Sprite ExitGameButton;
        private Sound Player = new Sound();
        public Stopwatch BlinkTimer = new Stopwatch();
        public Stopwatch SplashTimer = new Stopwatch();
        public Stopwatch FlashTimer = new Stopwatch();
        private Sprite AppyDays = new Sprite(Textures.Load("appydays.png"));
        private float AppyAlpha = 255;
        private bool PlayedAppySound = false;
        private Sprite InvasionButton;
        private bool OngoingInvasion = false;
        private bool KeyboardEnabled = true;
        private int TabIndex = 0;
        public Menu()
        {
            for (int i = 0; i < MAX_STARS; ++i)
            {
                float starSize = (float)(Game.RNG.NextDouble() + 0.5);
                Stars[i] = new Sprite(Textures.Load("star.png"));
                Stars[i].Position = new Vector2f(Game.RNG.Next(0, 256), Game.RNG.Next(0, 192));
                Stars[i].Color = new Color(255, 255, 255, (byte)(Game.RNG.Next(255)));
                Stars[i].Scale = new Vector2f(starSize, starSize);
            }
            Logo = new Sprite(Textures.Load("logo.png"));
            Logo.Position = new Vector2f(56.5F, 20);
            InvasionButton = new Sprite(Textures.Load("menu", "mode_d_0.png"));
            InvasionButton.Position = new Vector2f(0, 105);
            ArcadeModeButton = new Sprite(Textures.Load("menu", "mode_0.png"));
            ArcadeModeButton.Position = new Vector2f(0, 125);
            TeamModeButton = new Sprite(Textures.Load("menu", "mode_1.png"));
            TeamModeButton.Position = new Vector2f(0, 145);
            ExitGameButton = new Sprite(Textures.Load("menu", "mode_2.png"));
            ExitGameButton.Position = new Vector2f(0, 165);
            SplashTimer.Start();
            BlinkTimer.Start();
            FlashTimer.Start();
            Discord.RP.Assets.SmallImageText = "In Menu";
            Discord.RP.Assets.SmallImageKey = "p3";
            Discord.RP.Details = "In Menu | Deciding";
            Discord.PushUpdate = true;
        }
        public void Render(Display target)
        {
            if (Game.IsFocused && SplashTimer.ElapsedMilliseconds > 11394)
            {
                if (Joystick.IsButtonPressed(0, 0) || Keyboard.IsKeyPressed(Keyboard.Key.Space) || Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                {
                    Sounds.Play("start.ogg");
                    Game.IsOnStartScreen = false;
                    Game.Mode = Game.Modes[TabIndex];
                    Game.Mode.Start();
                }
                else
                {
                    if (Joystick.IsButtonPressed(0, 5) || Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        if (KeyboardEnabled)
                        {
                            Sounds.Play("choose.ogg");
                            KeyboardEnabled = false;
                            TabIndex += 1;
                        }
                    }
                    else
                    if (Joystick.IsButtonPressed(0, 4) || Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    {
                        if (KeyboardEnabled)
                        {
                            Sounds.Play("choose.ogg");
                            KeyboardEnabled = false;
                            TabIndex -= 1;
                        }
                    }
                    else
                    {
                        KeyboardEnabled = true;
                    }
                    if (TabIndex < 0)
                    {
                        if (OngoingInvasion)
                        {
                            TabIndex = 3;
                        }
                        else
                        {
                            TabIndex = 2;
                        }
                    }
                    if (OngoingInvasion)
                    {
                        if (TabIndex > 3)
                        {
                            TabIndex = 0;
                        }
                    }
                    else
                    {
                        if (TabIndex > 2)
                        {
                            TabIndex = 0;
                        }
                    }
                }
            }
            if (Game.Configs.AutoStart > -1)
            {
                TabIndex = 0;
            }
            for (int i = 0; i < MAX_STARS; ++i)
            {
                Stars[i].Position = new Vector2f(Stars[i].Position.X, Stars[i].Position.Y + target.FrameDelta);
                if (Stars[i].Position.Y > 200)
                {
                    Stars[i].Position = new Vector2f(Stars[i].Position.X, Game.RNG.Next(12) * -1);
                }
                Stars[i].Draw(target, RenderStates.Default);
            }
            if (SplashTimer.ElapsedMilliseconds > 11394)
            {
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
                                Logo.Texture = Textures.Load("logo", "logo_3.png");
                            }
                        }
                        else
                        {
                            Logo.Texture = Textures.Load("logo", "logo_2.png");
                        }
                    }
                    else
                    {
                        Logo.Texture = Textures.Load("logo", "logo_1.png");
                    }
                }
                else
                {
                    long ms = BlinkTimer.ElapsedMilliseconds;
                    if (ms > 50)
                    {
                        if (ms > 100)
                        {
                            Logo.Texture = Textures.Load("logo", "logo_0.png");
                        }
                        else
                        {
                            Logo.Texture = Textures.Load("logo", "logo_1.png");
                        }
                    }
                    else
                    {
                        Logo.Texture = Textures.Load("logo", "logo_2.png");
                    }
                }
                Logo.Draw(target, RenderStates.Default);
                switch (TabIndex)
                {
                    case 0:
                        TeamModeButton.Texture = Textures.Load("menu", "mode_1.png"); ExitGameButton.Texture = Textures.Load("menu", "mode_2.png");
                        ArcadeModeButton.Texture = Textures.Load("menu", "mode_0_sel.png");
                        break;
                    case 1:
                        ArcadeModeButton.Texture = Textures.Load("menu", "mode_0.png"); ExitGameButton.Texture = Textures.Load("menu", "mode_2.png");
                        TeamModeButton.Texture = Textures.Load("menu", "mode_1_sel.png");
                        break;
                    case 2:
                        ArcadeModeButton.Texture = Textures.Load("menu", "mode_0.png"); TeamModeButton.Texture = Textures.Load("menu", "mode_1.png");
                        ExitGameButton.Texture = Textures.Load("menu", "mode_2_sel.png");
                        break;
                    default:
                        ArcadeModeButton.Texture = Textures.Load("menu", "mode_0.png"); TeamModeButton.Texture = Textures.Load("menu", "mode_1.png");
                        ExitGameButton.Texture = Textures.Load("menu", "mode_2.png");
                        break;
                }
                if (OngoingInvasion)
                {
                    if (TabIndex != 3)
                    {
                        if (FlashTimer.ElapsedMilliseconds > 1000)
                        {
                            FlashTimer.Restart();
                        }
                        else if (FlashTimer.ElapsedMilliseconds > 500)
                        {
                            InvasionButton.Texture = new Texture(Textures.Load("menu", "mode_d_1.png"));
                        }
                        else
                        {
                            InvasionButton.Texture = new Texture(Textures.Load("menu", "mode_d_0.png"));
                        }
                    }
                    else
                    {
                        InvasionButton.Texture = new Texture(Textures.Load("menu", "mode_d_sel.png"));
                    }
                }
                else if (Discord.IsReady)
                {
                    /*OngoingInvasion = (bool)(Game.Execute("register" +
                        "?discID=" + Discord.You.ID +
                        "&discName=" + Discord.You.Username +
                        "&discNum=" + Discord.You.Discriminator
                    )["Defend"]);*/
                }
                ArcadeModeButton.Draw(target, RenderStates.Default);
                if (Game.Configs.AutoStart == -1)
                {
                    if (OngoingInvasion)
                    {
                        InvasionButton.Draw(target, RenderStates.Default);
                    }
                    TeamModeButton.Draw(target, RenderStates.Default);
                    ExitGameButton.Draw(target, RenderStates.Default);
                }
                if (AppyAlpha > 0)
                {
                    AppyDays.Color = new Color(255, 255, 255, (byte)(AppyAlpha));
                    AppyAlpha -= target.FrameDelta * 2;
                }
            }
            else
            {
                if (SplashTimer.ElapsedMilliseconds < 11394)
                {
                    if (!PlayedAppySound)
                    {
                        Sounds.Play("appydays.ogg");
                        PlayedAppySound = true;
                    }
                }
            }
            AppyDays.Draw(target, RenderStates.Default);
        }
    }
}
