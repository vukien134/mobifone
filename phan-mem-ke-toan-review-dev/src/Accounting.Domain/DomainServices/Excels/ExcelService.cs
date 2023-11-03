using Accounting.Excels;
using Accounting.Helpers;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.Excels
{
    public class ExcelService : DomainService
    {
        #region Fields
        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<ImportExcelTemplate, string> _importExcelTemplateRepository;
        #endregion
        #region Ctor
        public ExcelService(IServiceProvider serviceProvider,
                    IRepository<ImportExcelTemplate, string> importExcelTemplateRepository)
        {
            _serviceProvider = serviceProvider;
            _importExcelTemplateRepository = importExcelTemplateRepository;
        }
        #endregion
        public async Task<List<T>> ImportFileToList<T>(byte[] bytes, string windowId)
            where T : class
        {
            var importExcelTemplate = await GetImportExcelTemplateByWindowIdAsync(windowId);
            importExcelTemplate.ImportExcelTemplateColumns = importExcelTemplate.ImportExcelTemplateColumns.OrderBy(p => p.ExcelCol).ToList();
            var result = new List<T>();
            using var ms = new MemoryStream(bytes);
            var xlsx = new XlsxWorkbook(ms);
            int lastRow = xlsx.GetFirstSheet().LastRowNum + 1; 
            int beginRow = importExcelTemplate.RowBegin.Value;
            for(int i = beginRow; i <= lastRow; i++)
            {
                var instance = ObjectHelper.CreateInstance<T>();
                foreach (var col in importExcelTemplate.ImportExcelTemplateColumns)
                {
                    var value = xlsx.GetCellValueFromExcel($"{col.ExcelCol}{i}");
                    ObjectHelper.SetProperty(instance, col.FieldName, value);
                }
                result.Add(instance);
            }
            
            return result;
        }
        #region Private
        public async Task<ImportExcelTemplate> GetImportExcelTemplateByWindowIdAsync(string windowId)
        {
            var queryable = await _importExcelTemplateRepository
                                    .WithDetailsAsync(p => p.ImportExcelTemplateColumns);
            queryable = queryable.Where(p => p.WindowId == windowId);
            return queryable.FirstOrDefault();
        }
        #endregion
    }
}
