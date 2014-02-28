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
using NLua;

namespace DynamicLua
{
    public class DynamicLuaFunction : DynamicObject
    {
        private LuaFunction function;

        internal DynamicLuaFunction(LuaFunction function)
            : base()
        {
            this.function = function;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = function.Call(args);
            return true;
        }
    }
}
