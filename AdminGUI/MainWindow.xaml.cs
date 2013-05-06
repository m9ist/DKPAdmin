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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogAnalyzer;

namespace AdminGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogParser _logParser = new LogParser();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // пока пусто
            string inpData = _logParser.ReadFile(@"C:\!Data\GitHub\DKPAdmin\Tests\hagakure.lua");

            string cleanData = _logParser.CleanEmptyChars(inpData);

            RichText.Text = cleanData;
        }

        private void Test2_OnClick(object sender, RoutedEventArgs e)
        {
            string inp =
                "{[\"reason\"] = \"|cffa335ee|Hitem:94821:0:0:0:0:0:0:885497856:90:0:0|h[Разрыватели артерий]|h|r\",[\"t\"] = 1366319194,[\"name\"] = \"Диагноз\",[\"adj\"] = -5,}, -- [69]{[\"reason\"] = \"|cffa335ee|Hitem:95511:0:0:0:0:0:0:885497856:90:0:0|h[Отсеченное щупальце Дуруму]|h|r\",[\"t\"] = 1366319222,[\"name\"] = \"Парасолька\",[\"adj\"] = -11,}, -- [70]";
            var s = _logParser.AnalyzeLuaString(ref inp);
        }
    }
}
