using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using AutomationAPI.Common;
using AutomationAPI.Domain.DataAccess;
using AutomationAPI.Domain.DataAccess.Impl;
using AutomationAPI.Domain.Models;
using ClosedXML.Excel;

namespace AutomationAPI.Service.Impl
{
    public class WipMovementOldService : IWipMovementOldService
    {
        IWipDataDAL _wipDataDal = new WipDataDAL();
        IWMProcessDAL _processDal = new WMProcessDAL();
        public List<WIPFileInfo> GetFileName()
        {
            List<WIPFileInfo> list = new List<WIPFileInfo>();
            string path = HttpRuntime.AppDomainAppPath.ToString() + @"OldWIP\";
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo f in dir.GetFiles("*.xlsx"))
            {
                list.Add(new WIPFileInfo
                {
                    FileName = f.ToString(),
                    FileNameHex = Convertor.ToHex(f.ToString()),
                    LastWriteTime = f.LastWriteTime,
                    CreationTime = f.CreationTime,
                });
            }
            return list;
        }

        public void DuelWipData(string fileName)
        {
            using (XLWorkbook wb = new XLWorkbook(fileName))
            {
                foreach (var sht in wb.Worksheets)
                {
                    var todayCell = sht.Row(3).AsRange().FindColumn(x => x.FirstCell().Value.ToString() == DateTime.Now.Date.ToString());
                    if (todayCell is null)
                        continue;

                    var lotsize = _wipDataDal.GetMPNLotSize(sht.Cell(1, 4).GetString());
                    if (lotsize.Rows.Count > 0)
                        sht.Cell("D2").FormulaA1 = string.Format("={0}*{1}", lotsize.Rows[0][0].ToString(), lotsize.Rows[0][1].ToString());

                    string mpnStr = sht.Cell(1, 6).GetString();
                    if (mpnStr == "")
                        mpnStr = sht.Cell(1, 4).GetString();

                    var list = GetLTData(mpnStr);
                    if (list.Count == 0)
                        continue;
                    int maxLT = list.Max(x => x.LT);
                    int minLT = list.Where(x => x.LT > 0).Min(x => x.LT);

                    //process
                    var cell = todayCell.FirstCell().CellLeft();
                    int col = cell.Address.ColumnNumber;
                    foreach (var r in sht.RowsUsed())
                    {
                        if (!r.Cell(6).GetString().StartsWith("1)") && !r.Cell(6).GetString().StartsWith("7)"))
                            continue;
                        var rowType = r.Cell(6).GetString().Substring(0, 1);
                        if (int.TryParse(Regex.Replace(r.Cell(2).GetString(), @"[^0-9]+", ""), out int cellLT))
                        {
                            var qty = list.Where(x => x.LT == maxLT + minLT - cellLT)
                                .Select(x => rowType == "1" ? x.Onhold : x.Onprocess).FirstOrDefault();
                            if (qty > 0)
                                r.Cell(col).SetValue(qty);
                        }
                    }
                }

                wb.Save();
            }
            GC.Collect();
        }
        public string Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            else
                return "File not exists";
            return "";
        }
        private List<LTData> GetLTData(string mpnStr)
        {
            var list = new List<LTData>();
            foreach (var m in mpnStr.Split('/'))
            {
                string mpn = m;
                if (m.Length > 6)
                    mpn = m.Substring(0, 6);

                var wipData = _wipDataDal.GetList(mpn);
                Hashtable ht1 = new Hashtable();
                ht1.Add("MPN", mpn.Length > 9 ? mpn.Substring(0, 9) : mpn);
                ht1.Add("HMCD", mpn.Length > 11 ? mpn.Substring(0,11) : mpn);
                var processListAll = _processDal.GetList(ht1);
                var headerList = processListAll.Where(x => x.IsEnd == 1 || x.IsEnd == null).Select(x => x.RootPath.Split('/')[0]).Distinct().OrderByDescending(x => x).ToArray();
                var processList = processListAll.Where(x => x.RootPath.Split('/')[0] == headerList[0]).ToList();
                processList.ForEach(ps => {
                    var wipList = wipData.Where(x => x.Level == ps.Level && (
                                        x.ProcessCode == ps.ProcessCode && x.ProcessNo == ps.ProcessNo //一般情况，对应最新品目的工程
                                    )).ToList();
                    list.Add(new LTData
                    {
                        LT = ps.LT,
                        Onhold = wipList.Sum(x => x.Onhold),
                        Onprocess = wipList.Sum(x => x.Quantity),
                    });
                });

                var oldWipList = processListAll.Join(wipData.Where(wip => !processList.Any(ps => ps.Level == wip.Level
                                            && wip.ProcessCode == ps.ProcessCode && wip.ProcessNo == ps.ProcessNo
                                    )),
                                    ps => new { ps.ItemCode, ps.ProcessNo, ps.ProcessCode },
                                    wip => new { ItemCode = wip.FullItemCode, wip.ProcessNo, wip.ProcessCode },
                                    (ps, wip) => new { ps, wip }).Where(x => x.ps.LT > 0).ToList();
                oldWipList.ForEach(owip =>
                {
                    var tempList = list.Where(l => l.LT <= owip.ps.LT).OrderByDescending(x => x.LT).FirstOrDefault();
                    tempList.Onprocess = tempList.Onprocess + owip.wip.Quantity;
                    tempList.Onhold = tempList.Onhold + owip.wip.Onhold;
                });
            }

            return list.GroupBy(x => x.LT).Select(g => new LTData
            {
                LT = g.Key,
                Onhold = g.Sum(x => x.Onhold),
                Onprocess = g.Sum(x => x.Onprocess),
            }).ToList();
        }
        struct LTData
        {
            public int LT { get; set; }
            public int Onhold { get; set; }
            public int Onprocess { get; set; }
        }
    }
}