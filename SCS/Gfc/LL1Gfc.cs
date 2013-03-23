using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public class LL1Gfc : Gfc
    {
        public LL1Gfc(string[] productions, string[] keywordsList, string acceptLeft)
            :this(Array.ConvertAll<string,GfcProductionDescription>(productions,ts=>new GfcProductionDescription(){Production=ts}),keywordsList,acceptLeft)
        {
        }
        public LL1Gfc(GfcProductionDescription[] productions, string[] keywordsList, string acceptLeft)
            : base(productions,keywordsList,acceptLeft)
        {
            foreach (GfcProduction production in this.Productions)
            {
                this.First.Add(
                    production,
                    new LL1GfcFirstCollection());
            }
            while (this.calFirst()) ;
        }
        public Dictionary<GfcProduction, LL1GfcFirstCollection> First=new Dictionary<GfcProduction,LL1GfcFirstCollection>();
        bool calFirst()//Calculate all productions' First
        {
            foreach (GfcProduction production in this.Productions)
            {
                bool allowNull = true;
                foreach (GfcSymbol part in production.Right)
                {
                    if (part.IsTerminal)
                    {
                        if (this.First[production].SafeAdd(part.TerminalToken))
                        {
                            return true;
                        }
                        allowNull = false;
                    }
                    else
                    {
                        allowNull = false;//Assume there's no epsilon first. Then search whether is there any.
                        foreach (GfcProduction partProd in part.Productions)//Add part(nonterminal)'s First in to production's First
                        {
                            if (this.First[production].SafeAddRange(
                                this.First[partProd]))
                            {
                                return true;
                            }
                            if (this.First[partProd].ContainEpsilon)
                            {
                                allowNull = true;
                            }
                        }
                    }
                    if (!allowNull)//Got an epsilon part? Stop
                    {
                        break;
                    }
                }
                if (allowNull!=this.First[production].ContainEpsilon)
                {
                    this.First[production].ContainEpsilon = allowNull;
                    return true;
                }
            }
            return false;
        }
    }
    public class LL1GfcFirstCollection : List<Token>
    {
        public bool ContainEpsilon;
        public LL1GfcFirstCollection()
            : base()
        {
            this.ContainEpsilon = false;
        }
        public bool SafeAdd(Token token)
        {
            if (!base.Contains(token))
            {
                base.Add(token);
                return true;
            }
            return false;
        }
        public bool SafeAddRange(ICollection<Token> tokens)
        {
            bool changed = false;
            foreach (Token token in tokens)
            {
                if (this.SafeAdd(token))
                {
                    changed = true;
                }
            }
            return changed;
        }
    }
}
