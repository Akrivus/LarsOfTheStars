using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace LarsOfTheStars.Source.Files
{
    public class Fonts
    {
        private static Dictionary<String, Font> LOADED_FONTS = new Dictionary<String, Font>();
        public static Font Load(string font)
        {
            string path = Locator.Get("textures", "fonts", font);
            if (!LOADED_FONTS.ContainsKey(path))
            {
                LOADED_FONTS[path] = new Font(path);
            }
            return LOADED_FONTS[path];
        }
    }
}
