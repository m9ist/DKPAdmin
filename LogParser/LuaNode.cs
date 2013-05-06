using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogAnalyzer
{
    /// <summary> сущность узла  </summary>
    public class LuaNode
    {
        /// <summary> тип узла </summary>
        private LuaNodeType _type;

        /// <summary> содержание узла, здесь будет ссылка на значение, которое при извлечении надо распаковывать </summary>
        private object _content;

        /// <summary> имя узла / название параметра </summary>
        public string NodeName;

        /// <summary> предопределенные типы для значения узла </summary>
        public enum LuaNodeType : int
        {
            String,
            Boolean,
            Integer,
            Real,
            Node,
            Empty
        };

        /// <summary> конструктор, по умолчанию задает узел пустым </summary>
        public LuaNode()
        {
            NodeType = LuaNodeType.Empty;
            NodeName = string.Empty;
        }

        /// <summary> устанавливает/читает тип узла </summary>
        public LuaNodeType NodeType
        {
            get { return _type; }
            set
            {
                _type = value;
                switch (value)
                {
                    case LuaNodeType.Empty:
                        _content = null;
                        break;
                    case LuaNodeType.Node:
                        _content = new List<LuaNode>();
                        break;
                }
            }
        }

        #region перегрузка AddValue(value)
        /// <summary> задает/добавляет значение для узла </summary>
        /// <param name="value"> значение узла </param>
        public void AddValue(int value)
        {
            if (_type == LuaNodeType.Integer) _content = value;
            else throw new Exception("У узла выставлен другой тип данных (не int).");
        }

        public void AddValue(string value)
        {
            if (_type == LuaNodeType.String) _content = value;
            else throw new Exception("У узла выставлен другой тип данных (не string).");
        }

        public void AddValue(double value)
        {
            if (_type == LuaNodeType.Real) _content = value;
            else throw new Exception("У узла выставлен другой тип данных (не real).");
        }

        public void AddValue(bool value)
        {
            if (_type == LuaNodeType.Boolean) _content = value;
            else throw new Exception("У узла выставлен другой тип данных (не boolean).");
        }

        public void AddValue(LuaNode value)
        {
            if (_type == LuaNodeType.Node)
            {
                ((List<LuaNode>)_content).Add(value);
            }
            else throw new Exception("У узла выставлен другой тип данных (не node).");
        }
        #endregion
    }
}
