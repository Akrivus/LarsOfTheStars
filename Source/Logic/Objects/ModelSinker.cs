using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelSinker : Model
    {
        public ModelSinker(float x = 0, float y = 0) : base(x, y, 0)
        {
            this.Position = new Vector2f(Game.RNG.Next(24, 232), 0);
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (this.IsNotDead() && Game.ServerPlayer1.Buff != ModelPlayer.PowerUp.FREEZE_ALL_SHIPS)
            {
                float NewY = this.Position.Y + (target.FrameDelta * 0.5F / Game.Configs.Difficulty);
                this.Position = new Vector2f(this.Position.X, NewY);
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
            for (int i = 0; i < 4; ++i)
            {
                Game.AddEntity(new RenderGib(new ModelGib(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), "sinker", i)));
                for (int j = 0; j < Game.Configs.MaxParticles / 10; ++j)
                {
                    Game.AddEntity(new RenderParticle(new ModelParticle(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                }
            }
        }
        public override bool CanBeSwept()
        {
            return !this.Deactivated;
        }
    }
}
