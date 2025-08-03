using HotelBill.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelBill.Controllers
{
    public class InvoiceController : Controller
    {
        Bill_DBEntities db = new Bill_DBEntities();

        public ActionResult GstInvoiceCreate(string draftNo)
        {
            InvoiceGstViewModel obj = new InvoiceGstViewModel();
            try
            {
                if (draftNo != null)
                {
                    var draftData = db.InvoiceDetails.Where(x => x.IDraftNo == draftNo && x.IinvoiceStatus == "draft" && x.IsActive == true).FirstOrDefault();
                    if (draftData.IsActive)
                    {
                        obj.IsDraft = true;
                        obj.IDraftNo = draftNo;
                        obj.INo = getNewInvoiceNo();
                        obj.IDate = Convert.ToDateTime(draftData.IDate);
                        //rest data fill form js

                    }
                    else
                    {
                        obj.IDraftNo = "error";
                        obj.INo = "error";
                    }

                    ViewBag.items = new SelectList(db.ItemMasters.ToList(), "ItemId", "ItemDetails");
                    ViewBag.customer = new SelectList(GetCustomerList(), "CustBasicInfo", "CustName");

                }
                else
                {
                    obj.IsDraft = false;
                    obj.IDraftNo = getNewDraftNo();
                    obj.INo = getNewInvoiceNo();
                    ViewBag.items = new SelectList(db.ItemMasters.ToList(), "ItemId", "ItemDetails");
                    ViewBag.customer = new SelectList(GetCustomerList(), "CustBasicInfo", "CustName");
                }
                return View(obj);
            }
            catch (Exception) { return View(); throw; }
            finally { obj = null; }
        }

        private List<CustomerInformation> GetCustomerList()
        {
            return (from cust in db.CustomerMasters
                    select new CustomerInformation
                    {
                        CustBasicInfo = cust.custId.ToString() + "-" + cust.gstType.ToString(),
                        CustName = cust.custName
                    }).ToList();
        }

        public string getNewDraftNo()
        {
            int dbCount = db.CounterMasters.Where(x => x.counterName == "invoiceDraft").FirstOrDefault().counterValue;
            string newNo = BillSupport.increamentDraft(dbCount);
            return newNo;
        }

        public string getNewInvoiceNo()
        {
            int dbCount = db.CounterMasters.Where(x => x.counterName == "invoiceGst").FirstOrDefault().counterValue;
            string newNo = BillSupport.increamentInvoice(dbCount);
            return newNo;
        }

        [HttpPost]
        public ActionResult GstInvoiceCreate(InvoiceGstViewModel model)
        {
            try
            {
                DeactiveOldDraft(model.IDraftNo);
                InvoiceDetail inv = new InvoiceDetail();
                if (model.IinvoiceStatus == "invoice")
                {
                    inv.INo = model.INo;
                }

                inv.IDraftNo = model.IDraftNo;
                inv.IinvoiceStatus = model.IinvoiceStatus;
                inv.IDate = model.IDate;
                inv.ICustId = model.ICustId;
                inv.TotalValue = model.TotalValue;
                inv.TotalGST = model.TotalGST;
                inv.TotalTotal = model.TotalTotal;
                inv.CreatedDate = DateTime.Now;
                inv.IsActive = true;
                db.InvoiceDetails.Add(inv);
                db.SaveChanges();

                foreach (var itm in model.ItemTransList)
                {
                    ItemTransaction itr = new ItemTransaction();
                    if (model.IinvoiceStatus == "invoice")
                    {
                        itr.InvoiceNoT = model.INo;
                    }

                    itr.DraftNoT = model.IDraftNo;
                    itr.ItemCodeT = itm.itm_ItemCodeT;
                    itr.ItemDetails = itm.itm_ItemDetails;
                    itr.HSN = itm.itm_HSN;
                    itr.Quantity = itm.itm_Quantity;
                    itr.Rate = itm.itm_Rate;
                    itr.Value = itm.itm_Value;
                    itr.GstRate = itm.itm_GstRate;
                    itr.GstValue = itm.itm_GstValue;
                    itr.Total = itm.itm_Total;
                    itr.CreatedDate = DateTime.Now;
                    itr.IsActive = true;
                    db.ItemTransactions.Add(itr);
                    db.SaveChanges();
                }

                int newDbDraftCounter = Convert.ToInt32(model.IDraftNo.Remove(0, 1));
                CounterMaster cntDraft = db.CounterMasters.Where(x => x.counterName == "invoiceDraft").FirstOrDefault();
                cntDraft.counterValue = newDbDraftCounter;
                db.SaveChanges();


                if (model.IinvoiceStatus == "invoice")
                {
                    int newDbCounter = Convert.ToInt32(model.INo.Remove(0, 3));
                    CounterMaster cnt = db.CounterMasters.Where(x => x.counterName == "invoiceGst").FirstOrDefault();
                    cnt.counterValue = newDbCounter;
                    db.SaveChanges();
                }



                ReturnResult result = new ReturnResult//to-note
                {
                    newDraftNo = getNewDraftNo(),
                    newInvoiceNo = getNewInvoiceNo()
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Json("0"); throw;
            }
            finally
            {

            }
        }

        [HttpPost]
        public ActionResult GetItmById(int id)
        {
            var item = db.ItemMasters.Where(x => x.ItemId == id).FirstOrDefault();
            return Json(item);
        }

        [HttpPost]
        public JsonResult GetItemByCode(string itemCode)
        {
            var singleItem = db.ItemMasters.Where(x => x.ItemDetails == itemCode).FirstOrDefault();
            return Json(singleItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GstInvoiceList()
        {
            var billList = (from a in db.InvoiceDetails
                            join b in db.CustomerMasters
                            on a.ICustId equals b.custId
                            where a.IinvoiceStatus == "invoice"
                            select new CustomerBillDTO
                            {
                                IId = a.IId,
                                INo = a.INo,
                                IDraftNo=a.IDraftNo,
                                IDate = a.IDate,
                                TotalValue = a.TotalValue,
                                TotalTotal = a.TotalTotal,
                                GrandTotal = a.GrandTotal,
                                IsActive = a.IsActive,
                                custName = b.custName,
                                addr1 = b.addr1,
                                addr2 = b.addr2,
                                addr3 = b.addr3,
                                gstIn = b.gstIn,
                                gstType = b.gstType
                            }).OrderByDescending(x => x.IId).ToList();

            return View(billList);
        }

        public ActionResult GstDraftList()
        {
            var billList = (from a in db.InvoiceDetails
                            join b in db.CustomerMasters
                            on a.ICustId equals b.custId
                            where a.IinvoiceStatus == "draft" && a.IsActive == true
                            select new CustomerBillDTO
                            {
                                IId = a.IId,
                                INo = a.INo,
                                IDate = a.IDate,
                                IDraftNo = a.IDraftNo,
                                TotalValue = a.TotalValue,
                                TotalTotal = a.TotalTotal,
                                GrandTotal = a.GrandTotal,
                                IsActive = a.IsActive,
                                custName = b.custName,
                                addr1 = b.addr1,
                                addr2 = b.addr2,
                                addr3 = b.addr3,
                                gstIn = b.gstIn,
                                gstType = b.gstType
                            }).OrderByDescending(x => x.IId).ToList();

            return View(billList);
        }

        public ActionResult GstInvoicePrint(string INo)
        {
            var inviceById = (from s in db.InvoiceDetails
                              join c in db.CustomerMasters
                              on s.ICustId equals c.custId
                              where s.INo == INo
                              select new InvoiceGstViewModel
                              {
                                  INo = s.INo,
                                  custName = c.custName,
                                  cusstAdd1 = c.addr1,
                                  custAdd2 = c.addr2,
                                  custAdd3 = c.addr3,
                                  custGstin = c.gstIn,
                                  IGstType = c.gstType,
                                  IDate = s.IDate,
                                  TotalGST = s.TotalGST,
                                  TotalValue = s.TotalValue,
                                  TotalTotal = s.TotalTotal,
                                  GrandTotal = s.TotalTotal,
                              }).FirstOrDefault();

            var itemTransaction = (from ss in db.ItemTransactions
                                   where ss.InvoiceNoT == inviceById.INo
                                   select new InvoiceGstItemTransactionViewModel
                                   {
                                       itm_InvoiceNoT = ss.InvoiceNoT,
                                       itm_ItemCodeT = ss.ItemCodeT,
                                       itm_ItemDetails = ss.ItemDetails,
                                       itm_HSN = ss.HSN,
                                       itm_Quantity = ss.Quantity,
                                       itm_Rate = ss.Rate,
                                       itm_Value = ss.Value,
                                       itm_GstRate = ss.GstRate,
                                       itm_GstValue = ss.GstValue,
                                       itm_Total = ss.Total
                                   }).ToList();

            inviceById.ItemTransList = itemTransaction;
            inviceById.InWord = BillSupport.NumberToWords(Convert.ToInt32(inviceById.TotalTotal));
            return View(inviceById);
        }

        public JsonResult GetDraftData(string draftNo)
        {
            ReturnDraftResult data = new ReturnDraftResult();

            var invoiceData = db.InvoiceDetails.Where(x => x.IDraftNo == draftNo && x.IinvoiceStatus == "draft" && x.IsActive == true).FirstOrDefault();
           

            data.IId = invoiceData.IId;
            data.INo = invoiceData.INo;
            data.IDraftNo = invoiceData.IDraftNo;
            data.IDate = Convert.ToString(invoiceData.IDate);
            data.ICustId = db.CustomerMasters.Where(x => x.custId == invoiceData.ICustId).Select(y => y.custId).FirstOrDefault().ToString() + "-" + db.CustomerMasters.Where(x => x.custId == invoiceData.ICustId).Select(y => y.gstType).FirstOrDefault().ToString();;
            data.IinvoiceStatus = invoiceData.IinvoiceStatus;
            data.TotalValue = invoiceData.TotalValue;
            data.TotalGST = invoiceData.TotalGST;
            data.TotalTotal = invoiceData.TotalTotal;
            data.GrandTotal = invoiceData.GrandTotal;
            data.CreatedDate = invoiceData.CreatedDate;
            data.UpdatedDate = invoiceData.UpdatedDate;
            data.IsActive = invoiceData.IsActive;

            data.ItemTransactions = (from d in db.ItemTransactions
                                     where d.DraftNoT == draftNo && d.IsActive == true
                                     select new ItemTransactionDetails
                                     {
                                         IdT = d.IdT,
                                         InvoiceNoT = d.InvoiceNoT,
                                         DraftNoT = d.DraftNoT,
                                         ItemCodeT = d.ItemCodeT,
                                         ItemDetails = d.ItemDetails,
                                         HSN = d.HSN,
                                         Quantity = d.Quantity,
                                         Rate = d.Rate,
                                         Value = d.Value,
                                         GstRate = d.GstRate,
                                         GstValue = d.GstValue,
                                         Total = d.Total,
                                         CreatedDate = d.CreatedDate,
                                         UpdatedDate = d.UpdatedDate,
                                         IsActive = d.IsActive,
                                     }).ToList();




            //var draftItems = db.ItemTransactions.Where(x => x.DraftNoT == draftNo && x.IsActive == true).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        private void DeactiveOldDraft(string draftNo)
        {
            InvoiceDetail invDetails = db.InvoiceDetails.Where(x => x.IDraftNo == draftNo && x.IsActive == true).FirstOrDefault();


            if (invDetails != null)
            {
                invDetails.IsActive = false;
                invDetails.UpdatedDate = DateTime.Now;
                db.SaveChanges();
            }

            List<ItemTransaction> items = db.ItemTransactions.Where(x => x.DraftNoT == draftNo && x.IsActive == true).ToList();
            foreach (var eachItm in items)
            {
                ItemTransaction tempItem = db.ItemTransactions.Where(x => x.IdT == eachItm.IdT).FirstOrDefault();
                tempItem.IsActive = false;
                tempItem.UpdatedDate = DateTime.Now;
                db.SaveChanges();
            }
        }

        public ActionResult CancelInvoice(string dNo)
        {
            InvoiceDetail invoice = db.InvoiceDetails.Where(x => x.IDraftNo == dNo && x.IsActive == true).FirstOrDefault();
            invoice.INo = null;
            invoice.IinvoiceStatus = "draft";
            invoice.UpdatedDate = DateTime.Now;
            //db.SaveChanges();


            List<ItemTransaction> items = db.ItemTransactions.Where(x => x.DraftNoT == dNo && x.IsActive == true).ToList();
            foreach (var item in items)
            {
                item.InvoiceNoT = null;
                item.UpdatedDate = DateTime.Now;
            }
            db.SaveChanges();





            return RedirectToAction("GstDraftList");
        }
    }
}
