using Client.Application.Fault;
using Client.Application.Spaces;
using Client.Data.Fault;
using Client.Data.Spaces;
using Client.Domain.Fault;
using Client.Domain.Spaces;
using Client.Presentation.Services;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

//Tin Posavec

namespace Client.Presentation.Views.Fault
{
    public partial class UpravljanjeKvarovimaUserControl : UserControl
    {
        private readonly IFaultService _faultService;
        private List<FaultReport> _kvarovi = new();
        private List<Space> _prostori = new();
        private List<FaultType> _vrsteKvarova = new();
        private bool _isLoading = true;

        public UpravljanjeKvarovimaUserControl()
        {
            InitializeComponent();

            var faultRepo = new FaultRepository();
            var spaceRepo = new SpaceRepository();
            _faultService = new FaultService(faultRepo, spaceRepo);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UcitajPodatke();
        }

        private async Task UcitajPodatke()
        {
            try
            {
                PrikaziStatus("Učitavanje...");

                var statusi = _faultService.DohvatiSveStatuse();
                _prostori = await _faultService.DohvatiProstore();
                _vrsteKvarova = await _faultService.DohvatiVrsteKvarova();
                _kvarovi = await _faultService.DohvatiSveKvarove();

                CmbStatus.ItemsSource = statusi;
                CmbStatus.SelectedIndex = -1;
                CmbStatus.Text = "Odaberi status...";

                CmbProstor.ItemsSource = _prostori;
                CmbProstor.SelectedIndex = -1;
                CmbProstor.Text = "Odaberi prostor...";

                CmbVrstaKvara.ItemsSource = _vrsteKvarova;
                CmbVrstaKvara.SelectedIndex = -1;
                CmbVrstaKvara.Text = "Odaberi vrstu...";

                PrikaziKvarove(_kvarovi);
                SakrijStatus();
                _isLoading = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                PrikaziStatus($"Greška: {ex.Message}");
                _isLoading = false;
            }
        }

        private void PrikaziKvarove(List<FaultReport> kvarovi)
        {
            DgKvarovi.ItemsSource = kvarovi;
            TxtBrojKvarova.Text = $"Prikazano {kvarovi.Count} kvarova";

            if (kvarovi.Count == 0)
            {
                PrikaziStatus("Nema kvarova za prikaz.");
            }
            else
            {
                SakrijStatus();
            }
        }

        private void PrikaziStatus(string poruka)
        {
            TxtStatus.Text = poruka;
            TxtStatus.Visibility = Visibility.Visible;
        }

        private void SakrijStatus()
        {
            TxtStatus.Visibility = Visibility.Collapsed;
        }

        private async void BtnOsvjezi_Click(object sender, RoutedEventArgs e)
        {
            await UcitajPodatke();
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stats = await _faultService.DohvatiStatistiku();

                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF datoteke (*.pdf)|*.pdf",
                    DefaultExt = ".pdf",
                    FileName = $"Statistika_kvarova_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    PrikaziStatus("Generiranje PDF-a...");
                    var logoBytes = FaultStatisticsPdfExporter.LoadLogoFromResources();
                    await Task.Run(() => FaultStatisticsPdfExporter.Export(stats, saveDialog.FileName, logoBytes));
                    SakrijStatus();

                    var result = MessageBox.Show(
                        "PDF uspješno generiran!\n\nŽelite li otvoriti datoteku?",
                        "Uspjeh",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška kod exporta: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                SakrijStatus();
            }
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoading)
            {
                _ = PrimjeniFiltere();
            }
        }

        private void Filter_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoading)
            {
                _ = PrimjeniFiltere();
            }
        }

        private async Task PrimjeniFiltere()
        {
            try
            {
                string? status = CmbStatus.SelectedItem as string;
                int? prostorId = (CmbProstor.SelectedItem as Space)?.ProstorId;
                int? vrstaKvaraId = (CmbVrstaKvara.SelectedItem as FaultType)?.VrstaKvaraId;
                DateTime? odDatuma = DpOd.SelectedDate;
                DateTime? doDatuma = DpDo.SelectedDate;

                var filtriraniKvarovi = await _faultService.DohvatiKvaroveFiltrirano(
                    status, prostorId, vrstaKvaraId, odDatuma, doDatuma);

                PrikaziKvarove(filtriraniKvarovi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška kod filtriranja: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                SakrijStatus();
            }
        }

        private async void BtnPonisti_Click(object sender, RoutedEventArgs e)
        {
            CmbStatus.SelectedIndex = -1;
            CmbStatus.Text = "Odaberi status...";
            CmbProstor.SelectedIndex = -1;
            CmbProstor.Text = "Odaberi prostor...";
            CmbVrstaKvara.SelectedIndex = -1;
            CmbVrstaKvara.Text = "Odaberi vrstu...";
            DpOd.SelectedDate = null;
            DpDo.SelectedDate = null;

            _kvarovi = await _faultService.DohvatiSveKvarove();
            PrikaziKvarove(_kvarovi);
        }

        private void DgKvarovi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgKvarovi.SelectedItem is FaultReport kvar)
            {
                OtvoriDetaljeKvara(kvar.KvarId);
            }
        }

        private void BtnAkcija_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.IsOpen = true;
            }
        }

        private void MenuDetalji_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu &&
                    contextMenu.PlacementTarget is Button btn &&
                    btn.Tag is int kvarId)
                {
                    OtvoriDetaljeKvara(kvarId);
                }
            }
        }

        private async void OtvoriDetaljeKvara(int kvarId)
        {
            try
            {
                var kvar = await _faultService.DohvatiKvarPoId(kvarId);
                if (kvar == null)
                {
                    MessageBox.Show("Kvar nije pronađen.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var detaljiWindow = new DetaljiKvaraWindow(kvar, _faultService);
                detaljiWindow.Owner = Window.GetWindow(this);
                detaljiWindow.StatusPromijenjen += async (s, args) =>
                {
                    await PrimjeniFiltere();
                };
                detaljiWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška kod otvaranja detalja: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
