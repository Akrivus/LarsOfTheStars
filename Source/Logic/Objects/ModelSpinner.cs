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
            MaxDamage = 3 * (Game.Mode.Level / 20 + 1);
            if (Game.RNG.Next(4) < 3)
            {
                SweepRight = Game.RNG.Next(2) == 0;
            }
            else
            {
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
                SweepRight = Destination.X < 128;
            }
            Position = new Vector2f(SweepRight ? Game.RNG.Next(28, 128) : Game.RNG.Next(128, 228), 0);
            Buffer = Position.X;
            Start = Position;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (IsNotDead() && Game.ServerPlayer1.Buff != ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                float NewY = Position.Y + (target.FrameDelta / 2);
                if (Position.X > 228 && SweepRight)
                {
                    SweepRight = false;
                }
                else if (Position.X < 28 && !SweepRight)
                {
                    SweepRight = true;
                }
                float NewX = Position.X + (target.FrameDelta / 2 / Game.Configs.Difficulty) * (SweepRight ? 1 : -1);
                Position = new Vector2f(NewX, NewY);
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
                for (int i = 0; i < 6; ++i)
                {
                    Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "spinner", i)));
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
