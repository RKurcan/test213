using Riddhasoft.Employee.Services;
using Riddhasoft.OfficeSetup.Services;
using Riddhasoft.Services.Common;
using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class DailyManualPunchReportApiController : ApiController
    {
        [HttpPost]
        public KendoGridResult<object> GenerateReport(KendoReportViewModel vm)
        {
            int BranchId = RiddhaSession.BranchId ?? 0;
            string language = RiddhaSession.Language;
            DateTime OnDate = DateTime.Parse(vm.OnDate).Date;
            SDepartment _departmentServices = new SDepartment();
            SManualPunch manualPunchServices = new SManualPunch();
            int[] employees = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;
            var manualPunches = manualPunchServices.List().Data.Where(x => x.BranchId == BranchId && DbFunctions.TruncateTime(x.DateTime) == OnDate).ToList();
            var result = (from c in manualPunches
                          join d in employees
                             on c.EmployeeId equals d

                          select new ManualPunchGridViewModel()
                          {
                              EmployeeCode = c.Employee.Code,
                              EmployeeName = language == "ne" && c.Employee.NameNp != null ? c.Employee.NameNp : c.Employee.Name,
                              Date = c.DateTime.ToString("yyyy/MM/dd"),
                              Time = c.DateTime.ToString(@"hh\:mm"),
                              Remarks = c.Remark,
                              SectionName = c.Employee.Section == null ? "" : c.Employee.Section.Name,
                              DepartmentName = c.Employee.Section.Department == null ? "" : c.Employee.Section.Department.Name,
                              DesignationLevel = c.Employee.Designation == null ? 0 : c.Employee.Designation.DesignationLevel,
                              DesignationName = c.Employee.Designation == null ? "" : c.Employee.Designation.Name,
                          }).OrderBy(x => x.DesignationLevel).ThenBy(x => x.EmployeeName).ToList();
            return new KendoGridResult<object>()
            {
                Data = result,
                TotalCount = result.Count
            };
        }
    }

    public class ManualPunchGridViewModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Remarks { get; set; }
        public string SectionName { get; internal set; }
        public string DepartmentName { get; set; }
        public int DesignationLevel { get; set; }
        public string DesignationName { get; set; }
    }
}
