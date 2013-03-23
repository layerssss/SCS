using System;
using System.Collections.Generic;
using System.Text;

namespace SCS.Gfc
{
    public class GfcProductionDescription
    {
        public string Production;
        public event GfcProductionProcessingHandler Processing
        {
            add
            {
                this.Handler = value;
            }
            remove
            {
                if (this.Handler == value)
                {
                    this.Handler = null;
                }
            }
        }
        public GfcProductionProcessingHandler Handler;
    }
}
