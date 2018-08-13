using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using System;
using SFML.Graphics;
using SFML.Window;


namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelDiver : Model
    {
        private float Buffer = 100;
        private bool DiveRight = false;
        public Vector2f Destination;
        public Vector2f Start;
        public Vector2f LastPosition = new Vector2f(0, 0);
        public ModelDiver(float x = 0, float y = 0) : base(x, y, Game.RNG.Next(-45, 45))
        {
            if (Game.Mode.IsMultiUser())
            {
                if (Game.RNG.Next(2) == 0)
                {
                    this.Destination = new Vector2f(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y + Game.RNG.Next(16) - 8);
                }
                else
                {
                    this.Destination = new Vector2f(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y + Game.RNG.Next(16) - 8);
                }
            }
            else
            {
                this.Destination = new Vector2f(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y + Game.RNG.Next(16) - 8);
            }
            this.DiveRight = this.Destination.X < 128;
            this.Buffer = Math.Max(40, Game.RNG.Next(256 - (int)(this.Destination.X)));
            this.Position = new Vector2f(this.GeneratePosition(), 0);
            if (Math.Abs(this.Destination.X - this.Position.X) < 40)
            {
                this.Buffer = Math.Max(40, Game.RNG.Next(256 - (int)(this.Destination.X)));
                this.GeneratePosition();
            }
            this.Start = this.Position;
        }
        private float GeneratePosition()
        {
            float NewX = 0;
            if (this.DiveRight)
            {
                NewX = this.Destination.X + this.Buffer;
            }
            else
            {
                NewX = this.Destination.X - this.Buffer;
            }
            return NewX;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (this.IsNotDead() && Game.ServerPlayer1.Buff != ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                float NewX = this.Position.X + (target.FrameDelta * 0.5F) * (this.DiveRight ? -1 : 1);
                float NewY = -(this.Destination.Y) * (NewX - (this.Destination.X + this.Buffer)) * (NewX - (this.Destination.X - this.Buffer)); NewY = NewY / (this.Buffer * this.Buffer);
                if (NewY - this.LastPosition.Y > 96)
                {
                    this.Deactivated = true;
                }
                else
                {
                    this.LastPosition = this.Position;
                }
                this.Position = new Vector2f(NewX, NewY);
                this.Rotation = (float)(Math.Atan2(-2 * this.Destination.Y * (NewX - this.Destination.X) / (this.Buffer * this.Buffer), 1)) * 57.2957795131F;
                this.Rotation = this.Rotation + (this.DiveRight ? 90 : 270);
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
        public override void OnDeath()
        {
            if (!this.OutsideOfScreen())
            {
                for (int i = 0; i < 4; ++i)
                {
                    Game.AddEntity(new RenderGib(new ModelGib(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), "diver", i)));
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
