using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PlanterApp.Presentation.Converters
{
    public class PlantTraitIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (values == null || values.Length != 2)
            //{
            //    return DependencyProperty.UnsetValue;
            //}

            //var traitNames = values[0] as IEnumerable<string>;
            //var traitValues = values[1] as string[];

            //if (traitNames == null || traitValues == null || traitNames.Count() != traitValues.Length)
            //{
            //    return DependencyProperty.UnsetValue;
            //}

            //var traits = new List<Trait>(traitNames.Count());
            //for (int i = 0; i < traitNames.Count(); i++)
            //{
            //    traits.Add(new Trait { Name = traitNames.ElementAt(i), Value = traitValues[i] });
            //}

            //return traits;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
