using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelBill.Models;
using System.Globalization;

namespace HotelBill.Controllers
{
    public class ProductController : Controller
    {
        Bill_DBEntities db = new Bill_DBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ItemAdd()
        {
            ItemMaster obj = new ItemMaster();
            obj.ItemCode = getNewItemNo();
            return View(obj);
        }

        public string getNewItemNo()
        {
            int dbCount = db.CounterMasters.Where(x => x.counterName == "item").FirstOrDefault().counterValue;
            string newNo = BillSupport.increamentItem(dbCount);
            return newNo;
        }


        [HttpPost]
        public ActionResult ItemAdd(ItemMaster itm)
        {
            try
            {
                var checkDuplicate = duplicateCheck(itm.ItemDetails);
                if (!checkDuplicate)
                {
                    ItemMaster itm2 = new ItemMaster();
                    itm2.ItemCode = itm.ItemCode;
                    itm2.ItemDetails = itm.ItemDetails;
                    itm2.HSN = itm.HSN;
                    itm2.Rate = itm.Rate;
                    itm2.Gst = itm.Gst;
                    itm2.IsActive = true;
                    itm2.CreatedDate = DateTime.Now;
                    db.ItemMasters.Add(itm2);
                    db.SaveChanges();

                    int newDbCounter = Convert.ToInt32(itm.ItemCode.Remove(0, 3));
                    CounterMaster cnt = db.CounterMasters.Where(x => x.counterName == "item").FirstOrDefault();
                    cnt.counterValue = newDbCounter;
                    db.SaveChanges();

                    return RedirectToAction("ItemList");
                }
                ViewBag.isPresent = "Item Name exist please chanege the item details";
                return View();
            }
            catch (Exception) { return View(); }
        }

        public ActionResult ItemList()
        {
            var itemList = db.ItemMasters.OrderByDescending(x => x.ItemId).ToList();
            return View(itemList);
        }

        public ActionResult ItemEdit(int ?id)
        {
            if(id!=0 || id != null )
            {
                var item = db.ItemMasters.Where(x => x.ItemId == id).FirstOrDefault();
                return View(item);
            }
            ViewBag.type = "notfound";
            return View();
        }

        private bool duplicateCheck(string itemName)
        {
            var isThere = db.ItemMasters.Any(x => x.ItemDetails == itemName);
            return isThere;
        }
        private bool duplicateCheckCust(string CustName)
        {
            var isThere = db.CustomerMasters.Any(x => x.custName == CustName);
            return isThere;
        }

        [HttpPost]
        public ActionResult ItemEdit(ItemMaster itm)
        {
            try
            {
                ItemMaster itm2 = db.ItemMasters.Where(x => x.ItemId == itm.ItemId).FirstOrDefault();
                itm2.ItemDetails = itm.ItemDetails;
                itm2.HSN = itm.HSN;
                itm2.Rate = itm.Rate;
                itm2.Gst = itm.Gst;
                itm2.IsActive = itm.IsActive;
                itm2.UpdatedDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ItemList");
            }
            catch (Exception) { return View(); }
        }



        public ActionResult ItemView(int? id)
        {
            if (id != 0 || id != null)
            {
                var item = db.ItemMasters.Where(x => x.ItemId == id).FirstOrDefault();
                return View(item);
            }
            ViewBag.type = "notfound";
            return View();
        }

        public ActionResult CustomerAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CustomerAdd(CustomerMaster cust)
        {
            var checkDuplicate = duplicateCheckCust(cust.custName);
            if (!checkDuplicate)
            {
                cust.isActive = true;
                cust.createdDate = DateTime.Now;
                db.CustomerMasters.Add(cust);
                db.SaveChanges();
                return RedirectToAction("CustomerList");
            }
            ViewBag.isPresent = "Custpmer Name exist please chanege the Customer Name";
            return View();
        }

        public ActionResult CustomerList()
        {
            var custList = db.CustomerMasters.OrderBy(x => new { x.isActive, x.custName }).ToList().OrderByDescending(x=>x.custId);
            return View(custList);
        }

        public ActionResult CustomerView(int id)
        {
            var cust = db.CustomerMasters.Where(x => x.custId == id).FirstOrDefault();
            return View(cust);
        }

        public ActionResult CustomerEdit(int id)
        {
            var cust = db.CustomerMasters.Where(x => x.custId == id).FirstOrDefault();
            return View(cust);
        }

        [HttpPost]
        public ActionResult CustomerEdit(CustomerMaster cust)
        {
            try
            {
                CustomerMaster cust2 = db.CustomerMasters.Where(x => x.custId == cust.custId).FirstOrDefault();
                cust2.custName = cust.custName;
                cust2.addr1 = cust.addr1;
                cust2.addr2 = cust.addr2;
                cust2.addr3 = cust.addr3;
                cust2.gstType = cust.gstType;
                cust2.gstIn = cust.gstIn;
                cust2.isActive = cust.isActive;
                cust2.updatedDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("CustomerList");
            }
            catch (Exception) { return View(); throw; }
        }

        public ActionResult ItemAdd2()
        {
            return View();
        }

        public ActionResult ItemAddEmpty()
        {
            return View();
        }

        public ActionResult CustomersList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCustomers()
        {
            var custList = (from ss in db.CustomerMasters
                            select new CustomerDTO
                            {
                                addr1 = ss.addr1,
                                addr2 = ss.addr2,
                                addr3 = ss.addr3,
                                createdDate = ss.createdDate.ToString(),
                                updatedDate = ss.updatedDate.ToString(),
                                custId = ss.custId,
                                isActive = ss.isActive,
                                custName = ss.custName,
                                gstIn = ss.gstIn
                            }).ToList();
            return Json(custList);
        }
    }
}
