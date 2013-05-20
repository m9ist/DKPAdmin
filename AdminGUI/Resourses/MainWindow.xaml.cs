using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WPF.MDI;

namespace AdminGUI.Resourses
{
    /// <summary> вспомогательная структура для хранения связки окно - запись в меню </summary>
    struct SaveWindowStructs
    {
        /// <summary> пункт меню, ассоциированный с данным окном </summary>
        private readonly MenuItem _mi;
        /// <summary> окошко </summary>
        private readonly MdiChild _mdiChild;
        /// <summary> инициализация структуры </summary>
        /// <param name="child">окно</param>
        /// <param name="menuItem">пункт меню, который привязывается к данному окну</param>
        public SaveWindowStructs(MdiChild child, MenuItem menuItem)
        {
            _mi = menuItem;
            _mdiChild = child;
        }
        /// <summary> возвращаем пункт меню связанный в этой структуре </summary>
        /// <returns></returns>
        public MenuItem GetMenu()
        {
            return _mi;
        }
        /// <summary> проверяет является ли окно - окном привязанным к структуре </summary>
        /// <param name="child">окошко</param>
        /// <returns>true если эта структура создана по данному окну</returns>
        public bool IsChild(MdiChild child)
        {
            return Equals(child, _mdiChild);
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary> список окон и присоединенных к ним пунктов меню </summary>
        private readonly List<SaveWindowStructs> _windowList = new List<SaveWindowStructs>();

        /// <summary> конструктор </summary>
        public MainWindow()
        {
            InitializeComponent();
            Title += string.Format(" (version {0})", Assembly.GetExecutingAssembly().GetName().Version);
        }

        /// <summary> создает окошко для анализа лога </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_LogAnalyzer(object sender, RoutedEventArgs e)
        {
            const string title = "Log analyse tool";
            // создаем окошко
            var tmp = new MdiChild
                {
                    Content = new WindowManagementLog(),
                    Title = title,
                    MinimizeBox = false
                };
            // добавляем на экран
            AddNewWindow(tmp, title);
        }

        /// <summary> добавляет на экран окно + запоминает в структурах для дальнейших манипуляций </summary>
        /// <param name="window">созданное окно</param>
        /// <param name="title">заголовок окна (для идентификации)</param>
        private void AddNewWindow(MdiChild window, string title)
        {
            // доавбляем на экран
            MdiWindowContainer.Children.Add(window);
            // добавляем обработчик к закрытию окна
            window.Closed += MenuItemClose;
            // добавляем его в список открытых окон
            MenuItem mi;
            MenuWindowList.Items.Add(mi = new MenuItem { Header = title });
            mi.Click += (o, args) => window.Focus();
            // запоминаем его в списке
            _windowList.Add(new SaveWindowStructs(window, mi));
        }

        /// <summary> удаляет себя из списка окон </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemClose(object sender, RoutedEventArgs e)
        {
            // ищем менюшку связанную с этим объектом
            MenuItem mi = null;
            SaveWindowStructs? savedStruct = null;
            foreach (SaveWindowStructs structse in _windowList)
            {
                if (structse.IsChild((MdiChild) sender))
                {
                    mi = structse.GetMenu();
                    savedStruct = structse;
                }
            }
            // убиваем менюшку и запись в списке
            if (mi != null)
            {
                MenuWindowList.Items.Remove(mi);
                _windowList.Remove((SaveWindowStructs) savedStruct);
            }
        }

        /// <summary> создает окошко по выводу XML файла </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_XmlOutput(object sender, RoutedEventArgs e)
        {
            const string title = "XML output";
            // создаем окошко
            var tmp = new MdiChild
            {
                Content = new XmlOutputWindow(),
                Title = title,
                MinimizeBox = false
            };
            // добавляем на экран
            AddNewWindow(tmp, title);
        }
    }
}
