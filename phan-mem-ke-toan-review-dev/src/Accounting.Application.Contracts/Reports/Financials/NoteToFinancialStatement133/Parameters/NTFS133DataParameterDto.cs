using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class NTFS133DataParameterDto
    {
        public int Year { get; set; }
        public int UsingDecision { get; set; } // QD_TC
    }
}
