using Riddhasoft.Employee.Entities;
using Riddhasoft.Employee.Services;
using Riddhasoft.Entity.User;
using Riddhasoft.OfficeSetup.Entities;
using Riddhasoft.OfficeSetup.Services;
using Riddhasoft.Services.Common;
using Riddhasoft.Services.User;
using RTech.Demo.Areas.Employee.Controllers.Api;
using RTech.Demo.Areas.Office.Controllers.Api;
using RTech.Demo.Models;
using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static RTech.Demo.Areas.Report.Controllers.Api.MonthlyAttendanceReportApiController;

namespace RTech.Demo.Areas.Report.Controllers.Api
{
    public class AttendanceReportApiController : ApiController
    {
        SDateTable dateTableServices = null;
        List<SectionGridVm> sectionsbyDepartment = new List<SectionGridVm>();

        SSection _sectionServices = null;
        public AttendanceReportApiController()
        {
            dateTableServices = new SDateTable();
            _sectionServices = new SSection();
        }
        // GET api/attendancereportapi
        public ServiceResult<List<ReportItem>> Get()
        {
            int roleId = RiddhaSession.RoleId;
            SUserRole userRoleServices = new SUserRole();
            Common common = new Common();
            var allReportItems = common.populateList();
            if (roleId != 0)
            {
                allReportItems = (from c in allReportItems
                                  join d in userRoleServices.ListActionRights().Data.Where(x => x.RoleId == roleId) on c.ActionCode equals d.MenuAction.ActionCode
                                  select c).ToList();
            }
            return new ServiceResult<List<ReportItem>>()
            {
                Data = allReportItems,
                Status = ResultStatus.Ok
            };
        }

        public ServiceResult<List<DepartmentGridVm>> GetDepartments()
        {
            SDepartment services = new SDepartment();
            int? branchId = RiddhaSession.BranchId;
            int roleId = RiddhaSession.RoleId;
            string language = RiddhaSession.Language.ToString();

            var departmentLst = new List<DepartmentGridVm>();


            if (roleId > 0)
            {

                DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
                switch (dataVisibilityLevel)
                {
                    case DataVisibilityLevel.Self:
                    case DataVisibilityLevel.Unit:
                    case DataVisibilityLevel.Department:
                        departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId && x.Id == RiddhaSession.DepartmentId)
                                         select new DepartmentGridVm()
                                         {
                                             Id = c.Id,
                                             BranchId = c.BranchId,
                                             Code = c.Code,
                                             //Name = c.Name,
                                             //NameNp = c.NameNp,
                                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                             NumberOfStaff = c.NumberOfStaff
                                         }).ToList();
                        break;
                    case DataVisibilityLevel.Branch:
                    case DataVisibilityLevel.All:
                        departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId)
                                         select new DepartmentGridVm()
                                         {
                                             Id = c.Id,
                                             BranchId = c.BranchId,
                                             Code = c.Code,
                                             //Name = c.Name,
                                             //NameNp = c.NameNp,
                                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                             NumberOfStaff = c.NumberOfStaff
                                         }).ToList();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId)
                                 select new DepartmentGridVm()
                                 {
                                     Id = c.Id,
                                     BranchId = c.BranchId,
                                     Code = c.Code,
                                     //Name = c.Name,
                                     //NameNp = c.NameNp,
                                     Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                     NumberOfStaff = c.NumberOfStaff
                                 }).ToList();
            }

            return new ServiceResult<List<DepartmentGridVm>>()
            {
                Data = departmentLst,
                Status = ResultStatus.Ok
            };
        }
        public ServiceResult<List<DepartmentGridVm>> GetDepartmentsByBranch(string branchIds)
        {
            SDepartment services = new SDepartment();
            int roleId = RiddhaSession.RoleId;
            string[] branches = branchIds.Split(',');
            string language = RiddhaSession.Language.ToString();
            var departmentLst = new List<DepartmentGridVm>();
            if (roleId > 0)
            {

                DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
                switch (dataVisibilityLevel)
                {
                    case DataVisibilityLevel.Self:
                    case DataVisibilityLevel.Unit:
                    case DataVisibilityLevel.Department:
                        //departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId && x.Id == RiddhaSession.DepartmentId)
                        departmentLst = (from c in services.List().Data.ToList()
                                         join d in branches on c.BranchId equals int.Parse(d)
                                         where c.Id == RiddhaSession.DepartmentId
                                         select new DepartmentGridVm()
                                         {
                                             Id = c.Id,
                                             BranchId = c.BranchId,
                                             Code = c.Code,
                                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                             NumberOfStaff = c.NumberOfStaff
                                         }).ToList();
                        break;
                    case DataVisibilityLevel.Branch:
                        departmentLst = (from c in services.List().Data.ToList()
                                         join d in branches on c.BranchId equals int.Parse(d)
                                         select new DepartmentGridVm()
                                         {
                                             Id = c.Id,
                                             BranchId = c.BranchId,
                                             Code = c.Code,
                                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                             NumberOfStaff = c.NumberOfStaff
                                         }).ToList();
                        break;
                    case DataVisibilityLevel.All:
                        departmentLst = (from c in services.List().Data.ToList()
                                         join d in branches on c.BranchId equals int.Parse(d)
                                         select new DepartmentGridVm()
                                         {
                                             Id = c.Id,
                                             BranchId = c.BranchId,
                                             Code = c.Code,
                                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                             NumberOfStaff = c.NumberOfStaff
                                         }).ToList();
                        break;
                    case DataVisibilityLevel.ReportingHierarchy:
                        departmentLst = (from c in services.List().Data.ToList()
                                         join d in branches on c.BranchId equals int.Parse(d)
                                         select new DepartmentGridVm()
                                         {
                                             Id = c.Id,
                                             BranchId = c.BranchId,
                                             Code = c.Code,
                                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                             NumberOfStaff = c.NumberOfStaff
                                         }).ToList();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                departmentLst = (from c in services.List().Data.ToList()
                                 join d in branches on c.BranchId equals int.Parse(d)
                                 select new DepartmentGridVm()
                                 {
                                     Id = c.Id,
                                     BranchId = c.BranchId,
                                     Code = c.Code,
                                     Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                     NumberOfStaff = c.NumberOfStaff
                                 }).ToList();
            }

            return new ServiceResult<List<DepartmentGridVm>>()
            {
                Data = departmentLst,
                Status = ResultStatus.Ok
            };
        }

        #region For New Part Report 
        //public ServiceResult<List<DropdownViewModel>> GetUnitTypeDepartment()
        //{
        //    SSection services = new SSection();
        //    int roleId = RiddhaSession.RoleId;
        //    string language = RiddhaSession.Language.ToString();

        //    var departmentLst = new List<DropdownViewModel>();

        //    var list = services.List().Data.Where(x => x.UnitType == UnitType.Department && x.BranchId == RiddhaSession.BranchId).ToList();
        //    if (roleId > 0)
        //    {
        //        DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
        //        switch (dataVisibilityLevel)
        //        {
        //            case DataVisibilityLevel.Self:
        //            case DataVisibilityLevel.Unit:
        //                AddSectionParentForDropDown(RiddhaSession.SectionId, list, departmentLst);
        //                break;
        //            case DataVisibilityLevel.Department:
        //                departmentLst = (from c in list
        //                                 where c.Id == RiddhaSession.DepartmentId
        //                                 select new DropdownViewModel()
        //                                 {
        //                                     Id = c.Id,
        //                                     Code = c.Code,
        //                                     Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
        //                                 }).ToList();
        //                break;
        //            case DataVisibilityLevel.Branch:
        //                departmentLst = (from c in list
        //                                 select new DropdownViewModel()
        //                                 {
        //                                     Id = c.Id,
        //                                     Code = c.Code,
        //                                     Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
        //                                 }).ToList();
        //                break;
        //            case DataVisibilityLevel.All:
        //                departmentLst = (from c in list
        //                                 select new DropdownViewModel()
        //                                 {
        //                                     Id = c.Id,
        //                                     Code = c.Code,
        //                                     Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
        //                                 }).ToList();
        //                break;
        //            case DataVisibilityLevel.ReportingHierarchy:
        //                departmentLst = (from c in list
        //                                 select new DropdownViewModel()
        //                                 {
        //                                     Id = c.Id,
        //                                     Code = c.Code,
        //                                     Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
        //                                 }).ToList();
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        departmentLst = (from c in list
        //                         select new DropdownViewModel()
        //                         {
        //                             Id = c.Id,
        //                             Code = c.Code,
        //                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
        //                         }).ToList();
        //    }

        //    return new ServiceResult<List<DropdownViewModel>>()
        //    {
        //        Data = departmentLst,
        //        Status = ResultStatus.Ok
        //    };
        //}

        public ServiceResult<ReportDataVisibilityViewModel> GetUnitTypeDepartment()
        {

            SSection services = new SSection();
            int roleId = RiddhaSession.RoleId;
            string language = RiddhaSession.Language.ToString();
            ReportDataVisibilityViewModel vm = new ReportDataVisibilityViewModel();
            var dropDownList = new List<DropdownViewModel>();

            var list = services.List().Data.Where(x => x.BranchId == RiddhaSession.BranchId).ToList();
            var deparmentlist = list.Where(x => x.UnitType == UnitType.Department && x.ParentId == null).ToList();
            vm.IsSelf = false;
            if (roleId > 0)
            {
                DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
                switch (dataVisibilityLevel)
                {
                    case DataVisibilityLevel.Self:
                        List<string> listSection = new List<string>();
                        listSection.Add(RiddhaSession.SectionId.ToString());
                        ESection selfEmployeeList = services.List().Data.Where(x => x.Id == RiddhaSession.SectionId && x.BranchId == RiddhaSession.BranchId).FirstOrDefault();
                        string[] sections = listSection.ToArray();
                        vm.Employees = GetEmployeesForReport(sections, RiddhaSession.EmployeeId);
                        vm.IsSelf = true;
                        switch ((UnitType)selfEmployeeList.UnitType)
                        {
                            case UnitType.Department:
                                vm.Departments = new List<DropdownViewModel>();
                                vm.Departments.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name
                                });

                                break;
                            case UnitType.Directorate:
                                vm.Directorates = new List<DropdownViewModel>();
                                vm.Directorates.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name
                                });
                                break;
                            case UnitType.Section:
                                vm.Sections = new List<DropdownViewModel>();
                                vm.Sections.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name
                                });
                                break;
                            case UnitType.Unit:
                                vm.Units = new List<DropdownViewModel>();
                                vm.Units.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name
                                });
                                break;
                            default:

                                break;
                        }
                        break;

                    case DataVisibilityLevel.Department:
                        list = list.Where(x => x.UnitType == UnitType.Department).ToList();
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.Departments = dropDownList;
                        break;
                    case DataVisibilityLevel.Directorate:
                        list = list.Where(x => x.UnitType == UnitType.Directorate).ToList();
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.Directorates = dropDownList;
                        break;
                    case DataVisibilityLevel.Section:
                        list = list.Where(x => x.UnitType == UnitType.Section).ToList();
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.Sections = dropDownList;
                        break;
                    case DataVisibilityLevel.Unit:
                        list = list.Where(x => x.UnitType == UnitType.Unit).ToList();
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.Units = dropDownList;
                        break;

                    default:
                        vm.Departments = (from c in deparmentlist
                                          select new DropdownViewModel()
                                          {
                                              Id = c.Id,
                                              Code = c.Code,
                                              Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                              ParentId = c.ParentId
                                          }).ToList();
                        AddSectionChildForDropDownForTreeStructure(deparmentlist, vm.Departments, UnitType.Department);
                        break;
                }
            }
            else
            {
                vm.Departments = (from c in deparmentlist
                                  select new DropdownViewModel()
                                  {
                                      Id = c.Id,
                                      Code = c.Code,
                                      ParentId = c.ParentId,
                                      Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                  }).ToList();
                AddSectionChildForDropDownForTreeStructure(deparmentlist, vm.Departments, UnitType.Department);
            }

            return new ServiceResult<ReportDataVisibilityViewModel>()
            {
                Data = vm,
                Status = ResultStatus.Ok
            };
        }


        [HttpGet]
        public ServiceResult<ReportSerachViewModel> GetDirectorateByDepartment(string id)
        {
            return new ServiceResult<ReportSerachViewModel>()
            {
                Data = GetFilteredUniTypeWithEmployeeList(id, UnitType.Directorate),
                Status = ResultStatus.Ok
            };
        }

        [HttpGet]
        public ServiceResult<ReportSerachViewModel> GetSectionsByDirectorate(string id)
        {
            return new ServiceResult<ReportSerachViewModel>()
            {
                Data = GetFilteredUniTypeWithEmployeeList(id, UnitType.Section),
                Status = ResultStatus.Ok
            };
        }
        [HttpGet]
        public ServiceResult<ReportSerachViewModel> GetUnitsBySections(string id)
        {
            return new ServiceResult<ReportSerachViewModel>()
            {
                Data = GetFilteredUniTypeWithEmployeeList(id, UnitType.Unit),
                Status = ResultStatus.Ok
            };

        }
        public ReportSerachViewModel GetFilteredUniTypeWithEmployeeList(string id, UnitType unitType)
        {
            SSection sectionServices = new SSection();
            string[] fileteredParams = id.Split(',');

            List<string> filterEmptyParams = new List<string>();
            foreach (var item in fileteredParams)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    filterEmptyParams.Add(item);
                }


            }
            fileteredParams = filterEmptyParams.ToArray();
            string language = RiddhaSession.Language.ToString();
            var list = (from c in sectionServices.List().Data.Where(x => x.UnitType == unitType).ToList()
                        join d in fileteredParams on c.ParentId equals int.Parse(d)
                        select c).ToList();
            var checkedList = (from c in list
                               select new DropdownViewModel
                               {
                                   Id = c.Id,
                                   Code = c.Code,
                                   Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                   ParentId = c.ParentId,
                                   UnitType=(int)c.UnitType
                               }).ToList();
            //AddSectionChildForDropDown(list, dectoriatesByDepartment, UnitType.Directorate);
            AddSectionChildForDropDownForTreeStructure(list, checkedList, unitType);
            ReportSerachViewModel result = new ReportSerachViewModel()
            {
                CheckList = checkedList,
                EmployeeList = GetEmployeesForReport(fileteredParams),
            };
            return result;


        }
        public List<EmployeeGridVm> GetEmployeesForReport(string[] filteredParam, int employyeId = 0)
        {
            string language = RiddhaSession.Language.ToString();
            SEmployee _employeeServices = new SEmployee();
            var employees = (from c in _employeeServices.List().Data.ToList()
                             join d in filteredParam on c.SectionId equals int.Parse(d)
                             select new EmployeeGridVm
                             {
                                 Id = c.Id,
                                 Name = c.Code + "-" + (language == "ne" && string.IsNullOrEmpty(c.NameNp) == false ? c.NameNp : c.Name),
                                 DesignationLevel = c.Designation.DesignationLevel,
                                 EmployeeName = c.Name,
                                 EmployeeCode = c.Code,
                                 DesignationName = c.Designation.DesignationLevel.ToString().Length == 1 ? "0" + c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name : c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name,
                                 UnitType = c.Section.Name.ToString()//unit type change in unit name
                             }).OrderBy(x => x.DesignationName).ThenBy(x => x.EmployeeName).ToList();
            if (employyeId != 0)
            {
                employees = employees.Where(x => x.Id == employyeId).ToList();
            }
            return employees;
        }
        public void AddSectionParentForDropDown(int sectionId, List<ESection> list, List<DropdownViewModel> dropDownList)
        {
            string language = RiddhaSession.Language.ToString();
            var parentlist = list.Where(x => x.Id == sectionId && x.BranchId == RiddhaSession.BranchId).ToList();
            var addList = (from c in parentlist
                           select new DropdownViewModel
                           {
                               Id = c.Id,
                               Code = c.Code,
                               Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                               ParentId = c.ParentId,
                               UnitType = (int)c.UnitType
                           }).ToList();
            dropDownList.AddRange(addList);
            //AddSectionChildForDropDownForTreeStructure(parentlist, dropDownList);

        }
        public void AddSectionChildForDropDown(List<ESection> list, List<DropdownViewModel> dropdownList, UnitType unitType = 0)
        {
            string language = RiddhaSession.Language.ToString();

            foreach (var item in list)
            {
                var sections = _sectionServices.List().Data.Where(x => x.ParentId == item.Id).ToList();
                if (unitType != 0)
                {
                    sections = sections.Where(x => x.UnitType == unitType).ToList();
                }
                if (sections.Count() > 0)
                {
                    var addList = (from c in sections
                                   select new DropdownViewModel
                                   {
                                       Id = c.Id,
                                       Code = c.Code,
                                       Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,

                                   }).ToList();
                    dropdownList.AddRange(addList);
                    AddSectionChildForDropDown(sections, dropdownList, unitType);

                }

            }
        }
        public void AddSectionChildForDropDownForTreeStructure(List<ESection> list, List<DropdownViewModel> dropdownList, UnitType unitType = 0)
        {
            string language = RiddhaSession.Language.ToString();

            foreach (var item in list)
            {
                var sections = _sectionServices.List().Data.Where(x => x.ParentId == item.Id).ToList();
                if (unitType != 0)
                {
                    sections = sections.Where(x => x.UnitType == unitType).ToList();
                }

                if (sections.Count() > 0)
                {
                    var addchildList = (from c in sections
                                        select new DropdownViewModel
                                        {
                                            Id = c.Id,
                                            Code = c.Code,
                                            Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                            ParentId = c.ParentId,
                                            UnitType = (int)c.UnitType
                                        }).ToList();
                    var dropdownListParent = dropdownList.Where(x => x.Id == item.Id).FirstOrDefault();
                    dropdownListParent.Children = new List<DropdownViewModel>();
                    dropdownListParent.Children.AddRange(addchildList);
                    //dropdownList.AddRange(addList);
                    AddSectionChildForDropDownForTreeStructure(sections, dropdownListParent.Children, unitType);

                }





            }
        }
        public void AddSectionChildForDropDownForSearch(List<ESection> list, List<DropdownViewModel> dropdownList)
        {
            string language = RiddhaSession.Language.ToString();

            foreach (var item in list)
            {
                var sections = list.Where(x => x.ParentId == item.Id).ToList();

                if (sections.Count() > 0)
                {
                    var addchildList = (from c in sections
                                        select new DropdownViewModel
                                        {
                                            Code = c.Code,
                                            Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                            ParentId = c.ParentId,
                                            UnitType = (int)c.UnitType,
                                            Checked = true
                                        }).ToList();
                    var dropdownListParent = dropdownList.Where(x => x.Id == item.Id).FirstOrDefault();
                    dropdownListParent.Children = new List<DropdownViewModel>();
                    dropdownListParent.Children.AddRange(addchildList);
                    //dropdownList.AddRange(addList);
                    AddSectionChildForDropDownForSearch(sections, dropdownListParent.Children);
                }

            }
        }
        [HttpGet]
        public ServiceResult<List<EmployeeGridVm>> GetEmployeeByUnit(string id)
        {
            string[] fileteredParams = id.Split(',');
            List<string> filterEmptyParams = new List<string>();
            foreach (var item in fileteredParams)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    filterEmptyParams.Add(item);
                }
            }
            fileteredParams = filterEmptyParams.ToArray();
            return new ServiceResult<List<EmployeeGridVm>>()
            {
                Data = GetEmployeesForReport(fileteredParams),
                Status = ResultStatus.Ok,

            };
        }


        #endregion

        #region New Report 2021/11/16
        public ServiceResult<TreeStructureViewModel> GetTreeStructure()
        {
            SCompany _companyServices = new SCompany();
            SSection services = new SSection();
            int roleId = RiddhaSession.RoleId;
            string language = RiddhaSession.Language.ToString();
            TreeStructureViewModel vm = new TreeStructureViewModel();
            var dropDownList = new List<DropdownViewModel>();

            var list = services.List().Data.Where(x => x.BranchId == RiddhaSession.BranchId).ToList();
            var deparmentlist = list.Where(x => x.UnitType == UnitType.Department && x.ParentId == null).ToList();

            vm.IsSelf = false;
            if (roleId > 0)
            {
                DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
                switch (dataVisibilityLevel)
                {
                    case DataVisibilityLevel.Self:
                        List<string> listSection = new List<string>();
                        listSection.Add(RiddhaSession.SectionId.ToString());
                        ESection selfEmployeeList = services.List().Data.Where(x => x.Id == RiddhaSession.SectionId && x.BranchId == RiddhaSession.BranchId).FirstOrDefault();
                        string[] sections = listSection.ToArray();
                        vm.Employees = GetEmployeesForReport(sections, RiddhaSession.EmployeeId);
                        vm.IsSelf = true;
                        vm.TreeData = new List<DropdownViewModel>();
                        switch ((UnitType)selfEmployeeList.UnitType)
                        {
                            case UnitType.Department:
                                vm.UnitType = UnitType.Department.ToString();
                                vm.TreeData.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name,
                                    UnitType= (int)selfEmployeeList.UnitType
                                });

                                break;
                            case UnitType.Directorate:

                                vm.UnitType = UnitType.Directorate.ToString();
                                vm.TreeData.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name,
                                    UnitType = (int)selfEmployeeList.UnitType
                                });
                                break;
                            case UnitType.Section:

                                vm.UnitType = UnitType.Section.ToString();
                                vm.TreeData.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name,
                                    UnitType = (int)selfEmployeeList.UnitType
                                });
                                break;
                            case UnitType.Unit:
                                vm.UnitType = UnitType.Unit.ToString();
                                vm.TreeData.Add(new DropdownViewModel()
                                {
                                    Id = selfEmployeeList.Id,
                                    Code = selfEmployeeList.Code,
                                    Name = selfEmployeeList.Name,
                                    UnitType = (int)selfEmployeeList.UnitType
                                });
                                break;
                            default:

                                break;
                        }
                        break;

                    case DataVisibilityLevel.Department:
                        list = deparmentlist;
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.TreeData = dropDownList;
                        vm.UnitType = UnitType.Department.ToString();
                        break;
                    case DataVisibilityLevel.Directorate:
                        list = list.Where(x => x.UnitType == UnitType.Directorate).ToList();
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.TreeData = dropDownList;
                        vm.UnitType = UnitType.Directorate.ToString();
                        break;
                    case DataVisibilityLevel.Section:
                        list = list.Where(x => x.UnitType == UnitType.Section).ToList();
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.TreeData = dropDownList;
                        vm.UnitType = UnitType.Section.ToString();
                        break;
                    case DataVisibilityLevel.Unit:
                        list = list.Where(x => x.UnitType == UnitType.Unit).ToList();
                        AddSectionParentForDropDown(RiddhaSession.SectionId, list, dropDownList);
                        vm.TreeData = dropDownList;
                        vm.UnitType = UnitType.Unit.ToString();
                        break;
                    case DataVisibilityLevel.All:
                        vm.UnitType = _companyServices.GetCompanyName(RiddhaSession.CompanyId);
                        vm.TreeData = (from c in deparmentlist
                                       select new DropdownViewModel()
                                       {
                                           Id = c.Id,
                                           Code = c.Code,
                                           ParentId = c.ParentId,
                                           Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                           UnitType = (int)c.UnitType,
                                       }).ToList();

                        //AddSectionChildForDropDownForTreeStructure(deparmentlist, vm.TreeData);
                        break;
                    default:
                        vm.UnitType = _companyServices.GetCompanyName(RiddhaSession.CompanyId);
                        vm.TreeData = (from c in deparmentlist
                                       select new DropdownViewModel()
                                       {
                                           Id = c.Id,
                                           Code = c.Code,
                                           ParentId = c.ParentId,
                                           Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                           UnitType = (int)c.UnitType
                                       }).ToList();

                        //AddSectionChildForDropDownForTreeStructure(deparmentlist, vm.TreeData);
                        break;
                }
            }
            else
            {
                vm.UnitType = _companyServices.GetCompanyName(RiddhaSession.CompanyId);
                vm.TreeData = (from c in deparmentlist
                               select new DropdownViewModel()
                               {
                                   Id = c.Id,
                                   Code = c.Code,
                                   ParentId = c.ParentId,
                                   Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                   UnitType = (int)c.UnitType
                               }).ToList();
                //AddSectionChildForDropDownForTreeStructure(deparmentlist, vm.TreeData);
            }
            vm.TreeData.OrderBy(x => x.UnitType).ToList();
            return new ServiceResult<TreeStructureViewModel>()
            {
                Data = vm,
                Status = ResultStatus.Ok
            };
        }
        public ServiceResult<List<DropdownViewModel>> GetChildSectionFromParentId(int parentId)
        {
            var section = (from c in _sectionServices.List().Data.Where(x => x.ParentId == parentId)
                           select new DropdownViewModel()
                           {
                               Name = c.Name,
                               Code = c.Code,
                               Id = c.Id,
                               ParentId = c.ParentId,
                               UnitType=(int)c.UnitType
                           }).ToList();
            return new ServiceResult<List<DropdownViewModel>>()
            {
                Data = section,
                Message = "",
                Status = ResultStatus.Ok
            };
        }
        #endregion
        [HttpGet]
        public ServiceResult<List<SectionGridVm>> GetSectionsByDepartment(string id)
        {
            SSection sectionServices = new SSection();
            string[] departments = id.Split(',');

            string language = RiddhaSession.Language.ToString();
            if (RiddhaSession.RoleId == 0)
            {
                sectionsbyDepartment = (from c in sectionServices.List().Data.Where(x => x.Branch.CompanyId == RiddhaSession.CompanyId).ToList()
                                            //where c.BranchId == RiddhaSession.BranchId
                                        join d in departments on c.DepartmentId equals int.Parse(d)
                                        select new SectionGridVm
                                        {
                                            Id = c.Id,
                                            Code = c.Code,
                                            //Name = c.Name,
                                            //NameNp = c.NameNp,
                                            Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                            BranchId = c.BranchId,
                                            DepartmentId = c.DepartmentId,
                                            ParentId = c.ParentId

                                        }).ToList();
                AddSectionChild(sectionsbyDepartment);
            }
            else
            {
                DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
                switch (dataVisibilityLevel)
                {
                    case DataVisibilityLevel.Self:
                    case DataVisibilityLevel.Unit:


                        AddSectionParent(RiddhaSession.SectionId);

                        break;
                    case DataVisibilityLevel.Department:
                    case DataVisibilityLevel.Branch:
                    case DataVisibilityLevel.ReportingHierarchy:
                    case DataVisibilityLevel.All:
                        sectionsbyDepartment = (from c in sectionServices.List().Data.ToList()
                                                where c.BranchId == RiddhaSession.BranchId
                                                join d in departments on c.DepartmentId equals int.Parse(d)
                                                select new SectionGridVm
                                                {
                                                    Id = c.Id,
                                                    Code = c.Code,
                                                    //Name = c.Name,
                                                    //NameNp = c.NameNp,
                                                    Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                                    BranchId = c.BranchId,
                                                    DepartmentId = c.DepartmentId,
                                                }).ToList();
                        break;
                    default:
                        break;
                }
            }


            return new ServiceResult<List<SectionGridVm>>()
            {
                Data = sectionsbyDepartment,
                Status = ResultStatus.Ok
            };
        }

        [HttpGet]
        public ServiceResult<List<EmployeeGridVm>> GetEmployeeBySection(string id, int activeInactiveMode)
        {
            int? branchId = RiddhaSession.BranchId;
            string language = RiddhaSession.Language.ToString();
            SEmployee employeeServices = new SEmployee();
            List<EmployeeGridVm> employee = new List<EmployeeGridVm>();
            var empLst = Common.GetEmployees().Data;
            if (RiddhaSession.PackageId > 0 && activeInactiveMode == 0)
            {
                empLst = empLst.Where(x => x.EmploymentStatus == EmploymentStatus.NormalEmployment || x.EmploymentStatus == EmploymentStatus.OnContract || x.EmploymentStatus == EmploymentStatus.PermanentJob || x.EmploymentStatus == EmploymentStatus.Retiring).ToList();
            }
            //Changes For Report Search Parameter..
            string[] sections = id.Split(',');
            if (RiddhaSession.RoleId == 0)
            {
                employee = (from c in empLst
                            join d in sections on c.SectionId equals int.Parse(d)
                            select new EmployeeGridVm
                            {
                                Id = c.Id,
                                Name = c.Code + "-" + (language == "ne" && string.IsNullOrEmpty(c.NameNp) == false ? c.NameNp : c.Name),
                                DesignationLevel = c.Designation.DesignationLevel,
                                EmployeeName = c.Name,
                                EmployeeCode = c.Code,
                                DesignationName = c.Designation.DesignationLevel.ToString().Length == 1 ? "0" + c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name : c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name
                            }).OrderBy(x => x.DesignationName).ThenBy(x => x.EmployeeName).ToList();

            }
            else
            {
                DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
                switch (dataVisibilityLevel)
                {
                    case DataVisibilityLevel.Self:
                        employee = (from c in empLst.Where(x => x.Id == RiddhaSession.EmployeeId).ToList()
                                    select new EmployeeGridVm
                                    {
                                        Id = c.Id,
                                        Name = c.Code + "-" + (language == "ne" && string.IsNullOrEmpty(c.NameNp) == false ? c.NameNp : c.Name),
                                        DesignationLevel = c.Designation.DesignationLevel,
                                        EmployeeName = c.Name,
                                        EmployeeCode = c.Code,
                                        DesignationName = c.Designation.DesignationLevel.ToString().Length == 1 ? "0" + c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name : c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name
                                    }).OrderBy(x => x.DesignationName).ThenBy(x => x.EmployeeName).ToList();

                        break;
                    case DataVisibilityLevel.Unit:
                    case DataVisibilityLevel.Department:
                    case DataVisibilityLevel.Branch:
                    case DataVisibilityLevel.ReportingHierarchy:
                    case DataVisibilityLevel.All:
                        employee = (from c in empLst
                                    join d in sections on c.SectionId equals int.Parse(d)
                                    select new EmployeeGridVm
                                    {
                                        Id = c.Id,
                                        Name = c.Code + "-" + (language == "ne" && string.IsNullOrEmpty(c.NameNp) == false ? c.NameNp : c.Name),
                                        DesignationLevel = c.Designation.DesignationLevel,
                                        EmployeeName = c.Name,
                                        EmployeeCode = c.Code,
                                        DesignationName = c.Designation.DesignationLevel.ToString().Length == 1 ? "0" + c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name : c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name
                                    }).OrderBy(x => x.DesignationName).ThenBy(x => x.EmployeeName).ToList();

                        break;
                    default:
                        break;
                }
            }
            return new ServiceResult<List<EmployeeGridVm>>()
            {
                Data = employee,
                Status = ResultStatus.Ok
            };
        }



        public void AddSectionParent(int sectionId)
        {
            string language = RiddhaSession.Language.ToString();
            var list = _sectionServices.List().Data.Where(x => (x.Id == sectionId) && x.BranchId == RiddhaSession.BranchId).ToList();
            var addList = (from c in list
                           select new SectionGridVm
                           {
                               Id = c.Id,
                               Code = c.Code,

                               Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                               BranchId = c.BranchId,
                               DepartmentId = c.DepartmentId,
                               ParentId = c.ParentId
                           }).ToList();
            sectionsbyDepartment.AddRange(addList);
            AddSectionChild(list);
        }
        public void AddSectionChild(List<ESection> list)
        {
            string language = RiddhaSession.Language.ToString();
            foreach (var item in list)
            {
                var sections = _sectionServices.List().Data.Where(x => x.ParentId == item.Id).ToList();
                if (sections.Count() > 0)
                {
                    var addList = (from c in sections
                                   select new SectionGridVm
                                   {
                                       Id = c.Id,
                                       Code = c.Code,
                                       Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                       BranchId = c.BranchId,
                                       DepartmentId = c.DepartmentId,
                                       ParentId = c.ParentId
                                   }).ToList();
                    sectionsbyDepartment.AddRange(addList);
                    AddSectionChild(sections);

                }


            }


        }

        public void AddSectionChild(List<SectionGridVm> list)
        {
            string language = RiddhaSession.Language.ToString();
            foreach (var item in list)
            {
                var sections = _sectionServices.List().Data.Where(x => x.ParentId == item.Id).ToList();
                if (sections.Count() > 0)
                {
                    var addList = (from c in sections
                                   select new SectionGridVm
                                   {
                                       Id = c.Id,
                                       Code = c.Code,
                                       Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                       BranchId = c.BranchId,
                                       DepartmentId = c.DepartmentId,
                                       ParentId = c.ParentId
                                   }).ToList();
                    sectionsbyDepartment.AddRange(addList);
                    AddSectionChild(sections);

                }


            }


        }
        [HttpGet]
        public ServiceResult<List<EmployeeGridVm>> GetAllEmployees()
        {
            int branchId = RiddhaSession.BranchId ?? 0;
            SEmployee employeeServices = new SEmployee();
            var employee = (from c in employeeServices.List().Data.Where(x => x.BranchId == branchId)
                            select new EmployeeGridVm()
                            {
                                Id = c.Id,
                                Name = c.Name
                            }).ToList();
            return new ServiceResult<List<EmployeeGridVm>>()
            {
                Data = employee.OrderBy(x => x.Name).ToList(),
                Message = "",
                Status = ResultStatus.Ok
            };

        }

        [HttpGet]
        public ServiceResult<List<string>> GetEnglishMonths()
        {
            string lang = RiddhaSession.Language;
            return new ServiceResult<List<string>>()
            {
                Data = dateTableServices.GetEnglishMonths(lang),
                Status = ResultStatus.Ok
            };
        }
        [HttpGet]
        public ServiceResult<List<string>> GetNepaliMonths()
        {
            string lang = RiddhaSession.Language;
            return new ServiceResult<List<string>>()
            {
                Data = dateTableServices.GetNepaliMonths(lang),
                Status = ResultStatus.Ok
            };
        }
        #region OLDSEARCH
        //[HttpGet]
        //public ServiceResult<EmployeeComputerCodeSearchVm> SearchEmployeeByComputerCode(int code)
        //{
        //    string language = RiddhaSession.Language.ToString();
        //    SEmployee sEmployee = new SEmployee();
        //    var employee = sEmployee.List().Data.Where(x => x.DeviceCode == code).FirstOrDefault();
        //    var empList = sEmployee.List().Data.Where(x => x.DeviceCode == code).ToList();
        //    if (employee != null)
        //    {
        //        var section = _sectionServices.List().Data.Where(x => x.Id == employee.SectionId).FirstOrDefault();
        //        var sectionTolist = _sectionServices.List().Data.ToList();
        //        List<UnitLevelHierarchy> unitLevel = GetUnitLevelHierarchies(section.ParentId ?? 0, section.Id);


        //        EmployeeComputerCodeSearchVm vm = new EmployeeComputerCodeSearchVm();

        //        foreach (var item in unitLevel)
        //        {
        //            switch (item.UnitTypeId)
        //            {
        //                case UnitType.Department:


        //                    vm.Departments = (from c in sectionTolist.Where(x => x.Id == item.Id)
        //                                      select new CheckBoxModel()
        //                                      {
        //                                          Id = c.Id,
        //                                          Name = c.Name,
        //                                          Checked = true
        //                                      }).ToList();
        //                    break;
        //                case UnitType.Directorate:

        //                    vm.Directorates = (from c in sectionTolist.Where(x => x.Id == item.Id)
        //                                       select new CheckBoxModel()
        //                                       {
        //                                           Id = c.Id,
        //                                           Name = c.Name,
        //                                           Checked = true
        //                                       }).ToList();
        //                    break;
        //                case UnitType.Section:

        //                    vm.Sections = (from c in sectionTolist.Where(x => x.Id == item.Id)
        //                                   select new CheckBoxModel()
        //                                   {
        //                                       Id = c.Id,
        //                                       Name = c.Name,
        //                                       Checked = true
        //                                   }).ToList();
        //                    break;
        //                case UnitType.Unit:

        //                    vm.Units = (from c in sectionTolist.Where(x => x.Id == item.Id)
        //                                select new CheckBoxModel()
        //                                {
        //                                    Id = c.Id,
        //                                    Name = c.Name,
        //                                    Checked = true
        //                                }).ToList();
        //                    break;
        //                default:
        //                    break;
        //            }


        //        }
        //        try
        //        {
        //            vm.Employees = (from c in empList
        //                            select new EmployeeGridVm()
        //                            {
        //                                Id = c.Id,
        //                                Name = c.Code + "-" + (language == "ne" && string.IsNullOrEmpty(c.NameNp) == false ? c.NameNp : c.Name),
        //                                DesignationLevel = c.Designation.DesignationLevel,
        //                                EmployeeName = c.Name,
        //                                EmployeeCode = c.Code,
        //                                DesignationName = c.Designation.DesignationLevel.ToString().Length == 1 ? "0" + c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name : c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name,
        //                                Checked = true
        //                            }).ToList();
        //        }
        //        catch (Exception ex)
        //        {

        //            throw;
        //        }



        //        return new ServiceResult<EmployeeComputerCodeSearchVm>()
        //        {
        //            Data = vm,
        //            Message = "",
        //            Status = ResultStatus.Ok
        //        };
        //    }
        //    return new ServiceResult<EmployeeComputerCodeSearchVm>()
        //    {
        //        Data = null,
        //        Message = "Employee not found.",
        //        Status = ResultStatus.processError,
        //    };

        //}

        #endregion
        [HttpGet]
        public ServiceResult<EmployeeComputerCodeSearchVm> SearchEmployeeByComputerCode(int code)
        {
            string language = RiddhaSession.Language.ToString();
            SEmployee sEmployee = new SEmployee();
            var employee = sEmployee.List().Data.Where(x => x.DeviceCode == code).FirstOrDefault();
            var empList = sEmployee.List().Data.Where(x => x.DeviceCode == code).ToList();
            if (employee != null)
            {
                var section = _sectionServices.List().Data.Where(x => x.Id == employee.SectionId).FirstOrDefault();
                var sectionTolist = _sectionServices.List().Data.ToList();
                List<UnitLevelHierarchy> unitLevel = GetUnitLevelHierarchies(section.ParentId ?? 0, section.Id);


                EmployeeComputerCodeSearchVm vm = new EmployeeComputerCodeSearchVm();
                vm.Departments = new List<DropdownViewModel>();
                unitLevel = unitLevel.OrderBy(x => x.UnitTypeId).ToList();

                var parent = (from c in unitLevel.ToList()
                              select new DropdownViewModel()
                              {
                                  Id = c.Id,
                                  Checked = true,
                                  Name = c.Name
                              }).FirstOrDefault();
                vm.Departments.Add(parent);

                var sectionList = (from c in unitLevel
                                   select new ESection()
                                   {
                                       Id = c.Id,
                                       Name = c.Name,
                                       ParentId = c.ParentId,
                                   }).ToList();

                AddSectionChildForDropDownForSearch(sectionList, vm.Departments);

                try
                {
                    vm.Employees = (from c in empList
                                    select new EmployeeGridVm()
                                    {
                                        Id = c.Id,
                                        Name = c.Code + "-" + (language == "ne" && string.IsNullOrEmpty(c.NameNp) == false ? c.NameNp : c.Name),
                                        DesignationLevel = c.Designation.DesignationLevel,
                                        EmployeeName = c.Name,
                                        EmployeeCode = c.Code,
                                        DesignationName = c.Designation.DesignationLevel.ToString().Length == 1 ? "0" + c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name : c.Designation.DesignationLevel.ToString() + "-" + c.Designation.Name,
                                        Checked = true
                                    }).ToList();
                }

                catch (Exception ex)
                {

                    throw;
                }


                return new ServiceResult<EmployeeComputerCodeSearchVm>()
                {
                    Data = vm,
                    Message = "",
                    Status = ResultStatus.Ok
                };
            }
            return new ServiceResult<EmployeeComputerCodeSearchVm>()
            {
                Data = null,
                Message = "Employee not found.",
                Status = ResultStatus.processError,
            };

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
                UnitType = Enum.GetName(typeof(UnitType), unit.UnitType),
                UnitTypeId = unit.UnitType,
                ParentId = unit.ParentId
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
                    UnitType = Enum.GetName(typeof(UnitType), unitlevel_query.UnitType),
                    UnitTypeId = unitlevel_query.UnitType
                });
                AddParent(hierarchies, unitlevel_query.ParentId ?? 0, order + 1);
            };
        }



    }

    public class KendoReportViewModel : KendoPageListArguments
    {
        public string FISCALYEAR { get; set; }
        public string BranchIds { get; set; }
        public string EmpIds { get; set; }
        public string DeptIds { get; set; }
        public string SectionIds { get; set; }
        public string CourseIds { get; set; }
        public string LeaveMasterIds { get; set; }
        public string OnDate { get; set; }
        public string ToDate { get; set; }
        public int DayWise { get; set; }
        public int Year { get; set; }
        public int MonthId { get; set; }
        public int ToMonthId { get; set; }
        public int FiscalYearId { get; set; }
        public int ActiveInactiveMode { get; set; }
        public bool IncludePunchTime { get; set; }
        public bool OTV2 { get; set; }
        public int Type { get; set; }
    }

    public class EmployeeComputerCodeSearchVm
    {
        public List<DropdownViewModel> Departments { get; set; }
        public List<CheckBoxModel> Directorates { get; set; }
        public List<CheckBoxModel> Sections { get; set; }
        public List<CheckBoxModel> Units { get; set; }
        public List<EmployeeGridVm> Employees { get; set; }
    }

    public class TreeStructureViewModel
    {
        public List<DropdownViewModel> TreeData { get; set; }
        public List<EmployeeGridVm> Employees { get; set; }
        public bool IsSelf { get; set; }
        public string UnitType { get; set; }
        public string UnitTypeName { get { return this.UnitType.ToString(); } }
    }
}
