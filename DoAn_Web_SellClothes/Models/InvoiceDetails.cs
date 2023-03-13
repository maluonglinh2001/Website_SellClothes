using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_Web_SellClothes.Models
{
    public class InvoiceDetails
    {
        public string ImageProduct { get; set; }
        public string NameProduct { get; set; }
        public string SizeProduct { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int ThanhTienInvoiceDetails { get; set; }
        public int idProduct { get; set; }
    }
}