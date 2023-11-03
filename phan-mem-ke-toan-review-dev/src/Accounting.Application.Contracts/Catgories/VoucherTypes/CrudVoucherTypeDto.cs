using Accounting.BaseDtos;

namespace Accounting.Catgories.VoucherTypes
{
    public class CrudVoucherTypeDto : CruBaseDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ListVoucher { get; set; }
        public string ListGroup { get; set; }
    }
}