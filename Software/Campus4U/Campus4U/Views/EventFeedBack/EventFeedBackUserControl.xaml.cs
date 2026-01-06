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

        public int KorisnikId { get; set; }

        public int DogadajId { get; set; }

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
                    false,
                    DogadajId,
                    KorisnikId);

                var uspjeh = _servis.Dodaj(komentar);
                if (!uspjeh)
                {
                    MessageBox.Show("Spremanje komentara nije uspjelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await PonovoUcitajKomentareAsync(DogadajId);
                txtKomentar.Text = string.Empty;
                cboOcjena.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška kod spremanja komentara: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task PonovoUcitajKomentareAsync(int dogadajId)
        {
            var komentari = await _servis.DohatiSve(dogadajId, KorisnikId);
            Comments.Clear();
            foreach (var komentar in komentari)
            {
                Comments.Add(komentar);
            }
        }

        private async void btnObrisiKomentar_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var komentar = (EventFeedbackComment)button.DataContext;

            var potvrda = MessageBox.Show("Jeste li sigurni da želite obrisati ovaj komentar?", "Potvrda brisanja",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (potvrda != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                var uspjeh = _servis.Obrisi(komentar.Id);
                if (!uspjeh)
                {
                    MessageBox.Show("Brisanje komentara nije uspjelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await PonovoUcitajKomentareAsync(DogadajId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška kod brisanja komentara: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
