using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class VoucherTemplate : AuditedEntity<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string FileTemplate { get; set; }
        public string WindowId { get; set; }
        public string UrlApi { get; set; }
        public Window Window { get; set; }
    }
}
