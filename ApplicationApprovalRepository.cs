using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Core;

namespace Pronali.Data.Repositories.Core
{
    public class ApplicationApprovalRepository : BaseRepository<ApplicationApproval>, IApplicationApprovalRepository
    {
        private readonly ApplicationDbContext db;
        public ApplicationApprovalRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public List<ApplicationApproval> GetWithAllEmployeeData()
        {
            return db.ApplicationApproval.Include(x => x.Approver).ToList();
        }

        public void SetApprover(ApplicationType applicationType, long employeeId,string id,DateTime time, string createdby)
        {
            var config = db.SystemPreference.FirstOrDefault();
            var employee = db.Employee.FirstOrDefault(x => x.Id == employeeId);
            var approverId = new List<long>();
            var icon = "";
            var title = "";
            var url = "";
            string[] level = new string[] { };
            switch (applicationType)
            {
                case ApplicationType.Leave:
                    icon = "<div class=\"notify-icon bg-blue\"><i class=\"mdi mdi mdi-bullhorn\"></i></div>";
                    url = "/hr/leave/manage";
                    title = "Leave Application";
                    level = config.ApprovalLevelForLeaveApplication.Split(',');
                    break;
                case ApplicationType.ShortLeave:
                    icon = "<div class=\"notify-icon bg-blue\"><i class=\"mdi mdi mdi-bullhorn\"></i></div>";
                    url = "/hr/leave/manage";
                    title = "Short Leave Application";
                    level = config.ApprovalLevelForLeaveApplication.Split(',');
                    break;
                case ApplicationType.BusinessTravel:
                    icon = "<div class=\"notify-icon bg-soft-dark\"><i class=\"mdi mdi mdi-bullhorn\"></i></div>";
                    url = "/hr/businesstravel/manage";
                    title = "Business Travel Application";
                    level = config.ApprovalLevelForBusinessApplication.Split(',');
                    break;
                case ApplicationType.ShortBusinessTravel:
                    icon = "<div class=\"notify-icon bg-soft-dark\"><i class=\"mdi mdi mdi-bullhorn\"></i></div>";
                    url = "/hr/businesstravel/manage";
                    title = "Short Business Travel Application";
                    level = config.ApprovalLevelForBusinessApplication.Split(',');
                    break;
                case ApplicationType.Early:
                    icon = "<div class=\"notify-icon bg-dark\"><i class=\"mdi mdi mdi-bullhorn\"></i></div>";
                    url = "/hr/EarlyOutPermission/manage";
                    title = "Early Out Permission";
                    level = config.ApprovalLevelForEarlyOut.Split(',');
                    break;
                case ApplicationType.Late:
                    icon = "<div class=\"notify-icon bg-warning\"><i class=\"mdi mdi mdi-bullhorn\"></i></div>";
                    url = "/hr/LatePermission/manage";
                    title = "Late In Permission";
                    level = config.ApprovalLevelForLateIn.Split(',');
                    break;
            }


            if (config.ApprovalSystem)
            {
                foreach (var item in level)
                {
                    switch (item)
                    {
                        case "Level-1":
                            var companyHead = db.CompanyHead.Where(x => x.CompanyId == employee.CompanyId).ToList();
                            foreach (var sub in companyHead)
                            {
                                approverId.Add(sub.EmployeeId);
                                var approval = new ApplicationApproval
                                {
                                    ApplicationType = applicationType,
                                    ApplicationId = id,
                                    ApproverId = sub.EmployeeId,
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    Status = ApplicationStatus.Pending,
                                    Level = 1
                                };
                                db.ApplicationApproval.Add(approval);
                                db.SaveChanges();

                                //send notification to user
                                var notificatoin = new Notification
                                {
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    IsRead = false,
                                    Details = "Approval request from (" + employee.MaskingId + ")  " + employee.FirstName + " at " + time.ToString(),
                                    ReceivedFrom = employee.UserId,
                                    SendTo = sub.Employee.UserId,
                                    Title = title,
                                    Url =url,
                                    Avatar =icon
                                };
                                db.Notification.Add(notificatoin);
                                db.SaveChanges();

                            }
                            break;
                        case "Level-2":
                            var sisterHead = db.SisterConcernHead.Where(x => x.SisterConcernId == employee.SisterConcernId).ToList();
                            foreach (var sub in sisterHead)
                            {
                                if (approverId.Contains(sub.EmployeeId))
                                {
                                    continue;
                                }
                                approverId.Add(sub.EmployeeId);
                                var approval = new ApplicationApproval
                                {
                                    ApplicationType = applicationType,
                                    ApplicationId = id,
                                    ApproverId = sub.EmployeeId,
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    Status = ApplicationStatus.Pending,
                                    Level = 2
                                };
                                db.ApplicationApproval.Add(approval);
                                db.SaveChanges();

                                //send notification to user
                                var notificatoin = new Notification
                                {
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    IsRead = false,
                                    Details = "Approval request from (" + employee.MaskingId + ")  " + employee.FirstName + " at " + time.ToString(),
                                    ReceivedFrom = employee.UserId,
                                    SendTo = sub.Employee.UserId,
                                    Title = title,
                                    Url = url,
                                    Avatar = icon
                                };
                                db.Notification.Add(notificatoin);
                                db.SaveChanges();
                            }
                            break;
                        case "Level-3":
                            var divisionHead = db.DivisionalHead.Where(x => x.DivisionId == employee.DivisionId).ToList();
                            foreach (var sub in divisionHead)
                            {
                                if (approverId.Contains(sub.EmployeeId))
                                {
                                    continue;
                                }
                                approverId.Add(sub.EmployeeId);
                                var approval = new ApplicationApproval
                                {
                                    ApplicationType = applicationType,
                                    ApplicationId = id,
                                    ApproverId = sub.EmployeeId,
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    Status = ApplicationStatus.Pending,
                                    Level = 3
                                };
                                db.ApplicationApproval.Add(approval);
                                db.SaveChanges();

                                //send notification to user
                                var notificatoin = new Notification
                                {
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    IsRead = false,
                                    Details = "Approval request from (" + employee.MaskingId + ")  " + employee.FirstName + " at " + time.ToString(),
                                    ReceivedFrom = employee.UserId,
                                    SendTo = sub.Employee.UserId,
                                    Title = title,
                                    Url = url,
                                    Avatar = icon
                                };
                                db.Notification.Add(notificatoin);
                                db.SaveChanges();

                            }
                            break;
                        case "Level-4":
                            var branchHead = db.BranchHead.Where(x => x.BranchId == employee.BranchId).ToList();
                            foreach (var sub in branchHead)
                            {
                                if (approverId.Contains(sub.EmployeeId))
                                {
                                    continue;
                                }
                                approverId.Add(sub.EmployeeId);
                                var approval = new ApplicationApproval
                                {
                                    ApplicationType = applicationType,
                                    ApplicationId = id,
                                    ApproverId = sub.EmployeeId,
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    Status = ApplicationStatus.Pending,
                                    Level = 4
                                };
                                db.ApplicationApproval.Add(approval);
                                db.SaveChanges();


                                //send notification to user
                                var notificatoin = new Notification
                                {
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    IsRead = false,
                                    Details = "Approval request from (" + employee.MaskingId + ")  " + employee.FirstName + " at " + time.ToString(),
                                    ReceivedFrom = employee.UserId,
                                    SendTo = sub.Employee.UserId,
                                    Title = title,
                                    Url = url,
                                    Avatar = icon
                                };
                                db.Notification.Add(notificatoin);
                                db.SaveChanges();
                            }
                            break;
                        case "Level-5":
                            var departmentHead = db.DepartmentalHead.Where(x => x.DepartmentId == employee.DepartmentId).ToList();
                            foreach (var sub in departmentHead)
                            {
                                if (sub.EmployeeId == employeeId)
                                {
                                    continue;
                                }
                                if (approverId.Contains(sub.EmployeeId))
                                {
                                    continue;
                                }
                                approverId.Add(sub.EmployeeId);
                                var approval = new ApplicationApproval
                                {
                                    ApplicationType = applicationType,
                                    ApplicationId = id,
                                    ApproverId = sub.EmployeeId,
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    Status = ApplicationStatus.Pending,
                                    Level = 5
                                };
                                db.ApplicationApproval.Add(approval);
                                db.SaveChanges();


                                //send notification to user
                                var notificatoin = new Notification
                                {
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    IsRead = false,
                                    Details = "Approval request from (" + employee.MaskingId + ")  " + employee.FirstName + " at " + time.ToString(),
                                    ReceivedFrom = employee.UserId,
                                    SendTo = sub.Employee.UserId,
                                    Title = title,
                                    Url = url,
                                    Avatar = icon
                                };
                                db.Notification.Add(notificatoin);
                                db.SaveChanges();
                            }
                            break;
                        case "Level-6":
                            if (employee.SuperirorId != null)
                            {
                                if (approverId.Contains(employee.SuperirorId.Value))
                                {
                                    continue;
                                }

                                var approval = new ApplicationApproval
                                {
                                    ApplicationType = applicationType,
                                    ApplicationId = id,
                                    ApproverId = employee.SuperirorId.Value,
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    Status = ApplicationStatus.Pending,
                                    Level = 6
                                };
                                db.ApplicationApproval.Add(approval);
                                db.SaveChanges();


                                //send notification to user
                                var notificatoin = new Notification
                                {
                                    CreatedBy = createdby,
                                    CreatedDate = time,
                                    IsActive = true,
                                    IsDeleted = false,
                                    IsRead = false,
                                    Details = "Approval request from (" + employee.MaskingId + ")  " + employee.FirstName + " at " + time.ToString(),
                                    ReceivedFrom = employee.UserId,
                                    SendTo = employee.Superiror.UserId,
                                    Title = title,
                                    Url = url,
                                    Avatar = icon
                                };
                                db.Notification.Add(notificatoin);
                                db.SaveChanges();
                            }
                            break;
                    }
                }
            }
            else
            {
                // if false then goes only superior

                if (employee.SuperirorId != null)
                {
                    var approval = new ApplicationApproval
                    {
                        ApplicationType = applicationType,
                        ApplicationId = id,
                        ApproverId = employee.SuperirorId.Value,
                        CreatedBy = createdby,
                        CreatedDate = time,
                        IsActive = true,
                        IsDeleted = false,
                        Status = ApplicationStatus.Pending,
                        Level = 6
                    };
                    db.ApplicationApproval.Add(approval);
                    db.SaveChanges();

                    //send notification to user
                    var notificatoin = new Notification
                    {
                        CreatedBy = createdby,
                        CreatedDate = time,
                        IsActive = true,
                        IsDeleted = false,
                        IsRead = false,
                        Details = "Approval request from (" + employee.MaskingId + ")  " + employee.FirstName + " at " + time.ToString(),
                        ReceivedFrom = employee.UserId,
                        SendTo = employee.Superiror.UserId,
                        Title = title,
                        Url = url,
                        Avatar = icon
                    };
                    db.Notification.Add(notificatoin);
                    db.SaveChanges();
                }
            }

        }

    }
}
