using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Client.Presentation.Views;

public partial class OnboardingView : UserControl
{
    public event EventHandler<OnboardingSubmitEvent>? Submitted;
    private string? selectedProfileImagePath;

    public OnboardingView()
    {
        InitializeComponent();
    }

    public void SetInitialValues(string? ime, string? prezime, string? email, string? brojSobe, string? korIme, string? brojTelefona)
    {
        TxtIme.Text = ime ?? string.Empty;
        TxtPrezime.Text = prezime ?? string.Empty;
        TxtEmail.Text = email ?? string.Empty;
        TxtSoba.Text = brojSobe ?? string.Empty;
        TxtKorIme.Text = korIme ?? string.Empty;
        TxtBrojTelefona.Text = brojTelefona ?? string.Empty;
        SetStatus(string.Empty);
    }

    public void SetBusy(bool busy)
    {
        TxtIme.IsEnabled = TxtPrezime.IsEnabled = TxtSoba.IsEnabled = TxtKorIme.IsEnabled = TxtBrojTelefona.IsEnabled = BtnSave.IsEnabled = BtnProfileImageOnBorading.IsEnabled = !busy;
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
        var korIme = string.IsNullOrWhiteSpace(TxtKorIme.Text) ? null : TxtKorIme.Text.Trim();
        var brojTelefona = string.IsNullOrWhiteSpace(TxtBrojTelefona.Text) ? null : TxtBrojTelefona.Text.Trim();

        Submitted?.Invoke(this, new OnboardingSubmitEvent(ime, prezime, email, brojSobe, korIme, brojTelefona, selectedProfileImagePath));
    }

    private void BtnProfileImageOnBorading_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var picker = new OpenFileDialog
        {
            Title = "Izaberite profilnu sliku",
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp",
            Multiselect = false
        };

        if(picker.ShowDialog() == true)
        {
            selectedProfileImagePath = picker.FileName;
            SetStatus($"Odabrana slika: {System.IO.Path.GetFileName(selectedProfileImagePath)}");
        }
    }
}

public sealed class OnboardingSubmitEvent(string ime, string prezime, string email, string? brojSobe, string? korisnickoIme, string? brojTelefona, string? profileImagePath) : EventArgs
{
    public string Ime { get; } = ime;
    public string Prezime { get; } = prezime;
    public string Email { get; } = email;
    public string? BrojSobe { get; } = brojSobe;
    public string? KorisnickoIme { get; } = korisnickoIme;
    public string? BrojTelefona { get; } = brojTelefona;
    public string? ProfileImagePath { get; } = profileImagePath;
}