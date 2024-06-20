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
            MaxDamage = 1 + (Game.Mode.Level / 20);
            if (Game.Mode.IsMultiUser())
            {
                if (Game.RNG.Next(2) == 0)
                {
                    Destination = Game.ServerPlayer1;
                }
                else
                {
                    Destination = Game.ServerPlayer2;
                }
            }
            else
            {
                Destination = Game.ServerPlayer1;
            }
            if (Game.RNG.Next(2) > 0)
            {
                SweepRight = Destination.Position.X < 128;
            }
            else
            {
                SweepRight = Game.RNG.Next(2) == 0;
            }
            Position = new Vector2f(SweepRight ? Game.RNG.Next(28, 128) : Game.RNG.Next(128, 228), 0);
            Buffer = Position.X;
            Start = Position;
            SquatPoint = Game.RNG.Next(24, 120);
            FireTimer.Start();
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (IsNotDead() && Destination.IsNotDead() && Game.ServerPlayer1.Buff != ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                float dY = Destination.Position.Y - Position.Y;
                float dX = Destination.Position.X - Position.X;
                Rotation = (float)(Math.Atan2(dY, dX) * (180 / Math.PI)) + 90;
                if (FireTimer.ElapsedMilliseconds > (Squatting ? 50 * Game.Configs.Difficulty : 300 * Game.Configs.Difficulty))
                {
                    FireTimer.Restart();
                    float velX = (float)(Math.Sin(0.01744444444 * Rotation));
                    float velY = (float)(Math.Cos(0.01744444444 * Rotation));
                    Game.AddEntity(new RenderLaser(new ModelLaser(Position.X + (velX * 2), Position.Y + (velY * 2), 4.0F, velX, velY, Rotation).CreatedByNPC().SetColor(Color.Yellow)));
                    Sounds.Play("laser.ogg");
                }
                if (!Squatting)
                {
                    float NewY = Position.Y + (target.FrameDelta);
                    if (Position.X > 228 && SweepRight)
                    {
                        SweepRight = false;
                    }
                    else if (Position.X < 28 && !SweepRight)
                    {
                        SweepRight = true;
                    }
                    float NewX = Position.X + (target.FrameDelta) * (SweepRight ? 1 : -1);
                    Position = new Vector2f(NewX, NewY);
                    if (Position.Y > SquatPoint && Position.X > 32 && Position.X < 96)
                    {
                        Squatting = true;
                    }
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
        }
        public override void OnDeath()
        {
            if (!OutsideOfScreen())
            {
                for (int i = 0; i < 6; ++i)
                {
                    Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "archer", i)));
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
