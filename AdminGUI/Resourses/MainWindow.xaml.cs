using System.Windows;
using WPF.MDI;

namespace AdminGUI.Resourses
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
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
