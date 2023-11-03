﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Reports.Statements.T200.Defaults
{
    public class DefaultFStatement200L13 : AuditedEntity<string>
    {
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string NumberCode { get; set; }
        public int? Rank { get; set; }
        public string Formular { get; set; }
        public string Acc { get; set; }
        //TIEN_GT1
        public decimal? ValueAmount1 { get; set; }
        //TIEN_LAI1
        public decimal? InterestAmount1 { get; set; }
        //TIEN_TN1
        public decimal? DebtPayingAmount1 { get; set; }
        //TIEN_GT2
        public decimal? ValueAmount2 { get; set; }
        //TIEN_LAI2
        public decimal? InterestAmount2 { get; set; }
        //TIEN_TN2
        public decimal? DebtPayingAmount2 { get; set; }
        public decimal? Up { get; set; }
        public decimal? Down { get; set; }
        public string Title { get; set; }
    }
}
