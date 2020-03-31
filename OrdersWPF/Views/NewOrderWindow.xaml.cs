﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using VotGESOrders.OrdersService;

namespace VotGESOrders.Views
{
    /// <summary>
    /// Логика взаимодействия для NewOrderWindow.xaml
    /// </summary>
    public partial class NewOrderWindow : Window
    {
        public bool IsNewOrder { get; set; }
        public Order CurrentOrder { get; set; }
        public Order ParentOrder { get; set; }

        public NewOrderWindow()
        {
            InitializeComponent();
            if (!OrdersClientContext.Current.CurrentUser.AllowCreateCrashOrder)
            {
                OrdersClientContext.Current.OrderInfoSingle.OrderTypes.Remove(OrderTypeEnum.crash);
                OrdersClientContext.Current.OrderInfoSingle.OrderTypes.Remove(OrderTypeEnum.no);
            }

            cmbOrderTypes.ItemsSource = OrdersClientContext.Current.OrderInfoSingle.OrderTypes;

            treeObjects.ItemsSource = from OrderObject o in OrdersClientContext.Current.AllOrderObjects where o.ObjectID == 0 select o;
            lstUsers.ItemsSource = from OrdersUser u in OrdersClientContext.Current.ALLUsers where u.AllowAgreeOrders select u;
            /*cmbReadyTime.ItemsSource = new List<String> { "5 минут", "15 минут", "30 минут", "45 минут", "1 час", "2 часа", "3 часа", "4 часа",
				"5 часов", "6 часов", "7 часов", "8 часов", "9 часов", "10 часов", "Время заявки" };
			orderForm.AutoCommit = false;
			orderForm.AutoEdit = false;

			this.HasCloseButton = false;*/
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            OrderOperations.Current.ApplyCreate(CurrentOrder, IsNewOrder, ParentOrder);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {            
            OrdersClientContext.Current.RefreshOrders(true); 
            this.DialogResult = false;
        }

        protected  void OnOpened()
        {
            //orderForm.CancelEdit();
            

            if (IsNewOrder && !CurrentOrder.OrderIsExtend && !CurrentOrder.OrderIsFixErrorEnter)
            {
                CurrentOrder.PlanStartDate = DateTime.Now.Date.AddDays(1).AddHours(8);
                CurrentOrder.PlanStopDate = DateTime.Now.Date.AddDays(1).AddHours(17);
                if (CurrentOrder.AgreeUsersDict == null)
                    CurrentOrder.AgreeUsersDict = new Dictionary<int, string>();
            }
            if (CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter)
            {
                treeObjects.Visibility = System.Windows.Visibility.Collapsed;
                cmbOrderTypes.Visibility = System.Windows.Visibility.Collapsed;
                txtOrderType.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                treeObjects.Visibility = System.Windows.Visibility.Visible;
                cmbOrderTypes.Visibility = System.Windows.Visibility.Visible;
                txtOrderType.Visibility = System.Windows.Visibility.Collapsed;
            }


            /*if ((CurrentOrder.OrderType == OrderTypeEnum.crash || CurrentOrder.OrderType == OrderTypeEnum.no) && !CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter)
            {
                PlanStartDate.Label = CurrentOrder.OrderType == OrderTypeEnum.crash ? "Факт отказ" : "Факт вывод";
                CurrentOrder.ReadyTime = "Время заявки";
                CurrentOrder.PlanStartDate = DateTime.Now;
            }
            else
            {
                PlanStartDate.Label = "План старт";
            }

            if (CurrentOrder.SelOrderObject != null)
            {
                TreeViewItem item = treeObjects.ItemContainerGenerator.ContainerFromItem(CurrentOrder.SelOrderObject) as TreeViewItem;
                if (item != null)
                {
                    item.IsSelected = true;
                }
            }

            //treeObjects.IsEnabled = !CurrentOrder.OrderIsExtend;

            Title = IsNewOrder ? "Создание заявки" : String.Format("Заявка №{0} от {1}", CurrentOrder.OrderNumber.ToString(OrderInfo.NFI), CurrentOrder.OrderDateCreate.ToShortDateString());

            PlanStartDatePicker.Enabled = !CurrentOrder.OrderIsExtend;
            OrderObjectAddInfo.IsEnabled = !CurrentOrder.OrderIsExtend && !CurrentOrder.OrderIsFixErrorEnter;
            OrderText.IsEnabled = !CurrentOrder.OrderIsExtend && !CurrentOrder.OrderIsFixErrorEnter;*/

        }


        private void treeObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            OrderObject obj = treeObjects.SelectedItem as OrderObject;
            if (obj != null)
            {
                //CurrentOrder.SelOrderObject = obj;
                CurrentOrder.SelOrderObjectID = obj.ObjectID;
                CurrentOrder.SelOrderObjectText = obj.FullName;
            }
        }


        private void lstUsers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OrdersUser user = lstUsers.SelectedItem as OrdersUser;
            if ((CurrentOrder.AgreeText == null) || (CurrentOrder.AgreeUsersDict == null))
            {
                CurrentOrder.AgreeText = "";
                CurrentOrder.AgreeUsersDict = new Dictionary<int, string>();
            }
            if (user != null)
            {
                if (CurrentOrder.AgreeUsersDict.Keys.Contains(user.UserID))
                {
                    CurrentOrder.AgreeUsersDict.Remove(user.UserID);
                }
                else
                {
                    CurrentOrder.AgreeUsersDict.Add(user.UserID, user.FullName);
                }
                CurrentOrder.AgreeText = string.Join("; ", from string name in CurrentOrder.AgreeUsersDict.Values select name);
                CurrentOrder.AgreeUsersIDSText = string.Join(";", from int key in CurrentOrder.AgreeUsersDict.Keys select key.ToString());
            }
        }

        private void cmbOrderTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if ((CurrentOrder.OrderType == OrderTypeEnum.crash || CurrentOrder.OrderType == OrderTypeEnum.no) && !CurrentOrder.OrderIsExtend || CurrentOrder.OrderIsFixErrorEnter)
            {
                PlanStartDate.Label = CurrentOrder.OrderType == OrderTypeEnum.crash ? "Факт отказ" : "Факт вывод";
                CurrentOrder.ReadyTime = "Время заявки";
                CurrentOrder.PlanStartDate = DateTime.Now;
            }
            else
            {
                PlanStartDate.Label = "План старт";
            }*/
        }

    }
}
