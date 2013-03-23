using System;
using System.Collections.Generic;
using System.Text;

namespace SCSTest
{
    public class NfaTestData : NFA.INfaStatusData<char>
    {
        public bool End;
        public bool GetIsEndStatus()
        {
            return End;
        }
        public List<string> Names;
        public NfaTestData(string i)
        {
            this.End = false;
            this.Names = new List<string>();
            this.Names.Add(i);
        }
    }
    public class NfaTest
    {
        public static NfaTestData DataMerger(NfaTestData a, NfaTestData b)
        {
            foreach (string s in b.Names)
            {
                if (!a.Names.Contains(s))
                {
                    a.Names.Add(s);
                }
            }
            return a;
        }

    }
}
