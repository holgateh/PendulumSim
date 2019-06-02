using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseWork_Project
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        public StartUpWindow()
        {
            InitializeComponent();
        }

        //When the spring pendulum button has been clicked...
        private void SpringPendulumOption_Click(object sender, RoutedEventArgs e)
        {

            var mainWindow = new MainWindow(Mode.SpringPendulumSimulation) {Visibility = Visibility.Visible};
            this.Close();
        }

        //When the pendulum button has been clicked...
        private void PendulumOption_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(Mode.PendulumSimulation) {Visibility = Visibility.Visible};
            this.Close();
        }

        //When the double pendulum button has been clicked...
        private void DoublePendulumOption_OnClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(Mode.DoublePendulum) {Visibility = Visibility.Visible};
            this.Close();
        }

        //When the exit button has been clicked...
        private void ExitOption_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
