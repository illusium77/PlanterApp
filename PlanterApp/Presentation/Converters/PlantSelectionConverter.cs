using PlanterApp.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PlanterApp.Presentation.Converters
{
    public class PlantSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = parameter as string;
            var isSelected = (bool)value;

            if (target == "BorderColor")
            {
                return isSelected ? Brushes.Red.Color : Brushes.Black.Color;
            }

            if (target == "ShadowColor")
            {
                return isSelected ? Brushes.Red.Color : Brushes.Black.Color;
            }

            if (target == "BorderThickness")
            {
                return isSelected ? 2 : 1;
            }

            if (target == "Opacity")
            {
                return isSelected ? 1.0 : 1.0;
            }

            if (target == "TextColor")
            {
                return isSelected ? Brushes.WhiteSmoke : Brushes.Black;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
