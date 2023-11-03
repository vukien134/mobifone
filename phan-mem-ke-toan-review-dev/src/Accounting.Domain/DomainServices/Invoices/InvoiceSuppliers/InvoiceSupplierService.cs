using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Invoices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Invoices.InvoiceSuppliers
{
    public class InvoiceSupplierService : BaseDomainService<InvoiceSupplier, string>
    {
        private readonly WebHelper _webHelper;
        public InvoiceSupplierService(IRepository<InvoiceSupplier, string> repository,
                                      WebHelper webHelper)
            : base(repository)
        {
            _webHelper = webHelper;
        }
        public async Task<bool> IsExistCode(InvoiceSupplier entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(InvoiceSupplier entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.InvoiceSupplier, ErrorCode.Duplicate),
                        $"InvoiceSupplier Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<InvoiceSupplier> GetSupplierActive()
        {
            var queryable = await this.GetQueryableAsync();
            var suppliers = await queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                             && p.Active == "C").ToListAsync();
            if (suppliers.Count == 0)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.InvoiceSupplier, ErrorCode.Other),
                        $"Quý khách chưa thực hiện khai báo/kích hoạt Nhà cung cấp hóa đơn điện tử.\n Vui lòng kiểm tra lại!");
            }
            if (suppliers.Count > 1)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.InvoiceSupplier, ErrorCode.Other),
                        $"Có quá nhiều nhà cung cấp hóa đơn điện tử được kích hoạt");
            }

            return suppliers.First();
        }
    }
}