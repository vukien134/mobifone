﻿using System;
using Accounting.BaseDtos;

namespace Accounting.Reports.Financials.Tenant
{
    public class FStatement200L07Dto : TenantOrgDto
    {
        public int? Year { get; set; }
        public int? UsingDecision { get; set; }
        public int? Sort { get; set; }
        public string Bold { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string DebitOrCredit { get; set; }
        public string Type { get; set; }
        public string NumberCode { get; set; }
        public int? Rank { get; set; }
        public string Formular { get; set; }
        public string OriginalPriceAcc { get; set; }
        public string PreventivePriceAcc { get; set; }
        public decimal? OriginalPrice2 { get; set; }
        public decimal? PreventivePrice2 { get; set; }
        public decimal? OriginalPrice1 { get; set; }
        public decimal? PreventivePrice1 { get; set; }
        public string Title { get; set; }
    }
}

