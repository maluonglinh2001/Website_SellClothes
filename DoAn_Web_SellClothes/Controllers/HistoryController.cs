using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_Web_SellClothes.Models;
namespace DoAn_Web_SellClothes.Controllers
{
    public class HistoryController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: History
        public ActionResult History()
        {
            Account ac = (Account)Session["user"];
            var history = from h in data.Invoices where h.IdAccount == ac.IdAccount orderby h.IdInvoice descending select h;
            foreach(var item in history)
            {
                if (item.StatusInvoice == false)
                {
                    ViewBag.StatusInvoice = "Chưa giao hàng";
                }
                else if (item.StatusInvoice == true)
                {
                    ViewBag.StatusInvoice = "Đã giao hàng";
                }
                if (item.Paid == false)
                {
                    ViewBag.Paid = "Chưa thanh toán";
                }
                else if(item.Paid==true)
                {
                    ViewBag.Paid = "Đã thanh toán";
                }
            }
            return View(history);
        }

        public ActionResult HistoryDetails()
        {
            int idInvoice = Int32.Parse(Request.QueryString["idInvoice"]);
            ViewBag.idInvoice = idInvoice;
            var invoicedetail = from ind in data.InvoiceDetails
                                join i in data.Invoices on ind.IdInvoice equals i.IdInvoice
                                join s in data.SizeProducts on ind.IdSizeProduct equals s.IdSizeProduct
                                join p in data.Products on ind.IdProduct equals p.IdProduct
                                where ind.IdInvoice == idInvoice
                                select new InvoiceDetails {
                                    idProduct = p.IdProduct,
                                    ImageProduct = p.ImageProduct,
                                    NameProduct = p.NameProduct,
                                    SizeProduct = s.NameSizeProduct,
                                    Quantity = ind.Quantity,
                                    UnitPrice = ind.UnitPrice,
                                    ThanhTienInvoiceDetails = ind.Quantity * ind.UnitPrice
                                };
            
            return View(invoicedetail);
        }
    }
}