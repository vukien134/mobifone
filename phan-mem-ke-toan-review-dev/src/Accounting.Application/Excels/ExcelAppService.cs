using Accounting.DomainServices.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Windows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Excels
{
    public class ExcelAppService : AccountingAppService, IExcelAppService
    {
        #region Fields
        private readonly ExcelService _excelService;
        #endregion
        #region Ctor
        public ExcelAppService(ExcelService excelService)
        {
            _excelService = excelService;
        }
        #endregion
        public Task ImportFile([FromForm] IFormFile file, ExcelRequestDto dto)
        {
            return Task.CompletedTask;
        }
        public async Task<ImportExcelTemplateDto> GetImportExcelTemplateByWindowId(string windowId)
        {
            var importExcelTemplate = await _excelService.GetImportExcelTemplateByWindowIdAsync(windowId);
            if (importExcelTemplate == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ImportExcelTemplate, ErrorCode.NotFoundEntity),
                       $"Excel Template With WindowId = '{windowId}' not found");
            }
            var dto = ObjectMapper.Map<ImportExcelTemplate, ImportExcelTemplateDto>(importExcelTemplate);
            dto.ImportExcelTemplateColumns = dto.ImportExcelTemplateColumns
                                                .OrderBy(p => p.Ord).ToList();
                                                    
            return dto;
        }
    }
}
