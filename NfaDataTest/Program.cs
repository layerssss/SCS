using System;
using System.Collections.Generic;
using System.Text;
using NFA.Process;
using NFA;
using SCSTest;
namespace NfaDataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Nfa<char, NfaTestData> n1 = new Nfa<char, NfaTestData>();
            NfaStatus<char, NfaTestData> s1 = n1.NewStatus(new NfaTestData("1"));
            NfaStatus<char, NfaTestData> s2 = n1.NewStatus(new NfaTestData("2"));
            NfaStatus<char, NfaTestData> s3 = n1.NewStatus(new NfaTestData("3"));
            NfaStatus<char, NfaTestData> s4 = n1.NewStatus(new NfaTestData("4"));
            NfaStatus<char, NfaTestData> s5 = n1.NewStatus(new NfaTestData("5"));
            NfaStatus<char, NfaTestData> s6 = n1.NewStatus(new NfaTestData("6"));
            NfaStatus<char, NfaTestData> s7 = n1.NewStatus(new NfaTestData("7"));
            NfaStatus<char, NfaTestData> s8 = n1.NewStatus(new NfaTestData("8"));
            NfaStatus<char, NfaTestData> s9 = n1.NewStatus(new NfaTestData("9"));
            NfaStatus<char, NfaTestData> s10 = n1.NewStatus(new NfaTestData("10"));
            n1.StartStatus = s1;
            #region SingularityTest
            //s1.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s2, 'a'));
            //s1.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s3, 'b'));
            //s1.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s4, 'c'));
            //s3.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s5, 'c'));
            //s3.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s6, 'd'));
            //s4.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s6, 'e')); 
            //Singularity<char, NfaTestData>.MakeSingularity(n1, NfaTest.DataMerger);
            #endregion
            #region DeterminsticTest
            //s1.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s2));
            //s2.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s3, 'a'));
            //s3.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s2));
            //s3.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s4));
            //s1.Transitions.Add(new NfaStatusTransition<char,NfaTestData>(s4));
            //Deterministic<char, NfaTestData>.MakeDeterministic(n1, NfaTest.DataMerger);
            #endregion
            #region DeterminsticTest2
            //s1.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s2, 'l'));
            //s2.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s3));
            //s3.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s10));
            //s3.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s4));
            //s4.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s5));
            //s4.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s7));
            //s5.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s6, 'l'));
            //s6.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s9));
            //s7.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s8, 'd'));
            //s8.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s9));
            //s9.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s4));
            //s9.Transitions.Add(new NfaStatusTransition<char, NfaTestData>(s10));
            Determinstic<char, NfaTestData>.MakeDeterminstic(n1, NfaTest.DataMerger);
            #endregion
            s1.AddTransition(new NfaStatusTransition<char, NfaTestData>(s2));
            s2.AddTransition(new NfaStatusTransition<char, NfaTestData>(s1));
            n1.Traverse(Visiter);
            //Merger<char, NfaTestData>.Merge(n1, s1, s2, NfaTest.DataMerger);
            Determinstic<char, NfaTestData>.MakeDeterminstic(n1, NfaTest.DataMerger);
        }
        public static bool Visiter(NfaStatus<char, NfaTestData> status)
        {

            return true;
        }
    }
}
