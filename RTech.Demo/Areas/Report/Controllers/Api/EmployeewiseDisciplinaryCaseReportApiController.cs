using Riddhasoft.Employee.Entities;
using Riddhasoft.Employee.Services;
using Riddhasoft.HRM.Entities;
using Riddhasoft.HRM.Services;
using Riddhasoft.Services.Common;
using Riddhasoft.Services.User;
using Riddhasoft.User.Entity;
using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class EmployeewiseDisciplinaryCaseReportApiController : ApiController
    {
        public KendoGridResult<object> GenerateReport(KendoReportViewModel vm)
        {
            int branchId = RiddhaSession.BranchId ?? 0;

            SDisciplinaryCases caseService = new SDisciplinaryCases();
            List<EDisciplinaryCases> cases = new List<EDisciplinaryCases>();
            List<EDisciplinaryCasesDetail> caseDetails = new List<EDisciplinaryCasesDetail>();
            cases = caseService.List().Data.Where(x => x.BranchId == branchId).ToList();
            caseDetails = caseService.ListDetail().Data.ToList();

            SUser userService = new SUser();
            List<EUser> users = new List<EUser>();
            users = userService.List().Data.Where(x => x.BranchId == branchId).ToList();

            int[] employeeIds = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;

            var result = new List<DisciplinaryCaseGridViewModel>();

            result = (from c in cases
                      join d in caseDetails
                          on c.Id equals d.DisciplinaryCasesId
                      join e in employeeIds
                          on d.EmployeeId equals e
                      join f in users
                             on c.CreatedBy equals f.Id
                      select new DisciplinaryCaseGridViewModel()
                      {
                          CaseName = c.CaseName,
                          EmployeeName = d.Employee.Code + " - " + d.Employee.Name,
                          Description = c.Description,
                          CreatedOn = c.CreatedOn.ToString("yyyy/MM/dd"),
                          Status = c.DisciplinaryStatus.ToString(),
                          Actions = c.DisciplinaryActions.ToString(),
                          ForwardTo = c.ForwardTo.Name,
                          CreatedBy = f.Name,
                          DesignationName = d.Employee.Designation == null ? "" : d.Employee.Designation.Name,
                          SectionName = d.Employee.Section == null ? "" : d.Employee.Section.Name,
                        Department = d.Employee.Section.Department == null ? "" : d.Employee.Section.Department.Name,
                          DesignationLevel = d.Employee.Designation == null ? 0 : d.Employee.Designation.DesignationLevel
                      }).OrderBy(x => x.Department).ThenBy(x => x.SectionName).ThenBy(x => x.DesignationLevel).ThenBy(x => x.EmployeeName).ToList();

            return new KendoGridResult<object>()
            {
                Data = result.Skip(vm.Skip).Take(vm.Take),
                Status = ResultStatus.Ok,
                TotalCount = result.Count()
            };

        }
    }
}
