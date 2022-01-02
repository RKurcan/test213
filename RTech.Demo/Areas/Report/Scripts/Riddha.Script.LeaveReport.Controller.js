function leaveReportController(date, companyName) {
    var self = this;
    var config = new Riddha.config();
    var language = config.CurrentLanguage;
    var curDate = config.CurDate;
    self.Report = ko.observable();
    self.ReportId = ko.observable(0);
    self.Departments = ko.observableArray([]);
    self.Sections = ko.observableArray([]);
    self.Employees = ko.observableArray([]);
    self.CheckAllDepartments = ko.observable(false);
    self.CheckAllBranches = ko.observable(false);
    self.CheckAllSections = ko.observable(false);
    self.CheckAllEmployees = ko.observable(false);
    self.OnDate = ko.observable(date).extend({ date: 'yyyy/MM/dd' });
    self.EndDate = ko.observable(date).extend({ date: 'yyyy/MM/dd' });
    self.Months = ko.observableArray([]);
    self.MonthId = ko.observable(0);
    self.Year = ko.observable(Riddha.util.getYear(curDate));
    self.ActiveInactiveMode = ko.observable("0");
    self.VisibleEndDate = ko.observable(false);
    self.FiscalYears = ko.observableArray([]);
    self.LeaveMasters = ko.observableArray([]);

    self.Directorates = ko.observableArray([]);
    self.FilteredDirectorate = ko.observableArray([]);
    self.SearchDirectorateText = ko.observable('');
    self.CheckAllDirectorates = ko.observable(false);
    self.Units = ko.observableArray([]);
    self.FilteredUnits = ko.observableArray([]);
    self.SearchUnitText = ko.observable('');
    self.CheckAllUnits = ko.observable(false);
    self.SearchDepartmentText = ko.observable('');
    self.FilteredDepartment = ko.observableArray([]);
    self.SearchSectionText = ko.observable('');
    self.FilteredSection = ko.observableArray([]);
    self.CheckedUnits = ko.observableArray([]);
    self.SearchEmployeeText = ko.observable('');
    self.FilteredEmployee = ko.observableArray([]);
    //getDepartments();
    //function getDepartments() {
    //    Riddha.ajax.get("/Api/AttendanceReportApi/GetDepartments", null)
    //        .done(function (result) {
    //            var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data), checkBoxModel);
    //            self.Departments(data);
    //        });
    //};



    getFiscalYears();
    function getFiscalYears() {
        Riddha.ajax.get("/Api/LeaveReportApi/GetFiscalYear", null)
            .done(function (result) {
                var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data), checkBoxModel);
                self.FiscalYears(data);
            });
    };

    ////Checkall Section
    //self.CheckAllDepartments.subscribe(function (newValue) {
    //    ko.utils.arrayForEach(self.Departments(), function (item) {
    //        item.Checked(newValue);
    //    });
    //    if (newValue) {
    //        self.GetSections();
    //    }
    //    else {
    //        self.Sections([]);
    //        self.CheckAllSections(false);
    //        //self.CheckAllEmployees(false);
    //    }
    //});

    //self.CheckAllSections.subscribe(function (newValue) {
    //    ko.utils.arrayForEach(self.Sections(), function (item) {
    //        item.Checked(newValue);
    //    });
    //    if (newValue) {
    //        self.GetEmployee();
    //    }
    //    else {
    //        self.Employees([]);
    //        self.CheckAllEmployees(false);
    //    }
    //});

    //self.CheckAllEmployees.subscribe(function (newValue, e) {
    //    //ko.utils.arrayForEach(self.Employees(), function (item) {
    //    //    item.Checked(newValue);
    //    //});
    //});

    //self.GetSections = function () {
    //    var departments = "";
    //    ko.utils.arrayForEach(self.Departments(), function (data) {
    //        if (data.Checked() == true) {
    //            if (departments.length != 0)
    //                departments += "," + data.Id();
    //            else
    //                departments = data.Id() + '';
    //        }
    //        else {
    //            self.Sections([]);
    //        }
    //    });
    //    if (departments.length > 0) {
    //        Riddha.ajax.get("/Api/AttendanceReportApi/GetSectionsByDepartment/" + departments)
    //            .done(function (result) {
    //                var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data), checkBoxModel);
    //                self.Sections(data);
    //            });
    //    }
    //};

    //self.GetEmployee = function () {
    //    var sections = "";
    //    ko.utils.arrayForEach(self.Sections(), function (data) {
    //        if (data.Checked() == true) {
    //            if (sections.length != 0)
    //                sections += "," + data.Id();
    //            else
    //                sections = data.Id() + '';
    //        }
    //        else {
    //            self.Employees([]);
    //        }
    //    });
    //    if (sections.length > 0) {
    //        Riddha.ajax.get("/Api/AttendanceReportApi/GetEmployeeBySection?id=" + sections + "&activeInactiveMode=" + self.ActiveInactiveMode())
    //            .done(function (result) {
    //                var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data), checkBoxModel);
    //                self.Employees(data);
    //            });
    //    }
    //};

    ///New Report Format
    self.UnitTypeName = ko.observable();
    self.IsSelf = ko.observable(false);
    GetDepartments();
    function GetDepartments() {
        Riddha.ajax.get("/Api/AttendanceReportApi/GetTreeStructure")
            .done(function (result) {
                var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data.TreeData), checkBoxModel);
                self.Departments(data);
                self.FilteredDepartment(data);
                var employeeData = Riddha.ko.global.arrayMap(ko.toJS(result.Data.Employees), EmployeeCheckModel);
                self.Employees(employeeData);
                self.FilteredEmployee(employeeData)
                self.IsSelf(result.Data.IsSelf);
                self.UnitTypeName(result.Data.UnitType);
            });

    };
    self.GetChildCheckList = function (array) {
        var checkedIds = '';
        ko.utils.arrayForEach(array, function (data) {
            if (data.Checked() == true) {
                if (checkedIds.length != 0)
                    checkedIds += "," + data.Id();
                else
                    checkedIds = data.Id() + '';

            }

            var chhildCheckedIds = self.GetChildCheckList(data.Children());
            if (chhildCheckedIds != '')
                checkedIds += "," + chhildCheckedIds;




        });
        return checkedIds;
    }
    function GetSelectedCheckedItemByArray(array) {
        var items = "";
        ko.utils.arrayForEach(array, function (data) {
            if (data.Checked() == true) {
                if (items.length != 0)
                    items += "," + data.Id();
                else
                    items = data.Id() + '';

            }

            var checkedIds = self.GetChildCheckList(data.Children());
            if (checkedIds != '')
                items += "," + checkedIds;
        });

        return items;
    }

    self.GetDirectorates = function () {
        self.Employees([]);
        self.FilteredEmployee([]);
        var departments = GetSelectedCheckedItemByArray(self.Departments());

        if (departments.length > 0) {

            /*Riddha.ajax.get("/Api/AttendanceReportApi/GetDepartmentsByBranch?branchIds=" + branches)*/
            Riddha.ajax.get("/Api/AttendanceReportApi/GetDirectorateByDepartment?id=" + departments)
                .done(function (result) {
                    var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data.CheckList), checkBoxModel);


                    self.Directorates(data);
                    self.FilteredDirectorate(data);
                    var customerData = Riddha.ko.global.arrayMap(ko.toJS(result.Data.EmployeeList), EmployeeCheckModel);
                    self.Employees(customerData);
                    self.FilteredEmployee(customerData);
                });
        }
    };

    self.GetSectionsForCheckList = function () {
        self.Employees([]);
        self.FilteredEmployee([]);
        var directores = GetSelectedCheckedItemByArray(self.Directorates());


        if (directores.length > 0) {

            Riddha.ajax.get("/Api/AttendanceReportApi/GetSectionsByDirectorate?id=" + directores)
                .done(function (result) {
                    var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data.CheckList), checkBoxModel);
                    self.Sections(data);
                    self.FilteredSection(data);
                    var customerData = Riddha.ko.global.arrayMap(ko.toJS(result.Data.EmployeeList), EmployeeCheckModel);
                    self.Employees(customerData);
                    self.FilteredEmployee(customerData);
                });
        }
    }

    self.GetUnits = function () {
        self.Employees([]);
        self.FilteredEmployee([]);
        var sections = GetSelectedCheckedItemByArray(self.Sections());
        if (sections.length > 0) {

            Riddha.ajax.get("/Api/AttendanceReportApi/GetUnitsBySections?id=" + sections)
                .done(function (result) {
                    var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data.CheckList), checkBoxModel);
                    self.Units(data);
                    self.FilteredUnits(data);
                    var customerData = Riddha.ko.global.arrayMap(ko.toJS(result.Data.EmployeeList), EmployeeCheckModel);
                    self.Employees(customerData);
                    self.FilteredEmployee(customerData);
                });
        }
    }

    self.GetEmployee = function () {
        self.Employees([]);
        self.FilteredEmployee([]);
        var units = GetSelectedCheckedItemByArray(self.Units());
        if (units.length > 0) {

            Riddha.ajax.get("/Api/AttendanceReportApi/GetEmployeeByUnit?id=" + units)
                .done(function (result) {
                    var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data), EmployeeCheckModel);
                    self.Employees(data);
                    self.FilteredEmployee(data);
                });
        }
        else {
            self.Employees([]);
            self.FilteredEmployee([]);
        }
    };

    var count = 0;
    self.ChildSearchText = function (array, filteredArray, newValue) {


        ko.utils.arrayForEach(array, function (item) {

            if (count == 0) {
                filteredArray(Riddha.ko.global.filter(item.Children, newValue));
            }
            if (filteredArray().length != 0) {
                count = 1;
            };
            if (count == 0) {
                if (filteredArray().length == 0) {
                    self.ChildSearchText(item.Children(), filteredArray, newValue);
                }
            }


        });
    }

    self.SearchDepartmentText.subscribe(function (newValue) {
        count = 0;
        if (newValue == '') {
            self.FilteredDepartment(self.Departments());
        } else {
            self.FilteredDepartment(Riddha.ko.global.filter(self.Departments, newValue));

            if (self.FilteredDepartment().length == 0) {
                self.ChildSearchText(self.Departments(), self.FilteredDepartment, newValue);
            }

        }


    });

    self.SearchDirectorateText.subscribe(function (newValue) {
        count = 0;
        if (newValue == '') {
            self.FilteredDirectorate(self.Directorates());
        } else {
            self.FilteredDirectorate(Riddha.ko.global.filter(self.Directorates, newValue));
            if (self.FilteredDirectorate().length == 0) {
                self.ChildSearchText(self.Directorates(), self.FilteredDirectorate, newValue);
            }
        }
    });

    self.SearchSectionText.subscribe(function (newValue) {
        count = 0;
        if (newValue == '') {
            self.FilteredSection(self.Sections());
        } else {
            self.FilteredSection(Riddha.ko.global.filter(self.Sections, newValue));
            if (self.FilteredSection().length == 0) {
                self.ChildSearchText(self.Sections(), self.FilteredSection, newValue);
            }
        }
    });

    self.SearchUnitText.subscribe(function (newValue) {
        count = 0;
        if (newValue == '') {
            self.FilteredUnits(self.Units());
        } else {
            self.FilteredUnits(Riddha.ko.global.filter(self.Units, newValue));
            if (self.FilteredUnits().length == 0) {
                self.ChildSearchText(self.Units(), self.FilteredUnits, newValue);
            }
        }
    });

    self.SearchEmployeeText.subscribe(function (newValue) {
        if (newValue == '') {
            self.FilteredEmployee(self.Employees());
        } else {
            self.FilteredEmployee(Riddha.ko.global.filter(self.Employees, newValue));
        }
    });

    //Checkall Department

    self.CheckChild = function (array, newValue) {
        ko.utils.arrayForEach(array, function (item) {
            item.Checked(newValue);
            self.CheckChild(item.Children(), newValue);
        });
    }

    self.CheckAllBranches.subscribe(function (newValue) {
        ko.utils.arrayForEach(self.Branches(), function (item) {
            item.Checked(newValue);
        });
        if (newValue) {
            self.GetDepartments();
        }
        else {
            self.Departments([]);
            self.FilteredDepartment([]);
            self.CheckAllDepartments(false);
        }
    });
    self.CheckAllDepartments.subscribe(function (newValue) {
        ko.utils.arrayForEach(self.Departments(), function (item) {
            item.Checked(newValue);
            self.CheckChild(item.Children(), newValue);
        });
        /*  $.each(ko.toJS(self.Departments()), function (i, item) { self.CheckedDepartments.push(item.Id) })*/
        if (newValue) {
            self.GetDirectorates();
        }
        else {
            self.CheckAllDirectorates(false);
            self.Directorates([]);
            self.FilteredDirectorate([]);
        }


    });

    self.CheckAllDirectorates.subscribe(function (newValue) {
        ko.utils.arrayForEach(self.Directorates(), function (item) {
            item.Checked(newValue);
            self.CheckChild(item.Children(), newValue);
        });
        if (newValue) {
            self.GetSectionsForCheckList();
        }
        else {
            self.Sections([]);
            self.FilteredSection([]);
            self.CheckAllSections(false);
        }
    });

    self.CheckAllSections.subscribe(function (newValue) {
        ko.utils.arrayForEach(self.Sections(), function (item) {
            item.Checked(newValue);
            self.CheckChild(item.Children(), newValue);
        });
        /*$.each(ko.toJS(self.Sections()), function (i, item) { self.CheckedSections.push(item.Id) })*/
        if (newValue) {
            self.GetUnits();
        }
        else {
            self.Units([]);
            self.FilteredUnits([]);
            self.CheckAllUnits(false);
        }

    });

    self.CheckAllUnits.subscribe(function (newValue) {
        ko.utils.arrayForEach(self.Units(), function (item) {
            item.Checked(newValue);
            self.CheckChild(item.Children(), newValue);
        });
        //$.each(ko.toJS(self.Units()), function (i, item) { self.CheckedUnits.push(item.Id) })
        if (newValue) {
            self.GetEmployee();
        }
        else {
            self.Employees([]);
            self.FilteredEmployee([]);
            self.CheckAllEmployees(false);
        }
        //ko.utils.arrayForEach(self.Units(), function (item) {
        //    item.Checked(newValue);
        //});
    });


    self.CheckAllEmployees.subscribe(function (newValue, e) {
        //ko.utils.arrayForEach(self.Employees(), function (item) {
        //    item.Checked(newValue);
        //});
    });
    var count = 0;

    self.RemoveSectionChild = function (parentData, parentId) {
        count = 0;
        ko.utils.arrayForEach(parentData, function (item) {

            if (item.Id() == parentId) {
                item.Children([]);
                count++;
            }

            if (count == 0) {
                self.RemoveSectionChild(item.Children(), parentId);
            }

        });
    }

    self.AddSectionChild = function (parentData, childata, parentId) {
        count = 0;
        ko.utils.arrayForEach(parentData, function (item) {
            if (item.Id() == parentId) {
                ko.utils.arrayForEach(childata, function (child) {
                    item.Children.push(child);
                });
                count++;
            }
            if (count == 0) {
                self.AddSectionChild(item.Children(), childata, parentId);
            }

        });
    }

    self.GetChildrenFromParent = function ($data) {

        self.getChildList($data.Id());
    }
    self.RemoveChildrentFromParent = function ($data) {
        self.RemoveSectionChild(self.Departments(), $data.Id())
    }
    self.getChildList = function (parentId) {

        Riddha.ajax.get("/Api/AttendanceReportApi/GetChildSectionFromParentId?parentId=" + parentId)
            .done(function (result) {
                var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data), checkBoxModel);
                self.AddSectionChild(self.Departments(), data, parentId);
            });
    }
    self.GetChecked = function ($parent, $data) {

        checkChildren($data);
        switch ($parent.type) {
            case 1:
                self.GetDirectorates();
                break;
            case 2:
                self.GetSectionsForCheckList();
                break;
            case 3:
                self.GetUnits();
                break;
            case 4:
                self.GetEmployee();
                break;
            default:
                break;
        }


    }
    function checkChildren($data) {
        var checked = $data.Checked();
        ko.utils.arrayForEach($data.Children(), function (item) {
            item.Checked(checked);
            checkChildren(item);
        });
    }
    //Added by Ganesh Shrestha 2021/09/19

    self.CallHTMLReport = function (url, params) {

        self.GenrateSampleReport(url, params);
    }

    self.GenrateSampleReport = function (url, params) {
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        //form.setAttribute("action", "/Report/SettingReport/Index");
        form.setAttribute("action", url);
        form.setAttribute("target", "_blank");

        for (var key in params) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);
            form.appendChild(hiddenField);
        }

        form.appendChild(hiddenField);
        document.body.appendChild(form);
        form.submit();
    };

    function getMonths() {
        var monthUrl = "";
        if (config.CurrentOperationDate == "ne") {
            monthUrl = "/Api/AttendanceReportApi/GetNepaliMonths";
        }
        else {
            monthUrl = "/Api/AttendanceReportApi/GetEnglishMonths"
        }
        Riddha.ajax.get(monthUrl)
            .done(function (result) {
                if (result.Status == 4) {
                    for (var i = 0; i < result.Data.length; i++) {
                        self.Months.push(new GlobalDropdownModel({ Id: i + 1, Name: result.Data[i] }));
                    }
                    self.MonthId(Riddha.util.getMonth(curDate));
                }
            })
    }
    getMonths();

    self.ShowModal = function (id, title) {
        self.VisibleEndDate(false);
        self.OnDate(curDate);
        self.Report(title);
        self.ReportId(id);
        if (id == 30 || id == 31) {
            setDefDate();
            //self.VisibleEndDate(true);
        }
        if (id == 32) {
            setDefDate();
            //self.VisibleEndDate(true);
        }
        if (id == 30 || id == 31 || id == 32) {

            $("#leaveReportModel").modal('show');
        }
    };

    self.leaveMasterWiseReportShowModal = function (id, title) {
        $("#leaveMasterWiseReportModal").modal('show');
    }

    self.LeaveMasterWiseCloseModal = function () {
        $("#leaveMasterWiseReportModal").modal('hide');
    };

    self.CloseModal = function () {
        $("#leaveReportModel").modal('hide');
    };

    self.Reset = function () {

        ko.utils.arrayForEach(self.Departments(), function (data) {
            data.Checked(false);

        });

        self.FilteredDirectorate([]);
        self.FilteredUnits([]);
        self.FilteredDepartment([]);
        self.FilteredEmployee([]);
        GetDepartments();
    };

    self.dateLength = ko.observable(28);

    self.ComputeMaxDate = ko.computed(function () {

        Riddha.global.getMaxDaysInMonth(self.Year(), self.MonthId())
            .then(function (maxDays) {
                self.dateLength(maxDays);
                //for ondate and end date 
                setDefDate()
            });
    });

    function setDefDate() {
        if (Riddha.config().CurrentOperationDate == 'en') {
            self.OnDate(setDateToTextBox(self.Year(), self.MonthId(), 1));
            self.EndDate(setDateToTextBox(self.Year(), self.MonthId(), self.dateLength()));
        }
        if (Riddha.config().CurrentOperationDate == 'ne') {
            var onDateNe = setDateToTextBox(self.Year(), self.MonthId(), 1);
            self.OnDate(BS2AD(onDateNe));
            var toDateNe = setDateToTextBox(self.Year(), self.MonthId(), self.dateLength());
            self.EndDate(BS2AD(toDateNe));
        }
        function setDateToTextBox(year, month, id) {
            return '' + year + '/' + Riddha.util.padLeft(month, 2) + '/' + Riddha.util.padLeft(id, 2);
        }
    };

    function getSelectedEmps() {
        var employees = "";
        ko.utils.arrayForEach(self.Employees(), function (data) {
            if (data.Checked() == true) {
                if (employees.length != 0)
                    employees += "," + data.Id();
                else
                    employees = data.Id() + '';
            }
        });
        if (employees.length == 0) {
            ko.utils.arrayForEach(self.Employees(), function (data) {
                if (employees.length != 0)
                    employees += "," + data.Id();
                else
                    employees = data.Id() + '';
            });
        }
        return employees;
    }

    function getSelectedDepartments() {
        var deps = "";
        ko.utils.arrayForEach(self.Departments(), function (data) {
            if (data.Checked() == true) {
                if (deps.length != 0)
                    deps += "," + data.Id();
                else
                    deps = data.Id() + '';
            }
        });
        return deps;
    }

    function getSelectedSections() {
        var sections = "";
        ko.utils.arrayForEach(self.Units(), function (data) {
            if (data.Checked() == true) {
                if (sections.length != 0)
                    sections += "," + data.Id();
                else
                    sections = data.Id() + '';
            }
        });
        return sections;
    }

    function getSelectedUnits() {
        var sections = "";
        ko.utils.arrayForEach(self.Units(), function (data) {
            if (data.Checked() == true) {
                if (sections.length != 0)
                    sections += "," + data.Id();
                else
                    sections = data.Id() + '';
            }
        });
        return sections;
    }

    function getSelectedFiscalYear() {
        var fiscalYear = "";
        ko.utils.arrayForEach(self.FiscalYears(), function (data) {
            if (data.Checked() == true) {
                if (fiscalYear.length != 0)
                    fiscalYear += "," + data.Id();
                else
                    fiscalYear = data.Id() + '';
            }
        });
        return fiscalYear;
    }


    self.KendoGridOptionsForLeaveWiseSummary = function (title) {
        var employees = getSelectedEmps();
        var departments = getSelectedDepartments();
        var sections = getSelectedSections();


        return {
            title: title,
            target: "#kendoLeaveWiseSummaryReportWindow",
            url: "/Api/LeaveReportApi/GenerateReport",
            paramData: { DeptIds: departments, SectionIds: sections, EmpIds: employees, onDate: self.OnDate(), ToDate: self.EndDate(), ActiveInactiveMode: self.ActiveInactiveMode() },
            //height: docHeight,
            width: '70%',
            height: docHeight - 20,
            multiSelect: false,
            maximize: true,
            actions: [
                "Close"
            ],
            group: false,
            groupParam: [
                {
                    field: "LeaveName",

                }],
            //sort: { field: "WorkDate", dir: "asc" },
            pageSize: 50,
            columns: self.columSpecForLeaveSummary
        }
    }

    var docHeight = $(document).height();

    self.KendoGridOptionsForLeaveSummary = function (title) {
        var employees = getSelectedEmps();
        var departments = getSelectedDepartments();
        var units = getSelectedUnits();
        //var fiscalYear = getSelectedFiscalYear();
        return {
            title: title,
            target: "#kendoLeaveSummaryReportWindow",
            url: "/Api/LeaveReportApi/GenerateReport",
            paramData: { DeptIds: departments, SectionIds: units, EmpIds: employees, onDate: self.OnDate(), ToDate: self.EndDate(), ActiveInactiveMode: self.ActiveInactiveMode() },
            //height: docHeight,
            width: '70%',
            height: docHeight - 20,
            multiSelect: false,
            maximize: true,
            actions: [
                "Close"
            ],
            group: false,
            groupParam: [
                {
                    field: "Name",

                }],
            //sort: { field: "WorkDate", dir: "asc" },
            pageSize: 50,
            columns: self.columSpecForLeaveSummary
        }
    }

    self.KendoGridOptionsForLeaveReport = function (title) {
        var employees = getSelectedEmps();
        var departments = getSelectedDepartments();
        var sections = getSelectedSections();
        return {
            title: title,
            target: "#pivotgrid",
            targetPivotGrid: "#pivotgrid",
            url: "/Api/LeaveSummaryReportApi/GenerateReport",
            paramData: { DeptIds: departments, SectionIds: sections, EmpIds: employees, onDate: self.OnDate(), ToDate: self.EndDate(), ActiveInactiveMode: self.ActiveInactiveMode() },
            height: docHeight - 20,
            multiSelect: false,
            maximize: true,
            pageSize: 10000,
            modelFields: {
                EmployeeName: { type: "string" },
                LeaveName: { type: "string" },
                TakenLeave: { type: "number" }
            },
            cubeDimensions: {
                EmployeeName: {
                    caption: language == "ne" ? "सबै कर्मचारीहरु" : "All Employees"
                },
                LeaveName: {
                    caption: language == "ne" ? "सबै बिदाहरु" : "All Leaves"
                },
            },
            measureField: "TakenLeave",
            columns: [{
                name: "LeaveName",
                //name: language == "ne" ? "बिदाको नाम " : "LeaveName",
                headerAttribures: { style: "white-space:normal" },
                expand: true
            }],
            rows: [{ name: "EmployeeName", expand: true }],
        }
    }

    self.columSpecForLeaveSummary = [
        {
            field: "Name", title: language == "ne" ? "कर्मचारी नाम" : "Employee Name", headerAttributes: { style: "text-align:center;" }, filterable: true,

        },
        {
            field: "SectionName", title: language == "ne" ? "Unit" : "Unit", headerAttributes: { style: "text-align:center;" }, filterable: true,
        },
        {
            field: "LeaveName", title: language == "ne" ? "बिदा" : "Leave", headerAttributes: { style: "text-align:center;" }, filterable: true,

        },
        {
            field: "Balance", title: language == "ne" ? "उद्घाटन ब्यालेन्स" : "Opening Balance", headerAttributes: { style: "text-align:center;" }, filterable: true,

        },
        {
            field: "TakenLeave", title: language == "ne" ? "लिएको" : "Taken", headerAttributes: { style: "text-align:center;" }, filterable: true,

        },
        {
            field: "RemLeave", title: language == "ne" ? "बाँकी" : "Remaining", headerAttributes: { style: "text-align:center;" }, filterable: true,

        },
    ];

    function getSuitableTitle(title, date) {
        if (date == false) {
            if (language == "ne")
                return title + " " + SuitableDate(self.OnDate());
            else
                return title + " on " + SuitableDate(self.OnDate());
        }
        else {
            if (language == "ne")
                return title + " " + SuitableDate(self.OnDate()) + " बाट " + SuitableDate(self.EndDate()) + " सम्म ";
            else
                return title + " from " + SuitableDate(self.OnDate()) + " to " + SuitableDate(self.EndDate());
        }
    };


    function getSelectedLeaveMasters() {

        var leavemasters = "";
        ko.utils.arrayForEach(self.LeaveMasters(), function (data) {
            if (data.Checked() == true) {
                if (leavemasters.length != 0)
                    leavemasters += "," + data.Id();
                else
                    leavemasters = data.Id() + '';
            }
        });
        if (leavemasters == "" || leavemasters.length == 0) {
            ko.utils.arrayForEach(self.LeaveMasters(), function (data) {
                if (leavemasters.length != 0)
                    leavemasters += "," + data.Id();
                else
                    leavemasters = data.Id() + '';
            });
        }
        return leavemasters;
    }

    getLeaveMasterForCheckBoxList();
    function getLeaveMasterForCheckBoxList() {
        Riddha.ajax.get("/Api/LeaveMasterApi/GetLeaveMaster", null)
            .done(function (result) {
                var data = Riddha.ko.global.arrayMap(ko.toJS(result.Data), checkBoxModel);
                self.LeaveMasters(data);
            });
    }

    self.KendoGridOptionsForLeaveMasterSummary = function (title) {
        var leaveMasters = getSelectedLeaveMasters();
        return {
            title: title,
            target: "#kendoLeaveWiseSummaryReportWindow",
            url: "/Api/LeaveReportApi/GenerateLeaveMasterWiseLeaveReport",
            paramData: { LeaveMasterIds: leaveMasters, ActiveInactiveMode: self.ActiveInactiveMode() },
            width: '70%',
            height: docHeight - 20,
            multiSelect: false,
            maximize: true,
            actions: [
                "Close"
            ],
            group: false,
            groupParam: [
                {
                    field: "Section", title: language == "ne" ? "Unit" : "Unit"

                },
                {
                    field: "Employee",

                }],
            //sort: { field: "WorkDate", dir: "asc" },
            pageSize: 50,
            columns: self.columSpecForLeaveMasterWieReport
        }
    }

    self.columSpecForLeaveMasterWieReport = [
        {
            field: "Employee", title: language == "ne" ? "कर्मचारी नाम" : "Employee Name", headerAttributes: { style: "text-align:center;" }, filterable: false,

        },
        {
            field: "LeaveName", title: language == "ne" ? "बिदा" : "Leave", headerAttributes: { style: "text-align:center;" }, filterable: false,

        },
        {
            field: "FromDate", title: language == "ne" ? "FromDate " : "From Date", headerAttributes: { style: "text-align:center;" }, filterable: false, template: "#=SuitableDate(FromDate)#",

        },
        {
            field: "ToDate", title: language == "ne" ? "लिएको" : "To Date", headerAttributes: { style: "text-align:center;" }, filterable: false, template: "#=SuitableDate(ToDate)#",

        },
        {
            field: "LeaveDays", title: language == "ne" ? "बाँकी" : "Leave Days", headerAttributes: { style: "text-align:center;" }, filterable: false,

        },
        {
            field: "Description", title: language == "ne" ? "बाँकी" : "Description", headerAttributes: { style: "text-align:center;" }, filterable: false,

        },
    ];
}