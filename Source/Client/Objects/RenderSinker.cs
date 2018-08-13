using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderSinker : Render
    {
        public RenderSinker(ModelSinker model) : base(model)
        {
            this.Sprite = new Sprite(Textures.Load("enemies", "sinker.png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
            this.Sprite.Scale = new Vector2f(2, 2);
        }
    }
}
