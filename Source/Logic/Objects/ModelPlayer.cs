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
            this.MaxDamage = 10;
            this.DamageTimer.Start();
            this.Rank = rank;
            this.BuffTimer.Start();
            this.FireTimer.Start();
        }
        public override void Update(Display target)
        {
            base.Update(target);
            if (this.IsNotDead())
            {
                if (Game.IsFocused)
                {
                    if (Joystick.IsConnected((uint)(this.Rank - 1))) {
                        this.Right((int)(Joystick.GetAxisPosition((uint)(this.Rank - 1), Joystick.Axis.X) / 25), target.FrameDelta);
                        this.Right((int)(Joystick.GetAxisPosition((uint)(this.Rank - 1), Joystick.Axis.U) / 25), target.FrameDelta);
                        if (Math.Abs(Joystick.GetAxisPosition((uint)(this.Rank - 1), Joystick.Axis.Z)) > 50)
                        {
                            if (this.FireTimer.ElapsedMilliseconds - this.LastTriggerPlacement > 200 || this.FireTimer.ElapsedMilliseconds < target.FrameDelta * 16 + 1 || this.LastTriggerPlacement < 10)
                            {
                                this.FireLaser(this.Position.X - 0.5F, this.Position.Y - 16, target);
                                this.LastTriggerPlacement = this.FireTimer.ElapsedMilliseconds;
                            }
                        }
                        else
                        {
                            this.FireTimer.Restart();
                            this.LastTriggerPlacement = 0;
                        }
                    }
                    bool FireKeyPressed = Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down);
                    if (Game.Mode.IsMultiUser())
                    {
                        FireKeyPressed = this.Rank == 1 ? Keyboard.IsKeyPressed(Keyboard.Key.S) : Keyboard.IsKeyPressed(Keyboard.Key.Down);
                    }
                    bool RightKeyPressed = Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right);
                    if (Game.Mode.IsMultiUser())
                    {
                        RightKeyPressed = this.Rank == 1 ? Keyboard.IsKeyPressed(Keyboard.Key.D) : Keyboard.IsKeyPressed(Keyboard.Key.Right);
                    }
                    bool LeftKeyPressed = Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left);
                    if (Game.Mode.IsMultiUser())
                    {
                        LeftKeyPressed = this.Rank == 1 ? Keyboard.IsKeyPressed(Keyboard.Key.A) : Keyboard.IsKeyPressed(Keyboard.Key.Left);
                    }
                    if (FireKeyPressed)
                    {
                        if (this.FiringEnabled)
                        {
                            this.FireLaser(this.Position.X - 0.5F, this.Position.Y - 16, target);
                        }
                    }
                    else
                    {
                        this.FiringEnabled = true;
                    }
                    if (RightKeyPressed && this.MovementEnabled)
                    {
                        this.Right(2, target.FrameDelta);
                    }
                    else
                    if (LeftKeyPressed && this.MovementEnabled)
                    {
                        this.Left(2, target.FrameDelta);
                    }
                    else
                    {
                        this.MovementEnabled = true;
                    }
                }
                if (this.BuffTimer.ElapsedMilliseconds > 10000 && this.Buff != PowerUp.NONE)
                {
                    this.Buff = PowerUp.NONE;
                    target.SpeedFactor = 1;
                }
            }
            else if (!this.Deactivated)
            {
                Sounds.Play("death.ogg");
                this.Kill();
            }
        }
        public override bool IsDead()
        {
            return this.Damage >= this.MaxDamage;
        }
        public override void OnDeath()
        {
            if (this.Rank == 1)
            {
                Sounds.PlayRandom("p1", "end");
            }
            for (int i = 0; i < 6; ++i)
            {
                Game.AddEntity(new RenderGib(new ModelGib(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), "incinerator", i)));
                for (int j = 0; j < Game.Configs.MaxParticles / 10; ++j)
                {
                    Game.AddEntity(new RenderParticle(new ModelParticle(this.Position.X + (Game.RNG.Next(16) - 8), this.Position.Y + (Game.RNG.Next(16) - 8), Game.RNG.Next(360), Color.White)));
                }
            }
        }
        public override bool OnLaserHit(ModelLaser laser)
        {
            if (this.IsNotDead() && !laser.OwnedByPlayer)
            {
                this.DamagePlayer(1);
                return true;
            }
            return false;
        }
        public void DamagePlayer(float amount)
        {
            if (this.Buff == PowerUp.IMMUNE_TO_HITS)
            {
                Sounds.Play("boom.ogg");
            }
            else
            {
                Game.Mode.Interact(1, -1);
                if (this.DamageTimer.ElapsedMilliseconds > 500)
                {
                    this.Damage += amount;
                    this.DamageTimer.Restart();
                    Sounds.Play("boom.ogg");
                }
            }
        }
        public void FireLaser(float x, float y, Display target)
        {
            Game.AddEntity(new RenderLaser(new ModelLaser(x, y, 2.0F).SetColor(this.Buff == PowerUp.HYPERLASER ? Color.Black : Color.Green)));
            Sounds.Play("laser.ogg");
            this.FiringEnabled = false;
            if (this.Buff == PowerUp.MULTILASER)
            {
                Game.AddEntity(new RenderLaser(new ModelLaser(x, y, 2.0F * (this.Buff == PowerUp.HYPERLASER ? 2.0F : 1.0F), -0.5F, 0.5F, -45).SetColor(this.Buff == PowerUp.HYPERLASER ? Color.Black : Color.Green)));
                Game.AddEntity(new RenderLaser(new ModelLaser(x, y, 2.0F * (this.Buff == PowerUp.HYPERLASER ? 2.0F : 1.0F), 0.5F, 0.5F, 45).SetColor(this.Buff == PowerUp.HYPERLASER ? Color.Black : Color.Green)));
            }
        }
    }
}
