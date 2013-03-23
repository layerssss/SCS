using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public abstract class GfcGrammaBasic
    {
        public LL1Gfc GetData()
        {
            List<GfcProductionSet> grammas = new List<GfcProductionSet>();
            string acceptLeft = this.Include(grammas);
            List<string> keywords = new List<string>();
            List<GfcProductionDescription> productions = new List<GfcProductionDescription>();
            foreach (GfcProductionSet gramma in grammas)
            {
                gramma.DefineProductions();
                if (gramma.Keywords != null)
                {
                    foreach (string keyword in gramma.Keywords)
                    {
                        if (!keywords.Contains(keyword))
                        {
                            keywords.Add(keyword);
                        }
                    }
                }
                foreach (GfcProductionDescription production in gramma.Descriptions)
                {
                    productions.Add(production);
                }
            }
            LL1Gfc gfc = new LL1Gfc(productions.ToArray(), keywords.ToArray(), acceptLeft);
            return gfc;
        }
        public abstract string Include(List<GfcProductionSet> grammas);
    }
}
