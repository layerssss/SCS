using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public class GfcSymbol:IComparable<GfcSymbol>
    {
        public int CompareTo(GfcSymbol symbol)
        {
            return this.Id.GetHashCode().CompareTo(symbol.Id.GetHashCode());
        }
        public bool IsTerminal;
        public Guid Id = Guid.NewGuid();
        public Token TerminalToken;
        public string SymbolName = "";
        public GfcSymbol(string symbolName)
        {
            this.IsTerminal = false;
            this.SymbolName = symbolName;
        }
        public List<GfcProduction> Productions=new List<GfcProduction>();
        public GfcSymbol(Token token)
        {
            this.IsTerminal = true;
            this.TerminalToken = token;
        }
        
    }
}
