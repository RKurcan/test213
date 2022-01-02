﻿using Riddhasoft.DB;
using Riddhasoft.Employee.Entities;
using Riddhasoft.Report;
using Riddhasoft.Services.Common;
using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class LeaveSummaryReportApiController : ApiController
    {
        [HttpPost]
        public KendoGridResult<object> GenerateReport(KendoReportViewModel vm)
        {
            SLeaveReport reportService = new SLeaveReport(RiddhaSession.Language);
            int[] employees = Common.GetEmpIdsForReportParam(vm.DeptIds, vm.SectionIds, vm.EmpIds).Data;
            var result = reportService.GetLeaveReportFromSp(vm.OnDate.ToDateTime(), vm.ToDate.ToDateTime(), RiddhaSession.BranchId.ToInt(), RiddhaSession.FYId).Data;
            if (RiddhaSession.PackageId > 0 && vm.ActiveInactiveMode == 0)
            {
                result = result.Where(x => x.EmploymentStatus == EmploymentStatus.NormalEmployment || x.EmploymentStatus == EmploymentStatus.OnContract || x.EmploymentStatus == EmploymentStatus.PermanentJob || x.EmploymentStatus == EmploymentStatus.Retiring).ToList();
            }
            List<LeaveGridVm> reportData = new List<LeaveGridVm>();
            if (employees.Count() > 0)
            {
                reportData = (from c in result
                              join d in employees
                              on c.EmployeeId equals d
                              select new LeaveGridVm()
                              {
                                  EmployeeName = c.Name,
                                  LeaveName = c.LeaveName,
                                  TakenLeave = c.TakenLeave,
                                  SectionName=c.SectionName

                              }).ToList();
            }
            else
            {
                reportData = null;
            }
            return new KendoGridResult<object>()
            {
                Data = reportData.Skip(vm.Skip).Take(vm.Take),
                Message = "",
                Status = ResultStatus.Ok,
                TotalCount = reportData.Count
            };
        }
        public class LeaveGridVm
        {
            public string EmployeeName { get; set; }
            public string LeaveName { get; set; }
            public decimal TakenLeave { get; set; }
            public string SectionName { get;  set; }
        }
    }
}
