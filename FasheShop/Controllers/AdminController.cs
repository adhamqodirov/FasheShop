using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FasheShop.Models;
using System.IO;
using FasheShop.Control;
using System.Xml.Serialization;

namespace FasheShop.Controllers
{
    public class AdminController : Controller
    {
        DBContext db = new DBContext();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            ViewBag.dailyOrders = db.Orders.Count(x=>x.OrderDate.Value.Day>DateTime.Now.Day && x.OrderDate.Value.Day>DateTime.Now.Day-10);
            ViewBag.usersCount = db.Users.ToList().Count;
            ViewBag.allProducts = db.Products.ToList().Count;
            ViewBag.newProducts = db.News.Count(x => x.Date.Value.Day > DateTime.Now.Day && x.Date.Value.Day > DateTime.Now.Day - 10);
            
            return View();
        }

        public ActionResult logout()
        {
            Status.cart.Clear();
            Status.user = new Models.User();
            Status.isRu = false;
            Status.admin = new Models.User();
            return RedirectToAction("loginform", "home");
        }

        public ActionResult Export()
        {
          return View(db.Products.ToList());
        }

        [Serializable]
        public   class ExportProduct
        {
            public int ID { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public string Picture { get; set; }
            public string Description { get; set; }
            public int CategoryID { get; set; }
            public System.DateTime ReceivedTime { get; set; }
            public int Sale { get; set; }
            public decimal SalePrice { get; set; }
            public int FeaturedProduct { get; set; }
            public int SaleTimeProduct { get; set; }
            public System.DateTime ExpiredDate { get; set; }
            public string language { get; set; }
        }

        [HttpPost]
        public ActionResult Export(string id)
        {
            if (id == "1")
            {

              
            

                var formatter = new XmlSerializer(typeof(ExportProduct[]));
                var stream = new MemoryStream();
                ExportProduct[] products = new ExportProduct[db.Products.Count(x=>x.ID>0)];
                int i = 0;
                foreach (var item in db.Products)
                {
                    ExportProduct ex = new ExportProduct();
                    ex.ID = item.ID;

                    ex.Picture = item.Picture != null ? item.Picture : "";
                    ex.Price = item.Price != null ? (decimal)item.Price : 0;
                    ex.ProductName = item.ProductName != null ? item.ProductName : "";
                    ex.ReceivedTime = item.ReceivedTime != null ? (DateTime)item.ReceivedTime : DateTime.Now;
                    ex.Sale = item.Sale != null ? (int)item.Sale : 0;
                    ex.SalePrice = item.SalePrice != null ? (decimal)item.SalePrice : 0;
                    ex.SaleTimeProduct = item.SaleTimeProduct != null ? (int)item.SaleTimeProduct : 0;
                    ex.FeaturedProduct = item.FeaturedProduct != null ? (int)item.FeaturedProduct : 0;
                    ex.ExpiredDate = item.ExpiredDate != null ? (DateTime)item.ExpiredDate : DateTime.Now;
                    ex.Description = item.Description != null ? item.Description : "";
                    ex.CategoryID = item.CategoryID != null ? (int)item.CategoryID : 0;
                    ex.language = item.language != null ? item.language : "";
                    products[i] = ex;
                    i++;


                }


               
                    formatter.Serialize(stream, products);
               
                
                stream.Position = 0;
                ViewBag.message = "Products are exported";
                return File(stream, "application/xml", "allitemlist.xml");             
              
            }

            else
            {
                ViewBag.message = "Products are not exported";
                return View(db.Products.ToList());
            }
           

        }

        public ActionResult Import(HttpPostedFileBase file)
        {
            int count = 0;
            XmlSerializer formatter = new XmlSerializer(typeof(ExportProduct[]));
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/"), fileName);
                file.SaveAs(path);

                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    ExportProduct[] newpeople = (ExportProduct[])formatter.Deserialize(fs);
                    List<ExportProduct> differents = new List<ExportProduct>();
                    foreach (ExportProduct p in newpeople)
                    {
                        bool bor = false;
                        foreach (var item in db.Products)
                        {
                            if (p.ID == item.ID)
                            {
                                bor = true;                           
                                break;
                            }
                        }

                        if (bor)
                        {
                            bor = false;
                        }
                        else
                        {
                            differents.Add(p);
                        }
                    }
                   
                    foreach (var item in differents)
                    {
                        Product p = new Product();
                        p.ID = item.ID;
                        p.language = item.language;
                        p.Picture = item.Picture;
                        p.Price = item.Price;
                        p.ProductName = item.ProductName;
                        p.ReceivedTime = item.ReceivedTime;
                        p.Sale = item.Sale;
                        p.SalePrice = item.SalePrice;
                        p.SaleTimeProduct = item.SaleTimeProduct;
                        p.CategoryID = item.CategoryID;
                        p.Description = item.Description;
                        p.ExpiredDate = item.ExpiredDate;
                        p.FeaturedProduct = item.FeaturedProduct;

                        count++;

                        db.Products.Add(p);

                    }
                    db.SaveChanges();
                }
              
            }

            ViewBag.message = count+ " products are imported!";
            return View("Export",db.Products.ToList());
         
        }

        public ActionResult userOrders()
        {
            
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            ViewBag.name = Status.user.Firstname;
           
            ViewBag.products = db.Products;

            return View();
        }

        public ActionResult Messages()
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            ViewBag.messages = db.Contacts.ToList();
            ViewBag.states = db.States.ToList();
            return View();
        }

        public ActionResult Options()
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            return View(db.CustomizeSettings.ToList());
        }
      
        public ActionResult EditOptions (int id)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            CustomizeSetting selected = db.CustomizeSettings.Find(id);
            ViewBag.custom = selected.code;
            ViewData["code"] = selected.code;
            return View();
        }

        [HttpPost]

        public ActionResult EditOptions(FormCollection form, HttpPostedFileBase file)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            string str = (Request.Form["code"]);

            CustomizeSetting custom = new CustomizeSetting();
            custom = db.CustomizeSettings.FirstOrDefault(x=>x.code == str);

          



            if ((Request.Form["name"] != null && Request.Form["code"] != "" && Request.Form["Description"] != "" && Request.Form["FullDescription"] != "" && file != null))
            {

                string path = Path.Combine(Server.MapPath("~/Content/images/"), Path.GetFileName(file.FileName));
                file.SaveAs(path);

                path = Path.Combine(Server.MapPath("~/Content/images/"), Path.GetFileName(file.FileName));
                file.SaveAs(path);

                custom.path = "/Content/images/" + file.FileName;
                custom.name = Request.Form["name"];
                custom.newprice = Request.Form["newprice"];
                custom.fulldescription = Request.Form["FullDescription"];
                custom.description = Request.Form["Description"];





                db.Entry(custom);
                db.SaveChanges();
               

                return RedirectToAction("options","admin");
            }
            else
            {
              
                ViewBag.mess = "Fill the Form....";
                return View();
            }




        }

    }
}