using System;
using System.Windows;
using System.ComponentModel;

namespace VotGESOrders
{
	public class GlobalStatus : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public GlobalStatus() {
			IsBusy = false;
		}

		public void init() {
			try {
				OrdersClientContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);
				LastUpdate = OrdersClientContext.Current.LastUpdate;
				HomeHeader = "Список заявок";
			} catch (Exception e){
				Logger.logMessage(e.ToString());
			}
		}

		void Current_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "LastUpdate") {
				LastUpdate = OrdersClientContext.Current.LastUpdate;
			}
		}


		protected string status;
		public string Status {
			get {
				return status;
			}
			set {
				status = value;
				NotifyChanged("Status");
			}
		}

		protected bool isBusy;
		public bool IsBusy {
			get {
				return isBusy;
			}
			set {
				isBusy = value;
				if (isBusy) {
					isChangingOrder = false;
				}
				NotifyChanged("IsBusy");
				CanRefresh = !IsBusy && !IsChangingOrder;
			}
		}

		protected bool isChangingOrder;
		public bool IsChangingOrder {
			get { return isChangingOrder; }
			set { 
				isChangingOrder = value;
				NotifyChanged("IsChangingOrder");
				CanRefresh = !IsBusy && !IsChangingOrder;
			}
		}

		protected bool isError;
		public bool IsError {
			get { return isError; }
			set {
				isError = value;
				NotifyChanged("IsError");
			}
		}

		private bool canRefresh;
		public bool CanRefresh {
			get { return canRefresh; }
			set { 
				canRefresh = value;
				NotifyChanged("CanRefresh");
			}
		}


		private bool needRefresh;
		public bool NeedRefresh {
			get { return needRefresh; }
			set { 
				needRefresh = value;
				NotifyChanged("NeedRefresh");
			}
		}

		private string homeHeader;
		public string HomeHeader {
			get { return homeHeader; }
			set { 
				homeHeader = value;
				NotifyChanged("HomeHeader");
			}
		}

		private DateTime lastUpdate;
		public DateTime LastUpdate {
			get {
				return lastUpdate;
			}
			protected set {
				lastUpdate = value;
				NotifyChanged("LastUpdate");
				NeedRefresh = false;
			}
		}

		public static GlobalStatus Current {
			get {
				return Application.Current.Resources["globalStatus"] as GlobalStatus;
			}
		}
	}
}
