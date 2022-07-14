using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using AutomationAPI.Common;
using AutomationAPI.Domain.DataAccess;
using AutomationAPI.Domain.DataAccess.Impl;
using AutomationAPI.Domain.Models;

namespace AutomationAPI.Service.Impl
{
    public class WipMovementService : BaseService, IWipMovementService
    {
        IWipMovementDAL _dal = new WipMovementDAL();
        IWMProcessService _process = new WMProcessService();
        IWMProjectService _project = new WMProjectService();
        IWMIODataService _ioData = new WMIODataService();
        IWipDataService _wip = new WipDataService();
        public DataTable GetMPN()
        {
            return _dal.GetMPN();
        }

        public DataTable GetFinishGoods()
        {
            return _dal.GetFinishGoods();
        }
        public List<string> GetProject()
        {
            return _project.GetList(new Hashtable()).Select(x=>x.CustProject).Distinct().ToList();
        }
        public string Upload(HttpPostedFile file)
        {
            bool isUpload = false;
            using (Stream ulfile = new MemoryStream())
            {
                file.InputStream.CopyTo(ulfile);
                ulfile.Position = 0; //EOF in header
                bool fileSave = false;
                ExcelHelper excel = new ExcelHelper(ulfile);
                for (int k = 0; k < excel.GetSheetCount(); k++)
                {
                    int fileType = 0;
                    if (excel.CompareTitle(k, HttpContext.Current.Server.MapPath("~/Templates/WMProcess.xlsx")))
                        fileType = 1;
                    else if (excel.CompareTitle(k, HttpContext.Current.Server.MapPath("~/Templates/WMProject.xlsx")))
                        fileType = 2;
                    else if (excel.GetSheetName(k).ToString().StartsWith("Daily IO"))
                    {
                        fileType = 3;
                        fileSave = true;
                    }

                    if (fileType == 0)
                        continue;
                    isUpload = true;

                    switch (fileType)
                    {
                        case 1:
                            var dt = excel.ExcelToDataTable(k, 1, 0);
                            if (dt.Rows.Count == 0)
                                return "没有数据！";
                            List<WMProcess> list1 = new List<WMProcess>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i][0].ToString().Trim() == "")
                                    continue;
                                try
                                {
                                    list1.Add(new WMProcess
                                    {
                                        CreateBy = "Upload",
                                        CreateDate = DateTime.Now,
                                        MPN = dt.Rows[i][0].ToString().Trim(),
                                        ItemCode = dt.Rows[i][1].ToString().Trim(),
                                        Level = Convertor.ToInt(dt.Rows[i][2]),
                                        KeyProcess = dt.Rows[i][3].ToString().Trim().Length > 0 ? "Y" : "",
                                        LT = Convertor.ToInt(dt.Rows[i][4]),
                                        ProcessNo = Convertor.ToInt(dt.Rows[i][5]),
                                        ProcessCode = dt.Rows[i][6].ToString().Trim(),
                                        Description = dt.Rows[i][7].ToString().Trim(),
                                        ItemType = dt.Rows[i][8].ToString().Trim(),
                                        RootPath = dt.Rows[i][9].ToString().Trim(),
                                    });
                                }
                                catch (Exception e)
                                {
                                    return "Row:" + (i + 1).ToString() + e;
                                }
                            }
                            if (list1.Any(x => x.LT >0) && list1.Any(x => x.KeyProcess.Length > 0))
                                _process.Import(list1);
                            break;
                        case 2:
                            dt = excel.ExcelToDataTable(k, 1, 0);
                            if (dt.Rows.Count == 0)
                                return "没有数据！";
                            List<WMProject> list2 = new List<WMProject>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i][2].ToString() + dt.Rows[i][3].ToString() == "")
                                    continue;
                                try
                                {
                                    list2.Add(new WMProject
                                    {
                                        CreateBy = "Upload",
                                        CreateDate = DateTime.Now,
                                        CustProject = dt.Rows[i][0].ToString().Trim(),
                                        InterProject = dt.Rows[i][1].ToString().Trim(),
                                        FPCItem = dt.Rows[i][2].ToString().Trim(),
                                        SMTItem = dt.Rows[i][3].ToString().Trim(),
                                        Customer = dt.Rows[i][4].ToString().Trim(),
                                        Color = dt.Rows[i][5].ToString().Trim(),
                                    });
                                    list2[list2.Count - 1].MPN = list2[list2.Count - 1].SMTItem == "" ? list2[list2.Count - 1].FPCItem : list2[list2.Count - 1].SMTItem;
                                }
                                catch (Exception e)
                                {
                                    return "Row:" + (i + 1).ToString() + e;
                                }
                            }
                            _project.Import(list2);
                            break;
                        case 3:
                            dt = excel.ExcelToDataTable(k, 3, 3);
                            if (dt.Rows.Count == 0)
                                return "没有数据！";

                            List<WMIOData> list3 = new List<WMIOData>();
                            List<string> listItem = new List<string>();

                            int curIdx = 0;
                            int curRow = 0;
                            DateTime curDate = DateTime.Now;
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                for (int i = 1; i < dt.Columns.Count; i++)
                                {
                                    if (!DateTime.TryParse(dt.Rows[j][i].ToString(), out curDate))
                                        continue;

                                    curIdx = i;
                                    curRow = j;
                                    break;
                                }
                                if (curIdx > 0)
                                    break;
                            }

                            for (int i = 1; i < dt.Rows.Count; i++)
                            {
                                var item = dt.Rows[i][5].ToString().Trim().ToLower();
                                var project = dt.Rows[i][3].ToString().Trim().ToLower() + dt.Rows[i][4].ToString().Trim().ToLower();
                                if ((item.EndsWith("output") || item.EndsWith("yield")) && !item.StartsWith("cum"))
                                    listItem.Add(project + item);
                                else
                                    continue;

                                //列转行
                                for (int j = curIdx; j < dt.Columns.Count; j++)
                                {
                                    try
                                    {
                                        list3.Add(new WMIOData
                                        {
                                            CreateBy = "Upload",
                                            CreateDate = DateTime.Now,
                                            Project = dt.Rows[i][3].ToString().Trim(),
                                            FlexName = dt.Rows[i][4].ToString().Trim(),
                                            Item = dt.Rows[i][5].ToString().Trim(),
                                            DRI = dt.Rows[i][7].ToString().Trim(),
                                            PlanDate = DateTime.Parse(dt.Rows[curRow][j].ToString()),
                                            Quantity = Convertor.ToDecimal(dt.Rows[i][j]),
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        return "Row:" + (i + 1).ToString() + e;
                                    }
                                }
                            }
                            _ioData.Import(list3);
                            break;
                    }
                }

                if (fileSave)
                {
                    string path = HttpRuntime.AppDomainAppPath.ToString() + @"OldWIP\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (File.Exists(path + file.FileName))
                        File.Delete(path + file.FileName);
                    file.SaveAs(path + file.FileName);
                }
                excel.Dispose();
            }
            if(!isUpload)
                return "非正确上传文件，请参考模版文件";
            return "";
        }

        public DataTable WMProjectDL()
        {
            var list = _project.GetList(new Hashtable());
            if (list == null || list.Count == 0)
                return null;

            string fileName = "Project_" + DateTime.Now.ToString("MMddHHmmss") + ".xlsx";
            var dt = Convertor.ToDataTable(list.Select(x => new
            {
                x.CustProject,
                x.InterProject,
                x.FPCItem,
                x.SMTItem,
                x.Customer,
                x.Color,
            }).ToList());
            return dt;
        }

        public List<WMProcess> GetWMProcess(string mpn)
        {
            Hashtable ht = new Hashtable();
            ht.Add("MPN", mpn.Length > 9 ? mpn.Substring(0, 9) : mpn);
            ht.Add("HMCD", mpn.Length > 11 ? mpn.Substring(0, 11) : mpn);
            return _process.GetList(ht);
        }

        public DataSet WMProcessDL(string mpn, string template)
        {
            Hashtable ht = new Hashtable();
            ht.Add("MPN", mpn.Length > 9 ? mpn.Substring(0, 9) : mpn);
            ht.Add("HMCD", mpn.Length > 11 ? mpn.Substring(0, 11) : mpn);
            var list = template == null ? _process.GetList(ht) : _process.GetTemplateList(ht);
            if (list == null || list.Count == 0)
                return null;

            var headerList = list.Where(x => x.IsEnd == 1 || x.IsEnd == null).Select(x => x.RootPath.Split('/')[0]).Distinct().OrderByDescending(x => x);
            DataSet ds = new DataSet();
            foreach (var hd in headerList)
            {
                var dt = Convertor.ToDataTable(list.Where(x => hd.Contains(x.RootPath.Split('/')[0])).Select(x => new
                {
                    x.MPN,
                    x.ItemCode,
                    x.Level,
                    x.KeyProcess,
                    x.LT,
                    x.ProcessNo,
                    x.ProcessCode,
                    x.Description,
                    x.ItemType,
                    hd = hd + "/" + (x.RootPath.Split('/').Length > 1 ? x.RootPath.Split('/')[1] : "")
                }).ToList().
                OrderByDescending(x => x.Level.ToString().Substring(0, 1)).
                ThenBy(x => x.hd.Split('/').Length > 1 ? x.hd.Split('/')[1] : "").
                ThenBy(x => x.ProcessNo));

                dt.TableName = hd;
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataTable WMProjectDL(string project, string flexname)
        {
            var list = _ioData.GetList(null);
            if (list == null || list.Count == 0)
                return null;
            var dt = Convertor.ToDataTable(list.Select(x => new
            {
            }).ToList());
            return dt;
        }
        public DataTable GetWIPMovement(string mpn, string debug)
        {
            //根据品目获取工顺
            Hashtable ht1 = new Hashtable();
            ht1.Add("MPN", mpn.Length > 9 ? mpn.Substring(0, 9) : mpn);
            ht1.Add("HMCD", mpn.Length > 11 ? mpn.Substring(0, 11) : mpn);
            var processListAll = _process.GetList(ht1);
            if (processListAll.Count() == 0)
                return null;
            int maxLT = processListAll.Max(x => x.LT);

            //根据smt品目获取Project
            Hashtable ht2 = new Hashtable();
            ht2.Add("MPN", mpn);
            var pr = _project.GetList(ht2).FirstOrDefault() ?? new WMProject();

            CalendarHelper _cal = new CalendarHelper(DateTime.Now.Year, DateTime.Now.Year + 3);
            //获取PlanOut
            Hashtable ht3 = new Hashtable();
            ht3.Add("Project", pr.CustProject);
            ht3.Add("PlanDateFrom", DateTime.Now.Date);
            ht3.Add("PlanDateTo", _cal.GetWorkDate(maxLT));
            var ioList = _ioData.GetList(ht3) ?? new List<WMIOData>();

            //获取WIP
            var wipList = _wip.GetList(mpn).Where(x => x.ProcessNo != 0).ToList();

            //部品仓库存
            var innerOnhand = _wip.GetInnerOnhand(mpn);

            //成品仓库存
            var fgOnhand = _wip.GetFGOnhand(mpn);

            //calendar
            List<List<WipMovement>> listAll = new List<List<WipMovement>>();
            var headerList = processListAll.Where(x => x.IsEnd == 1 || x.IsEnd == null).Select(x => x.RootPath.Split('/')[0]).Distinct().OrderByDescending(x => x).ToArray();

            List<WipMovement> list = new List<WipMovement>();
            int rowNo = 1;
            int preLT = 0;
            int wippcs = 0;
            int onhold = 0;
            int idxInner = 0;
            var today = _cal.GetWorkDate(0);
            int koto = wipList.Select(x => x.Koto).FirstOrDefault();
            var processList = processListAll.Where(x => x.RootPath.Split('/')[0] == headerList[0]);
            DateTime preDay = (DateTime)ht3["PlanDateTo"];
            processList.OrderByDescending(x => x.LT).ThenBy(x => x.Id).ToList().ForEach(ps =>
            {
                var processWip = wipList.Where(x => x.Level == ps.Level && (
                                        x.ProcessCode == ps.ProcessCode && x.ProcessNo == ps.ProcessNo //一般情况，对应最新品目的工程
                                    ));
                wippcs = processWip.Sum(x => x.Quantity);
                onhold = processWip.Sum(x => x.Onhold);
                if (list.Count > 0 && ps.KeyProcess != "Y" && debug != "Y")
                {
                    list[list.Count - 1].WIPPCS = list[list.Count - 1].WIPPCS + wippcs;
                    list[list.Count - 1].WIPPNL = koto == 0 ? 0 : (int)Math.Ceiling(list[list.Count - 1].WIPPCS * 1.0 / koto);
                    list[list.Count - 1].OnholdQty = list[list.Count - 1].OnholdQty + onhold;
                }
                if (ps.KeyProcess != "Y" && debug != "Y")
                    return;
                rowNo++;//相当于excel的行号 为了使用公式
                var vm = new WipMovement();
                vm.Project = pr.CustProject;
                vm.Process = ps.Description;
                vm.LeadTime = ps.LT;
                vm.OutputDate = _cal.GetWorkDate(ps.LT);

                var yield = ioList.Where(x => x.Item.Equals(pr.Customer + " yield", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == today).Select(y => y.Quantity).FirstOrDefault();
                if (yield == 0)
                    yield = ioList.Where(x => x.Item.Equals(ps.ItemType + " yield", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == today).Select(y => y.Quantity).FirstOrDefault();
                vm.Yield = yield == 0 ? 1 : yield;

                vm.WIPPCS = wippcs;
                wippcs = 0;

                vm.WIPPNL = koto == 0 ? 0 : (int)Math.Ceiling(vm.WIPPCS * 1.0 / koto);

                vm.OnholdQty = onhold;
                onhold = 0;


                if (preLT == ps.LT)
                    vm.PlanOutput = 0;
                else
                {
                    decimal planOut = 0;
                    if (ps.ItemType.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase)))
                        planOut = ioList.Where(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    else if (ps.ItemType.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase)))
                        planOut = ioList.Where(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    else if (ps.ItemType.ToUpper() == "FPC" && ioList.Exists(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase)))
                        planOut = ioList.Where(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    vm.PlanOutput = (int)Math.Ceiling(planOut);
                    preLT = ps.LT;
                    preDay = vm.OutputDate;
                }

                if (ps.ItemType.ToUpper() == "SMT" && idxInner == 0)
                {
                    idxInner = list.Count();
                    var vmt = new WipMovement();
                    vmt.Project = vm.Project;
                    vmt.Process = "FPC-EOH";
                    vmt.LeadTime = vm.LeadTime;
                    vmt.Yield = vm.Yield;
                    vmt.OutputDate = vm.OutputDate;
                    vmt.WIPPCS = innerOnhand;
                    vmt.WIPPNL = koto == 0 ? 0 : (int)Math.Ceiling(vmt.WIPPCS * 1.0 / koto);

                    vmt.WIPOutputYield = $"=G{rowNo}*D{rowNo}";
                    vmt.WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
                    vmt.PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
                    vmt.CumDelta = $"=J{rowNo}-L{rowNo}";
                    vmt.WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";

                    list.Add(vmt);
                    rowNo++;
                }
                vm.WIPOutputYield = $"=G{rowNo}*D{rowNo}";
                vm.WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
                vm.PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
                vm.CumDelta = $"=J{rowNo}-L{rowNo}";
                vm.WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";

                list.Add(vm);
            });
            //成品仓库存
            {
                rowNo++;
                var vm = list[list.Count - 1];
                var vmt = new WipMovement();
                vmt.Project = vm.Project;
                vmt.Process = "FG-EOH";
                vmt.LeadTime = 0;
                vmt.Yield = 1;
                vmt.OutputDate = today;
                vmt.WIPPCS = fgOnhand;
                vmt.WIPPNL = koto == 0 ? 0 : (int)Math.Ceiling(vmt.WIPPCS * 1.0 / koto);

                vmt.WIPOutputYield = $"=G{rowNo}*D{rowNo}";
                vmt.WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
                vmt.PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
                vmt.CumDelta = $"=J{rowNo}-L{rowNo}";
                vmt.WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";

                list.Add(vmt);
            }
            //休日 plan out
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (list[i].Process == "FPC-EOH" || list[i].Process == "FG-EOH")
                    continue;
                int days = (int)(list[i - 1].OutputDate - list[i].OutputDate).TotalDays;
                if (days < 2)
                    continue;
                for (int j = 1; j < days; j++)
                {
                    var vm = new WipMovement()
                    {
                        ITEMTYPE = list[i - 1].ITEMTYPE,
                        OutputDate = list[i].OutputDate.AddDays(j),
                        Project = "休日",
                        LeadTime = list[i - 1].LeadTime,
                    };
                    decimal planOut = 0;
                    bool isCal = true;
                    if (vm.ITEMTYPE?.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        isCal = false;
                        planOut += ioList.Where(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    }
                    if (isCal && vm.ITEMTYPE?.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase)))
                        planOut = ioList.Where(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    else if (isCal && vm.ITEMTYPE?.ToUpper() == "FPC" && ioList.Exists(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase)))
                        planOut = ioList.Where(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    if (planOut == 0)
                        continue;
                    vm.PlanOutput = (int)Math.Ceiling(planOut);
                    list.Insert(i, vm);
                }
            }
            //重新调整公式
            for (int i = 0; i < list.Count; i++)
            {
                //公式项
                rowNo = i + 2;
                list[i].WIPOutputYield = $"=G{rowNo}*D{rowNo}";
                list[i].WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
                list[i].PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
                list[i].CumDelta = $"=J{rowNo}-L{rowNo}";
                list[i].WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";
            }
            if (list.Count > 1)
                //LT=0 特殊处理 同一列既有公式又有数字
                list[list.Count - 1].WIPoutputCum = "=/*" + list[list.Count - 1].WIPPCS;
            //把旧改号变更工程的数量加到最靠近已加工的一道工序
            var oldWipList = processListAll.Join(wipList.Where(wip => !processList.Any(ps => ps.Level == wip.Level
                                            && wip.ProcessCode == ps.ProcessCode && wip.ProcessNo == ps.ProcessNo
                                    )),
                                    ps => new { ps.ItemCode, ps.ProcessNo, ps.ProcessCode },
                                    wip => new { ItemCode = wip.FullItemCode, wip.ProcessNo, wip.ProcessCode },
                                    (ps, wip) => new { ps, wip }).Where(x => x.ps.LT > 0).ToList();
            oldWipList.ForEach(owip =>
            {
                var tempList = list.Where(l => l.LeadTime <= owip.ps.LT).OrderByDescending(x => x.LeadTime).FirstOrDefault();
                tempList.WIPPCS = tempList.WIPPCS + owip.wip.Quantity;
                tempList.OnholdQty = tempList.OnholdQty + owip.wip.Onhold;
                tempList.WIPPNL = tempList.WIPPNL + owip.wip.Koto == 0 ? 0 : (int)Math.Ceiling(tempList.WIPPCS * 1.0 / owip.wip.Koto);
            });
            var dt = Convertor.ToDataTable(list);
            dt.Columns.Remove("Koto");
            dt.Columns.Remove("Item");
            dt.Columns.Remove("MPN");
            dt.Columns.Remove("ITEMTYPE");
            return dt; 
        }

        public DataTable GetWIPMovementByProject(string project)
        {
            Hashtable ht1 = new Hashtable();
            //根据Project取MPN
            ht1.Add("CustProject", project);
            ht1.Add("MPN", "ALL");
            var prTable = _project.GetTable(ht1);
            var prList = Convertor.TableToList<WMProject>(prTable);
            MemoryStream stream = new MemoryStream();
            DataTable dt = new DataTable("MPNList");
            var ds = new DataSet("XML");
            ds.Tables.Add(dt);
            dt.Columns.Add(new DataColumn("MPN"));
            for (int i = 0; i < prTable.Rows.Count; i++)
            {
                dt.Rows.Add(prTable.Rows[i]["MPN"]);
            }
            dt.WriteXml(stream);
            SqlXml mpnXml = new SqlXml(stream);
            var mpnStr = string.Join(",", prList.Select(x => x.MPN).ToList()) +",";
            //获取工顺
            ht1.Clear();
            ht1.Add("MPNList", mpnXml);
            var processListAll = _process.GetList(ht1).OrderByDescending(x => x.LT).ThenByDescending(x => x.ProcessNo).ToList();
            if (processListAll == null || processListAll.Count == 0)
                return null;
            //获取WIP
            ht1.Clear();
            ht1.Add("MPN", mpnStr);
            var wipList = _wip.GetList(ht1).Where(x => x.ProcessNo != 0).ToList();
            foreach (var ps in processListAll)
            {
                var tempList = wipList.Where(x => x.FullItemCode == ps.ItemCode && x.ProcessNo == ps.ProcessNo).ToList();
                foreach (var wip in tempList){
                    ps.QUANTITY += wip.Quantity;
                    ps.KOTO = wip.Koto;
                    ps.ONHOLD += wip.Onhold;
                    wipList.Remove(wip);
                }
            }
            foreach (var ps in processListAll)
            {
                var tempList = wipList.Where(x => 
                    x.FullItemCode.StartsWith(ps.ItemCode.Substring(0,6)) 
                    && x.FullItemCode.Trim().EndsWith(ps.ItemCode.Substring(11).Trim())
                    && x.ProcessNo == ps.ProcessNo).ToList();
                foreach (var wip in tempList)
                {
                    ps.QUANTITY += wip.Quantity;
                    ps.KOTO = wip.Koto;
                    ps.ONHOLD += wip.Onhold;
                    wipList.Remove(wip);
                }
            }
            foreach (var wip in wipList)
            {
                var ps = processListAll.Where(x =>
                    wip.FullItemCode.StartsWith(x.ItemCode.Substring(0, 6))
                    && wip.FullItemCode.Trim().EndsWith(x.ItemCode.Substring(11).Trim())
                    && wip.ProcessNo > x.ProcessNo).FirstOrDefault();
                if (ps == null)
                    ps = processListAll.Where(x =>
                        wip.Level >= x.Level
                        && wip.ProcessNo >= x.ProcessNo).FirstOrDefault();
                if (ps == null)
                    continue;
                ps.QUANTITY += wip.Quantity;
                ps.KOTO = wip.Koto;
                ps.ONHOLD += wip.Onhold;
            }
            processListAll.Add(new WMProcess() { ItemType = processListAll[processListAll.Count - 1].ItemType, KeyProcess = "" });

            int maxLT = processListAll.Max(x => x.LT);
            CalendarHelper _cal = new CalendarHelper(DateTime.Now.Year, DateTime.Now.Year + 3);
            //获取PlanOut
            Hashtable ht3 = new Hashtable();
            ht3.Add("Project", project);
            ht3.Add("PlanDateFrom", DateTime.Now.Date);
            ht3.Add("PlanDateTo", _cal.GetWorkDate(maxLT));
            var ioList = _ioData.GetList(ht3) ?? new List<WMIOData>();
            //
            List<WipMovement> list = new List<WipMovement>();
            var vm = new WipMovement();
            string lastKeyProcess = "";
            var today = _cal.GetWorkDate(0);
            int preLT = 0;
            int idxInner = 0;
            foreach (var ps in processListAll)
            {
                if(lastKeyProcess == "Y" && ps.KeyProcess == "")
                {
                    int rowNo = list.Count + 2;
                    vm.Project = project;
                    vm.OutputDate = _cal.GetWorkDate(vm.LeadTime);
                    if (vm.OnholdQty == 0)
                        vm.OnholdQty = null;
                    decimal yield = 0;
                    foreach(var pr in prList)
                    {
                        yield = ioList.Where(x => x.Item.Equals(pr.Customer + " yield", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == today).Select(y => y.Quantity).FirstOrDefault();
                    }
                    if (yield == 0)
                        yield = ioList.Where(x => x.Item.Equals(ps.ItemType + " yield", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == today).Select(y => y.Quantity).FirstOrDefault();
                    vm.Yield = yield == 0 ? 1 : yield;
                    vm.WIPPNL = vm.KOTO == 0 ? 0 : (int)Math.Ceiling(vm.WIPPCS * 1.0 / vm.KOTO);
                    //
                    if (preLT == vm.LeadTime)
                        vm.PlanOutput = 0;
                    else
                    {
                        decimal planOut = 0;
                        bool isCal = true;
                        foreach (var pr in prList)
                        {
                            if (vm.ITEMTYPE.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                isCal = false;
                                planOut += ioList.Where(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                            }
                        }
                        if (isCal && vm.ITEMTYPE.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase)))
                            planOut = ioList.Where(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                        else if (isCal && vm.ITEMTYPE.ToUpper() == "FPC" && ioList.Exists(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase)))
                            planOut = ioList.Where(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                        vm.PlanOutput += (int)Math.Ceiling(planOut);
                    }
                    //公式项
                    preLT = vm.LeadTime;
                    vm.WIPOutputYield = $"=G{rowNo}*D{rowNo}";
                    vm.WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
                    vm.PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
                    vm.CumDelta = $"=J{rowNo}-L{rowNo}";
                    vm.WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";
                    //
                    list.Add(vm);
                    vm = new WipMovement() { OnholdQty = 0 };
                }
                if (ps.ItemType.ToUpper() == "SMT" && idxInner == 0)
                {
                    idxInner++;
                    list.Add(addFPCOnhand(list, mpnStr, today));
                }
                vm.WIPPCS += ps.QUANTITY;
                vm.OnholdQty += ps.ONHOLD;
                vm.Process = ps.Description;
                vm.LeadTime = ps.LT;
                vm.MPN = ps.MPN;
                vm.KOTO = ps.KOTO == 0 ? vm.KOTO : ps.KOTO;
                vm.ITEMTYPE = ps.ItemType;
                lastKeyProcess = ps.KeyProcess;
            }
            list.Add(addFGOnhand(list, mpnStr, today));
            {
                list.Reverse();
                decimal yield = 0;
                foreach (var mv in list)
                {
                    if (mv.Process == "FPC-EOH")
                        mv.Yield = yield;
                    yield = mv.Yield;
                }
                list.Reverse();
            }
            //休日 plan out
            for (int i = list.Count-1; i > 0; i--)
            {
                if (list[i].Process == "FPC-EOH" || list[i].Process == "FG-EOH")
                    continue;
                int days = (int)(list[i - 1].OutputDate - list[i].OutputDate).TotalDays;
                if (days < 2)
                    continue;
                for(int j = 1; j < days; j++)
                {
                    vm = new WipMovement() {
                        ITEMTYPE = list[i - 1].ITEMTYPE,
                        OutputDate = list[i].OutputDate.AddDays(j),
                        Project = "休日",
                        LeadTime = list[i - 1].LeadTime,
                    };
                    decimal planOut = 0;
                    bool isCal = true;
                    foreach (var pr in prList)
                    {
                        if (vm.ITEMTYPE?.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            isCal = false;
                            planOut += ioList.Where(x => x.Item.Equals(pr.Customer + " output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                        }
                    }
                    if (isCal && vm.ITEMTYPE?.ToUpper() == "SMT" && ioList.Exists(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase)))
                        planOut = ioList.Where(x => x.Item.Equals("smt output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    else if (isCal && vm.ITEMTYPE?.ToUpper() == "FPC" && ioList.Exists(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase)))
                        planOut = ioList.Where(x => x.Item.Equals("fpc output", StringComparison.InvariantCultureIgnoreCase) && x.PlanDate == vm.OutputDate).Sum(y => y.Quantity);
                    if (planOut == 0)
                        continue; 
                    vm.PlanOutput = (int)Math.Ceiling(planOut);
                    list.Insert(i, vm);
                }
            }
            //重新调整公式
            for (int i = 0; i < list.Count; i++)
            {
                //公式项
                int rowNo = i + 2;
                list[i].WIPOutputYield = $"=G{rowNo}*D{rowNo}";
                list[i].WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
                list[i].PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
                list[i].CumDelta = $"=J{rowNo}-L{rowNo}";
                list[i].WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";
            }
            if (list.Count > 1)
            {
                //LT=0 特殊处理 同一列既有公式又有数字
                list[list.Count - 1].WIPoutputCum = "=/*" + list[list.Count - 1].WIPPCS;
                return Convertor.ToDataTable(list.Select(x=> new
                {
                    x.Project,
                    x.Process,
                    x.LeadTime,
                    x.Yield,
                    x.OutputDate,
                    x.WIPPNL,
                    x.WIPPCS,
                    x.OnholdQty,
                    x.WIPOutputYield,
                    x.WIPoutputCum,
                    x.PlanOutput,
                    x.PlanOutputCum,
                    x.CumDelta,
                    x.WIPOutput
                }));
            }
            return null;
        }

        public DataTable GetDetailByProject(string project)
        {
            Hashtable ht1 = new Hashtable();
            //根据Project取MPN
            ht1.Add("CustProject", project);
            ht1.Add("MPN", "ALL");
            var prTable = _project.GetTable(ht1);
            var prList = Convertor.TableToList<WMProject>(prTable);
            MemoryStream stream = new MemoryStream();
            DataTable dt = new DataTable("MPNList");
            var ds = new DataSet("XML");
            ds.Tables.Add(dt);
            dt.Columns.Add(new DataColumn("MPN"));
            for (int i = 0; i < prTable.Rows.Count; i++)
            {
                dt.Rows.Add(prTable.Rows[i]["MPN"]);
            }
            dt.WriteXml(stream);
            SqlXml mpnXml = new SqlXml(stream);
            var mpnStr = string.Join(",", prList.Select(x => x.MPN).ToList()) + ",";
            //获取WIP
            ht1.Clear();
            ht1.Add("MPN", mpnStr);
            var wipList = _wip.GetList(ht1).Where(x => x.ProcessNo != 0).ToList();
            //获取工顺
            ht1.Clear();
            ht1.Add("MPNList", mpnXml);
            var processListAll = _process.GetList(ht1).OrderByDescending(x => x.LT).ThenBy(x => x.ProcessNo); 
            foreach (var ps in processListAll)
            {
                var tempList = wipList.Where(x => x.FullItemCode == ps.ItemCode && x.ProcessNo == ps.ProcessNo).ToList();
                foreach (var wip in tempList)
                {
                    ps.QUANTITY += wip.Quantity;
                    ps.KOTO = wip.Koto;
                    ps.ONHOLD += wip.Onhold;
                    wipList.Remove(wip);
                }
            }
            foreach (var ps in processListAll)
            {
                var tempList = wipList.Where(x =>
                    x.FullItemCode.StartsWith(ps.ItemCode.Substring(0, 6))
                    && x.FullItemCode.Trim().EndsWith(ps.ItemCode.Substring(11).Trim())
                    && x.ProcessNo == ps.ProcessNo).ToList();
                foreach (var wip in tempList)
                {
                    ps.QUANTITY += wip.Quantity;
                    ps.KOTO = wip.Koto;
                    ps.ONHOLD += wip.Onhold;
                    wipList.Remove(wip);
                }
            }
            foreach (var wip in wipList)
            {
                var ps = processListAll.Where(x =>
                    wip.FullItemCode.StartsWith(x.ItemCode.Substring(0, 6))
                    && wip.FullItemCode.Trim().EndsWith(x.ItemCode.Substring(11).Trim())
                    && wip.ProcessNo > x.ProcessNo).FirstOrDefault();
                if (ps == null)
                    ps = processListAll.Where(x =>
                        wip.Level >= x.Level
                        && wip.ProcessNo >= x.ProcessNo).FirstOrDefault();
                if (ps == null)
                    continue;
                ps.QUANTITY += wip.Quantity;
                ps.KOTO = wip.Koto;
                ps.ONHOLD += wip.Onhold;
            }

            return Convertor.ToDataTable(processListAll.Select(x => new
            {
                x.MPN,
                x.ItemCode,
                x.Level,
                x.KeyProcess,
                x.LT,
                x.ProcessNo,
                x.ProcessCode,
                x.Description,
                x.ItemType,
                x.QUANTITY,
                x.KOTO,
                x.ONHOLD
            }));
        }

        private WipMovement addFGOnhand(List<WipMovement> list, string mpnStr, DateTime today)
        {
            WipMovement vm = new WipMovement();
            var vmt = list[list.Count - 1];
            vm.Project = vmt.Project;
            vm.Process = "FG-EOH";
            vm.LeadTime = 0;
            vm.Yield = 1;
            vm.KOTO = vmt.KOTO;
            vm.OutputDate = today;
            vm.WIPPCS = _wip.GetFGOnhand(mpnStr);
            vm.WIPPNL = vm.KOTO == 0 ? 0 : (int)Math.Ceiling(vm.WIPPCS * 1.0 / vm.KOTO);
            //
            int rowNo = list.Count + 2;
            vm.WIPOutputYield = $"=G{rowNo}*D{rowNo}";
            vm.WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
            vm.PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
            vm.CumDelta = $"=J{rowNo}-L{rowNo}";
            vm.WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";
            return vm;
        }
        private WipMovement addFPCOnhand(List<WipMovement>list, string mpnStr, DateTime today)
        {
            WipMovement vm = new WipMovement();
            var vmt = list[list.Count - 1];
            vm.Project = vmt.Project;
            vm.Process = "FPC-EOH";
            vm.LeadTime = vmt.LeadTime;
            vm.Yield = vmt.Yield;
            vm.KOTO = vmt.KOTO;
            vm.OutputDate = vmt.OutputDate;
            vm.WIPPCS = _wip.GetInnerOnhand(mpnStr);
            vm.WIPPNL = vm.KOTO == 0 ? 0 : (int)Math.Ceiling(vm.WIPPCS * 1.0 / vm.KOTO);
            //
            int rowNo = list.Count + 2;
            vm.WIPOutputYield = $"=G{rowNo}*D{rowNo}";
            vm.WIPoutputCum = $"=IF(A{rowNo}=A{rowNo + 1},J{rowNo + 1}+I{rowNo},I{rowNo})";
            vm.PlanOutputCum = $"=IF(A{rowNo}=A{rowNo + 1},L{rowNo + 1}+K{rowNo},K{rowNo})";
            vm.CumDelta = $"=J{rowNo}-L{rowNo}";
            vm.WIPOutput = $"=IF(E{rowNo}=E{rowNo - 1},0,SUMIFS($I:$I,$A:$A,$A{rowNo},$C:$C,$C{rowNo}))";
            return vm;
        }
    }
}