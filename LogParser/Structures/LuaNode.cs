using System;
using System.Collections.Generic;
using System.Globalization;

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
        public enum LuaNodeType
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

        /// <summary> выдает контент типа int </summary>
        /// <returns> null если контент другого типа </returns>
        public int? GetIntContent()
        {
            return _type == LuaNodeType.Integer
                       ? (int) _content
                       : (int?) null;
        }

        /// <summary> выдает контент типа List[LuaNode] </summary>
        /// <returns> null если контент друго типа </returns>
        public List<LuaNode> GetNodeContent()
        {
            return _type == LuaNodeType.Node
                       ? (List<LuaNode>) _content
                       : new List<LuaNode>();
        }

        /// <summary> выдает контент типа string </summary>
        /// <returns> null если контент другого типа </returns>
        public string GetStringContent()
        {
            return _type == LuaNodeType.String
                       ? (string) _content
                       : _type == LuaNodeType.Integer
                             ? ((int) _content).ToString(CultureInfo.InvariantCulture)
                             : null;
        }

        /// <summary> выдает контент типа boolean </summary>
        /// <returns> null если контент другого типа </returns>
        public bool? GetBoolContent()
        {
            return _type == LuaNodeType.Boolean 
                       ? (bool) _content
                       : (bool?) null;
        }

        /// <summary> выдает контент типа real </summary>
        /// <returns> null если контент другого типа </returns>
        public float? GetRealContent()
        {
            return _type == LuaNodeType.Real
                       ? (float) _content
                       : (float?) null;
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
