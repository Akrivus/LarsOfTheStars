using LarsOfTheStars.Source.Files;
using LarsOfTheStars.Source.Logic.Objects;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarsOfTheStars.Source.Client.Objects
{
    public class RenderBoss : Render
    {
        public ModelBoss Boss;
        public RenderBoss(ModelBoss model) : base(model)
        {
            Sprite = new Sprite(Textures.Load("boss", "boss_0.png"));
            Sprite.Origin = new Vector2f(Sprite.Texture.Size.X / 2, Sprite.Texture.Size.Y / 2);
            Sprite.Scale = new Vector2f(2, 2);
            Boss = model;
        }
        public override void Update(Display target)
        {
            if (!Boss.HasRightGun && !Boss.HasLeftGun)
            {
                Sprite.Texture = Textures.Load("boss", "boss_3.png");
            }
            else if (!Boss.HasRightGun)
            {
                Sprite.Texture = Textures.Load("boss", "boss_2.png");
            }
            else if (!Boss.HasLeftGun)
            {
                Sprite.Texture = Textures.Load("boss", "boss_1.png");
            }
            else
            {
                Sprite.Texture = Textures.Load("boss", "boss_0.png");
            }
            base.Update(target);
        }
    }
}
