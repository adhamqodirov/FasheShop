using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FasheShop.Models;
using FasheShop.Control;
using CaptchaMvc.HtmlHelpers;

using System.Net;
using System.Net.Mail;


namespace FasheShop.Controllers
{
    public class HomeController : Controller
    {
        DBContext db = new DBContext();
        // GET: Home


        public ActionResult LoginForm()
        {
            
            return View();
        }


        [HttpPost]
        public ActionResult LoginForm(string your_name, string your_pass, string remember_me)
        {

            if (your_name == "" && your_pass == "")
            {
                ViewBag.error = "Please enter login details!!!";
                return View();
            }
            if (your_name=="" && your_pass != "")
            {
                ViewBag.error = "Username field is empty!!!";
                return View();
            }
            if (your_pass == "" && your_name != "")
            {
                ViewBag.error = "Password field is empty!!!";
                return View();
            }

            if (your_name != null && your_pass != null && your_name!="" && your_pass!="")
            {
                
                User user = new Models.User();
                user = db.Users.FirstOrDefault(x => x.Username == your_name && x.Password == your_pass);
                if(user == null)
                {
                    ViewBag.error = "You entered incorrect details!!!";
                    return View();
                }
                Status.user = user;
                return RedirectToAction("index", "home");
            }
            else
            {
                ViewBag.error = "You entered incorrect details!!!";
                return View();
            }


        }
        public ActionResult RegForm()
        {
            return View();
        }

   
        public ActionResult ru()
        {
            Status.isRu = true;
            List<News> news = new List<News>();
            List<Product> products = new List<Product>();

          
            news = db.News.Where(x => x.language == "RU").ToList();
            products = db.Products.Where(x => x.language == "RU").ToList();
          
            return RedirectToAction("index");
        }
        public ActionResult en()
        {
            Status.isRu = false;
            List<News> news = new List<News>();
            List<Product> products = new List<Product>();


            news = db.News.Where(x => x.language == "EN").ToList();
            products = db.Products.Where(x => x.language == "EN").ToList();

            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult RegForm(User us, string name, string email, string pass, string re_pass)
        {

            if (name == "" || name == null && email == null|| email==""&& pass=="" || pass==null && re_pass == "" || re_pass == null)
            {
                ViewBag.error = "Fill in empty fields!!!";
                return View();
            }

            if (name == null || name == "")
            {
                ViewBag.error = "Firstname field is empty!!!";
                return View();
            }

            if (email == null || email == "")
            {
                ViewBag.error = "Email field is empty!!!";
                return View();
            }


            if (pass == null || pass == "")
            {
                ViewBag.error = "Password field is empty!!!";
                return View();
            }

            

            if (this.IsCaptchaValid("The answer is not correct!"))
            {


                if (name != null && email != null && pass != "" && re_pass != "")
                {
                    if (pass != re_pass)
                    {
                        ViewBag.error = "Password doesn't match!!!";
                        return View();
                    }
                    else
                    {
                        var result = db.Users.FirstOrDefault(x=>x.Username==email);
                        if (result != null)
                        {
                            ViewBag.error = "This Username is taken!!!";
                            return View();
                        }

                        User user = new Models.User();
                        user.Firstname = name;
                        user.Username = email;
                        user.Password = pass;

                        db.Users.Add(user);
                        db.SaveChanges();
                        db.UserRoles.Add(new UserRole() { UserID = user.ID, RoleID = 2 });
                       
                        Status.user = user;

                        var fromAddress = new MailAddress("dnetseniordeveloper@gmail.com", "do not Reply");
                        var toAddress = new MailAddress(email, name);
                        const string fromPassword = "adham7529752";
                        const string subject = "[FasheShop] Registration Confirmation";
                        const string body = "We are excited to tell you that your account is" +
                    " successfully created!  ";

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                        };
                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = subject,
                            Body = body
                        })
                        {
                            smtp.Send(message);
                        }


                        return RedirectToAction("index", "home");
                    }

                }
                else
                {
                    ViewBag.error = "Fill the form!!!";
                    return View();
                }
            }

            ViewBag.error = "Please verify you are not Robot!";
            return View();




        }
        [HttpPost]
        public ActionResult addToCart(int productId)
        {
            Status.cart.Add(new Order() { OrderDate = DateTime.Now, ProductID = productId, Quantity = 1, TotalPrice = db.Products.Find(productId).Price });



            return Index();
        }

        [HttpPost]
        public ActionResult updatecart(FormCollection collection, List<Product> cart)
        {
            string value = collection["num-product1"];
            string id = collection["iddd"];
            string []quantity = value.Split(',');
            string[] ides = id.Split(',');
            
            for (int i = 0; i < ides.Length; i++)
            {
                foreach (var item in Status.cart)
                {
                    if (Convert.ToInt32(ides[i]) == item.ProductID)
                    {
                        int quan = Convert.ToInt32(quantity[i]);
                        item.Quantity = quan;
                        item.TotalPrice = item.Quantity * item.TotalPrice;
                    }
                }
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.products = db.Products;
            return View("cart");
        }
        public ActionResult apply()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.products = db.Products;
            ViewBag.message = "Your items added to your order list! See your personal account";
            return View("cart");
        }

        public ActionResult Index()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            if (!Status.isRu)
            {
                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.home = "sale-noti";
                ViewBag.products = db.Products.Where(x => x.language == "EN").ToList();

                ViewBag.news = db.News.Where(x => x.language == "EN").ToList().OrderByDescending(x => x.Date).Take(3).ToList();


                ViewBag.saleproducts = db.Products.Where(x => x.language == "EN").ToList().OrderBy(i => i.Sale > 0 && i.ReceivedTime < i.ReceivedTime).Take(5);

                return View(db.Products.Where(x => x.language == "EN").ToList().OrderByDescending(x => x.ReceivedTime).Take(10).ToList());
            }
            else
            {
                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.home = "sale-noti";
                ViewBag.products = db.Products.Where(x => x.language == "RU").ToList();

                ViewBag.news = db.News.Where(x => x.language == "RU").ToList().OrderByDescending(x => x.Date).Take(3).ToList();


                ViewBag.saleproducts = db.Products.Where(x => x.language == "RU").ToList().OrderBy(i => i.Sale > 0 && i.ReceivedTime < i.ReceivedTime).Take(5);

                return View(db.Products.Where(x => x.language == "RU").ToList().OrderByDescending(x => x.ReceivedTime).Take(10).ToList());
            }
           
        }


        public ActionResult clear()
        {
            Status.cart.Clear();
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
           

            getOptions();
            ViewBag.home = "sale-noti";


            ViewBag.news = db.News.ToList();


            ViewBag.saleproducts = db.Products.OrderBy(i => i.Sale > 0 && i.ReceivedTime < i.ReceivedTime).Take(5);

            return View("Index",db.Products.OrderByDescending(x => x.ReceivedTime).Take(10).ToList());
        }

        public ActionResult About()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);

            getOptions();
            ViewBag.about = "sale-noti";
           
            return View();
        }

        public ActionResult Login()
        {

            if (Status.user.ID != 0)
            {

                if(Status.user.Username == "admin")
                {
                    return RedirectToAction("index", "admin");
                }
                else return RedirectToAction("userorders", "admin");
            }

            else
            {
                ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
                return View();
            }
          
        }

        [HttpPost]
        public ActionResult Login(string loginUsername, string loginPassword)
        {
            if(loginUsername=="admin" && loginPassword == "12345")
            {
                User user = new Models.User();
                user = db.Users.FirstOrDefault(x => x.Username == loginUsername && x.Password == loginPassword);
                
                Status.admin = user;
               return RedirectToAction("index", "admin");
            }
            else
            {
                ViewBag.error = "Password or Login is not correct!!!";
                return View();
            }

           
        }
        public ActionResult Shop()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.shop = "sale-noti";
            ViewBag.categories = db.ProductTypes.ToList();
            ViewBag.summary = db.Products.Count(x => x.ID > 0);
            ViewBag.loadless = "0";
            if (!Status.isRu)
            {
                ViewBag.products = db.Products.Where(x => x.language == "EN").ToList();
                decimal max = int.MinValue, min = int.MaxValue;

                foreach (var item in db.Products)
                {
                    if (item.Price > max)
                    {
                        max = (decimal)item.Price;
                    }
                    if (item.Price < min)
                    {
                        min = (decimal)item.Price;
                    }
                }

                ViewBag.min = Convert.ToInt32(min);
                ViewBag.max = Convert.ToInt32(max);


                return View(db.Products.Where(x => x.language == "EN").ToList().Take(12));
            }
            else
            {
                ViewBag.products = db.Products.Where(x=>x.language=="RU").ToList();
                decimal max = int.MinValue, min = int.MaxValue;

                foreach (var item in db.Products)
                {
                    if (item.Price > max)
                    {
                        max = (decimal)item.Price;
                    }
                    if (item.Price < min)
                    {
                        min = (decimal)item.Price;
                    }
                }

                ViewBag.min = Convert.ToInt32(min);
                ViewBag.max = Convert.ToInt32(max);


                return View(db.Products.Where(x => x.language == "RU").ToList().Take(12));
            }
           
        }
        public ActionResult getall()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.shop = "sale-noti";
            ViewBag.categories = db.ProductTypes.ToList();
            ViewBag.summary = db.Products.Count(x => x.ID > 0);

            decimal max = int.MinValue, min = int.MaxValue;

            foreach (var item in db.Products)
            {
                if (item.Price > max)
                {
                    max = (decimal)item.Price;
                }
                if (item.Price < min)
                {
                    min = (decimal)item.Price;
                }
            }
            if (!Status.isRu)
            {
                ViewBag.min = Convert.ToInt32(min);
                ViewBag.max = Convert.ToInt32(max);
                ViewBag.loadless = "1";
                ViewBag.products = db.Products.Where(x => x.language == "EN").ToList();
                return View("shop", db.Products.Where(x => x.language == "EN").ToList());
            }

            else
            {
                ViewBag.min = Convert.ToInt32(min);
                ViewBag.max = Convert.ToInt32(max);
                ViewBag.loadless = "1";
                ViewBag.products = db.Products.Where(x => x.language == "RU").ToList();
                return View("shop", db.Products.Where(x => x.language == "RU").ToList());
            }

           
        }


        [HttpPost]
        public ActionResult filter(string sorting)
        {
            List<Product> result = new List<Product>();

            if (!Status.isRu)
            {

                if (sorting == "Price: low to high")
                {

                    result = db.Products.Where(x => x.language == "EN").ToList().OrderBy(x => x.Price).ToList();
                }
                else if (sorting == "Price: high to low")
                {
                    result = db.Products.Where(x => x.language == "EN").ToList().OrderByDescending(x => x.Price).ToList();
                }
                else if (sorting == "Last added")
                {
                    result = db.Products.Where(x => x.language == "EN").ToList().OrderByDescending(x => x.ReceivedTime).ToList();
                }
                else if (sorting == "Default Sorting")
                {
                    result = db.Products.Where(x => x.language == "EN").ToList().ToList().ToList();
                }



                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.shop = "sale-noti";
                ViewBag.categories = db.ProductTypes.ToList();
                ViewBag.summary = db.Products.Where(x => x.language == "EN").ToList().Count(x => x.ID > 0);
                ViewBag.loadless = "0";


                return View("shop", result.Where(x => x.language == "EN").ToList().Take(12));
            }

            else
            {
                if (sorting == "Price: low to high")
                {

                    result = db.Products.Where(x => x.language == "RU").ToList().OrderBy(x => x.Price).ToList();
                }
                else if (sorting == "Price: high to low")
                {
                    result = db.Products.Where(x => x.language == "RU").ToList().OrderByDescending(x => x.Price).ToList();
                }
                else if (sorting == "Last added")
                {
                    result = db.Products.Where(x => x.language == "RU").ToList().OrderByDescending(x => x.ReceivedTime).ToList();
                }
                else if (sorting == "Default Sorting")
                {
                    result = db.Products.Where(x => x.language == "RU").ToList().ToList().ToList();
                }



                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.shop = "sale-noti";
                ViewBag.categories = db.ProductTypes.ToList();
                ViewBag.summary = db.Products.Where(x => x.language == "RU").ToList().Count(x => x.ID > 0);
                ViewBag.loadless = "0";


                return View("shop", result.Where(x => x.language == "RU").ToList().Take(12));
            }

        }
        public ActionResult Cart()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.products = db.Products;

            return View();
        }

        public void getOptions()
        {
            ViewBag.a1 = db.CustomizeSettings.FirstOrDefault(x => x.code == "a1");
            ViewBag.a2 = db.CustomizeSettings.FirstOrDefault(x => x.code == "a2 ");
            ViewBag.h1 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h1");
            ViewBag.h2 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h2");
            ViewBag.h3 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h3");
            ViewBag.h4 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h4");
            ViewBag.h5 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h5");
            ViewBag.h6 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h6");
            ViewBag.h7 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h7");
            ViewBag.h8 = db.CustomizeSettings.FirstOrDefault(x => x.code == "h8");
            ViewBag.f1 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f1");
            ViewBag.f2 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f2");
            ViewBag.f3 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f3");
            ViewBag.f4 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f4");
            ViewBag.f5 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f5");
            ViewBag.f6 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f6");
            ViewBag.f7 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f7");
            ViewBag.f8 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f8");

            ViewBag.f11 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f11");
            ViewBag.f12 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f12");
            ViewBag.f13 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f13");
            ViewBag.f14 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f14");
            ViewBag.f15 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f15");
            ViewBag.f16 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f16");
            ViewBag.f10 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f10");
            ViewBag.f9 = db.CustomizeSettings.FirstOrDefault(x => x.code == "f9");

            ViewBag.fc = db.CustomizeSettings.FirstOrDefault(x => x.code == "fc");
            ViewBag.fl = db.CustomizeSettings.FirstOrDefault(x => x.code == "fl");
            ViewBag.fh = db.CustomizeSettings.FirstOrDefault(x => x.code == "fh");
            ViewBag.pageimages = db.CustomizeSettings.FirstOrDefault(x => x.code == "pageimages");
            ViewBag.logo = db.CustomizeSettings.FirstOrDefault(x => x.code == "logo");
        }
        public ActionResult Blog()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            if (!Status.isRu)
            {
                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.loadless = "0";
                ViewBag.blog = "sale-noti";
                return View(db.News.Where(x => x.language == "EN").ToList().ToList().Take(4).OrderByDescending(x => x.Date));
            }
            else
            {
                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.loadless = "0";
                ViewBag.blog = "sale-noti";
                return View(db.News.Where(x => x.language == "RU").ToList().ToList().Take(4).OrderByDescending(x => x.Date));
            }

            
        }

        public ActionResult allblog()
        {
            if (Status.user == null)
            {
                return RedirectToAction("loginform", "home");
            }
            if (!Status.isRu)
            {
                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.loadless = "1";
                ViewBag.blog = "sale-noti";
                return View("blog", db.News.Where(x => x.language == "EN").ToList().ToList().OrderByDescending(x => x.Date));
            }
            else
            {
                ViewBag.cart = Status.cart;
                ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
                getOptions();
                ViewBag.loadless = "1";
                ViewBag.blog = "sale-noti";
                return View("blog", db.News.Where(x => x.language == "RU").ToList().ToList().OrderByDescending(x => x.Date));
            }
           
        }

        [HttpGet]
        public ActionResult singleblog(int id)
        {
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.singleblog = "sale-noti";
            News news = db.News.Find(id);
            ViewBag.news = news;
            return View();
        }
        public ActionResult Contact()
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.contact = "sale-noti";
            ViewBag.c1 = db.CustomizeSettings.FirstOrDefault(x => x.code == "c1");
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string name, string phonenumber, string email, string message)
        {
            if (Status.user == null)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);

            getOptions();
            Contact newQuery = new Models.Contact();
            newQuery.Firstname = name;
            newQuery.Phone = phonenumber;
            newQuery.Username = email;
            newQuery.Message = message;
            newQuery.StateID = 1;
            newQuery.ArrivedTime = DateTime.Now;

            db.Contacts.Add(newQuery);
            db.SaveChanges();
            ViewBag.mess = "You have successfully sent your message, we will get back to you as soon as possible";


            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            ViewBag.contact = "sale-noti";
            ViewBag.c1 = db.CustomizeSettings.FirstOrDefault(x => x.code == "c1");
       

            return View("Contact");
        }

        [HttpPost]
        public ActionResult singleblog(string name, string phonenumber, string email, string message)
        {
            if (Status.user.ID == 0)
            {
                return RedirectToAction("loginform", "home");
            }
            ViewBag.cart = Status.cart;
            ViewBag.total = Status.cart.Sum(x => x.TotalPrice);
            getOptions();
            Contact newQuery = new Models.Contact();
            newQuery.Firstname = name;
            newQuery.Phone = phonenumber;
            newQuery.Username = email;
            newQuery.Message = message;
            newQuery.StateID = 1;
            newQuery.ArrivedTime = DateTime.Now;

            db.Contacts.Add(newQuery);
            db.SaveChanges();
            ViewBag.success = "You have successfully sent your message, we will get back to you as soon as possible";

            return View();
        }



    }
}