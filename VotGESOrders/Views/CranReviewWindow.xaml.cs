using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VotGESOrders.CranService;

namespace VotGESOrders.Views
{
	public partial class CranReviewWindow : ChildWindow {
		public CranTaskInfo CurrentTask { get; set; }
        public List<String> CranUsers { get; set; }
        public CranReviewWindow() {
			InitializeComponent();
			CransContext.Single.Client.CreateCranTaskCompleted += Client_CreateCranTaskCompleted;
		}

		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			CransContext.Single.Client.CreateCranTaskCompleted -= Client_CreateCranTaskCompleted;
		}

		void Client_CreateCranTaskCompleted(object sender, CranService.CreateCranTaskCompletedEventArgs e) {
			GlobalStatus.Current.IsBusy = false;
			ReturnMessage ret = e.Result as ReturnMessage;
			MessageBox.Show(ret.Message);
			if (ret.Result) {
				this.DialogResult = true;
			}
		}

		public void init(CranTaskInfo task,List<String> CranUsers) {
            this.CranUsers = CranUsers;
			CurrentTask = task;
            acbCranUser.ItemsSource = CranUsers;
			pnlTask.DataContext = CurrentTask;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (GlobalStatus.Current.IsBusy)
				return;
      if (!CurrentTask.Allowed && !CurrentTask.Denied) {
        MessageBox.Show("Выберите действие");
        return;
      }

      if (CurrentTask.Allowed && CurrentTask.CranUser.Length<5) {
        MessageBox.Show("Введите крановщика");
        return;
      }
      if (CurrentTask.Allowed && CurrentTask.AllowDateEnd < CurrentTask.AllowDateStart) {
				MessageBox.Show("Время окончания меньше времени начала");
				return;
			}
			/*if (CurrentTask.Allowed && CurrentTask.AllowDateStart<DateTime.Now) {
				MessageBox.Show("Время заявки меньше текущего");
				return;
			}*/
			GlobalStatus.Current.IsBusy = true;
			CransContext.Single.Client.CreateCranTaskAsync(CurrentTask);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		private void btnAllow_Click(object sender, RoutedEventArgs e) {
			CurrentTask.Allowed = true;
			CurrentTask.AllowDateStart = CurrentTask.NeedStartDate;
			CurrentTask.AllowDateEnd = CurrentTask.NeedEndDate;
			CurrentTask.Denied = false;
		}

		private void btnDenie_Click(object sender, RoutedEventArgs e) {
			CurrentTask.Denied = true;
			CurrentTask.Allowed = false;
		}

    private void btnCranUser_Click(object sender, RoutedEventArgs e) {
      acbCranUser.Focus();
      if (String.IsNullOrEmpty(acbCranUser.Text))
        acbCranUser.Text = " ";
      acbCranUser.IsDropDownOpen = true;
    }
  }
}

