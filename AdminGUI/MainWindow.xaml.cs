using System;
using System.Collections.Generic;
using System.IO;
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
using LogAnalyzer.Structures;

namespace AdminGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogParser _logParser = new LogParser();
        //string inpData = @"C:\!Data\GitHub\DKPAdmin\Tests\hagakure.lua";
        private const string InpData = @"D:\WoW\WTF\Account\N00BE\SavedVariables\hagakure.lua";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var reader = new StreamReader(InpData, Encoding.UTF8);
            string content = string.Empty;
            try
            {
                content = reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
            }

            string cleanData = _logParser.CleanEmptyChars(content);

            var analizedLog = _logParser.AnalyzeLuaString(ref cleanData);

            // ищем сначала узел faction
            var search = _logParser.SearchNodeWithName(analizedLog, "faction");
            // в нем ищем узел log
            if (search != null)
                search = _logParser.SearchNodeWithName(search, "log");

            var LogRecords = new List<LogRecord>();
            foreach (var iNode in search.GetNodeContent())
            {
                LogRecords.Add(new LogRecord(iNode));
            }
        }

        private void Test2_OnClick(object sender, RoutedEventArgs e)
        {
            string inp =
                "[\"log\"] = {{[\"m_value\"] = 1,[\"user\"] = \"Эллиандрессе\",[\"t\"] = 1367512961,[\"event\"] = \"онлайн\",[\"s_value\"] = 0,}, -- [1]{[\"m_value\"] = 2,[\"user\"] = \"Эллиандрессе\",[\"t\"] = 1367512976,[\"event\"] = \"онлайн\",[\"s_value\"] = 0,}, -- [2]},";
            var s = _logParser.AnalyzeLuaString(ref inp);
        }
    }
}
