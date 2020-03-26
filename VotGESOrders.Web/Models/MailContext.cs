using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VotGESOrders.Web.Logging;
using System.Net.Mail;
using System.IO;

namespace VotGESOrders.Web.Models {
	public class MailContext {
		protected static string smtpServer;
		protected static string smtpUser;
		protected static string smtpPassword;
		protected static string smtpDomain;
		protected static int smtpPort;
		protected static string smtpFrom;

		static MailContext() {
			/*smtpServer = System.Configuration.ConfigurationManager.AppSettings["smtpServer"];
			smtpUser = System.Configuration.ConfigurationManager.AppSettings["smtpUser"];
			smtpPassword = System.Configuration.ConfigurationManager.AppSettings["smtpPassword"];
			smtpDomain = System.Configuration.ConfigurationManager.AppSettings["smtpDomain"];
			smtpFrom = System.Configuration.ConfigurationManager.AppSettings["smtpFrom"];
			Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["smtpPort"], out smtpPort);*/


		}
		public static void sendMail(string header, Order order, bool isNewOrder, bool expired, Order prevOrder = null) {
			/*if (HttpContext.Current.Request.Url.Port == 8072 || HttpContext.Current.Request.Url.Port == 8090)
				return;*/
			try {
				IQueryable users = OrdersUser.getAllUsers();
				List<string> mailToList = new List<string>();

                foreach (OrdersUser user in users) {
                    if (
                        /*user.SendAllAgreeMail && order.AgreeUsers.Contains(user) && !mailToList.Contains(user.Mail) && !onlyAuthor ||
						user.SendAllMail && !mailToList.Contains(user.Mail) ||
						user.SendCreateMail && order.UserCreateOrderID == user.UserID && !mailToList.Contains(user.Mail) ||
						onlyAuthor && order.UserCreateOrderID == user.UserID && !mailToList.Contains(user.Mail) ||
						isNewOrder && (user.SendAllCreateMail || user.SendAgreeMail && order.AgreeUsers.Contains(user)) && !mailToList.Contains(user.Mail) && !onlyAuthor*/
                        user.SendAllMail && !mailToList.Contains(user.Mail) ||
                        isNewOrder && (user.SendAllCreateMail || order.UserCreateOrderID == user.UserID || user.SendAgreeMail && order.AgreeUsers.Contains(user)) && !mailToList.Contains(user.Mail) ||
                        expired && (order.UserCreateOrderID == user.UserID || user.SendAllCreateMail) && !mailToList.Contains(user.Mail) ||
                        order.FullOrderObjectInfo.Contains("Кран") && order.FullOrderObjectInfo.IndexOf("Кран")<3 && user.Name.ToUpper()=="CRAN" && !mailToList.Contains(user.Mail)&&
                            (isNewOrder||order.OrderState == OrderStateEnum.created||order.OrderState==OrderStateEnum.opened||order.OrderState==OrderStateEnum.completed)
                                                
						) {
						if (user.Mails.Count > 0) {
							foreach (string mail in user.Mails) {
								if (!String.IsNullOrEmpty(mail)) {
									mailToList.Add(mail);
								}
							}
						}
					}
				}

				string message = OrderView.getOrderHTML(order);
				if (prevOrder != null) {
					message += "<hr/>" + OrderView.getOrderHTML(prevOrder, false);
				}
				//message += String.Format("<h3><a href='{0}'>Перейти к списку заявок</a></h3>", String.Format("http://{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port));

				if (mailToList.Count > 0) {
					//SendMailLocal("mx-votges-021.corp.gidroogk.com", 25, "", "", "", "SR-VOTGES-INT@votges.rushydro.ru", mailToList, header, message, true);
					SendMailLocal(smtpServer, smtpPort, smtpUser, smtpPassword, smtpDomain, smtpFrom, mailToList, header, message, true);
				}
			}
			catch (Exception e) {
				Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()), Logger.LoggerSource.server);
			}
		}

		public static void sendCranTask(string header, CranTaskInfo task) {
			try {
				IQueryable users = OrdersUser.getAllUsers();
				List<string> mailToList = new List<string>();


				foreach (OrdersUser user in users) {
					if (
						( user.SendAllCranTask ) ||
						( task.SelAuthor.ToLower() == user.Name.ToLower() )||
						( task.TaskState==CranTaskState.created && user.SendAllCreateCranTask  )
						){						
						if (user.Mails.Count > 0) {
							foreach (string mail in user.Mails) {
								if (!String.IsNullOrEmpty(mail) && !mailToList.Contains(mail.ToLower())) {
									mailToList.Add(mail.ToLower());
								}
							}
						}
					}
				}
        try {
          mailToList.Add("chekunovamv@rushydro.ru");
        } catch { }

				Attachment attach = null;

				/*try {
					MemoryStream stream = new MemoryStream();
					StreamWriter writer = new StreamWriter(stream);
					writer.Write(CranTaskInfo.getTaskPrintHTML(task));
					writer.Flush();
					stream.Position = 0;

					attach = new Attachment(stream, String.Format("Заявка{0}.xls", task.Number), "application/vnd.ms-excel");
				}
				catch (Exception e) {
					Logger.info(e.ToString(), Logger.LoggerSource.server);
				}*/


				string message = CranTaskInfo.getTaskHTML(task);

				//message += String.Format("<h3><a href='{0}'>Перейти к списку заявок</a></h3>", String.Format("http://{0}:{1}/#/CransPage", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port));

				if (mailToList.Count > 0) {
					SendMailLocal(smtpServer, smtpPort, smtpUser, smtpPassword, smtpDomain, smtpFrom, mailToList, header, message, true, attach);
				}
			}
			catch (Exception e) {
				Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()), Logger.LoggerSource.server);
			}
		}




		public static void sendOrdersList(string header, List<Order> orders) {
			try {
				IQueryable users = OrdersUser.getAllUsers();
				List<string> mailToList = new List<string>();

				OrdersUser CurrentUser = OrdersUser.getCurrentUser();
				foreach (string mail in CurrentUser.Mails) {
					if (!String.IsNullOrEmpty(mail)) {
						mailToList.Add(mail);
					}
				}

				bool isFirst = true;
				if (mailToList.Count > 0) {
					string message = "";
					foreach (Order order in orders) {
						message += OrderView.getOrderHTML(order, isFirst) + "<hr/>";
						isFirst = false;
					}
					//SendMailLocal("mx-votges-021.corp.gidroogk.com", 25, "", "", "", "SR-VOTGES-INT@votges.rushydro.ru", mailToList, header, message,true);
					SendMailLocal(smtpServer, smtpPort, smtpUser, smtpPassword, smtpDomain, smtpFrom, mailToList, header, message, true);
				}
			}
			catch (Exception e) {
				Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()), Logger.LoggerSource.server);
			}
		}

        public static void sendOrdersListShort(string header, List<Order> orders, List<string> mailToList)
        {
            try
            {            
                bool isFirst = true;
                if (mailToList.Count > 0)
                {
                    string message = "";
                    foreach (Order order in orders)
                    {
                        message += OrderView.getOrderHTMLShort(order, isFirst) + "<hr/>";
                        isFirst = false;
                    }
                    //SendMailLocal("mx-votges-021.corp.gidroogk.com", 25, "", "", "", "SR-VOTGES-INT@votges.rushydro.ru", mailToList, header, message,true);
                    SendMailLocal(smtpServer, smtpPort, smtpUser, smtpPassword, smtpDomain, smtpFrom, mailToList, header, message, true);
                }
            }
            catch (Exception e)
            {
                Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()), Logger.LoggerSource.server);
            }
        }

        public static void sendOrdersListTable(string header, List<Order> orders, List<string> mailToList)
        {
            try
            {
                bool isFirst = true;
                if (mailToList.Count > 0)
                {
                    string trForm = "<html><body><table border='1'><tr><th>№</th><th>Оборудование</th><th>Текст заявки</th><th>План</th><th>Автор</th><th>Факт</th><th>Разр.ввод</th></tr>";
                    string style = "<Style>td{text-align:center;} td.comments{text-align:left;} td, th {border-width: 1px;	border-style: solid;	border-color: #BBBBFF;	padding-left: 3px;	padding-right: 3px;}</Style>";
                    string message = style+trForm;
                    foreach (Order order in orders)
                    {
                        message += OrderView.getOrderHTMLTR(order, isFirst);
                        isFirst = false;
                    }
                    //SendMailLocal("mx-votges-021.corp.gidroogk.com", 25, "", "", "", "SR-VOTGES-INT@votges.rushydro.ru", mailToList, header, message,true);
                    SendMailLocal(smtpServer, smtpPort, smtpUser, smtpPassword, smtpDomain, smtpFrom, mailToList, header, message, true);
                    message += "</table></body></html>";
                }
            }
            catch (Exception e)
            {
                Logger.error(String.Format("Ошибка при отправке почты: {0}", e.ToString()), Logger.LoggerSource.server);
            }
        }


        private static bool SendMailLocal(string smtp_server, int port, string mail_user, string mail_password, string domain, string mail_from, List<string> mailToList, string subject, string message, bool is_html, Attachment attach = null) {

			System.Net.Mail.MailMessage mess = new System.Net.Mail.MailMessage();

			mess.From = new MailAddress(mail_from);
			mess.Subject = subject; mess.Body = message;
			foreach (string mail in mailToList) {
				mess.To.Add(mail);
			}

			mess.SubjectEncoding = System.Text.Encoding.UTF8;
			mess.BodyEncoding = System.Text.Encoding.UTF8;
			mess.IsBodyHtml = is_html;

			if (attach != null) {
				mess.Attachments.Add(attach);
			}

			System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtp_server, port);
			client.EnableSsl = true;
			if (string.IsNullOrEmpty(mail_user)) {
				client.UseDefaultCredentials = true;
			}
			else {
				client.Credentials = new System.Net.NetworkCredential(mail_user, mail_password, domain);
			}
			// Отправляем письмо
			client.Send(mess);
			return true;
		}





	}
}