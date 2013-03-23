using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public abstract class GfcProductionSet
    {
        public string[] Keywords;
        public abstract void DefineProductions();
        public void SetKeywords(params string[] keywords)
        {
            this.Keywords = keywords;
        }
        public List<string> Productions = new List<string>();
        public List<GfcProductionDescription> Descriptions = new List<GfcProductionDescription>();
        public GfcProductionDescription DefineProduction(string production)
        {
            this.Productions.Add(production);
            GfcProductionDescription description = new GfcProductionDescription();
            description.Production = production;
            this.Descriptions.Add(description);
            return description;

        }
    }
}
