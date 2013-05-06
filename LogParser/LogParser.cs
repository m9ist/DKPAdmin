﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LogAnalyzer
{
    /// <summary> логика парсинга .lua файла </summary>
    public class LogParser
    {
        /// <summary> значения, которые являются специальным символами / на выброс </summary>
        private string[] _emptyChars =
            {
                "\r",
                "\n",
                "\t",
                "hagakureDB = {",
                "\\\""
            };

        /// <summary> читаем данные из файла в строку </summary>
        /// <param name="path"> путь до файла </param>
        /// <returns> строка - содержимое файла </returns>
        public string ReadFile(string path)
        {
            var reader = new StreamReader(path, Encoding.UTF8);
            try
            {
                string content = reader.ReadToEnd();

                return content;
            }
            catch
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary> очищает строчку от ненужных символов </summary>
        /// <param name="data">строка для очистки</param>
        /// <returns> очищенная от спец. символов строка </returns>
        public string CleanEmptyChars(string data)
        {
            string iteration = _emptyChars.Aggregate(data, (current, s) => current.Replace(s, string.Empty));

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
                int nextClose = data.IndexOf(@"}");
                int nextOpen = data.IndexOf(@"{");
                int nextComma = data.IndexOf(@",");

                // если открытие идет раньше, то значит текущий узел - это node и надо дальше отдавать на 
                // распарсинг вычитая начало строки и распарсивая имя если есть
                if (nextOpen > -1 && nextClose > nextOpen && nextComma > nextOpen)
                {
                    // вытаскиваем с начала имя есть есть
                    string name = data.Substring(0, nextOpen);

                    // укорачиваем строку
                    string next = data.Substring(nextOpen + 1, data.Length - nextOpen - 1);

                    // запускаем на дальнейший анализ
                    //LuaNode toAdd = AnalyzeLuaString()

                    // если в начале было пустое имя, вытаскиваем его из конца
                    if (name == string.Empty)
                    {
                        
                    }

                    // и убиваем закрывающуюся скобку
                }
                else if (nextComma > 0 && (nextClose == -1 || nextClose > nextComma))// если сначала идет запятая, те у нас строка типа ["name"] = value,
                {
                    // у нас выражение типа ["name"] = value,
                    int nameStarts = data.IndexOf("\"");
                    int nameEnds = data.IndexOf("\"", nameStarts + 1);
                    int equalPlace = data.IndexOf(" = ");

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
    }
}
