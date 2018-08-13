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
            this.Sprite = new Sprite(Textures.Load("gibs", model.Item + "_gib_" + model.Index + ".png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
        }
        public override void Update(Display target)
        {
            if (this.FaderAlpha > 0 && Game.Configs.FadeOut)
            {
                this.Sprite.Color = new Color(this.Color.R, this.Color.G, this.Color.B, (byte)(this.FaderAlpha));
                this.FaderAlpha -= target.FrameDelta * 4;
            }
            else
            {
                this.Base.Kill();
            }
            base.Update(target);
        }
    }
}
