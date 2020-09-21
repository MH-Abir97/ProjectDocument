using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Roaster;
using Pronali.Web.Areas.HR.Models.RoasterGroup;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class RoasterController:Controller
    {
        private readonly ApplicationDbContext db;
        public RoasterController(ApplicationDbContext _dbContext)
        {
            db = _dbContext;
        }

        public TimeSpan TwelveHourFormatStringToTimeSpan(string TwelveHourFormatString)
        {
            return DateTime.ParseExact(TwelveHourFormatString, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
        }

        public void GenerateWeeklyRoastering( int roasterGroupId, vMRoasterGroupDesignation vMRoasterGroupDesignation, DateTime startDate, DateTime endDate)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //int NumberOfEmployee = vMRoasterGroupDesignation. numberOfEmployee;
            //string ShiftGroupName = shiftGroupName;

            RoasterGroup roasterGroup = db.RoasterGroup.Include(c => c.RoasterGroupDetailsList).ThenInclude(c => c.Shift).ThenInclude(c => c.ShiftDetailsList).AsNoTracking().FirstOrDefault(c => c.Id == roasterGroupId);
            List<Shift> shiftList = roasterGroup.RoasterGroupDetailsList.Select(x => x.Shift).ToList();
            List<ShiftVm> shiftVmList = shiftList.Select(c => new ShiftVm() { Id = c.Id, Name = c.Name, StartTime = TwelveHourFormatStringToTimeSpan(c.ShiftDetailsList.FirstOrDefault().OfficeStartTime) }).ToList();
            shiftVmList = shiftVmList.OrderBy(c => c.StartTime).ToList();

            int NumberOfEmployeeNeededPerShiftWithExtra = Convert.ToInt32(Math.Ceiling(vMRoasterGroupDesignation.AvailableEmployee / Convert.ToDouble(shiftList.Count())));
            int NumberOfEmployeeNeededPerShift = Convert.ToInt32(Math.Floor(Convert.ToDouble(NumberOfEmployeeNeededPerShiftWithExtra) * (100.00 / 117.00)));
            int NumberOfExtraEmployeeNeededPerShift = NumberOfEmployeeNeededPerShiftWithExtra - NumberOfEmployeeNeededPerShift;
            int shiftGap = 1;

            for (var day = startDate.Date; day.Date <= endDate.Date; day = day.AddDays(1))
            {
                var previousDay = day.AddDays(-1);
                var dayBeforePreviousDay = day.AddDays(-2);

                if (!db.Roaster.Any(c => c.Date.Date == previousDay.Date & (shiftVmList.Any(x => x.Id == c.ShiftId))))
                {
                    List<vMRoasterGroupEmployee> employeeList = vMRoasterGroupDesignation.vMRoasterGroupEmployeeList;


                    foreach (ShiftVm shift in shiftVmList)
                    {
                        ShiftVm shiftToRegister = shift;

                        List<ShiftVm> shiftVmListForRotation = shiftVmList;
                        while (shift != shiftVmListForRotation.FirstOrDefault())
                        {
                            shiftVmListForRotation = ShiftListHelper.ShiftRight<ShiftVm>(shiftVmListForRotation, shiftGap);
                        }

                        ShiftVm nextShift = ShiftListHelper.ShiftRight<ShiftVm>(shiftVmListForRotation, shiftGap).FirstOrDefault();

                        while (shift != shiftVmListForRotation.FirstOrDefault())
                        {
                            shiftVmListForRotation = ShiftListHelper.ShiftRight<ShiftVm>(shiftVmListForRotation, shiftGap);
                        }

                        ShiftVm previousShift = ShiftListHelper.ShiftLeft<ShiftVm>(shiftVmListForRotation, shiftGap).FirstOrDefault();

                        //shift registering part
                        List<vMRoasterGroupEmployee> NumberOfEmployeeNeededPerShiftEmployeeList = employeeList.Where(c => c.StartingShift == shiftToRegister.Id).Take(NumberOfEmployeeNeededPerShift).ToList();

                        foreach (var employee in NumberOfEmployeeNeededPerShiftEmployeeList)
                        {
                            db.Database.ExecuteSqlCommand("insert into Hr.Roaster(EmployeeId, Date, ShiftId, PreviousShiftId, CreatedDate, IsActive, IsDeleted) values(" + employee.Id + ",'" + day.Year + "-" + day.Month + "-" + day.Day + "'," + shiftToRegister.Id + ", " + previousShift.Id + ", '" + DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day + "', "+1+ ", " + 0 + ")");
                            employeeList.Remove(employee);
                        }

                        //weekend giving part
                        List<vMRoasterGroupEmployee> NumberOfExtraEmployeeNeededPerShiftEmployeeList = employeeList.Where(c => c.StartingShift == shiftToRegister.Id).Take(NumberOfExtraEmployeeNeededPerShift).ToList();

                        foreach (var employee in NumberOfExtraEmployeeNeededPerShiftEmployeeList)
                        {
                            db.Database.ExecuteSqlCommand("insert into Hr.Roaster(EmployeeId, Date, ShiftId, PreviousShiftId, CreatedDate, IsActive, IsDeleted) values(" + employee.Id + ",'" + day.Year + "-" + day.Month + "-" + day.Day + "',NULL, " + shift.Id + ", '" + DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day + "'," + 1 + ", " + 0 + ")");
                            employeeList.Remove(employee);
                        }

                    }
                }
                else
                {
                    foreach (ShiftVm shift in shiftVmList)
                    {
                        ShiftVm shiftToRegister = shift;

                        List<ShiftVm> shiftVmListForRotation = shiftVmList;
                        while (shift != shiftVmListForRotation.FirstOrDefault())
                        {
                            shiftVmListForRotation = ShiftListHelper.ShiftRight<ShiftVm>(shiftVmListForRotation, shiftGap);
                        }

                        ShiftVm nextShift = ShiftListHelper.ShiftRight<ShiftVm>(shiftVmListForRotation, shiftGap).FirstOrDefault();

                        while (shift != shiftVmListForRotation.FirstOrDefault())
                        {
                            shiftVmListForRotation = ShiftListHelper.ShiftRight<ShiftVm>(shiftVmListForRotation, shiftGap);
                        }

                        ShiftVm previousShift = ShiftListHelper.ShiftLeft<ShiftVm>(shiftVmListForRotation, shiftGap).FirstOrDefault();

                        List<Roaster> employeeShiftListOfShiftToRegister = db.Roaster.AsNoTracking().Where(c => c.Date.Date == previousDay.Date && c.ShiftId == shiftToRegister.Id && c.PreviousShiftId == previousShift.Id).ToList();
                        List<Roaster> employeeShiftListOfShiftToRegisterToAddOrRemove = employeeShiftListOfShiftToRegister.ToList();

                        var beforeSevenDaysDay = day.AddDays(-7);
                        int numberOfEmployeeGivenWeekend = 0;
                        int numberOfEmployeeAssignToWorkInTheShift = 0;


                        //to give weekend
                        foreach (var employeeShift in employeeShiftListOfShiftToRegister)
                        {
                            List<Roaster> employeeShiftListForLastSevenDayOfTheEmployee =
                                db.Roaster.AsNoTracking().Where(c => c.EmployeeId == employeeShift.EmployeeId && c.PreviousShiftId == previousShift.Id && c.Date.Date <= previousDay && c.Date.Date >= beforeSevenDaysDay.Date).ToList();

                            if (employeeShiftListForLastSevenDayOfTheEmployee.Count == 6)
                            {
                                //if shiftId is null in last 7 days then not eligible for weekend
                                if (employeeShiftListForLastSevenDayOfTheEmployee.Any(c => c.ShiftId == null))
                                {
                                    break;
                                }
                                else
                                {
                                    db.Database.ExecuteSqlCommand("insert into Hr.Roaster(EmployeeId, Date, ShiftId, PreviousShiftId, CreatedDate , IsActive, IsDeleted) values(" + employeeShift.EmployeeId + ",'" + day.Year + "-" + day.Month + "-" + day.Day + "',NULL, " + shiftToRegister.Id + ",'" + DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day + "', " + 1 + ", " + 0 + ")");
                                    numberOfEmployeeGivenWeekend = numberOfEmployeeGivenWeekend + 1;
                                    employeeShiftListOfShiftToRegisterToAddOrRemove.Remove(employeeShift);
                                }
                            }
                        }

                        employeeShiftListOfShiftToRegister = employeeShiftListOfShiftToRegisterToAddOrRemove.ToList();

                        int employeeNeededToGiveWeekendRandomlyFromTheShift = NumberOfExtraEmployeeNeededPerShift - numberOfEmployeeGivenWeekend;
                        int employeeGivenWeekendRandomly = 0;
                        //if all NumberOfExtraEmployeeNeededPerShift not given weekend, give weekend randomly
                        if (numberOfEmployeeGivenWeekend < NumberOfExtraEmployeeNeededPerShift)
                        {
                            List<Roaster> employeeShiftListWhoWillBeGivenWeekendRandomly = employeeShiftListOfShiftToRegister.Take(employeeNeededToGiveWeekendRandomlyFromTheShift).ToList();

                            foreach (var employeeShift in employeeShiftListWhoWillBeGivenWeekendRandomly)
                            {
                                if (employeeGivenWeekendRandomly < employeeNeededToGiveWeekendRandomlyFromTheShift)
                                {
                                    db.Database.ExecuteSqlCommand("insert into Hr.Roaster(EmployeeId, Date, ShiftId, PreviousShiftId, CreatedDate, IsActive, IsDeleted) values(" + employeeShift.EmployeeId + ",'" + day.Year + "-" + day.Month + "-" + day.Day + "',NULL, " + shiftToRegister.Id + ",'" + DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day + "', " + 1 + ", " + 0 + ")");

                                    numberOfEmployeeGivenWeekend = numberOfEmployeeGivenWeekend + 1;
                                    employeeGivenWeekendRandomly = employeeGivenWeekendRandomly + 1;
                                    employeeShiftListOfShiftToRegisterToAddOrRemove.Remove(employeeShift);
                                }
                            }
                        }

                        employeeShiftListOfShiftToRegister = employeeShiftListOfShiftToRegisterToAddOrRemove.ToList();

                        //to assign same shift for rest of the employee after given weekend to others
                        foreach (var employeeShift in employeeShiftListOfShiftToRegister)
                        {
                            db.Database.ExecuteSqlCommand("insert into Hr.Roaster(EmployeeId, Date, ShiftId, PreviousShiftId, CreatedDate, IsActive, IsDeleted) values(" + employeeShift.EmployeeId + ",'" + day.Year + "-" + day.Month + "-" + day.Day + "'," + shiftToRegister.Id + ", " + previousShift.Id + ", '" + DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day + "', " + 1 + ", " + 0 + ")");

                            numberOfEmployeeAssignToWorkInTheShift = numberOfEmployeeAssignToWorkInTheShift + 1;

                            employeeShiftListOfShiftToRegisterToAddOrRemove.Remove(employeeShift);
                        }

                        employeeShiftListOfShiftToRegister = employeeShiftListOfShiftToRegisterToAddOrRemove.ToList();

                        List<Roaster> employeeShiftListOfPreviousShiftWhoWereInWeekend = db.Roaster.AsNoTracking().Where(c => c.Date.Date == previousDay.Date && c.ShiftId == null && c.PreviousShiftId == previousShift.Id).ToList();
                        List<Roaster> employeeShiftListOfPreviousShiftWhoWereInWeekendToAddOrRemove = employeeShiftListOfPreviousShiftWhoWereInWeekend.ToList();

                        //to assign roasterd shift for employees who were in previous shift and in weekend
                        foreach (var employeeShift in employeeShiftListOfPreviousShiftWhoWereInWeekend)
                        {
                            db.Database.ExecuteSqlCommand("insert into Hr.Roaster(EmployeeId, Date, ShiftId, PreviousShiftId, CreatedDate, IsActive, IsDeleted) values(" + employeeShift.EmployeeId + ",'" + day.Year + "-" + day.Month + "-" + day.Day + "'," + shiftToRegister.Id + ", " + previousShift.Id + ", '" + DateTime.UtcNow.Year + "-" + DateTime.UtcNow.Month + "-" + DateTime.UtcNow.Day + "', " + 1 + ", " + 0 + ")");
                            numberOfEmployeeAssignToWorkInTheShift = numberOfEmployeeAssignToWorkInTheShift + 1;
                            employeeShiftListOfPreviousShiftWhoWereInWeekendToAddOrRemove.Remove(employeeShift);
                        }

                        employeeShiftListOfPreviousShiftWhoWereInWeekend = employeeShiftListOfPreviousShiftWhoWereInWeekendToAddOrRemove.ToList();

                    }
                }
            }
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
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

                employeeToAdd.Id = employee.Id;
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

        public IActionResult CreateOrEdit()
        {
            return PartialView("CreateOrEdit");
        }

        public IActionResult ProcessRoaster(string dateRange, int roasterGroupId)
        {
            try
            {
                List<string> dateRangeList = dateRange.Split(" - ").ToList();

                var start = DateTime.Parse(dateRangeList[0]);
                var end = DateTime.Parse(dateRangeList[1]);

                var roasterGroup = db.RoasterGroup
                .Include(c => c.RoasterGroupEmployeeList).ThenInclude(c => c.Employee).ThenInclude(c => c.Designation)
                .Include(c => c.RoasterGroupEmployeeList).ThenInclude(c => c.Employee).ThenInclude(c => c.Shift)
                .Include(c => c.RoasterGroupDetailsList).ThenInclude(c => c.Shift).FirstOrDefault(c => c.Id == roasterGroupId);

                List<vMRoasterGroupDesignation> vMRoasterGroupDesignationList = GetRoasteringInfoDesignationWise(EmployeeChoiceFlagForRoaster.SelectedRoasterEmployee, roasterGroup).Where(c => c.IsRoasterable == true).ToList();

                foreach(var roasterGroupDesignation in vMRoasterGroupDesignationList)
                {
                    GenerateWeeklyRoastering(roasterGroupId, roasterGroupDesignation, start, end);
                }


                //ap.ProcessRoaster(start, end, roasterGroupId);

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
    }
}