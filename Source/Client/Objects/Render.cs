using LarsOfTheStars.Source.Logic.Objects;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class Render
    {
        public Sprite Sprite;
        public Model Base;
        public Render(Model Model)
        {
            Base = Model;
        }
        public virtual void Update(Display target)
        {
            Base.BoundingBox = Sprite.GetGlobalBounds();
            Base.Update(target);
            if (Base.IsNotDead())
            {
                Sprite.Position = new Vector2f(Base.Position.X, Base.Position.Y);
                Sprite.Rotation = Base.Rotation;
                Sprite.Draw(target, RenderStates.Default);
            }
        }
        public bool SafeToDelete()
        {
            return Base == null || Base.Deactivated;
        }
    }
}
