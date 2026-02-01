using Client.Application.Favorites;
using Client.Data.Favorites;
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

namespace Client.Presentation.Views.Spaces
{
    /// <summary>
    /// Interaction logic for ReservationView.xaml
    /// </summary>
    /// Marko Mišić
    public partial class ReservationView : Window
    {
        public Space TrenutniProstor { get; set; }
        public readonly ReservationRepository reservationRepository = new ReservationRepository();
        private readonly ISpacesFavoritesService _favoritesService;
        private int idKorisnika;
        public ReservationView(Space prostor, int idKorisnika)
        {
            InitializeComponent();
            _favoritesService = new SpacesFavoritesService(new SpacesFavoritesRepository());
            PopuniVremena();
            TrenutniProstor = prostor;
            this.DataContext = TrenutniProstor;
            this.idKorisnika = idKorisnika;
            Loaded += ReservationView_Loaded;
        }

        private void PopuniVremena()
        {
            var vremena = new List<string>();

            for (int h = 0; h < 24; h++)
            {
                vremena.Add($"{h:00}:00");
                vremena.Add($"{h:00}:15");
            }

            odVrijemeCombo.ItemsSource = vremena;
            doVrijemeCombo.ItemsSource = vremena;

            odVrijemeCombo.SelectedItem = "09:00";
            doVrijemeCombo.SelectedItem = "10:00";
        }


        public void BtnNatrag_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public async void BtnRezerviraj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Datum.SelectedDate == null)
                {
                    throw new Exception("Morate odabrati datum");
                }

                DateTime datum = Datum.SelectedDate.Value;
                TimeSpan odVrijeme = TimeSpan.Parse((string)odVrijemeCombo.SelectedItem);
                TimeSpan doVrijeme = TimeSpan.Parse((string)doVrijemeCombo.SelectedItem);

                DateTime pocetak = datum.Add(odVrijeme);
                DateTime kraj = datum.Add(doVrijeme);

                if (pocetak >= kraj)
                {
                    throw new Exception("Krajnje vrijeme mora biti nakon početnog vremena");
                }

                var trajanje = kraj - pocetak;
                if (trajanje > TimeSpan.FromHours(3))
                {
                    throw new Exception("Rezervacija ne smije trajati duže od 3 sata.");
                }

                if (pocetak < DateTime.Now.AddMinutes(5))
                {
                    throw new Exception("Rezervaciju morate napraviti najkasnije 5 minuta prije početka i ne može biti u prošlosti.");
                }

                int brojOsoba = int.Parse(TxtBrojOsoba.Text);
                int zauzeto = await reservationRepository.DohvatiZauzetoMjesta(TrenutniProstor.ProstorId, pocetak, kraj);
                int slobodno = TrenutniProstor.Kapacitet - zauzeto;

                if (brojOsoba > slobodno)
                {
                    throw new Exception("Nema dovoljno slobodnih mjesta za odabrani termin.");
                }

                Rezervacija novaRezervacija = new Rezervacija
                (
                    0,
                    TrenutniProstor,
                    idKorisnika,
                    pocetak,
                    kraj,
                    "Aktivno",
                    brojOsoba,
                    DateTime.Now
                );

                await reservationRepository.SpremiRezervaciju(novaRezervacija);
                MessageBox.Show("Uspješno ste rezervirali prostor.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška pri rezervaciji", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            int trenutno = int.Parse(TxtBrojOsoba.Text);
            if (trenutno > 1) TxtBrojOsoba.Text = (trenutno - 1).ToString();
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            int trenutno = int.Parse(TxtBrojOsoba.Text);

            if (trenutno >=5)
            {
                return;
            }

            TxtBrojOsoba.Text = (trenutno + 1).ToString();
        }

        //Nikola Kihas
        private async void BtnDodajUFavorite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dodano = await _favoritesService.DodajFavoritaProstorijeAsync(idKorisnika, TrenutniProstor.ProstorId);
                PostaviVidljivostFavorita(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška pri dodavanju u favorite", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnUkloniIzFavorita_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uklonjeno = await _favoritesService.UkloniFavoritaProstorijeAsync(idKorisnika, TrenutniProstor.ProstorId);
                PostaviVidljivostFavorita(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška pri uklanjanju iz favorita", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ReservationView_Loaded(object sender, RoutedEventArgs e)
        {
            await OsvjeziFavoriteGumbeAsync();
        }

        private async Task OsvjeziFavoriteGumbeAsync()
        {
            try
            {
                if (TrenutniProstor is null)
                {
                    PostaviVidljivostFavorita(false);
                    return;
                }

                var favoriti = await _favoritesService.DohvatiFavoriteKorisnikaAsync(idKorisnika);
                var jeFavorit = favoriti.Any(p => p.ProstorId == TrenutniProstor.ProstorId);
                PostaviVidljivostFavorita(jeFavorit);
            }
            catch
            {
                PostaviVidljivostFavorita(false);
            }
        }

        private void PostaviVidljivostFavorita(bool jeFavorit)
        {
            if (BtnDodajUFavorite is not null)
            {
                BtnDodajUFavorite.Visibility = jeFavorit ? Visibility.Collapsed : Visibility.Visible;
            }

            if (BtnUkloniIzFavorita is not null)
            {
                BtnUkloniIzFavorita.Visibility = jeFavorit ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
