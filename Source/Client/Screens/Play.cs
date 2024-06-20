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
                Stars[i] = new Sprite(Textures.Load("star.png"));
                Stars[i].Position = new Vector2f(Game.RNG.Next(0, 256), Game.RNG.Next(0, 192));
                Stars[i].Color = new Color(255, 255, 255, (byte)(Game.RNG.Next(255)));
                Stars[i].Scale = new Vector2f(starSize, starSize);
            }
        }
        public void Render(Display target)
        {
            for (int i = 0; i < MAX_STARS; ++i)
            {
                Stars[i].Position = new Vector2f(Stars[i].Position.X, Stars[i].Position.Y + target.FrameDelta);
                if (Stars[i].Position.Y > 200)
                {
                    Stars[i].Position = new Vector2f(Stars[i].Position.X, Game.RNG.Next(12) * -1);
                }
                Stars[i].Draw(target, RenderStates.Default);
            }
        }
    }
}
