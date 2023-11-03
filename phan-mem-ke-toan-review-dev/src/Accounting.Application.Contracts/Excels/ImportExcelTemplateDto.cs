using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Excels
{
    public class ImportExcelTemplateDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string WindowId { get; set; }
        public string UrlApi { get; set; }
        public int? RowBegin { get; set; }
        public string HelpContent { get; set; }
        public ICollection<ImportExcelTemplateColumnDto> ImportExcelTemplateColumns { get; set; }
    }
}
