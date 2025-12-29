using System.Windows;

using Client.Application.Auth;
using Client.Data.Auth;
using Client.Domain.Auth;
using Duende.IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Configuration;

namespace Client.Presentation
{
    //Luka Kanjir
    public partial class MainWindow : Window
    {
        private bool isAuthenticated;
        private ITokenStore tokenStore;
        private IBrowser browser;
        private IAuthProvider authProvider;
        private IAuthService authService;
        
        
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

            tokenStore = new SecureTokenStore();
            browser = new SystemBrowser();
            authProvider = new OidcProvider(options, browser);
            authService = new AuthService(authProvider, tokenStore);
            
            diagnostics = new AuthDiagnostics(authService, options.Domain);
        }
        
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
        
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}