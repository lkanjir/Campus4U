using Client.Data.Spaces;
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
using Client.Presentation.Views.Spaces;
using Client.Domain.Spaces;

namespace Client.Presentation.Views.Spaces
{
    /// <summary>
    /// Interaction logic for SpacesView.xaml
    /// </summary>
    /// 
    //Marko Mišić
    public partial class SpacesView : Window
    {
        private readonly SpaceRepository prostorRepo;
        private readonly TipProstora tipProstora;
        private int KorisnikID;
        
        public SpacesView(TipProstora tip, int korisnikID)
        {
            InitializeComponent();
            prostorRepo = new SpaceRepository();
            Loaded += SpacesView_Loaded;
            tipProstora = tip;
            this.KorisnikID = korisnikID;
        }

        private async void SpacesView_Loaded(object sender, RoutedEventArgs e)
        {
            GridProstori.ItemsSource = await prostorRepo.DohvatiPoTipu(tipProstora);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnRezerviraj_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button gumb)
            {
                return;
            }
            if (gumb.CommandParameter is not Space prostor)
            {
                return;
            }

            var pogled = new ReservationView(prostor, this.KorisnikID)
            {
                Owner = this
            };

            pogled.ShowDialog();
        }
    }
}
