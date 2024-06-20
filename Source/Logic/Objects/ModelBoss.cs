using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelBoss : Model
    {
        public enum BossState
        {
            Entrance,   // Vertical motion, immune to damage, Emerald yells at you.
            Scramble,   // Horizontal motion, immune to damage, Emerald laughs.
            Barrage,    // No motion, not immune to damage, fires lasers.
            Reset,      // No motion, not immune to damage.
            Escape      // Emerald yells at you, leaves.
        }
        public BossState State = BossState.Entrance;
        public List<Vector2f> Points = new List<Vector2f>();
        public bool HasRightGun = true;
        public bool HasLeftGun = true;
        public bool MovingLeft = true;
        public bool FiringLeft = true;
        public int Passes = 1;
        public Stopwatch ScrambleTimer = new Stopwatch();
        public Stopwatch BarrageTimer = new Stopwatch();
        public Stopwatch FireTimer = new Stopwatch();
        public Stopwatch ResetTimer = new Stopwatch();
        public ModelBoss(float x = 128, float y = -48) : base(x, y, 0)
        {
            FireTimer.Start();
            MaxDamage = 100;
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (State == BossState.Entrance)
            {
                if (Position.Y < 12)
                {
                    Move(0, 0.5F, target.FrameDelta);
                }
                else
                {
                    State = BossState.Scramble;
                    ScrambleTimer.Start();
                }
            }
            if (State == BossState.Scramble)
            {
                if (MovingLeft)
                {
                    Move(-0.3F / Passes, 0, target.FrameDelta);
                    if (Position.X < 96)
                    {
                        MovingLeft = false;
                        Damage -= 1;
                    }
                }
                else
                {
                    Move(0.3F / Passes, 0, target.FrameDelta);
                    if (Position.X > 160)
                    {
                        MovingLeft = true;
                        Damage -= 1;
                    }
                }
                if (ScrambleTimer.ElapsedMilliseconds > 6000)
                {
                    State = BossState.Barrage;
                    BarrageTimer.Start();
                }
            }
            if (State == BossState.Barrage)
            {
                if (FireTimer.ElapsedMilliseconds > 100)
                {
                    float RandomAngle = Game.RNG.Next(45);
                    float Ratio = RandomAngle / 45;
                    if (FiringLeft && HasLeftGun)
                    {
                        Game.AddEntity(new RenderLaser(new ModelLaser(Position.X - 94, Position.Y + 4, 4.0F, Ratio, -1, -RandomAngle).CreatedByNPC().SetColor(Color.Green)));
                        Sounds.Play("laser.ogg");
                        FiringLeft = false;
                    }
                    else if (HasRightGun)
                    {
                        Game.AddEntity(new RenderLaser(new ModelLaser(Position.X + 92, Position.Y + 4, 4.0F, -Ratio, -1, RandomAngle).CreatedByNPC().SetColor(Color.Green)));
                        Sounds.Play("laser.ogg");
                        FiringLeft = true;
                    }
                    FireTimer.Restart();
                }
                if (BarrageTimer.ElapsedMilliseconds > 8000)
                {
                    State = BossState.Reset;
                    ResetTimer.Start();
                }
                else if (BarrageTimer.ElapsedMilliseconds > 4000)
                {
                    if (MovingLeft)
                    {
                        Move(-0.3F / Passes, 0, target.FrameDelta);
                        if (Position.X < 96)
                        {
                            MovingLeft = false;
                            Damage -= 1;
                        }
                    }
                    else
                    {
                        Move(0.3F / Passes, 0, target.FrameDelta);
                        if (Position.X > 160)
                        {
                            MovingLeft = true;
                            Damage -= 1;
                        }
                    }
                }
            }
            if (State == BossState.Reset)
            {
                ScrambleTimer.Reset();
                BarrageTimer.Reset();
                if (ResetTimer.ElapsedMilliseconds > 400 / Passes)
                {
                    if (Position.Y > 12 * Passes)
                    {
                        State = BossState.Scramble;
                        ScrambleTimer.Start();
                        ++Passes;
                    }
                    else
                    {
                        Move(0, 0.25F, target.FrameDelta);
                        ResetTimer.Restart();
                    }
                }
            }
            if (IsCollidingWithPlayer1())
            {
                Game.ServerPlayer1.Kill();
            }
            if (IsCollidingWithPlayer2())
            {
                Game.ServerPlayer2.Kill();
            }
            if (Damage < 0)
            {
                Damage = 0;
            }
        }
        public override bool OnLaserHit(ModelLaser laser)
        {
            if (laser.OwnedByPlayer)
            {
                Vector2f pos = new Vector2f((int)(laser.Position.X - Position.X + 48), (int)(laser.Position.Y - Position.Y - 24));
                bool hitByLaser = false;
                if (HasLeftGun && Damage > 30 && new FloatRect(0, 12, 16, 16).Contains(pos.X, pos.Y))
                {
                    if (State == BossState.Reset || (!HasLeftGun && !HasRightGun))
                    {
                        HasLeftGun = false;
                        if (HasRightGun)
                        {
                            Damage += laser.FlashRainbowColors ? 80 : 30;
                        }
                        else
                        {
                            Damage += laser.FlashRainbowColors ? 8 : 3;
                        }
                        Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "boss", 0)));
                        for (int i = 0; i < Game.Configs.MaxParticles / 10; ++i)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                        }
                    }
                    hitByLaser = true;
                }
                if (HasRightGun && Damage > 30 && new FloatRect(80, 12, 16, 16).Contains(pos.X, pos.Y))
                {
                    if (State == BossState.Reset || (!HasLeftGun && !HasRightGun))
                    {
                        HasRightGun = false;
                        if (HasLeftGun)
                        {
                            Damage += laser.FlashRainbowColors ? 80 : 30;
                        }
                        else
                        {
                            Damage += laser.FlashRainbowColors ? 8 : 3;
                        }
                        Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "boss", 1)));
                        for (int i = 0; i < Game.Configs.MaxParticles / 10; ++i)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                        }
                    }
                    hitByLaser = true;
                }
                if (new FloatRect(18, 0, 60, 48).Contains(pos.X, pos.Y))
                {
                    if (State == BossState.Reset || (!HasLeftGun && !HasRightGun))
                    {
                        Damage += laser.FlashRainbowColors ? 3 : 1;
                    }
                    hitByLaser = true;
                }
                if (hitByLaser)
                {
                    if (Damage >= MaxDamage)
                    {
                        Game.Mode.Interact(laser.Rank, 100);
                        Sounds.Play("death.ogg");
                        Kill();
                    }
                    else
                    {
                        Sounds.Play("boom.ogg");
                        for (int j = 0; j < Game.Configs.MaxParticles / 2; ++j)
                        {
                            Game.AddEntity(new RenderParticle(new ModelParticle(laser.Position.X + (Game.RNG.Next(16) - 8), laser.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), laser.Color)));
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public override void OnDeath()
        {
            for (int i = 2; i < 6; ++i)
            {
                Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "boss", i)));
                for (int j = 0; j < Game.Configs.MaxParticles / 10; ++j)
                {
                    Game.AddEntity(new RenderParticle(new ModelParticle(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                }
            }
            if (HasLeftGun)
            {
                Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "boss", 0)));
            }
            if (HasRightGun)
            {
                Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "boss", 1)));
            }
        }
        public override bool CanBeSwept()
        {
            return !Deactivated;
        }
        public override bool OutsideOfScreen()
        {
            return false;
        }
    }
}
