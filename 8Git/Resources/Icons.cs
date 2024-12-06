using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace _8Git
{
    public class Icons
    {
        public static List<Icon> allIcons = new List<Icon>();
        public static List<Bitmap> allBitmaps = new List<Bitmap>();

        public static Icon CreateCircleIcon(string color, int width, int height)
        {
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Transparent);
                    using (Brush brush = new SolidBrush(Common.GetColor(color)))
                    {
                        graphics.FillEllipse(brush, 0, 0, width, height);
                    }
                }

                IntPtr hIcon = bitmap.GetHicon();
                Icon icon = Icon.FromHandle(hIcon);
                allIcons.Add(icon);
                return icon;
            }
        }

        public static Icon CreateUnicodeIcon(string unicodeCharacter = "\u2600", string color = "#FF0000", string background = "", int iconSize = 256, float correction = 0.9f)
        {
            using (Bitmap bitmap = (Bitmap)CreateUnicodeImage(unicodeCharacter, color, background, iconSize, correction))
            {
                IntPtr hIcon = bitmap.GetHicon();
                Icon icon = Icon.FromHandle(hIcon);
                allIcons.Add(icon);
                return icon;
            }
        }

        public static Image CreateUnicodeImage(string unicodeCharacter = "\u2600", string color = "#FF0000", string background = "", int iconSize = 256, float correction = 0.9f)
        {
            using (Bitmap bitmap = new Bitmap(iconSize, iconSize))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                if (background != "")
                {
                    graphics.Clear(Common.GetColor(background));
                }
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                Font font = new Font("Segoe UI Emoji", iconSize * correction, FontStyle.Regular, GraphicsUnit.Pixel);

                SizeF characterSize = graphics.MeasureString(unicodeCharacter, font);

                float x = (iconSize - characterSize.Width) / 2;
                float y = (iconSize - characterSize.Height) / 2;

                using (Brush brush = new SolidBrush(Common.GetColor(color)))
                {
                    graphics.DrawString(unicodeCharacter, font, brush, x, y);
                }

                Bitmap result = (Bitmap)bitmap.Clone();
                allBitmaps.Add(result);
                return result;
            }
        }

        public static void DisposeIcon(Icon icon)
        {
            allIcons.Remove(icon);
            icon.Dispose();            
        }

        public static void DisposeAllIcon()
        {
            foreach (Icon icon in allIcons)
            {
                icon.Dispose();
            }

            allIcons.Clear();
        }
    }
}
