using Client.Data.Spaces;
using Client.Domain.Spaces;
using Client.Domain.Templates;
using Client.Presentation.Views.Spaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Client.Presentation.Views.Shared;
//Nikola Kihas
public partial class SpacesDashboardView : UserControl
{

    public int KorisnikId { get; set; }
    private readonly SpaceRepository prostorRepo;
    public ObservableCollection<FavoriteSpaceItem> SpacesForCards { get; } = new();

    public SpacesDashboardView()
    {
        InitializeComponent();
        DataContext = this;
        prostorRepo = new SpaceRepository();
        DohvatiUcioniceZaRezervaciju();
    }

    private void BtnKategorijaUcionice_Click(object sender, RoutedEventArgs e)
    {
        var spacesWindow = new SpacesView(TipProstora.Ucionica, KorisnikId);
        spacesWindow.Show();
    }

    private void BtnKategorijaTeretane_Click(object sender, RoutedEventArgs e)
    {
        var spacesWindow = new SpacesView(TipProstora.Teretana, KorisnikId);
        spacesWindow.Show();
    }

    private async void DohvatiUcioniceZaRezervaciju()
    {
        var prostorije = await prostorRepo.DohvatiPoTipu(TipProstora.Ucionica);
        SpacesForCards.Clear();
        foreach (var prostor in prostorije.Take(3))
        {
            SpacesForCards.Add(new FavoriteSpaceItem
            {
                Title = prostor.Naziv,
                Capacity = prostor.Kapacitet.ToString(),
                Description = prostor.Opis,
                ImagePath = prostor.SlikaPutanja ?? string.Empty
            });
        }
    }
}
