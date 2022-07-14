using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace AutomationAPI.Common
{
    public class ExcelHelper
    {
        private IWorkbook _workBook = null;
        private int _sheetCount = 0;
        public ExcelHelper()
        {
            _workBook = new XSSFWorkbook();
        }
        public void Dispose()
        {
            _workBook.Close();
            _workBook = null;
            _sheetCount = 0;
            GC.Collect();
        }

        public ExcelHelper(string dirPath)
        {
            using (FileStream fs = new FileStream(dirPath, FileMode.Open, FileAccess.Read))
            {
                _workBook = new XSSFWorkbook(fs);
                _sheetCount = ((XSSFWorkbook)_workBook).Count;
            }
        }
        public ExcelHelper(Stream fs)
        {
            _workBook = new XSSFWorkbook(fs);
            _sheetCount = _workBook.NumberOfSheets;
        }
        /// <summary>
        /// 客户端下载
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="dirPath">服务器端下载目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="startRow">数据起始行</param>
        public MemoryStream CreateExcelWithTemplate(DataTable data, int startRow)
        {
            var sheet = _workBook.GetSheetAt(0);

            var currRowIndex = startRow;
            foreach (DataRow dr in data.Rows)
            {
                if (currRowIndex > startRow)
                    sheet.CopyRow(currRowIndex - 1, currRowIndex);
                var row = sheet.GetRow(currRowIndex);
                if (row == null)
                    row = sheet.CreateRow(currRowIndex);
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    var cell = row.GetCell(i);
                    if (cell == null)
                    {
                        cell = row.CreateCell(i);
                    }
                    cell.CellFormula = null;
                    if (dr[i].GetType().Name == "String" && dr[i].ToString().StartsWith("=/*")) //特殊处理 同一列既有公式又有数字的情况
                        cell.SetCellValue(double.Parse(dr[i].ToString().Substring(3)));
                    else if (dr[i].GetType().Name == "String" && dr[i].ToString().StartsWith("="))
                        cell.CellFormula = dr[i].ToString().Substring(1);
                    else if (dr[i].GetType().Name == "String" || dr[i].GetType().Name == "DBNull")
                        cell.SetCellValue(dr[i].ToString());
                    else if (dr[i].GetType().Name == "DateTime")
                        cell.SetCellValue(System.DateTime.Parse(dr[i].ToString()));
                    else
                        cell.SetCellValue(double.Parse(dr[i].ToString()));
                }
                currRowIndex++;
            }
            sheet.ForceFormulaRecalculation = true;

            MemoryStream ms = new MemoryStream();
            _workBook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            _workBook.Close();
            return ms;
        }
        /// <summary>
        /// 客户端下载
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="dirPath">服务器端下载目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="startRow">数据起始行</param>
        public MemoryStream CreateExcelWithTemplate(DataSet ds, int startRow)
        {
            if (ds.Tables.Count == 0)
                return null;

            var sheet = _workBook.GetSheetAt(0);
            int tableCount = 0;
            foreach (DataTable data in ds.Tables)
            {
                tableCount++;
                sheet = _workBook.GetSheetAt(0).CopySheet("Sheets"+ tableCount);
                var currRowIndex = startRow;

                foreach (DataRow dr in data.Rows)
                {
                    if (currRowIndex > startRow)
                        sheet.CopyRow(currRowIndex - 1, currRowIndex);
                    var row = sheet.GetRow(currRowIndex);
                    if (row == null)
                        row = sheet.CreateRow(currRowIndex);
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        var cell = row.GetCell(i);
                        if (cell == null)
                        {
                            cell = row.CreateCell(i);
                        }
                        cell.CellFormula = null;
                        if (dr[i].GetType().Name == "String" && dr[i].ToString().StartsWith("=/*")) //特殊处理 同一列既有公式又有数字的情况
                            cell.SetCellValue(double.Parse(dr[i].ToString().Substring(3)));
                        else if (dr[i].GetType().Name == "String" && dr[i].ToString().StartsWith("="))
                            cell.CellFormula = dr[i].ToString().Substring(1);
                        else if (dr[i].GetType().Name == "String" || dr[i].GetType().Name == "DBNull")
                            cell.SetCellValue(dr[i].ToString());
                        else if (dr[i].GetType().Name == "DateTime")
                            cell.SetCellValue(System.DateTime.Parse(dr[i].ToString()));
                        else
                            cell.SetCellValue(double.Parse(dr[i].ToString()));
                    }
                    currRowIndex++;
                }
                sheet.ForceFormulaRecalculation = true;
            }
            if (_workBook.NumberOfSheets > 1)
                _workBook.RemoveSheetAt(0);
            MemoryStream ms = new MemoryStream();
            _workBook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            _workBook.Close();
            return ms;
        }
        public bool CompareTitle(int sheetNo, string templatePath)
        {
            ISheet sheetSrc = null;
            if (sheetNo < _sheetCount)
                sheetSrc = _workBook.GetSheetAt(sheetNo);
            XSSFWorkbook workbook;
            using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fs);
                var sheetDst = workbook.GetSheetAt(0);

                for (int i = 0; sheetDst.GetRow(i) != null; i++)
                {
                    if (sheetSrc.GetRow(i) == null) 
                        return false;
                    for (int j = 0; sheetDst.GetRow(i).GetCell(j) != null; j++)
                    {

                        if (sheetSrc.GetRow(i).GetCell(j) == null) 
                            return false;
                        if (sheetDst.GetRow(i).GetCell(j).StringCellValue != sheetSrc.GetRow(i).GetCell(j).StringCellValue)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(int sheetNo, int startRow, int titleRow)
        {
            DataTable data = new DataTable();

            ISheet sheet = null;
            if (sheetNo < _sheetCount)
                sheet = _workBook.GetSheetAt(sheetNo);

            if (sheet != null)
            {
                IRow firstRow = sheet.GetRow(titleRow);
                int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (cell != null)
                    {
                        string cellValue = GetStringValue(cell);
                        if (cellValue != null)
                        {
                            data.Columns.Add(new DataColumn());
                        }
                    }
                }

                //最后一列的标号
                int rowCount = sheet.LastRowNum;
                ICell tempcell = null;
                for (int i = startRow; i <= rowCount; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue; //没有数据的行默认是null　　　　　　　

                    DataRow dataRow = data.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        if (j < 0)
                            break;
                        tempcell = row.GetCell(j);
                        if (tempcell != null)
                            try
                            {
                                dataRow[j] = GetStringValue(tempcell);
                            }
                            catch(Exception e)
                            {
                                dataRow[j] = GetStringValue(tempcell);
                            }

                    }
                    data.Rows.Add(dataRow);
                }
            }
            return data;
        }

        public int GetSheetCount()
        {
            return _sheetCount;
        }

        public string GetSheetName(int sheetNo)
        {
            return _workBook.GetSheetAt(sheetNo).SheetName;
        }

        private string GetStringValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        try
                        {
                            return cell.DateCellValue.ToString();
                        }
                        catch (Exception e)
                        {
                        }
                        try
                        {
                            return DateTime.FromOADate(cell.NumericCellValue).ToString();
                        }
                        catch (Exception e)
                        {
                            return cell.NumericCellValue.ToString();
                        }
                    }
                    return cell.NumericCellValue.ToString();

                case CellType.String:
                    return cell.StringCellValue;

                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();

                case CellType.Formula:
                    if (cell.CachedFormulaResultType == CellType.Numeric)
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            try
                            {
                                return cell.DateCellValue.ToString();
                            }
                            catch (Exception e)
                            {
                            }
                            try
                            {
                                return DateTime.FromOADate(cell.NumericCellValue).ToString();
                            }
                            catch (Exception e)
                            {
                                return cell.NumericCellValue.ToString();
                            }
                        }
                        else
                            return cell.NumericCellValue.ToString();
                    else
                        return cell.StringCellValue;
                default:
                    return string.Empty;
            }
        }

    }
}
