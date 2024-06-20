using LarsOfTheStars.Source.Client;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelLaser : Model
    {
        public bool OwnedByPlayer = true;
        public int Rank = 1;
        public Color Color = Color.Black;
        public bool FlashRainbowColors = false;
        public float RainbowIndex = 0;
        public float XTrajectory = 0;
        public float YTrajectory = 0;
        public float Velocity = 2F;
        public ModelLaser(float x, float y, float v, float xT = 0, float yT = 1, float rotation = 0) : base(x, y, rotation)
        {
            Color = Color.Black;
            Velocity = v;
            XTrajectory = xT;
            YTrajectory = yT;
        }
        public ModelLaser SetColor(Color color)
        {
            Color = color;
            if ((Color.R + Color.G + Color.B) == 0)
            {
                FlashRainbowColors = true;
                Velocity *= 1.5F;
            }
            return this;
        }
        public ModelLaser CreatedByNPC()
        {
            OwnedByPlayer = false;
            return this;
        }
        public ModelLaser SetRank(int rank)
        {
            Rank = rank;
            return this;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (IsNotDead())
            {
                float NewX = Position.X + Velocity * XTrajectory * target.FrameDelta;
                float NewY = Position.Y - Velocity * YTrajectory * target.FrameDelta;
                Position = new Vector2f(NewX, NewY);
                if (Position.X + Position.Y != 0)
                {
                    for (int i = 0; i < Game.ServerEntities.Count; ++i)
                    {
                        Model model = Game.ServerEntities[i];
                        if (model.GetType() != typeof(ModelLaser) && model.IsCollidingWith(this))
                        {
                            if (model.OnLaserHit(this))
                            {
                                Kill();
                                return;
                            }
                        }
                    }
                    if (Game.ServerPlayer1.IsCollidingWith(this))
                    {
                        if (Game.ServerPlayer1.OnLaserHit(this))
                        {
                            Kill();
                            return;
                        }
                    }
                    if (Game.ServerPlayer2.IsCollidingWith(this))
                    {
                        if (Game.ServerPlayer2.OnLaserHit(this))
                        {
                            Kill();
                            return;
                        }
                    }
                }
                if (FlashRainbowColors || (Color.R + Color.G + Color.B) == 0)
                {
                    float convolve = RainbowIndex % 1 * 6;
                    byte ascending = (byte)(convolve % 1 * 255);
                    byte descending = (byte)(255 - ascending);
                    switch ((int)(convolve))
                    {
                        case 0:
                            Color = new Color(255, ascending, 0);
                            break;
                        case 1:
                            Color = new Color(descending, 255, 0);
                            break;
                        case 2:
                            Color = new Color(0, 255, ascending);
                            break;
                        case 3:
                            Color = new Color(0, descending, 255);
                            break;
                        case 4:
                            Color = new Color(ascending, 0, 255);
                            break;
                        default:
                            Color = new Color(255, 0, descending);
                            break;
                    }
                    FlashRainbowColors = true;
                    RainbowIndex += target.FrameDelta / 75;
                    if (RainbowIndex > 360)
                    {
                        RainbowIndex = 0;
                    }
                }
                else
                {
                    RainbowIndex = 0;
                }
            }
        }
    }
}
