using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Controllers;
using Pronali.Web.Extension;
using Pronali.Web.Helper;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class EmployeeController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly DropdownHelper _dropdownHelper;
        private readonly IImagePath _imagePath;
        
        public EmployeeController(IUnitOfWork _unitOfWork, IHostingEnvironment env, IImagePath imagePath) : base(_unitOfWork)
        {
            _env = env;
            _dropdownHelper = new DropdownHelper();
            _imagePath = imagePath;
        }
        //sd
        /////////////////////////////////////////////LOAD data iNTo ADVANCE SEARCh/////////////////////////////////////
        public JsonResult drpJoinYear()
        {
            var items = _dropdownHelper.Get50YearsDropdownList();
            return Json(items);
        }
        public JsonResult drpJoinMonth()
        {
            var items = _dropdownHelper.GetAllMonthsDropdownList();
            return Json(items);
        }
        public JsonResult drpCompany()
        {
            var items = db.Company.GetAll().OrderBy(o => o.CompanyName).Select(x => new SelectListItem() { Text = x.CompanyName, Value = x.Id.ToString()}).ToList();
            return Json(items);
        }
        public JsonResult drpSister()
        {
            var items = db.SisterConcern.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpDivision()
        {
            var items = db.Division.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpBranch()
        {
            var items = db.Branch.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpDepartment()
        {
            var items = db.Department.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpDesignation()
        {
            var items = db.Designation.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpSection()
        {
            var items = db.Section.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpShift()
        {
            var items = db.Shift.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpLine()
        {
            var items = db.Line.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpFloor()
        {
            var items = db.Floor.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpEmployeeGroup()
        {
            var items = db.EmployeeGroup.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpSuperior()
        {
            var items = db.Employee.GetAll().OrderBy(o => o.FullName).Select(x => new SelectListItem() { Text = x.FullName, Value = x.MaskingId.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpJObLocation()
        {
            var items = db.JobLocation.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpGender()
        {
            var items = _dropdownHelper.GetGenderDropdownList();
            return Json(items);
        }
        public JsonResult drpReligion()
        {
            var items = _dropdownHelper.GetReligionDropdownList();
            return Json(items);
        }
        public JsonResult drpMachine()
        {
            var items = db.Machine.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Json(items);
        }
        public JsonResult drpSpecialCase()
        {
            var items = _dropdownHelper.GetSpecialCaseDropdownList();
            return Json(items);
        }
        public JsonResult drpIncentive()
        {
            var items = _dropdownHelper.GetIncentiveDropdownList();
            return Json(items);
        }
        public JsonResult drpMaritalStatus()
        {
            var items = _dropdownHelper.GetMaritalStatusDropdownList();
            return Json(items);
        }
        public JsonResult drpBloodGroup()
        {
            var items = _dropdownHelper.GetBloodGroupDropdownList();
            return Json(items);
        }


        //////////////////////****END****///////////////////////LOAD data iNTo ADVANCE SEARCh/////////////////////****END****////////////////


        public IActionResult EmployeeUpdate( int EmployeeId)
        {
            VmEmployeeCreate vmEmployeeCreate = new VmEmployeeCreate();
            var Data = db.Employee.GetFirstOrDefaultWithRelatedData(model=>model.Id == EmployeeId);
           
            var result = EmployeeUpdateMethod(vmEmployeeCreate, Data);
           
            return PartialView("EmployeeUpdatePartial", result);
        }

        public IActionResult EmployeeDetails( long EmployeeId )
        {
            VmEmployeeCreate vmEmployeeCreate = new VmEmployeeCreate();
            var Data = db.Employee.GetFirstOrDefaultWithRelatedData(model => model.Id == EmployeeId);
           // Data.PhotoUrl = _imagePath.GetFilePathAsSourceUrl(Data.PhotoUrl);
            var result = EmployeeUpdateMethod(vmEmployeeCreate, Data);

            return PartialView("EmployeeDetailsPartial", result);
        }
        [HttpPost]
        public IActionResult EmployeeUpdate( VmEmployeeCreate vm )
        {
            if (ModelState.IsValid)
            {
                var result = db.Employee.GetFirstOrDefault(c => c.Id == vm.Id);

                result.MaskingId = vm.MaskingId;
                result.AttendanceMachineId = vm.AttendanceMachineId;
                result.FirstName = vm.FirstName;
                result.LastName = vm.LastName;
                result.JobDescription = vm.JobDescription;
                result.JoinDate = Convert.ToDateTime(vm.JoinDate);
                result.CompanyId = vm.CompanyId;
                result.BranchId = vm.BranchId;
                result.DesignationId = vm.DesignationId;
                result.DepartmentId = vm.DepartmentId;
                result.SectionId = vm.SectionId;
                result.ShiftId = vm.ShiftId;
                result.FloorId = vm.FloorId;
                result.LineId = vm.LineId;
                result.MachineId = vm.MachineId;
                result.JobStatus = vm.JobStatus;
                result.BirthCertificate = vm.BirthCertificate;
                result.BirthCertificateUrl = vm.BirthCertificateUrl;
                result.BirthPlace = vm.BirthPlace;
                result.BloodGroup = vm.BloodGroup;
                result.DrivingLicense = vm.DrivingLicense;
                result.DrivingLicenseUrl = vm.DrivingLicenseUrl;
                result.Email = vm.Email;
                result.EmergencyContactNumber = vm.EmergencyContactNumber;
                result.EmergencyContactPersonName = vm.EmergencyContactPersonName;
                result.EmergencyContactPersonRelation = vm.EmergencyContactPersonRelation;
                result.FatherName = vm.FatherName;
                result.Gender = vm.Gender;
                result.MaritalStatus = vm.MaritalStatus;
                result.MotherName = vm.MotherName;
                result.NID = vm.NID;
                result.Nationality = vm.Nationality;
                result.NidUrl = vm.NidUrl;
                result.OtherDocumentTitle = vm.OtherDocumentTitle;
                result.OtherDocumentUrl = vm.OtherDocumentUrl;
                result.Passport = vm.Passport;
                result.PassportUrl = vm.PassportUrl;
                result.PermanentAddress = vm.PermanentAddress;
                result.Phone = vm.Phone;
                result.PresentAddress = vm.PresentAddress;
                result.Reference = vm.Reference;
                result.Remarks = vm.Remarks;
                result.Religion = vm.Religion;
                result.SpecialCase = vm.SpecialCase;
                result.SpouseName = vm.SpouseName;
                result.EmployeeGroupId = vm.EmployeeGroupId;
                result.SuperirorId = vm.SuperiorId;
                result.JobLocationId = vm.JobLocationId;
                result.MachineId = vm.MachineId;
                result.EmergencyContactNumber = vm.EmergencyContactNumber;
                result.EmergencyContactPersonName = vm.EmergencyContactPersonName;
                result.EmergencyContactPersonRelation = vm.EmergencyContactPersonRelation;

                if (vm.PhotoUrl != null)
                {
                    var extn = string.IsNullOrEmpty(Path.GetExtension(vm.PhotoUrl.FileName)) ? ".jpg" : Path.GetExtension(vm.PhotoUrl.FileName);
                    var fileName = vm.MaskingId + extn;
                    var path = _imagePath.GetImagePath(fileName, "Uploads", "Employee");
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        vm.PhotoUrl.CopyTo(stream);
                    }
                    result.PhotoUrl = _imagePath.GetImagePathForDb(path);
                }

                db.Save();

                //employee leave
                if (vm.LeaveList.Count > 0)
                {
                    var LeaveListValues = db.EmployeeLeave.GetFirstOrDefault(x => x.EmployeeId == vm.Id);
                    if (LeaveListValues != null)
                    {
                        db.EmployeeLeave.Remove(LeaveListValues);
                        db.Save();
                        foreach (var item in vm.LeaveList)
                        {
                            var leave = new EmployeeLeave
                            {
                                Allocate = item.Allocate,
                                LeaveId = item.LeaveId,
                                EmployeeId = vm.Id,
                                Enjoyed = 0,
                                Year = DateTime.Now.Year
                            };
                            db.EmployeeLeave.Add(leave);
                            db.Save();
                        }
                    }
                }

                //employee salary
                if (vm.SalaryStructureList.Count > 0)
                {
                    var SalaryStructure = db.EmployeeSalaryBase.GetFirstOrDefault(d => d.EmployeeId == vm.Id);
                    if (SalaryStructure != null)
                    {
                        db.EmployeeSalaryBase.Remove(SalaryStructure);
                        db.Save();
                        foreach (var item in vm.SalaryStructureList)
                        {
                            if (vm.SalaryGrade != null)
                            {
                                var salary = new EmployeeSalaryBase()
                                {
                                    SalaryStructureId = vm.SalaryGrade.Value,
                                    SalaryBreakupId = item.SalaryBreakupId,
                                    Amount = item.Amount,
                                    EffectiveFrom = DateTime.Now,
                                    Remarks = item.Remarks,
                                    EmployeeId = vm.Id,
                                };
                                db.EmployeeSalaryBase.Add(salary);
                                db.Save();
                            }

                        }

                    }
                }


                ModelState.Clear();
                VmEmployeeCreate something = new VmEmployeeCreate();
                something = GetVmEmployeeCreateAdditionalInfo(something);
                return PartialView ("EmployeeUpdatePartial", something);
                
            }
            VmEmployeeCreate anotherVm = new VmEmployeeCreate();
            anotherVm = GetVmEmployeeCreateAdditionalInfo(anotherVm);
            return PartialView("EmployeeUpdatePartial", anotherVm);
        }


        public JsonResult getEmployeeNo()
        {
            var employees = db.Employee.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false).ToList();
            var data =new {current= employees.Count(x => x.JobStatus == JobStatus.Confirmed || x.JobStatus == JobStatus.Probation),
                            suspended = employees.Count(x => x.JobStatus == JobStatus.Suspended),
                            terminated = employees.Count(x => x.JobStatus == JobStatus.Terminated),
                            former = employees.Count(x => x.JobStatus == JobStatus.Retired || x.JobStatus == JobStatus.Resigned)};
            return Json(data);
        }

      
        public JsonResult todaysBusiness()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var onLeaveToday = db.BusinessApplication.FindWithRelatedData(x => x.FromDate <= Today && x.ToDate >= Tommorow);
            var todaysLeaveList = new List<TodaysLeave>();
            foreach (var item in onLeaveToday)
            {
                var days = item.ToDate.AddDays(1) - item.FromDate;
                var leave = new TodaysLeave
                {
                    Name = item.Employee.FullName,
                    EmployeeId = item.Employee.MaskingId,
                    Remarks = item.Comments ?? "",
                    Days = days.Days.ToString(),
                    From = item.FromDate.ToShortDateString(),
                    To = item.ToDate.ToShortDateString(),
                    Status = item.Status.ToString(),
                    Purpose = item.Purpose,
                };
                if (item.ApproverId != null)
                {
                    leave.ApprovedBy = "(" + item.Approver.MaskingId + ") " + item.Approver.FullName;
                    leave.ApprovedTime = item.ApprovedTime.Value.ToString();
                }
                else
                {
                    leave.ApprovedBy = "";
                    leave.ApprovedTime = "";
                }
                if (!string.IsNullOrEmpty(item.Employee.PhotoUrl))
                {
                    leave.PhotoUrl = _imagePath.GetFilePathAsSourceUrl(item.Employee.PhotoUrl);
                }

                todaysLeaveList.Add(leave);
            }


            //total number of rows count     
            recordsTotal = todaysLeaveList.Count();

            //Paging     
            var data = todaysLeaveList.Skip(skip).Take(pageSize).ToList();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
        public IActionResult current()
        {
            var vm = new vmEmployeeDashboard();
            return View(vm);
        }

        public IActionResult Import()
        {
            return PartialView("Import");
        }


        public JsonResult VerifyImportFile(IFormFile file)
        {
            var msg = "File Name: " + file.FileName + " ";
            var fields = new List<string>();
            var extension = Path.GetExtension(file.FileName);
            var count = 1;
            var line = 1;

            if (extension == ".xls" || extension == ".xlsx")
            {
                using (var stream = file.OpenReadStream())
                {
                    IExcelDataReader reader;
                    switch (extension)
                    {
                        case ".xls":
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                            break;
                        case ".xlsx":
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            break;
                        default:
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            break;
                    }

                    var isHeading = true;
                    var benefits = new List<string>();
                    while (reader != null && reader.Read())
                    {
                        if (isHeading)
                        {

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                fields.Add(reader.GetValue(i).ToString());
                            }

                            for (int i = 37; i < reader.FieldCount; i++)
                            {
                                benefits.Add(reader.GetValue(i).ToString());
                            }
                            //check benefit
                            foreach (var item in benefits)
                            {
                                var isAvailable = db.SalaryBreakup.GetAll().Where(x => x.SalaryGroup == SalaryGroup.Benefit).FirstOrDefault(x => x.Name.ToLower() == item.ToLower().Trim());
                                if (isAvailable == null)
                                {
                                    msg += "Given benefit name '" + item + "' does not exist in database!";
                                    // break;
                                }
                            }
                            isHeading = false; continue;
                        }

                        try
                        {

                            //check employee record
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i))
                                {
                                    if (i == 0 || i == 5 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11 || i == 12 || i == 13 || i == 14 || i == 15 || i == 16 || i == 17 || i == 18 || i == 19 || i == 20 || i == 21 || i == 23 || i == 24 || i == 35 || i == 36)
                                    {
                                        continue;
                                    }
                                    msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ": is Required.";
                                    line++;
                                }
                                else
                                {
                                    //check join date
                                    if (i == 11)
                                    {
                                        string param = reader.GetString(9);
                                        var result = CheckDate(param);
                                        if (!result)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ": date format invalid.";
                                            line++;
                                        }
                                    }
                                    //check company
                                    if (i == 0)
                                    {
                                        var result = db.Company.GetFirstOrDefault(x => x.CompanyName.ToLower() == reader.GetString(i).Trim().ToLower());
                                        if (result == null)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ":  does not found.";
                                            line++;
                                        }
                                    }

                                    //check employeeId
                                    if (i == 7)
                                    {
                                        var result = db.Employee.GetFirstOrDefault(x => x.MaskingId.ToLower() == reader.GetString(i).Trim().ToLower());
                                        if (result != null)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ":  is already exist.";
                                            line++;
                                        }
                                    }
                                    //check attendance machine id
                                    if (i == 8)
                                    {
                                        var result = db.Employee.GetFirstOrDefault(x => x.AttendanceMachineId.ToLower() == reader.GetString(i).Trim().ToLower());
                                        if (result != null)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ": is already exist.";
                                            line++;
                                        }
                                    }
                                    //check shift
                                    if (i == 12)
                                    {
                                        var result = db.Shift.GetFirstOrDefault(x => x.Name.ToLower() == reader.GetString(i).Trim().ToLower());
                                        if (result == null)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ": does not found.";
                                            line++;
                                        }
                                    }
                                    //check employee group
                                    if (i == 18)
                                    {
                                        var result = db.EmployeeGroup.GetFirstOrDefault(x => x.Name.ToLower() == reader.GetString(i).Trim().ToLower());
                                        if (result == null)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ": does not found.";
                                            line++;
                                        }
                                    }
                                    //check job status
                                    if (i == 16)
                                    {
                                        var result = (JobStatus) Enum.Parse(typeof(JobStatus),
                                            reader.GetString(i).Trim());
                                        if (result == null)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ": does not found.";
                                            line++;
                                        }
                                    }
                                    //check job location
                                    if (i == 17)
                                    {
                                        var result = db.JobLocation.GetFirstOrDefault(x => x.Name.ToLower() == reader.GetString(i).Trim().ToLower());
                                        if (result == null)
                                        {
                                            msg += "</br>" + line + ". Row: " + count + " =>  " + fields[i] + ":  does not found.";
                                            line++;
                                        }
                                    }
                                }

                            }

                            count++;

                        }
                        catch (Exception e)
                        {
                            msg += "</br>" + e.Message;
                            line++;
                        }

                    }

                }

            }


            if (msg == "File Name: " + file.FileName + " ")
            {
                msg = null;
            }




            return Json(msg);
        }

        protected bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(date, new string[] { "MM/dd/yyyy" , "dd/MM/yyyy", "yyyy/MM/dd" },CultureInfo.InvariantCulture);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public IActionResult Create()
        {
            var result = db.Employee.GetAll().ToList();
            List<VmEmployeeCreate> vmEmployeeList = new List<VmEmployeeCreate>();

            foreach (var item in result)
            {
                vmEmployeeList.Add(new VmEmployeeCreate
                {
                    MaskingId = item.MaskingId,
                    Combine = item.MaskingId + " || " + item.FullName
                });
            }

            ViewBag.item = new SelectList(vmEmployeeList, "MaskingId", "Combine");

            VmEmployeeCreate vm = new VmEmployeeCreate();
            vm = GetVmEmployeeCreateAdditionalInfo(vm);

            return PartialView("Create", vm);
        }

        [HttpPost]
        public IActionResult Create(VmEmployeeCreate vm)
        {
            if (ModelState.IsValid)
            {
                var employee = new Employee
                {
                    MaskingId = vm.MaskingId,
                    AttendanceMachineId = vm.AttendanceMachineId,
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    JobDescription = vm.JobDescription,
                    JoinDate = Convert.ToDateTime(vm.JoinDate),
                    CompanyId = vm.CompanyId,
                    BranchId = vm.BranchId,
                    DesignationId = vm.DesignationId,
                    DepartmentId = vm.DepartmentId,
                    SectionId = vm.SectionId,
                    ShiftId = vm.ShiftId,
                    FloorId = vm.FloorId,
                    LineId = vm.LineId,
                    MachineId = vm.MachineId,
                    JobStatus = vm.JobStatus,
                    BirthCertificate = vm.BirthCertificate,
                    BirthCertificateUrl = vm.BirthCertificateUrl,
                    BirthPlace = vm.BirthPlace,
                    BloodGroup = vm.BloodGroup,
                    DrivingLicense = vm.DrivingLicense,
                    DrivingLicenseUrl = vm.DrivingLicenseUrl,
                    Email = vm.Email,
                    EmergencyContactNumber = vm.EmergencyContactNumber,
                    EmergencyContactPersonName = vm.EmergencyContactPersonName,
                    EmergencyContactPersonRelation = vm.EmergencyContactPersonRelation,
                    FatherName = vm.FatherName,
                    Gender = vm.Gender,
                    MaritalStatus = vm.MaritalStatus,
                    MotherName = vm.MotherName,
                    NID = vm.NID,
                    Nationality = vm.Nationality,
                    NidUrl = vm.NidUrl,
                    OtherDocumentTitle = vm.OtherDocumentTitle,
                    OtherDocumentUrl = vm.OtherDocumentUrl,
                    Passport = vm.Passport,
                    PassportUrl = vm.PassportUrl,
                    PermanentAddress = vm.PermanentAddress,
                    Phone = vm.Phone,
                    PresentAddress = vm.PresentAddress,
                    Reference = vm.Reference,
                    Remarks = vm.Remarks,
                    Religion = vm.Religion,
                    SpecialCase = vm.SpecialCase,
                    SpouseName = vm.SpouseName,
                    EmployeeGroupId = vm.EmployeeGroupId,
                    SuperirorId = vm.SuperiorId,
                    JobLocationId = vm.JobLocationId,
                    
                };
                if (vm.PhotoUrl != null)
                {
                    var extn = string.IsNullOrEmpty(Path.GetExtension(vm.PhotoUrl.FileName)) ? ".jpg" : Path.GetExtension(vm.PhotoUrl.FileName);
                    var fileName = vm.MaskingId + extn;
                    var path = _imagePath.GetImagePath(fileName, "Uploads", "Employee");
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        vm.PhotoUrl.CopyTo(stream);
                    }
                    employee.PhotoUrl = _imagePath.GetImagePathForDb(path);
                }
                db.Employee.Add(employee);
                db.Save();

                //employee leave
                foreach (var item in vm.LeaveList)
                {
                    var leave = new EmployeeLeave
                    {
                        Allocate = item.Allocate,
                        LeaveId = item.LeaveId,
                        EmployeeId = employee.Id,
                        Enjoyed = 0,
                        Year = DateTime.Now.Year
                    };
                    db.EmployeeLeave.Add(leave);
                    db.Save();
                }
                //employee salary
                foreach (var item in vm.SalaryStructureList)
                {
                    if (vm.SalaryGrade != null)
                    {
                        var salary = new EmployeeSalaryBase()
                        {
                            SalaryStructureId = vm.SalaryGrade.Value,
                            SalaryBreakupId = item.SalaryBreakupId,
                            Amount = item.Amount,
                            EffectiveFrom =DateTime.Now,
                            Remarks = item.Remarks,
                        };
                        db.EmployeeSalaryBase.Add(salary);
                        db.Save();
                    }

                }

                ModelState.Clear();
                VmEmployeeCreate model = new VmEmployeeCreate();
                model = GetVmEmployeeCreateAdditionalInfo(model);
                return PartialView("Create", model);
            }
            return PartialView("Create", vm);
        }

        private VmEmployeeCreate EmployeeUpdateMethod( VmEmployeeCreate vmEmployeeCreate, Employee Result )
        {
            var result = db.Employee.GetAll().ToList();
            List<VmEmployeeCreate> vmEmployeeList = new List<VmEmployeeCreate>();

            foreach (var item in result)
            {
                vmEmployeeList.Add(new VmEmployeeCreate
                {
                    MaskingId = item.MaskingId,
                    Combine = item.MaskingId + " || " + item.FullName
                });
            }

            ViewBag.item = new SelectList(vmEmployeeList, "MaskingId", "Combine");

            vmEmployeeCreate.CompanyList = db.Company.GetAll().OrderBy(o => o.CompanyName).Select(x => new SelectListItem() { Text = x.CompanyName, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.BranchList = db.Branch.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.DepartmentList = db.Department.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.SectionList = db.Section.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.DesignationList = db.Designation.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.LineList = db.Line.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.FloorList = db.Floor.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.MachineList = db.Machine.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.GenderList = _dropdownHelper.GetGenderDropdownList();
            vmEmployeeCreate.BloodGroupList = _dropdownHelper.GetBloodGroupDropdownList();
            vmEmployeeCreate.MaritialStatusList = _dropdownHelper.GetMaritalStatusDropdownList();
            vmEmployeeCreate.ReligionList = _dropdownHelper.GetReligionDropdownList();
            vmEmployeeCreate.EmployeeGroupList = db.EmployeeGroup.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.SuperiorList = db.Employee.GetAll().OrderBy(o => o.FullName).Select(x => new SelectListItem() { Text = x.FullName, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.JobLocationList = db.JobLocation.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.ShiftList = db.Shift.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            // select dropdown value
            //Official Information

            // Get All Name
            vmEmployeeCreate.CompanyName = Result.Company.CompanyName;
            vmEmployeeCreate.DepartmentName = Result.Department.Name;
            vmEmployeeCreate.FloorName = Result.Floor != null ? Result.Floor.Name: "";
            vmEmployeeCreate.JobLocationName = Result.JobLocation != null? Result.JobLocation.Name: "";
            vmEmployeeCreate.BranchName = Result.Branch != null ? Result.Branch.Name: "";
            vmEmployeeCreate.LineName = Result.Line != null? Result.Line.Name: "";
            vmEmployeeCreate.EmployeeGroupName = Result.EmployeeGroup != null ? Result.EmployeeGroup.Name: "";
            vmEmployeeCreate.SuperiorName = Result.Superiror != null ? Result.Superiror.FullName : "";
            vmEmployeeCreate.SectionName = Result.Section != null ? Result.Section.Name: "";
            vmEmployeeCreate.ShiftName = Result.Shift != null ? Result.Shift.Name: "";
            string GetPhotoUrl = Result.PhotoUrl != null ? _imagePath.GetFilePathAsSourceUrl(Result.PhotoUrl) : "";
            vmEmployeeCreate.NewPhotoUrl = GetPhotoUrl; 
            //
            vmEmployeeCreate.CompanyId = Result.CompanyId;
            
            vmEmployeeCreate.DepartmentId = Result.DepartmentId;
            //vmEmployeeCreate.SuperiorId = Result.SuperiorId;

            vmEmployeeCreate.FloorId = Result.FloorId;
            vmEmployeeCreate.JoinDate = Result.JoinDate.ToShortDateString();
            vmEmployeeCreate.FirstName = Result.FirstName;
            vmEmployeeCreate.JobLocationId = Result.JobLocationId;
            vmEmployeeCreate.BranchId = Result.BranchId;
            vmEmployeeCreate.DesignationId = Result.DesignationId;
            vmEmployeeCreate.LineId = Result.LineId;
            vmEmployeeCreate.Id = Result.Id;
            vmEmployeeCreate.LastName = Result.LastName;
            vmEmployeeCreate.EmployeeGroupId = Result.EmployeeGroupId;
            vmEmployeeCreate.SectionId = Result.SectionId;
            vmEmployeeCreate.ShiftId = Result.ShiftId;
            vmEmployeeCreate.MachineId = Result.MachineId;
            vmEmployeeCreate.JobStatus = Result.JobStatus;
            vmEmployeeCreate.MaskingId = Result.MaskingId;
            vmEmployeeCreate.SpecialCase = Result.SpecialCase;
            vmEmployeeCreate.Overtime = Result.OverTime;
            vmEmployeeCreate.Incentive = Result.Incentive;
            vmEmployeeCreate.ProvidendFund = Result.ProvidendFund;
            vmEmployeeCreate.Gratuity = Result.Gratuity;
            //end official information

            vmEmployeeCreate.JobDescription = Result.JobDescription;
            // Job Description

            //personal information
            vmEmployeeCreate.FatherName = Result.FatherName;
            vmEmployeeCreate.MotherName = Result.MotherName;
            vmEmployeeCreate.Phone = Result.Phone;
            vmEmployeeCreate.DateofBirth = Result.DateofBirth;
            vmEmployeeCreate.Gender = Result.Gender;
            vmEmployeeCreate.BloodGroup = Result.BloodGroup;
            vmEmployeeCreate.MaritalStatus = Result.MaritalStatus;
            vmEmployeeCreate.PresentAddress = Result.PresentAddress;
            vmEmployeeCreate.PermanentAddress = Result.PermanentAddress;
            vmEmployeeCreate.Email = Result.Email;
            vmEmployeeCreate.BirthPlace = Result.BirthPlace;
            vmEmployeeCreate.Religion = Result.Religion;
            vmEmployeeCreate.Nationality = Result.Nationality;
            vmEmployeeCreate.SpouseName = Result.SpouseName;
            vmEmployeeCreate.NID = Result.NID;
            vmEmployeeCreate.Passport = Result.Passport;
            vmEmployeeCreate.DrivingLicense = Result.DrivingLicense;
            vmEmployeeCreate.BirthCertificate = Result.BirthCertificate;
            vmEmployeeCreate.Reference = Result.Reference;
            vmEmployeeCreate.Remarks = Result.Remarks;
            vmEmployeeCreate.EmergencyContactPersonName = Result.EmergencyContactPersonName;
            vmEmployeeCreate.EmergencyContactNumber = Result.EmergencyContactNumber;
            vmEmployeeCreate.EmergencyContactPersonRelation = Result.EmergencyContactPersonRelation;
            

            
            //end personal information

            foreach (var item in db.Leave.GetAll().Where(x => x.IsDefault == true))
            {
                vmEmployeeCreate.LeaveList.Add(new vmEmployeeLeave { Allocate = item.Allocate, LeaveName = item.Name, LeaveId = item.Id });
            }

            foreach (var item in db.SalaryStructure.GetAll())
            {
                vmEmployeeCreate.SalaryGradeList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            vmEmployeeCreate.LeaveEffectiveYear = DateTime.Now.Year;

            return vmEmployeeCreate;
        }
        private VmEmployeeCreate GetVmEmployeeCreateAdditionalInfo(VmEmployeeCreate vmEmployeeCreate)
        {

            vmEmployeeCreate.CompanyList = db.Company.GetAll().OrderBy(o=>o.CompanyName).Select(x => new SelectListItem(){Text = x.CompanyName, Value = x.Id.ToString()}).ToList();
            vmEmployeeCreate.BranchList = db.Branch.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() {Text = x.Name, Value = x.Id.ToString()}).ToList();
            vmEmployeeCreate.DepartmentList = db.Department.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem(){ Text = x.Name, Value = x.Id.ToString()}).ToList();
            vmEmployeeCreate.SectionList = db.Section.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.DesignationList = db.Designation.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.LineList = db.Line.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.FloorList = db.Floor.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.MachineList = db.Machine.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.GenderList = _dropdownHelper.GetGenderDropdownList();
            vmEmployeeCreate.BloodGroupList = _dropdownHelper.GetBloodGroupDropdownList();
            vmEmployeeCreate.MaritialStatusList = _dropdownHelper.GetMaritalStatusDropdownList();
            vmEmployeeCreate.ReligionList = _dropdownHelper.GetReligionDropdownList();
            vmEmployeeCreate.EmployeeGroupList = db.EmployeeGroup.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.SuperiorList = db.Employee.GetAll().OrderBy(o => o.FullName).Select(x => new SelectListItem() { Text = x.FullName, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.JobLocationList = db.JobLocation.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            vmEmployeeCreate.ShiftList = db.Shift.GetAll().OrderBy(o => o.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            foreach (var item in db.Leave.GetAll().Where(x=>x.IsDefault==true))
            {
                vmEmployeeCreate.LeaveList.Add(new vmEmployeeLeave{Allocate = item.Allocate,LeaveName = item.Name,LeaveId = item.Id});
            }

            foreach (var item in db.SalaryStructure.GetAll())
            {
                vmEmployeeCreate.SalaryGradeList.Add(new SelectListItem{Text = item.Name,Value = item.Id.ToString()});
            }

            vmEmployeeCreate.LeaveEffectiveYear = DateTime.Now.Year;
            
            return vmEmployeeCreate;
        }


        public JsonResult GetSalaryStructure(int id)
        {
            var vm = new VmEmployeeCreate();
            foreach (var item in db.SalaryStructureDetails.GetAllWithRelation().Where(x=>x.SalaryStructureId==id))
            {
                vm.SalaryStructureList.Add(new vmEmployeeSalaryStructure
                {
                    SalaryGroup = item.SalaryBreakup.SalaryGroup.ToString(),
                    Amount = item.Amount,
                    Percentage = item.Percentage,
                    SalaryBreakupId = item.SalaryBreakupId,
                    SalaryBreakupName = item.SalaryBreakup.Name,
                    SalaryStructureId = item.SalaryStructureId,
                    SalaryStructureName = item.SalaryStructure.Name,
                    EffectiveFrom = DateTime.Now.ToString("yyyy/MM/dd")
                });
            }
            return Json(vm.SalaryStructureList);
        }




        [DisableRequestSizeLimit]
        public async Task<IActionResult> ImportEmployees(List<IFormFile> files)
        {
            int Totalrecords = 0;
            int successfull = 0;
            int failed = 0;
            if (files != null && files.Count > 0)
            {
                var employeeList = new List<vmEmployeeImport>();
                foreach (IFormFile item in files)
                {
                    var extension = Path.GetExtension(item.FileName);
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" ||
                        extension.ToLower() == ".png" || extension.ToLower() == ".gif" || extension.ToLower() == ".bmp")
                    {
                        var filename = Path.GetFileNameWithoutExtension(item.FileName);
                        var employeeData = db.Employee.GetFirstOrDefault(x => x.MaskingId == filename);
                        if (employeeData != null)
                        {
                            _imagePath.RemoveFileOfPath(employeeData.PhotoUrl);
                            employeeData.PhotoUrl = await _imagePath.SaveToFolderAndReturnPathForEmployee(item,employeeData.MaskingId);
                            db.Employee.Update(employeeData);
                            db.Save();
                            successfull++;
                            Totalrecords++;
                        }
                        else
                        {
                            failed++;
                        }
                        continue;
                    }

                    using (var stream = item.OpenReadStream())
                    {
                        IExcelDataReader reader;
                        switch (extension)
                        {
                            case ".xls":
                                reader = ExcelReaderFactory.CreateBinaryReader(stream);
                                break;
                            case ".xlsx":
                                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                                break;
                            default:
                                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                                break;
                        }

                        var isHeading = true;
                        var benefits = new List<string>();
                        while (reader != null && reader.Read())
                        {

                            if (isHeading)
                            {
                                for (int i = 33; i < reader.FieldCount; i++)
                                {
                                    benefits.Add(reader.GetValue(i).ToString().ToLower());
                                }
                                isHeading = false; continue;
                            }


                              var employee = new vmEmployeeImport();
                              employee.Company = Convert.ToString(reader.GetValue(0));
                              employee.SisterConcern = Convert.ToString(reader.GetValue(1));
                              employee.Division = Convert.ToString(reader.GetValue(2));
                              employee.Branch = Convert.ToString(reader.GetValue(3));
                              employee.Department = Convert.ToString(reader.GetValue(4));
                              employee.Section = Convert.ToString(reader.GetValue(5));
                              employee.Designation = Convert.ToString(reader.GetValue(6));
                              employee.MaskingId = Convert.ToString(reader.GetValue(7));
                              employee.AttendanceMachineId = Convert.ToString(reader.GetValue(8));
                              employee.FirstName = Convert.ToString(reader.GetValue(9));
                              employee.LastName = Convert.ToString(reader.GetValue(10));
                              employee.JoinDate = Convert.ToString(reader.GetValue(11));
                              employee.Shift = Convert.ToString(reader.GetValue(12));
                              employee.OverTime = Convert.ToString(reader.GetValue(13));
                              employee.Incentive = Convert.ToString(reader.GetValue(14));
                              employee.Weekend = Convert.ToString(reader.GetValue(15));
                              employee.JobStatus = Convert.ToString(reader.GetValue(16));
                              employee.JobLocation = Convert.ToString(reader.GetValue(17));
                              employee.EmployeeGroup = Convert.ToString(reader.GetValue(18));
                              employee.FatherName = Convert.ToString(reader.GetValue(19));
                              employee.MotherName = Convert.ToString(reader.GetValue(20));
                              employee.Phone = Convert.ToString(reader.GetValue(21));
                              employee.Email = Convert.ToString(reader.GetValue(22));
                              employee.Gender = Convert.ToString(reader.GetValue(23));
                              employee.Religion = Convert.ToString(reader.GetValue(24));
                              employee.DateofBirth = Convert.ToString(reader.GetValue(25));
                              employee.BirthPlace = Convert.ToString(reader.GetValue(26));
                              employee.NID = Convert.ToString(reader.GetValue(27));
                              employee.BloodGroup = Convert.ToString(reader.GetValue(28));
                              employee.MaritalStatus = Convert.ToString(reader.GetValue(29));
                              employee.SpouseName = Convert.ToString(reader.GetValue(30));
                              employee.PresentAddress = Convert.ToString(reader.GetValue(31));
                              employee.PermanentAddress = Convert.ToString(reader.GetValue(32));
                              employee.Nationality = Convert.ToString(reader.GetValue(33));
                              employee.Remarks = Convert.ToString(reader.GetValue(34));
                              employee.SalaryGrade = Convert.ToString(reader.GetValue(35));
                              employee.Gross = Convert.ToString(reader.GetValue(36));

                                for (int i = 37; i < reader.FieldCount; i++)
                                {
                                    employee.Benefits.Add(Convert.ToString(reader.GetValue(i)));
                                }

                                employeeList.Add(employee);
                                Totalrecords++;

                        }
                        foreach (var emp in employeeList)
                        {
                            try
                            {
                                var gross = Convert.ToDecimal(emp.Gross);
                                DateTime joindate = DateTime.ParseExact(emp.JoinDate, new string[] { "MM/dd/yyyy", "dd/MM/yyyy", "yyyy/MM/dd" }, CultureInfo.InvariantCulture);

                                var employee = new Employee();
                                employee.MaskingId = emp.MaskingId;
                                employee.AttendanceMachineId = emp.AttendanceMachineId;
                                employee.CompanyId = db.Common.getCompanyId(emp.Company);
                                employee.SisterConcernId = db.Common.getSisterConcernId(emp.SisterConcern, emp.Company);
                                employee.DivisionId = db.Common.getDivisionId(emp.Division, emp.Company);
                                employee.BranchId = db.Common.getBranchId(emp.Branch, emp.Company);
                                employee.DepartmentId = db.Common.getDepartmentId(emp.Department);
                                employee.SectionId = db.Common.getSectionId(emp.Section);
                                employee.DesignationId = db.Common.getDesignationId(emp.Designation);
                                employee.ShiftId = db.Common.getShiftId(emp.Shift);
                                employee.JoinDate = joindate;
                                employee.JobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), emp.JobStatus);
                                employee.JobLocationId = db.Common.getJobLocationId(emp.JobLocation);
                                employee.EmployeeGroupId = db.Common.getEmployeeGroupId(emp.EmployeeGroup);
                                employee.FirstName = emp.FirstName;
                                employee.LastName = emp.LastName;
                                employee.FatherName = emp.FatherName;
                                employee.MotherName = emp.MotherName;
                                employee.SpouseName = emp.SpouseName;
                                employee.Phone = emp.Phone;
                                employee.Email = emp.Email;
                                employee.Gender = emp.Gender;
                                employee.Religion = emp.Religion;
                                employee.DateofBirth = emp.DateofBirth;
                                employee.BirthPlace = emp.BirthPlace;
                                employee.Nationality = emp.Nationality;
                                employee.BloodGroup = emp.BloodGroup;
                                employee.MaritalStatus = emp.MaritalStatus;
                                employee.PresentAddress = emp.PresentAddress;
                                employee.PermanentAddress = emp.PermanentAddress;
                                employee.NID = emp.NID;
                                employee.Remarks = emp.Remarks;
                                if (!string.IsNullOrEmpty(emp.OverTime))
                                {
                                    employee.OverTime = Convert.ToBoolean(emp.OverTime);
                                }
                                if (!string.IsNullOrEmpty(emp.Incentive))
                                {
                                    employee.Incentive = Convert.ToBoolean(emp.Incentive);
                                }
                                db.Employee.Add(employee);
                                db.Save();

                                //save employee weekend
                                string[] weekend = emp.Weekend.Split(',');
                                foreach (var w in weekend)
                                {
                                    var employeeWeekend = new EmployeeWeekend
                                    {
                                        EmployeeId = employee.Id,
                                        Dayname = w,
                                    };
                                    db.EmployeeWeekend.Add(employeeWeekend);
                                    db.Save();
                                }

                                // save employee salary
                                var salaryStructureId = db.Common.getSalaryStructureId(emp.SalaryGrade);
                                foreach (var s in db.SalaryStructureDetails.GetAllWithRelation().Where(x=>x.SalaryStructureId==salaryStructureId))
                                {
                                    if (s.SalaryBreakup.SalaryGroup == SalaryGroup.Gross)
                                    {
                                        var percent = Convert.ToDecimal(s.Percentage);
                                        var salary = new EmployeeSalaryBase
                                        {
                                            SalaryStructureId = salaryStructureId,
                                            SalaryBreakupId = s.SalaryBreakupId,
                                            Amount = (gross * percent) / 100,
                                            EffectiveFrom = joindate,
                                            EmployeeId = employee.Id
                                        };
                                        db.EmployeeSalaryBase.Add(salary);
                                        db.Save();
                                    }
                                    else 
                                    {
                                        for (int i = 0; i < benefits.Count; i++)
                                        {
                                            if (benefits[i] == s.SalaryBreakup.Name.ToLower())
                                            {
                                                var amount = Convert.ToDecimal(emp.Benefits[i]);
                                                var salary = new EmployeeSalaryBase
                                                {
                                                    SalaryStructureId = salaryStructureId,
                                                    SalaryBreakupId = s.SalaryBreakupId,
                                                    Amount =amount,
                                                    EffectiveFrom = joindate,
                                                    EmployeeId = employee.Id
                                                };
                                                db.EmployeeSalaryBase.Add(salary);
                                                db.Save();
                                                break;
                                            }
                                        }

                                    }

                                }

                             

                                successfull++;
                            }
                            catch (Exception ex)
                            {

                                failed++;
                                continue;

                            }

                        }
                    }
                }

                return Json(new { Totalrecords, successfull, failed });
            }
            else
            {
                return this.Content("Fail");
            }

        }



        [HttpPost]
        public JsonResult loadEmployees()
        {
            CultureInfo culture = new CultureInfo("en-US");
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var keyword = Request.Form["keyword"].FirstOrDefault();
            var joinYear = Request.Form["joinYear"].FirstOrDefault();
            var joinMonth = Request.Form["joinMonth"].FirstOrDefault();
            var companyId = Request.Form["companyId"].FirstOrDefault();
            var departmentId = Request.Form["departmentId"].FirstOrDefault();
            var lineId = Request.Form["lineId"].FirstOrDefault();
            var gender = Request.Form["gender"].FirstOrDefault();
            var specialCase = Request.Form["specialCase"].FirstOrDefault();
            var sisterConcernId = Request.Form["sisterConcernId"].FirstOrDefault();
            var designationId = Request.Form["designationId"].FirstOrDefault();
            var floorId = Request.Form["floorId"].FirstOrDefault();
            var religion = Request.Form["religion"].FirstOrDefault();
            var incentive = Request.Form["incentive"].FirstOrDefault();
            var divisionId = Request.Form["divisionId"].FirstOrDefault();
            var sectionId = Request.Form["sectionId"].FirstOrDefault();
            var employeeGroupId = Request.Form["employeeGroupId"].FirstOrDefault();
            var machineId = Request.Form["machineId"].FirstOrDefault();
            var maritalStatus = Request.Form["maritalStatus"].FirstOrDefault();
            var branchId = Request.Form["branchId"].FirstOrDefault();
            var shiftId = Request.Form["shiftId"].FirstOrDefault();
            var jobLocationId = Request.Form["jobLocationId"].FirstOrDefault();
            var jobStatusId = Request.Form["jobStatusId"].FirstOrDefault();
            var bloodGroup = Request.Form["bloodGroup"].FirstOrDefault();
            
         
            var departmentFromChart = Request.Form["departmentFromChart"].FirstOrDefault();
            var branchIdFromChart = Request.Form["branchIdFromChart"].FirstOrDefault();
            var designationIdFromChart = Request.Form["designationIdFromChart"].FirstOrDefault();
            var genderIdFromChart = Request.Form["genderIdFromChart"].FirstOrDefault();
            var joinYearIdFromChart = Request.Form["joinYearIdFromChart"].FirstOrDefault();
            var shiftIdFromChart = Request.Form["shiftIdFromChart"].FirstOrDefault();



            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var list = db.Employee.GetAllWithRelatedData(c=> c.IsActive == true && c.IsDeleted == false);
            List<Employee> employees = new List<Employee>();
            //replace photoURL path
            foreach (var item in list)
            {
                var emp = item;
                if (!string.IsNullOrEmpty(item.PhotoUrl))
                {
                    emp.PhotoUrl = _imagePath.GetFilePathAsSourceUrl(item.PhotoUrl);
                }
                else
                {
                    emp.PhotoUrl = _imagePath.GetFilePathAsSourceUrl("/images/Uploads/Employee/AlterImage.png");
                }
                employees.Add(emp);
            }



            if (!string.IsNullOrEmpty(departmentFromChart))
            {
                var dept = Convert.ToInt32(departmentFromChart);
                employees = employees.Where(x => x.DepartmentId == dept).ToList();
            }
            if (!string.IsNullOrEmpty(designationIdFromChart))
            {
                var desig = Convert.ToInt32(designationIdFromChart);
                employees = employees.Where(x => x.DesignationId == desig).ToList();
            }
            if (!string.IsNullOrEmpty(branchIdFromChart))
            {
                var bran = Convert.ToInt32(branchIdFromChart);
                employees = employees.Where(x => x.BranchId == bran).ToList();
            }
            if (!string.IsNullOrEmpty(genderIdFromChart))
            {
                employees = employees.Where(x => x.Gender == genderIdFromChart).ToList();
            }
            if (!string.IsNullOrEmpty(joinYearIdFromChart))
            {
                List<Employee> emp = new List<Employee>();
                foreach (var item in employees)
                {
                    try
                    {
                        string year = Convert.ToDateTime(item.JoinDate, culture).Year.ToString();
                        if (year.Length == 4)
                        {
                            if (year == joinYearIdFromChart)
                            {
                                emp.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                }
                employees = emp;
            }

            if (!string.IsNullOrEmpty(shiftIdFromChart))
            {
                employees = employees
                                .Where(x => x.ShiftId == int.Parse(shiftIdFromChart))
                                .ToList();
            }



            if (!string.IsNullOrEmpty(joinYear))
            {
                employees = employees.Where(x => x.JoinDate.Year == int.Parse(joinYear)).ToList();
            }
            if (!string.IsNullOrEmpty(joinMonth))
            {
                employees = employees.Where(x => x.JoinDate.Month == int.Parse(joinMonth)).ToList();
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                employees = employees.Where(x => x.CompanyId == int.Parse(companyId)).ToList();
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                employees = employees.Where(x => x.DepartmentId == int.Parse(departmentId)).ToList();
            }
            if (!string.IsNullOrEmpty(lineId))
            {
                employees = employees.Where(x => x.LineId == int.Parse(lineId)).ToList();
            }
            if (!string.IsNullOrEmpty(gender))
            {
                employees = employees.Where(x => x.Gender == gender).ToList();
            }
            if (!string.IsNullOrEmpty(specialCase))
            {
                employees = employees.Where(x => x.SpecialCase == Convert.ToBoolean(specialCase)).ToList();
            }
            if (!string.IsNullOrEmpty(sisterConcernId))
            {
                employees = employees.Where(x => x.SisterConcernId == int.Parse(sisterConcernId)).ToList();
            }
            if (!string.IsNullOrEmpty(designationId))
            {
                employees = employees.Where(x => x.DesignationId == int.Parse(designationId)).ToList();
            }
            if (!string.IsNullOrEmpty(floorId))
            {
                employees = employees.Where(x => x.FloorId == int.Parse(floorId)).ToList();
            }
            if (!string.IsNullOrEmpty(religion))
            {
                employees = employees.Where(x => x.Religion == religion).ToList();
            }
            if (!string.IsNullOrEmpty(incentive))
            {
                employees = employees.Where(x => x.Incentive == Convert.ToBoolean(incentive)).ToList();
            }
            if (!string.IsNullOrEmpty(divisionId))
            {
                employees = employees.Where(x => x.DivisionId == int.Parse(divisionId)).ToList();
            }
            if (!string.IsNullOrEmpty(sectionId))
            {
                employees = employees.Where(x => x.SectionId == int.Parse(sectionId)).ToList();
            }
            if (!string.IsNullOrEmpty(employeeGroupId))
            {
                employees = employees.Where(x => x.EmployeeGroupId == int.Parse(employeeGroupId)).ToList();
            }
            if (!string.IsNullOrEmpty(machineId))
            {
                employees = employees.Where(x => x.MachineId == int.Parse(machineId)).ToList();
            }
            if (!string.IsNullOrEmpty(maritalStatus))
            {
                employees = employees.Where(x => x.MaritalStatus == maritalStatus).ToList();
            }
            if (!string.IsNullOrEmpty(branchId))
            {
                employees = employees.Where(x => x.BranchId == int.Parse(branchId)).ToList();
            }
            if (!string.IsNullOrEmpty(shiftId))
            {
                employees = employees.Where(x => x.ShiftId == int.Parse(shiftId)).ToList();
            }
            if (!string.IsNullOrEmpty(jobLocationId))
            {
                employees = employees.Where(x => x.JobLocationId == int.Parse(jobLocationId)).ToList();
            }
            if (!string.IsNullOrEmpty(jobStatusId))
            {
                employees = employees.Where(x => x.JobStatus == (JobStatus)Enum.Parse(typeof(JobStatus), jobStatusId)).ToList();
            }
            if (!string.IsNullOrEmpty(bloodGroup))
            {
                employees = employees.Where(x => x.BloodGroup == bloodGroup).ToList();
            }








            //check job status
            if (searchValue == "current")
            {
                employees = employees.Where(x => x.JobStatus == JobStatus.Confirmed|| x.JobStatus == JobStatus.Probation).ToList();
                searchValue = "";
            }
            if (searchValue == "suspended")
            {
                employees = employees.Where(x => x.JobStatus == JobStatus.Suspended).ToList();
                searchValue = "";
            }
            if (searchValue == "terminate")
            {
                employees = employees.Where(x => x.JobStatus == JobStatus.Terminated).ToList();
                searchValue = "";
            }
            if (searchValue == "former")
            {
                employees = employees.Where(x => x.JobStatus == JobStatus.Retired || x.JobStatus == JobStatus.Resigned).ToList();
                searchValue = "";
            }


            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                employees = employees.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                employees = employees.OrderByDescending(o => o.MaskingId).ToList();
            }

          




            //Search    
            if (!string.IsNullOrEmpty(searchValue)|| !string.IsNullOrEmpty(keyword))
            {
                employees = employees.Where(m => m.MaskingId.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.AttendanceMachineId.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Company.CompanyName.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Department.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Shift.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Designation.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.EmployeeGroup.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.JobLocation.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.FullName.ToLower().Contains(searchValue.ToLower())).ToList();

            }

            recordsTotal = employees.Count();

            employees=employees.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = employees });


        }

        public JsonResult EmployeeChart(string type,int ? company,int ? sister, int ? division, int ? branch)
        {
            var Label = new List<string>();
            var Data = new List<string>();
            var Id = new List<string>();
            var employees = db.Employee.GetAll().ToList();
            if (company != null)
            {
                employees = employees.Where(x => x.CompanyId == company).ToList();
            }
            if (sister != null)
            {
                employees = employees.Where(x => x.SisterConcernId == sister).ToList();
            }
            if (division != null)
            {
                employees = employees.Where(x => x.DivisionId == division).ToList();
            }
            if (branch != null)
            {
                employees = employees.Where(x => x.BranchId == branch).ToList();
            }
            switch (type)
            {
                case "department":
                                    var departments = db.Department.GetAll();
                                    foreach (var item in departments)
                                    {
                                        var emp = employees.Where(x => x.DepartmentId == item.Id).ToList();
                                        if (emp.Count== 0)
                                        {
                                            continue;
                                        }
                                        Label.Add(item.Name);
                                        Data.Add(emp.Count().ToString());
                                        Id.Add(item.Id.ToString());
                                    }
                                    break;
                case "designation":
                                    var designations = db.Designation.GetAll();
                                    foreach (var item in designations)
                                    {
                                        var emp = employees.Where(x => x.DesignationId == item.Id).ToList();
                                        if (emp.Count == 0)
                                        {
                                            continue;
                                        }

                                        Label.Add(item.Name);
                                        Data.Add(emp.Count().ToString());
                                        Id.Add(item.Id.ToString());
                                    }
                                    break;
                case "shift":
                                    var shifts = db.Shift.GetAll();
                                    foreach (var item in shifts)
                                    {
                                        var emp = employees.Where(x => x.ShiftId == item.Id).ToList();
                                        if (emp.Count == 0)
                                        {
                                            continue;
                                        }
                                        Label.Add(item.Name);
                                        Data.Add(emp.Count().ToString());
                                        Id.Add(item.Id.ToString());
                                    }
                                    break;
                case "gender":
                                    var genders = _dropdownHelper.GetGenderDropdownList();
                                    foreach (var item in genders)
                                    {
                                        var emp = employees.Where(x => x.Gender == item.Value).ToList();
                                        if (emp.Count == 0)
                                        {
                                            continue;
                                        }
                                        Label.Add(item.Value);
                                        Data.Add(emp.Count().ToString());
                                        Id.Add(item.Value);
                                    }
                                    break;
                case "year":

                                    int invalid = 0;
                                    var year = new List<int>();
                                    foreach (var item in employees)
                                    {
                                        try
                                        {
                                            var date = DateTime.ParseExact(item.JoinDate.ToString(), new string[] { "MM/dd/yyyy", "dd/MM/yyyy", "yyyy/MM/dd" }, CultureInfo.InvariantCulture);
                                            year.Add(date.Year);
                                        }
                                        catch (Exception)
                                        {
                                            invalid++;
                                        }
                                    }
                                    foreach (var item in year.GroupBy(x => x.ToString()))
                                    {
                                        if (item.Key == "1")
                                        {
                                            invalid++;
                                            continue;
                                        }
                                       
                                        Label.Add(item.Key);
                                        Data.Add(item.Count().ToString());
                                        Id.Add(item.Key);
                                    }
                                    if (invalid > 0)
                                    {
                                        Label.Add("Invalid");

                                        Data.Add(invalid.ToString());

                                        Id.Add("Invalid");
                                    }
                                    break;
            }
            return Json(new { Label, Data, Id });
        }



        public JsonResult EmployeeShiftChart()
        {
            List<string> Label = new List<string>();

            List<string> Data = new List<string>();

            List<string> Id = new List<string>();

            

            return Json(new { Label, Data, Id });
        }

    }
}