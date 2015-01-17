using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PlanterApp.Presentation.Views
{
    /// <summary>
    /// Interaction logic for EditableTextBlock.xaml
    /// </summary>
    public partial class EditableTextBlock : TextBox
    {
        public EditableTextBlock()
        {
            InitializeComponent();
        }

        private void EnableEdit(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as EditableTextBlock;
            if (textBox != null)
            {
                textBox.Focusable = true;
                textBox.Cursor = null;

                Action action = () =>
                {
                    textBox.Focus();

                    if (textBox.Text == null)
                    {
                        textBox.Text = string.Empty;
                    }

                    textBox.CaretIndex = textBox.Text.Length;
                };
                Dispatcher.CurrentDispatcher.BeginInvoke(action);
            }
        }

        private void DisableEdit(object sender, RoutedEventArgs e)
        {
            var textBox = sender as EditableTextBlock;
            if (textBox != null)
            {
                textBox.Focusable = false;
                textBox.Cursor = Cursors.Arrow;
            }
        }
    }
}
