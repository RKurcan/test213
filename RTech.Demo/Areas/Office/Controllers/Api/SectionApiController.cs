using Riddhasoft.Entity.User;
using Riddhasoft.OfficeSetup.Entities;
using Riddhasoft.OfficeSetup.Services;
using Riddhasoft.Services.Common;
using Riddhasoft.User.Entity;
using RTech.Demo.Areas.Report.Controllers.Api;
using RTech.Demo.Filters;
using RTech.Demo.Models;
using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace RTech.Demo.Areas.Office.Controllers.Api
{
    public class SectionApiController : ApiController
    {
        SSection sectionServices = null;
        LocalizedString loc = null;
        List<SectionGridVm> sectionsbyDepartment = new List<SectionGridVm>();
        List<DropdownViewModel> ParentLists = new List<DropdownViewModel>();

        public SectionApiController()
        {
            sectionServices = new SSection();
            loc = new LocalizedString();
        }
        [ActionFilter("1043")]
        public ServiceResult<List<SectionGridVm>> Get(int branchId)
        {
            SSection service = new SSection();
            List<ESection> list = new List<ESection>();
            if (branchId == 0)
            {
                var branch = new SBranch().List().Data.Where(x => x.Id == RiddhaSession.BranchId).FirstOrDefault();
                if (branch.IsHeadOffice)
                {
                    list = service.List().Data.Where(x => x.Branch.CompanyId == RiddhaSession.CompanyId).ToList();
                }
                else
                {
                    list = service.List().Data.Where(x => x.BranchId == RiddhaSession.BranchId).ToList();
                }
            }
            else
            {
                list = service.List().Data.Where(x => x.BranchId == branchId).ToList();
            }
            var sectionLst = (from c in list
                              select new SectionGridVm()
                              {
                                  Id = c.Id,
                                  BranchId = c.BranchId,
                                  DepartmentId = c.DepartmentId,
                                  Code = c.Code,
                                  Name = c.Name,
                                  NameNp = c.NameNp,
                                  ParentId = c.ParentId,
                                  UnitCode = c.UnitCode
                              }).ToList();

            return new ServiceResult<List<SectionGridVm>>()
            {
                Data = sectionLst,
                Status = ResultStatus.Ok
            };
        }


        [HttpGet]
        public ServiceResult<List<SectionGridVm>> GetSectionsByDepartment(int id)
        {
            SSection sectionServices = new SSection();


            string language = RiddhaSession.Language.ToString();
            if (RiddhaSession.RoleId == 0)
            {
                sectionsbyDepartment = (from c in sectionServices.List().Data.Where(x => x.Branch.CompanyId == RiddhaSession.CompanyId && x.DepartmentId == id).ToList()


                                        select new SectionGridVm
                                        {
                                            Id = c.Id,
                                            Code = c.Code,
                                            Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                            BranchId = c.BranchId,
                                            DepartmentId = c.DepartmentId,

                                        }).ToList();
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
                        sectionsbyDepartment = (from c in sectionServices.List().Data.Where(x => x.Branch.CompanyId == RiddhaSession.CompanyId && x.DepartmentId == id).ToList()


                                                select new SectionGridVm
                                                {
                                                    Id = c.Id,
                                                    Code = c.Code,
                                                    Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                                    BranchId = c.BranchId,
                                                    DepartmentId = c.DepartmentId,

                                                }).ToList();
                        break;
                    case DataVisibilityLevel.Branch:
                    case DataVisibilityLevel.ReportingHierarchy:
                    case DataVisibilityLevel.All:
                        sectionsbyDepartment = (from c in sectionServices.List().Data.ToList()
                                                where c.BranchId == RiddhaSession.BranchId
                                                && c.DepartmentId == id
                                                select new SectionGridVm
                                                {
                                                    Id = c.Id,
                                                    Code = c.Code,
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

        public void AddSectionParent(int sectionId)
        {
            string language = RiddhaSession.Language.ToString();
            var list = sectionServices.List().Data.Where(x => (x.Id == sectionId) && x.BranchId == RiddhaSession.BranchId).ToList();
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
                var sections = sectionServices.List().Data.Where(x => x.ParentId == item.Id).ToList();
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
        public ServiceResult<List<SectionGridVm>> GetSections(int id)
        {

            List<SectionGridVm> sections = (from c in sectionServices.List().Data.Where(x => x.DepartmentId == id).ToList()
                                                //where c.BranchId == RiddhaSession.BranchId
                                            select new SectionGridVm
                                            {
                                                Id = c.Id,
                                                Code = c.Code,
                                                Name = c.Name,
                                                NameNp = c.NameNp,
                                                BranchId = c.BranchId,
                                                DepartmentId = c.DepartmentId,
                                                UnitType = c.UnitType,
                                                ParentId = c.ParentId
                                            }).ToList();
            return new ServiceResult<List<SectionGridVm>>()
            {
                Data = sections,
                Status = ResultStatus.Ok
            };
        }
        public ServiceResult<SectionGridVm> GetSection(int id)
        {

            var section = (from c in sectionServices.List().Data.Where(x => x.Id == id).ToList()
                           select new SectionGridVm
                           {
                               Id = c.Id,
                               Code = c.Code,
                               Name = c.Name,
                               NameNp = c.NameNp,
                               BranchId = c.BranchId,
                               DepartmentId = c.DepartmentId,
                               UnitType = c.UnitType,
                               ParentId = c.ParentId
                           }).FirstOrDefault();
            return new ServiceResult<SectionGridVm>()
            {
                Data = section,
                Status = ResultStatus.Ok
            };
        }
        [ActionFilter("1022")]
        public ServiceResult<ESection> Post(ESection model)
        {
            if (model.BranchId == 0)
            {
                model.BranchId = RiddhaSession.BranchId;
            }
            //model.BranchId = RiddhaSession.BranchId;

            SDepartment _departmentServices = new SDepartment();

            var defaultDepartment = _departmentServices.List().Data.Where(x => x.Code.ToLower().Trim() == "nphq").FirstOrDefault();
            if (defaultDepartment != null)
            {
                model.DepartmentId = defaultDepartment.Id;
            }
            else
            {
                return new ServiceResult<ESection>()
                {
                    Data = null,
                    Message = "Default Department not set",
                    Status = ResultStatus.processError
                };
            }
            var result = sectionServices.Add(model);
            if (result.Status == ResultStatus.Ok)
            {
                Common.AddAuditTrail("1006", "1022", RiddhaSession.CurDate.ToDateTime(), RiddhaSession.UserId, model.Id, loc.Localize(result.Message));
            }
            return new ServiceResult<ESection>()
            {
                Data = result.Data,
                Message = loc.Localize(result.Message),
                Status = result.Status
            };
        }
        [ActionFilter("1023")]
        public ServiceResult<ESection> Put(ESection model)
        {
            SDepartment _departmentServices = new SDepartment();
            model.BranchId = RiddhaSession.BranchId;
            var defaultDepartment = _departmentServices.List().Data.Where(x => x.Code.ToLower().Trim() == "nphq").FirstOrDefault();
            if (defaultDepartment != null)
            {
                model.DepartmentId = defaultDepartment.Id;
            }
            else
            {
                return new ServiceResult<ESection>()
                {
                    Data = null,
                    Message = "Default Department not set",
                    Status = ResultStatus.processError
                };
            }
            var result = sectionServices.Update(model);
            if (result.Status == ResultStatus.Ok)
            {
                Common.AddAuditTrail("1006", "1023", RiddhaSession.CurDate.ToDateTime(), RiddhaSession.UserId, model.Id, loc.Localize(result.Message));
            }
            return new ServiceResult<ESection>()
            {
                Data = result.Data,
                Status = result.Status,
                Message = loc.Localize(result.Message)
            };
        }
        [HttpDelete, ActionFilter("1024")]
        public ServiceResult<int> Delete(int id)
        {
            var section = sectionServices.List().Data.Where(x => x.Id == id).FirstOrDefault();
            var count = sectionServices.List().Data.Where(x => x.ParentId == id).Count();
            if (count != 0)
            {
                return new ServiceResult<int>()
                {
                    Data = 0,
                    Status = ResultStatus.processError,
                    Message = section.UnitType.ToString()+ " has "+count+" child, cannot deleted."
                };
            }

   
            var result = sectionServices.Remove(section);
            if (result.Status == ResultStatus.Ok)
            {
                Common.AddAuditTrail("1006", "1024", RiddhaSession.CurDate.ToDateTime(), RiddhaSession.UserId, id, loc.Localize(result.Message));
            }
            return new ServiceResult<int>()
            {
                Data = result.Data,
                Status = result.Status,
                Message = loc.Localize(result.Message)
            };
        }



        [HttpGet]
        public ServiceResult<List<DropdownViewModel>> GetParentList(int unitType)
        {
            SSection sectionServices = new SSection();

            string language = RiddhaSession.Language.ToString();

            var list = sectionServices.List().Data;
            switch (unitType)
            {
                case 1:
                    list = list.Where(x => x.UnitType == UnitType.Department);
                    break;
                case 2:
                    list = list.Where(x => x.UnitType == UnitType.Directorate || x.UnitType == UnitType.Department);
                    break;
                case 3:
                    list = list.Where(x => x.UnitType == UnitType.Section || x.UnitType == UnitType.Directorate || x.UnitType == UnitType.Department);
                    break;
                case 4:
                default:

                    break;
            }

            if (RiddhaSession.RoleId == 0)
            {
                ParentLists = (from c in list.Where(x => x.Branch.CompanyId == RiddhaSession.CompanyId).ToList()
                               select new DropdownViewModel
                               {
                                   Id = c.Id,
                                   Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                               }).ToList();
            }
            else
            {
                DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;
                switch (dataVisibilityLevel)
                {
                    case DataVisibilityLevel.Self:

                    case DataVisibilityLevel.Department:

                        AddSectionParentForParentList(RiddhaSession.SectionId, list.ToList());

                        break;
                    case DataVisibilityLevel.Directorate:


                        AddSectionParentForParentList(RiddhaSession.SectionId, list.ToList());
                        break;
                    case DataVisibilityLevel.Section:


                        AddSectionParentForParentList(RiddhaSession.SectionId, list.ToList());
                        break;
                    case DataVisibilityLevel.Unit:

                        AddSectionParentForParentList(RiddhaSession.SectionId, list.ToList());

                        break;
                    case DataVisibilityLevel.Branch:
                    case DataVisibilityLevel.ReportingHierarchy:
                    case DataVisibilityLevel.All:
                        ParentLists = (from c in list.ToList()
                                       where c.BranchId == RiddhaSession.BranchId
                                       select new DropdownViewModel
                                       {
                                           Id = c.Id,
                                           Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                       }).ToList();
                        break;
                    default:
                        break;
                }
            }

            return new ServiceResult<List<DropdownViewModel>>()
            {
                Data = ParentLists,
                Status = ResultStatus.Ok
            };
        }

        public void AddSectionParentForParentList(int sectionId, List<ESection> sectionList)
        {
            string language = RiddhaSession.Language.ToString();
            var list = sectionList.Where(x => (x.Id == sectionId) && x.BranchId == RiddhaSession.BranchId).ToList();
            var addList = (from c in list
                           select new DropdownViewModel
                           {
                               Id = c.Id,
                               Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,

                           }).ToList();
            ParentLists.AddRange(addList);
            AddSectionChildForParentList(list);
        }

        public void AddSectionChildForParentList(List<ESection> list)
        {
            string language = RiddhaSession.Language.ToString();
            foreach (var item in list)
            {
                var sections = sectionServices.List().Data.Where(x => x.ParentId == item.Id).ToList();
                if (sections.Count() > 0)
                {
                    var addList = (from c in sections
                                   select new DropdownViewModel
                                   {
                                       Id = c.Id,
                                       Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,

                                   }).ToList();
                    ParentLists.AddRange(addList);
                    AddSectionChildForParentList(sections);

                }

            }

        }
        public ServiceResult<List<DepartmentGridVm>> GetDepartmentsForDropdown(int branchId)
        {
            SDepartment services = new SDepartment();
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
                        departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId && x.Id == RiddhaSession.DepartmentId)
                                         select new DepartmentGridVm()
                                         {
                                             Id = c.Id,
                                             BranchId = c.BranchId,
                                             Code = c.Code,
                                             Name = language == "ne" && c.NameNp != null ? c.NameNp : c.Name,
                                             NumberOfStaff = c.NumberOfStaff
                                         }).ToList();
                        break;
                    case DataVisibilityLevel.Department:
                        departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId && x.Id == RiddhaSession.DepartmentId)
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
                    case DataVisibilityLevel.ReportingHierarchy:
                    case DataVisibilityLevel.All:
                        departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId)
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
                departmentLst = (from c in services.List().Data.Where(x => x.BranchId == branchId)
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

        public ServiceResult<DropdownViewModel> GetUnitsByCode(string code)
        {
            var section = sectionServices.List().Data.Where(x => x.Code.ToLower().Trim() == code.ToLower().Trim()).FirstOrDefault();
            if (section != null)
            {
                DropdownViewModel vm = new DropdownViewModel()
                {
                    Id = section.Id,
                    Code = section.Code,
                    Name = section.Name
                };
                return new ServiceResult<DropdownViewModel>()
                {
                    Data = vm,
                    Status = ResultStatus.Ok,

                };
            }
            else
            {
                return new ServiceResult<DropdownViewModel>()
                {
                    Data = null,
                    Status = ResultStatus.processError,
                    Message = "Code Not Found",
                };
            }
        }

        [HttpPost, ActionFilter("1043")]
        public KendoGridResult<List<SectionGridVm>> GetSectionKendoGrid(KendoPageListArguments arg)
        {
            string language = RiddhaSession.Language.ToString();
            var branchId = RiddhaSession.BranchId;
            SSection sectionServices = new SSection();
            IQueryable<ESection> sectionQuery;
            if (arg.BranchId == 0)
            {
                var branch = new SBranch().List().Data.Where(x => x.Id == RiddhaSession.BranchId).FirstOrDefault();
                if (branch.IsHeadOffice)
                {
                    sectionQuery = sectionServices.List().Data.Where(x => x.Branch.CompanyId == RiddhaSession.CompanyId);
                }
                else
                {
                    sectionQuery = sectionServices.List().Data.Where(x => x.BranchId == RiddhaSession.BranchId);
                }
            }
            else
            {
                sectionQuery = sectionServices.List().Data.Where(x => x.BranchId == arg.BranchId);
            }
            if (RiddhaSession.UserType == (int)UserType.User && RiddhaSession.RoleId != 0)
            {
                sectionQuery = AddKendoSectionParent(RiddhaSession.SectionId).AsQueryable();
            }

            int totalRowNum = sectionQuery.Count();
            string searchField = arg.Filter.Filters.Count() <= 0 ? "" : arg.Filter.Filters[0].Field;
            string searchOp = arg.Filter.Filters.Count() <= 0 ? "" : arg.Filter.Filters[0].Operator;
            string searchValue = arg.Filter.Filters.Count() <= 0 ? "" : arg.Filter.Filters[0].Value;
            IQueryable<ESection> paginatedQuery;
            switch (searchField)
            {
                case "Code":
                    if (searchOp == "startswith")
                    {
                        paginatedQuery = sectionQuery.Where(x => x.Code.StartsWith(searchValue.Trim())).OrderByDescending(x => x.Id).ThenBy(x => x.Name);
                    }
                    else
                    {
                        paginatedQuery = sectionQuery.Where(x => x.Code == searchValue.Trim()).OrderByDescending(x => x.Id).ThenBy(x => x.Name);
                    }
                    break;
                case "Name":
                    if (searchOp == "startswith")
                    {
                        paginatedQuery = sectionQuery.Where(x => x.Name.StartsWith(searchValue.Trim())).OrderByDescending(x => x.Id).ThenBy(x => x.Code);
                    }
                    else
                    {
                        paginatedQuery = sectionQuery.Where(x => x.Name == searchValue.Trim()).OrderByDescending(x => x.Id).ThenBy(x => x.Code);
                    }
                    break;
                case "DepartmentName":
                    if (searchOp == "startswith")
                    {
                        paginatedQuery = sectionQuery.Where(x => x.Department.Name.StartsWith(searchValue.Trim())).OrderByDescending(x => x.Id).ThenBy(x => x.Code);
                    }
                    else
                    {
                        paginatedQuery = sectionQuery.Where(x => x.Department.Name == searchValue.Trim()).OrderByDescending(x => x.Id).ThenBy(x => x.Code);
                    }
                    break;
                default:
                    paginatedQuery = sectionQuery.OrderByDescending(x => x.Id).ThenBy(x => x.Name);

                    break;
            }
            var sectionlist = (from c in paginatedQuery
                               join d in paginatedQuery on c.ParentId equals d.Id into df
                               from d in df.DefaultIfEmpty()

                               select new SectionGridVm()
                               {
                                   Id = c.Id,
                                   Code = c.Code,
                                   Name = c.Name,
                                   NameNp = c.NameNp,
                                   BranchId = c.BranchId,
                                   DepartmentId = c.DepartmentId,
                                   DepartmentName = c.Department == null ? "" : ((!string.IsNullOrEmpty(c.Department.NameNp) && language == "ne") ? c.Department.NameNp : c.Department.Name),
                                   BranchName = c.Branch.Name,
                                   ParentId = c.ParentId,
                                   UnitCode = c.UnitCode,
                                   ParentName = d == null ? "" : d.Name,
                                   UnitType = c.UnitType
                               }).OrderBy(x => x.Id).ToList();
            return new KendoGridResult<List<SectionGridVm>>()
            {
                Data = sectionlist.Skip(arg.Skip).Take(arg.Take).ToList(),
                Status = ResultStatus.Ok,
                TotalCount = sectionlist.Count()
            };
        }

        public List<ESection> AddKendoSectionParent(int sectionId)
        {
            List<ESection> sectionList = new List<ESection>();
            string language = RiddhaSession.Language.ToString();
            DataVisibilityLevel dataVisibilityLevel = (DataVisibilityLevel)RiddhaSession.DataVisibilityLevel;

            switch (dataVisibilityLevel)
            {
                case DataVisibilityLevel.Self:
                    return sectionList = sectionServices.List().Data.Where(x => x.Id == sectionId && x.BranchId == RiddhaSession.BranchId).ToList();
            }
            var list = sectionServices.List().Data.Where(x => x.Id == sectionId && x.BranchId == RiddhaSession.BranchId).ToList();
            sectionList.AddRange(list);
            AddKendoSectionChild(list, sectionList);
            return sectionList;
        }
        public void AddKendoSectionChild(List<ESection> list, List<ESection> FinalList)
        {
            string language = RiddhaSession.Language.ToString();
            foreach (var item in list)
            {
                var sections = sectionServices.List().Data.Where(x => x.ParentId == item.Id).ToList();
                if (sections.Count() > 0)
                {
                    FinalList.AddRange(sections);
                    AddKendoSectionChild(sections, FinalList);
                }

            }


        }
        private bool checkValidString(string value)
        {
            return !(string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value));
        }
        private int getDepartmentId(IQueryable<EDepartment> departmentQuery, string code, int branchId)
        {
            if (string.IsNullOrEmpty(code))
            {
                return 0;
            }
            var department = departmentQuery.Where(x => x.BranchId == branchId && x.Code.ToUpper() == code.Trim().ToUpper()).FirstOrDefault();
            if (department == null)
            {
                return 0;
            }
            else
            {
                return department.Id;
            }
        }
        [HttpPost]
        public ServiceResult<List<ESection>> Upload()
        {
            var branch = RiddhaSession.CurrentUser.Branch;
            var request = HttpContext.Current.Request;
            List<ESection> SectionLst = new List<ESection>();
            using (var package = new OfficeOpenXml.ExcelPackage(request.InputStream))
            {
                IQueryable<EDepartment> departmentQuery = new SDepartment().List().Data;
                TimeSpan defaultTime = "00:00".ToTimeSpan();
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;
                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    ESection model = new ESection();
                    model.BranchId = branch.Id;
                    model.Code = (workSheet.Cells[rowIterator, 1].Value ?? string.Empty).ToString();
                    if (checkValidString(model.Code) == false)
                    {
                        continue;
                    }
                    model.Name = (workSheet.Cells[rowIterator, 2].Value ?? string.Empty).ToString();
                    if (checkValidString(model.Name) == false)
                    {
                        continue;
                    }
                    model.NameNp = (workSheet.Cells[rowIterator, 3].Value ?? string.Empty).ToString();
                    model.DepartmentId = getDepartmentId(departmentQuery, (workSheet.Cells[rowIterator, 4].Value ?? string.Empty).ToString(), branch.Id);


                    SectionLst.Add(model);
                }
                var uniqueLst = SectionLst.GroupBy(x => x.Code)
              .Where(g => g.Count() == 1)
              .Select(y => new { Element = y.Key, Counter = y.Count() })
              .ToList();
                var listToSave = (from c in SectionLst
                                  join d in uniqueLst on c.Code equals d.Element
                                  select c).ToList();
                var result = new ServiceResult<List<ESection>>();
                if (listToSave.Count() > 0)
                {
                    result = sectionServices.UploadExcel(listToSave, branch.Id);
                }
                if (uniqueLst.Count() != SectionLst.Count())
                {
                    return new ServiceResult<List<ESection>>()
                    {
                        Data = listToSave,
                        Message = listToSave.Count().ToString() + " out of " + SectionLst.Count().ToString() + " Saved Successfully",
                        Status = ResultStatus.Ok
                    };
                }
                return new ServiceResult<List<ESection>>()
                {
                    Data = result.Data,
                    Status = result.Status,
                    Message = loc.Localize(result.Message)
                };
            }
        }
    }
    public class SectionGridVm
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameNp { get; set; }
        public int DepartmentId { get; set; }
        public int? BranchId { get; set; }

        public int? ParentId { get; set; }

        public string UnitCode { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentNameNp { get; set; }
        public string ParentName { get; set; }
        public UnitType UnitType { get; set; }
        public string UnitTypeName { get { return this.UnitType.ToString(); } }
    }

    public class TreeViewSectionVm
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
