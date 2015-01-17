using PlanterApp.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PlanterApp.Presentation.Converters
{
    public class PlantStatusToImageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                return DependencyProperty.UnsetValue;
            }

            var state = (PlantState)values[0];
            var isSelected = (bool)values[1];

            switch (state)
            {
                case PlantState.Empty:
                    return isSelected ? Application.Current.Resources["PlantEmptySelected"] : Application.Current.Resources["PlantEmpty"];
                case PlantState.Planted:
                    return isSelected ? Application.Current.Resources["PlantPlantedSelected"] : Application.Current.Resources["PlantPlanted"];
                case PlantState.Alive:
                    return isSelected ? Application.Current.Resources["PlantAliveSelected"] : Application.Current.Resources["PlantAlive"];
                case PlantState.Buds:
                    return isSelected ? Application.Current.Resources["PlantBudsSelected"] : Application.Current.Resources["PlantBuds"];
                case PlantState.Flowering:
                    return isSelected ? Application.Current.Resources["PlantFloweringSelected"] : Application.Current.Resources["PlantFlowering"];
                case PlantState.Seeds:
                    return isSelected ? Application.Current.Resources["PlantSeedsSelected"] : Application.Current.Resources["PlantSeeds"];
                case PlantState.Dead:
                    return isSelected ? Application.Current.Resources["PlantDeadSelected"] : Application.Current.Resources["PlantDead"];
                case PlantState.InitFailed:
                    return isSelected ? Application.Current.Resources["PlantInitFailedSelected"] : Application.Current.Resources["PlantInitFailed"];
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
