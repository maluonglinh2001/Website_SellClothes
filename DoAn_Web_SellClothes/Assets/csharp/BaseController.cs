using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_Web_SellClothes.Assets.csharp
{
    public class BaseController : Controller
    {
       public void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if(type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if(type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
    }
}