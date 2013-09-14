/* Copyright 2011 Niklas Rother
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicLua;

namespace DynamicLuaTests
{
    [TestClass]
    [DeploymentItem("lua52.dll")]
    public class MyTests
    {
        private TestContext testContextInstance;
        private dynamic lua;

        private bool delegateTest = false;

        public TestContext TestContext
        { get { return testContextInstance; } set { testContextInstance = value; } }

        [TestInitialize]
        public void Init()
        {
            lua = new DynamicLua.DynamicLua();
        }

        [TestCleanup]
        public void Cleanup()
        {
            lua.Dispose();
            lua = null;
        }

        [TestMethod]
        public void TestIndexing()
        {
            lua.indexteststring = "a string";
            lua.indextestint = 42;

            Assert.AreEqual(lua.indexteststring, "a string");
            Assert.AreEqual(lua.indextestint, 42);
        }

        [TestMethod]
        public void TestDynamicInvoke()
        {
            lua("dyninvoketest = 'a string'");
            Assert.AreEqual(lua.dyninvoketest, "a string");
        }

        [TestMethod]
        public void TestTables()
        {
            lua("tabletest1 = { tmp = 156}");
            Assert.AreEqual(lua.tabletest1.tmp, 156);

            lua.NewTable("tabletest2");
            lua.tabletest2.tmp = 157;
            Assert.AreEqual(lua.tabletest2.tmp, 157);

            Assert.IsNull(lua.tabletest3);
        }

        [TestMethod]
        [DeploymentItem("filetest1.lua")]
        [DeploymentItem("filetest2.lua")]
        public void TestFiles()
        {
            lua.DoFile(@"filetest1.lua");
            Assert.AreEqual(lua.filetest1, "ok");
            lua.filetestfunc1();
            Assert.AreEqual(lua.filetest2, true);

            lua.LoadFile(@"filetest2.lua")();
            Assert.AreEqual(lua.filetest3, "ok");
            lua.filetestfunc2();
            Assert.AreEqual(lua.filetest4, true);
        }

        [TestMethod]
        public void TestLoadString()
        {
            lua.LoadString("a = true")();
            Assert.IsTrue(lua.a);
        }

        [TestMethod]
        public void TestDelegates()
        {
            delegateTest = false;
            lua.delegatetestfunc = new Action(this.TestDelegatesHelper);
            lua.delegatetestfunc();
            Assert.IsTrue(delegateTest);
            delegateTest = false;
            lua("delegatetestfunc()");
            Assert.IsTrue(delegateTest);
            delegateTest = false;
            dynamic func = lua.delegatetestfunc;
            Assert.IsInstanceOfType(func, typeof(DynamicLuaFunction));
            func();
            Assert.IsTrue(delegateTest);
        }

        [TestMethod]
        public void TestUnwrapping()
        {
            lua("a=false");
            lua("function test() a=true end");
            dynamic func = lua.test;
            func();
            Assert.IsTrue(lua.a);
        }

        private void TestDelegatesHelper()
        {
            delegateTest = true;
        }

        [TestMethod]
        public void TestConvertingAutoConvert()
        {
            string test1 = lua("return 'test'");
            foreach (var item in lua("return 1,2,3,4"))
            {
                Assert.IsInstanceOfType(item, typeof(double));
            }
        }

        [TestMethod]
        public void TestDispose()
        {
            lua.Dispose();
        }

        [TestMethod]
        public void TestIndexingAndCall()
        {
            lua.del = new Action(TestDelegatesHelper);
            delegateTest = false;
            lua["del"]();
            Assert.IsTrue(delegateTest);
            lua("del=nil; function del() test=true end");
            lua["del"]();
            Assert.IsTrue(lua.test);
        }

        [TestMethod]
        public void TestFunctionsInTable()
        {
            lua.NewTable("tab");
            lua.tab.test = new Action(TestDelegatesHelper);
            delegateTest = false;
            lua.tab.test();
            Assert.IsTrue(delegateTest);
            dynamic tab = lua.tab;
            delegateTest = false;
            tab.test();
            Assert.IsTrue(delegateTest);
        }

        [TestMethod]
        public void TestMetamethodAdd() //And sub, mul, etc.
        {
            lua("c1 = { num = 42 }; c2 = { num = 7 }");
            lua("mtc = { __add = function(t, other) return t.num + other.num end }");
            lua("setmetatable(c1, mtc)");
            lua("setmetatable(c2, mtc)");

            Assert.AreEqual(49.0, lua.c1 + lua.c2);
        }

        [TestMethod]
        public void TestMetamethodIndex()
        {
            dynamic tab = lua.NewTable("tab");
            dynamic mt = lua.NewTable("mt");
            mt.__index = new Func<dynamic, string, double>((t, i) => Math.Pow(int.Parse(i), 2));
            tab.SetMetatable(mt);

            for (int i = 0; i <= 10; i++)
            {
                Assert.AreEqual(i * i, tab[i]);
            }
        }

        [TestMethod]
        public void TestMetamethodNewIndex()
        {
            bool test = false;
            dynamic tab = lua.NewTable("tab");
            dynamic mt = lua.NewTable("mt");
            mt.__newindex = new Action<dynamic, string, object>((t, i, v) => test = true);
            tab.SetMetatable(mt);

            tab.abc = "cdf";

            Assert.IsTrue(test);
        }

        [TestMethod]
        public void TestMetamethodLogic()
        {
            dynamic tab1 = lua.NewTable("tab1");
            tab1.num = 1;
            dynamic tab2 = lua.NewTable("tab2");
            tab2.num = 2;
            dynamic mt = lua.NewTable("mt");
            mt.__lt = new Func<dynamic, dynamic, bool>((t, other) => t["num"] < other["num"]);
            mt.__le = new Func<dynamic, dynamic, bool>((t, other) => t["num"] <= other["num"]);
            tab1.SetMetatable(mt);
            tab2.SetMetatable(mt);

            Assert.IsTrue(tab1 < tab2);
            Assert.IsFalse(tab1 > tab2);

            tab1.num = 2;

            Assert.IsTrue(tab1 <= tab2);
            Assert.IsTrue(tab1 >= tab2);
        }

        [TestMethod]
        public void TestMetamethodNegate()
        {
            dynamic tab = lua.NewTable("tab");
            tab.num = 42;
            dynamic mt = lua.NewTable("mt");
            mt.__unm = new Func<dynamic, double>((t) => -t["num"]);
            tab.SetMetatable(mt);

            Assert.AreEqual(-42.0, -tab);
        }

        [TestMethod]
        public void TestMetamethodToString()
        {
            dynamic tab = lua.NewTable("tab");
            dynamic mt = lua.NewTable("mt");
            mt.__tostring = new Func<dynamic, string>((t) => "tostring");
            tab.SetMetatable(mt);

            Assert.AreEqual("tostring", tab.ToString());
            Assert.AreEqual("tostring", (string)tab);
        }

        [TestMethod]
        public void TestMetamethodCall()
        {
            dynamic tab = lua.NewTable("tab");
            dynamic mt = lua.NewTable("mt");
            mt.__call = new Func<dynamic, object>((t) => "call no args");
            tab.SetMetatable(mt);

            Assert.AreEqual("call no args", (string)tab()); //Need explicit cast for Assert...
        }

        [TestMethod]
        public void TestMetamethodPower()
        {
            dynamic tab = lua.NewTable("tab");
            tab.num = 42;
            dynamic mt = lua.NewTable("mt");
            mt.__pow = new Func<dynamic, double, double>((t, p) => Math.Pow(t["num"], p));
            tab.SetMetatable(mt);

            Assert.AreEqual(Math.Pow(42.0, 2.0), tab.Power(2.0)[0]);
        }

        [TestMethod]
        public void TestGetMetatable()
        {
            dynamic tab = lua.NewTable("tab");
            dynamic mt = lua.NewTable("mt");
            tab.SetMetatable(mt);

            Assert.AreEqual(mt, tab.GetMetatable());
        }

        [TestMethod]
        public void TestTableOperators()
        {
            dynamic tab = lua.NewTable("tab");
            dynamic tab2 = lua.tab; //Not the same CLR instance, but the same table!

            Assert.AreEqual(tab, tab2);
            Assert.IsTrue(tab == tab2);
            Assert.IsFalse(tab != tab2);
            tab.GetHashCode();
        }

        [TestMethod]
        public void TestTableEnumerator()
        {
            dynamic tab = lua("return {a = 1, b = 2, c = 3}")[0];

            int i = 0;
            foreach (KeyValuePair<object, object> kvp in tab)
            {
                string key = (string)kvp.Key;
                double value = (double)kvp.Value;

                switch (key)
                {
                    case "a":
                        Assert.AreEqual(1.0, value);
                        break;
                    case "b":
                        Assert.AreEqual(2.0, value);
                        break;
                    case "c":
                        Assert.AreEqual(3.0, value);
                        break;
                }
                i++;
            }
            Assert.AreEqual(3, i);
        }

        [TestMethod]
        public void TestArrayEnumerators()
        {
            dynamic arr = lua("return 'a','b','c'");

            int i = 0;
            foreach (string value in arr)
            {
                switch (i)
                {
                    case 0:
                        Assert.AreEqual("a", value);
                        break;
                    case 1:
                        Assert.AreEqual("b", value);
                        break;
                    case 2:
                        Assert.AreEqual("c", value);
                        break;
                }
                i++;
            }
            Assert.AreEqual(3, i);

            i = 0;
            foreach (object value in arr)
            {
                switch (i)
                {
                    case 0:
                        Assert.AreEqual("a", value);
                        break;
                    case 1:
                        Assert.AreEqual("b", value);
                        break;
                    case 2:
                        Assert.AreEqual("c", value);
                        break;
                }
                i++;
            }
            Assert.AreEqual(3, i);
        }

        [TestMethod]
        public void TestTablePassing()
        {
            dynamic tab = lua("return {a = 1, b = 2, c = 3}");
            lua.tab = tab;
            Assert.AreEqual("table", lua("return type(tab)")[0]);

        }
    }
}
