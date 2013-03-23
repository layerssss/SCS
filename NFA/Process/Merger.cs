using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    public class Merger<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        Merger(NfaStatus<TCondition,TData> status1,NfaStatus<TCondition,TData> status2,NfaStatusDataMergingHandler<TCondition,TData> op)
        {
            if (status1.Nfa != status2.Nfa)
            {
                throw (new Exception());
            }
            foreach (NfaStatusTransition<TCondition, TData> transition in status2.Next)
            {
                status1.AddTransition(transition);
            }
            foreach (NfaStatusTransition<TCondition, TData> transition in status2.Prev)
            {
                transition.NextStatus = status1;
                status1.Prev.Add(transition);
            }
            status1.Data = op(status1.Data, status2.Data);
            strict<TCondition, TData>.MakeStrict(status1.Nfa);
        }
        public static void Merge(NfaStatus<TCondition, TData> status1, NfaStatus<TCondition, TData> status2, NfaStatusDataMergingHandler<TCondition, TData> op)
        {
            new Merger<TCondition, TData>(status1, status2,op);
        }
    }
}
