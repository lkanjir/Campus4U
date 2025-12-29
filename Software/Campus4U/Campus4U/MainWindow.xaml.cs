using System.Windows;

using Client.Application.Auth;
using Client.Data.Auth;
using Client.Domain.Auth;
using Client.Presentation.Views;
using Duende.IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Configuration;

namespace Client.Presentation
{
    //Luka Kanjir
    public partial class MainWindow
    {
        private bool isAuthenticated;
        private bool isBusy;
        private string currentRole = "student";
        
        private IAuthService authService;

        private readonly StudentView studentView = new();
        private readonly StaffView staffView = new();
        public MainWindow()
        {
            InitializeComponent();
            
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var options = config.GetSection("Auth0").Get<AuthOptions>();
            if (options is null)
                throw new InvalidOperationException("appsettings.json ne postoji ili je Auth0 config krivog formata");

            ITokenStore tokenStore = new SecureTokenStore();
            IBrowser browser = new SystemBrowser();
            IAuthProvider authProvider = new OidcProvider(options, browser);
            authService = new AuthService(authProvider, tokenStore);

            SetStatus(string.Empty);
            ApplyUiState();
        }

        private void ApplyUiState()
        {
            BtnLogin.IsEnabled = !isBusy && !isAuthenticated;
            BtnLogout.IsEnabled = !isBusy && isAuthenticated;

            PanelHeader.Visibility = isAuthenticated ? Visibility.Visible : Visibility.Collapsed;
            RoleContent.Visibility = isAuthenticated ? Visibility.Visible : Visibility.Collapsed;
            PanelSignedOut.Visibility = isAuthenticated ? Visibility.Collapsed : Visibility.Visible;

            if (isAuthenticated) ApplyRoleContent();
            else RoleContent.Content = null;
        }

        private void ApplyRoleContent()
        {
            if (!isAuthenticated)
            {
                RoleContent.Content = null;
                return;
            }
            
            RoleContent.Content = currentRole switch
            {
                "osoblje" => staffView,
                "student" => studentView,
                _ => "student"
            };
        }

        private void SetStatus(string message)
        {
            TxtStatus.Text = message;
            TxtStatus.Visibility = string.IsNullOrWhiteSpace(message) ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await RestoreAuthState();
        }

        private async Task RestoreAuthState()
        {
            SetBusy(true);
            SetStatus("Učitavanje sesije");
            try
            {
                var result = await authService.RestoreSessionAsync();
                switch (result.State)
                {
                    case AuthSessionRestoreState.SignedIn:
                    case AuthSessionRestoreState.Refreshed:
                        isAuthenticated = true;
                        SetRoleFromToken(result.Token?.Role);
                        break;
                    case AuthSessionRestoreState.ExpiredNoRefreshToken:
                        isAuthenticated = false;
                        SetStatus("Sesija je istekla (nema refresh tokena).");
                        break;
                    case AuthSessionRestoreState.RefreshFailed:
                        isAuthenticated = false;
                        SetStatus($"Sesija je istekla, greska kod osvjezavanja: {result.Error}");
                        break;
                    default:
                        isAuthenticated = false;
                        SetStatus(string.Empty);
                        break;
                }
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            SetBusy(true);
            try
            {
                await authService.LogoutAsync();
                isAuthenticated = false;
                SetStatus("Uspješna odjava");
            }
            catch (Exception ex)
            {
                isAuthenticated = false;
                SetStatus($"Greška kod odjave: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            SetBusy(true);
            SetStatus("Prijava u tijeku...");
            try
            {
                var result = await authService.LoginAsync();
                if (!result.IsSuccess)
                {
                    isAuthenticated = false;
                    SetStatus($"Prijava nije uspjela: {result.Error}");
                }
                else
                {
                    isAuthenticated = true;
                    SetRoleFromToken(result.Role);
                    SetStatus(string.Empty);
                }
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetRoleFromToken(string? role) => currentRole = string.IsNullOrEmpty(role) ? "student" : role.Trim().ToLowerInvariant();

        private void SetBusy(bool busy)
        {
            isBusy = busy;
            ApplyUiState();
        }
    }
}