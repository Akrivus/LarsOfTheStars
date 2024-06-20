using LarsOfTheStars.Source.Files;
using LarsOfTheStars.Source.Logic.Objects;
using SFML.Graphics;
using SFML.Window;
using System;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderIndicator : Render
    {
        public float FaderAlpha = 255;
        public Text Text = new Text("Bingo bongo!", Fonts.Load("start2p.ttf"), 30);
        public RenderIndicator(ModelIndicator model) : base(model)
        {
            Text.Scale = new Vector2f(0.1F, 0.1F);
            Text.Origin = new Vector2f(Text.GetLocalBounds().Width / 2, Text.GetLocalBounds().Height / 2);
            Text.DisplayedString = model.Text;
        }
        public override void Update(Display target)
        {
            ModelIndicator indicator = (ModelIndicator)(Base);
            indicator.BoundingBox = Text.GetGlobalBounds();
            indicator.Update(target);
            if (indicator.IsNotDead())
            {
                Text.Position = indicator.Position;
                if (FaderAlpha > 0 && Game.Configs.FadeOut)
                {
                    Text.Color = new Color(indicator.Color.R, indicator.Color.G, indicator.Color.B, (byte)(FaderAlpha));
                    FaderAlpha -= target.FrameDelta * 4;
                }
                else
                {
                    indicator.Kill();
                }
                Text.Draw(target, RenderStates.Default);
            }
        }
    }
}
