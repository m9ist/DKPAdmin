using System;
using System.Globalization;
using LogAnalyzer.Structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogAnalyzer;
using System.Linq;

namespace ModulTests
{
	[TestClass]
	public class TestLogParser
	{
        private readonly LogParser _logParser = new LogParser();

        #region тесты на распознавание разных типов переменных
        [TestMethod]
        public void TestAnalyzeLuaString_IntNode()
        {
            int[] arrI = {1, 321, 10000, 1000000, Int32.MaxValue};

            foreach (int i in arrI )
            {
                string variable = string.Format("[\"m_value\"] = {0},{1}", i, @"}");
                LuaNode res = _logParser.AnalyzeLuaString(ref variable);

                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);

                res = res.GetNodeContent().FirstOrDefault();
                Assert.IsNotNull(res);
                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Integer);
                Assert.IsTrue(res.GetIntContent() == i);
            }
        }

        [TestMethod]
        public void TestAnalyzeLuaString_StringNode()
        {
            string[] arrI = { "12", "123.2321", "123,321", "ASDdas21", "Авы234" };

            foreach (string i in arrI)
            {
                string variable = string.Format("[\"m_value\"] = \"{0}\",{1}", i, @"}");
                LuaNode res = _logParser.AnalyzeLuaString(ref variable);

                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);

                res = res.GetNodeContent().FirstOrDefault();
                Assert.IsNotNull(res);
                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.String);
                Assert.IsTrue(res.GetStringContent() == i);
            }
        }

        [TestMethod]
        public void TestAnalyzeLuaString_RealNode()
        {
            double[] arrI = { 321.321, 123.2, 23131.321 };

            foreach (double i in arrI)
            {
                string variable = string.Format("[\"m_value\"] = {0},{1}", i.ToString(CultureInfo.InvariantCulture).Replace(",", "."), @"}");
                LuaNode res = _logParser.AnalyzeLuaString(ref variable);

                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);

                res = res.GetNodeContent().FirstOrDefault();
                Assert.IsNotNull(res);
                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Real);
                Assert.IsTrue(res.GetRealContent() == i);
            }
        }

        [TestMethod]
        public void TestAnalyzeLuaString_BoolNode()
        {
            bool[] arrI = { true, false };

            foreach (bool i in arrI)
            {
                string variable = string.Format("[\"m_value\"] = {0},{1}", i ? "true" : "false", @"}");
                LuaNode res = _logParser.AnalyzeLuaString(ref variable);

                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);

                res = res.GetNodeContent().FirstOrDefault();
                Assert.IsNotNull(res);
                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Boolean);
                Assert.IsTrue(res.GetBoolContent() == i);
            }
        }

        [TestMethod]
        public void TestAnalyzeLuaString_TestEmptyArray()
        {
            string[] tests =
                {
                    "[\"test\"] = {},}",
                    "{}, -- [1]}"
                };

            foreach (var variable in tests)
            {
                string tmp = variable;
                LuaNode res = _logParser.AnalyzeLuaString(ref tmp);

                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);

                res = res.GetNodeContent().FirstOrDefault();
                Assert.IsNotNull(res);
                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);
            }
        }
        #endregion

        #region проверка на распознавание разных типов имен
        [TestMethod]
        public void TestAnalyzeLuaString_TestNameAfter()
        {
            const string name = "123";

            string variable = string.Format("{0}, -- [{1}]", @"{}", name);

            LuaNode res = _logParser.AnalyzeLuaString(ref variable);

            Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);

            res = res.GetNodeContent().FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.NodeName == name);
        }

        [TestMethod]
        public void TestAnalyzeLuaString_TestNameBefore()
        {
            string[] names = { "albert", "213", "ADS324sdf", "Авыаор123_" };

            foreach (string name in names)
            {
                string variable = string.Format("[\"{0}\"] = 123,{1}", name, @"}");

                LuaNode res = _logParser.AnalyzeLuaString(ref variable);

                Assert.IsTrue(res.NodeType == LuaNode.LuaNodeType.Node);

                res = res.GetNodeContent().FirstOrDefault();
                Assert.IsNotNull(res);
                Assert.IsTrue(res.NodeName == name);
            }
        }
        #endregion

        // преобразовение времени
        // 1369342447 24,05,2013 03,54,07
        // 1369342024 24,05,2013 03,47,04
        // 1369340890 24.05.13 03.28.10
        // 1368992269 20.05.13 02.37.49
    }
}
