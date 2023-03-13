using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DoAn_Web_SellClothes.Models;

namespace DoAn_Web_SellClothes.Controllers
{
    public class AccountController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: Account
        private static readonly int CHECK_EMAIL = 1;
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection, Account ac)
        {
            var hoten = collection["fullname"];
            var email = collection["email"];
            var sodienthoai = collection["phone_number"];
            var diachi = collection["address"];
            var matkhau = collection["password"];
            var matkhaunhaplai = collection["confirmpassword"];
            if (String.IsNullOrEmpty(hoten) || String.IsNullOrEmpty(email) 
                || String.IsNullOrEmpty(matkhau) || String.IsNullOrEmpty(matkhaunhaplai) 
                || String.IsNullOrEmpty(diachi) || String.IsNullOrEmpty(sodienthoai))
            {
                ViewData["Error"] = "Vui lòng điền đầy đủ nội dung";
                return this.Register();
            }
            else if (checkUser(email, CHECK_EMAIL))
            {
                ViewData["Error"] = "Tài khoản đã tồn tại";
                return this.Register();
            }
            else if (sodienthoai.ToString().Length != 10)
            {
                ViewData["Error"] = "Số điện thoại phải 10 số";
                return this.Register();
            }
            else if(matkhau.ToString().Length>=24 || matkhau.ToString().Length <= 8)
            {
                ViewData["Error"] = "Độ dài mật khẩu nhiều hơn 8 và ít hơn 24";
                return this.Register();
            }
            else if (!String.Equals(matkhau.ToString(), matkhaunhaplai.ToString()))
            {
                ViewData["Error"] = "Mật khẩu không khớp";
                return this.Register();
            }
            else
            {
                ac.FullName = hoten;
                ac.Email = email;
                ac.PhoneNumber = sodienthoai;
                ac.AddressUser = diachi;
                ac.PasswordUser = MD5Hash(matkhau);
                data.Accounts.InsertOnSubmit(ac);
                data.SubmitChanges();
                return RedirectToAction("LogIn");
            }
        }
        private bool checkUser(string str, int value)
        {
            if (value == 1)
            {
                var a = data.Accounts.FirstOrDefault(p => p.Email == str);
                if (a != null) return true;
            }
            return false;
        }
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(FormCollection collection)
        {
            var tendangnhap = collection["username"];
            var matkhau = collection["password"];
            var user = data.Accounts.SingleOrDefault(p => p.Email == tendangnhap);
            if (String.IsNullOrEmpty(tendangnhap) || String.IsNullOrEmpty(matkhau))
            {
                ViewData["Error"] = "Vui lòng điền đầy đủ nội dung";
                return this.LogIn();
            }
            else if (user == null)
            {
                ViewData["Error"] = "Sai tài khoản";
                return this.LogIn();
            }
            else if (!String.Equals(MD5Hash(matkhau), user.PasswordUser))
            {
                ViewData["Error"] = "Sai mật khẩu";
                return this.LogIn();
            }
            else
            {
                Session["user"] = user;
                Session["name"] = user.FullName;
                Session["idAccount"] = user.IdAccount;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult _User()
        {
            return PartialView();
        }
        public ActionResult LogOut()
        {
            Session["user"] = null;
            Session["name"] = null;
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult AccountInformation()
        {
            Account ac = (Account)Session["user"];
            return View(ac);
        }
        [HttpPost]
        public ActionResult AccountInformation(FormCollection collection)
        {
            Account ac = (Account)Session["user"];
            var User = data.Accounts.SingleOrDefault(p => p.IdAccount == ac.IdAccount);
            var fullnameuser = collection["fullname"];
            var emailuser = collection["email"];
            var phoneuser = collection["phone_number"];
            var addressuser = collection["address"];
            var oldpassuser = collection["oldpass"];
            var newpassuser = collection["newpass"];
            var renewpassuser = collection["renewpass"];
            if (String.IsNullOrEmpty(fullnameuser) || String.IsNullOrEmpty(emailuser) || String.IsNullOrEmpty(phoneuser) ||
                String.IsNullOrEmpty(addressuser))
            {
                ViewBag.Error = "Thông tin không được để trống";
                return this.AccountInformation();
            }
            else if (String.IsNullOrEmpty(renewpassuser) && String.IsNullOrEmpty(newpassuser) && String.IsNullOrEmpty(oldpassuser) && !String.IsNullOrEmpty(fullnameuser) && !String.IsNullOrEmpty(emailuser) && !String.IsNullOrEmpty(phoneuser) &&
                !String.IsNullOrEmpty(addressuser))
            {
                User.FullName = fullnameuser;
                User.Email = emailuser;
                User.PhoneNumber = phoneuser;
                User.AddressUser = addressuser;
                Session["user"] = User;
                data.SubmitChanges();
                ViewData["Info"] = "Cập nhật thành công!";
                return this.AccountInformation();
            }
            else if (String.IsNullOrEmpty(renewpassuser)&&String.IsNullOrEmpty(newpassuser)&&!String.IsNullOrEmpty(oldpassuser) && !String.IsNullOrEmpty(fullnameuser) && !String.IsNullOrEmpty(emailuser) && !String.IsNullOrEmpty(phoneuser) &&
                !String.IsNullOrEmpty(addressuser))
            {
                ViewBag.Error = "Vui lòng nhập mật khẩu mới!";
                return this.AccountInformation();
            }
            else if (String.IsNullOrEmpty(renewpassuser)&&!String.IsNullOrEmpty(newpassuser) && !String.IsNullOrEmpty(oldpassuser) && !String.IsNullOrEmpty(fullnameuser) && !String.IsNullOrEmpty(emailuser) && !String.IsNullOrEmpty(phoneuser) &&
                !String.IsNullOrEmpty(addressuser))
            {
                ViewBag.Error = "Vui lòng nhập lại mật khẩu mới";
                return this.AccountInformation();
            }
            else if (!String.IsNullOrEmpty(renewpassuser) && !String.IsNullOrEmpty(newpassuser) && !String.IsNullOrEmpty(oldpassuser) && !String.IsNullOrEmpty(fullnameuser) && !String.IsNullOrEmpty(emailuser) && !String.IsNullOrEmpty(phoneuser) &&
                !String.IsNullOrEmpty(addressuser))
            {
                if (!String.Equals(MD5Hash(oldpassuser), User.PasswordUser))
                {
                    ViewBag.Error = "Mật khẩu không đúng!";
                    return this.AccountInformation();
                }
                else if (!String.Equals(newpassuser, renewpassuser))
                {
                    ViewBag.Error = "Mật khẩu mới và mật khẩu cũ không trùng khớp!";
                    return this.AccountInformation();
                }
                else
                {
                    User.FullName = fullnameuser;
                    User.Email = emailuser;
                    User.PhoneNumber = phoneuser;
                    User.AddressUser = addressuser;
                    User.PasswordUser = MD5Hash(newpassuser);
                    Session["user"] = User;
                    data.SubmitChanges();
                    ViewData["Info"] = "Cập nhật thành công!";
                    return this.AccountInformation();
                }
            }
            return this.AccountInformation();
        }
    }
}