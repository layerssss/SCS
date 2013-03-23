using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    public class Nonepsilon<TCondition, TData>
            where TData:INfaStatusData<TCondition>
            where TCondition:IEquatable<TCondition>
    {
        NfaStatusDataMergingHandler<TCondition, TData> DataMergingOp;
        Nonepsilon(Nfa<TCondition, TData> nfa, NfaStatusDataMergingHandler<TCondition, TData> op)
        {
            this.DataMergingOp = op;
            while (!nfa.Traverse(traverser)) ;
        }
        public static void MakeNonspsilon(Nfa<TCondition, TData> nfa, NfaStatusDataMergingHandler<TCondition, TData> op)
        {
            new Nonepsilon<TCondition, TData>(nfa, op);
        }
        bool traverser(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Next)
            {
                if (transition.IsEpsilion)
                {
                    Merger<TCondition, TData>.Merge(
                        status,
                        transition.NextStatus,
                        this.DataMergingOp
                        );
                    return false;
                }
            }
            return true;
        }
    }
}
