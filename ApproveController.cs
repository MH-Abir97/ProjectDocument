using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.Core.Models.ApplicationApproval;
using Pronali.Web.Controllers;
using Pronali.Web.Extension;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
  //  [Authorize]
    public class ApproveController : BaseController
    {
        public ApproveController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }

        public async Task<JsonResult> ApplicationApprove(long id, string status,string comments)
        {
            var approverId = User.GetCurrentEmployeeId(db.Employee);
            var applicationStatus = Data.Enum.ApplicationStatus.Pending;
            if (status == "Approve")
            {
                applicationStatus = Data.Enum.ApplicationStatus.Approved;
            }
            else if (status == "Reject")
            {
                applicationStatus = Data.Enum.ApplicationStatus.Rejected;
            }
           

            //start function............................................................................

            var approvalData = db.ApplicationApproval.GetFirstOrDefault(x => x.Id == id);
            if (approvalData != null)
            {
                approvalData.ApproveTime = DateTime.Now.ToString();
                approvalData.Status = applicationStatus;
                approvalData.ApproverId = approverId;
                approvalData.Comments = comments;
                db.ApplicationApproval.Update(approvalData);
                db.Save();

                //available approver under this id
                var approverList = db.ApplicationApproval.GetAll().Where(x => x.Level > approvalData.Level && x.ApplicationId == approvalData.ApplicationId && x.ApplicationType == approvalData.ApplicationType);
                foreach (var apprv in approverList)
                {
                    apprv.Lock = true;
                    db.ApplicationApproval.Update(apprv);
                    db.Save();
                }



                //leave

                if (approvalData.ApplicationType == ApplicationType.Leave)
                {
                    var applicationData = db.LeaveApplication.GetFirstOrDefault(x => x.Id ==Convert.ToInt64(approvalData.ApplicationId));
                    if (applicationStatus == ApplicationStatus.Rejected && applicationData.Status == ApplicationStatus.Pending)
                    {
                        //back leave balance
                        var days = applicationData.ToDate.AddDays(1) - applicationData.FromDate;
                        var employeeLeave = db.EmployeeLeave.GetFirstOrDefault(x =>
                            x.EmployeeId == applicationData.EmployeeId && x.LeaveId == applicationData.LeaveId);
                        employeeLeave.Enjoyed -= days.Days;
                        db.EmployeeLeave.Update(employeeLeave);
                        db.Save();
                    }
                    if (applicationData != null)
                    {
                        applicationData.Status = applicationStatus;
                        applicationData.Comments = comments;
                        applicationData.ApproverId = approverId;
                        applicationData.ApprovedTime = DateTime.Now;
                        db.LeaveApplication.Update(applicationData);
                        db.Save();
                    }
                }
                //short leave
                if (approvalData.ApplicationType == ApplicationType.ShortLeave)
                {
                    var applicationData = db.ShortLeaveApplication.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                    if (applicationData != null)
                    {
                        applicationData.Status = applicationStatus;
                        applicationData.Comments = comments;
                        applicationData.ApproverId = approverId;
                        applicationData.ApprovedTime = DateTime.Now;
                        db.ShortLeaveApplication.Update(applicationData);
                        db.Save();
                    }
                }
                //business
                if (approvalData.ApplicationType == ApplicationType.BusinessTravel)
                {
                    var applicationData = db.BusinessApplication.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                    if (applicationData != null)
                    {
                        applicationData.Status = applicationStatus;
                        applicationData.Comments = comments;
                        applicationData.ApproverId = approverId;
                        applicationData.ApprovedTime = DateTime.Now;
                        db.BusinessApplication.Update(applicationData);
                        db.Save();
                    }
                }
                //short business
                if (approvalData.ApplicationType == ApplicationType.ShortBusinessTravel)
                {
                    var applicationData = db.ShortBusinessApplication.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                    if (applicationData != null)
                    {
                        applicationData.Status = applicationStatus;
                        applicationData.Comments = comments;
                        applicationData.ApproverId = approverId;
                        applicationData.ApprovedTime = DateTime.Now;
                        db.ShortBusinessApplication.Update(applicationData);
                        db.Save();
                    }
                }
                //early out
                if (approvalData.ApplicationType == ApplicationType.Early)
                {
                    var applicationData = db.EarlyOutPermission.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                    if (applicationData != null)
                    {
                        applicationData.Status = applicationStatus;
                        applicationData.Comments = comments;
                        applicationData.ApproverId = approverId;
                        applicationData.ApprovedTime = DateTime.Now;
                        db.EarlyOutPermission.Update(applicationData);
                        db.Save();
                    }
                }
                //late in
                if (approvalData.ApplicationType == ApplicationType.Late)
                {
                    var applicationData = db.LatePermission.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                    if (applicationData != null)
                    {
                        applicationData.Status = applicationStatus;
                        applicationData.Comments = comments;
                        applicationData.ApproverId = approverId;
                        applicationData.ApprovedTime = DateTime.Now;
                        db.LatePermission.Update(applicationData);
                        db.Save();
                    }
                }

            }

            return Json(true);
        }

        public async Task<JsonResult> ApplicationApproveAll(List<long> idList, string status, string comments)
        {
            var approverId = User.GetCurrentEmployeeId(db.Employee);
            var applicationStatus = Data.Enum.ApplicationStatus.Pending;
            if (status == "Approve")
            {
                applicationStatus = Data.Enum.ApplicationStatus.Approved;
            }
            else if (status == "Reject")
            {
                applicationStatus = Data.Enum.ApplicationStatus.Rejected;
            }

          
            foreach (var id in idList)
            {
                //start function............................................................................

                var approvalData = db.ApplicationApproval.GetFirstOrDefault(x => x.Id == id);
                if (approvalData != null)
                {
                    approvalData.ApproveTime = DateTime.Now.ToString();
                    approvalData.Status = applicationStatus;
                    approvalData.ApproverId = approverId;
                    approvalData.Comments = comments;
                    db.ApplicationApproval.Update(approvalData);
                    db.Save();

                    //available approver under this id
                    var approverList = db.ApplicationApproval.GetAll().Where(x => x.Level > approvalData.Level && x.ApplicationId == approvalData.ApplicationId && x.ApplicationType == approvalData.ApplicationType);
                    foreach (var apprv in approverList)
                    {
                        apprv.Lock = true;
                        db.ApplicationApproval.Update(apprv);
                        db.Save();
                    }



                    //leave

                    if (approvalData.ApplicationType == ApplicationType.Leave)
                    {
                        var applicationData = db.LeaveApplication.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                        if (applicationStatus == ApplicationStatus.Rejected && applicationData.Status == ApplicationStatus.Pending)
                        {
                            //back leave balance
                            var days = applicationData.ToDate.AddDays(1) - applicationData.FromDate;
                            var employeeLeave = db.EmployeeLeave.GetFirstOrDefault(x =>
                                x.EmployeeId == applicationData.EmployeeId && x.LeaveId == applicationData.LeaveId);
                            employeeLeave.Enjoyed -= days.Days;
                            db.EmployeeLeave.Update(employeeLeave);
                            db.Save();
                        }
                        if (applicationData != null)
                        {
                            applicationData.Status = applicationStatus;
                            applicationData.Comments = comments;
                            applicationData.ApproverId = approverId;
                            applicationData.ApprovedTime = DateTime.Now;
                            db.LeaveApplication.Update(applicationData);
                            db.Save();
                        }
                    }
                    //short leave
                    if (approvalData.ApplicationType == ApplicationType.ShortLeave)
                    {
                        var applicationData = db.ShortLeaveApplication.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                        if (applicationData != null)
                        {
                            applicationData.Status = applicationStatus;
                            applicationData.Comments = comments;
                            applicationData.ApproverId = approverId;
                            applicationData.ApprovedTime = DateTime.Now;
                            db.ShortLeaveApplication.Update(applicationData);
                            db.Save();
                        }
                    }
                    //business
                    if (approvalData.ApplicationType == ApplicationType.BusinessTravel)
                    {
                        var applicationData = db.BusinessApplication.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                        if (applicationData != null)
                        {
                            applicationData.Status = applicationStatus;
                            applicationData.Comments = comments;
                            applicationData.ApproverId = approverId;
                            applicationData.ApprovedTime = DateTime.Now;
                            db.BusinessApplication.Update(applicationData);
                            db.Save();
                        }
                    }
                    //short business
                    if (approvalData.ApplicationType == ApplicationType.ShortBusinessTravel)
                    {
                        var applicationData = db.ShortBusinessApplication.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                        if (applicationData != null)
                        {
                            applicationData.Status = applicationStatus;
                            applicationData.Comments = comments;
                            applicationData.ApproverId = approverId;
                            applicationData.ApprovedTime = DateTime.Now;
                            db.ShortBusinessApplication.Update(applicationData);
                            db.Save();
                        }
                    }
                    //early out
                    if (approvalData.ApplicationType == ApplicationType.Early)
                    {
                        var applicationData = db.EarlyOutPermission.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                        if (applicationData != null)
                        {
                            applicationData.Status = applicationStatus;
                            applicationData.Comments = comments;
                            applicationData.ApproverId = approverId;
                            applicationData.ApprovedTime = DateTime.Now;
                            db.EarlyOutPermission.Update(applicationData);
                            db.Save();
                        }
                    }
                    //late in
                    if (approvalData.ApplicationType == ApplicationType.Late)
                    {
                        var applicationData = db.LatePermission.GetFirstOrDefault(x => x.Id == Convert.ToInt64(approvalData.ApplicationId));
                        if (applicationData != null)
                        {
                            applicationData.Status = applicationStatus;
                            applicationData.Comments = comments;
                            applicationData.ApproverId = approverId;
                            applicationData.ApprovedTime = DateTime.Now;
                            db.LatePermission.Update(applicationData);
                            db.Save();
                        }
                    }
                }



            }
            return Json(true);
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult LoadApplicationApproval()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            // List<ApplicationApproval> approvelList = db.ApplicationApproval.GetAll().Where(Model => Model.IsActive == true && Model.IsDeleted == false).ToList();

            List<ApplicationApproval> approvelList = db.ApplicationApproval.GetWithAllEmployeeData().Where(m=>m.IsActive==true && m.IsDeleted==false).ToList();

            List<VmApplicationApproval> ApprovalItem = new List<VmApplicationApproval>();
            foreach (var item in approvelList)
            {
                ApprovalItem.Add(new VmApplicationApproval
                {
                    Id = item.Id,
                    ApplicationId = item.ApplicationId,
                    ApplicationType = item.ApplicationType,
                    ApproveTime = item.ApproveTime,
                    ChangeApproveTime=item.ApproveTime,
                    ApproverId = item.ApproverId,
                    ApproverName = item.Approver.FullName,
                    Comments = item.Comments,
                   Status=item.Status,
                });
            }

            

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {

            }
            else
            {
                ApprovalItem = ApprovalItem.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                ApprovalItem = ApprovalItem.Where(model => model.Comments.Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = ApprovalItem.Count();

            //Paging     
            var data = ApprovalItem.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


    


        public JsonResult loadPrevApproveComments(long id)
        {

            var approverData = db.ApplicationApproval.GetFirstOrDefault(x => x.Id==id);
            var data = db.ApplicationApproval.GetWithAllEmployeeData().Where(x => x.ApplicationType ==approverData.ApplicationType && x.Comments!=null && x.ApplicationId == approverData.ApplicationId && x.Level > approverData.Level);

            return Json(data);
        }
    }
}