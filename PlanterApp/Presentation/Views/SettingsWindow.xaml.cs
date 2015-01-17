using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.Windows;
using PlanterApp.Applications.Views;

namespace PlanterApp.Presentation.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    [Export(typeof(ISettingsWindow))]
    public partial class SettingsWindow : Window, ISettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void ShowDialog(object owner)
        {
            Owner = owner as Window;
            ShowDialog();
        }
    }
}
