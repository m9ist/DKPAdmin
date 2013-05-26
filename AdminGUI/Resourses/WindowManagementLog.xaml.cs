using LogAnalyzer;
using LogAnalyzer.Structures;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace AdminGUI.Resourses
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class WindowManagementLog
    {
        // класс с логикой парсинга файла
        private readonly LogParser _logParser = new LogParser();

        /// <summary> путь до файла .lua </summary>
        //private const string _inpData = @"C:\!Data\GitHub\DKPAdmin\Tests\hagakure.lua";
        //private const string _inpData = @"D:\WoW\WTF\Account\N00BE\SavedVariables\hagakure.lua";
        private const string _inpData = @"E:\!data\GitHub\DKPAdmin\hagakure.lua";

        /// <summary> коллекция для хранения загруженного лога </summary>
        private readonly ObservableCollection<LogRecord> _log;

        /// <summary> инициализация окошка </summary>
        public WindowManagementLog()
        {
            InitializeComponent();
            _log = new ObservableCollection<LogRecord>();
            GuiTree.DataContext = _log;
        }

        /// <summary> делегат - обработка нажатия кнопки "загрузить лог" 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // собственно считываем инфу с файла в строчку
            var reader = new StreamReader(_inpData, Encoding.UTF8);
            string content;
            try
            {
                content = reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
            }

            // очищаем строчку от спец символов
            string cleanData = _logParser.CleanEmptyChars(content);
            // получаем дерево сохраненной информации аддона
            var analizedLog = _logParser.AnalyzeLuaString(ref cleanData);

            // ищем сначала узел faction
            var search = _logParser.SearchNodeWithName(analizedLog, "faction");
            // в нем ищем узел log
            search = _logParser.SearchNodeWithName(search, "log");
            // очищаем предыдущую подгрзку если есть и преобразовываем распарсенный лог
            // в нужный нам вид
            _log.Clear();
            foreach (var iNode in search.GetNodeContent())
            {
                _log.Add(new LogRecord(iNode));
            }
            FilterPassEvent(this, new RoutedEventArgs());
        }

        // обработчик фильтра включить/отключить просмотр "пасс" события
        private void FilterPassEvent(object sender, RoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(_log);
            if (view != null)
            {
                view.Filter = delegate(object item)
                    {
                        // проверяем зачекано ли "показывать пасс"
                        bool filtered = ((LogRecord) item).Type == LogRecord.LogEventTypes.Pass &&
                                        (FilterPass.IsChecked ?? false);
                        // зачекано ли "показывать лут"
                        filtered = ((LogRecord) item).Type == LogRecord.LogEventTypes.Loot
                                       ? FilterLoot.IsChecked ?? false
                                       : filtered;
                        // зачекано ли "показывать поправки"
                        filtered = ((LogRecord) item).Type == LogRecord.LogEventTypes.Adj
                                       ? FilterAdj.IsChecked ?? false
                                       : filtered;
                        // зачекано ли "показывать разное"
                        filtered = ((LogRecord) item).Type == LogRecord.LogEventTypes.Undefined
                                       ? FilterOther.IsChecked ?? false
                                       : filtered;
                        // зачекано ли "показывать входы/выходы из мира"
                        filtered = ((LogRecord) item).Type == LogRecord.LogEventTypes.Joined
                                   || ((LogRecord) item).Type == LogRecord.LogEventTypes.LogOutWorld
                                   || ((LogRecord) item).Type == LogRecord.LogEventTypes.Online
                                   || ((LogRecord) item).Type == LogRecord.LogEventTypes.RaidEvent
                                   || ((LogRecord) item).Type == LogRecord.LogEventTypes.RaidMember
                                   || ((LogRecord) item).Type == LogRecord.LogEventTypes.Joined
                                       ? FilterLogg.IsChecked ?? false
                                       : filtered;
                        // зачекано ли "показывать события ростера"
                        filtered = ((LogRecord) item).Type == LogRecord.LogEventTypes.Roster
                                       ? FilterRoster.IsChecked ?? false
                                       : filtered;

                        return filtered;
                    };
            }
        }
    }
}