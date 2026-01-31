using System.Windows.Controls;

namespace Client.Presentation.Views;

public partial class StaffView : UserControl
{
    private int _korisnikId;

    public StaffView()
    {
        InitializeComponent();
    }

    public int KorisnikId
    {
        get => _korisnikId;
        set
        {
            _korisnikId = value;
            if (Dashboard != null)
            {
                Dashboard.KorisnikId = value;
            }
        }
    }
}
