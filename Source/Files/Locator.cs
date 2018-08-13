using System;
using System.IO;

namespace LarsOfTheStars.Source.Files
{
    public class Locator
    {
        public static string Get(params string[] paths)
        {
            string platter = Path.Combine(Environment.CurrentDirectory, "assets");
            foreach (string path in paths) {
                platter = Path.Combine(platter, path);
            }
            return platter;
        }
        public static string Get(string prefix, string[] paths)
        {
            string platter = Path.Combine(Environment.CurrentDirectory, "assets", prefix);
            foreach (string path in paths)
            {
                platter = Path.Combine(platter, path);
            }
            return platter;
        }
    }
}
