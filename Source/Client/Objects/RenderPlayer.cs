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
            this.Sprite = new Sprite(Textures.Load("player", (model.Rank == 1 ?  "incinerator.png" : "skipper.png")));
            this.Decal = new Sprite(Textures.Load("player", "decal_" + model.Rank + ".png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
            this.Sprite.Scale = new Vector2f(2, 2);
            this.Decal.Origin = new Vector2f(this.Decal.Texture.Size.X / 2, this.Decal.Texture.Size.Y / 2);
            this.Decal.Scale = new Vector2f(2, 2);
            this.FireTimer.Start();
            for (int i = 0; i < MAX_PARTICLES; ++i)
            {
                this.Particles[i] = new Sprite(Textures.Load("particles", "rocket.png"));
                this.Particles[i].Position = new Vector2f(model.Position.X + (Game.RNG.Next(12) - 8), model.Position.Y + 20 + (Game.RNG.Next(8) - 4));
                this.Particles[i].Origin = new Vector2f(this.Particles[i].Texture.Size.X / 2, this.Particles[i].Texture.Size.Y / 2);
            }
        }
        public override void Update(Display target)
        {
            ModelPlayer player = (ModelPlayer)(this.Base);
            if (player.IsNotDead())
            {
                for (int i = 0; i < MAX_PARTICLES; ++i)
                {
                    this.Particles[i].Position = new Vector2f(this.Particles[i].Position.X, this.Particles[i].Position.Y + target.FrameDelta * 2);
                    if (Math.Abs(this.Particles[i].Position.Y - this.Sprite.Position.Y) > Game.RNG.Next(30, 40))
                    {
                        this.Particles[i].Position = new Vector2f(this.Sprite.Position.X + (Game.RNG.Next(12) - 6), this.Sprite.Position.Y + 20 + (Game.RNG.Next(8) - 4));
                        this.Particles[i].Color = Color.White;
                    }
                    else
                    if (this.Particles[i].Color.A > 0 && Game.Configs.FadeOut)
                    {
                        this.Particles[i].Color = new Color(255, 255, 255, (byte)((double)(this.Particles[i].Color.A) - target.FrameDelta * 20));
                    }
                    this.Particles[i].Draw(target, RenderStates.Default);
                }
            }
            base.Update(target);
            if (player.Buff != ModelPlayer.PowerUp.NONE)
            {
                if (!this.EffectTimer.IsRunning)
                {
                    this.EffectTimer.Start();
                }
                this.Decal.TextureRect = new IntRect(0, 0, 18, 18);
                this.Decal.Position = new Vector2f(player.Position.X, player.Position.Y);
                switch (player.Buff)
                {
                    case ModelPlayer.PowerUp.IMMUNE_TO_HITS:
                        this.Decal.Color = new Color(251, 173, 216);
                        break;
                    case ModelPlayer.PowerUp.HYPERLASER:
                        if (this.EffectTimer.ElapsedMilliseconds < 100)
                        {
                            this.Decal.Color = new Color(255, 0, 0);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 200)
                        {
                            this.Decal.Color = new Color(0, 127, 255);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 300)
                        {
                            this.Decal.Color = new Color(255, 0, 255);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 400)
                        {
                            this.Decal.Color = new Color(0, 255, 0);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 500)
                        {
                            this.Decal.Color = new Color(0, 255, 255);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 600)
                        {
                            this.Decal.Color = new Color(0, 0, 255);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 700)
                        {
                            this.Decal.Color = new Color(127, 0, 255);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 800)
                        {
                            this.Decal.Color = new Color(255, 0, 255);
                        }
                        else
                        {
                            this.EffectTimer.Restart();
                        }
                        break;
                    case ModelPlayer.PowerUp.MULTILASER:
                        if (this.EffectTimer.ElapsedMilliseconds < 200)
                        {
                            this.Decal.Color = new Color(127, 0, 255);
                        }
                        else if (this.EffectTimer.ElapsedMilliseconds < 400)
                        {
                            this.Decal.Color = new Color(0, 255, 0);
                        }
                        else
                        {
                            this.EffectTimer.Restart();
                        }
                        break;
                    case ModelPlayer.PowerUp.DESTROY_ALL_SHIPS:
                        this.Decal.Color = new Color(255, 127, 0);
                        break;
                    case ModelPlayer.PowerUp.FREEZE_ALL_SHIPS:
                        this.Decal.Color = new Color(0, 255, 255);
                        break;
                    case ModelPlayer.PowerUp.SPEED_UP_GAME:
                        this.Decal.Color = new Color(0, 255, 0);
                        break;
                    case ModelPlayer.PowerUp.SLOW_MOTION:
                        this.Decal.Color = new Color(127, 0, 255);
                        break;
                }
                this.Decal.Rotation = 180;
                this.Decal.Draw(target, RenderStates.Default);
            }
            else
            {
                if (player.IsNotDead() && player.Damage > 0)
                {
                    if (player.DamageTimer.ElapsedMilliseconds < 2000)
                    {
                        if (!this.FlashTimer.IsRunning)
                        {
                            this.FlashTimer.Start();
                        }
                        else if (this.FlashTimer.ElapsedMilliseconds < 200)
                        {
                            this.Decal.Position = new Vector2f(player.Position.X, player.Position.Y);
                            this.Decal.TextureRect = new IntRect(0, 0, 18, 18);
                            this.Decal.Color = new Color(255, 255, 255);
                            this.Decal.Rotation = 180;
                            this.Decal.Draw(target, RenderStates.Default);
                        }
                        else if (this.FlashTimer.ElapsedMilliseconds > 400)
                        {
                            this.FlashTimer.Restart();
                        }
                        this.Decal.TextureRect = new IntRect(0, 0, 18, (int)((1 - player.Damage / 10) * 18));
                        this.Decal.Position = new Vector2f(player.Position.X, player.Position.Y);
                        if (player.Rank == 1)
                        {
                            this.Decal.Color = new Color(255, 0, 127);
                        }
                        this.Decal.Rotation = 180;
                        this.Decal.Draw(target, RenderStates.Default);
                    }
                    else if (this.Decal.Color.A > 0 && Game.Configs.FadeOut)
                    {
                        if (this.FlashTimer.IsRunning)
                        {
                            this.FlashTimer.Stop();
                        }
                        this.Decal.TextureRect = new IntRect(0, 0, 18, (int)((1 - player.Damage / 10) * 18));
                        this.Decal.Position = new Vector2f(player.Position.X, player.Position.Y);
                        this.Decal.Color = new Color(255, 0, 127, (byte)((double)(this.Decal.Color.A) - target.FrameDelta * 20));
                        this.Decal.Rotation = 180;
                        this.Decal.Draw(target, RenderStates.Default);
                    }
                }
            }
        }
    }
}
