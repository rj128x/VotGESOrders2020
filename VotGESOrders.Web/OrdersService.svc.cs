using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Web
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "OrdersService" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы OrdersService.svc или OrdersService.svc.cs в обозревателе решений и начните отладку.
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class OrdersService
    {
        private OrdersContext context = new OrdersContext();
        private OrderObjectContext objContext = new OrderObjectContext();
        private OrdersUserContext usrContext = new OrdersUserContext();
        public OrdersService()
        {

            context = new OrdersContext();
            objContext = new OrderObjectContext();
            usrContext = new OrdersUserContext();
        }

        [OperationContract]
        public OrderFilter InitFilter()
        {
            return  OrderFilter.Single;
        }

        [OperationContract]
        public OrderInfo InitOrderInfo()
        {
            return OrderInfo.Current;
        }

        [OperationContract]
        public IQueryable<Order> LoadOrders(Guid guid)
        {
            LastUpdate.saveUpdate(guid);
            return context.Orders;
        }

        [OperationContract]
        public IQueryable<Order> GetFilteredOrdersFromFilter(OrderFilter Filter, Guid guid)
        {
            Filter.SelectedUsersStr = Filter.getSelectedUsersJoinStr();
            Filter.SelectedObjectsStr = Filter.getSelectedObjectsJoinStr();
            Logger.info("Получение списка заказов (GetFilteredOrdersFromXML)", Logger.LoggerSource.service);
            LastUpdate.saveUpdate(guid);
            return context.getOrders(Filter);
        }

        [OperationContract]
        public IQueryable<Order> GetFilteredOrdersFromFilterToMail(OrderFilter Filter, Guid guid)
        {
            Filter.SelectedUsersStr = Filter.getSelectedUsersJoinStr();
            Filter.SelectedObjectsStr = Filter.getSelectedObjectsJoinStr();
            Logger.info("Получение списка заказов (GetFilteredOrdersFromXML) В почту", Logger.LoggerSource.service);
            IQueryable<Order> ordersQuery = context.getOrders(Filter);
            List<Order> orders = ordersQuery.ToList();
            MailContext.sendOrdersList("Список заявкок", orders);
            return ordersQuery;
        }

        [OperationContract]
        public IQueryable<OrdersUser> LoadOrdersUsers()
        {
            Logger.info("Получение списка пользователей (LoadUsers)", Logger.LoggerSource.service);
            return OrdersUser.getAllUsers().AsQueryable();
        }

        [OperationContract]
        public IQueryable<OrderObject> LoadOrderObjects()
        {
            Logger.info("Получение списка объектов (LoadOrderObjects)", Logger.LoggerSource.service);
            return OrderObject.getAllObjects().AsQueryable();
        }

        [OperationContract]
        public void RegisterNew(Order order, Guid guid)
        {
            Logger.info("Создание заявки", Logger.LoggerSource.service);
            context.RegisterOrder(order, guid);
        }


        public void UpdateOrder(Order order)
        {
            //Logger.info("Сервис: Update " + order.OrderNumber.ToString());
            //context.UpdateOrder(order);
        }


        [OperationContract]
        public void RegisterChangeOrder(Order order, Guid guid)
        {
            Logger.info("изменение заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.ChangeOrder(order, guid);
        }

        [OperationContract]
        public void RegisterAcceptOrder(Order order, Guid guid)
        {
            Logger.info("разрешение заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.AcceptOrder(order, guid);
        }

        [OperationContract]
        public void RegisterBanOrder(Order order, Guid guid)
        {
            Logger.info("запрет заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.BanOrder(order, guid);
        }

        [OperationContract]
        public void RegisterCancelOrder(Order order, Guid guid)
        {
            Logger.info("снятие заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.CancelOrder(order, guid);
        }

        [OperationContract]
        public void RegisterOpenOrder(Order order, Guid guid)
        {
            Logger.info("открытие заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.OpenOrder(order, guid);
        }

        [OperationContract]
        public void RegisterCloseOrder(Order order, Guid guid)
        {
            Logger.info("разрешение ввода " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.CloseOrder(order, guid);
        }

        [OperationContract]
        public void RegisterCompleteOrder(Order order, Guid guid)
        {
            Logger.info("ввод оборудования " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.CompleteOrder(order, guid);
        }

        [OperationContract]
        public void RegisterRejectReviewOrder(Order order, Guid guid)
        {
            Logger.info("отмена рассмотрения заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.RejectReviewOrder(order, guid);
        }

        [OperationContract]
        public void RegisterRejectCancelOrder(Order order, Guid guid)
        {
            Logger.info("отмена снятия заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.RejectCancelOrder(order, guid);
        }

        [OperationContract]
        public void RegisterRejectOpenOrder(Order order, Guid guid)
        {
            Logger.info("отмена открытия заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.RejectOpenOrder(order, guid);
        }

        [OperationContract]
        public void RegisterRejectCloseOrder(Order order, Guid guid)
        {
            Logger.info("отмена разрешения на ввод заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.RejectCloseOrder(order, guid);
        }

        [OperationContract]
        public void RegisterRejectCompleteOrder(Order order, Guid guid)
        {
            Logger.info("отмена закрытия заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.RejectCompleteOrder(order, guid);
        }

        [OperationContract]
        public void RegisterEditOrder(Order order, Guid guid)
        {
            Logger.info("редактирование заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.RegisterEditOrder(order, guid);
        }

        [OperationContract]
        public void RegisterAddComment(Order order, string commentText, Guid guid)
        {
            Logger.info("комментирование заявки " + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.RegisterAddComment(order, commentText, guid);
        }

        [OperationContract]
        public void ReloadOrder(Order order, Guid guid)
        {
            Logger.info("обновление заявки" + order.OrderNumber.ToString(), Logger.LoggerSource.service);
            context.ReloadOrder(order);
        }

        [OperationContract]
        public bool ExistsChanges(Guid guid)
        {
            bool exist = LastUpdate.IsChanged(guid);
            //Logger.info(String.Format("Проверка изменений {0}, {1} : {2}",lastUpdate,guid,exist));
            return exist;
        }

        [OperationContract]
        public void UpdateOrderObject(OrderObject obj)
        {
            //Logger.info("Сервис: Update " + obj.ObjectName);
            //context.UpdateOrder(order);
        }


        [OperationContract]
        public void RegisterChangeObject(OrderObject newObject)
        {
            Logger.info("изменение оборудования " + newObject.ObjectName, Logger.LoggerSource.service);
            objContext.RegisterChangeOrderObject(newObject);
        }

        [OperationContract]
        public void RegisterDeleteObject(OrderObject newObject)
        {
            Logger.info("удаление оборудования " + newObject.ObjectName, Logger.LoggerSource.service);
            objContext.RegisterDeleteOrderObject(newObject);
        }

        public void UpdateOrdersUser(OrdersUser obj)
        {
            //Logger.info("Сервис: Update " + obj.FullName);
            //context.UpdateOrder(order);
        }

        [OperationContract]
        public void RegisterChangeUser(OrdersUser newObject)
        {
            Logger.info("изменение пользователя " + newObject.FullName, Logger.LoggerSource.service);
            usrContext.RegisterChangeUser(newObject);
        }

        [OperationContract]
        public void RegisterDeleteUser(OrdersUser newObject)
        {
            Logger.info("удаление пользователя " + newObject.FullName, Logger.LoggerSource.service);
            usrContext.RegisterDeleteUser(newObject);
        }

    }
}

