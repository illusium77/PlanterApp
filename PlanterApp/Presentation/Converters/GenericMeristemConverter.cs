using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PlanterApp.Presentation.Converters
{
    public class GenericMeristemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = parameter as string;

            if (target == "Name")
            {
                var type = (MeristemType)value;
                switch (type)
                {
                    case MeristemType.Stem:
                        return "S";
                    case MeristemType.Branch:
                        return "B";
                    case MeristemType.Flower:
                        return "F";
                    case MeristemType.TBD:
                    default:
                        return "?";
                }
            }
            else if (target == "Color")
            {
                var type = (MeristemType)value;
                switch (type)
                {
                    case MeristemType.TBD:
                        return Brushes.Red;
                    case MeristemType.Stem:
                        return Brushes.SeaGreen;
                    case MeristemType.Branch:
                        return Brushes.Green;
                    case MeristemType.Flower:
                        return Brushes.Orange;
                }
            }
            else if (target == "CanChangeType")
            {
                var type = (MeristemType)value;
                return type != MeristemType.Stem ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (target == "CanAddNode")
            {
                var type = (MeristemType)value;
                return type == MeristemType.Stem || type == MeristemType.Branch ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (target == "Test")
            {

            }


            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
