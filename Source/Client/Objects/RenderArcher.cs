using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderArcher : Render
    {
        private Sprite Gunner;
        public RenderArcher(ModelArcher model) : base(model)
        {
            this.Gunner = new Sprite(Textures.Load("enemies", "archer_1.png"));
            this.Gunner.Origin = new Vector2f(this.Gunner.Texture.Size.X / 2, this.Gunner.Texture.Size.Y / 2);
            this.Gunner.Scale = new Vector2f(2, 2);
            this.Sprite = new Sprite(Textures.Load("enemies", "archer_0.png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
            this.Sprite.Scale = new Vector2f(2, 2);
        }
        public override void Update(Display target)
        {
            this.Gunner.Position = this.Base.Position;
            this.Gunner.Rotation = 0;
            this.Gunner.Draw(target, RenderStates.Default);
            this.Sprite.Rotation = this.Base.Rotation;
            base.Update(target);
        }
    }
}
