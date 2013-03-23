using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    public class Determinstic<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        Determinstic(Nfa<TCondition, TData> nfa,NfaStatusDataMergingHandler<TCondition,TData> op)
        {
            Nonepsilon2<TCondition,TData>.MakeNonepsilon(nfa,op);
            Singularity<TCondition, TData>.MakeSingular(nfa, op);
        }
        public static void MakeDeterminstic(Nfa<TCondition, TData> nfa,NfaStatusDataMergingHandler<TCondition,TData> op)
        {
            new Determinstic<TCondition, TData>(nfa, op);
        }
    }
}
