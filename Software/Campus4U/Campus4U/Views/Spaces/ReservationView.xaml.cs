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
using Client.Data.Spaces;
using Client.Domain.Spaces;

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
        private int idKorisnika;
        public ReservationView(Space prostor, int idKorisnika)
        {
            InitializeComponent();
            PopuniVremena();
            TrenutniProstor = prostor;
            this.DataContext = TrenutniProstor;
            this.idKorisnika = idKorisnika;
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
            TxtBrojOsoba.Text = (trenutno + 1).ToString();
        }
    }
}
