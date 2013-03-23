using System;
using System.Collections.Generic;
using System.Text;


namespace SCS.NFA.Processing
{
    /// <summary>
    /// Some traversers to do basic modifing operation.
    /// </summary>
    /// <typeparam name="TCondition">The type of the condition.</typeparam>
    /// <typeparam name="TData">The type of the data.</typeparam>
    class traverser<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        public NfaStatus<TCondition, TData> Status1;
        public NfaStatus<TCondition, TData> Status2;
        public Nfa<TCondition, TData> Nfa;
        public NfaStatusDataMergingHandler<TCondition, TData> DataMergingOp;
        public List<NfaStatus<TCondition, TData>> StatusCollection;
        /// <summary>
        /// Moves the parents traverser.
        /// Sign one status' all parents to another one.
        /// Need:Status1,Status2,DataMergingOp.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool MoveParentsTraverser(NfaStatus<TCondition, TData> status)
        {
            //      status->Status1             status->
            //                          =>
            //              Status2             status->Status2
            if (this.Status1 == this.Status2)
            {
                throw (new Exception());
            }
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Transitions)
            {
                if (transition.NextStatus == this.Status1)
                {
                    #region copy transition to new one(status->this.Status2)
                    if (transition.IsEpsilion)
                    {
                        status.Transitions.Add(new NfaStatusTransition<TCondition, TData>(this.Status2));
                    }
                    else
                    {
                        status.Transitions.Add(new NfaStatusTransition<TCondition, TData>(this.Status2, transition.Condition));
                    }
                    #endregion
                    #region Merging data
                    Status2.Data = DataMergingOp(Status1.Data, Status2.Data);
                    #endregion
                    #region remove the old one.
                    transition.NextStatus = null;
                    #endregion
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds the status.
        /// To make sure one status has parent(s) is in an NFA.
        /// Need:Nfa,Status1,Status2.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool FindParentTraverser(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Transitions)
            {
                #region Parent found.Stop.
                if (transition.NextStatus == this.Status1)
                {
                    return false;
                }
                #endregion
            }
            return true;
        }
        /// <summary>
        /// Making singularity traverser.
        /// Need:Nfa,DataMergingOp.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool MakeSingularityTraverser(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Transitions)
            {
                List<NfaStatusTransition<TCondition, TData>> nl = status.Transitions.FindAll(t => t.Condition.Equals(transition.Condition));
                if (nl.Count > 1)//NotSingular
                {
                    Merger<TCondition, TData>.Merge(this.Nfa, nl[0].NextStatus, nl[1].NextStatus, this.DataMergingOp);
                    return false;//StopTraversing
                }
            }
            return true;
        }
        /// <summary>
        /// Making strict traverser.
        /// Need:.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool MakeStrictTraverser(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Transitions)
            {
                if (status.Transitions.RemoveAll(tt => tt.IsEpsilion && tt.NextStatus == status)>0)//SelfEpsilon
                {
                    return false;//StopTraversing
                }
                List<NfaStatusTransition<TCondition, TData>> nl = status.Transitions.FindAll(t => t.Condition.Equals(transition.Condition) && t.NextStatus.Equals(transition.NextStatus));
                if (nl.Count > 1)//NotStrict
                {
                    status.Transitions.Remove(transition);
                    return false;//StopTraversing
                }
            }
            return true;
        }
        /// <summary>
        /// Noes the null traverser.
        /// Need:
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool NoNullTraverser(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Transitions)
            {
                if (transition.NextStatus == null)//Already marked to removed.
                {
                    status.Transitions.Remove(transition);
                    return false;//StopTraversing
                }
            }
            return true;
        }
        /// <summary>
        /// NonEpsilon traverser.
        /// Need:DataMergerOp , Nfa
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool MakeNonEpsilonTraverser(NfaStatus<TCondition, TData> status)
        {
            foreach (NfaStatusTransition<TCondition, TData> transition in status.Transitions)
            {
                if (transition.IsEpsilion)
                {
                    Merger<TCondition, TData>.Merge(
                        this.Nfa, 
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
