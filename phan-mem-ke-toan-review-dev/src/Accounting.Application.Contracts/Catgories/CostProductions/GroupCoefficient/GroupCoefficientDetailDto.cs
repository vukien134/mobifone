﻿using Accounting.BaseDtos;


namespace Accounting.Catgories.CostProductions
{
    public class GroupCoefficientDetailDto : CruOrgBaseDto
    {
        public string id { get; set; }
        public string GroupCoefficientId { get; set; }
        public string FProductWork { get; set; }
        public int Year { get; set; }
        public string FProductWorkCode { get; set; }
        public string GroupCoefficientCode { get; set; }
        public decimal? January { get; set; }
        public decimal? February { get; set; }
        public decimal? March { get; set; }
        public decimal? April { get; set; }
        public decimal? May { get; set; }
        public decimal? June { get; set; }
        public decimal? July { get; set; }
        public decimal? August { get; set; }
        public decimal? September { get; set; }
        public decimal? October { get; set; }
        public decimal? November { get; set; }
        public decimal? December { get; set; }
        public string FProductWorkName { get; set; }
    }
}
