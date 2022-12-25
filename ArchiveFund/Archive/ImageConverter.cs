using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Archive
{
    public class ImageConverter : IValueConverter
    {
        // base64 -> BitmapImage
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string base64 = value as string;

            byte[] data = System.Convert.FromBase64String(base64);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(data);
            bi.EndInit();

            return bi;
        }

        // BitmapImage -> base64
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string path = value as string;

            if (!File.Exists(path)) return null;
            byte[] imageArray = File.ReadAllBytes(path);
            string base64 = System.Convert.ToBase64String(imageArray);

            return base64;
        }

        public BitmapImage ConvertToImage(string base64) => Convert(base64, null, null, null) as BitmapImage;

        public string ConvertToBase64(string path) => ConvertBack(path, null, null, null) as string;
    }
}
