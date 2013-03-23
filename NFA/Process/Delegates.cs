using System;
using System.Collections.Generic;
using System.Text;

namespace NFA.Process
{
    public delegate TData NfaStatusDataMergingHandler<TCondition, TData>(TData d1, TData d2) where TData : INfaStatusData<TCondition>;
}
