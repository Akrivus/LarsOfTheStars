using LarsOfTheStars.Source.Client;
using SFML.Graphics;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelIndicator : Model
    {
        public Color Color;
        public string Text;
        public ModelIndicator(float x, float y, string text, Color color) : base(x, y, 0)
        {
            Color = color;
            Text = text;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (IsNotDead())
            {
                Move(0, -1, target.FrameDelta, false);
            }
        }
    }
}
