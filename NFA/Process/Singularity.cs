using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    public class Singularity<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        NfaStatusDataMergingHandler<TCondition, TData> DataMergingOp;
        Singularity(Nfa<TCondition, TData> nfa, NfaStatusDataMergingHandler<TCondition, TData> op)
        {
            this.DataMergingOp = op;
            while (!nfa.Traverse(traverser)) ;
        }
        public static void MakeSingular(Nfa<TCondition, TData> nfa, NfaStatusDataMergingHandler<TCondition, TData> op)
        {
            new Singularity<TCondition, TData>(nfa, op);
        }
        bool traverser(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Next)
            {
                List<NfaStatusTransition<TCondition, TData>> nl = status.Next.FindAll(t => t.Condition.Equals(transition.Condition));
                if (nl.Count > 1)//NotSingular
                {
                    Merger<TCondition, TData>.Merge(nl[0].NextStatus, nl[1].NextStatus, this.DataMergingOp);
                    return false;//StopTraversing
                }
            }
            return true;
        }
    }
}
