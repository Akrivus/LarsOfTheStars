using LarsOfTheStars.Source.Client;
using System;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelGib : Model
    {
        public float Trajectory;
        public string Item;
        public int Index;
        public ModelGib(float x, float y, string item, int index) : base(x, y, Game.RNG.Next(360))
        {
            this.Trajectory = this.Rotation;
            this.Item = item;
            this.Index = index;
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
