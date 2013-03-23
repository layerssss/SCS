using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.NFA.Processing
{
    /// <summary>
    /// Because transition in NFA is a one-way data stucture.So we make a independent merging model.
    /// </summary>
    public class Merger<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        /// <summary>
        /// Merges two status in an NFA.
        /// They shouldn't be the same.
        /// </summary>
        /// <param name="nfa">The nfa.</param>
        /// <param name="s1">The first status to merge.</param>
        /// <param name="s2">The second status to merge.</param>
        /// <param name="statusDataMergingOp">The status data merging operation.</param>
        public static void Merge(Nfa<TCondition,TData> nfa,NfaStatus<TCondition, TData> s1, NfaStatus<TCondition, TData> s2,NfaStatusDataMergingHandler<TCondition,TData> statusDataMergingOp)
        {
            traverser<TCondition, TData> traversers = new traverser<TCondition, TData>();
            #region checking
            #region Are they the same one?
            if (s1 == s2)
            {
                //throw (new Exception("The two status are the same one!"));
                return;
            } 
            #endregion
            #region Searching parents of s1 if s1 is not the start status.(to make sure status1 is in this NFA)
            if (nfa.StartStatus != s1)
            {
                traversers.Status1 = s1;
                if (nfa.Traverse(traversers.FindParentTraverser))
                {
                    throw (new Exception("s1 is not a status of this NFA!"));
                }
            }
            #endregion
            #region Searching parents of s2 if s2 is not the start status.(to make sure status1 is in this NFA)
            if (nfa.StartStatus != s2)
            {
                traversers.Status1 = s2;
                if (nfa.Traverse(traversers.FindParentTraverser))
                {
                    throw (new Exception("s1 is not a status of this NFA!"));
                }
            }
            #endregion
            #endregion
            #region change the transition between them
            foreach (NfaStatusTransition<TCondition, TData> transition in s1.Transitions)
            {
                if (transition.NextStatus == s2)
                {
                    if (transition.IsEpsilion)
                    {
                        transition.NextStatus = null;
                    }
                    else
                    {
                        transition.NextStatus = s1;
                    }
                }
            }
            
            #endregion
            #region searching parents of s2 and change those transitions
            traversers.Status1 = s2;
            traversers.Status2 = s1;
            traversers.DataMergingOp = statusDataMergingOp;
            nfa.Traverse(traversers.MoveParentsTraverser);
            #endregion
            #region searching children of s2 and change those transitions
            foreach (NfaStatusTransition<TCondition, TData> transition in s2.Transitions)
            {
                //      s2->transition.NextStatus                   s2
                //                                   =>
                //      s1                                          s1->transition.NextStatus
                #region just sign a new one. there's no need to remove the old one.It's already isolated.
                if (transition.IsEpsilion)
                {
                    s1.Transitions.Add(new NfaStatusTransition<TCondition, TData>(transition.NextStatus));
                }
                else
                {
                    s1.Transitions.Add(new NfaStatusTransition<TCondition, TData>(transition.NextStatus, transition.Condition));
                } 
                #endregion
                #region Merging data
                s1.Data = statusDataMergingOp(s1.Data, s2.Data);
                #endregion
            }
            #endregion
            #region Make it strict.(to sweep : 1. trasitions with same parents and children . 2. tansitions marked to be removed .)
            Strict<TCondition, TData>.MakeStrict(nfa);
            #endregion
        }
    }
    public delegate TData NfaStatusDataMergingHandler<TCondition,TData>(TData d1,TData d2) where TData:INfaStatusData<TCondition>;
}
