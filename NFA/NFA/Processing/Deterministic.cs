using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.NFA.Processing
{
    public class Deterministic<TCondition, TData> 
            where TData:INfaStatusData<TCondition>
            where TCondition:IEquatable<TCondition>
    {
        public static void MakeDeterministic(Nfa<TCondition, TData> nfa,NfaStatusDataMergingHandler<TCondition,TData> op)
        {
            NonEpsilon<TCondition, TData>.MakeNonEpsilon(nfa, op);
            Singularity<TCondition, TData>.MakeSingularity(nfa, op);
        }
    }
}
