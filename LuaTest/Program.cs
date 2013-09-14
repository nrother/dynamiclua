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
using System.Collections.Generic;
using System.Text;
using DynamicLua;

namespace LuaTest
{
    class Program
    {
        //Main Entry Point for the Solution (Startup Project)
        //Just some random tests here. The real Unittests are in a seperate Project. See Readme for Details.
        static void Main(string[] args)
        {
            dynamic lua = new DynamicLua.DynamicLua();

            lua("c1 = { num = 42 }; c2 = { num = 7 }");
            lua("mtc = { __add = function(t, other) return t.num + other.num end }");
            lua("setmetatable(c1, mtc)");
            lua("setmetatable(c2, mtc)");

            Console.WriteLine(lua.c1 + lua.c2);

            lua.DoFile("test.lua");
            Console.WriteLine(lua.tab.test);

            dynamic tab = lua("return {a = 1, b = 2, c = 3}");
            lua.tab = tab;
            lua("print(type(tab))");
            lua("for k,v in pairs(tab) do print(k,v) end");

            Console.ReadKey(true);
        }
    }
}
