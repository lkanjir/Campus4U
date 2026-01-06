using System;
using System.Threading.Tasks;
using System.Windows;
using Client.Application.EventFeedBack;
using Client.Data.EventFeedBack;

namespace Client.Presentation.Views.EventFeedBack
{
    public partial class SimulacijaDogadajaKomentarWindow : Window
    {
        private readonly IEventFeedBackService _servis;
        private readonly int _korisnikId;

        public SimulacijaDogadajaKomentarWindow(int idKorisnika = 0)
        {
            InitializeComponent();

            _servis = new EventFeedBackService(new RepositoryEventFeedBack());
            _korisnikId = idKorisnika;
            EventFeedbackControl.DogadajId = 1;
            EventFeedbackControl.KorisnikId = _korisnikId;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await UcitajKomentareAsync(EventFeedbackControl.DogadajId);
        }

        private async Task UcitajKomentareAsync(int dogadajId)
        {
            try
            {
                var komentari = await _servis.DohatiSve(dogadajId, _korisnikId);
                EventFeedbackControl.Comments.Clear();
                foreach (var komentar in komentari)
                {
                    EventFeedbackControl.Comments.Add(komentar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greska kod dohvaæanja komentara: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
