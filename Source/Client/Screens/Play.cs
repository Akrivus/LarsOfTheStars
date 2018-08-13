using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Client.Screens
{
    public class Play
    {
        private static int MAX_STARS = Game.Configs.MaxStars;
        private Sprite[] Stars = new Sprite[MAX_STARS];
        public Play()
        {
            for (int i = 0; i < MAX_STARS; ++i)
            {
                float starSize = (float)(Game.RNG.NextDouble() + 0.5);
                this.Stars[i] = new Sprite(Textures.Load("star.png"));
                this.Stars[i].Position = new Vector2f(Game.RNG.Next(0, 256), Game.RNG.Next(0, 192));
                this.Stars[i].Color = new Color(255, 255, 255, (byte)(Game.RNG.Next(255)));
                this.Stars[i].Scale = new Vector2f(starSize, starSize);
            }
        }
        public void Render(Display target)
        {
            for (int i = 0; i < MAX_STARS; ++i)
            {
                this.Stars[i].Position = new Vector2f(this.Stars[i].Position.X, this.Stars[i].Position.Y + target.FrameDelta);
                if (this.Stars[i].Position.Y > 200)
                {
                    this.Stars[i].Position = new Vector2f(this.Stars[i].Position.X, Game.RNG.Next(12) * -1);
                }
                this.Stars[i].Draw(target, RenderStates.Default);
            }
        }
    }
}
