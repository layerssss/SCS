using System;
using System.Collections.Generic;
using System.Text;

namespace SCS
{
    abstract public class Context
    {
        public string Content;
        public int ProcessOffset;
        public void InitContext(string context)
        {
            this.Content = context;
            this.ProcessOffset = 0;
        }
        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <param name="posStart">The pos start.</param>
        /// <returns></returns>
        public int GetLineNumber(int posStart)
        {
            return GetLineNumber(this, posStart);
        }
        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <returns></returns>
        public int GetLineNumber()
        {
            return this.GetLineNumber(this.ProcessOffset);
        }
        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="posStart">The pos start.</param>
        /// <returns></returns>
        public static int GetLineNumber(Context context, int posStart)
        {
            string[] lines = context.Content.Remove(posStart).Split('\r');
            return lines.Length + 1;
        }
    }
}
