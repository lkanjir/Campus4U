using Client.Application.Fault;
using Client.Domain.Fault;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// Tin Posavec

namespace Client.Presentation.Views.Fault
{
    public partial class DetaljiKvaraWindow : Window
    {
        private readonly FaultReport _kvar;
        private readonly IFaultService _faultService;
        private string _trenutniStatus;

        public event EventHandler? StatusPromijenjen;

        public DetaljiKvaraWindow(FaultReport kvar, IFaultService faultService)
        {
            InitializeComponent();

            _kvar = kvar;
            _faultService = faultService;
            _trenutniStatus = kvar.Status;

            PrikaziPodatke();
            UcitajDozvoljeneStatuse();
        }

        private void PrikaziPodatke()
        {
            TxtNaslov.Text = $"Detalji kvara #{_kvar.KvarId}";
            TxtProstor.Text = _kvar.ProstorNaziv ?? "-";
            TxtVrstaKvara.Text = _kvar.VrstaKvaraNaziv ?? "-";
            TxtDatumPrijave.Text = _kvar.DatumPrijave.ToString("d. MMMM yyyy., HH:mm");
            TxtPrijavio.Text = _kvar.KorisnikImePrezime ?? "-";
            TxtOpis.Text = _kvar.Opis;

            
            TxtStatus.Text = _kvar.Status;
            PostaviStatusBoju(_kvar.Status);

          
            if (_kvar.Fotografija != null && _kvar.Fotografija.Length > 0)
            {
                BtnFotografija.Visibility = Visibility.Visible;
            }
        }

        private void PostaviStatusBoju(string status)
        {
            switch (status)
            {
                case "Aktivan":
                    BorderStatus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF3C7"));
                    TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#92400E"));
                    break;
                case "U obradi":
                    BorderStatus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DBEAFE"));
                    TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E40AF"));
                    break;
                case "Riješen":
                    BorderStatus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1FAE5"));
                    TxtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#065F46"));
                    break;
                default:
                    BorderStatus.Background = new SolidColorBrush(Colors.LightGray);
                    TxtStatus.Foreground = new SolidColorBrush(Colors.Black);
                    break;
            }
        }

        private void UcitajDozvoljeneStatuse()
        {
            CmbNoviStatus.ItemsSource = null;
            CmbNoviStatus.Items.Clear();

            var dozvoljeniStatusi = _faultService.DohvatiDozvoljeneStatuse(_trenutniStatus);

            if (dozvoljeniStatusi.Count == 0)
            {
                CmbNoviStatus.IsEnabled = false;
                BtnSpremi.IsEnabled = false;
                CmbNoviStatus.Items.Add("Nema dostupnih prijelaza");
                CmbNoviStatus.SelectedIndex = 0;
            }
            else
            {
                CmbNoviStatus.IsEnabled = true;
                BtnSpremi.IsEnabled = true;
                CmbNoviStatus.ItemsSource = dozvoljeniStatusi;
                CmbNoviStatus.SelectedIndex = 0;
            }
        }

        private async void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
            if (CmbNoviStatus.SelectedItem is not string noviStatus)
            {
                PrikaziPoruku("Odaberi novi status.", true);
                return;
            }

            try
            {
                BtnSpremi.IsEnabled = false;
                PrikaziPoruku("Spremanje...", false);

                var uspjeh = await _faultService.PromijeniStatusKvara(_kvar.KvarId, noviStatus);

                if (uspjeh)
                {
                    _trenutniStatus = noviStatus;
                    TxtStatus.Text = noviStatus;
                    PostaviStatusBoju(noviStatus);
                    UcitajDozvoljeneStatuse();
                    StatusPromijenjen?.Invoke(this, EventArgs.Empty);
                    PrikaziPoruku("Status uspješno promijenjen!", false);
                }
                else
                {
                    PrikaziPoruku("Greška kod spremanja.", true);
                    BtnSpremi.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                PrikaziPoruku($"Greška: {ex.Message}", true);
                BtnSpremi.IsEnabled = true;
            }
        }

        private void PrikaziPoruku(string poruka, bool greska)
        {
            TxtPoruka.Text = poruka;
            TxtPoruka.Foreground = greska
                ? new SolidColorBrush(Colors.Red)
                : (Brush)FindResource("BrushMuted");
            TxtPoruka.Visibility = Visibility.Visible;
        }

        private void BtnFotografija_Click(object sender, RoutedEventArgs e)
        {
            if (_kvar.Fotografija == null || _kvar.Fotografija.Length == 0)
                return;

            try
            {
                var window = new Window
                {
                    Title = "Fotografija kvara",
                    Width = 600,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this
                };

                var bitmap = new BitmapImage();
                using (var stream = new MemoryStream(_kvar.Fotografija))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }

                var image = new System.Windows.Controls.Image
                {
                    Source = bitmap,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(10)
                };

                window.Content = image;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška kod prikaza fotografije: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
