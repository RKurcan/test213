using Riddhasoft.Employee.Entities;
using Riddhasoft.HRM.Entities.Qualification;
using Riddhasoft.HRM.Services.Qualification;
using Riddhasoft.Services.Common;
using Riddhasoft.Services.User;
using Riddhasoft.User.Entity;
using RTech.Demo.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class EmployeeQualificationReportApiController : ApiController
    {
        public KendoGridResult<object> GenerateReport(KendoReportViewModel vm)
        {
            int branchId = RiddhaSession.BranchId ?? 0;
            int[] employeeIds = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;

            SEmployeeEducation educationService = new SEmployeeEducation();
            List<EEmployeeEducation> education = new List<EEmployeeEducation>();
            education = educationService.List().Data.Where(x => x.BranchId == branchId).ToList();

            //SUser userService = new SUser();
            //List<EUser> users = new List<EUser>();
            //users = userService.List().Data.Where(x => x.BranchId == branchId).ToList();

            var rpt = educationService.GetQualificationReport(branchId);
            List<QualificationViewModel> qualifications = (from s in rpt.Data.Skill
                                                           select new QualificationViewModel()
                                                           {
                                                               EmployeeId = s.EmployeeId,
                                                               EmployeeName = s.Employee.Code + " - " + s.Employee.Name,
                                                               Type = "Skill",
                                                               Name = s.Skills.Code + " - " + s.Skills.Name,
                                                               Description = s.Skills.Description,
                                                               DesignationName = s.Employee.Designation == null ? "" : s.Employee.Designation.Name,
                                                               SectionName = s.Employee.Section == null ? "" : s.Employee.Section.Name,
                                                               Department = s.Employee.Section.Department == null ? "" : s.Employee.Section.Department.Name,
                                                               DesignationLevel = s.Employee.Designation == null ? 0 : s.Employee.Designation.DesignationLevel,
                                                               //ApprovedById = s.ApprovedById??0
                                                           }
                                  ).Union(
                                      from e in rpt.Data.Education
                                      select new QualificationViewModel()
                                      {
                                          EmployeeId = e.EmployeeId,
                                          EmployeeName = e.Employee.Code + " - " + e.Employee.Name,
                                          Type = "Education",
                                          Name = e.Education.Code + " - " + e.Education.Name,
                                          Description = e.Education.Description,
                                          DesignationName = e.Employee.Designation == null ? "" : e.Employee.Designation.Name,
                                          SectionName = e.Employee.Section == null ? "" : e.Employee.Section.Name,
                                          Department = e.Employee.Section.Department == null ? "" : e.Employee.Section.Department.Name,
                                          DesignationLevel = e.Employee.Designation == null ? 0 : e.Employee.Designation.DesignationLevel,
                                          //ApprovedById = e.ApprovedById ?? 0
                                      }
                                 ).Union(
                                      from f in rpt.Data.License
                                      select new QualificationViewModel()
                                      {
                                          EmployeeId = f.EmployeeId,
                                          EmployeeName = f.Employee.Code + " - " + f.Employee.Name,
                                          Type = "License",
                                          Name = f.License.Code + " - " + f.License.Name,
                                          Description = f.License.Description,
                                          DesignationName = f.Employee.Designation == null ? "" : f.Employee.Designation.Name,
                                          SectionName = f.Employee.Section == null ? "" : f.Employee.Section.Name,
                                          Department = f.Employee.Section.Department == null ? "" : f.Employee.Section.Department.Name,
                                          DesignationLevel = f.Employee.Designation == null ? 0 : f.Employee.Designation.DesignationLevel,
                                          //ApprovedById = f.ApprovedById ?? 0
                                      }
                                 ).Union(
                                      from g in rpt.Data.Language
                                      select new QualificationViewModel()
                                      {
                                          EmployeeId = g.EmployeeId,
                                          EmployeeName = g.Employee.Code + " - " + g.Employee.Name,
                                          Type = "Language",
                                          Name = g.Language.Code + " - " + g.Language.Name,
                                          Description = g.Language.Description,
                                          DesignationName = g.Employee.Designation == null ? "" : g.Employee.Designation.Name,
                                          SectionName = g.Employee.Section == null ? "" : g.Employee.Section.Name,
                                          Department = g.Employee.Section.Department == null ? "" : g.Employee.Section.Department.Name,
                                          DesignationLevel = g.Employee.Designation == null ? 0 : g.Employee.Designation.DesignationLevel,
                                          //ApprovedById = g.ApprovedById ?? 0
                                      }
                                 ).ToList();

            var result = new List<EmployeeQualificationGridViewModel>();

            result = (from c in qualifications
                      join d in employeeIds
                         on c.EmployeeId equals d
                      //join e in users
                      //   on c.ApprovedById equals e.Id
                      select new EmployeeQualificationGridViewModel()
                      {
                          EmployeeId = c.EmployeeId,
                          Type = c.Type,
                          EmployeeName = c.EmployeeName,
                          Name = c.Name,
                          Description = c.Description,
                          DesignationName = c.DesignationName,
                          SectionName = c.SectionName,
                          Department = c.Department,
                          DesignationLevel = c.DesignationLevel
                          //ApprovedBy = e.Name
                      }).OrderBy(x => x.Department).ThenBy(x => x.SectionName).ThenBy(x => x.DesignationLevel).ThenBy(x => x.EmployeeName).ToList();
            return new KendoGridResult<object>()
            {
                Data = result.Skip(vm.Skip).Take(vm.Take),
                Status = ResultStatus.Ok,
                TotalCount = result.Count()
            };
        }
    }

    public class EmployeeQualificationGridViewModel
    {
        public int EmployeeId { get; set; }
        public string Type { get; set; }
        public string EmployeeName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ApprovedBy { get; set; }
        public string DesignationName { get; set; }
        public string SectionName { get; set; }
        public string Department { get;  set; }
        public int DesignationLevel { get;  set; }
    }

    public class QualificationViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DesignationName { get; set; }
        public string SectionName { get; set; }
        public string Department { get; set; }
        public int DesignationLevel { get; set; }
        //public int ApprovedById { get; set; }
    }
}
