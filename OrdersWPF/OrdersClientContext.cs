using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using VotGESOrders.OrdersService;
using System.Linq;
using System.Collections.Generic;
using VotGESOrders.AdditService;
using System.Collections.ObjectModel;

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

        public ObservableCollection<OrdersUser> ALLUsers { get; set; }
        public ObservableCollection<OrderObject> AllOrderObjects { get; set; }
        public ObservableCollection<Order> CurrentOrders { get; set; }
        public CollectionViewSource CurrentView { get; set; }

        public User CurrentUser { get; set; }

        public OrderInfo OrderInfoSingle { get; set; }
        public OrderFilter OrderFilterSingle { get; set; }

        //public List View { get; set; }


        public OrderFilter Filter { get; set; }


        protected void loadData()
        {
            GlobalStatus.Current.IsBusy = true;
            OrdersClient.GetFilteredOrdersFromFilterCompleted += OrdersClient_GetFilteredOrdersFromFilterCompleted;
            OrdersClient.GetFilteredOrdersFromFilterToMailCompleted += OrdersClient_GetFilteredOrdersFromFilterToMailCompleted;
            GlobalStatus.Current.Status = "Загрузка начальных данных";
            OrdersClient.InitOrderInfoCompleted += OrdersClient_InitOrderInfoCompleted;

            OrdersClient.InitOrderInfoAsync();

        }

        private void OrdersClient_GetFilteredOrdersFromFilterToMailCompleted(object sender, GetFilteredOrdersFromFilterToMailCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OrdersClient_GetFilteredOrdersFromFilterCompleted(object sender, GetFilteredOrdersFromFilterCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                CurrentOrders.Clear();
                foreach (Order order in e.Result)
                {
                    CurrentOrders.Add(order);
                }
                
                CurrentView.Source = CurrentOrders;
                LastUpdate = DateTime.Now;
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке данных ");
            }
            GlobalStatus.Current.IsBusy = false;
        }

        private void OrdersClient_InitOrderInfoCompleted(object sender, InitOrderInfoCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                OrderInfoSingle = e.Result;
                OrdersClient.InitFilterCompleted += OrdersClient_InitFilterCompleted; ;
                OrdersClient.InitFilterAsync();
            }
            else
            {
                MessageBox.Show("Не удалось загрузить начальные данные. Перезагрузите приложение");
            }
        }

        private void OrdersClient_InitFilterCompleted(object sender, InitFilterCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                OrderFilterSingle = e.Result;
                Filter = e.Result;
                Filter.FilterType = OrderFilterEnum.defaultFilter;
                
                AdditServiceClient adClient = new AdditServiceClient();
                adClient.GetAuthenticatedUserCompleted += AdClient_GetAuthenticatedUserCompleted;
                adClient.GetAuthenticatedUserAsync();
            }
            else
            {
                MessageBox.Show("Не удалось загрузить начальные данные. Перезагрузите приложение");
            }
        }

        private void AdClient_GetAuthenticatedUserCompleted(object sender, GetAuthenticatedUserCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                CurrentUser = e.Result;
                GlobalStatus.Current.Status = "Загрузка пользователей";
                OrdersClient.LoadOrdersUsersCompleted += OrdersClient_LoadOrdersUsersCompleted;
                OrdersClient.LoadOrdersUsersAsync();
            }
            else
            {
                MessageBox.Show("Ошибка при авторизации. Перезагрузите приложение");
            }
        }

        private void OrdersClient_LoadOrdersUsersCompleted(object sender, LoadOrdersUsersCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ALLUsers = e.Result;
                GlobalStatus.Current.Status = "Загрузка оборудования";
                OrdersClient.LoadOrderObjectsCompleted += OrdersClient_LoadOrderObjectsCompleted;
                OrdersClient.LoadOrderObjectsAsync();
            }

            else
            {
                MessageBox.Show("Ошибка при загрузке списка пользователей");
            }
        }
        
        private void OrdersClient_LoadOrderObjectsCompleted(object sender, LoadOrderObjectsCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                AllOrderObjects = e.Result;
                foreach (OrderObject obj in AllOrderObjects)
                {
                    obj.ChildObjects = new ObservableCollection<OrderObject>();
                    foreach (OrderObject chO in AllOrderObjects)
                    {
                        if (chO.ParentObjectID == obj.ObjectID)
                        {
                            obj.ChildObjects.Add(chO);
                            chO.ParentObjectID = obj.ObjectID;
                        }
                    }
                }
                GlobalStatus.Current.Status = "Загрузка Заявок";
                OrdersClient.LoadOrdersCompleted += OrdersClient_LoadOrdersCompleted;
                OrdersClient.LoadOrdersAsync(Current.SessionGUID);
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке списка объектов оборудования");
            }
        }

        private void OrdersClient_LoadOrdersCompleted(object sender, LoadOrdersCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                GlobalStatus.Current.Status = "Загрузка завершена";
                LastUpdate = DateTime.Now;
                CurrentOrders = new ObservableCollection<Order>();
                foreach (Order order in e.Result)
                {
                    CurrentOrders.Add(order);
                }
                CurrentView = new CollectionViewSource();
                CurrentView.Source = CurrentOrders;                
                if (FinishLoadingOrdersEvent != null)
                {
                    FinishLoadingOrdersEvent();
                }
                GlobalStatus.Current.IsBusy = false;
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке списка объектов оборудования");
            }

        }

        public delegate void DelegateLoadedAllData();
        public DelegateLoadedAllData FinishLoadingOrdersEvent = null;



        protected void RefreshOrdersFilterXML(bool clear, bool sendMail)
        {
            if (clear)
            {
                CurrentOrders.Clear();
            }
            GlobalStatus.Current.IsBusy = true;
            if (!sendMail)
            {
                OrdersClient.GetFilteredOrdersFromFilterAsync(Filter, OrdersClientContext.Current.SessionGUID);

            }
            else
            {
                OrdersClient.GetFilteredOrdersFromFilterToMailAsync(Filter, OrdersClientContext.Current.SessionGUID);
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
