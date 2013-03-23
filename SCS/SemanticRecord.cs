using System;
using System.Collections.Generic;
using System.Text;

namespace SCS
{
    public class SemanticRecord
    {
        public Token Token;
        public SemanticRecord(Token token)
        {
            this.Token = token;
        }
    }
}
