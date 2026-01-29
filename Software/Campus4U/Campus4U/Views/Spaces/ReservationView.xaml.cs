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
using Client.Domain.Spaces;

namespace Client.Presentation.Views.Spaces
{
    /// <summary>
    /// Interaction logic for ReservationView.xaml
    /// </summary>
    public partial class ReservationView : Window
    {
        public Space TrenutniProstor { get; set; }
        public ReservationView(Space prostor)
        {
            InitializeComponent();
            TrenutniProstor = prostor;
            this.DataContext = TrenutniProstor;
        }

        public void BtnNatrag_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void BtnRezerviraj_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            int trenutno = int.Parse(TxtBrojOsoba.Text);
            if (trenutno > 1) TxtBrojOsoba.Text = (trenutno - 1).ToString();
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            int trenutno = int.Parse(TxtBrojOsoba.Text);
            TxtBrojOsoba.Text = (trenutno + 1).ToString();
        }
    }
}
