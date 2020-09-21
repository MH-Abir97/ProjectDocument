using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Areas.POS.Data;
using Pronali.Web.Areas.POS.Models;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class VoucherController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly RawSQL _sql;
        private readonly IMapper _mapper;

        public VoucherController(IUnitOfWork work, IConfiguration configuration, IMapper mapper) : base(work)
        {
            _work = work;
            _sql = new RawSQL(configuration);
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetVoucherData(string voucherNumber)
        {
            var voucher = _work.VoucherHead.GetAllWithDetails(voucherNumber);

            VoucherLoadViewModel voucherLoadViewModel = new VoucherLoadViewModel
            {
                Id = voucher.Id,
                VoucherType = voucher.VoucherType,
                VoucherNumber = voucher.VoucherNumber,
                EntryDate = voucher.EntryDate,
                PaidTo = voucher.PaidTo,
                VoucherId = voucher.VoucherId,
                VoucherSaveState = voucher.VoucherSaveState
            };

            foreach (var item in voucher.VoucherDetails)
            {
                int i = 0;

                VoucherDetailLoadViewModel voucherDetailLoadViewModel = new VoucherDetailLoadViewModel
                {
                    SerialNo = ++i,
                    Particulars = item.Particulars,
                    VoucherItemType = item.TransactionTypes == TransactionTypes.Debit ? "Dr" : "Cr",
                    LedgerId = item.AccountSubLedgerId,
                    Ref = item.Ref == null ? "" : item.Ref,
                    DebitAmount = item.DebitAmount,
                    CreditAmount = item.CreditAmount,
                    Disabled = false
                };

                i = i + 1;

                voucherLoadViewModel.VoucherDetailLoadViewModels.Add(voucherDetailLoadViewModel);
            }

            return Json(voucherLoadViewModel);
        }

        #region Voucher CRUD

        public IActionResult CreateView()
        {
            return PartialView("_VoucherCreateView");
        }
        public IActionResult Create(Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                _work.Voucher.Add(voucher);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }

            return Json(false);
        }

        public IActionResult EditView(int voucherId)
        {
            var voucher = _work.Voucher.Get(voucherId);
            return PartialView("_VoucherEditView", voucher);
        }

        public IActionResult Edit(Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                var editedVoucher = _work.TempVoucher.Get(voucher.Id);

                editedVoucher.VoucherName = voucher.VoucherName;
                editedVoucher.VoucherType = voucher.VoucherType;

                _work.TempVoucher.Update(editedVoucher);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return PartialView("_VoucherEditView", voucher);
        }

        public IActionResult Delete(int voucherId)
        {
            var voucher = _work.Voucher.Get(voucherId);

            _work.Voucher.Remove(voucher);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult VoucherList()
        {
            return PartialView("_VoucherList");
        }

        public IActionResult LoadVouchers()
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

            var vouchers = _work.Voucher.GetAll();

            var voucherList = new List<TempVoucherVM>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                vouchers = vouchers.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                vouchers = vouchers.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                vouchers = vouchers.Where(x => x.VoucherName.Contains(searchValue) || x.VoucherType.ToString().Contains(searchValue)).ToList();
            }

            foreach (var item in vouchers)
            {
                voucherList.Add(new TempVoucherVM
                {
                    Id = item.Id,
                    VoucherName = item.VoucherName,
                    VoucherType = item.VoucherType.ToString(),
                });
            }

            //total number of rows count     
            recordsTotal = voucherList.Count();

            //Paging     
            var data = voucherList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        #endregion]

        #region Debit Voucher

        [HttpGet]
        public IActionResult DebitVoucher()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PartialDebitVoucher()
        {
            Voucher voucher = _work.Voucher.FindByVoucherType(VoucherTypes.DebitVoucher);

            VoucherNumber lastVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseLastOrDefault(voucher.Id);

            if (lastVoucherNumber == null)
            {
                lastVoucherNumber = new VoucherNumber();
            }

            VoucherNumber unsavedVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseUnsavedVoucher(voucher.Id);

            TempVoucherHead tempVoucherHead = new TempVoucherHead();

            if (unsavedVoucherNumber == null)
            {
                tempVoucherHead.SetVoucherNumber(VoucherTypes.DebitVoucher, lastVoucherNumber.Number);

                tempVoucherHead.VoucherId = voucher.Id;

                tempVoucherHead.VoucherType = voucher.VoucherType;

                tempVoucherHead.VoucherSaveState = VoucherSaveStates.Unsaved;

                _work.TempVoucherHead.Add(tempVoucherHead);

                // === voucher number === //

                VoucherNumber voucherNumber = new VoucherNumber
                {
                    Number = tempVoucherHead.VoucherNumber,
                    State = tempVoucherHead.VoucherSaveState,
                    VoucherId = voucher.Id
                };

                _work.VoucherNumber.Add(voucherNumber);

                // === === === //

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return PartialView("_DebitVoucher", tempVoucherHead);
                }
            }
            else
            {
                TempVoucherHead unsavedVoucher = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(unsavedVoucherNumber.Number);

                return PartialView("_DebitVoucher", unsavedVoucher);
            }

            return PartialView("_DebitVoucher", tempVoucherHead);
        }

        [HttpPost]
        public IActionResult DebitVoucher(DebitVoucherViewModel debitVoucherViewModel)
        {
            if (!string.IsNullOrEmpty(debitVoucherViewModel.VoucherNumber))
            {
                var tempVoucherHead = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(debitVoucherViewModel.VoucherNumber);

                if (tempVoucherHead != null)
                {
                    tempVoucherHead.VoucherNumber = debitVoucherViewModel.VoucherNumber;
                    tempVoucherHead.EntryDate = Convert.ToDateTime(debitVoucherViewModel.Date);
                    tempVoucherHead.PaidTo = debitVoucherViewModel.PaidTo;
                    tempVoucherHead.PreparedBy = debitVoucherViewModel.PreparedBy;
                    tempVoucherHead.PreparedByUserId = debitVoucherViewModel.PreparedByUserId;
                    tempVoucherHead.ReceivedBy = debitVoucherViewModel.ReceivedBy;
                    tempVoucherHead.VoucherSaveState = VoucherSaveStates.Running;

                    _work.TempVoucherHead.Update(tempVoucherHead);

                    // === voucher number === //

                    VoucherNumber voucherNumber = _work.VoucherNumber.Find(x => x.Number == debitVoucherViewModel.VoucherNumber).FirstOrDefault();

                    if (voucherNumber.State != VoucherSaveStates.Running)
                    {
                        voucherNumber.State = VoucherSaveStates.Running;

                        _work.VoucherNumber.Update(voucherNumber);
                    }

                    // === === === //

                    _sql.DeleteVoucherDetail(debitVoucherViewModel.VoucherNumber);  //DELETE PREVIOUS TEMP VOUCHER DETAILS

                    foreach (var item in debitVoucherViewModel.DebitVoucherItems)
                    {
                        TempVoucherDetail tempVoucherDetail = new TempVoucherDetail
                        {
                            VoucherNumber = debitVoucherViewModel.VoucherNumber,
                            Particulars = item.Particulars,
                            TempVoucherHeadId = tempVoucherHead.Id,
                            AccountSubLedgerId = item.LedgerId == 0 ? (int?)null : item.LedgerId,
                            Ref = item.Ref,
                            DebitAmount = item.DebitAmount,
                            CreditAmount = item.CreditAmount
                        };

                        if (item.VoucherItemType == "Dr")
                        {
                            tempVoucherDetail.TransactionTypes = TransactionTypes.Debit;
                        }
                        else if (item.VoucherItemType == "Cr")
                        {
                            tempVoucherDetail.TransactionTypes = TransactionTypes.Credit;
                        }

                        _work.TempVoucherDetail.Add(tempVoucherDetail);
                    }

                    bool isSaved = _work.Save() > 0;

                    if (isSaved)
                    {
                        return Json(true);
                    }

                    return Json(false);
                }
            }

            return Json(false);
        }

        [HttpPost]
        public IActionResult SaveDebitVoucher(DebitVoucherViewModel debitVoucherViewModel)
        {
            if (!string.IsNullOrEmpty(debitVoucherViewModel.VoucherNumber))
            {
                var tempVoucherHead = _work.TempVoucherHead.Find(x => x.VoucherNumber == debitVoucherViewModel.VoucherNumber).FirstOrDefault();

                if (tempVoucherHead != null)
                {
                    VoucherHead voucherHead = new VoucherHead
                    {
                        VoucherId = tempVoucherHead.VoucherId,
                        VoucherNumber = debitVoucherViewModel.VoucherNumber,
                        EntryDate = Convert.ToDateTime(debitVoucherViewModel.Date),
                        PaidTo = debitVoucherViewModel.PaidTo,
                        PreparedBy = debitVoucherViewModel.PreparedBy,
                        PreparedByUserId = debitVoucherViewModel.PreparedByUserId,
                        ReceivedBy = debitVoucherViewModel.ReceivedBy,
                        VoucherSaveState = VoucherSaveStates.Saved
                    };

                    _work.VoucherHead.Add(voucherHead);

                    // === voucher number === //

                    VoucherNumber voucherNumber = _work.VoucherNumber.Find(x => x.Number == debitVoucherViewModel.VoucherNumber).FirstOrDefault();

                    if (voucherNumber.State != VoucherSaveStates.Saved)
                    {
                        voucherNumber.State = VoucherSaveStates.Saved;

                        _work.VoucherNumber.Update(voucherNumber);
                    }

                    // === === === //

                    foreach (var item in debitVoucherViewModel.DebitVoucherItems)
                    {
                        VoucherDetail voucherDetail = new VoucherDetail
                        {
                            VoucherNumber = debitVoucherViewModel.VoucherNumber,
                            Particulars = item.Particulars,
                            VoucherHeadId = voucherHead.Id,
                            AccountSubLedgerId = item.LedgerId == 0 ? (int?)null : item.LedgerId,
                            Ref = item.Ref,
                            DebitAmount = item.DebitAmount,
                            CreditAmount = item.CreditAmount
                        };

                        if (item.VoucherItemType == "Dr")
                        {
                            voucherDetail.TransactionTypes = TransactionTypes.Debit;
                        }
                        else if (item.VoucherItemType == "Cr")
                        {
                            voucherDetail.TransactionTypes = TransactionTypes.Credit;
                        }

                        _work.VoucherDetail.Add(voucherDetail);
                    }

                    bool isSaved = _work.Save() > 0;

                    _sql.DeleteVoucherHead(debitVoucherViewModel.VoucherNumber); //CHANGE ISDELETED & ISACTIVE VALUE
                    _sql.DeleteVoucherDetail(debitVoucherViewModel.VoucherNumber); //DELETE TEMP VOUCHER DETAILS

                    if (isSaved)
                    {
                        return Json(true);
                    }

                    return Json(false);
                }
            }

            return Json(false);
        }

        public IActionResult LoadDebitVoucher(string voucherNumber)
        {
            var voucher = _work.VoucherHead.GetAllWithDetails(voucherNumber);

            TempVoucherHead tempVoucherHead = _mapper.Map<TempVoucherHead>(voucher);

            tempVoucherHead.VoucherSaveState = VoucherSaveStates.Loading;

            return PartialView("_DebitVoucher", tempVoucherHead);
        }

        #endregion

        #region Credit Voucher

        [HttpGet]
        public IActionResult CreditVoucher()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PartialCreditVoucher()
        {
            Voucher voucher = _work.Voucher.FindByVoucherType(VoucherTypes.CreditVoucher);

            VoucherNumber lastVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseLastOrDefault(voucher.Id);

            if (lastVoucherNumber == null)
            {
                lastVoucherNumber = new VoucherNumber();
            }

            VoucherNumber unsavedVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseUnsavedVoucher(voucher.Id);

            TempVoucherHead tempVoucherHead = new TempVoucherHead();

            if (unsavedVoucherNumber == null)
            {
                tempVoucherHead.SetVoucherNumber(VoucherTypes.CreditVoucher, lastVoucherNumber.Number);

                tempVoucherHead.VoucherId = voucher.Id;

                tempVoucherHead.VoucherType = voucher.VoucherType;

                tempVoucherHead.VoucherSaveState = VoucherSaveStates.Unsaved;

                _work.TempVoucherHead.Add(tempVoucherHead);

                // === voucher number === //

                VoucherNumber voucherNumber = new VoucherNumber
                {
                    Number = tempVoucherHead.VoucherNumber,
                    State = tempVoucherHead.VoucherSaveState,
                    VoucherId = voucher.Id
                };

                _work.VoucherNumber.Add(voucherNumber);

                // === === === //

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return PartialView("_CreditVoucher", tempVoucherHead);
                }
            }
            else
            {
                TempVoucherHead unsavedVoucher = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(unsavedVoucherNumber.Number);

                return PartialView("_CreditVoucher", unsavedVoucher);
            }

            return PartialView("_CreditVoucher", tempVoucherHead);
        }

        [HttpPost]
        public IActionResult CreditVoucher(CreditVoucherViewModel creditVoucherViewModel)
        {
            return Json(true);
        }

        #endregion

        #region Debit Note

        [HttpGet]
        public IActionResult DebitNote()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PartialDebitNote()
        {
            Voucher voucher = _work.Voucher.FindByVoucherType(VoucherTypes.DebitNote);

            VoucherNumber lastVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseLastOrDefault(voucher.Id);

            if (lastVoucherNumber == null)
            {
                lastVoucherNumber = new VoucherNumber();
            }

            VoucherNumber unsavedVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseUnsavedVoucher(voucher.Id);

            TempVoucherHead tempVoucherHead = new TempVoucherHead();

            if (unsavedVoucherNumber == null)
            {
                tempVoucherHead.SetVoucherNumber(VoucherTypes.DebitNote, lastVoucherNumber.Number);

                tempVoucherHead.VoucherId = voucher.Id;

                tempVoucherHead.VoucherType = voucher.VoucherType;

                tempVoucherHead.VoucherSaveState = VoucherSaveStates.Unsaved;

                _work.TempVoucherHead.Add(tempVoucherHead);

                // === voucher number === //

                VoucherNumber voucherNumber = new VoucherNumber
                {
                    Number = tempVoucherHead.VoucherNumber,
                    State = tempVoucherHead.VoucherSaveState,
                    VoucherId = voucher.Id
                };

                _work.VoucherNumber.Add(voucherNumber);

                // === === === //

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return PartialView("_DebitNote", tempVoucherHead);
                }
            }
            else
            {
                TempVoucherHead unsavedVoucher = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(unsavedVoucherNumber.Number);

                return PartialView("_DebitNote", unsavedVoucher);
            }

            return PartialView("_DebitNote", tempVoucherHead);
        }

        [HttpPost]
        public IActionResult DebitNote(DebitNoteViewModel creditNoteViewModel)
        {
            return View();
        }

        #endregion

        #region Credit Note

        [HttpGet]
        public IActionResult CreditNote()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PartialCreditNote()
        {
            Voucher voucher = _work.Voucher.FindByVoucherType(VoucherTypes.CreditNote);

            VoucherNumber lastVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseLastOrDefault(voucher.Id);

            if (lastVoucherNumber == null)
            {
                lastVoucherNumber = new VoucherNumber();
            }

            VoucherNumber unsavedVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseUnsavedVoucher(voucher.Id);

            TempVoucherHead tempVoucherHead = new TempVoucherHead();

            if (unsavedVoucherNumber == null)
            {
                tempVoucherHead.SetVoucherNumber(VoucherTypes.CreditNote, lastVoucherNumber.Number);

                tempVoucherHead.VoucherId = voucher.Id;

                tempVoucherHead.VoucherType = voucher.VoucherType;

                tempVoucherHead.VoucherSaveState = VoucherSaveStates.Unsaved;

                _work.TempVoucherHead.Add(tempVoucherHead);

                // === voucher number === //

                VoucherNumber voucherNumber = new VoucherNumber
                {
                    Number = tempVoucherHead.VoucherNumber,
                    State = tempVoucherHead.VoucherSaveState,
                    VoucherId = voucher.Id
                };

                _work.VoucherNumber.Add(voucherNumber);

                // === === === //

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return PartialView("_CreditNote", tempVoucherHead);
                }
            }
            else
            {
                TempVoucherHead unsavedVoucher = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(unsavedVoucherNumber.Number);

                return PartialView("_CreditNote", unsavedVoucher);
            }

            return PartialView("_CreditNote", tempVoucherHead);
        }

        [HttpPost]
        public IActionResult CreditNote(CreditNoteViewModel creditNoteViewModel)
        {
            return View();
        }

        #endregion

        #region Contra

        [HttpGet]
        public IActionResult Contra()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PartialContra()
        {
            Voucher voucher = _work.Voucher.FindByVoucherType(VoucherTypes.Contra);

            VoucherNumber lastVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseLastOrDefault(voucher.Id);

            if (lastVoucherNumber == null)
            {
                lastVoucherNumber = new VoucherNumber();
            }

            VoucherNumber unsavedVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseUnsavedVoucher(voucher.Id);

            TempVoucherHead tempVoucherHead = new TempVoucherHead();

            if (unsavedVoucherNumber == null)
            {
                tempVoucherHead.SetVoucherNumber(VoucherTypes.Contra, lastVoucherNumber.Number);

                tempVoucherHead.VoucherId = voucher.Id;

                tempVoucherHead.VoucherType = voucher.VoucherType;

                tempVoucherHead.VoucherSaveState = VoucherSaveStates.Unsaved;

                _work.TempVoucherHead.Add(tempVoucherHead);

                // === voucher number === //

                VoucherNumber voucherNumber = new VoucherNumber
                {
                    Number = tempVoucherHead.VoucherNumber,
                    State = tempVoucherHead.VoucherSaveState,
                    VoucherId = voucher.Id
                };

                _work.VoucherNumber.Add(voucherNumber);

                // === === === //

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return PartialView("_Contra", tempVoucherHead);
                }
            }
            else
            {
                TempVoucherHead unsavedVoucher = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(unsavedVoucherNumber.Number);

                return PartialView("_Contra", unsavedVoucher);
            }

            return PartialView("_Contra", tempVoucherHead);
        }

        [HttpPost]
        public IActionResult Contra(ContraViewModel contraViewModel)
        {
            return View();
        }

        #endregion

        #region Conveyance

        [HttpGet]
        public IActionResult Conveyance()
        {
            ViewData["AccountLedgerName"] = new SelectList(_work.AccountLedger.GetAll(), "Id", "AccountLedgerName");
            return View();
        }

        [HttpGet]
        public IActionResult PartialConveyance()
        {
            ViewData["AccountSubLedgerName"] = new SelectList(_work.AccountSubLedger.GetAll(), "Id", "AccountSubLedgerName");

            Voucher voucher = _work.Voucher.FindByVoucherType(VoucherTypes.Contra);

            VoucherNumber lastVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseLastOrDefault(voucher.Id);

            if (lastVoucherNumber == null)
            {
                lastVoucherNumber = new VoucherNumber();
            }

            VoucherNumber unsavedVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseUnsavedVoucher(voucher.Id);

            TempVoucherHead tempVoucherHead = new TempVoucherHead();

            if (unsavedVoucherNumber == null)
            {
                tempVoucherHead.SetVoucherNumber(VoucherTypes.Contra, lastVoucherNumber.Number);

                tempVoucherHead.VoucherId = voucher.Id;

                tempVoucherHead.VoucherType = voucher.VoucherType;

                tempVoucherHead.VoucherSaveState = VoucherSaveStates.Unsaved;

                _work.TempVoucherHead.Add(tempVoucherHead);

                // === voucher number === //

                VoucherNumber voucherNumber = new VoucherNumber
                {
                    Number = tempVoucherHead.VoucherNumber,
                    State = tempVoucherHead.VoucherSaveState,
                    VoucherId = voucher.Id
                };

                _work.VoucherNumber.Add(voucherNumber);

                // === === === //

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return PartialView("_Conveyance", tempVoucherHead);
                }
            }
            else
            {
                TempVoucherHead unsavedVoucher = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(unsavedVoucherNumber.Number);

                return PartialView("_Conveyance", unsavedVoucher);
            }

            return PartialView("_Conveyance", tempVoucherHead);
        }

        [HttpPost]
        public IActionResult Conveyance(ConveyanceViewModel conveyanceViewModel)
        {
            return Json(true);
        }

        #endregion

        #region Journal

        [HttpGet]
        public IActionResult Journal()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PartialJournal()
        {
            Voucher voucher = _work.Voucher.FindByVoucherType(VoucherTypes.Contra);

            VoucherNumber lastVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseLastOrDefault(voucher.Id);

            if (lastVoucherNumber == null)
            {
                lastVoucherNumber = new VoucherNumber();
            }

            VoucherNumber unsavedVoucherNumber = _work.VoucherNumber.GetVoucherTypeWiseUnsavedVoucher(voucher.Id);

            TempVoucherHead tempVoucherHead = new TempVoucherHead();

            if (unsavedVoucherNumber == null)
            {
                tempVoucherHead.SetVoucherNumber(VoucherTypes.Contra, lastVoucherNumber.Number);

                tempVoucherHead.VoucherId = voucher.Id;

                tempVoucherHead.VoucherType = voucher.VoucherType;

                tempVoucherHead.VoucherSaveState = VoucherSaveStates.Unsaved;

                _work.TempVoucherHead.Add(tempVoucherHead);

                // === voucher number === //

                VoucherNumber voucherNumber = new VoucherNumber
                {
                    Number = tempVoucherHead.VoucherNumber,
                    State = tempVoucherHead.VoucherSaveState,
                    VoucherId = voucher.Id
                };

                _work.VoucherNumber.Add(voucherNumber);

                // === === === //

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return PartialView("_Journal", tempVoucherHead);
                }
            }
            else
            {
                TempVoucherHead unsavedVoucher = _work.TempVoucherHead.GetVoucherNumberWiseVoucher(unsavedVoucherNumber.Number);

                return PartialView("_Journal", unsavedVoucher);
            }

            return PartialView("_Journal", tempVoucherHead);
        }

        [HttpPost]
        public IActionResult Journal(JournalViewModel journalViewModel)
        {
            return View();
        }

        #endregion

        #region Voucher Approval

        public IActionResult VoucherApproval()
        {
            return View();
        }

        public IActionResult LoadVoucherApproval()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault().ToLower();

            var unsaved = bool.Parse(Request.Form["unsaved"].FirstOrDefault());
            var holded = bool.Parse(Request.Form["holded"].FirstOrDefault());
            var isPrepared = bool.Parse(Request.Form["isPrepared"].FirstOrDefault());
            var isChecked = bool.Parse(Request.Form["isChecked"].FirstOrDefault());
            var isRecommanded = bool.Parse(Request.Form["isRecommanded"].FirstOrDefault());
            var isApproved = bool.Parse(Request.Form["isApproved"].FirstOrDefault());

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var voucherHeads = _work.VoucherHead.GetAll();

            var tempVoucherHead = new List<TempVoucherHead>();

            var voucherHeadList = new List<VoucherVM>();

            if (unsaved)
            {
                tempVoucherHead = _work.TempVoucherHead.Find(x => x.VoucherSaveState == VoucherSaveStates.Running).ToList();

                voucherHeads.AddRange(tempVoucherHead.Select(x => new VoucherHead
                {
                    VoucherNumber = x.VoucherNumber,
                    PaidTo = x.PaidTo,
                    EntryDate = x.EntryDate,
                    TotalAmount = x.TotalAmount,
                    PreparedBy = x.PreparedBy,
                    CheckedBy = x.CheckedBy,
                    RecommendedBy = x.RecommendedBy,
                    ApprovedBy = x.ApprovedBy,
                    VoucherSaveState = x.VoucherSaveState
                }));

                voucherHeads = voucherHeads.Where(x => x.VoucherSaveState == VoucherSaveStates.Running).ToList();
            }

            if (holded)
            {
                tempVoucherHead = _work.TempVoucherHead.Find(x => x.VoucherSaveState == VoucherSaveStates.Hold).ToList();

                voucherHeads.AddRange(tempVoucherHead.Select(x => new VoucherHead
                {
                    VoucherNumber = x.VoucherNumber,
                    PaidTo = x.PaidTo,
                    EntryDate = x.EntryDate,
                    TotalAmount = x.TotalAmount,
                    PreparedBy = x.PreparedBy,
                    CheckedBy = x.CheckedBy,
                    RecommendedBy = x.RecommendedBy,
                    ApprovedBy = x.ApprovedBy,
                    VoucherSaveState = x.VoucherSaveState
                }));

                voucherHeads = voucherHeads.Where(x => x.VoucherSaveState == VoucherSaveStates.Hold).ToList();
            }

            if (isPrepared)
            {
                voucherHeads = voucherHeads.Where(x => x.IsChecked == false).ToList();
            }

            if (isChecked)
            {
                voucherHeads = voucherHeads.Where(x => x.IsChecked == true).ToList();
            }

            if (isRecommanded)
            {
                voucherHeads = voucherHeads.Where(x => x.IsRecommended == true).ToList();
            }

            if (isApproved)
            {
                voucherHeads = voucherHeads.Where(x => x.IsApproved == true).ToList();
            }

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                voucherHeads = voucherHeads.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                voucherHeads = voucherHeads.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                voucherHeads = voucherHeads.Where(x => x.VoucherNumber.ToLower().Contains(searchValue) || x.PaidTo.ToLower().Contains(searchValue) || x.VoucherType.ToString().ToLower().Contains(searchValue)).ToList();
            }

            foreach (var item in voucherHeads)
            {
                voucherHeadList.Add(new VoucherVM
                {
                    VoucherNumber = item.VoucherNumber,
                    PaidTo = item.PaidTo,
                    Date = item.EntryDate,
                    TotalAmount = item.TotalAmount,
                    PreparedBy = item.PreparedBy,
                    CheckedBy = item.CheckedBy,
                    RecommandedBy = item.RecommendedBy,
                    ApprovedBy = item.ApprovedBy,
                    VoucherSaveState = item.VoucherSaveState
                });
            }

            //total number of rows count     
            recordsTotal = voucherHeadList.Count();

            //Paging     
            var data = voucherHeadList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        #endregion
    }
}