using System;
using System.Windows;
using System.Windows.Controls;
using Client.Application.EventFeedBack;
using Client.Data.EventFeedBack;

/// Nikola Kihas
namespace Client.Presentation.Views.EventFeedBack
{
    public partial class UpdateEventFeedBackWindow : Window
    {
        private readonly IEventFeedBackService _servis;
        private readonly EventFeedbackComment _komentar;

        public UpdateEventFeedBackWindow(EventFeedbackComment komentar)
        {
            InitializeComponent();

            _servis = new EventFeedBackService(new RepositoryEventFeedBack());
            _komentar = komentar;

            txtKomentar.Text = _komentar.Komentar;
            if (_komentar.Ocjena >= 1 && _komentar.Ocjena <= 5)
            {
                cboOcjena.SelectedIndex = _komentar.Ocjena - 1;
            }
        }

        private void btnOdustani_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnUpdateKomentar_Click(object sender, RoutedEventArgs e)
        {
            if (cboOcjena.SelectedItem is not ComboBoxItem odabranaOcjena)
            {
                MessageBox.Show("Odaberite ocjenu.", "Greska", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int ocjena = int.Parse(odabranaOcjena.Content.ToString() ?? "0");

            var tekstKomentara = txtKomentar.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tekstKomentara))
            {
                MessageBox.Show("Komentar ne smije biti prazan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updated = new EventFeedbackComment(
                _komentar.Id,
                DateTime.Now,
                ocjena,
                tekstKomentara,
                _komentar.ImePrezime,
                _komentar.MojKomentar,
                _komentar.DogadajId,
                _komentar.KorisnikId);

            var uspjeh = _servis.Uredi(updated);
            if (!uspjeh)
            {
                MessageBox.Show("Auriranje komentara nije uspjelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}