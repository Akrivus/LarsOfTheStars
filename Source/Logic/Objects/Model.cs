using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class Model
    {
        public Guid GlobalID = Guid.NewGuid();
        public Stopwatch LifeTimer = new Stopwatch();
        public FloatRect BoundingBox;
        public Vector2f Position;
        public float Rotation;
        public bool Deactivated;
        public float MaxDamage = 1;
        public float Damage;
        public Model(float x, float y, float rotation = 0)
        {
            LifeTimer.Start();
            Position = new Vector2f(x, y);
            Rotation = rotation;
        }
        public Vector2f GetPosition()
        {
            return new Vector2f(Position.X, Position.Y);
        }
        public virtual void Update(Display target)
        {
            if (OutsideOfScreen())
            {
                Kill();
            }
        }
        public void Move(float x, float y, float delta, bool keepAlive = true)
        {
            Vector2f OldPosition = Position;
            Position = new Vector2f(Position.X + (x * delta), Position.Y + (y * delta));
            if (OutsideOfScreen() && keepAlive)
            {
                Position = OldPosition;
            }
        }
        public void Left(float x, float delta, bool keepAlive = true)
        {
            Move(-x, 0, delta, keepAlive);
        }
        public void Right(float x, float delta, bool keepAlive = true)
        {
            Move(x, 0, delta, keepAlive);
        }
        public void Kill()
        {
            if (!Deactivated)
            {
                if (OutsideOfScreen())
                {
                    OnDespawn();
                }
                else
                {
                    OnDeath();
                }
                Deactivated = true;
            }
        }
        public virtual void OnDespawn()
        {
            if (CanBeSwept())
            {
                Game.Mode.Interact(1, -2);
                Game.Mode.Interact(2, -2);
            }
        }
        public virtual void OnDeath()
        {

        }
        public virtual bool OnLaserHit(ModelLaser laser)
        {
            if (CanBeSwept())
            {
                if (laser.OwnedByPlayer)
                {
                    Damage += laser.FlashRainbowColors ? 3 : 1;
                    if (Damage >= MaxDamage)
                    {
                        Game.Mode.Interact(laser.Rank, 1);
                        Sounds.Play("death.ogg");
                        Kill();
                    }
                    else
                    {
                        Sounds.Play("boom.ogg");
                        for (int j = 0; j < Game.Configs.MaxParticles / 2; ++j)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(laser.Position.X + (Game.RNG.Next(16) - 8), laser.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), laser.Color)));
                        }
                    }
                    return !laser.FlashRainbowColors;
                }
            }
            return false;
        }
        public virtual bool CanBeSwept()
        {
            return false;
        }
        public virtual bool IsDead()
        {
            return Damage >= MaxDamage || (Damage >= MaxDamage && OutsideOfScreen());
        }
        public bool IsNotDead()
        {
            return !IsDead();
        }
        public bool IsCollidingWithPlayer1()
        {
            return IsCollidingWith(Game.ServerPlayer1);
        }
        public bool IsCollidingWithPlayer2()
        {
            return IsCollidingWith(Game.ServerPlayer2);
        }
        public bool IsCollidingWith(Model model)
        {
            if (GetHashCode() != model.GetHashCode() && BoundingBox.Left > 0)
            {
                return BoundingBox.Intersects(model.BoundingBox) && model.IsNotDead();
            }
            return false;
        }
        public virtual bool OutsideOfScreen()
        {
            float X = Position.X;
            float Y = Position.Y;
            return (X > 256) || (X < 0) 
                || (Y > 192) || (Y < 0);
        }
    }
}
