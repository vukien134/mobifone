using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Accounting.Windows
{
    public class RegisterEvent : AuditedEntity<string>
    {
        public string WindowId { get; set; }
        public string TabId { get; set; }
        public string FieldId { get; set; }
        public string EventSettingId { get; set; }
        public EventSetting EventSetting { get; set; }
        public Window Window { get; set; }
        public Tab Tab { get; set; }
        public Field Field { get; set; }
        public void SetId(string id)
        {
            this.Id = id;
        }
    }
}
