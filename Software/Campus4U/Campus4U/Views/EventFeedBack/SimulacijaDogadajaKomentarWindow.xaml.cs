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

        public SimulacijaDogadajaKomentarWindow()
        {
            InitializeComponent();

            _servis = new EventFeedBackService(new RepositoryEventFeedBack());
            EventFeedbackControl.DogadajId = 1;
            EventFeedbackControl.KorisnikId = 6;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadCommentsAsync(EventFeedbackControl.DogadajId);
        }

        private async Task LoadCommentsAsync(int dogadajId)
        {
            try
            {
                var komentari = await _servis.DohatiSve(dogadajId);
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
