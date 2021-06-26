using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkColorWatcher
{
    public static class SettingsManager
    {
        private static Color color1 = Color.FromArgb(255, 1, 1, 1);
        private static Color color2 = Color.FromArgb(255, 43, 43, 43);

        public static Color Color1 => color1;
        public static Color Color2 => color2;

        public static void Load()
        {
            var settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.txt");

            if (!File.Exists(settingsPath))
                return;

            var settings = File.ReadAllLines(settingsPath);
            var dsettings = new Dictionary<string, string>();

            foreach (var ln in settings)
            {
                if (ln.Trim().Length < 1 || ln.Trim().StartsWith("#"))
                    continue;

                var parts = ln.Split(new[] { '=' }, 2);
                dsettings[parts[0]] = parts[1];
            }

            const string c1 = "color1";
            const string c2 = "color2";

            if (dsettings.ContainsKey(c1))
            {
                if (ParseColor(dsettings[c1], out Color pColor1))
                    color1 = pColor1;
            }

            if (dsettings.ContainsKey(c2))
            {
                if (ParseColor(dsettings[c2], out Color pColor2))
                    color2 = pColor2;
            }
        }

        private static bool ParseColor(string str, out Color color)
        {
            if (str.StartsWith("#"))
            {
                if (str.Length == 4)
                {
                    str = "#"
                        + str.Substring(1, 1) + str.Substring(1, 1)
                        + str.Substring(2, 1) + str.Substring(2, 1)
                        + str.Substring(3, 1) + str.Substring(3, 1);
                }


                if (str.Length == 7)
                {
                    var hR = str.Substring(1, 2);
                    var hG = str.Substring(3, 2);
                    var hB = str.Substring(5, 2);

                    Debug.WriteLine(hR);
                    Debug.WriteLine(hG);
                    Debug.WriteLine(hB);

                    byte r, g, b;
                    r = Convert.ToByte(hR, 16);
                    g = Convert.ToByte(hG, 16);
                    b = Convert.ToByte(hB, 16);

                    color = Color.FromArgb(255, r, g, b);
                    return true;
                }
                else
                {
                    color = Color.Black;
                    return false;
                }
            }
            else if (str.Contains(","))
            {
                var parts = str.Split(',');
                if (parts.Length != 3)
                {
                    color = Color.Black;
                    return false;
                }

                byte r, g, b;
                r = Convert.ToByte(parts[0], 10);
                g = Convert.ToByte(parts[1], 10);
                b = Convert.ToByte(parts[2], 10);
                color = Color.FromArgb(255, r, g, b);
                return true;
            }
            color = Color.Black;
            return false;
        }
    }
}
