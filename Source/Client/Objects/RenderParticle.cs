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
            Sprite = new Sprite(Textures.Load("particles", "rocket.png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
        }
        public override void Update(Display target)
        {
            ModelParticle particle = (ModelParticle)(Base);
            Sprite.Rotation = particle.Rotation;
            if (FaderAlpha > 0 && Game.Configs.FadeOut)
            {
                Sprite.Color = new Color(particle.Color.R, particle.Color.G, particle.Color.B, (byte)(FaderAlpha));
                FaderAlpha -= target.FrameDelta * 4;
            }
            else
            {
                particle.Kill();
            }
            base.Update(target);
        }
    }
}
