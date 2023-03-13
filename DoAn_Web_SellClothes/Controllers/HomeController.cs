using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_Web_SellClothes.Models;
namespace DoAn_Web_SellClothes.Controllers
{
    public class HomeController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
        [HttpPost]
        public ActionResult About(FormCollection collection, Feedback fb)
        {
            var namefeedback = collection["fullnamefeedback"];
            var emailfeedback = collection["emailfeedback"];
            var describefeedback = collection["textfeedback"];
            if (String.IsNullOrEmpty(namefeedback) || String.IsNullOrEmpty(emailfeedback) || String.IsNullOrEmpty(describefeedback))
            {
                ViewData["Error"] = "Vui lòng điền đầy đủ nội dung";
                return this.About();
            }
            else
            {
                fb.FullNameUserFeedback = namefeedback;
                fb.EmailUserFeedback = emailfeedback;
                fb.DescribeFeedback = describefeedback;
                data.Feedbacks.InsertOnSubmit(fb);
                data.SubmitChanges();
                return RedirectToAction("About");
            }
        }
    }
}