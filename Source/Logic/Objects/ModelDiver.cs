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
            MaxDamage = 1 * (Game.Mode.Level / 20 + 1);
            if (Game.Mode.IsMultiUser())
            {
                if (Game.RNG.Next(2) == 0)
                {
                    Destination = new Vector2f(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y + Game.RNG.Next(16) - 8);
                }
                else
                {
                    Destination = new Vector2f(Game.ServerPlayer2.Position.X, Game.ServerPlayer2.Position.Y + Game.RNG.Next(16) - 8);
                }
            }
            else
            {
                Destination = new Vector2f(Game.ServerPlayer1.Position.X, Game.ServerPlayer1.Position.Y + Game.RNG.Next(16) - 8);
            }
            DiveRight = Destination.X < 128;
            Buffer = Math.Max(40, Game.RNG.Next(256 - (int)(Destination.X)));
            Position = new Vector2f(GeneratePosition(), 0);
            if (Math.Abs(Destination.X - Position.X) < 40)
            {
                Buffer = Math.Max(40, Game.RNG.Next(256 - (int)(Destination.X)));
                GeneratePosition();
            }
            Start = Position;
        }
        private float GeneratePosition()
        {
            float NewX = 0;
            if (DiveRight)
            {
                NewX = Destination.X + Buffer;
            }
            else
            {
                NewX = Destination.X - Buffer;
            }
            return NewX;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (IsNotDead() && Game.ServerPlayer1.Buff != ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                float NewX = Position.X + (target.FrameDelta * 0.5F) * (DiveRight ? -1 : 1);
                float NewY = -(Destination.Y) * (NewX - (Destination.X + Buffer)) * (NewX - (Destination.X - Buffer)); NewY = NewY / (Buffer * Buffer);
                if (NewY - LastPosition.Y > 96)
                {
                    Deactivated = true;
                }
                else
                {
                    LastPosition = Position;
                }
                Position = new Vector2f(NewX, NewY);
                Rotation = (float)(Math.Atan2(-2 * Destination.Y * (NewX - Destination.X) / (Buffer * Buffer), 1)) * 57.2957795131F;
                Rotation = Rotation + (DiveRight ? 90 : 270);
                if (IsCollidingWithPlayer1())
                {
                    Game.ServerPlayer1.DamagePlayer(1);
                    Sounds.Play("boom.ogg");
                    Kill();
                }
                if (IsCollidingWithPlayer2())
                {
                    Game.ServerPlayer2.DamagePlayer(1);
                    Sounds.Play("boom.ogg");
                    Kill();
                }
            }
        }
        public override void OnDeath()
        {
            if (!OutsideOfScreen())
            {
                for (int i = 0; i < 4; ++i)
                {
                    Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "diver", i)));
                    for (int j = 0; j < Game.Configs.MaxParticles / 10; ++j)
                    {
                        Game.AddEntity(new RenderParticle(new ModelParticle(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                    }
                }
            }
        }
        public override bool CanBeSwept()
        {
            return !Deactivated;
        }
    }
}
