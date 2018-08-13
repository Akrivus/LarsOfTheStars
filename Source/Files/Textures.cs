using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace LarsOfTheStars.Source.Files
{
    public class Textures
    {
        private static Dictionary<String, Texture> LOADED_TEXTURES = new Dictionary<String, Texture>();
        public static Texture Load(params string[] paths)
        {
            string path = Locator.Get("textures", paths);
            if (!LOADED_TEXTURES.ContainsKey(path))
            {
                LOADED_TEXTURES[path] = new Texture(path);
            }
            return LOADED_TEXTURES[path];
        }
    }
}
