using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Client.Application.EventFeedBack;
using Client.Data.EventFeedBack;

namespace Client.Presentation.Views.EventFeedBack
{
    public partial class EventFeedBackUserControl : UserControl
    {
        private readonly IEventFeedBackService _servis;

        public ObservableCollection<EventFeedbackComment> Comments { get; } = new();

        public int DogadajId { get; set; } = 1;
        public int KorisnikId { get; set; } = 6;

        public EventFeedBackUserControl()
        {
            InitializeComponent();

            _servis = new EventFeedBackService(new RepositoryEventFeedBack());
        }

        private async void btnDodajKomentar_Click(object sender, RoutedEventArgs e)
        {
            var tekstKomentara = txtKomentar.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tekstKomentara))
            {
                MessageBox.Show("Komentar ne smije biti prazan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cboOcjena.SelectedItem is not ComboBoxItem odabranaOcjena)
            {
                MessageBox.Show("Odaberite ocjenu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int ocjena = int.Parse(odabranaOcjena.Content.ToString() ?? "0");
                var komentar = new EventFeedbackComment(
                    0,
                    DateTime.Now,
                    ocjena,
                    tekstKomentara,
                    string.Empty,
                    DogadajId,
                    KorisnikId);

                var uspjeh = _servis.Dodaj(komentar);
                if (!uspjeh)
                {
                    MessageBox.Show("Spremanje komentara nije uspjelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await ReloadCommentsAsync(DogadajId);
                txtKomentar.Text = string.Empty;
                cboOcjena.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška kod spremanja komentara: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ReloadCommentsAsync(int dogadajId)
        {
            var komentari = await _servis.DohatiSve(dogadajId);
            Comments.Clear();
            foreach (var komentar in komentari)
            {
                Comments.Add(komentar);
            }
        }
    }
}
