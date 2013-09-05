using System;
using System.Collections.Generic;
using System.Text;

namespace LuaInterface
{
    /// <summary>
    /// Base class to provide consistent disposal flow across lua objects. Uses code provided by Yves Duhoux and suggestions by Hans Schmeidenbacher and Qingrui Li 
    /// </summary>
    public abstract class LuaBase
    {
        private bool _Disposed;
        protected int Reference;
        protected Lua Interpreter;

        ~LuaBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposeManagedResources)
        {
            if (!_Disposed)
            {
                if (disposeManagedResources)
                {
                    if (Reference != 0)
                        Interpreter.dispose(Reference);
                }

                _Disposed = true;
            }
        }

        public override bool Equals(object o)
        {
            if (o is LuaBase)
            {
                LuaBase l = (LuaBase)o;
                return Interpreter.compareRef(l.Reference, Reference);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return Reference;
        }
    }
}
