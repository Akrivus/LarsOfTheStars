using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderDiver : Render
    {
        public RenderDiver(ModelDiver model) : base(model)
        {
            this.Sprite = new Sprite(Textures.Load("enemies", "diver.png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
            this.Sprite.Scale = new Vector2f(2, 2);
        }
    }
}
