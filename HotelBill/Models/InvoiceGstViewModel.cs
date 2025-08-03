using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace HotelBill.Models
{

    public class InvoiceGstViewModel
    {
        public int IId { get; set; }
        public string IDraftNo { get; set; }
        public string IinvoiceStatus { get; set; }
        public string custName { get; set; }
        public string cusstAdd1 { get; set; }
        public string custAdd2 { get; set; }
        public string custAdd3 { get; set; }
        public string custGstin { get; set; }
        public string INo { get; set; }
        public int ICustId { get; set; }
        public int? IGstType { get; set; }
        public Nullable<System.DateTime> IDate { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalGST { get; set; }
        public decimal TotalTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<InvoiceGstItemTransactionViewModel> ItemTransList { get; set; }
        public string InWord { get; set; }
        public bool IsDraft { get; set; }
       
    }
    public class InvoiceGstItemTransactionViewModel
    {
        public int itm_IdT { get; set; }
        public string itm_InvoiceNoT { get; set; }
        public string itm_ItemCodeT { get; set; }
        public string itm_ItemDetails { get; set; }
        public string itm_HSN { get; set; }
        public Nullable<int> itm_Quantity { get; set; }
        public decimal itm_Rate { get; set; }
        public decimal itm_Value { get; set; }
        public decimal itm_GstRate { get; set; }
        public decimal itm_GstValue { get; set; }

        public decimal itm_Total { get; set; }
        public Nullable<System.DateTime> itm_CreatedDate { get; set; }
        public Nullable<System.DateTime> itm_UpdatedDate { get; set; }
        public bool itm_IsActive { get; set; }
    }
    public class ReturnResult
    {
        public string newDraftNo { get; set; }
        public string newInvoiceNo { get; set; }

    }

    #region ReturnDraftResult
    public class ReturnDraftResult
    {
        public int IId { get; set; }
        public string INo { get; set; }
        public string IDraftNo { get; set; }
        public string IDate { get; set; }
        public string ICustId { get; set; }
        public string IinvoiceStatus { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalGST { get; set; }
        public decimal TotalTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public List<ItemTransactionDetails> ItemTransactions { get; set; }
    }
   
    public class ItemTransactionDetails
    {
        public int IdT { get; set; }
        public string InvoiceNoT { get; set; }
        public string DraftNoT { get; set; }
        public string ItemCodeT { get; set; }
        public string ItemDetails { get; set; }
        public string HSN { get; set; }
        public Nullable<int> Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        public decimal GstRate { get; set; }
        public decimal GstValue { get; set; }
        public decimal Total { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
    #endregion
   

   
}