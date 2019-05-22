using Common.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Client.Converters
{
    class CheckersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Cell cell = (Cell)value;
            int checkers = cell.NumOfCheckers;
            ObservableCollection<Ellipse> checkersCollection = new ObservableCollection<Ellipse>();
            for (int i = 0; i < checkers; i++)
            {
                Ellipse e = new Ellipse();
                e.Width = 20;
                e.Height = 20;
                if (cell.Color == CheckerColor.Black)
                    e.Fill = new SolidColorBrush(Colors.Black);
                else if (cell.Color == CheckerColor.White)
                {
                    e.Fill = new SolidColorBrush(Colors.White);
                    e.Stroke = new SolidColorBrush(Colors.Black);
                    e.StrokeThickness = 1;
                }

                checkersCollection.Add(e);
            }

            return checkersCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
