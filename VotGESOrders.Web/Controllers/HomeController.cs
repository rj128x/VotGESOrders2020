using VotGESOrders.Web.Models;
using System.Net.Mail;
using VotGESOrders.Web.ADONETEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VotGESOrders.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index() {
			ViewData["Message"] = "Добро пожаловать в ASP.NET MVC!";

			return View("VotGESOrders");
		}
				
		public ActionResult ProcessExpiredOrders() {
			OrdersContext context= new OrdersContext();
			IQueryable<Order> expiredOrders=context.OrdersActiveExpired;
			foreach (Order order in expiredOrders) {
				MailContext.sendMail(String.Format("Заявка №{0}. Заявка просрочена [{1}]",order.OrderYearNumber.ToString("#.##", OrderInfo.Current.NFI), order.FullOrderObjectInfo), 
					order,false, true);
				Logging.Logger.info(String.Format("Отправка письма о просроченной заявке №{0} - {1}", order.OrderNumber.ToString("#.##", OrderInfo.Current.NFI), order.UserCreateOrder.Mail), Logging.Logger.LoggerSource.server);
			}
			return View();
		}

        public ActionResult ProcessNotClosedOrders()
        {
            OrdersContext context = new OrdersContext();
            IQueryable<Order> orders = context.OrdersNotClosed;
            IQueryable users = OrdersUser.getAllUsers();
            List<string> mailToList = new List<string>();
            foreach (OrdersUser user in users)
            {
                if (user.SendAllMail||user.SendAllCreateMail||user.Name.Contains("SVOD"))
                {
                    if (user.Mails.Count > 0)
                    {
                        foreach (string mail in user.Mails)
                        {
                            if (!String.IsNullOrEmpty(mail))
                            {
                                mailToList.Add(mail);
                            }
                        }
                    }
                }
            }
            //mailToList.Add("alekseevvv@rushydro.ru");
            MailContext.sendOrdersListTable("Местные заявки", orders.ToList(), mailToList);
            return View();
        }

        public ActionResult TestMail() {
			System.Net.Mail.MailMessage mess =	new System.Net.Mail.MailMessage();

			mess.From = new MailAddress("ChekunovaMV@votges.rushydro.ru");
			mess.Subject = "test"; mess.Body = "test message";
			mess.To.Add("ChekunovaMV@votges.rushydro.ru");
			mess.To.Add("rj128x@gmail.com");
			

			mess.SubjectEncoding = System.Text.Encoding.UTF8;
			mess.BodyEncoding = System.Text.Encoding.UTF8;
			mess.IsBodyHtml = true;
			System.Net.Mail.SmtpClient client =	new System.Net.Mail.SmtpClient("mx-votges-121.corp.gidroogk.com", 25);
			client.Credentials = new System.Net.NetworkCredential("ChekunovaMV", "320204", "CORP");			
						
			// Отправляем письмо
			client.Send(mess);
			return View();
		}

		public ActionResult ProcessAllExpiredOrders() {
			VotGESOrdersEntities context=new VotGESOrdersEntities();
			IQueryable<Orders> expiredOrders=from Orders o in context.Orders select o;
			foreach (Orders order in expiredOrders) {
				Order.writeExpired(order);
			}
			context.SaveChanges();
			return View();
		}

		public ActionResult GetAllObjects() {
			VotGESOrdersEntities context = new VotGESOrdersEntities();
			IQueryable<OrderObjects> objects = from OrderObjects o in context.OrderObjects where o.objectID==0 select o;
			List<string> data = new List<string>();
			fillData(context, data, objects.ToArray()[0], "");
			String str = String.Join("<br>", data);
			Logging.Logger.info(str, Logging.Logger.LoggerSource.client);
			return View();

		}

		protected void fillData(VotGESOrdersEntities context,List<string> data,OrderObjects par,string level){
			IQueryable<OrderObjects> objects = from OrderObjects o in context.OrderObjects where o.parentID==par.objectID select o;
			data.Add(level+par.objectName);
			foreach (OrderObjects obj in objects){				
				fillData(context, data, obj, level + "=");
			}
		}

		public ActionResult About() {
			return View();
		}

		public ActionResult PrintCranTasks(int year1, int month1, int day1, int year2, int month2, int day2) {
			Logging.Logger.info("Печать списка заявок на кран",Logging.Logger.LoggerSource.server);
			DateTime Date1 = new DateTime(year1, month1, day1);
			DateTime Date2 = new DateTime(year2, month2, day2);
			CranFilter filter = new CranFilter();
			filter.DateStart = Date1;
			filter.DateEnd = Date2;
			CranFilter Result = CranTaskInfo.LoadCranTasks(filter);
			ViewResult view = View("PrintCranTasks", Result);
			return view;

		}
	}
}
