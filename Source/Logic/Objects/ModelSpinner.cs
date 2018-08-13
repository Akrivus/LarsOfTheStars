using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelSpinner : Model
    {
        private float Buffer = 100;
        private bool SweepRight = false;
        public Vector2f Destination;
        public Vector2f Start;
        public Vector2f LastPosition = new Vector2f(0, 0);
        public ModelSpinner(float x = 0, float y = 0) : base(x, y, 0)
        {
            this.MaxDamage = 3;
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
            this.SweepRight = this.Destination.X < 128;
            this.Position = new Vector2f(this.SweepRight ? Game.RNG.Next(28, 128) : Game.RNG.Next(128, 228), 0);
            this.Buffer = this.Position.X;
            this.Start = this.Position;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (this.IsNotDead() && Game.ServerPlayer1.Buff != ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                float NewY = this.Position.Y + (target.FrameDelta / 2);
                if (this.Position.X > 228 && this.SweepRight)
                {
                    this.SweepRight = false;
                }
                else if (this.Position.X < 28 && !this.SweepRight)
                {
                    this.SweepRight = true;
                }
                float NewX = this.Position.X + (target.FrameDelta / 2 / Game.Configs.Difficulty) * (this.SweepRight ? 1 : -1);
                this.Position = new Vector2f(NewX, NewY);
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
                for (int i = 0; i < 6; ++i)
                {
                    Game.AddEntity(new RenderGib(new ModelGib(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), "spinner", i)));
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
