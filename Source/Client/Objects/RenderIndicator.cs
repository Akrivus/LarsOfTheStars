using LarsOfTheStars.Source.Logic.Objects;
using SFML.Graphics;
using SFML.Window;
using System;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderIndicator : Render
    {
        public float FaderAlpha = 255;
        public Text Text = new Text("Bingo bongo!", new Font(Environment.CurrentDirectory + "/assets/textures/fonts/start2p.ttf"), 30);
        public RenderIndicator(ModelIndicator model) : base(model)
        {
            this.Text.Scale = new Vector2f(0.1F, 0.1F);
            this.Text.Origin = new Vector2f(this.Text.GetLocalBounds().Width / 2, this.Text.GetLocalBounds().Height / 2);
            this.Text.DisplayedString = model.Text;
        }
        public override void Update(Display target)
        {
            ModelIndicator indicator = (ModelIndicator)(this.Base);
            indicator.BoundingBox = this.Text.GetGlobalBounds();
            indicator.Update(target);
            if (indicator.IsNotDead())
            {
                this.Text.Position = indicator.Position;
                if (this.FaderAlpha > 0 && Game.Configs.FadeOut)
                {
                    this.Text.Color = new Color(indicator.Color.R, indicator.Color.G, indicator.Color.B, (byte)(this.FaderAlpha));
                    this.FaderAlpha -= target.FrameDelta * 4;
                }
                else
                {
                    indicator.Kill();
                }
                this.Text.Draw(target, RenderStates.Default);
            }
        }
    }
}
