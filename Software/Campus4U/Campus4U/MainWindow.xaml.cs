using System.Windows;
using Client.Presentation.Auth;
using Microsoft.Extensions.Configuration;

namespace Client.Presentation
{
    //Luka Kanjir
    public partial class MainWindow : Window
    {
        private readonly AuthService auth;

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
            TxtStatus.Text = "Prijava traje...";
            BtnLogin.IsEnabled = false;

            var result = await auth.LoginAsync();
            if (result.IsError) TxtStatus.Text = $"Greska: {result.Error}";
            else TxtStatus.Text = $"Prijavljen: {result.User?.FindFirst("name")?.Value}";
        }
    }
}