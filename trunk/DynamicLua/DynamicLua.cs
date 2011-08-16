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
using LuaInterface;
using System.Dynamic;
using System.Reflection;
using System.IO;

namespace DynamicLua
{
    public class DynamicLua : DynamicObject, IDisposable
    {
        private Lua lua;

        public Lua LuaInstance
        {
            get { return lua; }
        }

        public DynamicLua()
            : base()
        {
            lua = new Lua();
        }

        ~DynamicLua()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (lua != null)
            {
                lua.Close(); //no dispose(), this gives a access-violation...
                lua = null;
            }
        }

        public object[] DoFile(string path)
        {
            return lua.DoFile(Path.Combine(Environment.CurrentDirectory, path));
        }

        public DynamicLuaFunction LoadFile(string path)
        {
            return new DynamicLuaFunction(lua.LoadFile(Path.Combine(Environment.CurrentDirectory, path)));
        }

        public DynamicLuaFunction LoadString(string chunk, string name = "")
        {
            return new DynamicLuaFunction(lua.LoadString(chunk, name));
        }

        public DynamicLuaTable NewTable(string name)
        {
            lua.NewTable(name);
            return new DynamicLuaTable(lua[name] as LuaTable, lua, name);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetLuaValue(binder.Name);
            return true;
        }

        private object GetLuaValue(string name)
        {
            return LuaHelper.UnWrapObject(lua[name], lua, name);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetLuaMember(binder.Name, value);
            return true;
        }

        private void SetLuaMember(string name, object value)
        {
            object tmp = LuaHelper.WrapObject(value, name, lua);
            if (tmp != null) //if a function was registered tmp is null, but we dont want to nil the function :P
                lua[name] = tmp;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = GetLuaValue(indexes[0].ToString());
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            SetLuaMember(indexes[0].ToString(), value);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            LuaFunction func = lua.GetFunction(binder.Name);

            if (func != null)
            {
                result = new DynamicArray(func.Call(args), lua);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = new DynamicArray(lua.DoString(args[0].ToString()), lua);
            return true;
        }
    }
}
