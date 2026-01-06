using Client.Presentation.Views.EventFeedBack;
using System.Windows;
using System.Windows.Controls;

namespace Client.Presentation.Views;

public partial class StaffView : UserControl
{
    public int KorisnikId { get; set; }

    public StaffView()
    {
        InitializeComponent();
    }

    private void BtnEventFeedback_OnClick(object sender, System.Windows.RoutedEventArgs e)
    {
        var window = new SimulacijaDogadajaKomentarWindow(KorisnikId)
        {
            Owner = Window.GetWindow(this)
        };
        window.Show();
    }
}