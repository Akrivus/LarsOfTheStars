using LarsOfTheStars.Source.Client;
using System;
using SFML.Graphics;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelParticle : Model
    {
        public Color Color;
        public float Trajectory;
        public int Index;
        public ModelParticle(float x, float y, float rotation, Color color) : base(x, y, rotation)
        {
            Trajectory = Rotation;
            Color = color;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (IsNotDead())
            {
                Move((float)(Math.Sin(Trajectory)), (float)(Math.Cos(Trajectory)), target.FrameDelta, false);
                Rotation += target.FrameDelta;
            }
        }
    }
}
