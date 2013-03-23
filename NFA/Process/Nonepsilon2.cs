using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    class Nonepsilon2<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {

        public static void MakeNonepsilon(Nfa<TCondition, TData> nfa, NfaStatusDataMergingHandler<TCondition, TData> op)
        {
            new Nonepsilon2<TCondition, TData>(nfa, op);
        }
        Nonepsilon2(Nfa<TCondition, TData> nfa, NfaStatusDataMergingHandler<TCondition, TData> op)
        {
            this.closuresMapping = new Dictionary<epsilonClosure<TCondition, TData>, NfaStatus<TCondition, TData>>();
            this.op = op;
            nfa.Traverse(this.mappingClosuresTraverser);
            nfa.Traverse(this.makeNonepsilonTraverser);
            nfa.StartStatus = this.closuresMapping[epsilonClosure<TCondition, TData>.MakeEpsilonClosure(nfa.StartStatus)];
            strict<TCondition, TData>.MakeStrict(nfa);

        }
        NfaStatusDataMergingHandler<TCondition, TData> op;
        bool mappingClosuresTraverser(NfaStatus<TCondition, TData> status)
        {
            epsilonClosure<TCondition, TData> closure = epsilonClosure<TCondition, TData>.MakeEpsilonClosure(status);
            if (!this.closuresMapping.ContainsKey(closure))
            {
                this.closuresMapping.Add(closure, closure.MergeDataToNewStatus(op));
            }
            return true;
        }
        Dictionary<epsilonClosure<TCondition, TData>,NfaStatus<TCondition,TData>> closuresMapping;
        bool makeNonepsilonTraverser(NfaStatus<TCondition,TData> status)
        {
            epsilonClosure<TCondition, TData> closure = epsilonClosure<TCondition, TData>.MakeEpsilonClosure(status);
            foreach (NfaStatus<TCondition, TData> substatus in closure.Items)
            {
                foreach (NfaStatusTransition<TCondition, TData> transition in substatus.Next)
                {
                    if (!transition.IsEpsilion)
                    {
                        NfaStatus<TCondition, TData> nsrc = this.closuresMapping[closure];
                        NfaStatus<TCondition, TData> ndst = this.closuresMapping[epsilonClosure<TCondition, TData>.MakeEpsilonClosure(transition.NextStatus)];
                        if (!nsrc.Next.Exists(tt => tt.Condition.Equals(transition.Condition) && tt.NextStatus == ndst))
                        {
                            nsrc.AddTransition(new NfaStatusTransition<TCondition, TData>(
                                ndst,
                                transition.Condition));
                        }
                    }
                }
            }
            return true;
        }
    }
}
