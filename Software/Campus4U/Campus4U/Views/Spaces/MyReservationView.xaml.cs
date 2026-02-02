using Client.Data.Spaces;
using Client.Domain.Spaces;
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
using Client.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Presentation.Views.Spaces
{
    /// <summary>
    /// Interaction logic for MyReservationView.xaml
    /// </summary>
    public partial class MyReservationView : Window
    {
        private readonly ReservationRepository repo = new ReservationRepository();
        private readonly string _korisnikSub;
        private int korisnikId;
        private List<Rezervacija> sveRezervacije = new();
        public MyReservationView(string korisnikSub)
        {
            InitializeComponent();
            _korisnikSub = korisnikSub;
            Loaded += MyReservation_Loaded;
        }

        private async void MyReservation_Loaded(object sender, RoutedEventArgs e)
        {
            korisnikId = await DohvatiIdKorisnikaPoSub(_korisnikSub);
            sveRezervacije = await repo.DohvatiRezervacijeKorisnika(korisnikId);
            FiltrirajRezervacije_Click(null, null);
        }

        private async Task<int> DohvatiIdKorisnikaPoSub(string sub)
        {
            await using var db = new Campus4UContext();

            var korisnik = await db.Korisnici
                .Where(k => k.Sub == sub)
                .FirstOrDefaultAsync();

            if (korisnik == null)
            {
                throw new Exception("Korisnik nije pronađen.");
            }

            return korisnik.Id;
        }

        private void FiltrirajRezervacije_Click(object sender, RoutedEventArgs e)
        {
            var sada = DateTime.Now;

            IEnumerable<Rezervacija> filtrirane;

            if (BtnOtkazano.IsChecked == true)
            {
                filtrirane = sveRezervacije
                    .Where(r => r.Status == "Otkazano");
            }
            else if (BtnProslo.IsChecked == true)
            {
                filtrirane = sveRezervacije
                    .Where(r => r.Status != "Otkazano" && r.KrajnjeVrijeme < sada);
            }
            else
            {
                filtrirane = sveRezervacije
                    .Where(r => r.Status != "Otkazano" && r.KrajnjeVrijeme >= sada);
            }

            ReservationsItemsControl.ItemsSource = filtrirane
                .OrderBy(r => r.PocetnoVrijeme)
                .ToList();
        }

        private async void BtnOtkaziRezervaciju_Click(object sender, RoutedEventArgs e)
        {
            var odgovor = MessageBox.Show("Jeste li sigurni da želite otkazati ovu rezervaciju?", "Potvrda otkazivanja", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (odgovor != MessageBoxResult.Yes)
            {
                return;
            }

            if (sender is Button button && button.DataContext is Rezervacija rezervacija)
            {
                await repo.OtkaziRezervaciju(rezervacija.ID);
                sveRezervacije = await repo.DohvatiRezervacijeKorisnika(korisnikId);
                FiltrirajRezervacije_Click(null, null);
            }
        }

        private async void BtnUrediRezervaciju_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
