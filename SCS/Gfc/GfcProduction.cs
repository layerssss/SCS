using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public class GfcProduction
    {
        public List<GfcSymbol> Right = new List<GfcSymbol>();
        public GfcProduction(string productionName)
        {
            this.Name = productionName;
        }
        public string Name;
        public GfcProductionProcessingHandler Handler;
    }
    public delegate object GfcProductionProcessingHandler(Parser parser,int posStart,int posEnd,params GfcSemanticRecord[] rightParts);
}
