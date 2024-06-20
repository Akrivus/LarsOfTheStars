using System.Diagnostics;
using DiscordRPC;
using DiscordRPC.Message;

namespace LarsOfTheStars.Source.Integration
{
    public class Discord
    {
        private static string ID = "459433736249802762";
        private static Stopwatch CoolTimer = new Stopwatch();
        private static DiscordRpcClient RPC;
        public static RichPresence LastRP;
        public static RichPresence RP;
        public static User You;
        public static bool PushUpdate;
        public static bool IsReady;
        public static void Start()
        {
            CoolTimer.Start();
            if (Game.Configs.DiscordRPC)
            {
                RPC = new DiscordRpcClient(ID, true);
                RPC.SetSubscription(EventType.Join | EventType.Spectate | EventType.JoinRequest);
                RPC.OnReady += OnReady;
                RPC.OnClose += OnClose;
                RPC.OnError += OnError;
                RPC.OnSpectate += OnSpectate;
                RPC.OnJoinRequested += OnJoinRequested;
                RPC.OnJoin += OnJoin;
                RPC.Initialize();
                RPC.Invoke();
            }
            RP = new RichPresence();
            RP.Party = new Party()
            {
                Size = 1,
                Max = 1
            };
            RP.Timestamps = new Timestamps();
            RP.Secrets = new Secrets();
            RP.Assets = new Assets()
            {
                LargeImageText = "Playing Lars of the Stars",
                LargeImageKey = "game",
                SmallImageKey = "p3"
            };
        }
        public static void Update()
        {
            if (Game.Configs.DiscordRPC && IsReady)
            {
                RichPresence presence = new RichPresence();
                presence.Details = RP.Details;
                presence.State = RP.State;
                presence.Assets = RP.Assets;
                presence.Party = RP.Party;
                RPC.SetPresence(RP);
                PushUpdate = false;
            }
        }
        public static void Close()
        {
            if (Game.Configs.DiscordRPC && IsReady)
            {
                IsReady = false;
                RPC.Dispose();
            }
        }
        public static void Invoke()
        {
            try
            {
                if (Game.Configs.DiscordRPC)
                {
                    RPC.Invoke();
                    Game.Mode?.GetRPC();
                    if (CoolTimer.ElapsedMilliseconds > 1000)
                    {
                        if (PushUpdate && !Game.IsPaused)
                        {
                            Update();
                        }
                        CoolTimer.Restart();
                    }
                }
            }
            catch
            {
                return;
            }
        }
        private static void OnReady(object sender, ReadyMessage args)
        {
            You = args.User;
            IsReady = true;
        }
        private static void OnClose(object sender, CloseMessage args)
        {
            
        }
        private static void OnError(object sender, ErrorMessage args)
        {
            
        }
        private static void OnSpectate(object sender, SpectateMessage args)
        {
            
        }
        private static void OnJoinRequested(object sender, JoinRequestMessage args)
        {
            
        }
        private static void OnJoin(object sender, JoinMessage args)
        {
            
        }
    }
}
