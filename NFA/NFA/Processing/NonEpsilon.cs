using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.NFA.Processing
{
    /// <summary>
    /// 
    /// </summary>
    class NonEpsilon<TCondition, TData>
            where TData:INfaStatusData<TCondition>
            where TCondition:IEquatable<TCondition>
    {
        public static void MakeNonEpsilon(Nfa<TCondition, TData> nfa,NfaStatusDataMergingHandler<TCondition,TData> op)
        {
            traverser<TCondition, TData> traverser = new traverser<TCondition, TData>();
            traverser.DataMergingOp = op;
            traverser.Nfa = nfa;
            while (!nfa.Traverse(traverser.MakeNonEpsilonTraverser)) { }
        }
    }

}
