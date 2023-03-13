using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_Web_SellClothes.Models;
using PagedList;
namespace DoAn_Web_SellClothes.Controllers
{
    public class ProductController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: Product
        private List<Product> LaySanPhamMoi(int count)
        {
            return data.Products.OrderByDescending(s => s.IdProduct).Take(count).ToList();
        }

        public ActionResult SanPhamMoi()
        {
            var sanphammoi = LaySanPhamMoi(4);
            return PartialView(sanphammoi);
        }
        public ActionResult SanPhamNam()
        {
            var sanphamnam = from spn in data.ProductTypes where spn.IdSex == 1 select spn;
            return PartialView(sanphamnam);
        }
        public ActionResult SanPhamNu()
        {
            var sanphamnu = from spnu in data.ProductTypes where spnu.IdSex == 0 select spnu;
            return PartialView(sanphamnu);
        }

        public ActionResult SPTheoLoaiNam(int? id,int?page)
        {          
            var sanphamloainam = (from spln in data.Products where spln.IdProductType == id select spln).OrderByDescending(p=>p.IdProduct);
            int pageSize = 9; // mỗi trang 9 sản phẩm
            int pageNum = (page ?? 1); // nếu page = null => pageNum = 1
            return View(sanphamloainam.ToPagedList(pageNum,pageSize));
        }
        public ActionResult SPTheoLoaiNu(int?id, int?page)
        {
            var sanphamloainu = (from spln in data.Products where spln.IdProductType == id select spln).OrderByDescending(p=>p.IdProduct);
            int pageSize = 9; // mỗi trang 9 sản phẩm
            int pageNum = (page ?? 1); // nếu page = null => pageNum = 1
            return View(sanphamloainu.ToPagedList(pageNum,pageSize));
        }
        [HttpGet]
        public ActionResult ProductDetails(int? id)
        {
            var sanPham = data.Products.FirstOrDefault(p => p.IdProduct == id);
            var maSize = data.ProductDetails.Where(p => p.IdProduct == id).Select(p => p.IdSizeProduct).ToList();
            var soLuongTon = data.ProductDetails.Where(p => p.IdProduct == id).Select(p => p.SoLuongTon).ToList();
            List<string> nameSize = new List<string>();
            SizeProduct size;
            foreach (var item in maSize)
            {
                size = data.SizeProducts.FirstOrDefault(p => p.IdSizeProduct == item);
                nameSize.Add(size.NameSizeProduct);
            }
            sanPham.idSize = maSize;
            sanPham.soluongton = soLuongTon;
            sanPham.sizeProduct = nameSize;

            var demsanpham = soLuongTon.Sum(p => p.Value);
            if (demsanpham <= 0)
            {
                sanPham.StatusProduct = 0;
            }
            else
            {
                sanPham.StatusProduct = 1;
            }
            return View(sanPham);
        }
        public ActionResult ProductPage(int? page)
        {
            string keyword = Request.QueryString["keyword"];
            Session["keysearch"] = keyword;
            int pageSize = 9; // mỗi trang 9 sản phẩm
            int pageNum = (page ?? 1); // nếu page = null => pageNum = 1
            var listProduct = from sp in data.Products.OrderByDescending(p => p.IdProduct) select sp;
            if (!String.IsNullOrEmpty(keyword))
            {
                listProduct = listProduct.Where(p => p.NameProduct.Contains(keyword)).Take(9);
            }
            return PartialView(listProduct.ToPagedList(pageNum,pageSize));
        }
    }
}