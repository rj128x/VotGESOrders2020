using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using VotGESOrders.OrdersService;
using System.Linq;
using System.Collections.Generic;
using VotGESOrders.AdditService;

namespace VotGESOrders
{
    public class OrdersClientContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        protected OrdersClientContext()
        {
            SessionGUID = Guid.NewGuid();
        }

        private DateTime lastUpdate;
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdate;
            }
            protected set
            {
                lastUpdate = value;
                NotifyChanged("LastUpdate");
            }
        }

        public Guid SessionGUID { get; protected set; }

        protected static OrdersClientContext ordersContext;
        public static OrdersClientContext Current
        {
            get
            {
                return ordersContext;
            }
        }

        protected OrdersServiceClient context;
        public OrdersServiceClient OrdersClient { get; protected set; }

        public List<OrdersUser> ALLUsers { get; set; }
        public List<OrderObject> AllOrderObjects { get; set; }
        public List<Order> CurrentOrders { get; set; }

        public User CurrentUser { get; set; }

        public OrderInfo OrderInfoSingle { get; set; }
        public OrderFilter OrderFilterSingle { get; set; }

        //public List View { get; set; }


        public OrderFilter Filter { get; set; }


        protected void loadData()
        {

            OrderInfoSingle = OrdersClient.InitOrderInfo();
            OrderFilterSingle = OrdersClient.InitFilter();
            AdditServiceClient adClient = new AdditServiceClient();
             CurrentUser = adClient.GetAuthenticatedUser();
            adClient.Close();
            ALLUsers = OrdersClient.LoadOrdersUsers().ToList();
            AllOrderObjects = OrdersClient.LoadOrderObjects().ToList();

            CurrentOrders = OrdersClient.LoadOrders(SessionGUID).ToList();
            loadOrders();
        }

        public delegate void DelegateLoadedAllData();
        public DelegateLoadedAllData FinishLoadingOrdersEvent = null;

        private void loadOrders()
        {
            //Logger.info(String.Format("readyObjects:{0} readyUsers:{1} readyOrders:{2} readyAll:{3}", readyObjects, readyUsers, readyOrders, readyAll));

            try
            {
               // OrderOperations.Current.CreateWindows();
               
                /*view = new PagedCollectionView(context.Orders);
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderState", System.ComponentModel.ListSortDirection.Ascending));
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderNumber", System.ComponentModel.ListSortDirection.Descending));*/
                LastUpdate = DateTime.Now;
                if (FinishLoadingOrdersEvent != null)
                {
                    FinishLoadingOrdersEvent();
                }
            }
            catch (Exception e)
            {
                //Logger.info("ошибка в loadOrders " + e.ToString());
            }
        }






        protected void RefreshOrdersFilterXML(bool clear, bool sendMail)
        {
            if (clear)
            {
                //CurrentOrders.Clear();
            }
            
            if (!sendMail)
            {

                context.GetFilteredOrdersFromFilter(Filter, OrdersClientContext.Current.SessionGUID);
            }
            else
            {
                    context.GetFilteredOrdersFromFilterToMail(Filter, OrdersClientContext.Current.SessionGUID);
            }
            LastUpdate = DateTime.Now;

        }

        public void RefreshOrders(bool clear)
        {
            //OrderOperations.Current.CurrentOrder = null;
            RefreshOrdersFilterXML(clear, false);
        }

        public void SendMail(bool clear)
        {
            //OrderOperations.Current.CurrentOrder = null;
            RefreshOrdersFilterXML(clear, true);
        }

        public static void init()
        {
            ordersContext = new OrdersClientContext();
            ordersContext.OrdersClient = new OrdersServiceClient();
            ordersContext.Filter = new OrderFilter();
            //GlobalStatus.Current.init();
        }

        public static void load()
        {
            ordersContext.loadData();
        }


    }
}
