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
            try
            {
                if (Datum.SelectedDate == null)
                {
                    throw new Exception("Morate odabrati datum");
                }

                DateTime datum = Datum.SelectedDate.Value;
                TimeSpan odVrijeme = TimeSpan.Parse(odVrijemeText.Text);
                TimeSpan doVrijeme = TimeSpan.Parse(odVrijemeText.Text);

                DateTime pocetak = datum.Add(odVrijeme);
                DateTime kraj = datum.Add(doVrijeme);

                if (pocetak >= kraj)
                {
                    throw new Exception("Krajnje vrijeme mora biti nakon početnog vremena");
                }

                if (pocetak < DateTime.Now.AddMinutes(5))
                {
                    throw new Exception("Rezervaciju morate napraviti najkasnije 5 minuta prije početka i ne može biti u prošlosti.");
                }

                int brojOsoba = int.Parse(TxtBrojOsoba.Text);

                

                Rezervacija novaRezervacija = new Rezervacija
                (
                    0,
                    TrenutniProstor,
                    9,
                    pocetak,
                    kraj,
                    "Aktivno"
                );

                reservationRepository.SpremiRezervaciju(novaRezervacija);
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
