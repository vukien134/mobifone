using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Accounting.Excels
{
    public class XlsxWorkbook : IDisposable
    {
        #region Fields
        private XSSFWorkbook _xlsWorkbook;
        private ISheet _sheet;
        #endregion
        public XlsxWorkbook(Stream stream)
        {
            _xlsWorkbook = new XSSFWorkbook(stream);
        }
        public List<T> ToList<T>()
        {
            List<T> result = new List<T>();
            int lastRow = _xlsWorkbook.GetSheetAt(0).LastRowNum + 1;
            for (int i = 1; i <= lastRow; i++)
            {
               
            }
            return result;
        }
        public object GetCellValueFromExcel(string cellName)
        {
            object result = null;

            ISheet sheet = GetFirstSheet();

            CellReference cellReference = new CellReference(cellName);
            IRow row = sheet.GetRow(cellReference.Row);

            if (row == null) return null;

            ICell cell = row.GetCell(cellReference.Col);

            if (cell != null)
            {
                switch (cell.CellType)
                {

                    case CellType.Boolean:
                        result = cell.BooleanCellValue;
                        break;
                    case CellType.Numeric:

                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            result = cell.DateCellValue;
                        }
                        else
                        {
                            result = cell.NumericCellValue;
                        }

                        break;
                    case CellType.String:
                        result = cell.StringCellValue;
                        break;
                }
            }
            return result;
        }
        public ISheet GetFirstSheet()
        {
            if (_sheet == null)
            {
                _sheet = _xlsWorkbook.GetSheetAt(0);
            }

            return _sheet;
        }
        public void Dispose()
        {
            if (_xlsWorkbook != null)
            {
                _xlsWorkbook.Close();
                _xlsWorkbook = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
