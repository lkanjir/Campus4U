using System.Windows;
using Client.Presentation.Auth;
using Microsoft.Extensions.Configuration;

namespace Client.Presentation
{
    //Luka Kanjir
    public partial class MainWindow : Window
    {
        private readonly AuthService auth;
        private bool isAuthenticated;

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

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            auth.ClearLocalSession();
            isAuthenticated = false;
            TxtStatus.Text = "Obrisana lokalna sesija";
            ApplyButtons();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await RestoreAuthStateAsyc();
        }

        private async Task RestoreAuthStateAsyc()
        {
            SetBusy(true);
            TxtStatus.Text = "Učitavanje sessiona";

            try
            {
                var token = await auth.GetTokenOrClearAsync();
                if (token is null)
                {
                    isAuthenticated = false;
                    TxtStatus.Text = "Niste prijavljeni";
                    return;
                }

                isAuthenticated = true;
                TxtStatus.Text = token.ExpiresAt is null
                    ? "Sesija je aktivna"
                    : $"Sesija je aktivna i ističe: {token.ExpiresAt.Value.LocalDateTime}";
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
        }

        private void SetBusy(bool busy)
        {
            BtnLogin.IsEnabled = !busy;
            BtnLogout.IsEnabled = !busy;
        }
    }
}