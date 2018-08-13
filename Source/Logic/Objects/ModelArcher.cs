using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelArcher : Model
    {
        private float Buffer = 100;
        private float SquatPoint = 0;
        private bool SweepRight = false;
        private bool Squatting = false;
        public ModelPlayer Destination;
        public Vector2f Start;
        public Vector2f LastPosition = new Vector2f(0, 0);
        public Stopwatch FireTimer = new Stopwatch();
        public ModelArcher(float x = 0, float y = 0) : base(x, y, 0)
        {
            if (Game.Mode.IsMultiUser())
            {
                if (Game.RNG.Next(2) == 0)
                {
                    this.Destination = Game.ServerPlayer1;
                }
                else
                {
                    this.Destination = Game.ServerPlayer2;
                }
            }
            else
            {
                this.Destination = Game.ServerPlayer1;
            }
            this.SweepRight = this.Destination.Position.X < 128;
            this.Position = new Vector2f(this.SweepRight ? Game.RNG.Next(28, 128) : Game.RNG.Next(128, 228), 0);
            this.Buffer = this.Position.X;
            this.Start = this.Position;
            this.SquatPoint = Game.RNG.Next(24, 120);
            this.FireTimer.Start();
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (this.IsNotDead() && this.Destination.IsNotDead() && Game.ServerPlayer1.Buff != ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                this.Rotation = (float)(Math.Atan2(this.Position.Y - this.Destination.Position.Y, this.Start.X - this.Destination.Position.X) * 57.2957795131) + 270;
                if (this.FireTimer.ElapsedMilliseconds > (this.Squatting ? 100 * Game.Configs.Difficulty : 500 * Game.Configs.Difficulty))
                {
                    this.FireTimer.Restart();
                    float velX = (float)(Math.Sin(0.01744444444 * this.Rotation));
                    float velY = (float)(Math.Cos(0.01744444444 * this.Rotation));
                    Game.AddEntity(new RenderLaser(new ModelLaser(this.Position.X + (velX * 2), this.Position.Y + (velY * 2), 2.0F, velX, velY, this.Rotation).CreatedByNPC().SetColor(Color.Yellow)));
                    Sounds.Play("laser.ogg");
                }
                if (!this.Squatting)
                {
                    float NewY = this.Position.Y + (target.FrameDelta / 3);
                    if (this.Position.X > 228 && this.SweepRight)
                    {
                        this.SweepRight = false;
                    }
                    else if (this.Position.X < 28 && !this.SweepRight)
                    {
                        this.SweepRight = true;
                    }
                    float NewX = this.Position.X + (target.FrameDelta / 3) * (this.SweepRight ? 1 : -1);
                    this.Position = new Vector2f(NewX, NewY);
                    if (this.Position.Y > this.SquatPoint && this.Position.X > 32 && this.Position.X < 96)
                    {
                        this.Squatting = true;
                    }
                    if (this.IsCollidingWithPlayer1())
                    {
                        Game.ServerPlayer1.DamagePlayer(1);
                        Sounds.Play("boom.ogg");
                        this.Kill();
                    }
                    if (this.IsCollidingWithPlayer2())
                    {
                        Game.ServerPlayer2.DamagePlayer(1);
                        Sounds.Play("boom.ogg");
                        this.Kill();
                    }
                }
            }
        }
        public override void OnDeath()
        {
            if (!this.OutsideOfScreen())
            {
                for (int i = 0; i < 6; ++i)
                {
                    Game.AddEntity(new RenderGib(new ModelGib(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), "archer", i)));
                    for (int j = 0; j < Game.Configs.MaxParticles / 10; ++j)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                    }
                }
            }
        }
        public override bool CanBeSwept()
        {
            return !this.Deactivated;
        }
    }
}
