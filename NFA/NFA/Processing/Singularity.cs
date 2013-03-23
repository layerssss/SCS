using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.NFA.Processing
{
    /// <summary>
    /// One status cannot produce two transitions with the same condition(char).
    /// We call this sigularity.
    /// </summary>
    /// <typeparam name="TCondition">The type of the condition.</typeparam>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class Singularity<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        public static void MakeSingularity(Nfa<TCondition, TData> nfa,NfaStatusDataMergingHandler<TCondition,TData> op)
        {
            traverser<TCondition, TData> traverser = new traverser<TCondition, TData>()
            {
                DataMergingOp = op,
                Nfa = nfa
            };
            while (!nfa.Traverse(traverser.MakeSingularityTraverser))
            {
            }
        }
    }
}
