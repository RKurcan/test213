using Riddhasoft.HRM.Services;
using Riddhasoft.Services.Common;
using RTech.Demo.Utilities;
using Riddhasoft.User.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Riddhasoft.Services.User;
using Riddhasoft.OfficeSetup.Entities;

namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class EmployeeTerminationReportApiController : ApiController
    {
        public KendoGridResult<object> GenerateReport(KendoReportViewModel vm)
        {
            int branchId = RiddhaSession.BranchId ?? 0;
            STermination terminationServices = new STermination();
            //List<EUser> users = new List<EUser>();
            //SUser userServices = new SUser();
            //users = userServices.List().Data.Where(x => x.BranchId == branchId).ToList();
            int[] employeeIds = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;
            List<TerminationGridViewModel> result = new List<TerminationGridViewModel>();
            var terminationQuery = terminationServices.List().Data.Where(x => x.BranchId == branchId && x.IsApproved == true);
            result = (from c in terminationQuery.ToList()
                      join d in employeeIds
                            on c.EmployeeId equals d
                      //join e in users
                      //      on c.ApprovedById equals e.Id
                      select new TerminationGridViewModel()
                      {
                          TerminationCode = c.Code,
                          EmployeeName = c.Employee.Code + " - " + c.Employee.Name,
                          SectionName = c.Employee.Section == null ? "" : c.Employee.Section.Name,
                          NoticeDate = c.NoticeDate.ToString("yyyy/MM/dd"),
                          EndDate = c.ServiceEndDate.ToString("yyyy/MM/dd"),
                          Reason = c.Reason,
                          Details = c.Details,
                          ChangeStatus = c.ChangeStatus.ToString(),
                          DesignationName = c.Employee.Designation == null ? "" : c.Employee.Designation.Name,
                          Department = c.Employee.Section.Department == null ? "" : c.Employee.Section.Department.Name,
                          DesignationLevel = c.Employee.Designation == null ? 0 : c.Employee.Designation.DesignationLevel
                      }).OrderBy(x => x.Department).ThenBy(x => x.SectionName).ThenBy(x => x.DesignationLevel).ThenBy(x => x.EmployeeName).ToList();
            return new KendoGridResult<object>()
            {
                Data = result.Skip(vm.Skip).Take(vm.Take),
                Status = ResultStatus.Ok,
                TotalCount = terminationQuery.Count()
            };
        }
    }

    public class TerminationGridViewModel
    {
        public string EmployeeName { get; set; }
        public string NoticeDate { get; set; }
        public string EndDate { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }
        public string ChangeStatus { get; set; }
        //public string ApprovedBy { get; set; }

        public string TerminationCode { get; set; }
        public string SectionName { get; set; }
        public string DesignationName { get; set; }
        public string Department { get; set; }
        public int DesignationLevel { get; set; }
    }
}
