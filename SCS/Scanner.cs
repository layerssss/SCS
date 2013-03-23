using System;
using System.Collections.Generic;
using System.Text;

namespace SCS
{
    public class Scanner:Context
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scanner"/> class.
        /// </summary>
        public Scanner(params string[] keywordsList)
        {
            scannerBuilder builder = new scannerBuilder();
            scannerBuilderTargetData data = builder.Build();
            this.startStatus = data.StartStatus;
            this.charHasher = data.CharHasher;
            this.edfaTable = data.TargetEdfa;
            this.keywordsList = keywordsList;
            if (this.startStatus.IsResult)
            {
                throw (new Exception("Error syntax definition!"));
            }
            this.scannerBuffer = new List<char>();
        }
        string[] keywordsList;
        ICharHasher charHasher;
        /// <summary>
        /// The Extended-DFA transition table
        /// </summary>
        Dictionary<scannerStatus, Dictionary<char,scannerStatus>> edfaTable;
        scannerStatus startStatus;
        List<char> scannerBuffer;
        public Token Scan()
        {
            this.scannerBuffer.Clear();
            scannerStatus status = this.startStatus;
            scannerStatus lastResult = null;
            int lastResultLength=0;
            while (true)
            {
                if (this.Content.Length==ProcessOffset)//EndofContext
                {
                    break;
                }
                else
                {
                    char c = this.Content[this.ProcessOffset];
                    if (c == ' ' || c == '\n' || c == '\r' || c == '\t')
                    {
                        if (this.scannerBuffer.Count == 0)//Skip white spaces
                        {
                            this.ProcessOffset++;
                            continue;
                        }
                        else
                        {
                            break;//Stop
                        }
                    }
                    char c2 = this.charHasher.Hash(c);
                    if (this.edfaTable[status].ContainsKey(c2))//CharFound
                    {
                        status = this.edfaTable[status][c2];
                        scannerBuffer.Add(c);
                    }
                    else
                    {
                        if (this.edfaTable[status].ContainsKey(this.charHasher.UndefinedChar))//AnyChar
                        {
                            status = this.edfaTable[status][this.charHasher.UndefinedChar];
                            scannerBuffer.Add(c);
                        }
                        else//NotFound
                        {
                            break;
                        }
                    }
                }
                this.ProcessOffset++;
                if (status.IsResult)
                {
                    lastResult = status;
                    lastResultLength = this.scannerBuffer.Count;
                }
            }
            this.ProcessOffset -= this.scannerBuffer.Count - lastResultLength;//Give back the string portion unrecognized
            if (lastResult == null)
            {
                if (this.ProcessOffset == this.Content.Length)
                {
                    throw (new ScannerEndOfFileException(this));
                }
                throw (new ScannerCannotRecognizeException(this.ProcessOffset, this));
            }
            string s = "";
            for (int i = 0; i < lastResultLength; i++)
            {
                s += this.scannerBuffer[i];
            }
            if (lastResult.ResultTokenType == TokenType.Identifier)
            {
                foreach (string keywords in this.keywordsList)
                {
                    if (keywords == s)
                    {
                        return new Token(TokenType.Keyword, s);
                    }
                }
            }
            if (lastResult.ResultTokenType == TokenType.BlockComment)
            {
                return this.Scan();
            }
            return new Token(lastResult.ResultTokenType, s);
        }
    }
    class scannerStatus : IEquatable<scannerStatus>
    {
        /// <summary>
        /// StatusIdentity for equal checking.
        /// </summary>
        public Guid Id;
        /// <summary>
        /// Should the status return a scan result.
        /// </summary>
        public bool IsResult;
        public TokenType ResultTokenType;
        public bool Equals(scannerStatus s)
        {
            return this.Id == s.Id;
        }
        public scannerStatus()
        {
            Id = Guid.NewGuid();
        }
    }
    public class ScannerException : System.Exception
    {
        public ScannerException(int i,Scanner sender)
        {
            this.Offset = i;
            this.scanner = sender;
            
        }
        public virtual new string Message
        {
            get
            {
                return this.ToString();
            }
        }
        public int Offset;
        Scanner scanner;
        public override string ToString()
        {
            return scanner.Content.Substring(Offset);
        }
    }
    public class ScannerCannotRecognizeException : ScannerException
    {
        public ScannerCannotRecognizeException(int i, Scanner sender)
            : base(i, sender)
        {
        }
    }
    public class ScannerEndOfFileException : ScannerException
    {
        public ScannerEndOfFileException(Scanner sender) :
            base(sender.Content.Length, sender)
        {
        }
        public override string Message
        {
            get
            {
                return "End Of File";
            }
        }
    }
    public enum TokenType
    {
        LiteralInteger,
        LiteralReal,
        LiteralString,
        LiteralClearString,
        LiteralChar,
        Identifier,
        Keyword,
        PunctuatorDot,
        PunctuatorComma,
        PunctuatorSplitter,
        PunctuatorBelow,
        PunctuatorAbove,
        PunctuatorPlus,
        PunctuatorMinus,
        PunctuatorEqual,
        PunctuatorMultiply,
        PunctuatorDivision,
        PunctuatorLBracket,
        PunctuatorRBracket,
        PunctuatorLBBracket,
        PunctuatorRBBracket,
        PunctuatorLMBracket,
        PunctuatorRMBracket,
        BlockComment,//Ignore
        NonTerminal//ReservedForGfcAutoInitial
    }
    public class Token : 
        IEquatable<Token>
    {
        public override int GetHashCode()
        {
            if (this.Type == TokenType.Keyword)
            {
                return this.Data.GetHashCode();
            }
            return this.Type.GetHashCode();
        }
        public TokenType Type;
        public string Data;
        public bool Equals(Token o)
        {
            if (this.Type != o.Type)
            {
                return false;
            }
            switch (this.Type)
            {
                case TokenType.Keyword:
                    return this.Data == o.Data;
                default:
                    return true;
            }
        }
        public Token(TokenType type, string data)
        {
            this.Type = type;
            this.Data = data;
        }
        public override string ToString()
        {
            if (this.Type == TokenType.Keyword)
            {
                return this.Data;
            }
            else
            {
                return this.Type.ToString();
            }
        }
    }
    interface ICharHasher
    {
        char Hash(char c);
        char UndefinedChar{get;}
    }
}
