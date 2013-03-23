using System;
using System.Collections.Generic;
using System.Text;
using NFA;
using NFA.Process;
namespace SCS
{
    /// <summary>
    /// Covert regular expressions to EDFA
    /// Reg->NFA   NFA->(E)DFA
    /// </summary>
    class scannerBuilder
    {
        Dictionary<scannerStatus, Dictionary<char, scannerStatus>> targetEDFA;
        NfaStatus<char, faStatusData> buildNfa(regularPart r, NfaStatus<char, faStatusData> startStatus)
        {
            NfaStatus<char, faStatusData> endStatus;
            NfaStatus<char, faStatusData> newStartStatus = startStatus.Nfa.NewStatus(new faStatusData());
            startStatus.AddTransition(new NfaStatusTransition<char, faStatusData>(newStartStatus));
            switch (r.Type)
            {
                case regularPartType.Concatenation:
                    endStatus = newStartStatus;
                    foreach (regularPart sr in r.SubParts)
                    {
                        endStatus = buildNfa(sr, endStatus);
                    }
                    break;
                case regularPartType.MultiChoice:
                    endStatus = newStartStatus.Nfa.NewStatus(new faStatusData());
                    foreach (regularPart sr in r.SubParts)
                    {
                        buildNfa(sr, newStartStatus).AddTransition(new NfaStatusTransition<char, faStatusData>(endStatus));
                    }
                    break;
                case regularPartType.SpecifyChar:
                    endStatus = startStatus.Nfa.NewStatus(new faStatusData());
                    newStartStatus.AddTransition(new NfaStatusTransition<char, faStatusData>(endStatus, r.Char));
                    break;
                default:
                    throw (new Exception("regularType undifined!"));
            }
            if (r.Optional)
            {
                newStartStatus.AddTransition(new NfaStatusTransition<char, faStatusData>(endStatus));
            }
            if (r.Repeat)
            {

                endStatus.AddTransition(new NfaStatusTransition<char, faStatusData>(newStartStatus));
            }
            return endStatus;
        }
        Nfa<char, faStatusData> nfa;
        /// <summary>
        /// Build an EDFA table from regular exps.
        /// </summary>
        /// <param name="target">The target.</param>
        public scannerBuilderTargetData Build()
        {
            scannerBuilderData data = new scannerBuilderData();
            nfa = new Nfa<char, faStatusData>();
            nfa.StartStatus = nfa.NewStatus(new faStatusData());
            #region ExpResolution
            foreach (TokenType type in data.Exps.Keys)
            {
                NfaStatus<char, faStatusData> newStatus = nfa.NewStatus(new faStatusData(type));
                buildNfa(data.Exps[type], nfa.StartStatus).AddTransition(new NfaStatusTransition<char, faStatusData>(newStatus));
            }
            #endregion
            this.targetEDFA =new Dictionary<scannerStatus,Dictionary<char,scannerStatus>>();
            Determinstic<char, faStatusData>.MakeDeterminstic(nfa, faStatusMergeChecker);
            this.statusMap = new Dictionary<NfaStatus<char, faStatusData>, scannerStatus>();
            this.nfa.Traverse(this.nfaMappingTraverser);
            this.nfa.Traverse(this.nfaTransitionTraverser);
            return new scannerBuilderTargetData()
            {
                StartStatus = this.statusMap[this.nfa.StartStatus],
                TargetEdfa=this.targetEDFA,
                CharHasher=data.CharHasher
            };
        }
        Dictionary<NfaStatus<char, faStatusData>, scannerStatus> statusMap;
        bool nfaMappingTraverser(NfaStatus<char, faStatusData> status)
        {
            scannerStatus newStatus = new scannerStatus();
            newStatus.IsResult = status.Data.GetIsEndStatus();
            if (newStatus.IsResult)
            {
                newStatus.ResultTokenType = status.Data.Type;
            }
            this.statusMap.Add(status, newStatus);
            return true;
        }
        bool nfaTransitionTraverser(NfaStatus<char, faStatusData> status)
        {
            this.targetEDFA.Add(
                this.statusMap[status],
                new Dictionary<char, scannerStatus>());
            foreach (NfaStatusTransition<char, faStatusData> transition in status.Next)
            {
                this.targetEDFA[this.statusMap[status]].Add(
                    transition.Condition,
                    this.statusMap[transition.NextStatus]);
            }
            return true;
        }
        private faStatusData faStatusMergeChecker(faStatusData d1, faStatusData d2)
        {
            faStatusData nd;//Should not modify the original data
            if (d1.GetIsEndStatus())
            {
                if (d2.GetIsEndStatus())//d1:end  d2:end
                {
                    if (d1.Type != d2.Type)
                    {
                        throw (new Exception("Merging different ending-status:"+d1.Type.ToString()+","+d2.Type.ToString()));
                    }
                    else//they are the same result type
                    {
                        nd = new faStatusData(d1.Type);
                    }
                }
                else//d1:end  d2:non-end
                {
                    nd = new faStatusData(d1.Type);
                }
            }
            else
            {
                if (d2.GetIsEndStatus())//d1:non-end d2:end
                {
                    nd = new faStatusData(d2.Type);
                }
                else//d1:non-end d2:non-end
                {
                    nd = new faStatusData();
                }
            }
            if (nd.GetIsEndStatus())
            {

            }
            return nd;
        }
    }
    /// <summary>
    /// Data to storaged in (N/ED)FA
    /// </summary>
    class faStatusData : INfaStatusData<char>
    {
        /// <summary>
        /// Determines whether [is end status].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is end status]; otherwise, <c>false</c>.
        /// </returns>
        public bool GetIsEndStatus()
        {
            return this.isEndStatus;
        }
        /// <summary>
        /// Set the 'is end status'.
        /// </summary>
        /// <param name="isEndStatus">if set to <c>true</c> [is end status].</param>
        public void SetIsEndStatus(bool isEndStatus)
        {
            this.isEndStatus = isEndStatus;
        }
        bool isEndStatus;
        /// <summary>
        /// Initializes a new instance of a non-end <see cref="faStatusData"/> class.
        /// </summary>
        /// <param name="isEndStatus">if set to <c>true</c> [is end status].</param>
        public faStatusData()
        {
            this.isEndStatus = false;
        }
        /// <summary>
        /// Initializes a new instance of an end <see cref="faStatusData"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public faStatusData(TokenType type)
        {
            this.isEndStatus = true;
            this.Type = type;
        }
        public TokenType Type;
    }
    /// <summary>
    /// Data useful for scannerBuilder.
    /// Define those RegularExps
    /// </summary>
    class scannerBuilderData
    {
        public Dictionary<TokenType, regularPart> Exps;
        /// <summary>
        /// Initializes a new instance of the <see cref="scannerBuilderData"/> class.
        /// </summary>
        public scannerBuilderData()
        {
            #region CharHasher
            //Due to ths huge status table growing.We cannot define the scanner regular char by char.
            //We must classify those different kinds of chars
            //a represent [b-fA-F]
            //g represent [h-wG-Wy-zY-Z]
            //x represent [X]
            //$ represent [AllOtherChar]
            //1 represent [2-9]
            this.CharHasher = new scannerCharHasher('$');
            this.CharHasher.AddMap('b', 'f', 'a');
            this.CharHasher.AddMap('A', 'F', 'a');
            this.CharHasher.AddMap('h', 'w', 'g'); 
            this.CharHasher.AddMap('G', 'W', 'g'); 
            this.CharHasher.AddMap('y', 'z', 'g'); 
            this.CharHasher.AddMap('Y', 'Z', 'g'); 
            this.CharHasher.AddMap('X', 'X', 'x'); 
            this.CharHasher.AddMap('2', '9', '1'); 
            #endregion
            #region define some elements
            #region Alphabets
            #region alphabetA
            regularPart alphabetA = new regularPart('a');
            #endregion
            #region alphabetG
            regularPart alphabetG = new regularPart('g');
            #endregion
            #region alphabetX
            regularPart alphabetX = new regularPart('x');
            #endregion
            #endregion
            #region Numbers
            #region NonZero
            regularPart nonZero = new regularPart('1');
            #endregion
            #region Zero
            regularPart zero = new regularPart('0');
            #endregion 
            #endregion
            #region SingleChar
            #region Alphabet
            regularPart alphabet = new regularPart(
                regularPartType.MultiChoice,
                alphabetA,
                alphabetG,
                alphabetX);
            #endregion
            #region Number
            regularPart number = new regularPart(
                regularPartType.MultiChoice,
                zero,
                nonZero);
            #endregion
            #region Hex
            regularPart hex = new regularPart(
                regularPartType.MultiChoice,
                number,
                alphabetA);
            #endregion           
            #region Undefined
            regularPart undefined = new regularPart('$');
            #endregion
            #region Qoute
            regularPart qoute = new regularPart('\"');
            #endregion
            #region IComma
            regularPart iComma = new regularPart('\'');
            #endregion
            #region Escape
            regularPart escape = new regularPart('\\');
            #endregion
            #region Underline
            regularPart underline = new regularPart('_');
            #endregion
            #region At
            regularPart at = new regularPart('@');
            #endregion
            #endregion
            #region Combination
            #region Numberstring||empty
            regularPart numberString = new regularPart(
                regularPartType.Concatenation,
                true,
                true,
                number);
            #endregion
            #region Hexstring||empty
            regularPart hexString = new regularPart(
                regularPartType.Concatenation,
                true,
                true,
                hex);
            #endregion
            #region NormalChar
            regularPart normalChar = new regularPart(
                regularPartType.MultiChoice,
                undefined,
                new regularPart(
                    regularPartType.Concatenation,
                    escape,
                    undefined));

            #endregion 
            #endregion
            #endregion
            #region Exps
            this.Exps = new Dictionary<TokenType, regularPart>();
            #region LiteralInteger
            Exps.Add(
                TokenType.LiteralInteger,
                new regularPart(
                    regularPartType.MultiChoice,
                    new regularPart(
                        regularPartType.Concatenation,
                        number,
                        numberString
                    ),//NormalInterger
                    new regularPart(
                        regularPartType.Concatenation,
                        zero,
                        alphabetX,
                        hex,
                        hexString
                    )//HexInterger
                )
            );
            #endregion
            #region LiteralReal
            this.Exps.Add(
                TokenType.LiteralReal,
                new regularPart(
                    regularPartType.Concatenation,
                    number,
                    numberString,
                    new regularPart('.'),
                    number,
                    numberString
                    )
                    );
            #endregion
            #region LiteralString
            Exps.Add(
                TokenType.LiteralString,
                new regularPart(
                    regularPartType.Concatenation,
                    qoute,
                    new regularPart(
                        regularPartType.Concatenation,
                        true,
                        true,
                        normalChar),
                    qoute));
            #endregion
            #region LiteralClearString
            Exps.Add(
                TokenType.LiteralClearString,
                new regularPart(
                    regularPartType.Concatenation,
                    at,
                    qoute,
                    new regularPart(
                        regularPartType.Concatenation,
                        true,
                        true,
                        normalChar),
                    qoute));
            #endregion
            #region LiteralChar
            Exps.Add(
                TokenType.LiteralChar,
                new regularPart(
                    regularPartType.MultiChoice,
                    new regularPart(
                        regularPartType.Concatenation,
                        iComma,
                        normalChar,
                        iComma
                    )//NormalString
                )
            );
            #endregion
            #region PunctuatorExps
            this.Exps.Add(TokenType.PunctuatorAbove, new regularPart('>'));
            this.Exps.Add(TokenType.PunctuatorBelow, new regularPart('<'));
            this.Exps.Add(TokenType.PunctuatorDivision, new regularPart('/'));
            this.Exps.Add(TokenType.PunctuatorEqual, new regularPart('='));
            this.Exps.Add(TokenType.PunctuatorLBBracket, new regularPart('{'));
            this.Exps.Add(TokenType.PunctuatorLBracket, new regularPart('('));
            this.Exps.Add(TokenType.PunctuatorLMBracket, new regularPart('['));
            this.Exps.Add(TokenType.PunctuatorMinus, new regularPart('-'));
            this.Exps.Add(TokenType.PunctuatorMultiply, new regularPart('*'));
            this.Exps.Add(TokenType.PunctuatorPlus, new regularPart('+'));
            this.Exps.Add(TokenType.PunctuatorRBBracket, new regularPart('}'));
            this.Exps.Add(TokenType.PunctuatorRBracket, new regularPart(')'));
            this.Exps.Add(TokenType.PunctuatorRMBracket, new regularPart(']'));
            this.Exps.Add(TokenType.PunctuatorDot, new regularPart('.'));
            this.Exps.Add(TokenType.PunctuatorComma, new regularPart(','));
            this.Exps.Add(TokenType.PunctuatorSplitter, new regularPart(';'));
            #endregion
            #region Identifier
            this.Exps.Add(TokenType.Identifier, new regularPart(
                   regularPartType.Concatenation,
                   new regularPart(
                       regularPartType.MultiChoice,
                       false,
                       true,
                       alphabet,
                       underline)
                       ,
                   new regularPart(
                       regularPartType.MultiChoice,
                       true,
                       true,
                       alphabet,
                       underline,
                       number)
                       ));
            #endregion
            #region NonTerminal(ReservedForGfcAutoInitial)
            this.Exps.Add(TokenType.NonTerminal,
                new regularPart(
                    regularPartType.Concatenation,
                    new regularPart('['),
                    new regularPart(
                        regularPartType.MultiChoice,
                        false,
                        true,
                        alphabet,
                        underline,
                        number
                        ),
                    new regularPart(']')));
            #endregion
            #region BlockComment(WontReturn)
            this.Exps.Add(TokenType.BlockComment,
                new regularPart(regularPartType.Concatenation,
                    new regularPart('/'),
                    new regularPart('*'),
                    new regularPart(regularPartType.Concatenation, true, true, new regularPart('$')),
                    new regularPart('*'),
                    new regularPart('/')
                    ));
            #endregion
            #endregion
        }
        public scannerCharHasher CharHasher;
    }
    class scannerCharHasher : ICharHasher
    {
        public char UndefinedChar
        {
            get
            {
                return this.undefinedChar;
            }
        }
        char undefinedChar;
        public char Hash(char c)
        {
            for (int i = 0; i < this.mapstring.Length; i+=3)
            {
                if (c >= this.mapstring[i] && c <= this.mapstring[i + 1])
                {
                    return this.mapstring[i + 2];
                }
            }
            return c;
        }
        public scannerCharHasher(char undifinedChar)
        {
            this.undefinedChar = undifinedChar;
            this.mapstring = new char[0];
        }
        char[] mapstring;
        /// <summary>
        /// Adds a mapping.
        /// </summary>
        /// <param name="startChar">The start char.</param>
        /// <param name="endChar">The end char.</param>
        /// <param name="hashChar">The hash char.</param>
        public void AddMap(char startChar, char endChar, char hashChar)
        {
            Array.Resize<char>(ref this.mapstring, this.mapstring.Length+3);
            this.mapstring[this.mapstring.Length - 3] = startChar;
            this.mapstring[this.mapstring.Length - 2] = endChar;
            this.mapstring[this.mapstring.Length - 1] = hashChar;
        }
    }
    struct scannerBuilderTargetData
    {
        public scannerStatus StartStatus;
        public ICharHasher CharHasher;
        public Dictionary<scannerStatus, Dictionary<char, scannerStatus>> TargetEdfa;
    }
}
