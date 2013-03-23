using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public class GfcSemanticRecord
    {
        public int PosEnd;
        public int PosStart;
        public Context Context;
        public string GetText()
        {
            return this.Context.Content.Substring(PosStart, PosEnd);
        }
        public object InterminalObject;
        public Token Token;
    }
}
