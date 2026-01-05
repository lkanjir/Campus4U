using Client.Presentation.Views.EventFeedBack;
using System.Windows;
using System.Windows.Controls;

namespace Client.Presentation.Views;

public partial class StaffView : UserControl
{
    public StaffView()
    {
        InitializeComponent();
    }

    private void BtnEventFeedback_OnClick(object sender, System.Windows.RoutedEventArgs e)
    {
        var window = new SimulacijaDogadajaKomentarWindow
        {
            Owner = Window.GetWindow(this)
        };
        window.Show();
    }
}