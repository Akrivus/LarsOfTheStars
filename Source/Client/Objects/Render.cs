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
            this.Base = Model;
        }
        public virtual void Update(Display target)
        {
            this.Base.BoundingBox = this.Sprite.GetGlobalBounds();
            this.Base.Update(target);
            if (this.Base.IsNotDead())
            {
                this.Sprite.Position = new Vector2f(this.Base.Position.X, this.Base.Position.Y);
                this.Sprite.Rotation = this.Base.Rotation;
                this.Sprite.Draw(target, RenderStates.Default);
            }
        }
        public bool SafeToDelete()
        {
            return this.Base == null || this.Base.Deactivated;
        }
    }
}
