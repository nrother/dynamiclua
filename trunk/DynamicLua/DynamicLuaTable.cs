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
using System.Linq.Expressions;
using System.Collections;

namespace DynamicLua
{
    public class DynamicLuaTable : DynamicObject, IEnumerable
    {
        private LuaTable table;
        private Lua state;
        private string path;

        public DynamicLuaTable(LuaTable table, Lua state, string path = null)
            : base()
        {
            this.table = table;
            this.state = state;
            if (path == null)
                path = LuaHelper.GetRandomString(8); //tables need a name, so we can access them
            this.path = path;
        } //TODO: Power() for C#, rawget?, rawset?

        ~DynamicLuaTable()
        {
            table.Dispose();
        }

        public ICollection Keys
        {
            get { return table.Keys; }
        }

        public ICollection Values
        {
            get { return table.Values; }
        }

        public IEnumerator GetEnumerator()
        {
            return table.GetEnumerator();
        }

        public object GetMetatable()
        {
            return LuaHelper.UnWrapObject(state.DoString(String.Format("getmetatable({0})", path), "DynamicLua internal operation")[0], state, path);
        }

        public void SetMetatable(DynamicLuaTable table)
        {
            state.DoString(String.Format("setmetatable({0}, {1})", path, table.path), "DynamicLua internal operation");
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetTableValue(binder.Name);
            return true;
        }

        private object GetTableValue(string name)
        {
            LuaFunction func = GetMetaFunction("index");
            if (func != null) //metatable and metamethod set
                return LuaHelper.UnWrapObject(func.Call(table, name)[0], state, path + "." + name);
            else
                return LuaHelper.UnWrapObject(table[name], state, path + "." + name);

            //use lua to get the object, to use the metatable
            //return LuaHelper.UnWrapObject(state.DoString(String.Format("return {0}[{1}]", path, name), "DynamicLua internal operation")[0], state, path + "." + name);
        }

        //Gets a function from this table's metatable or null if not found. Works even if the metatable is protected. The two underscores are added by this method!!
        private LuaFunction GetMetaFunction(string name)
        {
            if ((bool)state.DoString(String.Format("return debug.getmetatable({0}) == nil", path), "DynamicLua internal operation")[0])
                return null; //Metatable not set
            
            //This is NO performace problem, according to my benchmarks, debug.gmt() is even faster than the normal gmt()! (But you need 1 billion operations to messuare it... on my PC)
            object funcOrNative = state.DoString(String.Format("return debug.getmetatable({0}).__{1}", path, name), "DynamicLua internal operation")[0];

            if (funcOrNative == null) //Either the metatable is not set, or the requested function is not found. Just return null.
                return null;

            if (funcOrNative is LuaFunction)
                return (LuaFunction)funcOrNative;
            else //We need to warp it manually
                return (LuaFunction)typeof(LuaFunction).GetConstructor(new Type[] { funcOrNative.GetType(), typeof(Lua) }).Invoke(new object[] { funcOrNative, state });
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetTableMember(binder.Name, value);
            return true;
        }

        private void SetTableMember(string name, object value)
        {
            object tmp = LuaHelper.WrapObject(value, path + "." + name, state);
            if (tmp != null) //if a function was registered tmp is null, but we dont want to nil the function :P 
            {
                LuaFunction func = GetMetaFunction("newindex");
                if (func != null)
                    func.Call(table, name, value);
                else
                    table[name] = value;
            }
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = GetTableValue(indexes[0].ToString());
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            SetTableMember(indexes[0].ToString(), value);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            //TODO: Umbauen
            //Implemented in lua to use the metatable
            StringBuilder luaCall = new StringBuilder(path + "(");
            string name;
            foreach (object item in args) //Pass all parameters to lua and add the name to the parameter list
            {
                name = LuaHelper.GetRandomString(8);
                state[name] = item;
                luaCall.Append(name);
                luaCall.Append(",");
            }
            luaCall.Remove(luaCall.Length - 2, 1); //Remove last ","
            luaCall.Append(")");
            result = new DynamicArray(state.DoString(luaCall.ToString(), "DynamicLua internal operation"), state);
            return true;
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result) //TODO: Unit Tests
        {
            /* Implementation:
             * TODO: Write it
             */
            string metamethodName;
            bool switchOperands = false; //Lua has only comparison metamethods for less, so for greater the we have to switch the operands
            bool negateResult = false; //Lua has only metamethods for equal, so we need this trick here.
            switch (binder.Operation)
            {
                //Math
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked: //TODO: Thing about __concat
                    metamethodName = "add";
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                    metamethodName = "sub";
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                    metamethodName = "mul";
                    break;
                case ExpressionType.Divide:
                case ExpressionType.DivideAssign:
                    metamethodName = "div";
                    break;
                case ExpressionType.Modulo:
                case ExpressionType.ModuloAssign:
                    metamethodName = "mod";
                    break;
                case ExpressionType.Power:
                case ExpressionType.PowerAssign:
                    metamethodName = "pow";
                    break;
                //Logic Tests
                case ExpressionType.Equal:
                    metamethodName = "eq";
                    break;
                case ExpressionType.NotEqual:
                    metamethodName = "eq";
                    negateResult = true;
                    break;
                case ExpressionType.GreaterThan:
                    metamethodName = "lt";
                    switchOperands = true;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    metamethodName = "le";
                    switchOperands = true;
                    break;
                case ExpressionType.LessThan:
                    metamethodName = "lt";
                    break;
                case ExpressionType.LessThanOrEqual:
                    metamethodName = "le";
                    break;
                default: //This operation is not supported by Lua...
                    result = null;
                    return false;
            }

            LuaFunction mtFunc = GetMetaFunction(metamethodName);

            if (mtFunc == null)
            {
                result = null;
                return false;
            }

            object ret;
            if (!switchOperands)
                ret = mtFunc.Call(table, LuaHelper.WrapObject(arg, LuaHelper.GetRandomString(8), state))[0]; //Metamethods just return one value, or the other will be ignored anyway
            else
                ret = mtFunc.Call(LuaHelper.WrapObject(arg, LuaHelper.GetRandomString(8), state), table)[0];

            if (negateResult && ret is bool) //We can't negate if its not bool. If the metamethod returned someting other than bool and ~= is called there will be a bug. (But who would do this?)
                ret = !(bool)ret;

            result = ret;
            return true;
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            //TODO: Umabauen
            if (binder.Operation == ExpressionType.Negate || binder.Operation == ExpressionType.NegateChecked)
            {
                result = GetMetaFunction("unm").Call(table)[0];
                //result = state.DoString(String.Format("return -{0}", path), "DynmaicLua internal operation")[0];
                return true;
            }
            else
            {
                result = null; //Operation not supported
                return false;
            }
        }

        public override string ToString()
        {
            //TODO: Umbauen
            //Do the operation in lua, to use the metatable
            return state.DoString(String.Format("tostring({0})", path), "DynamicLua internal operation")[0].ToString();
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(LuaTable))
            {
                result = table;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
