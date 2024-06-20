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
            Trajectory = Rotation;
            Item = item;
            Index = index;
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
