﻿using Riddhasoft.Employee.Entities;
using Riddhasoft.HumanResource.Management.Report;
using Riddhasoft.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RTech.Demo.Utilities;
using Riddhasoft.Report.ReportViewModel;

namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class MonthlyEarlyInReportApiController : ApiController
    {
        [HttpPost]
        public KendoGridResult<object> GenerateReport(KendoReportViewModel vm)
        {
            SMonthlyWiseReport reportService = new SMonthlyWiseReport(RiddhaSession.Language);
            int[] employees = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;
            reportService.FilteredEmployeeIDs = employees;
            var result = reportService.GetAttendanceReportFromSp(vm.OnDate.ToDateTime(), vm.ToDate.ToDateTime(), RiddhaSession.BranchId.ToInt()).Data;
            if (RiddhaSession.PackageId > 0 && vm.ActiveInactiveMode == 0)
            {
                result = result.Where(x => x.EmploymentStatus == EmploymentStatus.NormalEmployment || x.EmploymentStatus == EmploymentStatus.OnContract || x.EmploymentStatus == EmploymentStatus.PermanentJob || x.EmploymentStatus == EmploymentStatus.Retiring).ToList();
            }
            List<MonthlyWiseReport> reportData = new List<MonthlyWiseReport>();
            if (employees.Count() > 0)
            {
                reportData = (from c in result
                              join d in employees
                              on c.EmployeeId equals d
                              where c.EarlyIn !="00:00" && c.EarlyIn!=""
                              select c
                                  ).ToList();
            }
            else
            {
                reportData = result;
            }

            return new KendoGridResult<object>()
            {
                //Data = reportData.OrderBy(x => x.EmployeeCode).Skip(vm.Skip).Take(vm.Take),
                Data = reportData,
                Status = ResultStatus.Ok,
                TotalCount = reportData.Count
            };
        }
    }
}
