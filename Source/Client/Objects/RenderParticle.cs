using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderParticle : Render
    {
        public float FaderAlpha = 255;
        public RenderParticle(ModelParticle model) : base(model)
        {
            this.Sprite = new Sprite(Textures.Load("particles", "rocket.png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
        }
        public override void Update(Display target)
        {
            ModelParticle particle = (ModelParticle)(this.Base);
            this.Sprite.Rotation = particle.Rotation;
            if (this.FaderAlpha > 0 && Game.Configs.FadeOut)
            {
                this.Sprite.Color = new Color(particle.Color.R, particle.Color.G, particle.Color.B, (byte)(this.FaderAlpha));
                this.FaderAlpha -= target.FrameDelta * 4;
            }
            else
            {
                particle.Kill();
            }
            base.Update(target);
        }
    }
}
