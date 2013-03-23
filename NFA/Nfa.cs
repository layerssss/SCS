using System;
using System.Collections.Generic;
using System.Text;

namespace NFA
{
    /// <summary>
    /// Data object for an NFA
    /// </summary>
    /// <typeparam name="TCondition">Type of transition condition.</typeparam>
    /// <typeparam name="TData">Type of status data.</typeparam>
    public class Nfa<TCondition, TData> 
        where TData : INfaStatusData<TCondition>
        where TCondition: IEquatable<TCondition>
    {
        /// <summary>
        /// The starting status.
        /// </summary>
        public NfaStatus<TCondition, TData> StartStatus;
        /// <summary>
        /// Traverses the specified op.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <returns>Whether the Traversing completed.</returns>
        public bool Traverse(NfaTraverseHandler<TCondition, TData> op)
        {
            return this.StartStatus.Traverse(op, Guid.NewGuid());
        }
        public NfaStatus<TCondition, TData> NewStatus(TData data)
        {
            return new NfaStatus<TCondition, TData>(data, this);
        }
        public void MakeStrict()
        {
            Process.strict<TCondition, TData>.MakeStrict(this);
        }
    }
    public class NfaStatus<TCondition, TData> : IEquatable<NfaStatus<TCondition, TData>>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        public Guid Id;
        public Nfa<TCondition, TData> Nfa;
        public TData Data;
        /// <summary>
        /// For traversing usage.
        /// </summary>
        Guid visited;
        public List<NfaStatusTransition<TCondition, TData>> Next;
        public List<NfaStatusTransition<TCondition,TData>> Prev;
        public void AddTransition(NfaStatusTransition<TCondition, TData> transition)
        {
            transition.PrevStatus = this;
            this.Next.Add(transition);
        }
        public NfaStatus(TData data,Nfa<TCondition,TData> nfa)
        {
            this.Id = Guid.NewGuid();
            this.Next = new List<NfaStatusTransition<TCondition, TData>>();
            this.Prev = new List<NfaStatusTransition<TCondition, TData>>();
            this.Data = data;
            this.visited = new Guid();
            this.Nfa = nfa;
        }
        public bool Equals(NfaStatus<TCondition, TData> s)
        {
            return this.Id == s.Id;
        }
        /// <summary>
        /// Traverses the specified op.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <param name="acceptedVisited">Which 'visited' accept.</param>
        /// <returns>Whether the Traversing completed.</returns>
        public bool Traverse(NfaTraverseHandler<TCondition, TData> op, Guid guid)
        {
            if (this.visited == guid)
            {
                return true;
            }
            this.visited = guid;//Mark it visited.
            if (!op( this))
            {
                return false;//Op stoped it.
            }
            foreach (NfaStatusTransition<TCondition, TData> transition in this.Next)
            {
                if (!transition.NextStatus.Traverse(op, guid))
                {
                    return false;//One substatus' op stoped it.
                }
            }
            return true;//Traversing completed.
        }
    }
    public class NfaStatusTransition<TCondition, TData>
        where TData : INfaStatusData<TCondition>
        where TCondition : IEquatable<TCondition>
    {
        public NfaStatus<TCondition, TData> NextStatus;
        public NfaStatus<TCondition, TData> PrevStatus;
        /// <summary>
        /// Build a non-epsilion transition.
        /// </summary>
        /// <param name="nextStatus">The next status.</param>
        /// <param name="condition">The condition.</param>
        public NfaStatusTransition(NfaStatus<TCondition, TData> nextStatus, TCondition condition)
            : this(nextStatus)
        {
            this.Condition = condition;
            this.IsEpsilion = false;
        }
        /// <summary>
        /// Build an epsilion transition.
        /// </summary>
        /// <param name="nextStatus">The next status.</param>
        public NfaStatusTransition(NfaStatus<TCondition, TData> nextStatus)
        {
            this.NextStatus = nextStatus;
            if (!nextStatus.Prev.Contains(this))
            {
                nextStatus.Prev.Add(this);
            }
            this.IsEpsilion = true;
        }
        public TCondition Condition;
        public bool IsEpsilion;
    }
    public interface INfaStatusData<TCondition>
    {
        bool GetIsEndStatus();
    }
    /// <summary>
    /// <param name="condition">The condition.</param>
    /// <param name="status">The status.)</param>
    /// <returns>Whether continue the traversing</returns>
    /// </summary>
    public delegate bool NfaTraverseHandler<TCondition, TData>(NfaStatus<TCondition, TData> status)
        where TData : INfaStatusData<TCondition>
        where TCondition: IEquatable<TCondition>;
}
