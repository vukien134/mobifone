﻿using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Financials.StatementOfValueAddedTax
{
    public class DataStatementTaxDto
    {
        public int? Year { get; set; }
        public int? Ord { get; set; }
        public string Printable { get; set; }
        public string Bold { get; set; }
        public string Ord0 { get; set; }
        public string Description { get; set; }
        public string DescriptionE { get; set; }
        public int? Rank { get; set; }
        public string NumberCode { get; set; }
        public string Formular { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string Condition { get; set; }
        public int? Sign { get; set; }
        public string NumberCode1 { get; set; }
        public decimal? Amount1 { get; set; }
        public string NumberCode2 { get; set; }
        public decimal? Amount2 { get; set; }
        public string PrintWhen { get; set; }
        public int Tag { get; set; }
        public string Id11 { get; set; }
        public string Id12 { get; set; }
        public string Id21 { get; set; }
        public string Id22 { get; set; }
        public string En1 { get; set; }
        public string En2 { get; set; }
        public string Re1 { get; set; }
        public string Re2 { get; set; }
        public string Va1 { get; set; }
        public string Va2 { get; set; }
        public string Mt1 { get; set; }
        public string Mt2 { get; set; }
        public string AssignValue { get; set; }
        public string TaxPeriod { get; set; }
        public decimal? AmountWithoutVat { get; set; }
        public decimal? AmountVat { get; set; }

    }
}
