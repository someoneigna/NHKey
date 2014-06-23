using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace NHkey.Helpers
{
    public static class BitmapHelper
    {
        /// <summary>
        /// Gets the icon bitmap for the specified program.
        /// </summary>
        /// <param name="filename">Full path to the program.</param>
        /// <returns>The icon as a BitmapSource </returns>
        public static BitmapSource GetIcon(string filename)
        {
            if (filename == null || !File.Exists(filename)) { return null; }
            System.Drawing.Bitmap bitmap = System.Drawing.Icon.ExtractAssociatedIcon(filename).ToBitmap();
            BitmapImage result = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;

                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

            }
            return result;
        }
    }
}
