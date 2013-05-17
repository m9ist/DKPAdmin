using System;
using System.ComponentModel;

namespace LogAnalyzer.Structures
{
    /// <summary> структура, содержащая в себе одну запись лога </summary>
    public class LogRecord : INotifyPropertyChanged
    {
        /// <summary> типы события в логе </summary>
        public enum LogEventTypes
        {
            Undefined = 0,
            Pass = 1,
            Loot = 2,
            Adj = 3,
            EnterWorld = 4,
            LogOutWorld = 5,
            RaidMember = 6,
            RaidEvent = 7,
            Invite = 8,
            Online = 9,
            Roster = 10,
            Joined = 11,
        }

        /// <summary> сопоставление перечислению LogEventTypes </summary>
        private readonly string[] _logEventTypes =
            {
                "неопределено",
                "пасс",
                "лут",
                "поправка",
                "входит",
                "выходит",
                "член",
                "рейд",
                "инв",
                "онлайн",
                "ростер", 
                "присоединился"
            };

        private string _user = string.Empty;
        private int _mvalue;
        private string _svalue = string.Empty;
        private string _sevent = string.Empty;
        private int _time;

        public LogEventTypes Type { private set; get; }

        /// <summary> персонаж - главное лицо записи (либо ростер тут забит) </summary>
        public string User
        {
            get { return _user; }
            private set
            {
                OnPropertyChanged("user");
                _user = value;
            }
        }

        /// <summary> зачастую значение поправки </summary>
        public int MValue
        {
            get { return _mvalue; }
            private set
            {
                OnPropertyChanged("mvalue");
                _mvalue = value;
            }
        }

        /// <summary> текст поправки </summary>
        public string SValue
        {
            get { return _svalue; }
            private set
            {
                OnPropertyChanged("svalue");
                _svalue = value;
            }
        }

        /// <summary> тип события лога </summary>
        public string SEvent
        {
            get { return _sevent; }
            private set
            {
                OnPropertyChanged("sevent");
                _sevent = value;
            }
        }

        /// <summary> время события </summary>
        public int Time
        {
            get { return _time; }
            private set
            {
                OnPropertyChanged("time");
                _time = value;
            }
        }

        /// <summary> создает экземпляр записи лога </summary>
        /// <param name="logNode"> ветка в которой содержится запись лога </param>
        public LogRecord(LuaNode logNode)
        {
            // проверяем что нам досталось
            if (logNode.NodeType != LuaNode.LuaNodeType.Node)
                throw new Exception("Wrong input LuaNode, it should be Node type");

            // забиваем наши поля
            foreach (var iNode in logNode.GetNodeContent())
            {
                switch (iNode.NodeName)
                {
                    case "m_value":
                        MValue = iNode.GetIntContent() ?? 0;
                        break;
                    case "user":
                        User = iNode.GetStringContent();
                        break;
                    case "t":
                        Time = iNode.GetIntContent() ?? 0;
                        break;
                    case "s_value":
                        SValue = iNode.GetStringContent();
                        break;
                    case "event":
                        SEvent = iNode.GetStringContent();
                        break;
                    default:
                        throw new Exception("It's not a log node!");
                }
            }

            // определяем тип поля
            for (int i = 0; i < _logEventTypes.Length; i++)
            {
                if (_sevent.IndexOf(_logEventTypes[i], StringComparison.InvariantCulture) >= 0)
                    Type = (LogEventTypes)i;
            }
        }

        public LogRecord(LuaNode logNode, int firstTime)
            : this(logNode)
        {
            Time = Time - firstTime;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
