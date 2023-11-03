using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class Tab : AuditedEntity<string>
    {
        public int Ord { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TabTable { get; set; }
        public string TabType { get; set; }
        public string TabView { get; set; }
        public string UrlApiCrud { get; set; }
        public string UrlApiData { get; set; }
        public string UrlApiDetail { get; set; }
        public string UrlApiTabDetail { get; set; }
        public string OrderBy { get; set; }
        public string WindowId { get; set; }
        public Window Window { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }
        public bool? HasQuickSearch { get; set; }        
        public ICollection<Field> Fields { get; set; }
        public ICollection<RegisterEvent> RegisterEvents { get; set; }
        public void SetId(string id)
        {
            this.Id = id;
        }
    }
}
