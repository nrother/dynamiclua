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
using System.Dynamic;
using System.Collections;
using LuaInterface;

namespace DynamicLua
{
    public class DynamicArray : DynamicObject, IEnumerable, IEnumerable<object>, IEnumerable<string>
    {
        private object[] array;
        private Lua state;

        public DynamicArray(object[] array, Lua state)
        {
            this.array = array != null ? array : new object[0];
            this.state = state;
        }

        public int Length
        {
            get { return array.Length; }
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            int index;
            result = null;
            if (!int.TryParse(indexes[0].ToString(), out index))
                return false;
            if (index >= array.Length)
                return false;

            result = LuaHelper.UnWrapObject(array[index], state);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            int index;
            result = null;
            if (!int.TryParse(binder.Name, out index))
                return false;
            if (index >= array.Length)
                return false;

            result = LuaHelper.UnWrapObject(array[index], state);
            return true;
        }

        /* Not supported, makes no sense
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            int index;
            if (!int.TryParse(indexes[0].ToString(), out index))
                return false;
            if (index >= array.Length)
                return false;

            array[index] = value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            int index;
            if (!int.TryParse(binder.Name, out index))
                return false;
            if (index >= array.Length)
                return false;

            array[index] = value;
            return true;
        }*/

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return i.ToString();
            }
            yield break;
        }

        public override bool TryConvert(ConvertBinder binder, out object result) //TODO: Lua Array?
        {
            result = null;

            if (binder.Type == typeof(object[]))
            {
                result = array;
                return true;
            }

            if (array.Length != 1)
                return false;

            if (array[0].GetType() == typeof(LuaTable) || array[0].GetType() == typeof(LuaFunction))
            {
                result = LuaHelper.UnWrapObject(array[0], state); //TODO: Wrong???
                return true;
            }

            if (array[0].GetType() != binder.Type)
                return false;

            result = Convert.ChangeType(array[0], binder.Type);
            return true;
        }

        public override string ToString()
        {
            if (array.Length == 1)
                return array[0].ToString();
            else
                return array.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return array[i];
            }
            yield break;
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return array[i];
            }
            yield break;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return array[i].ToString();
            }
            yield break;
        }
    }
}
