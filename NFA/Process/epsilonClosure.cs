using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    class epsilonClosure<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        public override bool Equals(object obj)
        {
            return this.GetHashCode() == ((epsilonClosure<TCondition, TData>)obj).GetHashCode();
        }
        public override int GetHashCode()
        {
            int i = 0;
            foreach (NfaStatus<TCondition, TData> status in this.Items)
            {
                i ^= status.Id.GetHashCode();
            }
            return i;
        }
        public Nfa<TCondition, TData> Nfa;
        public List<NfaStatus<TCondition,TData>> Items;
        epsilonClosure()
        {
            this.Items = new List<NfaStatus<TCondition, TData>>();
        }
        public static epsilonClosure<TCondition, TData> MakeEpsilonClosure(NfaStatus<TCondition, TData> status)
        {
            epsilonClosure<TCondition, TData> closure = new epsilonClosure<TCondition, TData>();
            closure.Nfa = status.Nfa;
            MakeEpsilonClosure(status, closure);
            return closure;
            
        }
        static void MakeEpsilonClosure(NfaStatus<TCondition, TData> status,epsilonClosure<TCondition, TData> closure)
        {
            closure.Items.Add(status);
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Next)
            {
                if (transition.IsEpsilion)
                {
                    if (!closure.Items.Contains(transition.NextStatus))
                    {
                        closure.Items.Add(transition.NextStatus);
                        MakeEpsilonClosure(transition.NextStatus, closure);
                    }
                }
            }
        }
        public NfaStatus<TCondition, TData> MergeDataToNewStatus(NfaStatusDataMergingHandler<TCondition,TData> op)
        {
            NfaStatus<TCondition, TData> nd = this.Nfa.NewStatus(this.Items[0].Data);
            for (int i = 1; i < this.Items.Count; i++)
            {
                nd.Data = op(nd.Data, this.Items[i].Data);
            }
            return nd;
        }
    }
}
