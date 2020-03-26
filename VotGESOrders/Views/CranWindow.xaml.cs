using System;
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
using VotGESOrders.CranService;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Views
{
	public partial class CranWindow : ChildWindow
	{
		public CranTaskInfo CurrentTask { get; set; }
		public List<String> Managers { get; set; }
		public List<String> StropUsers { get; set; }
		public Dictionary<int, string> Crans;
		public Dictionary<string, string> SelUsers;
		public CranWindow() {
			InitializeComponent();
			CransContext.Single.Client.CreateCranTaskCompleted += Client_CreateCranTaskCompleted;
			Crans = new Dictionary<int, string>();
			Crans.Add(0, "Выберите кран");
			Crans.Add(1, "Кран мостовой г/п 350/75/10 ст.№1 МЗ");
			Crans.Add(2, "Кран мостовой г/п 350/75/10 ст.№2 МЗ");
			Crans.Add(3, "Кран козловой г/п 2х20 ст.№1 СУС");
			Crans.Add(4, "Кран козловой г/п 2х20 ст.№2 СУС");
			Crans.Add(5, "Кран козловой г/п 2х63/2х5+16 ЩО НБ");
			Crans.Add(6, "Кран полукозловой г/п 2х150 ЩО ВБ");
			Crans.Add(7, "Кран мостовой г/п 50/10 Транс Башни");
			Crans.Add(8, "Кран козловой 2х125 ВСП");

			Crans.Add(9, "Кран козловой г/п 63/10т Произ площ");

			SelUsers = new Dictionary<string, string>();
			foreach (OrdersUser user in OrdersContext.Current.Context.OrdersUsers) {
				if (user.CanCreateCranTask) {
					SelUsers.Add(user.Name, user.FullName);
				}
			}

		}

		void Client_CreateCranTaskCompleted(object sender, CranService.CreateCranTaskCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			ReturnMessage ret = e.Result as ReturnMessage;
			MessageBox.Show(ret.Message);
			if (ret.Result) {
				this.DialogResult = true;
			}
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			CransContext.Single.Client.CreateCranTaskCompleted -= Client_CreateCranTaskCompleted;
		}

		public void init(CranTaskInfo task, List<String> Managers, List<String> StropUsers) {
			this.Managers = Managers;
			this.StropUsers = StropUsers;
			CurrentTask = task;
			pnlTask.DataContext = CurrentTask;
			//lstUsers.ItemsSource = from OrdersUser u in OrdersContext.Current.Context.OrdersUsers where u.CanAgreeCranTask select u;
			acbManager.ItemsSource = Managers;
			acbStropUser.ItemsSource = StropUsers;
			cmbCranName.ItemsSource = Crans;
			cmbAuthorSelName.ItemsSource = SelUsers;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (GlobalStatus.Current.IsBusy)
				return;
			if (String.IsNullOrEmpty(CurrentTask.SelAuthor)) {
				MessageBox.Show("Выберите автора заявки");
				return;
			}
			if (!this.SelUsers.Keys.Contains(CurrentTask.SelAuthor)) {
				MessageBox.Show("Выберите автора заявки");
				return;
			}
			if (CurrentTask.CranNumber == 0) {
				MessageBox.Show("Выберите кран");
				return;
			}
			if (CurrentTask.Manager.Length < 5) {
				MessageBox.Show("Введите ответственного");
				return;
			}
			if (CurrentTask.StropUser.Length < 5) {
				MessageBox.Show("Введите стропальщика");
				return;
			}
			if (string.IsNullOrEmpty(CurrentTask.Comment)) {
				MessageBox.Show("Введите текст заявки");
				return;
			}
			if (CurrentTask.NeedEndDate <= CurrentTask.NeedStartDate) {
				MessageBox.Show("Время окончания меньше времени начала");
				return;
			}

			/*if (CurrentTask.NeedStartDate <= DateTime.Now) {
	MessageBox.Show("Время заявки меньше текущего");
	return;
}*/

			CurrentTask.CranName = Crans[CurrentTask.CranNumber];
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			if (GlobalStatus.Current.IsBusy)
				return;
			this.DialogResult = false;
		}

		private void btnManager_Click(object sender, RoutedEventArgs e) {
			acbManager.Focus();
			if (String.IsNullOrEmpty(acbManager.Text))
				acbManager.Text = " ";
			acbManager.IsDropDownOpen = true;
		}

		private void btnStropUser_Click(object sender, RoutedEventArgs e) {
			acbStropUser.Focus();
			if (String.IsNullOrEmpty(acbStropUser.Text))
				acbStropUser.Text = " ";
			acbStropUser.IsDropDownOpen = true;
		}



		private void btnAddStropUser_Click(object sender, RoutedEventArgs e) {
			try {
				if (acbStropUser.Text.Length > 10) {
					try {
						if (CurrentTask.StropUser.Length > 10)
							CurrentTask.StropUser += "\r\n";
					} catch { }
					CurrentTask.StropUser += acbStropUser.Text;
					acbStropUser.Text = "";
				}
			} catch { }
		}

		private void btnClearStropUser_Click(object sender, RoutedEventArgs e) {
			CurrentTask.StropUser = "";			
		}
	}
}

