using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogAnalyzer.Structures
{
    /// <summary> структура, содержащая в себе одну запись лога </summary>
    public class LogRecord
    {
        /// <summary> типы события в логе </summary>
        public enum LogEventTypes
        {
            Roster = 0,
            Pass = 1,
            Loot = 2,
            Adj = 3,
            EnterWorld = 4,
            LogOutWorld = 5,
            RaidMember = 6,
            RaidEvent = 7,
            Invite = 8,
            Online = 9,
        }

        /// <summary> сопоставление перечислению LogEventTypes </summary>
        private string[] _logEventTypes =
            {
                "ростер", 
                "пасс",
                "лут",
                "поправка",
                "входит",
                "выходит",
                "член",
                "рейд",
                "инв",
                "онлайн",
            };

        /// <summary> персонаж - главное лицо записи (либо ростер тут забит) </summary>
        public string User { get; private set; }
        /// <summary> зачастую значение поправки </summary>
        public int MValue { get; private set; }
        /// <summary> текст поправки </summary>
        public string SValue { get; private set; }
        /// <summary> тип события лога </summary>
        public string SEvent { get; private set; }
        /// <summary> время события </summary>
        public string Time { get; private set; }

        /// <summary> создает экземпляр записи лога </summary>
        /// <param name="logNode"> ветка в которой содержится запись лога </param>
        public LogRecord(LuaNode logNode)
        {
            if (logNode.NodeType != LuaNode.LuaNodeType.Node)
                throw new Exception("Wrong input LuaNode, it should be Node type");

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
                        Time = iNode.GetStringContent();
                        break;
                    case "s_value":
                        SValue = iNode.GetStringContent();
                        break;
                    case "event":
                        SEvent = iNode.GetStringContent();
                        break;
                    default:
                        throw new Exception("It's not a log node!");
                        break;
                }
            }
        }
    }
}
