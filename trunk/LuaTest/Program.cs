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
            lua("abc = 5");
            lua.abc += 10;
            Console.WriteLine(lua.abc);
            
            
            /*LuaFunctions functions = new LuaFunctions();
            //Register "dump" Function
            lua("local seen={}; function dump(t,i) seen[t]=true; local s={}; local n=0; for k in pairs(t) do n=n+1 s[n]=k; end; table.sort(s); for k,v in ipairs(s) do print(i..v..' ('..type(t[v])..')'); v=t[v]; if type(v)==\"table\" and not seen[v] then dump(v,i..\"\t\");end;end;end;");

            //Metatables...
            dynamic mt = lua.NewTable("mt");
            mt.__index = new Func<object, object, int>((_1, _2) => 42);
            //lua("function mt:__index(...) print(#arg); for i=1,#arg do print(' '..tostring(arg[i])) end return 42 end");
            dynamic tab = lua.NewTable("tab");
            tab.SetMetatable(mt);

            //lua("print(tab.abc)");
            lua("print(mt.__index(tab, 'abc'))");
            Console.WriteLine(lua.tab.abc);

            /*lua("tab = {}");
            lua("function tab:test(...) print(#arg); for i=1,#arg do print(' '..arg[i]) end return 42 end");
            lua.tab.test("abc", 123);*/

            Console.ReadKey(true);
        }
    }

    class LuaFunctions
    {
        public void TestFunction()
        {
            Console.WriteLine("Test Function");
        }
    }
}
