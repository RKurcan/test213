using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTech.Demo.Areas.Report.Models
{
    public class RankWiseAttendanceReportVm
    {
        public int DesignationLevel { get; set; }
        public string Designation { get; set; }
        public int EnrolledCount { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateInCount { get; set; }
        public int LeaveCount { get; set; }
    }

    public class RankWiseAttendanceReport
    {
        public List<RankWiseAttendanceReportVm> RankWiseAttendance { get; set; }
    }
}