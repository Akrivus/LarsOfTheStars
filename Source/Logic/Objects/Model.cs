using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class Model
    {
        public FloatRect BoundingBox;
        public Vector2f Position;
        public float Rotation;
        public bool Deactivated;
        public float MaxDamage = 1;
        public float Damage;
        public Model(float x, float y, float rotation = 0)
        {
            this.Position = new Vector2f(x, y);
            this.Rotation = rotation;
        }
        public Vector2f GetPosition()
        {
            return new Vector2f(this.Position.X, this.Position.Y);
        }
        public virtual void Update(Display target)
        {
            if (this.OutsideOfScreen())
            {
                this.Kill();
            }
        }
        public void Move(float x, float y, float delta, bool keepAlive = true)
        {
            Vector2f OldPosition = this.Position;
            this.Position = new Vector2f(this.Position.X + (x * delta), this.Position.Y + (y * delta));
            if (this.OutsideOfScreen() && keepAlive)
            {
                this.Position = OldPosition;
            }
        }
        public void Left(float x, float delta, bool keepAlive = true)
        {
            this.Move(-x, 0, delta, keepAlive);
        }
        public void Right(float x, float delta, bool keepAlive = true)
        {
            this.Move(x, 0, delta, keepAlive);
        }
        public void Kill()
        {
            if (!this.Deactivated)
            {
                if (this.OutsideOfScreen())
                {
                    this.OnDespawn();
                }
                else
                {
                    this.OnDeath();
                }
                this.Deactivated = true;
            }
        }
        public virtual void OnDespawn()
        {
            if (this.CanBeSwept())
            {
                Game.Mode.Interact(1, -2);
            }
        }
        public virtual void OnDeath()
        {

        }
        public virtual bool OnLaserHit(ModelLaser laser)
        {
            if (this.CanBeSwept())
            {
                if (laser.OwnedByPlayer)
                {
                    this.Damage += laser.FlashRainbowColors ? this.MaxDamage : 1;
                    if (this.Damage >= this.MaxDamage)
                    {
                        Game.Mode.Interact(1, 1);
                        Sounds.Play("death.ogg");
                        this.Kill();
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
            return this.Damage >= this.MaxDamage || (this.Damage >= this.MaxDamage && this.OutsideOfScreen());
        }
        public bool IsNotDead()
        {
            return !this.IsDead();
        }
        public bool IsCollidingWithPlayer1()
        {
            return this.IsCollidingWith(Game.ServerPlayer1);
        }
        public bool IsCollidingWithPlayer2()
        {
            return this.IsCollidingWith(Game.ServerPlayer2);
        }
        public bool IsCollidingWith(Model model)
        {
            if (this.GetHashCode() != model.GetHashCode() && this.BoundingBox.Left > 0)
            {
                return this.BoundingBox.Intersects(model.BoundingBox) && model.IsNotDead();
            }
            return false;
        }
        public bool OutsideOfScreen()
        {
            float X = this.Position.X;
            float Y = this.Position.Y;
            return (X > 256) || (X < 0) 
                || (Y > 192) || (Y < 0);
        }
    }
}
