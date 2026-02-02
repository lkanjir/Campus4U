using Client.Data.Spaces;
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
using System.Windows.Shapes;
using Client.Presentation.Views.Spaces;
using Client.Domain.Spaces;
using Client.Application.Favorites;
using Client.Data.Favorites;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.Presentation.Views.Spaces
{
    /// <summary>
    /// Interaction logic for SpacesView.xaml
    /// </summary>
    /// 
    //Marko Mišić
    public partial class SpacesView : Window
    {
        private readonly SpaceRepository prostorRepo;
        private readonly ISpacesFavoritesService _favoritesService;
        private readonly TipProstora tipProstora;
        private int KorisnikID;
        private List<Space> sviProstori = new();
        
        public SpacesView(TipProstora tip, int korisnikID)
        {
            InitializeComponent();
            prostorRepo = new SpaceRepository();
            _favoritesService = new SpacesFavoritesService(new SpacesFavoritesRepository());
            Loaded += SpacesView_Loaded;
            tipProstora = tip;
            this.KorisnikID = korisnikID;
        }

        private async void SpacesView_Loaded(object sender, RoutedEventArgs e)
        {
            var prostori = await prostorRepo.DohvatiPoTipu(tipProstora);
            sviProstori = prostori.ToList();

            var favoriteIds = new HashSet<int>();
            if(KorisnikID > 0)
            {
                var favorites = await _favoritesService.DohvatiFavoriteKorisnikaAsync(KorisnikID);
                favoriteIds = favorites.Select(f => f.ProstorId).ToHashSet();
            }
            GridProstori.ItemsSource = prostori.Select(p => new SpaceCardItem(p, favoriteIds.Contains(p.ProstorId))).ToList();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnRezerviraj_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button gumb)
            {
                return;
            }
            if (gumb.CommandParameter is not SpaceCardItem item)
            {
                return;
            }

            var prostor = item.Space;

            var pogled = new ReservationView(prostor, this.KorisnikID)
            {
                Owner = this
            };

            pogled.ShowDialog();
            await OsvjeziFavoriteIkoneAsync();
        }

        private void primjeniFilter()
        {
            string u = TxtFilter.Text.Trim().ToLower();

            IEnumerable<Space> filtrirano = sviProstori;

            if (!string.IsNullOrEmpty(u))
            {
                filtrirano = filtrirano.Where(p =>
                    (p.Naziv ?? "").ToLower().Contains(u) ||
                    (p.Oprema ?? "").ToLower().Contains(u) ||
                    p.Kapacitet.ToString().Contains(u)
                );
            }

            GridProstori.ItemsSource = filtrirano.Select(p => new SpaceCardItem(p, false)) .ToList();
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            primjeniFilter();
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            TxtFilter.Text = string.Empty;
            GridProstori.ItemsSource = sviProstori;
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            primjeniFilter();
        }

        //Nikola Kihas
        private sealed class SpaceCardItem : INotifyPropertyChanged
        {
            public Space Space { get; }
            public string Naziv => Space.Naziv;
            public Dom Dom => Space.Dom;
            public int Kapacitet => Space.Kapacitet;
            public string Oprema => Space.Oprema;
            public string? SlikaPutanja => Space.SlikaPutanja;
            private bool _isFavorite;
            public bool IsFavorite
            {
                get => _isFavorite;
                set
                {
                    if (_isFavorite == value) return;
                    _isFavorite = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FavoriteIconPath));
                }
            }

            public string FavoriteIconPath => IsFavorite ? "/Images/Profile/favorite.png" : "/Images/Profile/not-favorite.png";

            public SpaceCardItem(Space space, bool isFavorite)
            {
                Space = space;
                _isFavorite = isFavorite;
            }

            public void SetFavorite(bool isFavorite)
            {
                IsFavorite = isFavorite;
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        
        private async void BtnToggleFavorite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (KorisnikID <= 0) return;
            if (sender is not FrameworkElement element) return;
            if (element.DataContext is not SpaceCardItem item) return;

            var isFavorite = await _favoritesService.ToggleFavoritaProstora(KorisnikID, item.Space.ProstorId);
            item.SetFavorite(isFavorite);
        }

        private async Task OsvjeziFavoriteIkoneAsync()
        {
            var favorites = await _favoritesService.DohvatiFavoriteKorisnikaAsync(KorisnikID);
            var favoriteIds = favorites.Select(f => f.ProstorId).ToHashSet();

            if (GridProstori.ItemsSource is IEnumerable<SpaceCardItem> items)
            {
                foreach (var item in items)
                {
                    item.SetFavorite(favoriteIds.Contains(item.Space.ProstorId));
                }
            }
        }
    }
}
