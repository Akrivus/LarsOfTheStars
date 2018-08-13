using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderSpinner : Render
    {
        private Sprite Spinner;
        public RenderSpinner(ModelSpinner model) : base(model)
        {
            this.Spinner = new Sprite(Textures.Load("enemies", "spinner_1.png"));
            this.Spinner.Origin = new Vector2f(this.Spinner.Texture.Size.X / 2, this.Spinner.Texture.Size.Y / 2);
            this.Spinner.Scale = new Vector2f(2, 2);
            this.Sprite = new Sprite(Textures.Load("enemies", "spinner_0.png"));
            this.Sprite.Origin = new Vector2f(this.Sprite.Texture.Size.X / 2, this.Sprite.Texture.Size.Y / 2);
            this.Sprite.Scale = new Vector2f(2, 2);
        }
        public override void Update(Display target)
        {
            this.Spinner.Position = this.Base.Position;
            this.Spinner.Rotation = this.Spinner.Rotation + target.FrameDelta;
            this.Spinner.Draw(target, RenderStates.Default);
            base.Update(target);
        }
    }
}
