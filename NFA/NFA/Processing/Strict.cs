using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.NFA.Processing
{
    /// <summary>
    /// There shouldn't be any transitions with 'null' child status.
    /// There shouldn't be two transitions with the same parent and child status and the same condition.
    /// There shouldn't be a self non-epsilon transition.
    /// Then we call it strict.
    /// Or it cannot be called an NFA(strict NFA).
    /// </summary>
    /// <typeparam name="TCondition">The type of the condition.</typeparam>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class Strict<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        /// <summary>
        /// Making it strict.
        /// </summary>
        /// <param name="nfa">The nfa.</param>
        public static void MakeStrict(Nfa<TCondition, TData> nfa)
        {
            traverser<TCondition, TData> traverser = new traverser<TCondition, TData>();
            while (!nfa.Traverse(traverser.NoNullTraverser))
            {
            }
            while(!nfa.Traverse(traverser.MakeStrictTraverser)){
            }
        }
    }
}
