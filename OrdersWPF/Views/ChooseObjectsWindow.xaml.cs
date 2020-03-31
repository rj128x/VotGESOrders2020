using System;
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
    /// Логика взаимодействия для ChooseObjectsWindow.xaml
    /// </summary>
    public partial class ChooseObjectsWindow : Window
    {
        public OrderFilter CurrentFilter { get; set; }
        protected List<OrderObject> prevSel;
		public ChooseObjectsWindow()
		{
			InitializeComponent();
			treeObjects.ItemsSource = from OrderObject o in OrdersClientContext.Current.AllOrderObjects where o.ObjectID == 0 select o;

		}
	

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			CurrentFilter.SelectedObjects.Clear();
			foreach (OrderObject obj in prevSel)
			{
				CurrentFilter.SelectedObjects.Add(obj);
			}
			this.DialogResult = false;
		}

		private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			OrderObject obj = lstSelectedObjects.SelectedItem as OrderObject;
			if (obj != null)
			{
				CurrentFilter.SelectedObjects.Remove(obj);
			}
		}

		private void treeObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			OrderObject obj = treeObjects.SelectedItem as OrderObject;
			if ((obj != null) && (!CurrentFilter.SelectedObjects.Contains(obj)))
			{
				CurrentFilter.SelectedObjects.Add(obj);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LayoutRoot.DataContext = CurrentFilter;
			prevSel = CurrentFilter.SelectedObjects.ToList();
		}
	}
}
