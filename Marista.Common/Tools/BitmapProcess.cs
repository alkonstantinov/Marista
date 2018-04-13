using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marista.Common.Tools
{
    public class BitmapProcess
    {
        public static byte[] Resize(byte[] img, int width, int height)
        {

            Bitmap original, resizedImage;
            try
            {

                using (var ms = new MemoryStream(img))
                {
                    original = new Bitmap(ms);
                }




                double ratio = Math.Min(
                    height / (double)original.Height,
                    width / (double)original.Width
                    );
                //resizedImage = new Bitmap(original, (int)(ratio * original.Width), (int)(ratio * original.Height));
                resizedImage = new Bitmap(width, height);
                Graphics gr = Graphics.FromImage(resizedImage);
                gr.DrawImage(original, new Rectangle(0, 0, (int)(ratio * original.Width), (int)(ratio * original.Height)));
                using (var stream = new MemoryStream())
                {
                    resizedImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

    }
}
