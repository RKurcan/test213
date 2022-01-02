using Riddhasoft.Employee.Entities;
using Riddhasoft.Employee.Services;
using Riddhasoft.HumanResource.Management.Report;
using Riddhasoft.OfficeSetup.Entities;
using Riddhasoft.OfficeSetup.Services;
using Riddhasoft.Report.ReportViewModel;
using RTech.Demo.Areas.Report.Controllers.Api;
using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RTech.Demo.Areas.Report.Controllers.Api.MonthlyAttendanceReportApiController;
using static RTech.Demo.Areas.Report.Controllers.Api.MonthlyEmployeeSummaryReportApiController;

namespace RTech.Demo.Areas.Report.Controllers
{
    public class AttendanceReportController : Controller
    {
        SSection _sectionServices = null;

        public AttendanceReportController()
        {
            _sectionServices = new SSection();
        }
        //
        // GET: /Report/AttendanceReport/
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult GenerateMonthlyReport(KendoReportViewModel vm)
        {

            SMonthlyWiseReport reportService = new SMonthlyWiseReport(RiddhaSession.Language);
            int[] employees;
            if (vm.ActiveInactiveMode == 0)
            {
                employees = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;
            }
            else
            {
                var allEmployee = new SEmployee().List().Data.Where(x => x.BranchId == RiddhaSession.BranchId);
                string filteredEmp = string.Join(",", allEmployee.Select(x => x.Id));
                employees = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, filteredEmp).Data;
            }
            var companyConfig = new SCompany().List().Data.Where(x => x.Id == RiddhaSession.CompanyId).FirstOrDefault();
            reportService.minimumOTHour = companyConfig.MinimumOTHour;
            reportService.FilteredEmployeeIDs = employees;
            var result = reportService.GetAttendanceReportFromSp(vm.OnDate.ToDateTime(), vm.ToDate.ToDateTime(), RiddhaSession.BranchId.ToInt(), vm.OTV2).Data;

            if (RiddhaSession.PackageId > 0 && vm.ActiveInactiveMode == 0)
            {
                result = result.Where(x => x.EmploymentStatus == EmploymentStatus.NormalEmployment || x.EmploymentStatus == EmploymentStatus.OnContract || x.EmploymentStatus == EmploymentStatus.PermanentJob || x.EmploymentStatus == EmploymentStatus.Retiring).ToList();
            }
            List<MonthlyWiseReport> reportData = new List<MonthlyWiseReport>();
            reportData = result;
            List<MOnthWiseEmployeeGroupReportVm> MOnthWiseEmployeeGroupReport = new List<MOnthWiseEmployeeGroupReportVm>();
            foreach (var item in employees)
            {
                MOnthWiseEmployeeGroupReportVm model = new MOnthWiseEmployeeGroupReportVm();
                var employeewisedata = reportData.Where(x => x.EmployeeId == item).ToList();
                model.EmployeeName = employeewisedata.Select(x => x.EmployeeCode).FirstOrDefault() + "-" + employeewisedata.Select(x => x.EmployeeName).FirstOrDefault();
                model.SectionName = employeewisedata.Select(x => x.SectionName).FirstOrDefault();
                model.SectionId = employeewisedata.Select(x => x.SectionId).FirstOrDefault();
                model.SectionCode = employeewisedata.Select(x => x.SectionCode).FirstOrDefault();
                model.DesignationName = employeewisedata.Select(x => x.DesignationName).FirstOrDefault();
                model.DesignationLevel = employeewisedata.Select(x => x.DesignationLevel).FirstOrDefault();
                model.monthlyWiseReports = new List<MonthlyWiseReport>();
                model.monthlyWiseReports.AddRange(employeewisedata);
                model.setOt();
                model.setEarlyIn();
                model.setEarlyOut();
                model.setLateIn();
                model.setLateOut();
                MOnthWiseEmployeeGroupReport.Add(model);
            }

            MOnthWiseEmployeeGroupReport = MOnthWiseEmployeeGroupReport.OrderBy(x => x.DesignationLevel).ThenBy(x => x.Employee).ToList();

            var units = (from c in reportData.GroupBy(x => x.SectionId).Select(x => x.FirstOrDefault())
                         select c.SectionId).ToArray();

            List<MOnthWiseDepartmentGroupReportVm> MOnthWiseDepartmentGroupReportVm = new List<MOnthWiseDepartmentGroupReportVm>();

            foreach (var item in units)
            {
                MOnthWiseDepartmentGroupReportVm model = new MOnthWiseDepartmentGroupReportVm();
                model.MOnthWiseEmployeeGroupReportVm = MOnthWiseEmployeeGroupReport.Where(x => x.SectionId == item).ToList();
                var unit = _sectionServices.List().Data.Where(x => x.Id == item).FirstOrDefault();
                if (unit != null)
                {
                    model.UnitLevelHierarchies = GetUnitLevelHierarchies(unit.ParentId ?? 0, unit.Id).OrderByDescending(x => x.Order).ToList();
                }

                MOnthWiseDepartmentGroupReportVm.Add(model);
            }
            MonthlyReportVm monthlyReport = new MonthlyReportVm()
            {
                Details = MOnthWiseDepartmentGroupReportVm
            };


            RiddhaSession.FromDate = vm.OnDate;
            RiddhaSession.ToDate = vm.ToDate;

            return View(monthlyReport);

        }
        public List<UnitLevelHierarchy> GetUnitLevelHierarchies(int ParentunitId, int unitId)
        {

            int order = 1;
            List<UnitLevelHierarchy> hierarchies = new List<UnitLevelHierarchy>();
            var unit = _sectionServices.List().Data.Where(x => x.Id == unitId).FirstOrDefault();
            hierarchies.Add(new UnitLevelHierarchy()
            {
                Name = unit.Name,
                Id = unit.Id,
                Order = order,
                UnitType = Enum.GetName(typeof(UnitType), unit.UnitType)
            });
            order++;
            AddParent(hierarchies, ParentunitId, order);


            return hierarchies;
        }

        public void AddParent(List<UnitLevelHierarchy> hierarchies, int unitId, int order)
        {

            var unitlevel_query = _sectionServices.List().Data.Where(x => x.Id == unitId).FirstOrDefault();

            if (unitlevel_query != null)
            {

                hierarchies.Add(new UnitLevelHierarchy()
                {
                    Name = unitlevel_query.Name,
                    Id = unitlevel_query.Id,
                    Order = order,
                    UnitType = Enum.GetName(typeof(UnitType), unitlevel_query.UnitType)
                });
                AddParent(hierarchies, unitlevel_query.ParentId ?? 0, order + 1);
            };
        }


        public ActionResult MonthlyEmployeeSummaryReport(KendoReportViewModel vm)
        {
            int totalDays = (vm.ToDate.ToDateTime() - vm.OnDate.ToDateTime()).Days + 1;
            SMonthlyWiseReport reportService = new SMonthlyWiseReport(RiddhaSession.Language);
            int[] employees = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;
            reportService.FilteredEmployeeIDs = employees;
            var result = reportService.GetAttendanceReportFromSp(vm.OnDate.ToDateTime(), vm.ToDate.ToDateTime(), RiddhaSession.BranchId.ToInt(), vm.OTV2).Data;
            if (RiddhaSession.PackageId > 0 && vm.ActiveInactiveMode == 0)
            {
                result = result.Where(x => x.EmploymentStatus == EmploymentStatus.NormalEmployment || x.EmploymentStatus == EmploymentStatus.OnContract || x.EmploymentStatus == EmploymentStatus.PermanentJob || x.EmploymentStatus == EmploymentStatus.Retiring).ToList();
            }
            int maxCount = 0;
            List<MonthlyWiseReport> reportData = new List<MonthlyWiseReport>();
            if (employees.Count() > 0)
            {
                maxCount = employees.Count();
                reportData = (from c in result
                              join d in employees on c.EmployeeId equals d
                              select c
                                 ).ToList();
            }
            else
            {
                reportData = result;
            }
            var summaryLst = (reportData.GroupBy(i => i.EmployeeId)
           .Select(i => new SummaryDaywiseReport()
           {
               EmployeeCode = i.FirstOrDefault().EmployeeCode,
               EmployeeName = i.FirstOrDefault().EmployeeName,
               DesignationName = i.FirstOrDefault().DesignationName,
               DepartmentName = i.FirstOrDefault().DepartmentName,
               DepartmentNamee = i.FirstOrDefault().DepartmentNamee,
               SectionName = i.FirstOrDefault().SectionName,
               SectionId = i.FirstOrDefault().SectionId,
               TotalDays = totalDays.ToString(),
               Absent = i.Where(j => j.Remark.ToLower() == "absent").Count().ToString(),
               OfficeOut = i.Where(j => j.OfficeVisit.ToLower() == "yes").Count().ToString(),
               KajOut = i.Where(j => j.Kaj.ToLower() == "yes").Count().ToString(),
               Present = i.Where(j => j.Remark.ToLower() == "present").Count().ToString(),
               PresentInHoliday = i.Where(j => j.Remark.ToLower() == "present" && j.Holiday.ToLower() == "yes").Count().ToString(),
               PresentInDayOff = i.Where(j => j.Remark.ToLower() == "present" && j.Weekend.ToLower() == "yes").Count().ToString(),
               Misc = i.Where(j => j.Remark.ToLower() == "misc").Count().ToString(),
               Weekend = i.Where(j => j.Weekend.ToLower() == "yes").Count().ToString(),
               Holiday = i.Where(j => j.Holiday.ToLower() == "yes").Count().ToString(),
               EarlyIn = i.Where(j => j.EarlyIn != "").Count().ToString(),
               EarlyOut = i.Where(j => j.EarlyOut != "").Count().ToString(),
               LateIn = i.Where(j => j.LateIn != "").Count().ToString(),
               LateOut = i.Where(j => j.LateOut != "").Count().ToString(),
               Leave = i.Where(j => j.OnLeave.ToLower() == "yes").Count().ToString(),
               Worked = i.Aggregate(new TimeSpan(), (sum, nextData) => sum.Add(nextData.Actual.ToTimeSpan())).TotalHours.ToString("#00.00"),
               Ot = GetOt(i),
               Remarks = getRemarks(i.Where(j => j.OnLeave.ToLower() == "yes").ToList(), i.Where(j => j.Remark.ToLower() == "absent").ToList())
           })).ToList();
            var units = (from c in summaryLst.GroupBy(x => x.SectionId).Select(x => x.FirstOrDefault())
                         select c.SectionId).ToArray();
            List<SummaryMonthlyListDetails> summaryDetails = new List<SummaryMonthlyListDetails>();
            foreach (var item in units)
            {

                SummaryMonthlyListDetails model = new SummaryMonthlyListDetails();
                model.Details = summaryLst.Where(x => x.SectionId == item).ToList();
                var unit = _sectionServices.List().Data.Where(x => x.Id == item).FirstOrDefault();
                if (unit != null)
                {
                    model.UnitLevelHierarchies = GetUnitLevelHierarchies(unit.ParentId ?? 0, unit.Id).OrderByDescending(x => x.Order).ToList();
                }

                summaryDetails.Add(model);
            }

            SummaryMonthlyList summary = new SummaryMonthlyList()
            {
                Details = summaryDetails
            };
            return View(summary);
        }

        private string GetOt(IGrouping<int, MonthlyWiseReport> i)
        {
            TimeSpan sumTillNowTimeSpan = TimeSpan.Zero;
            foreach (var item in i)
            {
                sumTillNowTimeSpan += item.Ot.ToTimeSpan();
            }
            double hours = sumTillNowTimeSpan.Hours + (sumTillNowTimeSpan.Days * 24);
            string time = hours + ":" + sumTillNowTimeSpan.Minutes;
            return time;
        }
        private string getRemarks(List<MonthlyWiseReport> leaveLst, List<MonthlyWiseReport> absentLst)
        {
            return joinStringForRemarks(leaveLst);

        }
        private List<DateRange> GetDateRanges(List<DateTime> dates)
        {
            if (dates == null || !dates.Any()) return null;
            dates = dates.OrderBy(x => x.Date).ToList();

            var dateRangeList = new List<DateRange>();

            DateRange dateRange = null;
            for (var i = 0; i < dates.Count; i++)
            {
                if (dateRange == null)
                {
                    dateRange = new DateRange { Start = dates[i] };
                }
                if (i == dates.Count - 1 || dates[i].Date.AddDays(1) != dates[i + 1].Date)
                {
                    dateRange.End = dates[i].Date;
                    dateRangeList.Add(dateRange);
                    dateRange = null;
                }
            }
            return dateRangeList;
        }
        private string joinStringForRemarks(List<MonthlyWiseReport> list)
        {
            if (list.Count() < 1)
            {
                return "";
            }
            string opDate = RiddhaSession.OperationDate;
            var dates = (from c in list
                         select c.WorkDate.ToDateTime()).ToList();
            List<DateRange> dateRanges = GetDateRanges(dates);
            IList<string> result = new List<string> { };
            foreach (var item in dateRanges)
            {
                if (opDate == "ne")
                {
                    string nepStartDate = list.Where(x => x.WorkDate.ToDateTime() == item.Start).FirstOrDefault().NepDate;
                    var startSplit = nepStartDate.Split('/');
                    if (item.Start == item.End)
                    {

                        result.Add(startSplit[2]);
                    }
                    else
                    {
                        string nepEndDate = list.Where(x => x.WorkDate.ToDateTime() == item.End).FirstOrDefault().NepDate;
                        var endSplit = nepEndDate.Split('/');
                        result.Add(startSplit[2] + "-" + endSplit[2]);
                    }
                }
                else
                {
                    if (item.Start == item.End)
                    {
                        result.Add(item.Start.Day.ToString());
                    }
                    else
                    {
                        result.Add(item.Start.Day.ToString() + "-" + item.End.Day.ToString());
                    }
                }

            }
            return string.Join(",", result);
        }
    }
}