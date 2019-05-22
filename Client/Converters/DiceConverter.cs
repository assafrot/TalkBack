using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Client.Converters
{
    class DiceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int die = (int)value;
            BitmapImage image = new BitmapImage(new Uri($"C:/Users/assaf/Desktop/Sela/SOA Project/TalkBack/Client/Images/{die}.png"));
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
