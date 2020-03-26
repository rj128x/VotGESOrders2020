using VotGESOrders.Web.Models;
using System.Net.Mail;
using VotGESOrders.Web.ADONETEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VotGESOrders.Web.Controllers {
	public class CranController : Controller {
		public ActionResult Index() {
			ViewData["Message"] = "Добро пожаловать в ASP.NET MVC!";

			return View("VotGESOrders");
		}

		public ActionResult ShowCranOrders() {
			return View("Cran");
		}

		


	}
}
