using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc.Gramma
{
    public class SRprogram:GfcProductionSet
    {
        public override void DefineProductions()
        {
            this.SetKeywords();
            this.DefineProduction("0  =>[program]=>{[programEList]}").Processing += new GfcProductionProcessingHandler(Program_Processing);
            this.DefineProduction("1  =>[programEList]=>[programE][programEList]").Processing += new GfcProductionProcessingHandler(Program_Processing2);
            this.DefineProduction("2  =>[programEList]=>").Processing += new GfcProductionProcessingHandler(program_Processing);
        }

        GfcSemanticRecord program_Processing(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return null;
        }

        GfcSemanticRecord Program_Processing2(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return null;
        }

        GfcSemanticRecord Program_Processing(Parser parser, int i, int j, GfcSemanticRecord[] rightParts)
        {
            return rightParts[1];
        }
    }
    public class programE
    {
        public programEType Type;
        public object Data;
    }
    public enum programEType
    {
        Timport,
        Tclass,
        Tnamespace,
    }
}
