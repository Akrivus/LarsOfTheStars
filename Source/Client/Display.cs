using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Client.Screens;
using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Files;
using LarsOfTheStars.Source.Integration;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace LarsOfTheStars.Source.Client
{
    public class Display : RenderWindow
    {
        public static bool Closing;
        public static Display Instance;
        public static void Show()
        {
            Game.ServerPlayer1 = new ModelPlayer(1);
            Game.ClientPlayer1 = new RenderPlayer(Game.ServerPlayer1);
            Game.ServerPlayer2 = new ModelPlayer(2);
            Game.ClientPlayer2 = new RenderPlayer(Game.ServerPlayer2);
            Instance = new Display();
        }
        private Menu StartScreen = new Menu();
        private Play GameScreen = new Play();
        private Stopwatch FrameTimer = new Stopwatch();
        private Stopwatch SpecTimer = new Stopwatch();
        private Stopwatch CallTimer = new Stopwatch();
        private bool TrailMode = false;
        private double FrameCount = 0;
        private long FrameIndex = 0;
        public float FrameDelta = 1;
        public float SpeedFactor = 1;
        public float IdealSpeedFactor = 1;
        public Display() : base(Game.Configs.Fullscreen ? VideoMode.DesktopMode : new VideoMode(Game.Configs.ScreenWidth, Game.Configs.ScreenHeight), Game.Name + " [FPS: 60 | Δ: 1.000000]", Game.Configs.Fullscreen ? Styles.Fullscreen : Game.Style)
        {
            SetIcon(24, 24, new Image(Environment.CurrentDirectory + "/assets/icon.png").Pixels);
            SetView(new View(new Vector2f(128, 96), new Vector2f(256, 192)));
            SetVerticalSyncEnabled(Game.Configs.VSync);
            SetFramerateLimit(Game.Configs.FrameRate);
            KeyReleased += OnKeyReleased;
            GainedFocus += OnGainedFocus;
            LostFocus += OnLostFocus;
            Resized += OnResized;
            Closed += OnClosed;
            AdjustView();
            FrameTimer.Start();
            SpecTimer.Start();
            CallTimer.Start();
            while (IsOpen())
            {
                DispatchEvents();
                Discord.Invoke();
                Joystick.Update();
                if (!Game.IsPaused)
                {
                    if (!TrailMode)
                    {
                        Clear(Color.Black);
                    }
                    if (StartScreen.SplashTimer.ElapsedMilliseconds > 15000)
                    {
                        Sounds.UpdatePlaylist();
                    }
                    if (Game.IsOnStartScreen)
                    {
                        StartScreen.Render(this);
                    }
                    else
                    {
                        Game.Mode.PreRender(this);
                        GameScreen.Render(this);
                        for (int i = 0; i < Game.ClientEntities.Count; ++i)
                        {
                            if (!Game.ClientEntities[i].SafeToDelete())
                            {
                                Game.ClientEntities[i].Update(this);
                                if (Game.ServerPlayer1.Buff == ModelPlayer.PowerUp.DESTROY_ALL_SHIPS && Game.ClientEntities[i].Base.Position.Y > 96 && Game.ServerEntities[i].CanBeSwept())
                                {
                                    Game.ServerEntities[i].Kill();
                                }
                            }
                        }
                        Game.Mode.Update(this);
                        Game.Mode.PostRender(this);
                    }
                    Display();
                }
                Debug.LastFrameTime = FrameTimer.Elapsed.TotalSeconds;
                Debug.FrameTimes.Add(Debug.LastFrameTime);
                FrameDelta = (float)(FrameTimer.Elapsed.TotalSeconds / 0.01666666666) * SpeedFactor;
                FrameCount += FrameTimer.Elapsed.TotalSeconds;
                FrameIndex += 1;
                FrameTimer.Restart();
                int load = 0;
                if (FrameCount > 1)
                {
                    SetTitle(Game.Name + " [FPS: " + FrameIndex + " | Δ: " + string.Format("{0:0.000000}", FrameDelta) + "]");
                    Game.FramesPerSecond = FrameIndex;
                    Debug.FrameRates.Add(FrameIndex);
                    FrameCount = 0;
                    FrameIndex = 0;
                    for (int i = 0; i < Game.ServerEntities.Count; ++i)
                    {
                        Model model = Game.ServerEntities[i];
                        if (model.Deactivated)
                        {
                            Game.ServerEntities.RemoveAt(i);
                            Game.ClientEntities.RemoveAt(i);
                            ++load;
                            --i;
                        }
                    }
                    if (load > 0)
                    {
                        Debug.LastSweepTime = FrameTimer.Elapsed.TotalSeconds;
                        Debug.SweepTimes.Add(Debug.LastSweepTime);
                        Debug.LastSweepLoad = load;
                        Debug.SweepLoads.Add(Debug.LastSweepLoad);
                    }
                }
                if (IdealSpeedFactor < SpeedFactor)
                {
                    SpeedFactor -= FrameDelta / 2;
                    if (Math.Abs(SpeedFactor - IdealSpeedFactor) < FrameDelta)
                    {
                        SpeedFactor = IdealSpeedFactor;
                    }
                }
                if (IdealSpeedFactor > SpeedFactor)
                {
                    SpeedFactor += FrameDelta / 2;
                    if (Math.Abs(SpeedFactor - IdealSpeedFactor) < FrameDelta)
                    {
                        SpeedFactor = IdealSpeedFactor;
                    }
                }
                if (Game.Stopped)
                {
                    PreClose();
                }
                if (CallTimer.ElapsedMilliseconds > 1000)
                {
                    Debug.SnapshotDemoninator += 1;
                    Debug.AverageSweepTime = Debug.SweepTimes.Count > 0 ? Debug.SweepTimes.Average() : 0;
                    Debug.AverageSweepLoad = Debug.SweepLoads.Count > 0 ? Debug.SweepLoads.Average() : 0;
                    Debug.AverageFrameTime = Debug.FrameTimes.Count > 0 ? Debug.FrameTimes.Average() : 0;
                    Debug.AverageFrameRate = Debug.FrameRates.Count > 0 ? Debug.FrameRates.Average() : 0;
                    CallTimer.Restart();
                }
            }
            PreClose();
        }
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    Game.IsPaused = !Game.IsPaused;
                    break;
                case Keyboard.Key.F1:
                    Game.IsOnStartScreen = true;
                    break;
                case Keyboard.Key.F2:
                    Capture().SaveToFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "LarsOfTheStars", string.Format("{0}-{1}-{2}_{3}.{4}.{5}.png", DateTime.Now.Year.ToString().PadLeft(2, '0'), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'), DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0'))));
                    Debug.LogSnapshot();
                    break;
                case Keyboard.Key.F3:
                    TrailMode = !TrailMode;
                    break;
                case Keyboard.Key.PageUp:
                    IdealSpeedFactor = 2.0F;
                    break;
                case Keyboard.Key.Home:
                    IdealSpeedFactor = 1.0F;
                    break;
                case Keyboard.Key.PageDown:
                    IdealSpeedFactor = 0.1F;
                    break;
                default:
                    break;
            }
        }
        private void OnGainedFocus(object sender, EventArgs e)
        {
            Game.IsFocused = true;
        }
        private void OnLostFocus(object sender, EventArgs e)
        {
            Game.IsFocused = false;
        }
        private void OnResized(object sender, SizeEventArgs e)
        {
            AdjustView();
        }
        private void AdjustView()
        {
            View View = new View(new Vector2f(128, 96), new Vector2f(256, 192));
            Vector2f ViewportSize = new Vector2f(Size.X, Size.Y);
            if (Size.X > Size.Y)
            {
                ViewportSize = new Vector2f(Size.Y * 1.333333F, Size.Y);
            }
            else
            {
                ViewportSize = new Vector2f(Size.X, Size.X / 1.333333F);
            }
            Vector2f Offset = new Vector2f((1 - (ViewportSize.X / Size.X)) / 2, (1 - (ViewportSize.Y / Size.Y)) / 2);
            View.Viewport = new FloatRect(Offset.X, Offset.Y, ViewportSize.X / Size.X, ViewportSize.Y / Size.Y);
            SetView(View);
        }
        private void OnClosed(object sender, EventArgs e)
        {
            PreClose();
        }
        private void PreClose()
        {
            if (!Closing)
            {
                Closing = true;
                Discord.Close();
                Close();
            }
        }
    }
}
