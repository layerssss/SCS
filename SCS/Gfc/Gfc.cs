using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public class Gfc
    {
        public GfcSymbol AcceptSymbol;
        public string[] Keywords;
        public List<GfcProduction> Productions = new List<GfcProduction>();
        public Gfc(GfcProductionDescription[] productions,string[] keywordsList,string acceptLeft)
        {
            this.Keywords = keywordsList;
            Scanner gfcScanner = new Scanner(keywordsList);
            Dictionary<string, GfcSymbol> lefts = new Dictionary<string, GfcSymbol>();
            foreach (GfcProductionDescription production in productions)
            {
                string[] parts = production.Production.Split(new string[] { "=>" }, StringSplitOptions.None);
                if (parts.Length != 3)
                {
                    continue;
                }
                if (!lefts.ContainsKey(parts[1]))
                {
                    lefts.Add(parts[1], new GfcSymbol(parts[1]));
                }
            }

            foreach (GfcProductionDescription production in productions)
            {
                string[] parts = production.Production.Split(new string[] { "=>" }, StringSplitOptions.None);
                if (parts.Length != 3)
                {
                    continue;
                }
                GfcProduction newProduction = new GfcProduction(parts[0]);
                newProduction.Handler = production.Handler;
                lefts[parts[1]].Productions.Add(newProduction);
                this.Productions.Add(newProduction);
                gfcScanner.InitContext(parts[2]);
                Token token;
                try
                {
                    while (true)
                    {
                        token = gfcScanner.Scan();
                        if (token.Type == TokenType.NonTerminal)
                        {
                            newProduction.Right.Add(lefts[token.Data]);
                            continue;
                        }
                        newProduction.Right.Add(new GfcSymbol(token));
                    }
                }
                catch (ScannerEndOfFileException) { }
            }
            this.AcceptSymbol = lefts[acceptLeft];
        }
    }
}
