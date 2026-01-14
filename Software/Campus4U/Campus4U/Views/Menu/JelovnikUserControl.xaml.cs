using System.Windows;
using System.Windows.Controls;
using Client.Application.Menu;
using Client.Data.Menu;
using Client.Domain.Menu;

// Tin Posavec

namespace Client.Presentation.Views.Menu
{
    public partial class JelovnikUserControl : UserControl
    {
        private readonly IMenuService _menuService;
        private List<DailyMenu> _jelovnici = new();
        private DailyMenu _odabraniDan = new DailyMenu(0, DateTime.Today, 0, DateTime.Now, new List<Meal>());
        private bool _prikazujeRucak = true;

        public JelovnikUserControl()
        {
            InitializeComponent();

            var repository = new MenuRepository();
            var webScraper = new MenuWebScraper();
            _menuService = new MenuService(repository, webScraper);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UcitajJelovnik();
        }

        private async Task UcitajJelovnik()
        {
            TxtStatus.Visibility = Visibility.Visible;
            ScrollJela.Visibility = Visibility.Collapsed;

            try
            {
                _jelovnici = (await _menuService.DohvatiJelovnikZaTjedan()).ToList();
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Greška pri učitavanju jelovnika.";
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_jelovnici.Count == 0)
            {
                TxtStatus.Text = "Nema dostupnih jelovnika.";
                return;
            }

            NapuniDane();

            var danas = _jelovnici.FirstOrDefault(j => j.Datum.Date == DateTime.Today);
            if (danas == null)
                danas = _jelovnici.First();
            OdaberiDan(danas);

            TxtStatus.Visibility = Visibility.Collapsed;
            ScrollJela.Visibility = Visibility.Visible;
            TxtZadnjeAzuriranje.Text = $"Zadnje ažurirano: {_jelovnici.Max(j => j.ZadnjeAzurirano):dd.MM.yyyy HH:mm}";
        }

        private void NapuniDane()
        {
            PanelDani.Children.Clear();

            foreach (var jelovnik in _jelovnici.OrderBy(x => x.Datum))
            {
                var btn = new Button
                {
                    Content = jelovnik.Datum.ToString("ddd\ndd.MM"),
                    Tag = jelovnik,
                    Width = 70,
                    Height = 50,
                    Margin = new Thickness(4, 0, 4, 0),
                    Style = (Style)FindResource("SecondaryButton")
                };
                btn.Click += BtnDan_Click;
                PanelDani.Children.Add(btn);
            }
        }

        private void OdaberiDan(DailyMenu dan)
        {
            _odabraniDan = dan;

            foreach (Button btn in PanelDani.Children)
            {
                var btnDan = (DailyMenu)btn.Tag;
                btn.Style = (Style)FindResource(btnDan.Datum == dan.Datum ? "PrimaryButton" : "SecondaryButton");
            }

            PrikaziJela();
        }

        private void PrikaziJela()
        {
            var prefix = _prikazujeRucak ? "Ručak" : "Večera";

            var kategorije = _odabraniDan.Jela
                .Where(j => j.Kategorija.StartsWith(prefix))
                .GroupBy(j => j.Kategorija.Replace($"{prefix} - ", ""))
                .Select(g => new KategorijaJela { Kategorija = g.Key, Jela = g.ToList() })
                .ToList();

            ListaJela.ItemsSource = kategorije;
        }

        private void BtnDan_Click(object sender, RoutedEventArgs e)
        {
            OdaberiDan((DailyMenu)((Button)sender).Tag);
        }

        private void BtnRucak_Click(object sender, RoutedEventArgs e)
        {
            _prikazujeRucak = true;
            BtnRucak.Style = (Style)FindResource("PrimaryButton");
            BtnVecera.Style = (Style)FindResource("SecondaryButton");
            PrikaziJela();
        }

        private void BtnVecera_Click(object sender, RoutedEventArgs e)
        {
            _prikazujeRucak = false;
            BtnVecera.Style = (Style)FindResource("PrimaryButton");
            BtnRucak.Style = (Style)FindResource("SecondaryButton");
            PrikaziJela();
        }

        private async void BtnOsvjezi_Click(object sender, RoutedEventArgs e)
        {
            BtnOsvjezi.IsEnabled = false;
            TxtStatus.Text = "Osvježavanje...";
            TxtStatus.Visibility = Visibility.Visible;
            ScrollJela.Visibility = Visibility.Collapsed;

            try
            {
                _jelovnici = (await _menuService.OsvjeziJelovnikSWeba()).ToList();
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Greška pri osvježavanju.";
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                BtnOsvjezi.IsEnabled = true;
                return;
            }

            if (_jelovnici.Count > 0)
            {
                NapuniDane();

                var danas = _jelovnici.FirstOrDefault(j => j.Datum.Date == DateTime.Today);
                if (danas == null)
                    danas = _jelovnici.First();
                OdaberiDan(danas);
                TxtZadnjeAzuriranje.Text = $"Zadnje ažurirano: {DateTime.Now:dd.MM.yyyy HH:mm}";
                TxtStatus.Visibility = Visibility.Collapsed;
                ScrollJela.Visibility = Visibility.Visible;
            }
            else
            {
                TxtStatus.Text = "Nema dostupnih jelovnika.";
            }

            BtnOsvjezi.IsEnabled = true;
        }
    }

    public class KategorijaJela
    {
        public string Kategorija { get; set; } = "";
        public List<Meal> Jela { get; set; } = new();
    }
}
