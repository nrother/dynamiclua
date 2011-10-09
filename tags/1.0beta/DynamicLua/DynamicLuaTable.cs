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
    public class DynamicLuaTable : DynamicObject, IEnumerable, IEnumerable<KeyValuePair<object, object>>
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
                path = LuaHelper.GetRandomString(); //tables need a name, so we can access them
            this.path = path;
        }

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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<KeyValuePair<object, object>>).GetEnumerator();
        }

        IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
        {
            foreach (DictionaryEntry item in table)
                yield return new KeyValuePair<object, object>(item.Key, item.Value);
            yield break;
        }

        public dynamic GetMetatable()
        {
            return LuaHelper.UnWrapObject(state.DoString(String.Format("return getmetatable({0})", path), "DynamicLua internal operation")[0], state);
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
            object tmp = LuaHelper.WrapObject(value, state, path + "." + name);
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
            for (int i = 0; i < args.Length; i++)
                args[i] = LuaHelper.WrapObject(args[i], state);

            LuaFunction func = GetMetaFunction("call");
            if (func != null)
            {
                if (args.Length == 0)
                    result = new DynamicArray(func.Call(table), state);
                else
                    result = new DynamicArray(func.Call(table, args), state);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
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

            if (!switchOperands)
                result = mtFunc.Call(table, LuaHelper.WrapObject(arg, state))[0]; //Metamethods just return one value, or the other will be ignored anyway
            else
                result = mtFunc.Call(LuaHelper.WrapObject(arg, state), table)[0];

            if (negateResult && result is bool) //We can't negate if its not bool. If the metamethod returned someting other than bool and ~= is called there will be a bug. (But who would do this?)
                result = !(bool)result;

            return true;
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (binder.Operation == ExpressionType.Negate || binder.Operation == ExpressionType.NegateChecked)
            {
                result = GetMetaFunction("unm").Call(table)[0];
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
            LuaFunction func = GetMetaFunction("tostring");
            if (func != null)
                return func.Call(table)[0].ToString();
            else
                return table.ToString();
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(LuaTable))
            {
                result = table;
                return true;
            }
            else if (binder.Type == typeof(String))
            {
                result = ToString();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Performs the Power operation on this table. This is only
        /// needed for languages like C# which have no native operators
        /// for this. VB etc. users should use the build-in a^b operator.
        /// </summary>
        public dynamic Power(object operand)
        {
            operand = LuaHelper.WrapObject(operand, state);

            LuaFunction func = GetMetaFunction("pow");
            if (func != null)
                return func.Call(table, operand);
            else
                throw new InvalidOperationException("Metamethod __pow not found");
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return table.Equals(((DynamicLuaTable)obj).table);
        }

        public override int GetHashCode()
        {
            return table.GetHashCode();
        }

        public static bool operator ==(DynamicLuaTable a, DynamicLuaTable b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DynamicLuaTable a, DynamicLuaTable b)
        {
            return !a.Equals(b);
        }
    }
}
