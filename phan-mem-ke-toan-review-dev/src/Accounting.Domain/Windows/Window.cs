using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class Window : AuditedEntity<string>
    {
        public Window()
        {
            Tabs = new Collection<Tab>();
            ImportExcelTemplates = new Collection<ImportExcelTemplate>();
            ExportExcelTemplates = new Collection<ExportExcelTemplate>();            
            VoucherTemplates = new Collection<VoucherTemplate>();
            Buttons = new Collection<Button>();
            RegisterEvents = new Collection<RegisterEvent>();
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string WindowType { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? MaxRowEditInForm { get; set; }
        public int? OrdRowTab { get; set; }
        public string VoucherCode { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }
        public string FormLayout { get; set; }
        public string UrlApiExportExcel { get; set; }
        public string UrlApiImportExcel { get; set; }
        public string InfoWindowId { get; set; }
        public ICollection<Tab> Tabs { get; set; }
        public ICollection<ImportExcelTemplate> ImportExcelTemplates { get; set; }
        public ICollection<ExportExcelTemplate> ExportExcelTemplates { get; set; }        
        public ICollection<VoucherTemplate> VoucherTemplates { get; set; }
        public ICollection<Button> Buttons { get; set; }
        public ICollection<RegisterEvent> RegisterEvents { get; set; }

        public void SetId(string id)
        {
            this.Id = id;
        }
    }
}
