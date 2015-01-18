using PlanterApp.Domain;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PlanterApp.Presentation.Converters
{
    public class PlantStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PlantState == false)
            {
                return null;
            }

            var state = (PlantState)value;
            switch (state)
            {
                case PlantState.Empty:
                    return Brushes.White;
                case PlantState.Planted:
                    return Brushes.DeepSkyBlue;
                case PlantState.Alive:
                    return Brushes.Green;
                case PlantState.Buds:
                    return Brushes.Salmon;
                case PlantState.Flowering:
                    return Brushes.Orange;
                case PlantState.Seeds:
                    return Brushes.Sienna;
                case PlantState.Dead:
                    return Brushes.SlateGray;
                case PlantState.InitFailed:
                    return Brushes.Red;
            }
            
            throw new InvalidOperationException("Enum value is unknown.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
