using System;
using System.IO;
using System.Windows.Media.Imaging;
using TagLib;

namespace Player.Models
{
    public class LoadImage
    {


        public BitmapImage LoadAlbumArt(string path)
        {
            TagLib.File musicFile = TagLib.File.Create(path);
            if (musicFile.Tag.Pictures.Length > 0)
            {
                IPicture albumArt = musicFile.Tag.Pictures[0];
                MemoryStream pictureMemoryStream = new MemoryStream(albumArt.Data.Data);
                pictureMemoryStream.Seek(0, SeekOrigin.Begin);
                return ReturnImageFromSource(pictureMemoryStream);
            }
            else
            {
                return SetDefaultImage();
            }
            
        }


        private BitmapImage ReturnImageFromSource(MemoryStream stream)
        {
            var imageBitmap = new BitmapImage();
            imageBitmap.BeginInit();
            imageBitmap.StreamSource = stream;
            imageBitmap.EndInit();
            return imageBitmap;
        }


        public BitmapImage SetDefaultImage()
        {
            var imageBitmap = new BitmapImage();
            imageBitmap.BeginInit();
            imageBitmap.UriSource = new Uri("../images/note.png", UriKind.Relative);
            imageBitmap.EndInit();
            return imageBitmap;
        }
    }
}