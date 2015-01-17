using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PlanterApp.Presentation.Converters
{
    public class NodeTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeType == false)
            {
                return null;
            }

            var type = (NodeType)value;
            switch (type)
            {
                case NodeType.TBD:
                    return Brushes.Red;
                case NodeType.Cotyledons:
                    return Brushes.ForestGreen;
                case NodeType.Basal:
                    return Brushes.OliveDrab;
                case NodeType.Stem:
                    return Brushes.DarkSeaGreen;
                case NodeType.Secondary:
                    return Brushes.YellowGreen;
            }
            
            throw new InvalidOperationException("Enum value is unknown.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
