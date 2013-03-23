using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    class strict<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        strict(Nfa<TCondition,TData> nfa)
        {
            while (!nfa.Traverse(this.traverser1)) ;
        }
        bool traverser1(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Next)
            {
                if (status.Next.RemoveAll(tt => tt.IsEpsilion && tt.NextStatus == status) > 0)//SelfEpsilon
                {
                    return false;//StopTraversing
                }
                List<NfaStatusTransition<TCondition, TData>> nl = status.Next.FindAll(t => t.Condition.Equals(transition.Condition) && t.NextStatus.Equals(transition.NextStatus));
                if (nl.Count > 1)//NotStrict
                {
                    status.Next.Remove(transition);
                    return false;//StopTraversing
                }
            }
            return true;
        }
        public static void MakeStrict(Nfa<TCondition, TData> nfa)
        {
            new strict<TCondition, TData>(nfa);
        }
    }
}
