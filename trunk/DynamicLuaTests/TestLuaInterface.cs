//Tests from the Original LuaInterface Project

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicLuaTests
{
    [TestClass]
    public class TestLuaInterface
    {
        private dynamic lua;

        [TestInitialize]
        public void Init()
        {
            lua = new DynamicLua.DynamicLua();
            //Make the luanet methods easier to access
            lua("load_assembly = luanet.load_assembly");
            lua("import_type = luanet.import_type");

            GC.Collect();  // runs GC to expose unprotected delegates
        }

        [TestCleanup]
        public void Destroy()
        {
            lua = null;
        }

        /*
         * Tests if DoString is correctly returning values
         */
        [TestMethod]
        public void DoString()
        {
            object[] res = lua("a=2\nreturn a,3");
            Assert.AreEqual(res[0], 2.0);
            Assert.AreEqual(res[1], 3.0);
        }
        /*
         * Tests getting of global numeric variables
         */
        [TestMethod]
        public void GetGlobalNumber()
        {
            lua("a=2");
            double num = lua.a;
            Assert.AreEqual(num, 2);
        }
        /*
         * Tests setting of global numeric variables
         */
        [TestMethod]
        public void SetGlobalNumber()
        {
            lua("a=2");
            lua["a"] = 3;
            double num = lua.a;
            Assert.AreEqual(num, 3);
        }
        /*
         * Tests getting of numeric variables from tables
         * by specifying variable path
         */
        [TestMethod]
        public void GetNumberInTable()
        {
            lua("a={b={c=2}}");
            double num = lua.a.b.c;
            Assert.AreEqual(num, 2);
        }
        /*
         * Tests setting of numeric variables from tables
         * by specifying variable path
         */
        [TestMethod]
        public void SetNumberInTable()
        {
            lua("a={b={c=2}}");
            lua["a.b.c"] = 3;
            double num = lua.a.b.c;
            Assert.AreEqual(num, 3);
        }
        /*
         * Tests getting of global string variables
         */
        [TestMethod]
        public void GetGlobalString()
        {
            lua("a=\"test\"");
            string str = lua.a;
            Assert.AreEqual(str, "test");
        }
        /*
         * Tests setting of global string variables
         */
        [TestMethod]
        public void SetGlobalString()
        {
            lua("a=\"test\"");
            lua["a"] = "new test";
            string str = lua.a;
            Assert.AreEqual(str, "new test");
        }
        /*
         * Tests getting of string variables from tables
         * by specifying variable path
         */
        [TestMethod]
        public void GetStringInTable()
        {
            lua("a={b={c=\"test\"}}");
            string str = lua.a.b.c;
            Assert.AreEqual(str, "test");
        }
        /*
         * Tests setting of string variables from tables
         * by specifying variable path
         */
        [TestMethod]
        public void SetStringInTable()
        {
            lua("a={b={c=\"test\"}}");
            lua["a.b.c"] = "new test";
            string str = lua.a.b.c;
            Assert.AreEqual(str, "new test");
        }
        /*
         * Tests getting and setting of global table variables
         */
        [TestMethod]
        public void GetAndSetTable()
        {
            lua("a={b={c=2}}\nb={c=3}");
            dynamic tab = lua.b;
            lua["a.b"] = tab;
            double num = lua.a.b.c;
            Assert.AreEqual(num, 3);
        }
        /*
         * Tests getting of numeric field of a table
         */
        [TestMethod]
        public void GetTableNumericField1()
        {
            lua("a={b={c=2}}");
            dynamic tab = lua.a.b;
            double num = tab["c"];
            Assert.AreEqual(num, 2);
        }
        /*
         * Tests getting of numeric field of a table
         * (the field is inside a subtable)
         */
        [TestMethod]
        public void GetTableNumericField2()
        {
            lua("a={b={c=2}}");
            dynamic tab = lua.a;
            double num = tab["b.c"];
            Assert.AreEqual(num, 2);
        }
        /*
         * Tests setting of numeric field of a table
         */
        [TestMethod]
        public void SetTableNumericField1()
        {
            lua("a={b={c=2}}");
            dynamic tab = lua.a.b;
            tab["c"] = 3;
            double num = lua.a.b.c;
            Assert.AreEqual(num, 3);
        }
        /*
         * Tests setting of numeric field of a table
         * (the field is inside a subtable)
         */
        [TestMethod]
        public void SetTableNumericField2()
        {
            lua("a={b={c=2}}");
            dynamic tab = lua.a;
            tab["b.c"] = 3;
            double num = lua.a.b.c;
            Assert.AreEqual(num, 3);
        }
        /*
         * Tests getting of string field of a table
         */
        [TestMethod]
        public void GetTableStringField1()
        {
            lua("a={b={c=\"test\"}}");
            dynamic tab = lua.a.b;
            string str = tab.c;
            Assert.AreEqual(str, "test");
        }
        /*
         * Tests getting of string field of a table
         * (the field is inside a subtable)
         */
        [TestMethod]
        public void GetTableStringField2()
        {
            lua("a={b={c=\"test\"}}");
            dynamic tab = lua.a;
            string str = tab.b.c;
            Assert.AreEqual(str, "test");
        }
        /*
         * Tests setting of string field of a table
         */
        [TestMethod]
        public void SetTableStringField1()
        {
            lua("a={b={c=\"test\"}}");
            dynamic tab = lua.a.b;
            tab["c"] = "new test";
            string str = lua.a.b.c;
            Assert.AreEqual(str, "new test");
        }
        /*
         * Tests setting of string field of a table
         * (the field is inside a subtable)
         */
        [TestMethod]
        public void SetTableStringField2()
        {
            lua("a={b={c=\"test\"}}");
            dynamic tab = lua.a;
            tab["b.c"] = "new test";
            string str = lua.a.b.c;
            Assert.AreEqual(str, "new test");
        }
        /*
         * Tests calling of a global function with zero arguments
         */
        [TestMethod]
        public void CallGlobalFunctionNoArgs()
        {
            lua("a=2\nfunction f()\na=3\nend");
            lua.f();
            double num = lua.a;
            Assert.AreEqual(num, 3);
        }
        /*
         * Tests calling of a global function with one argument
         */
        [TestMethod]
        public void CallGlobalFunctionOneArg()
        {
            lua("a=2\nfunction f(x)\na=a+x\nend");
            lua.f(1);
            double num = lua.a;
            Assert.AreEqual(num, 3);
        }
        /*
         * Tests calling of a global function with two arguments
         */
        [TestMethod]
        public void CallGlobalFunctionTwoArgs()
        {
            lua("a=2\nfunction f(x,y)\na=x+y\nend");
            lua.f(1, 3);
            double num = lua.a;
            Assert.AreEqual(num, 4);
        }
        /*
         * Tests calling of a global function that returns one value
         */
        [TestMethod]
        public void CallGlobalFunctionOneReturn()
        {
            lua("function f(x)\nreturn x+2\nend");
            object[] ret = lua.f(3);
            Assert.AreEqual(1, ret.Length);
            Assert.AreEqual(5.0, ret[0]);
        }
        /*
         * Tests calling of a global function that returns two values
         */
        [TestMethod]
        public void CallGlobalFunctionTwoReturns()
        {
            lua("function f(x,y)\nreturn x,x+y\nend");
            object[] ret = lua.f(3, 2);
            Assert.AreEqual(2, ret.Length);
            Assert.AreEqual(3.0, ret[0]);
            Assert.AreEqual(5.0, ret[1]);
        }
        /*
         * Tests calling of a function inside a table
         */
        [TestMethod]
        public void CallTableFunctionTwoReturns()
        {
            lua("a={}\nfunction a.f(x,y)\nreturn x,x+y\nend");
            object[] ret = lua.a.f(3, 2);
            Assert.AreEqual(2, ret.Length);
            Assert.AreEqual(3.0, ret[0]);
            Assert.AreEqual(5.0, ret[1]);
        }
        /*
         * Tests setting of a global variable to a CLR object value
         */
        [TestMethod]
        public void SetGlobalObject()
        {
            TestClass t1 = new TestClass();
            t1.testval = 4;
            lua["netobj"] = t1;
            object o = lua["netobj"];
            Assert.IsInstanceOfType(o, typeof(TestClass));
            TestClass t2 = o as TestClass;
            Assert.AreEqual(t2.testval, 4);
            Assert.AreSame(t1, t2);
        }
        /*
         * Tests setting of a table field to a CLR object value
         */
        [TestMethod]
        public void SetTableObjectField1()
        {
            lua("a={b={c=\"test\"}}");
            dynamic tab = lua.a.b;
            TestClass t1 = new TestClass();
            t1.testval = 4;
            tab.c = t1;
            TestClass t2 = lua.a.b.c;
            Assert.AreEqual(t2.testval, 4);
            Assert.AreSame(t1, t2);
        }
        /*
         * Tests reading and writing of an object's field
         */
        [TestMethod]
        public void AccessObjectField()
        {
            TestClass t1 = new TestClass();
            t1.val = 4;
            lua["netobj"] = t1;
            lua("var=netobj.val");
            double var = lua["var"];
            Assert.AreEqual(4, var);
            lua("netobj.val=3");
            Assert.AreEqual(3, t1.val);
        }
        /*
         * Tests reading and writing of an object's non-indexed
         * property
         */
        [TestMethod]
        public void AccessObjectProperty()
        {
            TestClass t1 = new TestClass();
            t1.testval = 4;
            lua["netobj"] = t1;
            lua("var=netobj.testval");
            double var = lua["var"];
            Assert.AreEqual(4, var);
            lua("netobj.testval=3");
            Assert.AreEqual(3, t1.testval);
        }
        /*
         * Tests calling of an object's method with no overloads
         */
        [TestMethod]
        public void CallObjectMethod()
        {
            TestClass t1 = new TestClass();
            t1.testval = 4;
            lua["netobj"] = t1;
            lua("netobj:setVal(3)");
            Assert.AreEqual(3, t1.testval);
            lua("val=netobj:getVal()");
            double val = lua.val;
            Assert.AreEqual(3.0, val);
        }
        /*
         * Tests calling of an object's method with overloading
         */
        [TestMethod]
        public void CallObjectMethodByType()
        {
            TestClass t1 = new TestClass();
            lua["netobj"] = t1;
            lua("netobj:setVal('str')");
            Assert.AreEqual("str", t1.getStrVal());
        }
        /*
         * Tests calling of an object's method with no overloading
         * and out parameters
         */
        [TestMethod] //TODO: Fix out Parameters
        public void CallObjectMethodOutParam()
        {
            TestClass t1 = new TestClass();
            lua["netobj"] = t1;
            lua("a,b=netobj:outVal()");
            double a = lua.a;
            double b = lua.b;
            Assert.AreEqual(3, a);
            Assert.AreEqual(5, b);
        }
        /*
         * Tests calling of an object's method with overloading and
         * out params
         */
        [TestMethod]
        public void CallObjectMethodOverloadedOutParam()
        {
            TestClass t1 = new TestClass();
            lua["netobj"] = t1;
            lua("a,b=netobj:outVal(2)");
            double a = lua.a;
            double b = lua.b;
            Assert.AreEqual(2, a);
            Assert.AreEqual(5, b);
        }
        /*
         * Tests calling of an object's method with ref params
         */
        [TestMethod]
        public void CallObjectMethodByRefParam()
        {
            TestClass t1 = new TestClass();
            lua["netobj"] = t1;
            lua("a,b=netobj:outVal(2,3)");
            double a = lua.a;
            double b = lua.b;
            Assert.AreEqual(2, a);
            Assert.AreEqual(5, b);
        }
        /*
         * Tests calling of two versions of an object's method that have
         * the same name and signature but implement different interfaces
         */
        [TestMethod]
        public void CallObjectMethodDistinctInterfaces()
        {
            TestClass t1 = new TestClass();
            lua["netobj"] = t1;
            lua("a=netobj:foo()");
            lua("b=netobj['IFoo1.foo'](netobj)");
            double a = lua.a;
            double b = lua.b;
            Assert.AreEqual(5, a);
            Assert.AreEqual(3, b);
        }
        /*
         * Tests instantiating an object with no-argument constructor
         */
        [TestMethod]
        public void CreateNetObjectNoArgsCons()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type(\"DynamicLuaTests.TestClass\")");
            lua("test=TestClass()");
            lua("test:setVal(3)");
            TestClass test = lua("return test");
            Assert.AreEqual(3, test.testval);
        }
        /*
         * Tests instantiating an object with one-argument constructor
         */
        [TestMethod]
        public void CreateNetObjectOneArgCons()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass(3)");
            TestClass test = lua("return test");
            Assert.AreEqual(3, test.testval);
        }
        /*
         * Tests instantiating an object with overloaded constructor
         */
        [TestMethod]
        public void CreateNetObjectOverloadedCons()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass('str')");
            TestClass test = lua("return test");
            Assert.AreEqual("str", test.getStrVal());
        }
        /*
         * Tests getting item of a CLR array
         */
        [TestMethod]
        public void ReadArrayField()
        {
            string[] arr = new string[] { "str1", "str2", "str3" };
            lua["netobj"] = arr;
            lua("val=netobj[1]");
            string val = lua.val;
            Assert.AreEqual("str2", val);
        }
        /*
         * Tests setting item of a CLR array
         */
        [TestMethod]
        public void WriteArrayField()
        {
            string[] arr = new string[] { "str1", "str2", "str3" };
            lua["netobj"] = arr;
            lua("netobj[1]='test'");
            Assert.AreEqual("test", arr[1]);
        }
        /*
         * Tests creating a new CLR array
         */
        [TestMethod]
        public void CreateArray()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type(\"DynamicLuaTests.TestClass\")");
            lua("arr=TestClass[3]");
            lua("for i=0,2 do arr[i]=TestClass(i+1) end");
            TestClass[] arr = lua.arr;
            Assert.AreEqual(arr[1].testval, 2);
        }
        /*
         * Tests passing a Lua function to a delegate
         * with value-type arguments
         */
        [TestMethod]
        public void LuaDelegateValueTypes()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("function func(x,y) return x+y; end");
            lua("test=TestClass()");
            lua("a=test:callDelegate1(func)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua function to a delegate
         * with value-type arguments and out params
         */
        [TestMethod]
        public void LuaDelegateValueTypesOutParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("function func(x) return x,x*2; end");
            lua("test=TestClass()");
            lua("a=test:callDelegate2(func)");
            double a = lua.a;
            Assert.AreEqual(6, a);
        }
        /*
         * Tests passing a Lua function to a delegate
         * with value-type arguments and ref params
         */
        [TestMethod]
        public void LuaDelegateValueTypesByRefParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("function func(x,y) return x+y; end");
            lua("test=TestClass()");
            lua("a=test:callDelegate3(func)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua function to a delegate
         * with value-type arguments that returns a reference type
         */
        [TestMethod]
        public void LuaDelegateValueTypesReturnReferenceType()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("function func(x,y) return TestClass(x+y); end");
            lua("test=TestClass()");
            lua("a=test:callDelegate4(func)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua function to a delegate
         * with reference type arguments
         */
        [TestMethod]
        public void LuaDelegateReferenceTypes()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("function func(x,y) return x.testval+y.testval; end");
            lua("test=TestClass()");
            lua("a=test:callDelegate5(func)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua function to a delegate
         * with reference type arguments and an out param
         */
        [TestMethod]
        public void LuaDelegateReferenceTypesOutParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("function func(x) return x,TestClass(x*2); end");
            lua("test=TestClass()");
            lua("a=test:callDelegate6(func)");
            double a = lua.a;
            Assert.AreEqual(6, a);
        }
        /*
         * Tests passing a Lua function to a delegate
         * with reference type arguments and a ref param
         */
        [TestMethod]
        public void LuaDelegateReferenceTypesByRefParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("function func(x,y) return TestClass(x+y.testval); end");
            lua("a=test:callDelegate7(func)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * calling one of its methods with value-type params
         */
        [TestMethod]
        public void LuaInterfaceValueTypes()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:test1(x,y) return x+y; end");
            lua("test=TestClass()");
            lua("a=test:callInterface1(itest)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * calling one of its methods with value-type params
         * and an out param
         */
        [TestMethod]
        public void LuaInterfaceValueTypesOutParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:test2(x) return x,x*2; end");
            lua("test=TestClass()");
            lua("a=test:callInterface2(itest)");
            double a = lua.a;
            Assert.AreEqual(6, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * calling one of its methods with value-type params
         * and a ref param
         */
        [TestMethod]
        public void LuaInterfaceValueTypesByRefParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:test3(x,y) return x+y; end");
            lua("test=TestClass()");
            lua("a=test:callInterface3(itest)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * calling one of its methods with value-type params
         * returning a reference type param
         */
        [TestMethod]
        public void LuaInterfaceValueTypesReturnReferenceType()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:test4(x,y) return TestClass(x+y); end");
            lua("test=TestClass()");
            lua("a=test:callInterface4(itest)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * calling one of its methods with reference type params
         */
        [TestMethod]
        public void LuaInterfaceReferenceTypes()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:test5(x,y) return x.testval+y.testval; end");
            lua("test=TestClass()");
            lua("a=test:callInterface5(itest)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * calling one of its methods with reference type params
         * and an out param
         */
        [TestMethod]
        public void LuaInterfaceReferenceTypesOutParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:test6(x) return x,TestClass(x*2); end");
            lua("test=TestClass()");
            lua("a=test:callInterface6(itest)");
            double a = lua.a;
            Assert.AreEqual(6, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * calling one of its methods with reference type params
         * and a ref param
         */
        [TestMethod]
        public void LuaInterfaceReferenceTypesByRefParam()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:test7(x,y) return TestClass(x+y.testval); end");
            lua("a=test:callInterface7(itest)");
            double a = lua.a;
            Assert.AreEqual(5, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * accessing one of its value-type properties
         */
        [TestMethod]
        public void LuaInterfaceValueProperty()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:get_intProp() return itest.int_prop; end");
            lua("function itest:set_intProp(val) itest.int_prop=val; end");
            lua("a=test:callInterface8(itest)");
            double a = lua.a;
            Assert.AreEqual(3, a);
        }
        /*
         * Tests passing a Lua table as an interface and
         * accessing one of its reference type properties
         */
        [TestMethod]
        public void LuaInterfaceReferenceProperty()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("itest={}");
            lua("function itest:get_refProp() return TestClass(itest.int_prop); end");
            lua("function itest:set_refProp(val) itest.int_prop=val.testval; end");
            lua("a=test:callInterface9(itest)");
            double a = lua.a;
            Assert.AreEqual(3, a);
        }


        /*
         * Tests making an object from a Lua table and calling the base
         * class version of one of the methods the table overrides.
         */
        [TestMethod]
        public void LuaTableBaseMethod()
        {
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test={}");
            lua("function test:overridableMethod(x,y) return 2*self.base:overridableMethod(x,y); end");
            lua("luanet.make_object(test,'DynamicLuaTests.TestClass')");
            lua("a=TestClass:callOverridable(test,2,3)");
            double a = lua.a;
            lua("free_object(test)");
            Assert.AreEqual(10.0, a);
        }
        /*
         * Tests getting an object's method by its signature
         * (from object)
         */
        [TestMethod]
        public void GetMethodBySignatureFromObj()
        {
            lua("load_assembly('mscorlib')");
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("setMethod=luanet.get_method_bysig(test,'setVal','System.String')");
            lua("setMethod('test')");
            TestClass test = lua.test;
            Assert.AreEqual("test", test.getStrVal());
        }
        /*
         * Tests getting an object's method by its signature
         * (from type)
         */
        [TestMethod]
        public void GetMethodBySignatureFromType()
        {
            lua("load_assembly('mscorlib')");
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("setMethod=luanet.get_method_bysig(TestClass,'setVal','System.String')");
            lua("setMethod(test,'test')");
            TestClass test = lua.test;
            Assert.AreEqual("test", test.getStrVal());
        }
        /*
         * Tests getting a type's method by its signature
         */
        [TestMethod]
        public void GetStaticMethodBySignature()
        {
            lua("load_assembly('mscorlib')");
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("make_method=luanet.get_method_bysig(TestClass,'makeFromString','System.String')");
            lua("test=make_method('test')");
            TestClass test = lua.test;
            Assert.AreEqual("test", test.getStrVal());
        }
        /*
         * Tests getting an object's constructor by its signature
         */
        [TestMethod]
        public void GetConstructorBySignature()
        {
            lua("load_assembly('mscorlib')");
            lua("load_assembly('DynamicLuaTests')");
            lua("TestClass=import_type('DynamicLuaTests.TestClass')");
            lua("test_cons=luanet.get_constructor_bysig(TestClass,'System.String')");
            lua("test=test_cons('test')");
            TestClass test = lua.test;
            Assert.AreEqual("test", test.getStrVal());
        }

        //---------------------------------------------

        /*
         * Tests capturing an exception
         */
        [TestMethod]
        public void ThrowException()
        {
            lua("luanet.load_assembly('mscorlib')");
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("err,errMsg=pcall(test.exceptionMethod,test)");
            bool err = lua.err;
            Exception ex = lua.errMsg;
            Assert.IsFalse(err);
            Assert.IsNotNull(ex);
            Assert.AreEqual("exception test", ex.InnerException.Message);
        }

        /*
         * Tests capturing an exception
         */
        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes=true)]
        public void ThrowUncaughtException()
        {
            lua("luanet.load_assembly('mscorlib')");
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("test:exceptionMethod()");
        }


        /*
         * Tests nullable fields
         */
        [TestMethod]
        public void TestNullable()
        {
            lua("luanet.load_assembly('mscorlib')");
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");

            lua("val=test.NullableBool");
            Assert.IsNull(lua.val);
            lua("test.NullableBool = true");
            lua("val=test.NullableBool");
            Assert.IsTrue(lua.val);
        }


        /*
         * Tests structure assignment
         */
        [TestMethod]
        public void TestStructs()
        {
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("TestStruct=luanet.import_type('DynamicLuaTests.TestStruct')");

            lua("struct=TestStruct(2)");
            lua("test.Struct = struct");
            lua("val=test.Struct.val");
            Assert.AreEqual(2.0, lua.val);
        }

        //Overloads are tested above...
        /*public void TestMethodOverloads()
        {
            lua("luanet.load_assembly('mscorlib')");
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("test:MethodOverload()");
            lua("test:MethodOverload(test)");
            lua("test:MethodOverload(1,1,1)");
            lua("test:MethodOverload(2,2,i)\r\nprint(i)");
        }*/

        /*
         * Tests for memory leaks
         */
        [TestMethod]
        public void TestDispose()
        {
            System.GC.Collect();
            long startingMem = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;

            for (int i = 0; i < 100; i++)
            {
                using (dynamic lua = new DynamicLua.DynamicLua())
                {
                    _Calc(lua, i);
                }
            }

            System.GC.Collect();
            Assert.AreEqual(startingMem / 1024 / 1024, System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024, 10); //10MB delta 
        }

        private void _Calc(dynamic lua, int i)
        {
            lua("sqrt = math.sqrt;" +
                "sqr = function(x) return math.pow(x,2); end;" +
                "log = math.log;" +
                "log10 = math.log10;" +
                "exp = math.exp;" +
                "sin = math.sin;" +
                "cos = math.cos;" +
                "tan = math.tan;" +
                "abs = math.abs;");

            lua("function calcVP(a,b) return a+b end");

            object[] ret = lua.calcVP(i, 20);
        }

        /*
         * Tests multithreading with lua.
         */
        [TestMethod]
        public void TestThreading()
        {
            Assert.Inconclusive("Error in LuaInterface, problems with locking.");
            
            /*DoWorkClass doWork = new DoWorkClass();
            lua.dowork = new Action(doWork.DoWork);

            bool failureDetected = false;
            int completed = 0;
            int iterations = 500;

            for (int i = 0; i < iterations; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                {
                    try
                    {
                        lua("dowork()");
                    }
                    catch
                    {
                        failureDetected = true;
                    }
                    completed++;
                }));
            }

            while (completed < iterations && !failureDetected)
                Thread.Sleep(50);

            Assert.IsFalse(failureDetected); */
        }

        /*
         * Test if a private method can be called from lua.
         *  If this is possible, the test fails.
         */
        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes=true)] //We want to catch a LuaException
        public void TestPrivateMethod()
        {
            lua("luanet.load_assembly('mscorlib')");
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test=TestClass()");
            lua("test:_PrivateMethod()");
        }

        /*
         * Tests making an object from a Lua table and calling one of
         * methods the table overrides.
         */
        [TestMethod]
        public void LuaTableOverridedMethod()
        {
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test={}");
            lua("function test:overridableMethod(x,y) return x*y; end");
            lua("luanet.make_object(test,'DynamicLuaTests.TestClass')");
            lua("a=TestClass.callOverridable(test,2,3)");
            double a = lua.a;
            lua("luanet.free_object(test)");
            Assert.AreEqual(6.0, a);
        }


        /*
         * Tests making an object from a Lua table and calling a method
         * the table does not override.
         */
        [TestMethod]
        public void LuaTableInheritedMethod()
        {
            lua("luanet.load_assembly('DynamicLuaTests')");
            lua("TestClass=luanet.import_type('DynamicLuaTests.TestClass')");
            lua("test={}");
            lua("function test:overridableMethod(x,y) return x*y; end");
            lua("luanet.make_object(test,'DynamicLuaTests.TestClass')");
            lua("test:setVal(3)");
            lua("a=test.testval");
            double a = lua.a;
            lua("luanet.free_object(test)");
            Assert.AreEqual(3.0, a);
        }


        /// <summary>
        /// Basic multiply method which expects 2 floats
        /// </summary>
        private float _TestException(float val, float val2)
        {
            return val * val2;
        }

        /*
         * Tests lua/CLR Exception handling
         */
        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes=true)] //We want to catch a LuaException
        public void TestEventException()
        {
            //Register a C# function
            lua.Multiply = new Func<float, float, float>(_TestException);

            //create the lua event handler code for the entity
            //includes the bad code!
            lua("function OnClick(sender, eventArgs)\r\n" +
                          "--Multiply expects 2 floats, but instead receives 2 strings\r\n" +
                          "Multiply(asd, we)\r\n" +
                        "end");

            //create the lua event handler code for the entity
            //good code
            //lua.DoString("function OnClick(sender, eventArgs)\r\n" +
            //              "--Multiply expects 2 floats\r\n" +
            //              "Multiply(2, 50)\r\n" +
            //            "end");

            //Create the event handler script
            lua("function SubscribeEntity(e)\r\ne.Clicked:Add(OnClick)\r\nend");

            //Create the entity object
            Entity entity = new Entity();

            //Register the entity object with the event handler inside lua
            lua.SubscribeEntity(entity);

            //Cause the event to be fired
            entity.Click();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes=true)] //We want to catch a LuaException
        public void TestExceptionWithChunkOverload()
        {
            lua("thiswillthrowanerror", "MyChunk");
        }

        [TestMethod]
        public void TestGenerics()
        {
            Assert.Inconclusive("Delegate on generic Method is not possible, needs further thinking."); //TODO: Maybe a test for a delegate on a specific method (foo<string>)?
            //Im not sure support for generic classes is possible to implement, see: http://msdn.microsoft.com/en-us/library/system.reflection.methodinfo.containsgenericparameters.aspx
            //specifically the line that says: "If the ContainsGenericParameters property returns true, the method cannot be invoked"

            //TestClassGeneric<string> genericClass = new TestClassGeneric<string>();

            //_Lua.RegisterFunction("genericMethod", genericClass, typeof(TestClassGeneric<>).GetMethod("GenericMethod"));
            //_Lua.RegisterFunction("regularMethod", genericClass, typeof(TestClassGeneric<>).GetMethod("RegularMethod"));

            //try
            //{
            //    lua("genericMethod('thestring')");
            //}
            //catch { }

            //try
            //{
            //    lua("regularMethod()");
            //}            
            //catch { }

            //if (genericClass.GenericMethodSuccess && genericClass.RegularMethodSuccess && genericClass.Validate("thestring"))
            //    Console.WriteLine("Generic class passed");
            //else
            //    Console.WriteLine("Generic class failed");
        }

        public static int func(int x, int y)
        {
            return x + y;
        }
        public int funcInstance(int x, int y)
        {
            return x + y;
        }

        /*
         * Test the registering of MANY methods in lua
         */
        [TestMethod]
        public void RegisterFunctionStressTest()
        {
            const int Count = 200;  // it seems to work with 41

            MyClass t = new MyClass();

            for (int i = 1; i < Count - 1; ++i)
            {
                lua["func" + i] = new Func<int>(t.Func1);
            }

            for (int i = 1; i < Count - 1; ++i)
            {
                double tmp =lua("return func"+i+"()");
                Assert.AreEqual(1.0, tmp);
            }
        }


        /*
         * Sample test script that shows some of the capabilities of
         * LuaInterface
         */
        public static void Main()
        {
            return;

            /*Console.WriteLine("Starting interpreter...");
            dynamic l = new DynamicLua.DynamicLua();

            Console.WriteLine("Reading test.lua file...");
            l.DoFile("test.lua");
            double width = l.GetNumber("width");
            double height = l.GetNumber("height");
            string message = l.GetString("message");
            double color_r = l.GetNumber("color.r");
            double color_g = l.GetNumber("color.g");
            double color_b = l.GetNumber("color.b");
            Console.WriteLine("Printing values of global variables width, height and message...");
            Console.WriteLine("width: " + width);
            Console.WriteLine("height: " + height);
            Console.WriteLine("message: " + message);
            Console.WriteLine("Printing values of the 'color' table's fields...");
            Console.WriteLine("color.r: " + color_r);
            Console.WriteLine("color.g: " + color_g);
            Console.WriteLine("color.b: " + color_b);
            width = 150;
            Console.WriteLine("Changing width's value and calling Lua function print to show it...");
            l["width"] = width;
            l.GetFunction("print").Call(width);
            message = "LuaNet Interface Test";
            Console.WriteLine("Changing message's value and calling Lua function print to show it...");
            l["message"] = message;
            l.GetFunction("print").Call(message);
            color_r = 30;
            color_g = 10;
            color_b = 200;
            Console.WriteLine("Changing color's fields' values and calling Lua function print to show it...");
            l["color.r"] = color_r;
            l["color.g"] = color_g;
            l["color.b"] = color_b;
            l.DoString("print(color.r,color.g,color.b)");
            Console.WriteLine("Printing values of the tree table's fields...");
            double leaf1 = l.GetNumber("tree.branch1.leaf1");
            string leaf2 = l.GetString("tree.branch1.leaf2");
            string leaf3 = l.GetString("tree.leaf3");
            Console.WriteLine("leaf1: " + leaf1);
            Console.WriteLine("leaf2: " + leaf2);
            Console.WriteLine("leaf3: " + leaf3);
            leaf1 = 30; leaf2 = "new leaf2";
            Console.WriteLine("Changing tree's fields' values and calling Lua function print to show it...");
            l["tree.branch1.leaf1"] = leaf1; l["tree.branch1.leaf2"] = leaf2;
            l.DoString("print(tree.branch1.leaf1,tree.branch1.leaf2)");
            Console.WriteLine("Returning values from Lua with 'return'...");
            object[] vals = l.DoString("return 2,3");
            Console.WriteLine("Returned: " + vals[0] + " and " + vals[1]);
            Console.WriteLine("Calling a Lua function that returns multiple values...");
            object[] vals1 = l.GetFunction("func").Call(2, 3);
            Console.WriteLine("Returned: " + vals1[0] + " and " + vals1[1]);
            Console.WriteLine("Creating a table and filling it from C#...");
            l.NewTable("tab");
            l.NewTable("tab.tab");
            l["tab.a"] = "a!";
            l["tab.b"] = 5.5;
            l["tab.tab.c"] = 6.5;
            l.DoString("print(tab.a,tab.b,tab.tab.c)");
            Console.WriteLine("Setting a table as another table's field...");
            l["tab.a"] = l["tab.tab"];
            l.DoString("print(tab.a.c)");
            Console.WriteLine("Registering a C# static method and calling it from Lua...");

            l.RegisterFunction("func1", null, typeof(TestLuaInterface).GetMethod("func"));
            vals1 = l.GetFunction("func1").Call(2, 3);
            Console.WriteLine("Returned: " + vals1[0]);
            TestLuaInterface obj = new TestLuaInterface();
            Console.WriteLine("Registering a C# instance method and calling it from Lua...");
            l.RegisterFunction("func2", obj, typeof(TestLuaInterface).GetMethod("funcInstance"));
            vals1 = l.GetFunction("func2").Call(2, 3);
            Console.WriteLine("Returned: " + vals1[0]);

            Console.WriteLine("Testing throwing an exception...");
            obj.ThrowUncaughtException();

            Console.WriteLine("Testing catching an exception...");
            obj.ThrowException();

            Console.WriteLine("Testing inheriting a method from Lua...");
            obj.LuaTableInheritedMethod();

            Console.WriteLine("Testing overriding a C# method with Lua...");
            obj.LuaTableOverridedMethod();

            Console.WriteLine("Stress test RegisterFunction (based on a reported bug)..");
            obj.RegisterFunctionStressTest();

            Console.WriteLine("Test structures...");
            obj.TestStructs();

            Console.WriteLine("Test Nullable types...");
            obj.TestNullable();

            Console.WriteLine("Test functions...");
            obj.TestFunctions();

            Console.WriteLine("Test method overloads...");
            obj.TestMethodOverloads();

            Console.WriteLine("Test accessing private method...");
            obj.TestPrivateMethod();

            Console.WriteLine("Test event exceptions...");
            obj.TestEventException();

            Console.WriteLine("Test chunk overload exception...");
            obj.TestExceptionWithChunkOverload();

            Console.WriteLine("Test generics...");
            obj.TestGenerics();

            Console.WriteLine("Test threading...");
            obj.TestThreading();

            Console.WriteLine("Test memory leakage...");
            obj.TestDispose();

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine(); */
        }
    }
}
