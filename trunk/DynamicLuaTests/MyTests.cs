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

        #region Zusätzliche Testattribute
        //
        // Sie können beim Schreiben der Tests folgende zusätzliche Attribute verwenden:
        //
        // Verwenden Sie ClassInitialize, um vor Ausführung des ersten Tests in der Klasse Code auszuführen.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Verwenden Sie ClassCleanup, um nach Ausführung aller Tests in einer Klasse Code auszuführen.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Mit TestInitialize können Sie vor jedem einzelnen Test Code ausführen. 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Mit TestCleanup können Sie nach jedem einzelnen Test Code ausführen.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

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
        public void TestFiles()
        {
            lua.DoFile(@"D:\c_sharp\Programme\DynamicLua\DynamicLuaTests\bin\Debug\filetest1.lua"); //TODO: No absolute paths!
            Assert.AreEqual(lua.filetest1, "ok");
            lua.filetestfunc1();
            Assert.AreEqual(lua.filetest2, true);

            lua.LoadFile(@"D:\c_sharp\Programme\DynamicLua\DynamicLuaTests\bin\Debug\filetest2.lua")();
            Assert.AreEqual(lua.filetest3, "ok");
            lua.filetestfunc2();
            Assert.AreEqual(lua.filetest4, true);
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
        public void TestsUnwrapping()
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
    }
}
