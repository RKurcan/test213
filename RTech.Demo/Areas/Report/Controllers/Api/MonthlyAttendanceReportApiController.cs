﻿using Riddhasoft.Employee.Entities;
using Riddhasoft.Employee.Services;
using Riddhasoft.HumanResource.Management.Report;
using Riddhasoft.OfficeSetup.Entities;
using Riddhasoft.OfficeSetup.Services;
using Riddhasoft.Report;
using Riddhasoft.Report.ReportViewModel;
using Riddhasoft.Services.Common;
using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Http;


namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class MonthlyAttendanceReportApiController : ApiController
    {
        //[HttpPost]
        //public KendoGridResult<object> GenerateReport(KendoReportViewModel vm)
        //{
        //    SMonthlyWiseReport reportService = new SMonthlyWiseReport(RiddhaSession.Language);
        //    int[] employees = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;
        //   var result = reportService.GetAttendanceReportFromSp(vm.OnDate.ToDateTime(), vm.ToDate.ToDateTime(), RiddhaSession.BranchId.ToInt()).Data;
        //    if (RiddhaSession.PackageId > 0 && vm.ActiveInactiveMode == 0)
        //    {
        //        result = result.Where(x => x.EmploymentStatus == EmploymentStatus.NormalEmployment || x.EmploymentStatus == EmploymentStatus.OnContract || x.EmploymentStatus == EmploymentStatus.PermanentJob || x.EmploymentStatus == EmploymentStatus.Retiring).ToList();
        //    }
        //    List<MonthlyWiseReport> reportData = new List<MonthlyWiseReport>();
        //    if (employees.Count()>0)
        //    {
        //       reportData = (from c in result
        //                          join d in employees
        //                          on c.EmployeeId equals d
        //                          select c
        //                         ).ToList();
        //    }
        //    else
        //    {
        //        reportData = result;
        //    }
        //    return new KendoGridResult<object>()
        //    {
        //        Data = reportData.OrderBy(x=>x.EmployeeCode).Skip(vm.Skip).Take(vm.Take),
        //        Status = ResultStatus.Ok,
        //        TotalCount = reportData.Count
        //    };
        //}

        [HttpPost]
        public ServiceResult<List<MOnthWiseDepartmentGroupReportVm>> GenerateReport(KendoReportViewModel vm)
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
            List<MOnthWiseEmployeeGroupReportVm> MOnthWiseEmployeeGroupReportVm = new List<MOnthWiseEmployeeGroupReportVm>();
            foreach (var item in employees)
            {
                MOnthWiseEmployeeGroupReportVm model = new MOnthWiseEmployeeGroupReportVm();
                var employeewisedata = reportData.Where(x => x.EmployeeId == item).ToList();
                model.EmployeeName = employeewisedata.Select(x => x.EmployeeCode).FirstOrDefault() + "-" + employeewisedata.Select(x => x.EmployeeName).FirstOrDefault();
                model.DepartmentCode = employeewisedata.Select(x => x.DepartmentCode).FirstOrDefault();
                model.SectionName = employeewisedata.Select(x => x.SectionName).FirstOrDefault();
                model.DesignationName = employeewisedata.Select(x => x.DesignationName).FirstOrDefault();
                model.DesignationLevel = employeewisedata.Select(x => x.DesignationLevel).FirstOrDefault();
                model.Employee = employeewisedata.Select(x => x.EmployeeName).FirstOrDefault();
                model.monthlyWiseReports = new List<MonthlyWiseReport>();
                model.monthlyWiseReports.AddRange(employeewisedata);
                MOnthWiseEmployeeGroupReportVm.Add(model);
            }

            MOnthWiseEmployeeGroupReportVm = MOnthWiseEmployeeGroupReportVm.OrderBy(x => x.DesignationLevel).ThenBy(x => x.Employee).ToList();

            var departments = (from c in reportData.GroupBy(x => x.DepartmentCode).Select(x => x.FirstOrDefault())
                               select c.DepartmentCode).ToArray();

            List<MOnthWiseDepartmentGroupReportVm> MOnthWiseDepartmentGroupReportVm = new List<MOnthWiseDepartmentGroupReportVm>();

            foreach (var item in departments)
            {
                MOnthWiseDepartmentGroupReportVm model = new MOnthWiseDepartmentGroupReportVm();
                model.MOnthWiseEmployeeGroupReportVm = MOnthWiseEmployeeGroupReportVm.Where(x => x.DepartmentCode == item).ToList();
                model.DepartmentName = model.MOnthWiseEmployeeGroupReportVm.Select(x => x.monthlyWiseReports.Select(y => y.DepartmentName).FirstOrDefault()).FirstOrDefault();
                MOnthWiseDepartmentGroupReportVm.Add(model);
            }
            return new ServiceResult<List<MOnthWiseDepartmentGroupReportVm>>()
            {
                Data = MOnthWiseDepartmentGroupReportVm,
                Status = ResultStatus.Ok,
            };
        }

        [HttpPost]
        public ServiceResult<object> GenerateReportWithoutGrouping(KendoReportViewModel vm)
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

            reportService.FilteredEmployeeIDs = employees;
            var result = reportService.GetAttendanceReportFromSp(vm.OnDate.ToDateTime(), vm.ToDate.ToDateTime(), RiddhaSession.BranchId.ToInt(), vm.OTV2).Data;
            if (RiddhaSession.PackageId > 0 && vm.ActiveInactiveMode == 0)
            {
                result = result.Where(x => x.EmploymentStatus == EmploymentStatus.NormalEmployment || x.EmploymentStatus == EmploymentStatus.OnContract || x.EmploymentStatus == EmploymentStatus.PermanentJob || x.EmploymentStatus == EmploymentStatus.Retiring).ToList();
            }
            List<MonthlyWiseReport> reportData = new List<MonthlyWiseReport>();
            reportData = result;
            return new ServiceResult<object>()
            {
                Data = reportData,
                Status = ResultStatus.Ok,
            };
        }

        [HttpPost]
        public ServiceResult<string> ExportToExcel(KendoReportViewModel vm)
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
            reportService.FilteredEmployeeIDs = employees;
            var data = reportService.GetAttendanceReportFromSp(vm.OnDate.ToDateTime(), vm.ToDate.ToDateTime(), RiddhaSession.BranchId.ToInt(), vm.OTV2).Data;
            var result = (from c in data
                          select new MonthlyWiseReportExcelexportVm()
                          {
                              Actual = c.Actual,
                              ActualLunchIn = c.ActualLunchIn,
                              ActualLunchOut = c.ActualLunchOut,
                              ActualTimeIn = c.ActualTimeIn,
                              ActualTimeOut = c.ActualTimeOut,
                              DayName = c.DayName,
                              DepartmentName = c.DepartmentName,
                              EarlyIn = c.EarlyIn,
                              EarlyOut = c.EarlyOut,
                              EmployeeName = c.EmployeeDeviceCode + " - " + c.EmployeeName,
                              LateIn = c.LateIn,
                              LateOut = c.LateOut,
                              NepDate = c.NepDate,
                              Ot = c.Ot,
                              PlannedLunchIn = c.PlannedLunchIn,
                              PlannedLunchOut = c.PlannedLunchOut,
                              PlannedTimeIn = c.PlannedTimeIn,
                              PlannedTimeOut = c.PlannedTimeOut,
                              Remark = c.Remark,
                              Standard = c.Standard,
                              WorkDate = c.WorkDate,
                          }).ToList();
            //var excleToDump = result.ExportToExcel<MonthlyWiseReportExcelexportVm>();

            var path = ResponseExcel(result);
            return new ServiceResult<string>()
            {
                Data = path,
                Status = ResultStatus.Ok
            };
        }

        private string ResponseExcel(List<MonthlyWiseReportExcelexportVm> result)
        {
            string branchCode = "";
            var currentUser = RiddhaSession.CurrentUser;
            if (currentUser != null && currentUser.Branch != null)
            {
                branchCode = currentUser.Branch.Code;
            }
            var id = Guid.NewGuid();
            var fileSpec = @"/Images/File/" + branchCode + "_" + id + ".xlsx";
            FileInfo fi = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath(@"/") + fileSpec);
            using (var package = new OfficeOpenXml.ExcelPackage(fi))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Report");
                worksheet = package.Workbook.Worksheets.Add("Assessment Attempts");
                worksheet.Row(1).Height = 20;

                worksheet.TabColor = System.Drawing.Color.Gold;
                worksheet.DefaultRowHeight = 12;
                worksheet.Row(1).Height = 20;

                worksheet.Cells[1, 1].Value = "Employee Number";
                worksheet.Cells[1, 2].Value = "Course Code";

                var cells = worksheet.Cells["A1:J1"];
                var rowCounter = 2;

                worksheet.Cells[rowCounter, 1].Value = "binod";
                worksheet.Cells[rowCounter, 2].Value = "adfa";

                rowCounter++;
                worksheet.Cells[rowCounter, 1].Value = "Raz";
                worksheet.Cells[rowCounter, 2].Value = "test";

                rowCounter++;
                worksheet.Cells[rowCounter, 1].Value = "Umeshj";
                worksheet.Cells[rowCounter, 2].Value = "helow";

                rowCounter++;
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();


                package.Workbook.Properties.Title = "Attempts";
                var context = this;

                package.Save();

            }
            return fileSpec;
        }



        public class MOnthWiseDepartmentGroupReportVm
        {

            public int UnitId { get; set; }
            public string DepartmentName { get; set; }
            public List<UnitLevelHierarchy> UnitLevelHierarchies { get; set; }
            public List<MOnthWiseEmployeeGroupReportVm> MOnthWiseEmployeeGroupReportVm { get; set; }
        }

        public class UnitLevelHierarchy
        {

            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public string UnitType { get; set; }
            public UnitType UnitTypeId { get; set; }
            public int? ParentId { get;  set; }
        }

        public class MonthlyReportVm
        {
            public List<MOnthWiseDepartmentGroupReportVm> Details { get; set; }
        }
        public class MOnthWiseEmployeeGroupReportVm
        {

            private string _totalOt = "";
            private string _totalEarlyIn = "";
            private string _totalEarlyOut = "";
            private string _totalLateIn = "";
            private string _totalLateOut = "";
            public string DepartmentCode { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public List<MonthlyWiseReport> monthlyWiseReports { get; set; }
            public string SectionName { get; set; }
            public string DesignationName { get; set; }
            public int DesignationLevel { get; set; }
            public string Employee { get; set; }
            public string SectionCode { get; set; }
            public int SectionId { get; set; }


            public void setOt()
            {
                var ot = (from c in this.monthlyWiseReports
                          select new
                          {
                              time = string.IsNullOrEmpty(c.Ot) == true ? new TimeSpan() : c.Ot.ToTimeSpan()
                          }).Sum(x => x.time.Ticks);


                _totalOt = TotalTime(ot);
            }

            public void setEarlyIn()
            {
                var ot = (from c in this.monthlyWiseReports
                          select new
                          {
                              time = string.IsNullOrEmpty(c.EarlyIn) == true ? new TimeSpan() : c.EarlyIn.ToTimeSpan()
                          }).Sum(x => x.time.Ticks);

                _totalEarlyIn = TotalTime(ot);
            }


            public void setEarlyOut()
            {
                var ot = (from c in this.monthlyWiseReports
                          select new
                          {
                              time = string.IsNullOrEmpty(c.EarlyOut) == true ? new TimeSpan() : c.EarlyOut.ToTimeSpan()
                          }).Sum(x => x.time.Ticks);

                _totalEarlyOut = TotalTime(ot);
            }

            public void setLateIn()
            {
                var ot = (from c in this.monthlyWiseReports
                          select new
                          {
                              time = string.IsNullOrEmpty(c.LateIn) == true ? new TimeSpan() : c.LateIn.ToTimeSpan()
                          }).Sum(x => x.time.Ticks);

                _totalLateIn = TotalTime(ot);
            }

            public void setLateOut()
            {
                var ot = (from c in this.monthlyWiseReports
                          select new
                          {
                              time = string.IsNullOrEmpty(c.LateOut) == true ? new TimeSpan() : c.LateOut.ToTimeSpan()
                          }).Sum(x => x.time.Ticks);

                _totalLateOut = TotalTime(ot);
            }
            public string TotalOt
            {
                get
                {


                    return _totalOt;

                }
            }
            public string TotalEarlyIn
            {
                get
                {


                    return _totalEarlyIn;

                }
            }
            public string TotalEarlyOut
            {
                get
                {


                    return _totalEarlyOut;

                }
            }
            public string TotalLateIn
            {
                get
                {


                    return _totalLateIn;

                }
            }
            public string TotalLateOut
            {
                get
                {


                    return _totalLateOut;

                }
            }


            public string TotalTime(long ot)
            {
                TimeSpan sumTillNowTimeSpan = TimeSpan.Zero;
                try
                {

                    sumTillNowTimeSpan = TimeSpan.FromTicks(ot);
                    double hours = sumTillNowTimeSpan.Hours + (sumTillNowTimeSpan.Days * 24);
                    string time = hours + ":" + sumTillNowTimeSpan.Minutes;
                    return time;
                }
                catch (Exception)
                {
                    return "00:00";

                }
            }
        }

        public class MonthlyWiseReportExcelexportVm
        {
            public string EmployeeName { get; set; }
            public string DepartmentName { get; set; }
            public string WorkDate { get; set; }
            public string NepDate { get; set; }
            public string DayName { get; set; }
            public string PlannedTimeIn { get; set; }
            public string PlannedTimeOut { get; set; }
            public string PlannedLunchIn { get; set; }
            public string PlannedLunchOut { get; set; }
            public string ActualTimeIn { get; set; }
            public string ActualTimeOut { get; set; }
            public string ActualLunchIn { get; set; }
            public string ActualLunchOut { get; set; }
            public string Standard { get; set; }
            public string Ot { get; set; }
            public string Actual { get; set; }
            public string LateIn { get; set; }
            public string EarlyIn { get; set; }
            public string EarlyOut { get; set; }
            public string LateOut { get; set; }
            public string Remark { get; set; }

        }
    }
}
