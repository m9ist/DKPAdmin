using System;
using System.Linq;
using LogAnalyzer.Structures;

namespace LogAnalyzer
{
    /// <summary> логика парсинга .lua файла </summary>
    public class LogParser
    {
        /// <summary> значения, которые являются специальным символами / на выброс </summary>
        private readonly string[] _emptyChars =
            {
                "\r",
                "\n",
                "\t",
                "hagakureDB = {",
                "\\\""
            };

        /// <summary> очищает строчку от ненужных символов </summary>
        /// <param name="data">строка для очистки</param>
        /// <returns> очищенная от спец. символов строка </returns>
        public string CleanEmptyChars(string data)
        {
            // очищаем от спец символов
            string iteration = _emptyChars.Aggregate(data, (current, s) => current.Replace(s, string.Empty));
            
            // убиваем последний @"{"
            return iteration.Substring(0, iteration.Length - 1);
        }

        /// <summary> непосредственное распарсивание в дерево .lua файла </summary>
        /// <param name="data"> подготовленная к распарсиванию строка </param>
        /// <returns> распарсенный текст </returns>
        public LuaNode AnalyzeLuaString(ref string data)
        {
            var result = new LuaNode {NodeType = LuaNode.LuaNodeType.Node};

            // запоминаем начальное значение длины для проверки остановки в цикле
            int oldLength = data.Length + 1;

            // запускаем цикл на добавление в текущее звено значений
            while (data.Length > 0 && data.Length < oldLength)
            {
                // начинаем с запоминания oldLength
                oldLength = data.Length;

                // ищем первое вхождение } и {
                int nextClose = data.IndexOf(@"}", StringComparison.Ordinal);
                int nextOpen = data.IndexOf(@"{", StringComparison.Ordinal);
                int nextComma = data.IndexOf(@",[", StringComparison.Ordinal);
                int nextCommaTmp = data.IndexOf(@",}", StringComparison.Ordinal);
                if (nextComma == -1 || (nextComma > nextCommaTmp && nextCommaTmp != -1)) nextComma = nextCommaTmp;

                // сначала надо проверить идет ли сначала закрывающаяся скобка, если так, то надо завершать цикл
                if (nextClose == 0)
                    break;

                // если открытие идет раньше, то значит текущий узел - это node и надо дальше отдавать на 
                // распарсинг вычитая начало строки и распарсивая имя если есть
                if (nextOpen > -1 && nextClose > nextOpen && nextComma > nextOpen)
                {
                    // вытаскиваем с начала имя есть есть
                    string name = data.Substring(0, nextOpen);

                    // укорачиваем строку
                    data = data.Substring(nextOpen + 1, data.Length - nextOpen - 1);

                    // запускаем на дальнейший анализ
                    LuaNode toAdd = AnalyzeLuaString(ref data);

                    if (data.IndexOf(@"},", StringComparison.Ordinal) != 0)
                        throw new Exception("Some error in parsing!");

                    // убиваем закрывающуюся скобку
                    data = data.Substring(data.IndexOf(@"},", StringComparison.Ordinal) + 2, data.Length - data.IndexOf(@"},", StringComparison.Ordinal) - 2);

                    // если в начале было пустое имя, вытаскиваем его из конца
                    if (name == string.Empty)
                    {
                        int iO = data.IndexOf(@"-- [", StringComparison.Ordinal) + 4;
                        int iE = data.IndexOf(@"]", StringComparison.Ordinal);
                        name = data.Substring(iO, iE - iO);

                        data = data.Substring(iE + 1, data.Length - iE - 1);
                    }
                    else
                    {
                        // надо убрать [\" и \"]
                        name = name.Substring(2, name.Length - 7);
                    }

                    // добавляем имя
                    toAdd.NodeName = name;

                    // добавляем узел с указанным именем
                    result.AddValue(toAdd);
                }
                else if (nextComma > 0 && (nextClose == -1 || nextClose > nextComma))// если сначала идет запятая, те у нас строка типа ["name"] = value,
                {
                    // у нас выражение типа ["name"] = value,
                    int nameStarts = data.IndexOf("\"", StringComparison.Ordinal);
                    int nameEnds = data.IndexOf("\"", nameStarts + 1, StringComparison.Ordinal);
                    int equalPlace = data.IndexOf(" = ", StringComparison.Ordinal);

                    // если что-то не так выдаем эксепшн
                    if (nameStarts == -1 || nameEnds == -1 || equalPlace == -1)
                        throw new Exception("Wrong format for text string!");

                    // вытаскиваем секцию с именем
                    string name = data.Substring(nameStarts + 1, nameEnds - nameStarts - 1);
                    // значение
                    string value = data.Substring(equalPlace + 3, nextComma - equalPlace - 3);

                    // и укорачиваем строку на запятую
                    data = data.Substring(nextComma + 1, data.Length - nextComma - 1);

                    // создаем статус конвертации
                    bool convert = false;
                    var toAdd = new LuaNode {NodeName = name};
                    // собственно добавляем сразу в результат
                    result.AddValue(toAdd);

                    // теперь проверяем что у нас имеется
                    // либо bool
                    if (value == "true" || value == "false")
                    {
                        toAdd.NodeType = LuaNode.LuaNodeType.Boolean;
                        toAdd.AddValue(value == "true");
                        convert = true;
                    }

                    // либо int
                    int res;
                    if (!convert && int.TryParse(value, out res))
                    {
                        // итого у нас int
                        toAdd.NodeType = LuaNode.LuaNodeType.Integer;
                        toAdd.AddValue(res);
                        convert = true;
                    }

                    // либо float
                    double fres;
                    if (!convert && double.TryParse(value.Replace(".", ","), out fres))
                    {
                        toAdd.NodeType = LuaNode.LuaNodeType.Real;
                        toAdd.AddValue(fres);
                        convert = true;
                    }

                    // либо string
                    if (!convert)
                    {
                        toAdd.NodeType = LuaNode.LuaNodeType.String;
                        toAdd.AddValue(value.Replace("\"", string.Empty));
                    }
                }
            }

            return result;
        }

        /// <summary> ищет в дереве узлов узел с логом и выдает его </summary>
        /// <param name="data"> распарсенный в дерево .lua </param>
        /// <param name="nodeName"> имя искомой ветки дерева </param>
        /// <returns> ветка с именем log </returns>
        public LuaNode SearchNodeWithName(LuaNode data, string nodeName)
        {
            return data.NodeName == nodeName
                        // если искомая ветка искомая - выдаем ее на результат
                       ? data
                       : data.NodeType != LuaNode.LuaNodeType.Node
                                // если узел не содержит внутри узел то выдаем null
                             ? null
                                // иначе надо запустить на проверку каждый его член ^^
                             : data.GetNodeContent()
                                   .Select(iNode => SearchNodeWithName(iNode, nodeName))
                                   .FirstOrDefault(search => search != null);
        }
    }
}
