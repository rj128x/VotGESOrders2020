using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using VotGESOrders.Web.ADONETEntities;
using VotGESOrders.Web.Logging;

namespace VotGESOrders.Web.Models
{

  public class CranFilter
  {
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public List<CranTaskInfo> Data { get; set; }
    public List<String> Managers { get; set; }
    public List<String> StropUsers { get; set; }
    public List<String> CranUsers { get; set; }
  }

  public class ReturnMessage
  {
    public string Message { get; set; }
    public bool Result { get; set; }

    public ReturnMessage() { }
    public ReturnMessage(bool result, string message) {
      Message = message;
      Result = result;
      Logger.info(String.Format("Возврат: {0} (1)", result, message), Logger.LoggerSource.server);
    }

  }

  public enum CranTaskState { created, reviewed, canceled, opened, finished, }
  public enum CranTaskAction {none, create,change, review, open, finish, cancel,returnCancel }

  public class CranTaskInfo
  {
    public static Dictionary<int, int> MaxYearPrevNumbers;
    public static string PathFiles;
    public static DateTime LastUpdate;
    public int CranNumber { get; set; }
    public string CranName { get; set; }
    public int Number { get; set; }
    public int YearNumber { get; set; }
    public DateTime NeedStartDate { get; set; }
    public DateTime NeedEndDate { get; set; }
    public string Comment { get; set; }
    public string Author { get; set; }
    public string SelAuthor { get; set; }
    public string AuthorText { get; set; }
    public string AuthorAllow { get; set; }
    public string AuthorCancel { get; set; }
    public string AuthorOpen { get; set; }
    public string AuthorFinish { get; set; }
    public string Manager { get; set; }
    public string StropUser { get; set; }
    public string CranUser { get; set; }
    public string ManagerShort { get; set; }
    public string CranUserShort { get; set; }
    public string StropUserShort { get; set; }
    public string AgreeComments { get; set; }
    public string ReviewComment { get; set; }
    public DateTime AllowDateStart { get; set; }
    public DateTime AllowDateEnd { get; set; }
    public DateTime RealDateStart { get; set; }
    public DateTime RealDateEnd { get; set; }
    public CranTaskAction TaskAction { get; set; }

    public bool OpenCurrentTime { get; set; }
    public bool FinishCurrentTime { get; set; }

    public bool Allowed { get; set; }
    public bool Denied { get; set; }
    public bool Cancelled { get; set; }
    public bool Finished { get; set; }
    public bool Opened { get; set; }
    public bool Reviewed { get; set; }
    public bool HasReviewComment { get; set; }
    public bool HasAgreeComment { get; set; }
    public string State { get; set; }
    public CranTaskState TaskState { get; set; }
    public bool init { get; set; }
    public bool change { get; set; }
    public bool check { get; set; }
    public bool changed { get; set; }
    public Dictionary<int, string> AgreeDict { get; set; }
    public string StateDB { get; set; }
    public DateTime DateCreate { get; set; }


    public bool canChange { get; set; }
    public bool canCheck { get; set; }
    public bool canComment { get; set; }
    public bool canCancel { get; set; }
    public bool canOpen { get; set; }
    public bool canFinish { get; set; }
    public bool canReturn { get; set; }

    public bool hasCrossTasks { get; set; }
    public string crossTasks { get; set; }

    public CranTaskInfo() {

    }

    static CranTaskInfo() {
      LastUpdate = DateTime.Now;
    }

    public CranTaskInfo(CranTask tbl) {
      OrdersUser currentUser = OrdersUser.getCurrentUser();
      CranNumber = tbl.CranNumber;
      Number = tbl.Number;
      YearNumber = tbl.Number - MaxYearPrevNumbers[tbl.DateCreate.Year];
      NeedStartDate = tbl.NeedDateStart;
      NeedEndDate = tbl.NeedDateEnd;
      Comment = tbl.Comment;
      CranName = tbl.CranName;
      Author = OrdersUser.loadFromCache(tbl.Author).FullName;
      SelAuthor = tbl.SelAuthor;
      AuthorText = tbl.AuthorText;
      State = "Новая";
      TaskState = CranTaskState.created;
      StateDB = tbl.State;
      Allowed = tbl.Allowed;
      Denied = tbl.Denied;
      Cancelled = tbl.Cancelled;
      Finished = tbl.Finished;
      Opened = tbl.Opened;
      Reviewed = Allowed || Denied;
      AgreeComments = tbl.AgreeComment;
      ReviewComment = tbl.ReviewComment;
      HasReviewComment = !String.IsNullOrEmpty(tbl.ReviewComment);
      HasAgreeComment = !String.IsNullOrEmpty(tbl.AgreeComment);
           

      canChange = (!Cancelled) && (!Allowed) && (!Denied) && (!Finished)&&(!Opened) /*&& (tbl.Author.ToLower() == currentUser.Name.ToLower() || tbl.SelAuthor.ToLower()==currentUser.Name.ToLower())*/;
      canCancel = (!Cancelled) && (!Denied) && (!Finished)&&(!Opened) /*&& (tbl.Author.ToLower() == currentUser.Name.ToLower()|| tbl.SelAuthor.ToLower() == currentUser.Name.ToLower())*/;
      canFinish = Opened && currentUser.CanFinishCranTask;
      canOpen = Allowed && currentUser.CanFinishCranTask;
      canReturn = Cancelled;

      canCheck = (currentUser.CanReviewCranTask) && (!Finished) && (!Cancelled)&&(!Opened);
      canComment = true;
      Manager = tbl.Manager;
      StropUser = tbl.StropUser;
      CranUser = tbl.CranUser;
      ManagerShort = getShortName(Manager);
      StropUserShort = getShortName(StropUser);
      CranUserShort = getShortName(CranUser);
      TaskAction = CranTaskAction.none;
      if (Denied) {
        State = "Отклонена";
        TaskState = CranTaskState.reviewed;
        canChange = false;
        
        AuthorAllow = OrdersUser.loadFromCache(tbl.AuthorAllow).FullName;
      }
      if (Allowed) {
        AuthorAllow = OrdersUser.loadFromCache(tbl.AuthorAllow).FullName;
        AllowDateStart = tbl.AllowedDateStart.Value;
        AllowDateEnd = tbl.AllowedDateEnd.Value;
        RealDateStart = tbl.RealDateStart.Value;
        RealDateEnd = tbl.RealDateEnd.Value;
        OpenCurrentTime = true;
        FinishCurrentTime = true;
        canChange = false;
        State = "Разрешена";
        TaskState = CranTaskState.reviewed;
      }

      if (Cancelled) {
        State = "Снята";
        TaskState = CranTaskState.canceled;
        AuthorCancel = OrdersUser.loadFromCache(tbl.AuthorCancel).FullName;
      }

      if (Opened) {
        if (!string.IsNullOrEmpty(tbl.AuthorOpen))
          AuthorOpen = OrdersUser.loadFromCache(tbl.AuthorOpen).FullName;
        //RealDateStart = tbl.RealDateStart.Value;
        OpenCurrentTime = false;
        canChange = false;
        TaskState = CranTaskState.opened;
        State = "Открыта";
      }

      if (Finished) {
        State = "Закрыта";
        TaskState = CranTaskState.finished;
        FinishCurrentTime = false;
        if (!string.IsNullOrEmpty(tbl.AuthorFinish))
          AuthorFinish = OrdersUser.loadFromCache(tbl.AuthorFinish).FullName;
      }

      DateCreate = tbl.DateCreate;
    }

    public static Dictionary<int, string> getAgreeUsers(string ids) {
      Dictionary<int, string> dict = new Dictionary<int, string>();
      try {
        string[] idArr = ids.Split(new char[] { ';' });

        foreach (string id in idArr) {
          try {
            OrdersUser user = OrdersUser.loadFromCache(Int32.Parse(id));
            dict.Add(user.UserID, user.FullName);
          } catch { }
        }
      } catch { }
      return dict;
    }

    public static ReturnMessage CreateCranTask(CranTaskInfo task) {
      Logger.info("Создание/изменение заявки на работу крана", Logger.LoggerSource.server);
      try {
        string result = "";
        string message = String.Format("Заявка на работу крана \"{0}\" №", task.CranName);
        OrdersUser currentUser = OrdersUser.getCurrentUser();
        VotGESOrdersEntities eni = new VotGESOrdersEntities();
        CranTask tbl = new CranTask();

        if (task.TaskAction == CranTaskAction.create) {
          Logger.info("Определение номера заявки на кран", Logger.LoggerSource.server);
          CranTask tsk = (from t in eni.CranTask orderby t.Number descending select t).FirstOrDefault();
          task.DateCreate = DateTime.Now;
          if (tsk != null) {
            task.Number = tsk.Number + 1;
          } else {
            task.Number = 1;
          }
          tbl.Allowed = false;
          tbl.Denied = false;
          tbl.Finished = false;
          tbl.Opened = false;
          tbl.Cancelled = false;
          tbl.Author = currentUser.Name;
          tbl.DateCreate = task.DateCreate;
          task.Author = currentUser.FullName;
          eni.CranTask.AddObject(tbl);
          result = "Заявка на кран успешно создана";
        } else {
          CranTask tsk = (from t in eni.CranTask where t.Number == task.Number select t).FirstOrDefault();
          if (tsk == null) {
            return new ReturnMessage(false, "Заявка не найдена");
          }
          tbl = tsk;
          result = "Заявка на кран успешно изменена";
        }
        task.YearNumber = task.Number - MaxYearPrevNumbers[task.DateCreate.Year];
        message += task.YearNumber + ". ";

        if ((task.NeedEndDate <= task.NeedStartDate) || (task.Allowed && (task.AllowDateEnd <= task.AllowDateStart))) {
          return new ReturnMessage(false, "Дата окончания заявки больше чем дата начала");
        }

        tbl.State = task.TaskState.ToString();
        tbl.Number = task.Number;
        tbl.CranName = task.CranName;
        tbl.AgreeComment = task.AgreeComments;
        tbl.NeedDateStart = task.NeedStartDate;
        tbl.NeedDateEnd = task.NeedEndDate;
        tbl.Comment = task.Comment;
        tbl.Manager = task.Manager;
        tbl.CranNumber = task.CranNumber;
        tbl.SelAuthor = task.SelAuthor;
        tbl.StropUser = task.StropUser;
        tbl.AuthorText = OrdersUser.loadFromCache(tbl.SelAuthor).FullName;

        if (task.TaskAction == CranTaskAction.finish) {
          if (tbl.Denied || tbl.Cancelled || (!tbl.Opened) || (!tbl.Allowed)) {
            return new ReturnMessage(false, "Невозможно завершить заявку. Заявка не открыта");            
          }
          if (task.Finished) {
            result = "Заявка на кран завершена";
            tbl.RealDateStart = task.RealDateStart;
            tbl.RealDateEnd = task.RealDateEnd;
            tbl.AuthorFinish = currentUser.Name;
            tbl.Finished = true;
            message += " Заявка завершена";
          }
        }
        if (task.TaskAction == CranTaskAction.open) {
          if (tbl.Denied || tbl.Cancelled || (!tbl.Allowed)) {
            return new ReturnMessage(false, "Невозможно открыть заявку. Заявка не разрешена");
          }
          if (task.Opened) {
            result = "Заявка на кран открыта";
            tbl.RealDateStart = task.RealDateStart;
            tbl.RealDateEnd = task.RealDateEnd;
            tbl.AuthorOpen = currentUser.Name;
            tbl.Opened = true;
            message += " Заявка открыта";
          }
        }
        if (task.TaskAction == CranTaskAction.review) {
          if (tbl.Cancelled || tbl.Finished ||tbl.Opened) {
            return new ReturnMessage(false, "Невозможно рассмотреть заявку. Заявка снята или закрыта");
          }
          tbl.AuthorAllow = currentUser.Name;
          tbl.ReviewComment = task.ReviewComment;
          if (task.Allowed) {
            tbl.AllowedDateStart = task.AllowDateStart;
            tbl.AllowedDateEnd = task.AllowDateEnd;
            tbl.RealDateStart = task.AllowDateStart;
            tbl.RealDateEnd = task.AllowDateEnd;
            tbl.CranUser = task.CranUser;
            tbl.Denied = false;
            tbl.Allowed = true;
            tbl.Cancelled = false;
            tbl.Opened = false;
            tbl.Finished = false;
            task.AuthorAllow = currentUser.FullName;
            result = "Заявка на кран разрешена";
            message += " Заявка разрешена";
          } else if (task.Denied) {
            tbl.AllowedDateStart = null;
            tbl.AllowedDateEnd = null;
            tbl.RealDateEnd = null;
            tbl.RealDateStart = null;
            tbl.Allowed = false;
            tbl.Denied = true;
            tbl.Cancelled = false;
            tbl.Opened = false;
            tbl.Finished = false;
            tbl.CranUser = "";
            task.AuthorAllow = currentUser.FullName;
            result = "Заявка на кран отклонена";
            message += " Заявка отклонена";
          }
        }
        if (task.TaskAction == CranTaskAction.cancel) {
          if (tbl.Opened || tbl.Finished ||tbl.Cancelled) {
            return new ReturnMessage(false, "Невозможно снять заявку. Заявка открыта");
          }
          if (task.Cancelled) {
            tbl.Denied = false;
            tbl.Allowed = false;
            tbl.Cancelled = true;
            tbl.AuthorAllow = null;
            tbl.AllowedDateStart = null;
            tbl.AllowedDateEnd = null;
            tbl.RealDateEnd = null;
            tbl.RealDateStart = null;
            tbl.AuthorCancel = currentUser.Name;
            task.AuthorAllow = currentUser.FullName;
            result = "Заявка на кран снята";
            message += " Заявка снята";
          }
        }
        if (task.TaskAction == CranTaskAction.returnCancel) {
          if (!tbl.Cancelled) {
            return new ReturnMessage(false, "Невозможно вернуть заявку");
          }
          if (!task.Cancelled) {
            tbl.Denied = false;
            tbl.Allowed = false;
            tbl.Cancelled = false;
            tbl.AuthorCancel = null;
            tbl.AllowedDateStart = null;
            tbl.AllowedDateEnd = null;
            tbl.RealDateEnd = null;
            tbl.RealDateStart = null;
            tbl.AuthorCancel = currentUser.Name;
            task.AuthorAllow = currentUser.FullName;
            result = "Возврат заявки на кран";
            message += " Возврат заявки";
          }
        }
        if (task.TaskAction == CranTaskAction.change) {
          message += " Заявка изменена";
        }

        eni.SaveChanges();
        if (task.TaskState != CranTaskState.opened)
          MailContext.sendCranTask(message, new CranTaskInfo(tbl));
        LastUpdate = DateTime.Now;
        return new ReturnMessage(true, result);
      } catch (Exception e) {
        Logger.info("Ошибка при создании/изменении заявки на работу крана " + e.ToString(), Logger.LoggerSource.server);
        LastUpdate = DateTime.Now;
        return new ReturnMessage(false, "Ошибка при создании/изменении заявки на работу крана");
      }
    }

    public static ReturnMessage AddComment(CranTaskInfo task, string comment) {
      Logger.info("Добавление комментария к заявке на работу крана", Logger.LoggerSource.server);
      try {
        string result = "";
        OrdersUser currentUser = OrdersUser.getCurrentUser();
        VotGESOrdersEntities eni = new VotGESOrdersEntities();
        CranTask tbl = new CranTask();

        CranTask tsk = (from t in eni.CranTask where t.Number == task.Number select t).FirstOrDefault();
        if (tsk == null) {
          return new ReturnMessage(false, "Заявка не найдена");
        }
        tbl = tsk;

        if (!string.IsNullOrEmpty(task.AgreeComments))
          task.AgreeComments += "\r\n";
        task.AgreeComments += String.Format("{2} {0}:\r\n   {1}", currentUser.FullName, comment, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
        tbl.AgreeComment = task.AgreeComments;


        eni.SaveChanges();
        string message = String.Format("Заявка на работу крана \"{0}\" №{1}. Комментарий", task.CranName, task.YearNumber);
        MailContext.sendCranTask(message, new CranTaskInfo(tbl));
        return new ReturnMessage(true, "Комментарий добавлен");
      } catch (Exception e) {
        Logger.info("Ошибка при создании/изменении заявки на работу крана " + e.ToString(), Logger.LoggerSource.server);
        return new ReturnMessage(false, "ошибка при добавлении комментария");
      }
    }

    protected static bool DatesCross(DateTime start1, DateTime end1, DateTime start2, DateTime end2, bool first = true) {
      //Logger.info(String.Format("DatesCross {0} {1} {2} {3} {4}", start1, end1, start2, end2, first), Logger.LoggerSource.server);
      bool cross =
          (start1 >= start2 && start1 < end2 ||
          end1 > start2 && end1 <= end2 ||
          start1 >= start2 && end1 <= end2 ||
          start1 <= start2 && end1 >= end2);
      if (!cross && first) {
        cross = DatesCross(start2, end2, start1, end1, false);
      }
      return cross;

    }

    public static CranFilter LoadCranTasks(CranFilter Filter = null) {
      Logger.info("Получение списка заявок на кран", Logger.LoggerSource.server);
      Logger.info(String.Format("Filter: {0}", Filter), Logger.LoggerSource.server);
      if (Filter == null) {
        Filter = new CranFilter();
        Filter.DateStart = DateTime.Now.Date;
        Filter.DateEnd = DateTime.Now.Date.AddDays(10);

      }
      
      Filter.Managers = ReadTextFile("Managers.txt");
      Filter.CranUsers = ReadTextFile("CranUsers.txt");
      Filter.StropUsers = ReadTextFile("StropUsers.txt");
      //Filter.Managers = Managers;
      VotGESOrdersEntities eni = new VotGESOrdersEntities();
      List<CranTaskInfo> result = new List<CranTaskInfo>();
      IQueryable<CranTask> data = from t in eni.CranTask
                                  where
                                    t.NeedDateStart > Filter.DateStart && t.NeedDateStart < Filter.DateEnd ||
                                    t.NeedDateEnd > Filter.DateStart && t.NeedDateEnd < Filter.DateEnd ||
                                    t.NeedDateStart < Filter.DateStart && t.NeedDateEnd > Filter.DateEnd ||
                                    (t.Allowed &&
                                    t.AllowedDateStart > Filter.DateStart && t.AllowedDateStart < Filter.DateEnd ||
                                    t.AllowedDateEnd > Filter.DateStart && t.AllowedDateEnd < Filter.DateEnd ||
                                    t.AllowedDateStart < Filter.DateStart && t.AllowedDateEnd > Filter.DateEnd)
                                  orderby t.CranNumber, t.NeedDateStart
                                  select t;
      foreach (CranTask tbl in data) {
        result.Add(new CranTaskInfo(tbl));
      }
      foreach (CranTaskInfo task in result) {
        task.hasCrossTasks = false;
        task.crossTasks = "";
        foreach (CranTaskInfo crossTask in result) {
          if (crossTask.Number == task.Number)
            continue;
          if (crossTask.CranNumber != task.CranNumber)
            continue;
          if (task.Denied || crossTask.Denied || task.Cancelled || crossTask.Cancelled || task.Finished || crossTask.Finished)
            continue;
          bool crossed = false;

          //Logger.info(String.Format("{0} - {1}",task.Number,crossTask.Number), Logger.LoggerSource.server);
          if (task.Allowed && crossTask.Allowed) {
            if (DatesCross(task.AllowDateStart, task.AllowDateEnd, crossTask.AllowDateStart, crossTask.AllowDateEnd)) {
              crossed = true;
            }

          }
          if (!task.Allowed && crossTask.Allowed) {
            if (DatesCross(task.NeedStartDate, task.NeedEndDate, crossTask.AllowDateStart, crossTask.AllowDateEnd)) {
              crossed = true;
            }
          }
          if (task.Allowed && !crossTask.Allowed) {
            if (DatesCross(task.AllowDateStart, task.AllowDateEnd, crossTask.NeedStartDate, crossTask.NeedEndDate)) {
              crossed = true;
            }
          }
          if (!task.Allowed && !crossTask.Allowed) {
            if (DatesCross(task.NeedStartDate, task.NeedEndDate, crossTask.NeedStartDate, crossTask.NeedEndDate)) {
              crossed = true;
            }
          }
          if (crossed) {
            task.hasCrossTasks = true;
            task.crossTasks += string.IsNullOrEmpty(task.crossTasks) ? crossTask.YearNumber.ToString() : "," + crossTask.YearNumber.ToString();
          }
        }
      }
      Filter.Data = result;
      Logger.info(String.Format("cnt: {0}", Filter.Data.Count), Logger.LoggerSource.server);
      return Filter;
    }

    public static List<string> ReadTextFile(string fileName) {      
      string[] lines = System.IO.File.ReadAllLines(PathFiles + fileName);
      List<string> result=new List<string>();
      foreach (string line in lines) {
        string fn = getFullName(line);
        if (fn.Length > 10) {
          result.Add(getFullName(line));
        }
      }
      result.Sort();
      return result;
      //return new List<string>();
    }

    /*public static void ReadManagers() {
			Logger.info("Получение списка ответственных", Logger.LoggerSource.server);
			Managers = new List<string>();
			try {
				VotGESOrdersEntities eni = new VotGESOrdersEntities();
				IQueryable<string> data = (from t in eni.CranTask select t.Manager).Distinct();
				foreach (string name in data) {
					if (!(Managers.Contains(name))) {
						Managers.Add(name);
					}
				}
			}
			catch (Exception e) {
				Logger.info("Ошибка при чтении доступных ответственных " + e.ToString(), Logger.LoggerSource.server);
			}
		}*/

    public static string getTaskHTML(CranTaskInfo order, bool showStyle = true) {
      try {
        OrdersUser currentUser = OrdersUser.getCurrentUser();
        string UserInfo = "";
        if (!order.Allowed && !order.Finished && !order.Denied && !order.Cancelled) {
          if (currentUser.Name.ToLower() != order.SelAuthor.ToLower()) {
            UserInfo = String.Format("Заявка подана/изменена из учетной записи {0} ({1})<br/>", currentUser.FullName, currentUser.Name);
          }
        }
        string style = showStyle ? "<Style>table {border-collapse: collapse;} td{text-align:center;} td.comments{text-align:left;} td, th {border-width: 1px;	border-style: solid;	border-color: #BBBBFF;	padding-left: 3px;	padding-right: 3px;}</Style>" : "";
        string htmlNumber = String.Format("Заявка на работу крана №{0} ", order.YearNumber);
        string htmlState = String.Format("Состояние: {0}", order.State);
        string htmlFirstTRTable = String.Format("<table width='100%'><tr><th>{0}</th><th>{1}</th></tr></table>", htmlNumber, htmlState);
        string htmlInfoTable = String.Format("<table width='100%'><tr><th colspan='2'>Информация о заявке</th></tr><tr><th width='40%'>Кран</th><th  width='60%'>Текст заявки</th></tr><tr><td width='40%'>{0}</td><td width='60%'>{1}</td></tr></table>",
          order.CranName, String.Format("{0}", order.Comment));


        string htmlDatesTable =
          String.Format("<table width='100%'><tr><th colspan='4'>Сроки заявки</th></tr><tr><th>&nbsp;</th><th>Сроки</th><th>Автор</th><th>***</th></tr><tr><th>Заявка</th><td>{0}<hr/>{1}</td><td>{2}</td><td>{3}</td></tr><tr><th>Разрешение</th><td>{4}<hr/>{5}</td><td>{6}</td><td>{7}</td></tr><tr><th>Факт</th><td>{8}<hr/>{9}</td><td>{10}<hr/>{11}</td><td>&nbsp;</td></tr></table>",
          order.NeedStartDate.ToString("dd.MM.yy HH:mm"), order.NeedEndDate.ToString("dd.MM.yy HH:mm"), OrdersUser.loadFromCache(order.SelAuthor).FullName, String.Format("<b>Ответственный:</b><br/>{0}<br/><b>Стропальщик:</b><br/>{1}", order.Manager, order.StropUser.Replace("\r\n","<br/>")),
          order.Allowed ? order.AllowDateStart.ToString("dd.MM.yy HH:mm") : order.Denied ? "Отклонено" : order.Cancelled ? "Снята" : "&nbsp;",
          order.Allowed ? order.AllowDateEnd.ToString("dd.MM.yy HH:mm") : order.Denied ? "Отклонено" : order.Cancelled ? "Снята" : "&nbsp;",
          order.Allowed || order.Denied ? order.AuthorAllow : order.Cancelled ? order.AuthorCancel : "-",
          order.Allowed ? String.Format("{0}<br/><b>Крановщик:</b><br/>{1}<br/>", order.ReviewComment, order.CranUser) : order.ReviewComment,
          order.Opened ? order.RealDateStart.ToString("dd.MM.yy HH:mm") : "-", 
          order.Finished ? order.RealDateEnd.ToString("dd.MM.yy HH:mm") : "-", 
          !string.IsNullOrEmpty(order.AuthorOpen) ? order.AuthorOpen : "-",
          !string.IsNullOrEmpty(order.AuthorFinish) ? order.AuthorFinish : "-");


        string aComments = string.IsNullOrEmpty(order.AgreeComments) ? "" : order.AgreeComments.Replace("\r\n", "<br/>");
        string fullTable = String.Format("<table width='100%'><tr><td>{0}</td></tr><tr><td>{1}</td></tr><tr><td>{2}</td></tr><tr><td>{3}</td></tr></table>",
          htmlFirstTRTable, htmlInfoTable, htmlDatesTable, aComments);
        return style + UserInfo + fullTable;
      } catch (Exception e) {
        Logger.info("Ошибка при формировании html представления " + e.ToString(), Logger.LoggerSource.server);
        return "";
      }
    }

    public static string getTaskPrintHTML(CranTaskInfo order, bool showStyle = true) {
      try {
        string style = showStyle ? "<Style>table {border-collapse: collapse;} th.solid, td.solid {border-width: 1px;	border-style: solid;	border-color: #000000;} td.under {border-bottom-width: 1px;	border-bottom-style: solid;	border-bottom-color: #000000;} </Style>" : "";
        string body = String.Format(@"
<table >
	<tr >
		<th  colspan='7' align='right'>Приложение 1 <br/>к Регламенту по предоставлению подъемных сооружений Воткинской ГЭС<br/>(Для статистического учета работ ПС ГЭС)</th>	
	</tr>
	<tr>	
		<th  colspan='7' align='center'><h2>ЗАЯВКА №{0}</h2></th>
	</tr>	
	<tr>	
		<th  colspan='7' align='center'><h3>{1}</h3></th>
	</tr>	
	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr>	
		<th/>
		<th/>
		<th colspan='3' align='center' class='under'>&nbsp;</th>
		<th/>
	</tr>
	<tr>	
		<td colspan='7' align='center'><i>Наименование организации</i></td>
	</tr>
	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr >
		<th align='center' valign='top' width='50' class='solid' rowspan='2' >№ п/п</th>
		<th align='center' valign='top' width='150' class='solid' rowspan='2' >Наименование ПС</th>
		<th align='center' valign='top' width='250' class='solid' rowspan='2' >Краткое содержание работ</th>

		<th align='center' valign='top' width='300' class='solid' colspan='2' >Ответственный стропальщик</th>
		<th align='center' valign='top' width='220' class='solid' colspan='2'>Период использования</th>
		</tr>
	<tr>	
		
		<th align='center' valign='center' width='230' class='solid' >Ф.И.О.</th>
		<th align='center' valign='center' width='70' class='solid' >№ удостоверения</th>
		<th align='center' valign='center' width='110' class='solid' >Начало</th>
		<th align='center' valign='center' width='110' class='solid' >Окончание</th>
		
	</tr>
	<tr>
		<td align='center' valign='top' class='solid' >{2}</td>
		<td align='center' valign='top' class='solid' >{3}</td>
		<td align='center' valign='top' class='solid' >{4}</td>
		<td align='center' valign='top' class='solid' colspan='2'>{5}</td>
		<td align='center' valign='top' class='solid' >{6}</td>
		<td align='center' valign='top' class='solid' >{7}</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
		<td align='center' valign='top' class='solid' >&nbsp;</td>
	</tr>

	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr>
		<td bordercolor='white'  colspan='4'>Специалист, ответственный за безопасное производство работ с применением ПС «Заказчика»</td>
		<td bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2' >{8}</td>
	</tr>


	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr><th colspan='7' bordercolor='white' colspan='2'>СОГЛАСОВАНО </th></tr>

	<tr><td  bordercolor='white' colspan='6' >&nbsp;</td></tr>
	<tr>
		<td  bordercolor='white' colspan='4'>Представитель Дирекции по реализации ПКМ (только для кранов МЗ) </td>
		<td  bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2'>&nbsp;</td>
	</tr>

	<tr><td  bordercolor='white' colspan='7' >&nbsp;</td></tr>
	<tr>
		<td  bordercolor='white' colspan='4'>Специалист, ответственный за осуществление производственного контроля при эксплуатации ПС (при необходимости)</td>
		<td  bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2'>&nbsp;</td>
	</tr>	

	<tr><td  bordercolor='white' colspan='7' >&nbsp;</td></tr>
	<tr><th colspan='6' bordercolor='white' colspan='2'>УТВЕРЖДЕНО </th></tr>

	<tr><td  bordercolor='white' colspan='7' >&nbsp;</td></tr>
	<tr>
		<td  bordercolor='white' colspan='4'>{9}</td>
		<td  bordercolor='white' class='under' align='right'>/</td>
		<td  bordercolor='white' class='under' colspan='2'>{10}</td>
	</tr>	


</table>
", order.YearNumber, order.DateCreate.ToString("dd.MM.yyyy"), 1, order.CranName, order.Comment,
  order.StropUser.Replace("\r\n","<br/>"),
  order.Allowed ? order.AllowDateStart.ToString("dd.MM.yy HH:mm") : order.NeedStartDate.ToString("dd.MM.yy HH:mm"),
  order.Allowed ? order.AllowDateEnd.ToString("dd.MM.yy HH:mm") : order.NeedEndDate.ToString("dd.MM.yy HH:mm"),
  order.CranNumber <= 2 ? "Представитель группы ТиГМО ПТС (только для кранов МЗ)" : "Исполнитель заявки",
  !String.IsNullOrEmpty(order.AuthorAllow) ? order.AuthorAllow : " ", !String.IsNullOrEmpty(order.AuthorAllow) ? order.AuthorAllow : " ");
        return style + body;
      } catch (Exception e) {
        Logger.info("Ошибка при формировании html представления " + e.ToString(), Logger.LoggerSource.server);
        return "";
      }
    }

		protected static string getShortNameOnce(string manager)
		{
			try	{
				string[] parts = manager.Split(new char[] { ':', '(' });
				if (parts.Length >= 2)
					return parts[1];
				else return manager;
			}
			catch (Exception e)		{
				return manager;
			}
		}

    public static string getShortName(string manager) {
			try
			{
				if (manager.Contains("\r\n"))
				{
					try
					{
						string[] managers = manager.Split(new string[] { "\r\n" }, 10, StringSplitOptions.RemoveEmptyEntries);
						string res = "";
						foreach (string man in managers)
						{
							res += getShortNameOnce(man) + "\r\n";
						}
						return res;
					}
					catch
					{
						return manager;
					}
				}
				else
				{
					return getShortNameOnce(manager);
				}
			}catch (Exception e)
			{
				return manager;
			}
			
    }

    public static string getFullName(string manager) {
      try {
        string[] parts = manager.Split(new char[] { ';' });
        string name=String.Format("{0}: {1}",parts[0],parts[1]);
        name=name.Trim(new char[] { ' ' });
        return name;
      } catch (Exception e) {
        return manager;
      }
    }

    public static void initTaskNumbers() {
      Logger.info("Чтение номера последней заявки  на кран в прошлом году", Logger.LoggerSource.server);
      MaxYearPrevNumbers = new Dictionary<int, int>();
      for (int year = 2010; year <= DateTime.Now.Year; year++) {
        try {
          VotGESOrdersEntities ctx = new VotGESOrdersEntities();
          int max = (from o in ctx.CranTask where o.DateCreate.Year == year - 1 select o.Number).Max();
          MaxYearPrevNumbers.Add(year, max);
          Logger.info("Присвоен номер " + max, Logger.LoggerSource.server);
        } catch (Exception e) {
          //Logger.info("Ошибка при получении номера " + e.ToString(), Logger.LoggerSource.server);
          MaxYearPrevNumbers.Add(year, 0);
          //MaxYearPrevNumber = 0;
        }
      }
    }
  }


}