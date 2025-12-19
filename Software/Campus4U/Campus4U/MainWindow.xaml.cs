using System.Windows;
using System.Windows.Media;
using Client.Domain.Auth;
using Client.Presentation.Auth;
using Duende.IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Configuration;

namespace Client.Presentation
{
    //Luka Kanjir
    public partial class MainWindow : Window
    {
        private readonly AuthService auth;
        private bool isAuthenticated;

        private readonly AuthDiagnostics diagnostics;

        public MainWindow()
        {
            InitializeComponent();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var options = config.GetSection("Auth0").Get<AuthOptions>();
            if (options is null)
                throw new InvalidOperationException("appsettings.json ne postoji ili je Auth0 config krivog formata");

            auth = new AuthService(options, new SecureTokenStore());

            diagnostics = new AuthDiagnostics(auth, options.Domain);
        }

        private async void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            SetBusy(true);
            TxtStatus.Text = "Prijava traje...";
            try
            {
                var result = await auth.LoginAsync();
                if (result.IsError)
                {
                    isAuthenticated = false;
                    TxtStatus.Text = $"Greska: {result.Error}";
                }
                else
                {
                    isAuthenticated = true;
                    TxtStatus.Text = $"Uspjeh: {result.User?.FindFirst("name")?.Value}";
                }
            }
            finally
            {
                SetBusy(false);
                ApplyButtons();
            }
        }

        private async void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await auth.LogoutAsync();
                isAuthenticated = false;

                TxtStatus.Text = result == BrowserResultType.Success
                    ? "Uspješna odjava i brisanje lokalne sesije"
                    : $"Lokalna sesija obrisana, Auth0 greska: {result}";
            }
            catch (Exception ex)
            {
                isAuthenticated = false;
                TxtStatus.Text = $"Lokalna sesija obrisana, greska: {ex.Message}";
            }
            finally
            {
                SetBusy(false);
                ApplyButtons();
            }
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await RestoreAuthStateAsyc();
        }

        private async Task RestoreAuthStateAsyc()
        {
            SetBusy(true);
            TxtStatus.Text = "Ucitavanje sesije";

            try
            {
                var result = await auth.RestoreSessionAsync();
                switch (result.State)
                {
                    case AuthSessionRestoreState.SignedIn:
                    case AuthSessionRestoreState.Refreshed:
                        isAuthenticated = true;
                        var token = result.Token;
                        TxtStatus.Text =
                            token?.ExpiresAt is null
                                ? "Sesija - bez expires at"
                                : $"Sesija - istice: {token.ExpiresAt.Value.LocalDateTime}";
                        break;
                    case AuthSessionRestoreState.ExpiredNoRefreshToken:
                        isAuthenticated = false;
                        TxtStatus.Text = "Sesija isteka, nema refresh tokena";
                        break;
                    case AuthSessionRestoreState.RefreshFailed:
                        isAuthenticated = false;
                        TxtStatus.Text = $"Sesija istekla, greska kod azuriranja: {result.Error}";
                        break;
                    default:
                        isAuthenticated = false;
                        TxtStatus.Text = "Niste prijavljeni";
                        break;
                }
            }
            finally
            {
                SetBusy(false);
                ApplyButtons();
            }
        }

        private void ApplyButtons()
        {
            BtnLogin.IsEnabled = !isAuthenticated;
            BtnLogout.IsEnabled = isAuthenticated;

            BtnTest.IsEnabled = isAuthenticated;
        }

        private void SetBusy(bool busy)
        {
            BtnLogin.IsEnabled = !busy;
            BtnLogout.IsEnabled = !busy;

            BtnTest.IsEnabled = !busy && isAuthenticated;
        }

        private async void BtnTest_OnClick(object sender, RoutedEventArgs e)
        {
            SetBusy(true);
            TxtStatus.Text = "Refresh userinfo ... ";
            TxtDiagnostics.Clear();

            try
            {
                var result = await diagnostics.RunAsync();
                isAuthenticated =
                    result.RestoreState is AuthSessionRestoreState.SignedIn or AuthSessionRestoreState.Refreshed;

                if (result.IsSuccess)
                {
                    var expires = result.ExpiresAt?.LocalDateTime;
                    TxtStatus.Text = $"OK: {expires}";
                    TxtDiagnostics.Text = result.Payload ?? "prazno";
                }
                else
                {
                    TxtStatus.Text = result.Error ?? "Dijagnostika: greska";
                    TxtDiagnostics.Text = result.Payload ?? "Nema tijela";
                }
            }
            finally
            {
                SetBusy(false);
                ApplyButtons();
            }
        }
    }
}