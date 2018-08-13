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
            this.Sprite = new Sprite(Textures.Load("laser.png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
            this.Sprite.Color = model.Color;
            this.Sprite.Scale = new Vector2f(2, 2);
        }
        public override void Update(Display target)
        {
            ModelLaser laser = ((ModelLaser) this.Base);
            if (laser.FlashRainbowColors)
            {
                this.Sprite.Color = laser.Color;
            }
            base.Update(target);
        }
    }
}
