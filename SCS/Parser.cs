using System;
using System.Collections.Generic;
using System.Text;
using SCS.Gfc;
namespace SCS
{
    public class Parser:Scanner
    {
        public Parser(LL1Gfc g)
            : base
            (g.Keywords)
        {
            this.gfc = g;
        }
        LL1Gfc gfc;
        ExtandedStack<GfcSymbol> parserStack = new ExtandedStack<GfcSymbol>();
        public ExtandedStack<GfcSemanticRecord> SemanticRecordsStack = new ExtandedStack<GfcSemanticRecord>();
        ExtandedStack<GfcProduction> parsingProductionsStack = new ExtandedStack<GfcProduction>();
        ExtandedStack<int> parsingProductionsCounterStack = new ExtandedStack<int>();
        ExtandedStack<int> lastProcessOffsetStack = new ExtandedStack<int>();
        public void Parse()
        {
            
            this.parserStack.Push(gfc.AcceptSymbol);
            #region InitialSR
            this.parsingProductionsCounterStack.Push(0);
            this.lastProcessOffsetStack.Push(ProcessOffset);
            GfcProduction endProduction = new GfcProduction("Parsed.");
            endProduction.Handler = ((p,i,j,o) => o[0].InterminalObject);
            endProduction.Right.Add(gfc.AcceptSymbol);
            this.parsingProductionsStack.Push(endProduction); 
            #endregion
            Token token = null;
            while (true)
            {
                #region Determine whether ended
                if (this.parserStack.Count == 0)
                {
                    if (this.parsingProductionsCounterStack.Count == 0)//There's still productions needs to be handle.
                    {
                        break;
                    }
                } 
                #endregion
                ScannerEndOfFileException eofException = null;
                #region Get the next token or an EOFException(dont let it be thrown,save it)
                if (token == null)
                {
                    try
                    {
                        token = this.Scan();
                    }
                    catch (ScannerEndOfFileException ex)
                    {
                        if (this.ProcessOffset != this.Content.Length)
                        {
                            throw (ex);
                        }
                        eofException = ex;//NO MORE TOKENS!!!
                    }
                } 
                #endregion
                #region ReduceSR
                if (this.parsingProductionsCounterStack.Top == this.parsingProductionsStack.Top.Right.Count)
                {

                    int srCount = this.parsingProductionsCounterStack.Pop();
                    GfcSemanticRecord[] srList = new GfcSemanticRecord[srCount];
                    for (srCount--; srCount >= 0; srCount--)
                    {
                        srList[srCount] = this.SemanticRecordsStack.Pop();
                    }
                    GfcProduction prod = this.parsingProductionsStack.Pop();
                    int lastPos = this.lastProcessOffsetStack.Pop();
                    this.SemanticRecordsStack.Push(
                        new GfcSemanticRecord() { 
                            InterminalObject = prod.Handler(this,lastPos,this.ProcessOffset, srList),
                            PosStart=lastPos,
                            PosEnd=this.ProcessOffset });
                    continue;
                }
                #endregion
                GfcSymbol curSymbol = this.parserStack.Pop();
                bool expanded = false;
                if (eofException == null)//Only when we have token can we expand or processTerminal
                {
                    #region processTerminal
                    if (curSymbol.IsTerminal)
                    {
                        if (token.Equals(curSymbol.TerminalToken))
                        {
                            #region PushSR
                            this.SemanticRecordsStack.Push(new GfcSemanticRecord() { Token = token });
                            this.parsingProductionsCounterStack.Top++;
                            #endregion
                            token = null;
                            continue;
                        }
                        throw (new ParserException(this.ProcessOffset, token.ToString() + "(" + token.Data + ")", curSymbol.TerminalToken.ToString()));
                    }
                    #endregion
                    #region try to find it in First ,then expand
                    foreach (GfcProduction production in curSymbol.Productions)
                    {
                        if (this.gfc.First[production].Contains(token))
                        {
                            #region expand
                            for (int i = production.Right.Count - 1; i >= 0; i--)
                            {
                                this.parserStack.Push(production.Right[i]);
                            } 
                            #endregion
                            expanded = true;
                            #region ShiftSR
                            this.lastProcessOffsetStack.Push(this.ProcessOffset);
                            this.parsingProductionsStack.Push(production);
                            this.parsingProductionsCounterStack.Top++;
                            this.parsingProductionsCounterStack.Push(0);
                            #endregion
                            break;
                        }
                    }
                    #endregion
                }
                #region CannotExpand-FindEpsilon
                if (!expanded)
                {
                    foreach (GfcProduction production in curSymbol.Productions)
                    {
                        if (this.gfc.First[production].ContainEpsilon)
                        {
                            expanded = true;
                            #region ShiftSR(Epsilon)
                            this.SemanticRecordsStack.Push(new GfcSemanticRecord()
                            {
                                InterminalObject = production.Handler(this,this.ProcessOffset, this.ProcessOffset),
                                PosStart = this.ProcessOffset,
                                PosEnd = this.ProcessOffset
                            });
                            this.parsingProductionsCounterStack.Top++;
                            #endregion
                            break;
                        }
                    }
                } 
                #endregion
                #region Cannot expand? parser error...
                if (!expanded)
                {
                    if (eofException != null)
                    {
                        #region No more tokens nor epsilon
                        throw (eofException); 
                        #endregion
                    }
                    List<string> exp = new List<string>();
                    foreach (GfcProduction p in curSymbol.Productions)
                    {
                        foreach (Token t in gfc.First[p])
                        {
                            exp.Add(t.ToString());
                        }
                    }
                    throw (new ParserException(ProcessOffset, "", exp.ToArray()));//sytax error. expected items can be found in curSymbol.Productions.
                } 
                #endregion
            }
            #region Excess content
            if (token!=null)
            {
                throw (new ParserException(this.ProcessOffset, token.ToString()+"("+token.Data+")", "EndOfFile"));
            } 
            #endregion
        }
    }
    public class ParserException : System.Exception
    {
        public List<string> Expecting = new List<string>();
        public string Cur;
        public int Offset;
        public ParserException(int offset,string cur, params string[] expecting)
        {
            this.Offset = offset;
            this.Cur = cur;
            foreach (string s in expecting)
            {
                this.Expecting.Add(s);
            }
        }
    }
}
