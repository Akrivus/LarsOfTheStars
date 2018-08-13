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
            this.Trajectory = this.Rotation;
            this.Color = color;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (this.IsNotDead())
            {
                this.Move((float)(Math.Sin(this.Trajectory)), (float)(Math.Cos(this.Trajectory)), target.FrameDelta, false);
                this.Rotation += target.FrameDelta;
            }
        }
    }
}
