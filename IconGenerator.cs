using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class IconGenerator
{
    public static void Main(string[] args)
    {
        var bitmap = new Bitmap(256, 256);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Transparent);
            graphics.FillEllipse(Brushes.Black, 0, 0, 256, 256);
        }

        bitmap.Save(args[0], ImageFormat.Icon);
    }
}
