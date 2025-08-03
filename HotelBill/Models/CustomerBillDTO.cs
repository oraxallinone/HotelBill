using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBill.Models
{
    public class CustomerBillDTO
    {
        public int IId { get; set; }
        public string INo { get; set; }
        public string IDraftNo { get; set; }
        public Nullable<System.DateTime> IDate { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public bool IsActive { get; set; }


        public string custName { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string addr3 { get; set; }
        public string gstIn { get; set; }
        public Nullable<int> gstType { get; set; }
    }
}