using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Client.Application.Favorites;
using Client.Data.Favorites;
using Client.Domain.Spaces;
using Client.Presentation.Views.Spaces;

namespace Client.Presentation.Views.Shared;
public partial class SpacesDashboardView : UserControl
{
    //Nikola Kihas
    private static readonly ImageSource DefaultSpaceImage =
        new BitmapImage(new Uri("pack://application:,,,/Images/no-img.png", UriKind.Absolute));

    private readonly ISpacesFavoritesService _favoritesService;
    private int _korisnikId;
    private int _loadVersion;

    public ObservableCollection<FavoriteSpaceCardItem> FavoriteSpaceCards { get; } = new();

    public int KorisnikId
    {
        get => _korisnikId;
        set
        {
            if (_korisnikId == value) return;
            _korisnikId = value;
            _ = LoadFavoritesAsync();
        }
    }

    public SpacesDashboardView()
    {
        InitializeComponent();
        DataContext = this;
        _favoritesService = new SpacesFavoritesService(new SpacesFavoritesRepository());
        Loaded += (_, __) => _ = LoadFavoritesAsync();
    }

    private void BtnKategorijaUcionice_Click(object sender, RoutedEventArgs e)
    {
        var spacesWindow = new SpacesView(TipProstora.Ucionica, KorisnikId)
        {
            Owner = Window.GetWindow(this)
        };
        spacesWindow.Closed += (_, __) => _ = LoadFavoritesAsync();

        spacesWindow.Show();
    }

    private void BtnKategorijaTeretane_Click(object sender, RoutedEventArgs e)
    {
        var spacesWindow = new SpacesView(TipProstora.Teretana, KorisnikId)
        {
            Owner = Window.GetWindow(this)
        };
        spacesWindow.Closed += (_, __) => _ = LoadFavoritesAsync();

        spacesWindow.Show();
    }

    private async Task LoadFavoritesAsync()
    {
        var version = ++_loadVersion;

        if (_korisnikId <= 0)
        {
            ShowNoFavorites("Ovdje će se prikazati favoriti kada oni budu oznaćeni.");
            return;
        }

        SetFavoritesStatus("Učitavanje favorita...");

        try
        {
            var favorites = await _favoritesService.DohvatiFavoriteKorisnikaAsync(_korisnikId);
            if (version != _loadVersion) return;

            var ucionice = favorites.Where(p => p.TipProstora == TipProstora.Ucionica).ToList();
            var teretane = favorites.Where(p => p.TipProstora == TipProstora.Teretana).ToList();

            var selected = ucionice.Count > 0 ? ucionice : teretane;
            if (selected.Count == 0)
            {
                ShowNoFavorites("Ovdje će se prikazati favoriti kada oni budu oznaćeni.");
                return;
            }

            TxtFavoritesTitle.Text = ucionice.Count > 0 ? "Favoriti učionica" : "Favoriti teretana";

            FavoriteSpaceCards.Clear();
            foreach (var space in selected.Take(3))
            {
                FavoriteSpaceCards.Add(new FavoriteSpaceCardItem(space, CreateImageSource(space.SlikaPutanja)));
            }

            TxtFavoritesStatus.Visibility = Visibility.Collapsed;
            ScrollFavorites.Visibility = Visibility.Visible;
        }
        catch
        {
            if (version != _loadVersion) return;
            ShowNoFavorites("Greška kod dohvaćanja favorita.");
        }
    }

    private void SetFavoritesStatus(string message)
    {
        TxtFavoritesTitle.Text = "Favoriti prostorija";
        TxtFavoritesStatus.Text = message;
        TxtFavoritesStatus.Visibility = Visibility.Visible;
        ScrollFavorites.Visibility = Visibility.Collapsed;
    }

    private void ShowNoFavorites(string message)
    {
        FavoriteSpaceCards.Clear();
        SetFavoritesStatus(message);
    }

    private static ImageSource CreateImageSource(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) return DefaultSpaceImage;

        try
        {
            return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
        }
        catch
        {
            return DefaultSpaceImage;
        }
    }

    private void FavoriteSpace_SeeRequested(object sender, int spaceId)
    {
        if (spaceId <= 0 || _korisnikId <= 0) return;

        var item = FavoriteSpaceCards.FirstOrDefault(p => p.SpaceId == spaceId);
        if (item?.Space is null) return;

        var pogled = new ReservationView(item.Space, _korisnikId)
        {
            Owner = Window.GetWindow(this)
        };
        pogled.ShowDialog();

        _ = LoadFavoritesAsync();
    }

    public sealed class FavoriteSpaceCardItem
    {
        public FavoriteSpaceCardItem(Space space, ImageSource image)
        {
            Space = space;
            SpaceId = space.ProstorId;
            Title = space.Naziv;
            Date = $"Kapacitet: {space.Kapacitet}";
            Description = space.Opis;
            Image = image;
        }

        public int SpaceId { get; }
        public string Title { get; }
        public string Date { get; }
        public string Description { get; }
        public ImageSource Image { get; }
        public Space Space { get; }
    }
}
