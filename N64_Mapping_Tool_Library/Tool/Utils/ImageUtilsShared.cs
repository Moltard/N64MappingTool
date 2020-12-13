using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace N64Library.Tool.Utils
{

    public static class ImageUtilsShared
    {

        /// <summary>
        /// Represents a transparent pixel
        /// </summary>
        private static readonly System.Drawing.Color transparentPixel = System.Drawing.Color.FromArgb(0, 0, 0, 0); // Pixel with Alpha 0

        /// <summary>
        /// Try to save an Image to a given path then Dispose of the data
        /// </summary>
        /// <param name="img">The Image to save</param>
        /// <param name="outputFile">The file to create</param>
        /// <returns>Return true if successful, false otherwise</returns>
        public static bool TrySaveImageDispose(Image img, string outputFile)
        {
            try
            {
                img.Save(outputFile);
            }
            catch
            {
                return false; // If impossible to delete
            }
            finally
            {
                img.Dispose();
            }
            return true;
        }

        /// <summary>
        /// Return a Bitmap if the given file exists, return null otherwise
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Bitmap CreateBitmap(Bitmap bmp)
        {
            if (bmp != null)
                return new Bitmap(bmp);
            return null;
        }

        /// <summary>
        /// Return a Bitmap if the given file exists, return null otherwise
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Bitmap CreateBitmap(string file)
        {
            if (System.IO.File.Exists(file))
                return new Bitmap(file);
            return null;
        }

        /// <summary>
        /// Return a Bitmap of the given size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CreateBitmap(int width, int height)
        {
            return new Bitmap(width,height);
        }

        /// <summary>
        /// Return true if both pictures are the same size, false otherwise
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool AreSameSize(string file1, string file2)
        {
            Bitmap img1 = CreateBitmap(file1);
            Bitmap img2 = CreateBitmap(file2);
            if (img1 != null && img2 != null)
            {
                return AreSameSize(img1, img2);
            }
            return false;
        }

        /// <summary>
        /// Return true if both pictures are the same size, false otherwise
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public static bool AreSameSize(Bitmap img1, Bitmap img2)
        {
            return img1.Width == img2.Width && img1.Height == img2.Height;
        }

        /// <summary>
        /// Return true if both pictures are the same size, false otherwise
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public static bool AreSameSize(BitmapData img1, BitmapData img2)
        {
            return img1.Width == img2.Width && img1.Height == img2.Height;
        }

        /// <summary>
        /// Flip a given image horizontally and/or vertically
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="outputFile"></param>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <returns></returns>
        public static bool FlipTexture(string fileName, bool horizontal, bool vertical, string outputFile)
        {
            Bitmap img = CreateBitmap(fileName);
            if (img != null)
            {
                Bitmap newImg = CreateBitmap(img);

                if (horizontal && vertical)
                {
                    newImg.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                }
                else
                {
                    if (horizontal)
                    {
                        newImg.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (vertical)
                    {
                        newImg.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                }
                img.Dispose(); // Allow to override the loaded picture without crashing
                if (System.IO.File.Exists(outputFile))
                    FileUtilsShared.TryDeleteFile(outputFile); // Delete the file if it already exists

                return TrySaveImageDispose(newImg, outputFile);
            }

            return false;
        }

        /// <summary>
        /// Mirror a given image horizontally and/or vertically
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="outputFile"></param>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <returns></returns>
        public static bool MirrorTexture(string fileName, bool horizontal, bool vertical, string outputFile)
        {
            Bitmap img = CreateBitmap(fileName);
            if (img != null)
            {
                int width = img.Width;
                int maxWidth = horizontal ? width * 2 : width;
                
                int height = img.Height;
                int maxHeight = vertical ? height * 2 : height;

                Bitmap newImg = CreateBitmap(maxWidth, maxHeight);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        System.Drawing.Color p = img.GetPixel(i, j);
                        newImg.SetPixel(i, j, p); // Set the pixel
                        int newWidth = maxWidth - 1 - i;
                        int newHeight = maxHeight - 1 - j;
                        if (horizontal)
                            newImg.SetPixel(newWidth, j, p); // Set the mirrored pixel
                        if (vertical)
                            newImg.SetPixel(i, newHeight, p); // Set the mirrored pixel
                        if (horizontal && vertical)
                            newImg.SetPixel(newWidth, newHeight, p); // Set the mirrored pixel
                    }
                }
                img.Dispose(); // Allow to override the loaded picture without crashing
                if (System.IO.File.Exists(outputFile))
                    FileUtilsShared.TryDeleteFile(outputFile); // Delete the file if it already exists

                return TrySaveImageDispose(newImg, outputFile);
            }

            return false;
        }

        /// <summary>
        /// Create a transparent image using the original image and its alpha 
        /// </summary>
        /// <param name="alphaFile"></param>
        /// <param name="textureFile"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static bool MakeTransparent(string alphaFile, string textureFile, string outputFile)
        {
            Bitmap BmpA = CreateBitmap(alphaFile);
            Bitmap BmpC = CreateBitmap(textureFile);

            if (BmpA != null && BmpC != null)
            {
                if (AreSameSize(BmpA, BmpC))
                {
                    int width = BmpA.Width;
                    int height = BmpA.Height;
                    Bitmap newImg = CreateBitmap(width, height);

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            System.Drawing.Color alphaPixel = BmpA.GetPixel(i, j);
                            System.Drawing.Color texturePixel = BmpC.GetPixel(i, j);
                            if (alphaPixel.R == 255 && alphaPixel.G == 255 && alphaPixel.B == 255) // White pixel
                                newImg.SetPixel(i, j, transparentPixel); // Set transparent pixel
                            else
                                newImg.SetPixel(i, j, texturePixel); // Set the texture pixel
                        }
                    }

                    BmpA.Dispose(); // Dispose the file streams so we can overwrite the files
                    BmpC.Dispose();
                    if (System.IO.File.Exists(outputFile))
                        FileUtilsShared.TryDeleteFile(outputFile); // Delete the file if it already exists

                    return TrySaveImageDispose(newImg, outputFile);
                }
            }
            return false;
        }

        /// <summary>
        /// Create the negative of a given image
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static bool NegativePicture(string fileName, string outputFile)
        {
            Bitmap img = CreateBitmap(fileName);
            if (img != null)
            {
                int width = img.Width;
                int height = img.Height;

                Bitmap newImg = CreateBitmap(width, height);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        System.Drawing.Color p = img.GetPixel(i, j);
                        System.Drawing.Color pNegative = NegativePixel(p);
                        newImg.SetPixel(i, j, pNegative); // Set the pixel
                    }
                }
                img.Dispose(); // Allow to override the loaded picture without crashing
                if (System.IO.File.Exists(outputFile))
                    FileUtilsShared.TryDeleteFile(outputFile); // Delete the file if it already exists

                return TrySaveImageDispose(newImg, outputFile);
            }

            return false;
        }

        /// <summary>
        /// Returns the negative of a given pixel
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static System.Drawing.Color NegativePixel(System.Drawing.Color p)
        {
            int a = p.A;
            int r = 255 - p.R;
            int g = 255 - p.G;
            int b = 255 - p.B;
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Iterate through the list of BitmapStoreData and return true if the given image is equal to one of them
        /// </summary>
        /// <param name="imgList"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static bool SamePictures(List<BitmapStoreData> imgList, BitmapStoreData img)
        {
            foreach (BitmapStoreData imgCompare in imgList)
            {
                if (SamePictures(img, imgCompare))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Compare 2 BitmapStoreData and return true if the images are the same
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public static bool SamePictures(BitmapStoreData img1, BitmapStoreData img2)
        {
            if (!AreSameSize(img1.BData, img2.BData)) // Not the same size
                return false;

            byte[] data1 = img1.Data;
            byte[] data2 = img2.Data;
            for (int i = 0; i < img1.Size; i += img1.BitsPerPixel / 8)
            {
                if (data1[i] != data2[i] || data1[i + 1] != data2[i + 1]
                    || data1[i + 2] != data2[i + 2] || data1[i + 3] != data2[i + 3]) // Pixels are different
                    return false;
            }
            return true;
        }

    }


    public class BitmapStoreData
    {
        public Bitmap Bmp { get; }
        public BitmapData BData { get; }
        public byte BitsPerPixel { get; }
        public int Size { get; }
        public byte[] Data { get; }

        public BitmapStoreData(Bitmap img)
        {
            Bmp = new Bitmap(img); // The PixelFormat becomes Format32bppArgb here

            BData = Bmp.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, Bmp.PixelFormat);

            BitsPerPixel = GetBitsPerPixel(BData.PixelFormat);

            if (BitsPerPixel == 0) // Invalid pixel format
            {
                Bmp.UnlockBits(BData);
                BData = null;
            }
            else
            {
                /*The size of the image in bytes */
                Size = BData.Stride * BData.Height;

                /*Allocate buffer for image*/
                Data = new byte[Size];

                /*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
                System.Runtime.InteropServices.Marshal.Copy(BData.Scan0, Data, 0, Size);
            }
        }

        /// <summary>
        /// Liberate the Bitmap from memory
        /// </summary>
        public void UnlockBits()
        {
            Bmp.UnlockBits(BData);
        }

        /// <summary>
        /// Return 24 if the pixel format is 24bppRgb, 32 if 32bpp(R/Ar/PAr)gb or 0 otherwise
        /// </summary>
        /// <param name="pixelFormat">Format of the pixel</param>
        /// <returns>Amount of bits</returns>
        private static byte GetBitsPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return 24;
                //break;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    return 32;
                //break;
                default:
                    return 0;
                    //throw new ArgumentException("Only 24 and 32 bit images are supported");

            }
        }


        /// <summary>
        /// Unlock from memory all the BitmapData stored in the list
        /// </summary>
        /// <param name="bitmapList"></param>
        public static void BitmapDataUnlock(List<BitmapStoreData> bitmapList)
        {
            foreach (BitmapStoreData bmp in bitmapList)
                bmp.UnlockBits();
        }

        /// <summary>
        /// Return a list of BitmapStoreData objects to iterate through them later
        /// </summary>
        /// <param name="listFileName"></param>
        /// <returns></returns>
        public static List<BitmapStoreData> GetListBitmapStoreData(List<string> listFileName)
        {
            List<BitmapStoreData> bitmapList = new List<BitmapStoreData>();
            foreach (string fileName in listFileName)
            {
                Bitmap img = ImageUtilsShared.CreateBitmap(fileName);
                if (img != null)
                {
                    BitmapStoreData bmp = new BitmapStoreData(img);
                    if (bmp.BData != null)
                    {
                        bitmapList.Add(bmp);
                    }
                }
            }
            return bitmapList;
        }
    }

}
