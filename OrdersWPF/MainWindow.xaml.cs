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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VotGESOrders;
using VotGESOrders.Views;
using VotGESOrders.AdditService;
using VotGESOrders.OrdersService;


namespace OrdersWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            OrdersClientContext.init();
            OrdersClientContext.load();
            InitializeComponent();
            //MessageBox.Show(OrdersClientContext.Current.CurrentOrders.Count.ToString());
            LoginName.DataContext = OrdersClientContext.Current.CurrentUser;
            LinkEditTree.Visibility = OrdersClientContext.Current.CurrentUser.AllowEditTree ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            LinkEditUsers.Visibility = OrdersClientContext.Current.CurrentUser.AllowEditUsers ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Logger.init("out", "test");

            MessageBox.Show(OrdersClientContext.Current.CurrentOrders.Count.ToString());
        }

        private void Link1_Click(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            HomeControl homeControl = new HomeControl();
            ContentGrid.Children.Add(homeControl);
            homeControl.InitializeComponent();
            homeControl.LoadControl();
        }
    }
}
