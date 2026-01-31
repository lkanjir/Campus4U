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

namespace Client.Presentation.Views.Spaces
{
    /// <summary>
    /// Interaction logic for MyReservationView.xaml
    /// </summary>
    public partial class MyReservationView : Window
    {
        public MyReservationView(string korisnikSub)
        {
            InitializeComponent();
        }

        private async void FiltrirajRezervacije_Click(object sender, RoutedEventArgs e)
        {
            // Implement filtering logic here
        }
    }
}
