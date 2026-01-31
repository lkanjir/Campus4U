using Client.Domain.Spaces;
using Client.Presentation.Views.Spaces;
using System.Windows;
using System.Windows.Controls;

namespace Client.Presentation.Views.Shared;
//Nikola Kihas
public partial class SpacesDashboardView : UserControl
{

    public int KorisnikId { get; set; }

    public SpacesDashboardView()
    {
        InitializeComponent();
        DataContext = this;
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
}
