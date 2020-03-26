using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using VotGESOrders.Logging;
using VotGESOrders.Views;

namespace VotGESOrders
{
	public partial class MainPage : UserControl
	{
		public static bool STARTING = true;

		public MainPage() {
			InitializeComponent();
		}

		public void startLoad() {
			Logger.info("Старт главной страницы");
			OrdersContext.init();
			if (Application.Current.IsRunningOutOfBrowser) {
				// Проверка наличия новых версий

				Application.Current.CheckAndDownloadUpdateCompleted += Current_CheckAndDownloadUpdateCompleted;
				Application.Current.CheckAndDownloadUpdateAsync();
			}
			LoginName.DataContext = WebContext.Current.User;
			LinkEditTree.Visibility = WebContext.Current.User.AllowEditTree ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			LinkEditUsers.Visibility = WebContext.Current.User.AllowEditUsers ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			OrdersContext.Current.FinishLoadingOrdersEvent += new OrdersContext.DelegateLoadedAllData(finish);
			OrdersContext.load();

		}

		private void Current_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e) {
			if (e.UpdateAvailable) {
				MessageBox.Show("Установлена новая версия. Перезапустите приложение.");
				App.Current.MainWindow.Close();
			} else if (e.Error != null && e.Error is PlatformNotSupportedException) {
				MessageBox.Show("Есть новые версии приложения"
					 + "однако для их применения необходима новая версия Silverlight. " +
					 "Посетите сайт http://silverlight.net для обновления Silverlight.");
				App.Current.MainWindow.Close();
			}
		}

		public void finish() {
			try {
				Logger.info("Main page finish "+ ContentFrame == null ? "ContentFrame is null" : ContentFrame.ToString());
				//throw new Exception();
				if (ContentFrame != null) {
					Logger.info(ContentFrame.Content == null ? "ContentFrame.content is null" : ContentFrame.Content.ToString());

					if (ContentFrame.Content == null) {
						Logger.info("Ручное перенаправление (contentframe=null)");
						STARTING = false;
						ContentFrame.Content = null;
						Home home = new Home();
						ContentFrame.Content = home;
					}
					else if (ContentFrame.Content is Home) {
						(ContentFrame.Content as Home).finish();
					} else if (ContentFrame.Content is CransPage) {
						(ContentFrame.Content as CransPage).init();
					}else  {
						Logger.info("Ручное перенаправление");
						STARTING = false;
						ContentFrame.Navigate(new Uri("/Home", UriKind.Relative));
					}
				} else {
					Logger.info("ContentFrame null ???");
					//throw new Exception("ContentFrame null ??? ");
				}
				STARTING = false;
			} catch (Exception e) {
				Logger.info("Ошибка в MainPage.finish " + e.ToString() + " " + e.StackTrace);
				try {
					Logger.info("Ручное перенаправление");

					STARTING = false;
					ContentFrame.Content = null;
					Home home = new Home();
					ContentFrame.Content = home;
					//ContentFrame.Navigate(new Uri("/Home", UriKind.Relative));
				} catch (Exception ex) {
					Logger.info("Ошибка при перенаправлении " + ex.ToString());
				}
			}

		}

		// После перехода в фрейме убедиться, что выбрана кнопка HyperlinkButton, представляющая текущую страницу
		private void ContentFrame_Navigated(object sender, NavigationEventArgs e) {
			foreach (UIElement child in LinksStackPanel.Children) {
				HyperlinkButton hb = child as HyperlinkButton;
				if (hb != null && hb.NavigateUri != null) {
					if (hb.NavigateUri.ToString().Equals(e.Uri.ToString())) {
						VisualStateManager.GoToState(hb, "ActiveLink", true);
					} else {
						VisualStateManager.GoToState(hb, "InactiveLink", true);
					}
				}
			}
		}

		// Если во время навигации возникает ошибка, отобразить окно ошибки
		private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
			e.Handled = true;
			/*ChildWindow errorWin = new ErrorWindow(e.Uri);
			errorWin.Show();*/
		}
	}
}