using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Files;
using LarsOfTheStars.Source.Integration;
using System.Diagnostics;
using System;
using SFML.Graphics;
using SFML.Window;

namespace LarsOfTheStars.Source.Logic.Objects
{
    public class ModelPlayer : Model
    {
        public enum PowerUp
        {
            NONE,
            IMMUNE_TO_HITS,
            HYPERLASER,
            MULTILASER,
            DESTROY_ALL_SHIPS,
            FREEZE_ALL_SHIPS,
            SPEED_UP_GAME,
            SLOW_MOTION
        }
        public Stopwatch DamageTimer = new Stopwatch();
        public Stopwatch FireTimer = new Stopwatch();
        public Stopwatch BuffTimer = new Stopwatch();
        public float LastTriggerPlacement = 0;
        public bool MovementEnabled;
        public bool FiringEnabled;
        public PowerUp Buff = PowerUp.NONE;
        public int Rank;
        public ModelPlayer(int rank) : base(128, 144, 0)
        {
            MaxDamage = 10;
            DamageTimer.Start();
            Rank = rank;
            BuffTimer.Start();
            FireTimer.Start();
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (IsNotDead())
            {
                if (Game.IsFocused)
                {
                    if (Game.Configs.JoystickEnabled && Joystick.IsConnected((uint)(Rank - 1)))
                    {
                        Right((int)(Joystick.GetAxisPosition((uint)(Rank - 1), Joystick.Axis.X) / 25), target.FrameDelta);
                        Right((int)(Joystick.GetAxisPosition((uint)(Rank - 1), Joystick.Axis.U) / 25), target.FrameDelta);
                        if (Math.Abs(Joystick.GetAxisPosition((uint)(Rank - 1), Joystick.Axis.Z)) > 50)
                        {
                            if (FireTimer.ElapsedMilliseconds - LastTriggerPlacement > 200 || FireTimer.ElapsedMilliseconds < target.FrameDelta * 16 + 1 || LastTriggerPlacement < 10)
                            {
                                FireLaser(Position.X - 0.5F, Position.Y - 16, target);
                                LastTriggerPlacement = FireTimer.ElapsedMilliseconds;
                            }
                        }
                        else
                        {
                            FireTimer.Restart();
                            LastTriggerPlacement = 0;
                        }
                    }
                    bool FireKeyPressed = Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down);
                    if (Game.Mode.IsMultiUser())
                    {
                        FireKeyPressed = Rank == 1 ? Keyboard.IsKeyPressed(Keyboard.Key.S) : Keyboard.IsKeyPressed(Keyboard.Key.Down);
                    }
                    bool RightKeyPressed = Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right);
                    if (Game.Mode.IsMultiUser())
                    {
                        RightKeyPressed = Rank == 1 ? Keyboard.IsKeyPressed(Keyboard.Key.D) : Keyboard.IsKeyPressed(Keyboard.Key.Right);
                    }
                    bool LeftKeyPressed = Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left);
                    if (Game.Mode.IsMultiUser())
                    {
                        LeftKeyPressed = Rank == 1 ? Keyboard.IsKeyPressed(Keyboard.Key.A) : Keyboard.IsKeyPressed(Keyboard.Key.Left);
                    }
                    if (FireKeyPressed)
                    {
                        if (FiringEnabled)
                        {
                            FireLaser(Position.X - 0.5F, Position.Y - 16, target);
                        }
                    }
                    else
                    {
                        FiringEnabled = true;
                    }
                    if (RightKeyPressed && MovementEnabled)
                    {
                        Right(3, target.FrameDelta);
                    }
                    else
                    if (LeftKeyPressed && MovementEnabled)
                    {
                        Left(3, target.FrameDelta);
                    }
                    else
                    {
                        MovementEnabled = true;
                    }
                }
                if (BuffTimer.ElapsedMilliseconds > 2000 && Buff != PowerUp.NONE)
                {
                    Buff = PowerUp.NONE;
                    target.SpeedFactor = 1;
                }
            }
            else if (!Deactivated)
            {
                Sounds.Play("death.ogg");
                Kill();
            }
        }
        public override bool IsDead()
        {
            return Damage >= MaxDamage;
        }
        public override void OnDeath()
        {
            Sounds.Play("fail.ogg");
            for (int i = 0; i < 6; ++i)
            {
                Game.AddEntity(new RenderGib(new ModelGib(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), "incinerator", i)));
                for (int j = 0; j < Game.Configs.MaxParticles / 10; ++j)
                {
                    Game.AddEntity(new RenderParticle(new ModelParticle(Position.X + (Game.RNG.Next(16) - 8), Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                }
            }
        }
        public override bool OnLaserHit(ModelLaser laser)
        {
            if (IsNotDead() && !laser.OwnedByPlayer)
            {
                DamagePlayer(1);
                return true;
            }
            return false;
        }
        public void DamagePlayer(float amount)
        {
            if (Buff == PowerUp.HYPERLASER)
            {
                Sounds.Play("boom.ogg");
            }
            else
            {
                if (DamageTimer.ElapsedMilliseconds > 500)
                {
                    Damage += amount;
                    DamageTimer.Restart();
                    Sounds.Play("boom.ogg");
                }
            }
        }
        public void FireLaser(float x, float y, Display target)
        {
            Game.AddEntity(new RenderLaser(new ModelLaser(x, y, 4.0F).SetColor(Buff == PowerUp.HYPERLASER ? Color.Black : (Rank == 1 ? Color.Green : Color.Magenta)).SetRank(Rank)));
            Sounds.Play("laser.ogg");
            FiringEnabled = false;
            if (Buff == PowerUp.MULTILASER)
            {
                Game.AddEntity(new RenderLaser(new ModelLaser(x, y, 4.0F, -0.5F, 0.5F, -45).SetColor(Buff == PowerUp.HYPERLASER ? Color.Black : (Rank == 1 ? Color.Green : Color.Magenta)).SetRank(Rank)));
                Game.AddEntity(new RenderLaser(new ModelLaser(x, y, 4.0F, 0.5F, 0.5F, 45).SetColor(Buff == PowerUp.HYPERLASER ? Color.Black : (Rank == 1 ? Color.Green : Color.Magenta)).SetRank(Rank)));
            }
        }
        public override bool OutsideOfScreen()
        {
            return base.OutsideOfScreen() && (Position.X < 32 || Position.X > 224);
        }
    }
}
