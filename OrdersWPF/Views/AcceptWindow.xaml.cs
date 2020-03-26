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
    /// Логика взаимодействия для AcceptWindow.xaml
    /// </summary>
    public partial class AcceptWindow : Window
    {
        public Order CurrentOrder { get; set; }
        public AcceptResult Result { get; set; }
        public bool isCancelWindow { get; set; }
        public bool isCommentWindow { get; set; }
        public AcceptWindow()
        {
            InitializeComponent();
        }
    }
}
