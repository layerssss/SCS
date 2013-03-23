using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc.Gramma
{
    public class SRIdentList:GfcProductionSet
    {
        public override void DefineProductions()
        {
            this.DefineProduction("9  =>[IdentList]=>ident[IdentListTail]").Processing += new GfcProductionProcessingHandler(IdentList_Processing);
            this.DefineProduction("10 =>[IdentListTail]=>.ident[IdentListTail]").Processing += new GfcProductionProcessingHandler(IdentList_Processing2);
            this.DefineProduction("11 =>[IdentListTail]=>").Processing += new GfcProductionProcessingHandler(IdentList_Processing2);
        }

        GfcSemanticRecord IdentList_Processing(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return null;
        }
        GfcSemanticRecord IdentList_Processing1(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return null;
        }
        GfcSemanticRecord IdentList_Processing2(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return null;
        }
    }
    public class SRIdent
    {
        public string Name;
        public SRIdent(string s)
        {
            this.Name = s;
        }
    }
}
