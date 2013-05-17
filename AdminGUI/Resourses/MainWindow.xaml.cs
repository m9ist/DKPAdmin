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
using WPF.MDI;

namespace AdminGUI.Resourses
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary> создает окошко для анализа лога </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_LogAnalyzer(object sender, RoutedEventArgs e)
        {
            MdiWindowContainer.Children.Add(new MdiChild{ Content = new WindowManagementLog(), Title = "Log analyse tool"});
        }
    }
}
