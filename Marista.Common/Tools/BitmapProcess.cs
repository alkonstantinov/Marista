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

                int rectHeight = height;
                int rectWidth = width;
                //if the image is squared set it's height and width to the smallest of the desired dimensions (our box). In the current example rectHeight<rectWidth
                if (original.Height == original.Width)
                {
                    resizedImage = new Bitmap(original, rectHeight, rectHeight);
                }
                else
                {
                    //calculate aspect ratio
                    float aspect = original.Width / (float)original.Height;
                    int newWidth, newHeight;
                    //calculate new dimensions based on aspect ratio
                    newWidth = (int)(rectWidth * aspect);
                    newHeight = (int)(newWidth / aspect);
                    //if one of the two dimensions exceed the box dimensions
                    if (newWidth > rectWidth || newHeight > rectHeight)
                    {
                        //depending on which of the two exceeds the box dimensions set it as the box dimension and calculate the other one based on the aspect ratio
                        if (newWidth > newHeight)
                        {
                            newWidth = rectWidth;
                            newHeight = (int)(newWidth / aspect);

                        }
                        else
                        {
                            newHeight = rectHeight;
                            newWidth = (int)(newHeight * aspect);

                        }
                    }
                    resizedImage = new Bitmap(original, newWidth, newHeight);


                    
                }
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
