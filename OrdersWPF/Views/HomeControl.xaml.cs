using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VotGESOrders.OrdersService;

namespace VotGESOrders.Views
{
    /// <summary>
    /// Логика взаимодействия для HomeControl.xaml
    /// </summary>
    public partial class HomeControl : UserControl
    {
        DispatcherTimer timerExistChanges;
        PrintDocument multidoc;
        public HomeControl()
        {
            InitializeComponent();
            timerExistChanges = new DispatcherTimer();
            timerExistChanges.Tick += TimerExistChanges_Tick;
            timerExistChanges.Interval = new TimeSpan(0, 0, 30);

        }

        private void TimerExistChanges_Tick(object sender, EventArgs e)
        {
            if (OrdersClientContext.Current.Filter.FilterType != OrderFilterEnum.userFilter)
            {
                bool existChanges=  OrdersClientContext.Current.OrdersClient.ExistsChanges(OrdersClientContext.Current.SessionGUID);

                    GlobalStatus.Current.IsError = false;
                    if (OrdersClientContext.Current.LastUpdate.AddMinutes(10) < DateTime.Now)
                    {
                        GlobalStatus.Current.NeedRefresh = true;
                    }
                    if (existChanges || GlobalStatus.Current.NeedRefresh)
                    {
                        if (GlobalStatus.Current.CanRefresh)
                        {
                            OrdersClientContext.Current.RefreshOrders(true);
                        }
                        else
                        {
                            GlobalStatus.Current.NeedRefresh = true;
                        }
                    }
            }
        }

        public void LoadControl()
        {
            try
            {
                timerExistChanges.Start();
            }
            catch { }
            pnlButtons.DataContext = OrdersClientContext.Current.CurrentUser;

            OrdersClientContext.Current.CurrentView.View.CollectionChanged += View_CollectionChanged;
            /*OrdersContext.Current.View.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(View_CollectionChanged);
            OrdersContext.Current.Context.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(context_PropertyChanged);*/

            
            ordersGridControl.ordersGrid.ItemsSource = OrdersClientContext.Current.CurrentView.View;
            ordersGridControl.ordersGrid.MouseLeftButtonUp += OrdersGrid_MouseLeftButtonUp;

            cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
            cntrlFilter.DataContext = OrdersClientContext.Current.Filter;
            cmbFilterType.ItemsSource = OrdersClientContext.Current.OrderFilterSingle.FilterTypes;
            cmbFilterType.DataContext = OrdersClientContext.Current.Filter;
            cntrlFilter.lstAllUsers.ItemsSource = OrdersClientContext.Current.ALLUsers;
            cntrlFilter.chooseObjectsWindow = new ChooseObjectsWindow();
            Logger.logMessage("Главная страница загружена");
        }

        private void OrdersGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Order order = ordersGridControl.ordersGrid.SelectedItem as Order;
            if (order != null)
            {
                cntrlOrder.DataContext = order;
                if (OrderOperations.Current.CurrentOrder == null)
                {
                    cntrlOrder.Visibility = System.Windows.Visibility.Visible;
                }
                OrderOperations.Current.CurrentOrder = order;
            }
        }

        private void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (OrderOperations.Current.CurrentOrder == null)
            {
                cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void CloseControl()
        {
            try
            {
                timerExistChanges.Stop();
            }
            catch { }
        }

        private void btnCreateBaseOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrderOperations.Current.CurrentOrder != null)
            {
                OrderOperations.Current.initCreateBase(OrderOperations.Current.CurrentOrder);
            }
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderOperations.Current.initCreate();
        }

        private void cmbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnVisFilter.Visibility = OrdersClientContext.Current.Filter.FilterType == OrderFilterEnum.userFilter ?
                System.Windows.Visibility.Visible :
                System.Windows.Visibility.Collapsed;
            cntrlFilter.Visibility = OrdersClientContext.Current.Filter.FilterType == OrderFilterEnum.userFilter ?
                System.Windows.Visibility.Visible :
                System.Windows.Visibility.Collapsed;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            OrdersClientContext.Current.RefreshOrders(true);
            //OrdersClientContext.Current.CurrentView.Refresh();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnVisDetails_Click(object sender, RoutedEventArgs e)
        {
            if (OrderOperations.Current.CurrentOrder == null)
            {
                cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if ((cntrlOrder.Visibility == System.Windows.Visibility.Collapsed) && (ordersGridControl.ordersGrid.SelectedItem != null))
                {
                    cntrlOrder.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    cntrlOrder.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void btnVisFilter_Click(object sender, RoutedEventArgs e)
        {
            if (cntrlFilter.Visibility == System.Windows.Visibility.Visible)
            {
                cntrlFilter.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                cntrlFilter.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
