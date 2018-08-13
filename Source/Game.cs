using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Client.Objects;
using LarsOfTheStars.Source.Integration;
using LarsOfTheStars.Source.Logic.Objects;
using LarsOfTheStars.Source.Logic.Modes;
using LarsOfTheStars.Source.Files;
using System;
using System.Collections.Generic;
using SFML.Window;

namespace LarsOfTheStars.Source
{
    public class Game
    {
        public static Styles Style = Styles.Default;
        public static string Name = "Lars of the Stars";
        public static bool AcceptQuarterToStart = false;
        public static bool IsOnStartScreen = true;
        public static bool IsFocused = true;
        public static bool IsPaused = false;
        public static bool IsRunning = true;
        public static bool Stopped = false;
        public static Mode Mode = null;
        public static Mode[] Modes = new Mode[]
        {
            new ArcadeMode(),
            new Mode()
        };
        public static float FramesPerSecond = 60;
        public static List<Render> ClientEntities = new List<Render>();
        public static List<Model> ServerEntities = new List<Model>();
        public static ModelPlayer ServerPlayer1;
        public static RenderPlayer ClientPlayer1;
        public static ModelPlayer ServerPlayer2;
        public static RenderPlayer ClientPlayer2;
        public static Configs Configs;
        public static Random RNG = new Random();
        public static int Version = 1;
        public static void Main(string[] args)
        {
            Configs.Load();
            Discord.Start();
            Debug.Initialize();
            Sounds.Load();
            Display.Show();
            Discord.Close();
            IsRunning = false;
        }
        public static void AddEntity(Render client)
        {
            ClientEntities.Add(client);
            ServerEntities.Add(client.Base);
        }
    }
}
