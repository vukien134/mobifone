using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Excels
{
    public interface IExcelAppService
    {
        Task ImportFile([FromForm] IFormFile file,ExcelRequestDto dto);
        Task<ImportExcelTemplateDto> GetImportExcelTemplateByWindowId(string windowId);
    }
}
