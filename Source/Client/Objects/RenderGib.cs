using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderGib : Render
    {
        public float FaderAlpha = 255;
        public Color Color = Color.White;
        public RenderGib(ModelGib model) : base(model)
        {
            Sprite = new Sprite(Textures.Load("gibs", model.Item + "_gib_" + model.Index + ".png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
            Sprite.Scale = new Vector2f(2, 2);
        }
        public override void Update(Display target)
        {
            if (FaderAlpha > 0 && Game.Configs.FadeOut)
            {
                Sprite.Color = new Color(Color.R, Color.G, Color.B, (byte)(FaderAlpha));
                FaderAlpha -= target.FrameDelta * 4;
            }
            else
            {
                Base.Kill();
            }
            base.Update(target);
        }
    }
}
