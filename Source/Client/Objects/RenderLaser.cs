using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderLaser : Render
    {
        public RenderLaser(ModelLaser model) : base(model)
        {
            Sprite = new Sprite(Textures.Load("laser.png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
            Sprite.Color = model.Color;
            Sprite.Scale = new Vector2f(2, 2);
        }
        public override void Update(Display target)
        {
            ModelLaser laser = ((ModelLaser) Base);
            if (laser.FlashRainbowColors)
            {
                Sprite.Color = laser.Color;
            }
            base.Update(target);
        }
    }
}
