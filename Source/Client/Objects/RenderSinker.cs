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
            Sprite = new Sprite(Textures.Load("enemies", "sinker.png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
            Sprite.Scale = new Vector2f(2, 2);
        }
    }
}
