using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBill.Models
{
    public class CustomerDTO
    {
        public int custId { get; set; }
        public string custName { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string addr3 { get; set; }
        public string gstIn { get; set; }
        public bool isActive { get; set; }
        public string createdDate { get; set; }
        public string updatedDate { get; set; }
    }
}