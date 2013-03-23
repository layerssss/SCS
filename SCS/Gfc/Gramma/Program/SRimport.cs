using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc.Gramma.Program
{
    public class SRimport:GfcProductionSet
    {
        public override void DefineProductions()
        {
            this.SetKeywords("using");

            this.DefineProduction("4  =>[programE]=>[import]").Processing += new GfcProductionProcessingHandler(Timport_Processing);
            this.DefineProduction("8  =>[import]=>using [IdentList];").Processing += new GfcProductionProcessingHandler(Timport_Processing2);
        }

        GfcSemanticRecord Timport_Processing(Parser parser,int i,int j,GfcSemanticRecord[] rightParts)
        {
            return rightParts[0];
        }
        GfcSemanticRecord Timport_Processing2(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return rightParts[0];
        }
    }
    public class SRimportE
    {
        public List<SRIdent> Dest;
    }
}
