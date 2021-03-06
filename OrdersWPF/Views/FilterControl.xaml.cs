﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VotGESOrders.OrdersService;

namespace VotGESOrders.Views
{
	public partial class FilterControl : UserControl
	{
		protected OrderFilter filter;
		public  ChooseObjectsWindow chooseObjectsWindow;
		public FilterControl() {
			InitializeComponent();
			//filter = DataContext as OrderFilter;
			if (OrdersClientContext.Current!=null && OrdersClientContext.Current.OrderFilterSingle != null) { 
				cmbFilterDate.ItemsSource = OrdersClientContext.Current.OrderFilterSingle.DateFilterTypes;
				cmbFilterUser.ItemsSource = OrdersClientContext.Current.OrderFilterSingle.UserFilterTypes;
			}
		}		

		private void lstAllUsers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			try {
				filter = DataContext as OrderFilter;
				OrdersUser user=lstAllUsers.SelectedItem as OrdersUser;
				if (!filter.SelectedUsers.Contains(user))
					filter.SelectedUsers.Add(user);
			} catch {
			}
		}

		private void lstSelUsers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			try {
				filter = DataContext as OrderFilter;
				OrdersUser user=lstSelUsers.SelectedItem as OrdersUser;
				if (filter.SelectedUsers.Contains(user))
					filter.SelectedUsers.Remove(user);
			} catch {
			}
		}
				
		private void btnChooseObjects_Click(object sender, RoutedEventArgs e) {
			chooseObjectsWindow = new ChooseObjectsWindow();
			chooseObjectsWindow.CurrentFilter = DataContext as OrderFilter;
			chooseObjectsWindow.ShowDialog();
		}

	}
}
