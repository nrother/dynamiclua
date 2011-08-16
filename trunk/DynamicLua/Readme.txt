lua.GetFunction("f").Call(args); => lua.f(args);
lua["abc"] => lua.abc;
lua.GetTable("a").GetTable("b").GetNumber("c"); => lua.a.b.c;
TestClass t = (TestClass)lua["t"]; => TestClass t = lua.t;
lua["func"]();
DynamicArray
LUA 1.5.4
Some Tests fail --> Error in Lua due to .NET 4


TODO:
//TODO:'s
Convert in base type..?
Enumarable Tables and Lua
DynamicLuaBase?
void Foo(params object[])
DynamicLuaTable as Dictinary?