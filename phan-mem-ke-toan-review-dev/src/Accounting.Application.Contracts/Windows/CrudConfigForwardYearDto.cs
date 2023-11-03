using System;
using Accounting.BaseDtos;

namespace Accounting.Windows
{
    public class CrudConfigForwardYearDto : CruOrgBaseDto
    {
        public int? Ord { get; set; }
        public string TableName { get; set; }
        public string FieldNot { get; set; }
        public string FieldValues { get; set; }
        public string Title { get; set; }
        public int SelectRow { get; set; }
        public int BusinessType { get; set; }

    }
}

