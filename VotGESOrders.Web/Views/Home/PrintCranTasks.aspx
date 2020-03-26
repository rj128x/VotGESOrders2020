<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="VotGESOrders.Web.Models" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Список заявок на работу крана</title>
    <style>
        table, tr, td, p {
            font-family: 'Arial';
            font-size: 10pt;
            padding: 0px;
            margin: 0px;
            padding-top: 0px;
            margin-top: 0px;
            padding-bottom: 0px;
            margin-bottom: 0px;
        }

        h1, h2, h3, h4, h5, h6, hr {
            padding: 5;
            margin: 5;
        }


        h1 {
            font-family: 'Arial';
            font-size: 14pt;
        }

        h2 {
            font-family: 'Arial';
            font-size: 10pt;
        }

        table {
            border-collapse: collapse;
        }

        td, th {
            border-width: 1px;
            border-style: solid;
            border-color: #BBBBFF;
            padding-left: 10px;
            padding-right: 10px;
            vertical-align: top;
        }

        th {
            text-align: center;
        }

        td.center {
            text-align: center;
        }



        table td.right, table th.right {
            text-align: right;
        }

        table td.fill, table th.fill {
            background-color: Gray;
        }
    </style>
</head>
<body>
    <%CranFilter answer = Model as CranFilter;  %>
    <div>
        <h1>Заявки на работу крана с <%=answer.DateStart.ToString("dd.MM.yyyy")%> по <%=answer.DateEnd.ToString("dd.MM.yyyy")%></h1>
        <table>
            <tr>
                <th style="width: 30px;">Номер</th>
                <th style="width: 120px;">Автор</th>
                <th>Задание</th>
                <th style="width: 100px;">Состояние</th>
                <th style="width: 100px;">Запр. время</th>
                <th style="width: 100px;">Разр. время</th>
                <th style="width: 100px;">Факт. время</th>
                <th>Комментарии</th>
            </tr>
            <%for (int cranN = 1; cranN <= 9; cranN++) {
                  List<CranTaskInfo> data = answer.Data.FindAll(cran => cran.CranNumber == cranN);
                  if (data.Count > 0) {%>
            <tr>
                <th colspan="7" ><%=data.First().CranName%></th>
            </tr>
            <%foreach (CranTaskInfo rec in data) { %>
            <tr>
                <td><%=rec.Number%></td>
                <td><%=rec.Author%>
                    <%=!string.IsNullOrEmpty(rec.Manager) ? "<p align='right'><b>Отв. </b>" + rec.ManagerShort + "</p>" : ""%>
                </td>
                <td><%=rec.Comment%>                    
                    <%=!string.IsNullOrEmpty(rec.crossTasks)?"<p align='right'><b>Конфликт: </b>"+rec.crossTasks+"</p>":"" %>
                    <%=!string.IsNullOrEmpty(rec.StropUser) ? "<p align='right'><b>Стропальщик </b>" + rec.StropUserShort.Replace("\r\n","<br/>") + "</p>" : ""%>
                    <%=!string.IsNullOrEmpty(rec.CranUser) ? "<p align='right'><b>Крановщик </b>" + rec.CranUserShort + "</p>" : ""%>
                </td>
                <td><%=rec.State%>
                    <%=(!rec.Finished &&!string.IsNullOrEmpty(rec.AuthorAllow)) ? "<p align='right'><i>" + rec.AuthorAllow + "</i></p>" : !string.IsNullOrEmpty(rec.AuthorFinish)?"<p align='right'><i>" + rec.AuthorFinish + "</i></p>":""%>
                    
                </td>
                <td><%=rec.NeedStartDate.ToString("dd.MM.yy HH:mm") + "<p align='right'>" + rec.NeedEndDate.ToString("dd.MM.yy HH:mm") + "</p>"%></td>
                <td><%=rec.Allowed ? "<b>" + rec.AllowDateStart.ToString("dd.MM.yy HH:mm") + "<p align='right'>" + rec.AllowDateEnd.ToString("dd.MM.yy HH:mm") + "</p></b>" : "-"%></td>
                <td><%=rec.Finished ? "<b>" + rec.RealDateStart.ToString("dd.MM.yy HH:mm") + "<p align='right'>" + rec.RealDateEnd.ToString("dd.MM.yy HH:mm") + "</p></b>" : "-"%></td>
                <td>
                    <%=!string.IsNullOrEmpty(rec.ReviewComment) ? rec.ReviewComment+"<hr/>" : ""%>
                    <%=!string.IsNullOrEmpty(rec.AgreeComments) ? rec.AgreeComments.Replace("\r\n", "<br/>") : ""%>

                </td>
            </tr>
            <%}
                  }
              } %>
        </table>
    </div>
</body>
</html>
