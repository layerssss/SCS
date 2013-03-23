using System;
using System.Collections.Generic;
using System.Text;

namespace SCS
{
    public class ExtandedStack<T>:Stack<T>
    {
        public T Top
        {
            set
            {
                this.Pop();
                this.Push(value);
            }
            get
            {
                T t = this.Pop();
                this.Push(t);
                return t;
            }
        }

    }
}
