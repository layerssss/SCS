using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc.Gramma
{
    public class SRUndefined:GfcProductionSet
    {
        public override void DefineProductions()
        {
            this.DefineProduction("20 =>[undefined]=>undefined").Processing += new GfcProductionProcessingHandler(SRUndefined_Processing);
            this.SetKeywords("undefined");
        }
        GfcSemanticRecord SRUndefined_Processing(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return new GfcSemanticRecord();
        }
    }
}
