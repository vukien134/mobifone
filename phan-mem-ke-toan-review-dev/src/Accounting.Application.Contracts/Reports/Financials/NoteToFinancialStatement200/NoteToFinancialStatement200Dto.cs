using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Financials
{
    public class NoteToFinancialStatement200Dto
    {
        public string GroupId { get; set; }
        public int Sort { get; set; }
        public int Ord { get; set; }
        public string Bold { get; set; }
        public string Target { get; set; }
        public string TargetE { get; set; }
        public decimal? Num01 { get; set; }
        public decimal? Num02 { get; set; }
        public decimal? Num03 { get; set; }
        public decimal? Num04 { get; set; }
        public decimal? Num05 { get; set; }
        public decimal? Num06 { get; set; }
        public decimal? Num07 { get; set; }
        public decimal? Num08 { get; set; }
        public decimal? Num09 { get; set; }
        public decimal? Num10 { get; set; }
        public string Printable { get; set; }
        public string IsGroup { get; set; }
        public string OrdCode { get; set; }
        public string Year { get; set; }
    }
}
