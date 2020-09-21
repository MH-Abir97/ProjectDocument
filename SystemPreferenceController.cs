using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.SystemPreference;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    public class SystemPreferenceController : BaseController
    {
        public SystemPreferenceController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public IActionResult Index()
        {
            ViewBag.Dept = new SelectList( db.Department.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false),"Id","Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(vmSystemPreference vmSystemPreference)
        {
            SystemPreference systemPref = db.SystemPreference.GetFirstOrDefault();
            if (systemPref == null)
            {
                SystemPreference systemPreference = new SystemPreference()
                {
                    AllowAttendancePunchInterval = vmSystemPreference.AllowAttendancePunchInterval,
                    LateCount = vmSystemPreference.LateCount,
                    GratuityFormula = vmSystemPreference.GratuityFormula,
                    EmployeeRequiredField = vmSystemPreference.EmployeeRequiredField,
                    ApprovalLevelForLeaveApplication = vmSystemPreference.LevelString,
                    //ApprovalLevelForLeaveApplicationDept = vmSystemPreference.DeptString,
                    OvertimeCalculation = vmSystemPreference.OvertimeCalculation,
                    ProvidendFund = vmSystemPreference.ProvidendFund,
                    Email = vmSystemPreference.EmailBool,
                    Sms = vmSystemPreference.Sms,

                    ApprovalLevelForBusinessApplication = vmSystemPreference.ApprovalLevelForBusinessApplication,
                    ApprovalLevelForEarlyOut = vmSystemPreference.ApprovalLevelForEarlyOut,
                    ApprovalLevelForEmployeeUpdate = vmSystemPreference.ApprovalLevelForEmployeeUpdate,
                    ApprovalLevelForFinalSettlement = vmSystemPreference.ApprovalLevelForFinalSettlement,
                    ApprovalLevelForLateIn = vmSystemPreference.ApprovalLevelForLateIn,
                    ApprovalLevelForLoan = vmSystemPreference.ApprovalLevelForLoan,
                    ApprovalLevelForManualAbsent = vmSystemPreference.ApprovalLevelForManualAbsent,
                    ApprovalLevelForManualPresent = vmSystemPreference.ApprovalLevelForManualPresent,
                    ApprovalLevelForPenalty = vmSystemPreference.ApprovalLevelForPenalty,
                    ApprovalLevelForPromotion = vmSystemPreference.ApprovalLevelForPromotion,
                    ApprovalLevelForResign = vmSystemPreference.ApprovalLevelForResign,
                    ApprovalLevelForRetirement = vmSystemPreference.ApprovalLevelForRetirement,
                    ApprovalLevelForRewards = vmSystemPreference.ApprovalLevelForRewards,
                    ApprovalLevelForRostering = vmSystemPreference.ApprovalLevelForRostering,
                    ApprovalLevelForSalaryHeldUp = vmSystemPreference.ApprovalLevelForSalaryHeldUp,
                    ApprovalLevelForSalarySheet = vmSystemPreference.ApprovalLevelForSalarySheet,
                    ApprovalLevelForShiftingChange = vmSystemPreference.ApprovalLevelForShiftingChange,
                    ApprovalLevelForTask = vmSystemPreference.ApprovalLevelForTask,
                    ApprovalLevelForUserBlockUnblock = vmSystemPreference.ApprovalLevelForUserBlockUnblock,
                    ApprovalLevelForUserPermission = vmSystemPreference.ApprovalLevelForUserPermission
                    
                };
                db.SystemPreference.Add(systemPreference);
            }
            else
            {
                systemPref.AllowAttendancePunchInterval = vmSystemPreference.AllowAttendancePunchInterval;
                systemPref.LateCount = vmSystemPreference.LateCount;
                systemPref.GratuityFormula = vmSystemPreference.GratuityFormula;
                systemPref.EmployeeRequiredField = vmSystemPreference.EmployeeRequiredField;
                systemPref.ApprovalLevelForLeaveApplication = vmSystemPreference.LevelString;
                systemPref.ApprovalLevelForLeaveApplicationDept = vmSystemPreference.DeptString;
                systemPref.OvertimeCalculation = vmSystemPreference.OvertimeCalculation;
                systemPref.ProvidendFund = vmSystemPreference.ProvidendFund;
                systemPref.Email = vmSystemPreference.EmailBool;
                systemPref.Sms = vmSystemPreference.Sms;

                systemPref.ApprovalLevelForBusinessApplication = vmSystemPreference.ApprovalLevelForBusinessApplication;
                systemPref.ApprovalLevelForEarlyOut = vmSystemPreference.ApprovalLevelForEarlyOut;
                systemPref.ApprovalLevelForEmployeeUpdate = vmSystemPreference.ApprovalLevelForEmployeeUpdate;
                systemPref.ApprovalLevelForFinalSettlement = vmSystemPreference.ApprovalLevelForFinalSettlement;
                systemPref.ApprovalLevelForLateIn = vmSystemPreference.ApprovalLevelForLateIn;
                systemPref.ApprovalLevelForLoan = vmSystemPreference.ApprovalLevelForLoan;
                systemPref.ApprovalLevelForManualAbsent = vmSystemPreference.ApprovalLevelForManualAbsent;
                systemPref.ApprovalLevelForManualPresent = vmSystemPreference.ApprovalLevelForManualPresent;
                systemPref.ApprovalLevelForPenalty = vmSystemPreference.ApprovalLevelForPenalty;
                systemPref.ApprovalLevelForPromotion = vmSystemPreference.ApprovalLevelForPromotion;
                systemPref.ApprovalLevelForResign = vmSystemPreference.ApprovalLevelForResign;
                systemPref.ApprovalLevelForRetirement = vmSystemPreference.ApprovalLevelForRetirement;
                systemPref.ApprovalLevelForRewards = vmSystemPreference.ApprovalLevelForRewards;
                systemPref.ApprovalLevelForRostering = vmSystemPreference.ApprovalLevelForRostering;
                systemPref.ApprovalLevelForSalaryHeldUp = vmSystemPreference.ApprovalLevelForSalaryHeldUp;
                systemPref.ApprovalLevelForSalarySheet = vmSystemPreference.ApprovalLevelForSalarySheet;
                systemPref.ApprovalLevelForShiftingChange = vmSystemPreference.ApprovalLevelForShiftingChange;
                systemPref.ApprovalLevelForTask = vmSystemPreference.ApprovalLevelForTask;
                systemPref.ApprovalLevelForUserBlockUnblock = vmSystemPreference.ApprovalLevelForUserBlockUnblock;
                systemPref.ApprovalLevelForUserPermission = vmSystemPreference.ApprovalLevelForUserPermission;
                db.SystemPreference.Update(systemPref);
            }
            db.Save();
            return View("Index");
        }
    }
}