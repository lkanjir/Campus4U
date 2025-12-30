using System.Windows;
using System.Windows.Controls;

namespace Client.Presentation.Views;

public partial class OnboardingView : UserControl
{
    public event EventHandler<OnboardingSubmitEvent>? Submitted;

    public OnboardingView()
    {
        InitializeComponent();
    }

    public void SetInitialValues(string? ime, string? prezime, string? email, string? brojSobe)
    {
        TxtIme.Text = ime ?? string.Empty;
        TxtPrezime.Text = prezime ?? string.Empty;
        TxtEmail.Text = email ?? string.Empty;
        TxtSoba.Text = brojSobe ?? string.Empty;
        SetStatus(string.Empty);
    }

    public void SetBusy(bool busy)
    {
        TxtIme.IsEnabled = TxtPrezime.IsEnabled = TxtSoba.IsEnabled = BtnSave.IsEnabled = !busy;
    }

    public void SetStatus(string status)
    {
        TxtStatus.Text = status;
        TxtStatus.Visibility = string.IsNullOrWhiteSpace(status) ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BtnSave_OnClick(object sender, RoutedEventArgs e)
    {
        var ime = TxtIme.Text?.Trim();
        var prezime = TxtPrezime.Text?.Trim();
        var email = TxtEmail.Text.Trim();

        if (string.IsNullOrWhiteSpace(ime) || string.IsNullOrWhiteSpace(prezime))
        {
            SetStatus("Ime i prezime su obavezni");
            return;
        }

        var brojSobe = string.IsNullOrWhiteSpace(TxtSoba.Text) ? null : TxtSoba.Text.Trim();

        Submitted?.Invoke(this, new OnboardingSubmitEvent(ime, prezime, email, brojSobe));
    }
}

public sealed class OnboardingSubmitEvent(string ime, string prezime, string email, string? brojSobe) : EventArgs
{
    public string Ime { get; } = ime;
    public string Prezime { get; } = prezime;
    public string Email { get; } = email;
    public string? BrojSobe { get; } = brojSobe;
}