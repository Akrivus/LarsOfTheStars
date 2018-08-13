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
        private Sprite Mod0;
        private Sprite Mod1;
        private Sprite Mod2;
        private Sound Player = new Sound();
        public Stopwatch BlinkTimer = new Stopwatch();
        private bool KeyboardEnabled = true;
        private int TabIndex = 0;
        public Menu()
        {
            for (int i = 0; i < MAX_STARS; ++i)
            {
                float starSize = (float)(Game.RNG.NextDouble() + 0.5);
                this.Stars[i] = new Sprite(Textures.Load("star.png"));
                this.Stars[i].Position = new Vector2f(Game.RNG.Next(0, 256), Game.RNG.Next(0, 192));
                this.Stars[i].Color = new Color(255, 255, 255, (byte)(Game.RNG.Next(255)));
                this.Stars[i].Scale = new Vector2f(starSize, starSize);
            }
            this.Logo = new Sprite(Textures.Load("logo.png"));
            this.Logo.Position = new Vector2f(56.5F, 20);
            this.Mod0 = new Sprite(Textures.Load("menu", "mode_0.png"));
            this.Mod0.Position = new Vector2f(0, 125);
            this.Mod1 = new Sprite(Textures.Load("menu", "mode_1.png"));
            this.Mod1.Position = new Vector2f(0, 145);
            this.Mod2 = new Sprite(Textures.Load("menu", "mode_2.png"));
            this.Mod2.Position = new Vector2f(0, 165);
            this.BlinkTimer.Start();
        }
        public void Render(Display target)
        {
            if (Game.IsFocused)
            {
                if (Joystick.IsButtonPressed(0, 0) || Keyboard.IsKeyPressed(Keyboard.Key.Space) || Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                {
                    if (this.TabIndex < 2)
                    {
                        Sounds.Play("start.ogg");
                        Game.IsOnStartScreen = false;
                        Game.Mode = Game.Modes[this.TabIndex];
                        Game.Mode.Start();
                    }
                    else
                    {
                        Game.Stopped = true;
                    }
                }
                else
                {
                    if (Joystick.IsButtonPressed(0, 5) || Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        if (this.KeyboardEnabled)
                        {
                            Sounds.Play("choose.ogg");
                            this.KeyboardEnabled = false;
                            this.TabIndex += 1;
                        }
                    }
                    else
                    if (Joystick.IsButtonPressed(0, 4) || Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    {
                        if (this.KeyboardEnabled)
                        {
                            Sounds.Play("choose.ogg");
                            this.KeyboardEnabled = false;
                            this.TabIndex -= 1;
                        }
                    }
                    else
                    {
                        this.KeyboardEnabled = true;
                    }
                    if (this.TabIndex < 0)
                    {
                        this.TabIndex = 2;
                    }
                    if (this.TabIndex > 2)
                    {
                        this.TabIndex = 0;
                    }
                }
            }
            if (Game.Configs.AutoStart > -1)
            {
                this.TabIndex = 0;
            }
            Discord.RP.Assets.SmallImageText = null;
            Discord.RP.Assets.SmallImageKey = "p3";
            Discord.RP.Details = null;
            Discord.RP.State = null;
            for (int i = 0; i < MAX_STARS; ++i)
            {
                this.Stars[i].Position = new Vector2f(this.Stars[i].Position.X, this.Stars[i].Position.Y + target.FrameDelta);
                if (this.Stars[i].Position.Y > 200)
                {
                    this.Stars[i].Position = new Vector2f(this.Stars[i].Position.X, Game.RNG.Next(12) * -1);
                }
                this.Stars[i].Draw(target, RenderStates.Default);
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
                            this.Logo.Texture = Textures.Load("logo", "logo_3.png");
                        }
                    }
                    else
                    {
                        this.Logo.Texture = Textures.Load("logo", "logo_2.png");
                    }
                }
                else
                {
                    this.Logo.Texture = Textures.Load("logo", "logo_1.png");
                }
            }
            else
            {
                long ms = this.BlinkTimer.ElapsedMilliseconds;
                if (ms > 50)
                {
                    if (ms > 100)
                    {
                        this.Logo.Texture = Textures.Load("logo", "logo_0.png");
                    }
                    else
                    {
                        this.Logo.Texture = Textures.Load("logo", "logo_1.png");
                    }
                }
                else
                {
                    this.Logo.Texture = Textures.Load("logo", "logo_2.png");
                }
            }
            this.Logo.Draw(target, RenderStates.Default);
            switch (this.TabIndex)
            {
                case 0:
                    this.Mod1.Texture = Textures.Load("menu", "mode_1.png"); this.Mod2.Texture = Textures.Load("menu", "mode_2.png");
                    this.Mod0.Texture = Textures.Load("menu", "mode_0_sel.png");
                    break;
                case 1:
                    this.Mod0.Texture = Textures.Load("menu", "mode_0.png"); this.Mod2.Texture = Textures.Load("menu", "mode_2.png");
                    this.Mod1.Texture = Textures.Load("menu", "mode_1_sel.png");
                    break;
                case 2:
                    this.Mod0.Texture = Textures.Load("menu", "mode_0.png"); this.Mod1.Texture = Textures.Load("menu", "mode_1.png");
                    this.Mod2.Texture = Textures.Load("menu", "mode_2_sel.png");
                    break;
                default:
                    break;
            }
            this.Mod0.Draw(target, RenderStates.Default);
            if (Game.Configs.AutoStart == -1)
            {
                this.Mod1.Draw(target, RenderStates.Default);
                this.Mod2.Draw(target, RenderStates.Default);
            }
        }
    }
}
