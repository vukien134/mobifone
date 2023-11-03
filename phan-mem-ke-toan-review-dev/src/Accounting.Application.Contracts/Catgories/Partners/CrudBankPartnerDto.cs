using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Partners
{
    public class CrudBankPartnerDto : CruOrgBaseDto
    {
        public string Ord0 { get; set; }
        public string PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public string BankAccNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Note { get; set; }        
    }
}
