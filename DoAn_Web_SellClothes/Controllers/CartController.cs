using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_Web_SellClothes.Assets.csharp;
using DoAn_Web_SellClothes.Models;
using DoAn_Web_SellClothes.MoMo;
using Newtonsoft.Json.Linq;

namespace DoAn_Web_SellClothes.Controllers
{
    public class CartController : BaseController
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: Cart
        public List<Giohang> LayGioHang()
        {
            List<Giohang> listgiohang = Session["Giohang"] as List<Giohang>;
            if (listgiohang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì khởi tạo listGioHang
                listgiohang = new List<Giohang>();
                Session["Giohang"] = listgiohang;
            }
            return listgiohang;
        }
        //Tổng số lượng
        private int TongSoLuong()
        {
            int tong = 0;
            List<Giohang> listgiohang = Session["Giohang"] as List<Giohang>;
            if (listgiohang != null)
            {
                tong = (int)listgiohang.Sum(n => n.iQuantityProduct);
            }
            return tong;
        }
        //Tính tổng tiền
        private int TongTien()
        {
            int tongtien = 0;
            List<Giohang> listgiohang = Session["Giohang"] as List<Giohang>;
            if (listgiohang != null)
            {
                tongtien = listgiohang.Sum(n => n.iThanhTien);
            }
            return tongtien;
        }
        //Cập nhật số lượng tồn của mỗi sản phẩm
        private void updateSoLuong(InvoiceDetail cthd)
        {
            var sp = data.ProductDetails.Single(p => p.IdProduct == cthd.IdProduct && p.IdSizeProduct == cthd.IdSizeProduct);
            sp.SoLuongTon = sp.SoLuongTon - cthd.Quantity;
            data.SubmitChanges();
        }
        //Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public ActionResult ThemGioHang(int? idProduct, string strURL) 
        {
            int? sizeid = null;
            //Lấy ra session    
            List<Giohang> listgiohang = LayGioHang();
            Session["Size"] = Request.Form["nameSize"];
            if (Request.Form["nameSize"] == null)
            {
                Session["Error"]= "Vui lòng chọn Size sản phẩm!";
                return Redirect(strURL);
            }
            else sizeid = Int32.Parse(Request.Form["nameSize"].ToString());
            int sl = Int32.Parse(Request.Form["quantity"].ToString());
            int soLuongTon = data.ProductDetails.SingleOrDefault(p => p.IdProduct == idProduct && p.IdSizeProduct == sizeid).SoLuongTon.Value;
            if(sl>soLuongTon)
            {
                Session["sl"] = 1;
                Session["Error1"] = "Số lượng mua lớn hơn số lượng tồn! Vui lòng chọn lại!";
                return Redirect(strURL);
            }
            else
            {
                Session["sl"] = null;
            }
            //Kiểm tra sản phẩm này tồn tại trong Session["Giohang"] chưa?
            Giohang giohang = listgiohang.Find(n => n.iIdProduct == idProduct && n.iSize==sizeid);
            if(giohang==null)
            {

                giohang = new Giohang(idProduct, sizeid, sl);
                listgiohang.Add(giohang);
                SetAlert("Thêm vào giỏ hàng thành công", "success");
                return Redirect(strURL);
                
            }
            else
            {
                giohang.iQuantityProduct++;
                SetAlert("Thêm vào giỏ hàng thành công", "success");
                return Redirect(strURL);
            }
        }
        public ActionResult Cart()
        {
            List<Giohang> listgiohang = LayGioHang();
            if(listgiohang.Count==0)
            {
                return RedirectToAction("ProductPage", "Product");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.TongTienShip = TongTien() + 25000;
            return View(listgiohang);
        }
        //Xóa 1 món hàng ra khỏi giỏ hàng
        public ActionResult RemoveItemInCart(int iProductId, int iSizeId)
        {
            List<Giohang> listProductInCart = LayGioHang();
            Giohang sp = listProductInCart.SingleOrDefault(n => n.iIdProduct == iProductId && n.iSize == iSizeId);
            if(sp != null)
            {
                listProductInCart.Remove(sp);
                return RedirectToAction("Cart");
            }
            if(listProductInCart.Count==0)
            {
                return RedirectToAction("ProductPage", "Product");
            }
            return RedirectToAction("Cart");
        }
        //Cập nhật lại số lượng trong giỏ hàng
        public ActionResult UpdateItemInCart(int iProductId, int iSizeId, FormCollection collection)
        {
            List<Giohang> listProductInCart = LayGioHang();
            Giohang sp = listProductInCart.SingleOrDefault(n => n.iIdProduct == iProductId && n.iSize == iSizeId);
            if (sp != null)
            {
                sp.iQuantityProduct = int.Parse(collection["quantity1"]);
            }
            return RedirectToAction("Cart");
        }
        //Xóa toàn bộ giỏ hàng
        public ActionResult RemoveCart()
        {
            List<Giohang> listProductInCart = LayGioHang();
            listProductInCart.Clear();
            return RedirectToAction("ProductPage", "Product");
        }
        [HttpGet]
        public ActionResult Checkout()
        {
            Account ac = (Account)Session["user"];
            Session["name"] = ac.FullName;
            Session["phone"] = ac.PhoneNumber;
            Session["address"] = ac.AddressUser;
            List<Giohang> listgiohang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.TongTienShip = TongTien() + 25000;
            return View(listgiohang);
        }
        [HttpPost]
        public ActionResult Checkout(string strURL, FormCollection collection)
        {
            Session["billing_name"] = null;
            Session["billing_phone"] = null;
            Session["billing_address"] = null;
            Session["billing_note"] = null;
            Session["billing_name"] = collection["billing_name"];
            Session["billing_phone"] = collection["billing_phone"];
            Session["billing_address"] = collection["billing_address"];
            Session["billing_note"] = collection["billing_note"];
            string httt = collection["Payment"];
            if(httt== "Thanh toán khi nhận hàng")
            {
                Account ac = (Account)Session["user"];
                Invoice ddh = new Invoice();
                List<Giohang> gh = LayGioHang();
                //List<InfoCustomerBill> info = null;

                ddh.IdAccount = ac.IdAccount;
                ddh.InvoiceNameReceiver = collection["billing_name"];
                ddh.InvoicePhoneReceiver = collection["billing_phone"];
                ddh.InvoiceAddressReceiver = collection["billing_address"];
                ddh.NoteInvoice = collection["billing_note"];
                ddh.InvoiceDate = DateTime.Now;
                ddh.TotalInvoice = TongTien() + 25000;
                ddh.PaymentsInvoice = collection["Payment"];
                ddh.StatusInvoice = false;
                ddh.Paid = false;
                data.Invoices.InsertOnSubmit(ddh);
                data.SubmitChanges();
                Session["idInvoice"] = ddh.IdInvoice;
                foreach (var item in gh)
                {
                    InvoiceDetail ctdh = new InvoiceDetail();
                    ctdh.IdSizeProduct = (int)item.iSize;
                    ctdh.IdProduct = (int)item.iIdProduct;
                    ctdh.IdInvoice = ddh.IdInvoice;
                    ctdh.Quantity = item.iQuantityProduct;
                    ctdh.UnitPrice = item.iPriceProduct;
                    int soLuongTon = data.ProductDetails.SingleOrDefault(p => p.IdProduct == item.iIdProduct && p.IdSizeProduct == item.iSize).SoLuongTon.Value;
                    if (soLuongTon < ctdh.Quantity)
                    {
                        ViewBag.SoLuongTon = "Sản phẩm hết hàng hoặc quá số lượng tồn, sản phẩm hết hàng sẽ được xóa khỏi gio hàng!";
                        List<Giohang> listProductInCart = LayGioHang();
                        Giohang sp = listProductInCart.SingleOrDefault(n => n.iIdProduct == item.iIdProduct && n.iSize == item.iSize);
                        listProductInCart.Remove(sp);
                        return this.Checkout();
                    }
                    updateSoLuong(ctdh);
                    data.InvoiceDetails.InsertOnSubmit(ctdh);
                }
                data.SubmitChanges();
                Session["Giohang"] = null;
                return RedirectToAction("Thanks", "Cart");
            }
            else if(httt== "Thanh toán MoMo")
            {
                List<Giohang> gh = LayGioHang();
                string endpoint = ConfigurationManager.AppSettings["endpoint"].ToString();
                string partnerCode = ConfigurationManager.AppSettings["partnerCode"].ToString();
                string accessKey = ConfigurationManager.AppSettings["accessKey"].ToString();
                string secretKey = ConfigurationManager.AppSettings["secretKey"].ToString();
                string oderInfor = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string orderInfo = "Proudct: ";
                for (int i = 0; i < gh.Count; i++)
                {

                    if (i == 0)
                    {
                        orderInfo += gh[i].iNameProduct + "(" + gh[i].iQuantityProduct + ")";
                    }
                    else
                    {
                        orderInfo += " + " + gh[i].iNameProduct + "(" + gh[i].iQuantityProduct + ")";
                    }
                }
                string returnUrl = ConfigurationManager.AppSettings["returnUrl"].ToString();
                string notifyUrl = ConfigurationManager.AppSettings["notifyUrl"].ToString();
                int tt = gh.Sum(n => n.iThanhTien);
                string amount = (tt + 25000).ToString();
                string orderid = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyUrl + "&extraData=" +
                    extraData;
                MoMoSecurity crypto = new MoMoSecurity();
                string signature = crypto.signSHA256(rawHash, secretKey);
                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyUrl },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };
                string reponseFromMoMo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
                JObject jmessage = JObject.Parse(reponseFromMoMo);
                return Redirect(jmessage.GetValue("payUrl").ToString());
            }
            return RedirectToAction("Thanks", "Cart");
        }
        public ActionResult ReturnUrl()
        {
                string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
                param = Server.UrlDecode(param);
                MoMoSecurity crypto = new MoMoSecurity();
                string secretKey = ConfigurationManager.AppSettings["secretKey"].ToString();
                string signature = crypto.signSHA256(param, secretKey);
                if (signature != Request["signature"].ToString())
                {
                    ViewBag.message = "Thông tin Request không hợp lệ";
                    //  return View("ErrorMoMo");

                }
                if (!Request.QueryString["errorCode"].Equals("0"))
                {
                    ViewBag.message = "Thanh toán thất bại";
                    // return View("ErrorMoMo");
                }
                else
                {
                    Account ac = (Account)Session["user"];
                    Invoice ddh = new Invoice();
                    List<Giohang> gh = LayGioHang();
                    ddh.IdAccount = ac.IdAccount;
                    ddh.InvoiceNameReceiver = (string)Session["billing_name"];
                    ddh.InvoicePhoneReceiver = (string)Session["billing_phone"];
                    ddh.InvoiceAddressReceiver = (string)Session["billing_address"];
                    ddh.NoteInvoice = (string)Session["billing_note"];
                    ddh.InvoiceDate = DateTime.Now;
                    ddh.TotalInvoice = TongTien() + 25000;
                    ddh.PaymentsInvoice = "Thanh toán MoMo";
                    ddh.StatusInvoice = false;
                    ddh.Paid = true;
                    data.Invoices.InsertOnSubmit(ddh);
                    data.SubmitChanges();
                    Session["idInvoice"] = ddh.IdInvoice;
                    foreach (var item in gh)
                    {
                        InvoiceDetail ctdh = new InvoiceDetail();
                        ctdh.IdSizeProduct = (int)item.iSize;
                        ctdh.IdProduct = (int)item.iIdProduct;
                        ctdh.IdInvoice = ddh.IdInvoice;
                        ctdh.Quantity = item.iQuantityProduct;
                        ctdh.UnitPrice = item.iPriceProduct;
                        int soLuongTon = data.ProductDetails.SingleOrDefault(p => p.IdProduct == item.iIdProduct && p.IdSizeProduct == item.iSize).SoLuongTon.Value;
                        if (soLuongTon < ctdh.Quantity)
                        {
                            ViewBag.SoLuongTon = "Sản phẩm hết hàng hoặc quá số lượng tồn, sản phẩm hết hàng sẽ được xóa khỏi gio hàng!";
                            List<Giohang> listProductInCart = LayGioHang();
                            Giohang sp = listProductInCart.SingleOrDefault(n => n.iIdProduct == item.iIdProduct && n.iSize == item.iSize);
                            listProductInCart.Remove(sp);
                            return this.Checkout();
                        }
                        updateSoLuong(ctdh);
                        data.InvoiceDetails.InsertOnSubmit(ctdh);
                    }
                    data.SubmitChanges();
                    ViewBag.message = "Thanh toán thành công";
                    Session["Giohang"] = null;
                    Session["billing_name"] = null;
                    Session["billing_phone"] = null;
                    Session["billing_address"] = null;
                    Session["billing_note"] = null;
                return View();
            }
            return View();    

        }
        public ActionResult NotifyUrl()
        {
            string param = "";
            param = "partner_code=" + Request["partner_code"] +
                    "&access_key=" + Request["access_key"] +
                    "&amount=" + Request["amount"] +
                    "&oder_id=" + Request["oder_id"] +
                    "&order_info=" + Request["order_info"] +
                    "&order_type=" + Request["order_type"] +
                    "&transacsion_id=" + Request["transacsion_id"] +
                    "&message=" + Request["message"] +
                    "&reponse_time=" + Request["reponse_time"] +
                    "&status_code=" + Request["status_code"];
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string secretKey = ConfigurationManager.AppSettings["secretKey"].ToString();
            string signature = crypto.signSHA256(param, secretKey);
            //Không được pháp cập nhật trạng thái đơn trong Database khác trạng thái đang chờ thanh toán
            if (signature != Request["signature"].ToString())
            {
                //cap nhat don hang that bai
            }
            string status_code = Request["status_code"].ToString();
            if ((status_code != "0"))
            {

            }
            else
            {
 
            }
        
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Thanks()
        {
            return View();
        }
    }
}
