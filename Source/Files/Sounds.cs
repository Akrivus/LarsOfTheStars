using System;
using System.Collections.Generic;
using System.IO;
using SFML.Audio;

namespace LarsOfTheStars.Source.Files
{
    public class Sounds
    {
        private static int GLOBAL_BUFFER_COUNT = 40;
        private static float GLOBAL_PITCH = 1.0F;
        private static List<Sound> GLOBAL_BUFFERS = new List<Sound>();
        private static Dictionary<String, SoundBuffer> LOADED_BUFFERS = new Dictionary<String, SoundBuffer>();
        private static List<Music> GLOBAL_PLAYLIST = new List<Music>();
        private static int PLAYLIST_POS = 0;
        public static void Load()
        {
            foreach (string file in Directory.GetFiles(Locator.Get("sounds/music")))
            {
                GLOBAL_PLAYLIST.Add(new Music(file));
            }
            GLOBAL_PLAYLIST.Shuffle();
            for (int i = 0; i < GLOBAL_BUFFER_COUNT; ++i)
            {
                GLOBAL_BUFFERS.Add(new Sound());
            }
        }
        public static void Play(params string[] paths)
        {
            string path = Locator.Get("sounds", paths);
            if (!LOADED_BUFFERS.ContainsKey(path))
            {
                LOADED_BUFFERS[path] = new SoundBuffer(path);
            }
            SoundBuffer buffer = LOADED_BUFFERS[path];
            for (int i = 0; i < GLOBAL_BUFFER_COUNT; ++i)
            {
                if (GLOBAL_BUFFERS[i].Status == SoundStatus.Stopped)
                {
                    GLOBAL_BUFFERS[i].SoundBuffer = buffer;
                    GLOBAL_BUFFERS[i].Volume = Game.Configs.SoundVolume;
                    GLOBAL_BUFFERS[i].Play();
                    return;
                }
            }
        }
        public static void PlayRandom(params string[] paths)
        {
            string path = Locator.Get("sounds", paths);
            string[] files = Directory.GetFiles(path);
            string file = files[Game.RNG.Next(files.Length)];
            Play(Locator.Get(path, Path.GetFileName(file)));
        }
        public static void StopAllSounds()
        {
            for (int i = 0; i < GLOBAL_BUFFER_COUNT; ++i)
            {
                if (GLOBAL_BUFFERS[i].Status != SoundStatus.Stopped)
                {
                    GLOBAL_BUFFERS[i].Stop();
                    return;
                }
            }
        }
        public static void UpdatePlaylist()
        {
            if (GLOBAL_PLAYLIST[PLAYLIST_POS].Status == SoundStatus.Stopped)
            {
                PLAYLIST_POS += 1;
                if (PLAYLIST_POS >= GLOBAL_PLAYLIST.Count)
                {
                    PLAYLIST_POS = 0;
                }
                GLOBAL_PLAYLIST[PLAYLIST_POS].Volume = Game.Configs.MusicVolume;
                GLOBAL_PLAYLIST[PLAYLIST_POS].Play();
            }
        }
        public static void SetPitch(float pitch)
        {
            GLOBAL_PITCH = pitch;
            for (int i = 0; i < GLOBAL_PLAYLIST.Count; ++i)
            {
                GLOBAL_PLAYLIST[i].Pitch = GLOBAL_PITCH;
            }
            for (int i = 0; i < GLOBAL_BUFFER_COUNT; ++i)
            {
                GLOBAL_BUFFERS[i].Pitch = GLOBAL_PITCH;
            }
        }
        public static void ResetPitch()
        {
            SetPitch(1.0F);
        }
    }
    public static class Extensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n -= 1;
                int k = Game.RNG.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
