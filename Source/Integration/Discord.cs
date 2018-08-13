using LarsOfTheStars.Source.Integration.REST;
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
        public static RichPresence RP;
        public static Entry You;
        public static bool IsReady;
        public static void Start()
        {
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
            if (Game.Configs.DiscordRPC && IsReady && (CoolTimer.ElapsedMilliseconds > 5000 || !CoolTimer.IsRunning))
            {

                if (RP.Details != null && RP.State != null && RP.Party != null)
                {
                    RichPresence presence = new RichPresence();
                    presence.Details = RP.Details;
                    presence.State = RP.State;
                    presence.Assets = RP.Assets;
                    presence.Party = RP.Party;
                    RPC.SetPresence(RP);
                    if (CoolTimer.IsRunning)
                    {
                        CoolTimer.Restart();
                    }
                    else
                    {
                        CoolTimer.Start();
                    }
                    Database.TryAgain = 0;
                }
            }
        }
        public static void Close()
        {
            if (Game.Configs.DiscordRPC && IsReady)
            {
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
                    Update();
                }
            }
            catch
            {
                return;
            }
        }
        private static void OnReady(object sender, ReadyMessage args)
        {
            You = new Entry();
            You.ID = args.User.ID;
            You.Username = args.User.Username;
            You.Discriminator = args.User.Discriminator.ToString().PadLeft(4, '0');
            You.Score = 0;
            IsReady = true;
            Database.SetScore(0);
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
