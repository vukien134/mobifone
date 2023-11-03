using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Partners
{
    public class PartnerComboItemDto : BaseComboItemDto
    {
        public string Address { get; set; }
        public string Representative { get; set; }
        public string TaxCode { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string OtherContact { get; set; }
        public string ContactPerson { get; set; }
    }
}
