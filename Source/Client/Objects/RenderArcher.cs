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
            Gunner = new Sprite(Textures.Load("enemies", "archer_1.png"));
            Gunner.Origin = new Vector2f(Gunner.Texture.Size.X / 2, Gunner.Texture.Size.Y / 2);
            Gunner.Scale = new Vector2f(2, 2);
            Sprite = new Sprite(Textures.Load("enemies", "archer_0.png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
            Sprite.Scale = new Vector2f(2, 2);
        }
        public override void Update(Display target)
        {
            Gunner.Position = Base.Position;
            Gunner.Rotation = 0;
            Gunner.Draw(target, RenderStates.Default);
            Sprite.Rotation = Base.Rotation;
            base.Update(target);
        }
    }
}
