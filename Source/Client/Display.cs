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
        private double FrameCount = 0;
        private long FrameIndex = 0;
        public float FrameDelta = 1;
        public float SpeedFactor = 1;
        public Display() : base(new VideoMode(Game.Configs.ScreenWidth, Game.Configs.ScreenHeight), Game.Name, Game.Configs.Fullscreen ? Styles.Fullscreen : Game.Style)
        {
            this.SetIcon(24, 24, new Image(Environment.CurrentDirectory + "/assets/icon.png").Pixels);
            this.SetView(new View(new Vector2f(128, 96), new Vector2f(256, 192)));
            this.SetVerticalSyncEnabled(Game.Configs.VSync);
            this.SetFramerateLimit(Game.Configs.FrameRate);
            this.KeyReleased += this.OnKeyReleased;
            this.GainedFocus += this.OnGainedFocus;
            this.LostFocus += this.OnLostFocus;
            this.Resized += this.OnResized;
            this.Closed += this.OnClosed;
            this.FrameTimer.Start();
            this.SpecTimer.Start();
            this.CallTimer.Start();
            while (this.IsOpen())
            {
                this.DispatchEvents();
                Sounds.UpdatePlaylist();
                Discord.Invoke();
                Joystick.Update();
                if (!Game.IsPaused)
                {
                    this.Clear(Color.Black);
                    if (Game.IsOnStartScreen)
                    {
                        this.StartScreen.Render(this);
                    }
                    else
                    {
                        Game.Mode.PreRender(this);
                        this.GameScreen.Render(this);
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
                    this.Display();
                }
                Debug.LastFrameTime = this.FrameTimer.Elapsed.TotalSeconds;
                Debug.FrameTimes.Add(Debug.LastFrameTime);
                this.FrameDelta = (float)(this.FrameTimer.Elapsed.TotalSeconds / 0.01666666666) * this.SpeedFactor;
                this.FrameCount += this.FrameTimer.Elapsed.TotalSeconds;
                this.FrameIndex += 1;
                this.FrameTimer.Restart();
                int load = 0;
                if (this.FrameCount > 1)
                {
                    Game.FramesPerSecond = this.FrameIndex;
                    Debug.FrameRates.Add(this.FrameIndex);
                    this.FrameCount = 0;
                    this.FrameIndex = 0;
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
                        Debug.LastSweepTime = this.FrameTimer.Elapsed.TotalSeconds;
                        Debug.SweepTimes.Add(Debug.LastSweepTime);
                        Debug.LastSweepLoad = load;
                        Debug.SweepLoads.Add(Debug.LastSweepLoad);
                    }
                }
                if (Game.Stopped)
                {
                    this.Close();
                }
                if (this.CallTimer.ElapsedMilliseconds > 20000)
                {
                    Debug.SnapshotDemoninator += 1;
                    Debug.AverageSweepTime = Debug.SweepTimes.Count > 0 ? Debug.SweepTimes.Average() : 0;
                    Debug.AverageSweepLoad = Debug.SweepLoads.Count > 0 ? Debug.SweepLoads.Average() : 0;
                    Debug.AverageFrameTime = Debug.FrameTimes.Count > 0 ? Debug.FrameTimes.Average() : 0;
                    Debug.AverageFrameRate = Debug.FrameRates.Count > 0 ? Debug.FrameRates.Average() : 0;
                    Debug.LogSnapshot();
                    this.CallTimer.Restart();
                }
            }
        }
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    Game.IsPaused = !Game.IsPaused;
                    break;
                case Keyboard.Key.F1:
                    this.Capture().SaveToFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "LarsOfTheStars", string.Format("{0}-{1}-{2}_{3}.{4}.{5}.png", DateTime.Now.Year.ToString().PadLeft(2, '0'), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'), DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0'))));
                    break;
                case Keyboard.Key.F2:
                    Game.IsOnStartScreen = true;
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
            this.GetView().Zoom(e.Height / 192);
        }
        private void OnClosed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
