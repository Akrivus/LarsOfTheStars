using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderPlayer : Render
    {
        private static int MAX_PARTICLES = Game.Configs.MaxParticles;
        private Sprite[] Particles = new Sprite[MAX_PARTICLES];
        private Sprite Decal;
        private Stopwatch FlashTimer = new Stopwatch();
        private Stopwatch EffectTimer = new Stopwatch();
        private Stopwatch FireTimer = new Stopwatch();
        public RenderPlayer(ModelPlayer model) : base(model)
        {
            Sprite = new Sprite(Textures.Load("player", (model.Rank == 1 ?  "incinerator.png" : "skipper.png")));
            Decal = new Sprite(Textures.Load("player", "decal_" + model.Rank + ".png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
            Sprite.Scale = new Vector2f(2, 2);
            Decal.Origin = new Vector2f(Decal.Texture.Size.X / 2, Decal.Texture.Size.Y / 2);
            Decal.Scale = new Vector2f(2, 2);
            FireTimer.Start();
            for (int i = 0; i < MAX_PARTICLES; ++i)
            {
                Particles[i] = new Sprite(Textures.Load("particles", "rocket.png"));
                Particles[i].Position = new Vector2f(model.Position.X + (Game.RNG.Next(12) - 8), model.Position.Y + 20 + (Game.RNG.Next(8) - 4));
                Particles[i].Origin = new Vector2f(Particles[i].Texture.Size.X / 2, Particles[i].Texture.Size.Y / 2);
            }
        }
        public override void Update(Display target)
        {
            ModelPlayer player = (ModelPlayer)(Base);
            if (player.IsNotDead())
            {
                for (int i = 0; i < MAX_PARTICLES; ++i)
                {
                    Particles[i].Position = new Vector2f(Particles[i].Position.X, Particles[i].Position.Y + target.FrameDelta * 2);
                    if (Math.Abs(Particles[i].Position.Y - Sprite.Position.Y) > Game.RNG.Next(30, 40))
                    {
                        Particles[i].Position = new Vector2f(Sprite.Position.X + (Game.RNG.Next(12) - 6), Sprite.Position.Y + 20 + (Game.RNG.Next(8) - 4));
                        Particles[i].Color = Color.White;
                    }
                    else
                    if (Particles[i].Color.A > 0 && Game.Configs.FadeOut)
                    {
                        Particles[i].Color = new Color(255, 255, 255, (byte)((double)(Particles[i].Color.A) - target.FrameDelta * 20));
                    }
                    Particles[i].Draw(target, RenderStates.Default);
                }
            }
            base.Update(target);
            if (player.Buff != ModelPlayer.PowerUp.NONE)
            {
                if (!EffectTimer.IsRunning)
                {
                    EffectTimer.Start();
                }
                Decal.TextureRect = new IntRect(0, 0, 18, 18);
                Decal.Position = new Vector2f(player.Position.X, player.Position.Y);
                switch (player.Buff)
                {
                    case ModelPlayer.PowerUp.IMMUNE_TO_HITS:
                        Decal.Color = new Color(251, 173, 216);
                        break;
                    case ModelPlayer.PowerUp.HYPERLASER:
                        if (EffectTimer.ElapsedMilliseconds < 100)
                        {
                            Decal.Color = new Color(255, 0, 0);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 200)
                        {
                            Decal.Color = new Color(0, 127, 255);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 300)
                        {
                            Decal.Color = new Color(255, 0, 255);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 400)
                        {
                            Decal.Color = new Color(0, 255, 0);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 500)
                        {
                            Decal.Color = new Color(0, 255, 255);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 600)
                        {
                            Decal.Color = new Color(0, 0, 255);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 700)
                        {
                            Decal.Color = new Color(127, 0, 255);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 800)
                        {
                            Decal.Color = new Color(255, 0, 255);
                        }
                        else
                        {
                            EffectTimer.Restart();
                        }
                        break;
                    case ModelPlayer.PowerUp.MULTILASER:
                        if (EffectTimer.ElapsedMilliseconds < 200)
                        {
                            Decal.Color = new Color(127, 0, 255);
                        }
                        else if (EffectTimer.ElapsedMilliseconds < 400)
                        {
                            Decal.Color = new Color(0, 255, 0);
                        }
                        else
                        {
                            EffectTimer.Restart();
                        }
                        break;
                    case ModelPlayer.PowerUp.DESTROY_ALL_SHIPS:
                        Decal.Color = new Color(255, 127, 0);
                        break;
                    case ModelPlayer.PowerUp.FREEZE_ALL_SHIPS:
                        Decal.Color = new Color(0, 255, 255);
                        break;
                    case ModelPlayer.PowerUp.SPEED_UP_GAME:
                        Decal.Color = new Color(0, 255, 0);
                        break;
                    case ModelPlayer.PowerUp.SLOW_MOTION:
                        Decal.Color = new Color(127, 0, 255);
                        break;
                }
                Decal.Rotation = 180;
                Decal.Draw(target, RenderStates.Default);
            }
            else
            {
                if (player.IsNotDead() && player.Damage > 0)
                {
                    if (player.DamageTimer.ElapsedMilliseconds < 2000)
                    {
                        if (!FlashTimer.IsRunning)
                        {
                            FlashTimer.Start();
                        }
                        else if (FlashTimer.ElapsedMilliseconds < 200)
                        {
                            Decal.Position = new Vector2f(player.Position.X, player.Position.Y);
                            Decal.TextureRect = new IntRect(0, 0, 18, 18);
                            Decal.Color = new Color(255, 255, 255);
                            Decal.Rotation = 180;
                            Decal.Draw(target, RenderStates.Default);
                        }
                        else if (FlashTimer.ElapsedMilliseconds > 400)
                        {
                            FlashTimer.Restart();
                        }
                    }
                }
            }
        }
    }
}
