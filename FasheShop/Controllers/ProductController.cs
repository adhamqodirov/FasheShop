using FasheShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FasheShop.Controllers
{
    public class ProductController : Controller
    {
        DBContext db = new DBContext();
        // GET: Product
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }
        //Adding Product to DataBase
        public ActionResult Create()
        {
            ViewData["list"] = db.ProductCategories.ToList();
            return View();
        }

        [HttpPost]

        public ActionResult Create(FormCollection form, HttpPostedFileBase file)
        {


            Product product = new Product();



            if ((Request.Form["catid"] != null && Request.Form["name"] != "" && Request.Form["price"] != "" && Request.Form["price"] != "" && file != null))
            {

                string path = Path.Combine(Server.MapPath("~/Content/images/"), Path.GetFileName(file.FileName));
                file.SaveAs(path);

                path = Path.Combine(Server.MapPath("~/Content/images/"), Path.GetFileName(file.FileName));
                file.SaveAs(path);

                product.Picture = "/Content/images/" + file.FileName;
                product.ProductName = Request.Form["name"];
                string narx = (Request.Form["price"]);
                product.Price = decimal.Parse(narx);
                product.Description = Request.Form["summary"];
                product.CategoryID = int.Parse(Request.Form["catid"]);
                product.ReceivedTime = DateTime.Parse(Request.Form["arrivaldate"]);
               // product.SaleTimeProduct = int.Parse(Request.Form["saledate"]);
                product.ExpiredDate = DateTime.Parse(Request.Form["expireddate"]);
                product.language = Request.Form["enru"]!=null && Request.Form["enru"]!=""? Request.Form["enru"]:"";
              
                //  product.SalePrice = decimal.Parse(Request.Form["saleprice"]);

                if (!string.IsNullOrEmpty(form["saleprice"]))
                {
                    product.SalePrice = decimal.Parse(Request.Form["saleprice"]);
                }
                if (!string.IsNullOrEmpty(form["saledate"]))
                {
                    product.SaleTimeProduct = int.Parse(Request.Form["saledate"]);
                }

                bool checkRespA = false;
                if (!string.IsNullOrEmpty(form["statussale"]))
                {
                    string checkResp = form["statussale"];
                    checkRespA = Convert.ToBoolean(checkResp);
                }

                bool checkRespB = false;
                if (!string.IsNullOrEmpty(form["featured"]))
                 {
                    string checkResp = form["featured"];
                     checkRespB = Convert.ToBoolean(checkResp);
                 }


                product.FeaturedProduct = (checkRespB) ? 1 : 0;
                product.Sale = (checkRespA) ? 1 : 0;
                db.Products.Add(product);
                db.SaveChanges();
                ViewData["list"] = db.ProductCategories.ToList();
                ViewBag.mess = "New Product is added to DataBase";
                ViewBag.flag = "1";

                return View();
            }
            else
            {
                ViewData["list"] = db.ProductCategories.ToList();
                ViewBag.flag = "0";
                ViewBag.mess = "Fill the Form....";
                return View();
            }




        }



        // Delete Product from DataBase
        public ActionResult Delete(int id)
        {
            Product selected = db.Products.Find(id);
            return View(selected);
        }

     
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (id== 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.ProductCategories, "id", "Name", product.CategoryID);
            return View(product);
        }
        [HttpPost]
   
        public ActionResult EditProduct([Bind(Include = "ID,ProductName,Price,Picture,Description,CategoryID,ReceivedTime,Sale,SalePrice,FeaturedProduct,SaleTimeProduct,ExpiredDate")] Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/products/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                    path = Path.Combine(Server.MapPath("~/Content/products/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);


                    product.Picture = "/Content/products/" + file.FileName;
                }
               
              
             //   product.Picture =  file.FileName;

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            ViewBag.CategoryId = new SelectList(db.ProductCategories, "id", "Name", product.CategoryID);
            return View(product);
        }
    }
}