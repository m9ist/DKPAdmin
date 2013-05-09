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
            string inpData = _logParser.ReadFile(@"D:\WoW\WTF\Account\N00BE\SavedVariables\hagakure.lua");
            //string inpData = _logParser.ReadFile(@"C:\!Data\GitHub\DKPAdmin\Tests\hagakure.lua");

            string cleanData = _logParser.CleanEmptyChars(inpData);

            var s = _logParser.AnalyzeLuaString(ref cleanData);
        }

        private void Test2_OnClick(object sender, RoutedEventArgs e)
        {
            string inp =
                "[\"log\"] = {{[\"m_value\"] = 1,[\"user\"] = \"Эллиандрессе\",[\"t\"] = 1367512961,[\"event\"] = \"онлайн\",[\"s_value\"] = 0,}, -- [1]{[\"m_value\"] = 2,[\"user\"] = \"Эллиандрессе\",[\"t\"] = 1367512976,[\"event\"] = \"онлайн\",[\"s_value\"] = 0,}, -- [2]},";
            var s = _logParser.AnalyzeLuaString(ref inp);
        }
    }
}
