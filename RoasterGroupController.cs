using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.RoasterGroup;
using System.Linq.Dynamic.Core;
using Pronali.Data.Enum;
using Microsoft.AspNetCore.Authorization;
using Pronali.Web.Services;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class RoasterGroupController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly RoasterService roasterService;

        public RoasterGroupController(ApplicationDbContext context)
        {
            db = context;
            roasterService = new RoasterService(context);
        }

        internal List<vMRoasterGroupDesignation> GetRoasteringInfoDesignationWise(EmployeeChoiceFlagForRoaster employeeListType, RoasterGroup roasterGroup)
        {
            List<vMRoasterGroupDesignation> vMRoasterGroupDesignationList = new List<vMRoasterGroupDesignation>();

            var shiftCount = roasterGroup.RoasterGroupDetailsList.Count();

            List<Employee> employeeList = new List<Employee>();

            if (employeeListType == EmployeeChoiceFlagForRoaster.AllEmployeeInShift)
            {
                employeeList = db.Employee
                    .Include(emp => emp.Designation)
                    .Include(emp => emp.Shift)
                    .Where(
                    emp => roasterGroup.RoasterGroupDetailsList.Any(rg => rg.ShiftId == emp.ShiftId)).ToList();
            }
            else
            {
                employeeList = roasterGroup.RoasterGroupEmployeeList.Select(c => c.Employee).ToList();
            }

            foreach (var employee in employeeList)
            {
                if (!vMRoasterGroupDesignationList.Any(rsg => rsg.DesignationId == employee.DesignationId))
                {
                    vMRoasterGroupDesignationList
                        .Add(new vMRoasterGroupDesignation()
                        {
                            DesignationId = employee.DesignationId,
                            DesignationName = employee.Designation.Name
                        });
                }

                var employeeToAdd = new vMRoasterGroupEmployee();

                employeeToAdd.EmployeeId = employee.MaskingId;
                employeeToAdd.EmployeeName = employee.FullName;
                employeeToAdd.StartingShift = employeeListType == EmployeeChoiceFlagForRoaster.AllEmployeeInShift ? employee.ShiftId : roasterGroup.RoasterGroupEmployeeList.FirstOrDefault(c => c.EmployeeId == employee.Id).Shift != null ? roasterGroup.RoasterGroupEmployeeList.FirstOrDefault(c => c.EmployeeId == employee.Id).StartingShiftId : null;
                employeeToAdd.StartingShiftName = employeeListType == EmployeeChoiceFlagForRoaster.AllEmployeeInShift ? employee.Shift.Name : roasterGroup.RoasterGroupEmployeeList.FirstOrDefault(c => c.EmployeeId == employee.Id).Shift != null ? roasterGroup.RoasterGroupEmployeeList.FirstOrDefault(c => c.EmployeeId == employee.Id).Shift.Name : "N/A";
                employeeToAdd.IsAlreadyAdded = roasterGroup.RoasterGroupEmployeeList.Any(c => c.EmployeeId == employee.Id);
                employeeToAdd.ShiftList = roasterGroup.RoasterGroupDetailsList.Select(c => new SelectListItem() { Text = c.Shift.Name, Value = c.ShiftId.ToString() }).ToList();

                vMRoasterGroupDesignationList
                    .FirstOrDefault(c => c.DesignationId == employee.DesignationId)
                    .vMRoasterGroupEmployeeList
                    .Add(employeeToAdd);
            }

            foreach (var designationGroup in vMRoasterGroupDesignationList)
            {
                int MinimumNumberOfEmployee = shiftCount;
                int NumberOfEmployeeNeededPerShift = Convert.ToInt32(Math.Ceiling(MinimumNumberOfEmployee / Convert.ToDouble(shiftCount)));
                int NumberOfEmployeeNeededPerShiftWithExtra = NumberOfEmployeeNeededPerShift + Convert.ToInt32(Math.Ceiling(NumberOfEmployeeNeededPerShift * (0.17)));
                int NumberOfExtraEmployeeNeededPerShift = NumberOfEmployeeNeededPerShiftWithExtra - NumberOfEmployeeNeededPerShift;

                designationGroup.AvailableEmployee = designationGroup.vMRoasterGroupEmployeeList.Count();
                designationGroup.MinimumWorkingEmployee = NumberOfEmployeeNeededPerShift * shiftCount;
                designationGroup.MinimumTotalEmployeeNeeded = NumberOfEmployeeNeededPerShiftWithExtra * shiftCount;
                designationGroup.MinimumExtraEmployeeNeeded = NumberOfExtraEmployeeNeededPerShift * shiftCount;

                designationGroup.ShiftWiseAvailableEmployee = new List<SelectListItem>();
                foreach (var shiftGroup in designationGroup.vMRoasterGroupEmployeeList.GroupBy(c => c.StartingShift))
                {
                    var selectList = new SelectListItem();
                    selectList.Text = shiftGroup.FirstOrDefault().StartingShiftName;
                    selectList.Value = shiftGroup.Count().ToString();

                    designationGroup.ShiftWiseAvailableEmployee.Add(selectList);
                }

                if (designationGroup.AvailableEmployee < designationGroup.MinimumTotalEmployeeNeeded)
                {
                    designationGroup.IsRoasterable = false;
                }
                else
                {
                    designationGroup.IsRoasterable = true;
                }

                if (employeeListType == EmployeeChoiceFlagForRoaster.SelectedRoasterEmployee)
                {
                    int previousShiftGroupCount = 0;
                    int currentShiftGroupCount = 0;
                    int startingCount = 1;
                    foreach (var shift in designationGroup.vMRoasterGroupEmployeeList.GroupBy(c => c.StartingShift).ToList())
                    {
                        currentShiftGroupCount = shift.Count();

                        if (startingCount != 1)
                        {
                            if (currentShiftGroupCount == previousShiftGroupCount)
                            {
                                designationGroup.IsRoasterable = true;
                            }
                            else
                            {
                                designationGroup.IsRoasterable = false;
                                break;
                            }
                        }

                        previousShiftGroupCount = currentShiftGroupCount;

                        startingCount = startingCount + 1;
                    }

                    if (startingCount - 1 != shiftCount)
                    {
                        designationGroup.IsRoasterable = false;
                    }
                }


                designationGroup.vMRoasterGroupEmployeeList = designationGroup.vMRoasterGroupEmployeeList.OrderBy(c => c.StartingShiftName).ToList();
            }

            return vMRoasterGroupDesignationList;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: HR/Employees/Create
        public IActionResult CreateOrEdit(int id)
        {
            vmRoasterGroupCreateOrEdit vmRoasterGroupCreateOrEdit = new vmRoasterGroupCreateOrEdit();

            if (id != 0)
            {
                var roasterGroup = db.RoasterGroup.Include(c => c.RoasterGroupDetailsList).ThenInclude(x => x.Shift).FirstOrDefault(c => c.Id == id);

                vmRoasterGroupCreateOrEdit = new vmRoasterGroupCreateOrEdit()
                {
                    Id = roasterGroup.Id,
                    Name = roasterGroup.Name
                };

                vmRoasterGroupCreateOrEdit.ShiftList = db.Shift.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
                vmRoasterGroupCreateOrEdit.ShiftGroupDetailsList = roasterGroup.RoasterGroupDetailsList.Select(x => x.ShiftId).ToList();
            }

            vmRoasterGroupCreateOrEdit.ShiftList = db.Shift.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return PartialView("CreateOrEdit", vmRoasterGroupCreateOrEdit);
        }

        public IActionResult Details(int id)
        {
            vmRoasterGroupCreateOrEdit vmRoasterGroupCreateOrEdit = new vmRoasterGroupCreateOrEdit();

            if (id != 0)
            {
                var roasterGroup = db.RoasterGroup.Include(c => c.RoasterGroupDetailsList).ThenInclude(c => c.Shift).FirstOrDefault(c => c.Id == id);

                vmRoasterGroupCreateOrEdit = new vmRoasterGroupCreateOrEdit()
                {
                    Id = roasterGroup.Id,
                    Name = roasterGroup.Name,
                };

                vmRoasterGroupCreateOrEdit.RoasterGroupDetailsList = roasterGroup.RoasterGroupDetailsList;

                return PartialView("Details", vmRoasterGroupCreateOrEdit);
            }
            else
            {
                ViewBag.GuidelineMessage = "Access Denied!";
                ViewBag.ErrorMessage = "You are not authorize to update roasterGroup.";
                return PartialView("Error");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(vmRoasterGroupCreateOrEdit vmRoasterGroupCreateOrEdit)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (vmRoasterGroupCreateOrEdit.Id != 0)
                    {
                        var roasterGroup = db.RoasterGroup.Find(vmRoasterGroupCreateOrEdit.Id);

                        var roasterGroupDetails = db.RoasterGroupDetails.Where(x => x.RoasterGroupId == roasterGroup.Id);

                        db.RemoveRange(roasterGroupDetails);

                        roasterGroup.Name = vmRoasterGroupCreateOrEdit.Name;

                        roasterGroup.RoasterGroupDetailsList = vmRoasterGroupCreateOrEdit.ShiftGroupDetailsList.Select(x => new RoasterGroupDetails() { ShiftId = x }).ToList();

                        db.RoasterGroup.Attach(roasterGroup);

                        db.Entry(roasterGroup).State = EntityState.Modified;

                        bool isUpdated = await db.SaveChangesAsync() > 0;

                        if (isUpdated)
                        {
                            ModelState.Clear();
                            vmRoasterGroupCreateOrEdit model = new vmRoasterGroupCreateOrEdit();
                            ViewBag.ValidationMessage = true;
                            return PartialView("CreateOrEdit", model);
                        }
                    }
                    else
                    {
                        RoasterGroup roasterGroup = new RoasterGroup
                        {
                            Name = vmRoasterGroupCreateOrEdit.Name,
                        };

                        roasterGroup.RoasterGroupDetailsList = vmRoasterGroupCreateOrEdit.ShiftGroupDetailsList.Select(x => new RoasterGroupDetails() { ShiftId = x }).ToList();

                        db.Add(roasterGroup);
                        bool isSaved = await db.SaveChangesAsync() > 0;

                        if (isSaved)
                        {
                            ModelState.Clear();
                            vmRoasterGroupCreateOrEdit model = new vmRoasterGroupCreateOrEdit();
                            ViewBag.ValidationMessage = true;
                            return PartialView("CreateOrEdit", model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return PartialView("Error");
                }

            }

            ModelState.AddModelError("Validation Failed!", "Error");

            if (vmRoasterGroupCreateOrEdit.Id != 0)
            {
                var roasterGroup = db.RoasterGroup.Include(c => c.RoasterGroupDetailsList).ThenInclude(x => x.Shift).FirstOrDefault(c => c.Id == vmRoasterGroupCreateOrEdit.Id);
                vmRoasterGroupCreateOrEdit.ShiftList = db.Shift.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
                vmRoasterGroupCreateOrEdit.ShiftGroupDetailsList = roasterGroup.RoasterGroupDetailsList.Select(x => x.ShiftId).ToList();
            }
            else
            {
                vmRoasterGroupCreateOrEdit.ShiftList = db.Shift.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            }

            ViewBag.ValidationMessage = false;

            return PartialView("CreateOrEdit", vmRoasterGroupCreateOrEdit);
        }

        public IActionResult Grid()
        {
            try
            {
                var roasterGroupList = db.RoasterGroup.Include(c => c.RoasterGroupDetailsList).OrderByDescending(c => c.Id).ToList();

                List<vmRoasterGroupGrid> vmRoasterGroupGrid = roasterGroupList.Select(x => new vmRoasterGroupGrid()
                {
                    Id = x.Id,
                    Name = x.Name,
                    DeleteClaim = true,
                    ReadClaim = true,
                    UpdateClaim = true
                }).ToList();

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //Sorting    
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
                {
                    vmRoasterGroupGrid = vmRoasterGroupGrid.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }
                else
                {
                    vmRoasterGroupGrid = vmRoasterGroupGrid.OrderByDescending(x => x.Id).ToList();
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    vmRoasterGroupGrid = vmRoasterGroupGrid.Where(
                        x => x.Name.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                //total number of rows count     
                recordsTotal = vmRoasterGroupGrid.Count();

                //Paging     
                var data = vmRoasterGroupGrid.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return Json("Error");
            }
        }

        public async Task<IActionResult> Delete(int roasterGroupId)
        {
            try
            {
                var roasterGroup = db.RoasterGroup.Find(roasterGroupId);

                db.RoasterGroup.Remove(roasterGroup);

                bool isDeleted = await db.SaveChangesAsync() > 0;

                return Json(isDeleted);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public async Task<IActionResult> RoasterGroupSelectList()
        {
            try
            {
                var roasterGroup = db.RoasterGroup.ToList();

                List<SelectListItem> RoasterGroupList = roasterGroup.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

                return Json(RoasterGroupList);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public async Task<IActionResult> RoasterGroupEmployeeList(int roasterGroupId)
        {
            try
            {
                var roasterGroup = db.RoasterGroup
                    .Include(c => c.RoasterGroupEmployeeList).ThenInclude(c => c.Employee).ThenInclude(c => c.Designation)
                    .Include(c => c.RoasterGroupEmployeeList).ThenInclude(c => c.Employee).ThenInclude(c => c.Shift)
                    .Include(c => c.RoasterGroupDetailsList).ThenInclude(c => c.Shift).FirstOrDefault(c => c.Id == roasterGroupId);

                List<vMRoasterGroupDesignation> vMRoasterGroupDesignationList = GetRoasteringInfoDesignationWise(EmployeeChoiceFlagForRoaster.AllEmployeeInShift, roasterGroup);

                return Json(vMRoasterGroupDesignationList);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public async Task<IActionResult> RoasterGroupSelectedEmployeeList(int roasterGroupId)
        {
            try
            {
                var roasterGroup = db.RoasterGroup
                    .Include(c => c.RoasterGroupEmployeeList).ThenInclude(c => c.Employee).ThenInclude(c => c.Designation)
                    .Include(c => c.RoasterGroupEmployeeList).ThenInclude(c => c.Employee).ThenInclude(c => c.Shift)
                    .Include(c => c.RoasterGroupDetailsList).ThenInclude(c => c.Shift).FirstOrDefault(c => c.Id == roasterGroupId);

                List<vMRoasterGroupDesignation> vMRoasterGroupDesignationList = GetRoasteringInfoDesignationWise(EmployeeChoiceFlagForRoaster.SelectedRoasterEmployee, roasterGroup);

                return Json(vMRoasterGroupDesignationList);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> RoasterGroupShiftList(int roasterGroupId)
        {
            try
            {
                var roasterGroup = db.RoasterGroup.Include(c => c.RoasterGroupDetailsList).ThenInclude(c => c.Shift).FirstOrDefault(c => c.Id == roasterGroupId);

                List<SelectListItem> shiftList = roasterGroup.RoasterGroupDetailsList.Select(x => new SelectListItem()
                {
                    Text = x.Shift.Name,
                    Value = x.ShiftId.ToString()
                }).ToList();

                return Json(shiftList);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> AddEmployeeForRoastering(string employeeId, int roasterGroupId)
        {
            try
            {
                bool isAddedForRoastering = false;
                var employee = db.Employee.Include(c => c.Shift).FirstOrDefault(c => c.MaskingId == employeeId);


                if (db.RoasterGroupEmployee.Any(c => c.EmployeeId == employee.Id && c.RoasterGroupId == roasterGroupId))
                {
                    isAddedForRoastering = true;
                }
                else
                {
                    RoasterGroupEmployee roasterGroupEmployee = new RoasterGroupEmployee();

                    roasterGroupEmployee.EmployeeId = employee.Id;
                    roasterGroupEmployee.RoasterGroupId = roasterGroupId;
                    roasterGroupEmployee.StartingShiftId = employee.ShiftId;

                    db.Add(roasterGroupEmployee);
                    isAddedForRoastering = db.SaveChanges() > 0;
                }

                return Json(isAddedForRoastering);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> RemoveEmployeeForRoastering(string employeeId, int roasterGroupId)
        {
            try
            {
                bool isAddedForRoastering = false;
                var employee = db.Employee.FirstOrDefault(c => c.MaskingId == employeeId);

                if (db.RoasterGroupEmployee.Any(c => c.EmployeeId == employee.Id && c.RoasterGroupId == roasterGroupId))
                {
                    var roasterGroupEmployee = db.RoasterGroupEmployee.FirstOrDefault(c => c.EmployeeId == employee.Id && c.RoasterGroupId == roasterGroupId);

                    db.Remove(roasterGroupEmployee);
                    isAddedForRoastering = db.SaveChanges() > 0;
                }

                return Json(isAddedForRoastering);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> ChangeStartingShift(string employeeId, int roasterGroupId, int startingShiftId)
        {
            try
            {
                bool isStartingShiftUpdated = false;
                var employee = db.Employee.FirstOrDefault(c => c.MaskingId == employeeId);

                if (db.RoasterGroupEmployee.Any(c => c.EmployeeId == employee.Id && c.RoasterGroupId == roasterGroupId))
                {
                    var roasterGroupEmployee = db.RoasterGroupEmployee.FirstOrDefault(c => c.EmployeeId == employee.Id && c.RoasterGroupId == roasterGroupId);

                    roasterGroupEmployee.StartingShiftId = startingShiftId;

                    db.RoasterGroupEmployee.Attach(roasterGroupEmployee);
                    db.Entry(roasterGroupEmployee).State = EntityState.Modified;
                    isStartingShiftUpdated = db.SaveChanges() > 0;
                }

                return Json(isStartingShiftUpdated);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}