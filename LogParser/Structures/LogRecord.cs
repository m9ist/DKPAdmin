using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        private Dictionary<LogEventTypes, string> _logEvents = new Dictionary<LogEventTypes, string>
            {
                {LogEventTypes.Undefined, "неопределено"},
                {LogEventTypes.Pass, "пасс"},
                {LogEventTypes.Loot, "лут"},
                {LogEventTypes.Adj, "поправка"},
                {LogEventTypes.EnterWorld, "входит"},
                {LogEventTypes.LogOutWorld, "выходит"},
                {LogEventTypes.RaidMember, "член"},
                {LogEventTypes.RaidEvent, "рейд"},
                {LogEventTypes.Invite, "инв"},
                {LogEventTypes.Online, "онлайн"},
                {LogEventTypes.Roster, "ростер"},
                {LogEventTypes.Joined, "присоединился"}
            };


        private int _mvalue;
        private string _sevent = string.Empty;
        private string _svalue = string.Empty;
        private DateTime _time;
        private string _user = string.Empty;

        /// <summary> создает экземпляр записи лога </summary>
        /// <param name="logNode"> ветка в которой содержится запись лога </param>
        public LogRecord(LuaNode logNode)
        {
            // проверяем что нам досталось
            if (logNode.NodeType != LuaNode.LuaNodeType.Node)
                throw new Exception("Wrong input LuaNode, it should be Node type");

            // забиваем наши поля
            foreach (LuaNode iNode in logNode.GetNodeContent())
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
                        Time = LogParser.ConvertWowTime(iNode.GetIntContent() ?? 0);
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
            Type = _logEvents.SingleOrDefault(i => _sevent.IndexOf(i.Value, StringComparison.InvariantCulture) >= 0).Key;
        }

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
        public DateTime Time
        {
            get { return _time; }
            private set
            {
                OnPropertyChanged("time");
                _time = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}