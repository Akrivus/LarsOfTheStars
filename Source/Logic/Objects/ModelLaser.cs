using LarsOfTheStars.Source.Client;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelLaser : Model
    {
        public bool OwnedByPlayer = true;
        public Color Color = Color.Black;
        public bool FlashRainbowColors = false;
        public float RainbowIndex = 0;
        public float XTrajectory = 0;
        public float YTrajectory = 0;
        public float Velocity = 2F;
        public ModelLaser(float x, float y, float v, float xT = 0, float yT = 1, float rotation = 0) : base(x, y, rotation)
        {
            this.Color = Color.Black;
            this.Velocity = v;
            this.XTrajectory = xT;
            this.YTrajectory = yT;
        }
        public ModelLaser SetColor(Color color)
        {
            this.Color = color;
            if ((this.Color.R + this.Color.G + this.Color.B) == 0)
            {
                this.FlashRainbowColors = true;
            }
            return this;
        }
        public ModelLaser CreatedByNPC()
        {
            this.OwnedByPlayer = false;
            return this;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (this.IsNotDead())
            {
                float NewX = this.Position.X + this.Velocity * this.XTrajectory * target.FrameDelta;
                float NewY = this.Position.Y - this.Velocity * this.YTrajectory * target.FrameDelta;
                this.Position = new Vector2f(NewX, NewY);
                if (this.Position.X + this.Position.Y != 0)
                {
                    for (int i = 0; i < Game.ServerEntities.Count; ++i)
                    {
                        Model model = Game.ServerEntities[i];
                        if (model.GetType() != typeof(ModelLaser) && model.IsCollidingWith(this))
                        {
                            if (model.OnLaserHit(this))
                            {
                                this.Kill();
                            }
                        }
                    }
                    if (Game.ServerPlayer1.IsCollidingWith(this))
                    {
                        if (Game.ServerPlayer1.OnLaserHit(this))
                        {
                            this.Kill();
                        }
                    }
                    if (Game.ServerPlayer2.IsCollidingWith(this))
                    {
                        if (Game.ServerPlayer2.OnLaserHit(this))
                        {
                            this.Kill();
                        }
                    }
                }
                if (this.FlashRainbowColors || (this.Color.R + this.Color.G + this.Color.B) == 0)
                {
                    float convolve = this.RainbowIndex % 1 * 6;
                    byte ascending = (byte)(convolve % 1 * 255);
                    byte descending = (byte)(255 - ascending);
                    switch ((int)(convolve))
                    {
                        case 0:
                            this.Color = new Color(255, ascending, 0);
                            break;
                        case 1:
                            this.Color = new Color(descending, 255, 0);
                            break;
                        case 2:
                            this.Color = new Color(0, 255, ascending);
                            break;
                        case 3:
                            this.Color = new Color(0, descending, 255);
                            break;
                        case 4:
                            this.Color = new Color(ascending, 0, 255);
                            break;
                        default:
                            this.Color = new Color(255, 0, descending);
                            break;
                    }
                    this.FlashRainbowColors = true;
                    this.RainbowIndex += target.FrameDelta / 75;
                    if (this.RainbowIndex > 360)
                    {
                        this.RainbowIndex = 0;
                    }
                }
                else
                {
                    this.RainbowIndex = 0;
                }
            }
        }
    }
}
