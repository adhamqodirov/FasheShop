using FasheShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FasheShop.Control
{
    public static class Status
    {
        public static List<Order> cart = new List<Order>();
        public static User user = new User();
        public static User admin = new User();
        public static bool isRu = false;
    }
}