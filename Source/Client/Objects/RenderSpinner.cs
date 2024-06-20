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
            Spinner = new Sprite(Textures.Load("enemies", "spinner_1.png"));
            Spinner.Origin = new Vector2f(Spinner.Texture.Size.X / 2, Spinner.Texture.Size.Y / 2);
            Spinner.Scale = new Vector2f(2, 2);
            Sprite = new Sprite(Textures.Load("enemies", "spinner_0.png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
            Sprite.Scale = new Vector2f(2, 2);
        }
        public override void Update(Display target)
        {
            Spinner.Position = Base.Position;
            Spinner.Rotation = Spinner.Rotation + target.FrameDelta;
            Spinner.Draw(target, RenderStates.Default);
            base.Update(target);
        }
    }
}
