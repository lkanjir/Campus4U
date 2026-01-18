using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Client.Application.Fault;
using Client.Data.Fault;
using Client.Data.Spaces;
using Client.Domain.Fault;
using Client.Domain.Spaces;
using Microsoft.Win32;

// Tin Posavec

namespace Client.Presentation.Views.Fault
{
    public partial class PrijavaKvaraUserControl : UserControl
    {
        private readonly IFaultService _faultService;
        private byte[]? _fotografija = null;
        private int _korisnikId;

        public event EventHandler? PovratakNaPocetni;

        public PrijavaKvaraUserControl()
        {
            InitializeComponent();

            var faultRepo = new FaultRepository();
            var spaceRepo = new SpaceRepository();
            _faultService = new FaultService(faultRepo, spaceRepo);
        }

        // Postavi korisnika koristim u glavnoj main formi da proslijedim korisnika ovoj... kad se napravi navigacija koristi se ta metoda
        public void PostaviKorisnika(int korisnikId)
        {
            _korisnikId = korisnikId;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UcitajPodatke();
        }

        private async Task UcitajPodatke()
        {
            TxtUcitavanje.Visibility = Visibility.Visible;
            CmbProstor.IsEnabled = false;
            CmbVrstaKvara.IsEnabled = false;

            try
            {
                var prostori = await _faultService.DohvatiProstore();
                var vrsteKvarova = await _faultService.DohvatiVrsteKvarova();

                CmbProstor.ItemsSource = prostori;
                CmbVrstaKvara.ItemsSource = vrsteKvarova;

                if (prostori.Count > 0)
                    CmbProstor.SelectedIndex = 0;

                if (vrsteKvarova.Count > 0)
                    CmbVrstaKvara.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                TxtUcitavanje.Visibility = Visibility.Collapsed;
                CmbProstor.IsEnabled = true;
                CmbVrstaKvara.IsEnabled = true;
            }
        }

        private void BtnDodajSliku_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Slike|*.jpg;*.jpeg;*.png;*.bmp|Sve datoteke|*.*",
                Title = "Odaberi fotografiju kvara"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var fileInfo = new FileInfo(dialog.FileName);
                    const long maxVelicina = 5 * 1024 * 1024; // 5 MB

                    if (fileInfo.Length > maxVelicina)
                    {
                        MessageBox.Show("Slika je prevelika. Maksimalna veličina je 5 MB.",
                            "Prevelika datoteka", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _fotografija = File.ReadAllBytes(dialog.FileName);

                    var bitmap = new BitmapImage();
                    using (var stream = new MemoryStream(_fotografija))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }

                    ImgPreview.Source = bitmap;
                    BorderSlika.Visibility = Visibility.Visible;
                    BtnUkloniSliku.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri učitavanju slike: {ex.Message}",
                        "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnUkloniSliku_Click(object sender, RoutedEventArgs e)
        {
            _fotografija = null;
            ImgPreview.Source = null;
            BorderSlika.Visibility = Visibility.Collapsed;
            BtnUkloniSliku.Visibility = Visibility.Collapsed;
        }

        private async void BtnPosalji_Click(object sender, RoutedEventArgs e)
        {
            if (CmbProstor.SelectedItem == null)
            {
                PrikaziStatus("Odaberi prostor.", true);
                return;
            }

            if (CmbVrstaKvara.SelectedItem == null)
            {
                PrikaziStatus("Odaberi vrstu kvara.", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtOpis.Text))
            {
                PrikaziStatus("Unesi opis problema.", true);
                return;
            }

            if (_korisnikId <= 0)
            {
                PrikaziStatus("Korisnik nije prijavljen.", true);
                return;
            }

            var prostor = CmbProstor.SelectedItem as Space;
            var vrstaKvara = CmbVrstaKvara.SelectedItem as FaultType;

            if (prostor == null || vrstaKvara == null)
            {
                PrikaziStatus("Greška pri odabiru.", true);
                return;
            }

            BtnPosalji.IsEnabled = false;
            PrikaziStatus("Slanje prijave...", false);

            try
            {
                var uspjeh = await _faultService.PrijaviKvar(
                    prostor.ProstorId,
                    vrstaKvara.VrstaKvaraId,
                    TxtOpis.Text.Trim(),
                    _fotografija,
                    _korisnikId
                );

                if (uspjeh)
                {
                    OcistiFormu();
                    PrikaziUspjeh();
                }
                else
                {
                    PrikaziStatus("Greška pri slanju prijave.", true);
                }
            }
            catch (Exception ex)
            {
                PrikaziStatus($"Greška: {ex.Message}", true);
            }
            finally
            {
                BtnPosalji.IsEnabled = true;
            }
        }

        private void PrikaziStatus(string poruka, bool greska)
        {
            TxtStatus.Text = poruka;

            if (greska)
                TxtStatus.Foreground = Brushes.Red;
            else
                TxtStatus.Foreground = (Brush)FindResource("BrushMuted");

        }

        private void OcistiFormu()
        {
            CmbProstor.SelectedIndex = 0;
            CmbVrstaKvara.SelectedIndex = 0;
            TxtOpis.Text = "";
            _fotografija = null;
            ImgPreview.Source = null;
            BorderSlika.Visibility = Visibility.Collapsed;
            BtnUkloniSliku.Visibility = Visibility.Collapsed;
            TxtStatus.Text = "";
        }

        private void PrikaziUspjeh()
        {
            PanelForma.Visibility = Visibility.Collapsed;
            PanelUspjeh.Visibility = Visibility.Visible;
        }

        private void BtnNovaPrijava_Click(object sender, RoutedEventArgs e)
        {
            PanelUspjeh.Visibility = Visibility.Collapsed;
            PanelForma.Visibility = Visibility.Visible;
        }

        private void BtnPovratak_Click(object sender, RoutedEventArgs e)
        {
            // Tu treba staviti kad se napravi početni ekran da se na to vraća
            PanelUspjeh.Visibility = Visibility.Collapsed;
            PanelForma.Visibility = Visibility.Visible;

            PovratakNaPocetni?.Invoke(this, EventArgs.Empty);
        }
    }
}
