using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc.Gramma
{
    public class Gramma:GfcGrammaBasic
    {
        
        public override string Include(List<GfcProductionSet> grammas)
        {
            grammas.Add(new SRprogram());
            grammas.Add(new SRIdentList());
            grammas.Add(new Program.SRimport());
            return "[program]";
        }
    }
}
