using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.ADONETEntities;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;
using System.Data;

namespace VotGESOrders.Web.Models
{

    public class User
    {
        // NOTE: Profile properties can be added here 
        // To enable profiles, edit the appropriate section of web.config file.

        // public string MyProfileProperty { get; set; }

        protected OrdersUser ordersUser;

        public void loadFromDB(string userName)
        {
            ordersUser = OrdersUser.loadFromCache(userName);
            FullName = ordersUser.FullName;
            AllowCreateOrder = ordersUser.AllowCreateOrder;
            AllowCreateCrashOrder = ordersUser.AllowCreateCrashOrder;
            AllowEditTree = ordersUser.AllowEditTree;
            AllowEditUsers = ordersUser.AllowEditUsers;
            AllowCreateCranTask = ordersUser.CanCreateCranTask;
            UserID = ordersUser.UserID;
        }


        public string FullName { get; set; }


        public bool AllowCreateOrder { get; set; }

        public bool AllowCreateCrashOrder { get; set; }

        public bool AllowEditTree { get; set; }

        public bool AllowEditUsers { get; set; }

        public bool AllowCreateCranTask { get; set; }

        public int UserID { get; set; }


        public override string ToString()
        {
            return ordersUser.ToString();
        }
    }


    public class OrdersUser
    {
        [Key]
        public int UserID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public List<String> Mails { get; set; }
        public bool SendAllMail { get; set; }
        public bool SendAgreeMail { get; set; }
        public bool SendCreateMail { get; set; }
        public bool SendAllAgreeMail { get; set; }
        public bool SendAllCreateMail { get; set; }
        public bool AllowCreateOrder { get; set; }
        public bool AllowCreateCrashOrder { get; set; }
        public bool AllowReviewOrder { get; set; }
        public bool AllowChangeOrder { get; set; }
        public bool AllowEditTree { get; set; }
        public bool AllowEditUsers { get; set; }
        public bool AllowEditOrders { get; set; }
        public bool AllowAgreeOrders { get; set; }
        public bool CanCreateCranTask { get; set; }
        public bool CanReviewCranTask { get; set; }
        public bool CanFinishCranTask { get; set; }
        public bool SendAllCreateCranTask { get; set; }
        public bool SendAllCranTask { get; set; }
        public string AddLogins { get; set; }
        public string AddFinishLogins { get; set; }

        public static OrdersUser getCurrentUser()
        {
            string login = "-";
            if (System.Web.HttpContext.Current != null)
            {
                login = System.Web.HttpContext.Current.User.Identity.Name.ToLower();
            }
            else
            {

                login = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            }


            return OrdersUser.loadFromCache(login);
        }

        protected static VotGESOrdersEntities context;
        protected static List<OrdersUser> allUsers;
        static OrdersUser()
        {
            init();
        }

        public OrdersUser()
        {
            AddLogins = "";
            AddFinishLogins = "";
            Name = "";
            UserID = -1;
        }

        public static void init()
        {
            allUsers = new List<OrdersUser>();
            context = new VotGESOrdersEntities();

            VotGESOrdersEntities ctx = new VotGESOrdersEntities();
            IQueryable<Users> dbUsers = from u in ctx.Users orderby u.fullName select u;
            foreach (Users dbUser in dbUsers)
            {
                allUsers.Add(getFromDB(dbUser));
            }

        }

        public static IQueryable<OrdersUser> getAllUsers()
        {
            return allUsers.AsQueryable();
        }

        public static OrdersUser loadFromCache(string userName)
        {
            try
            {
                //Logger.info(userName,Logger.LoggerSource.client);
                OrdersUser user = allUsers.AsQueryable().First(u => u.Name.ToLower() == userName.ToLower() || (u.AddLogins.ToLower() + ";").Contains(userName.ToLower() + ";"));

                return user;
            }
            catch (Exception e)
            {
                OrdersUser user = new OrdersUser();
                user.FullName = String.Format("{0}", userName);
                user.Name = userName;
                user.UserID = -1;

                //Logger.error(String.Format("Ошибка при получении краткой информации о пользователе из БД: {0}", userName), Logger.LoggerSource.server);
                return user;
            }
        }

        public static OrdersUser loadFromCache(int userID)
        {
            try
            {
                OrdersUser user = allUsers.AsQueryable().First(u => u.UserID == userID);
                return user;
            }
            catch (Exception e)
            {
                OrdersUser user = new OrdersUser();
                user.FullName = String.Format("{0}", userID);
                user.Name = userID.ToString();
                user.UserID = -1;
                //Logger.error(String.Format("Ошибка при получении краткой информации о пользователе из БД: {0}, {1}", userID, e), Logger.LoggerSource.server);
                return user;
            }
        }

        public static OrdersUser getFromDB(Users userDB)
        {
            try
            {
                OrdersUser user = new OrdersUser();
                user.UserID = userDB.userID;
                user.Name = userDB.name;
                user.FullName = userDB.fullName;
                user.Mail = userDB.mail;
                user.SendAgreeMail = userDB.sendAgreeMail;
                user.SendAllMail = userDB.sendAllMail;
                user.SendCreateMail = userDB.sendCreateMail;
                user.SendAllAgreeMail = userDB.sendAllAgreeMail;
                user.SendAllCreateMail = userDB.sendAllCreateMail;
                user.AllowCreateOrder = userDB.allowCreateOrder;
                user.AllowCreateCrashOrder = userDB.allowCreateCrashOrder;
                user.AllowReviewOrder = userDB.allowReviewOrder;
                user.AllowChangeOrder = userDB.allowChangeOrder;
                user.AllowEditTree = userDB.allowEditTree;
                user.AllowEditUsers = userDB.allowEditUsers;
                user.AllowEditOrders = userDB.allowEditOrders;
                user.AllowAgreeOrders = userDB.allowAgreeOrders;
                user.CanCreateCranTask = userDB.canCreateCranTask;
                user.CanReviewCranTask = userDB.canReviewCranTask;
                user.CanFinishCranTask = userDB.canFinishCranTask;
                user.SendAllCreateCranTask = userDB.sendAllCreateCranTask;
                user.SendAllCranTask = userDB.sendAllCranTask;
                user.AddLogins = userDB.AddLogins;
                user.AddFinishLogins = userDB.AddFinishLogins;

                try
                {
                    user.Mails = user.Mail.Split(';').ToList();
                }
                catch
                {
                    user.Mails = new List<string>();
                    if (!String.IsNullOrEmpty(user.Mail))
                    {
                        user.Mails.Add(user.Mail);
                    }
                }
                return user;
            }
            catch (Exception e)
            {
                Logger.error(String.Format("Ошибка при получении краткой информации о пользователе: {0}", e), Logger.LoggerSource.server);
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, FullName: {1}",
                Name, FullName, AllowCreateOrder);
        }
    }
}