using Client.Application.Auth;
using Client.Application.Users;
using Client.Data.Auth;
using Client.Data.Users;
using Client.Domain.Auth;
using Client.Presentation.Views;
using Client.Presentation.Views.Fault;
using Client.Presentation.Views.Spaces;
using Client.Presentation.Views.UserProfile;
using Duende.IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Client.Presentation
{
    //Luka Kanjir
    public partial class MainWindow
    {
        private bool isAuthenticated;
        private bool isBusy;
        private string currentRole = "student";
        private bool requiresOnboarding;
        private string? currentSub;
        private string? currentEmail;
        private int currentId;

        private IAuthService authService;
        private UserProfileService userProfileService;

        private readonly StudentView studentView = new();
        private readonly StaffView staffView = new();
        private readonly OnboardingView onboardingView = new();

        private readonly DispatcherTimer triggerTimer = new()
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        private readonly HttpClient http;
        private bool triggersStarted;

        //TODO: treba maknuti, samo privremeno, do navigacije
        private readonly PrijavaKvaraUserControl kvaroviView = new();
        private bool categoryWindowShown;

        public MainWindow()
        {
            InitializeComponent();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var authOptions = config.GetSection("Auth0").Get<AuthOptions>();
            if (authOptions is null)
                throw new InvalidOperationException("appsettings.json ne postoji ili je Auth0 config krivog formata");

            var apiBaseUrl = config["Api:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                throw new InvalidOperationException("API: BaseUrl nije definiran u appsettings.json");
            http = new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            };

            ITokenStore tokenStore = new SecureTokenStore();
            IBrowser browser = new SystemBrowser();
            IAuthProvider authProvider = new OidcProvider(authOptions, browser);
            authService = new AuthService(authProvider, tokenStore);

            IUserProfileRepository userProfileRepository = new UserProfileProfileRepository();
            userProfileService = new UserProfileService(userProfileRepository);
            onboardingView.Submitted += OnOnboardingSubmitted;

            triggerTimer.Tick += TriggerTimerOnTick;

            SetStatus(string.Empty);
            ApplyUiState();
        }

        private async void TriggerTimerOnTick(object? sender, EventArgs e)
        {
            try
            {
                await http.PostAsync("api/triggers/heartbeat", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"TRIGGER GRESKA: {ex.Message}");
            }
        }

        private async void OnOnboardingSubmitted(object? sender, OnboardingSubmitEvent e)
        {
            if (string.IsNullOrWhiteSpace(currentSub) || string.IsNullOrWhiteSpace(currentEmail))
            {
                isAuthenticated = false;
                requiresOnboarding = false;
                SetStatus("Nedostaju podatci iz Auth0 profila.");
                ApplyUiState();
                return;
            }

            SetBusy(true);
            onboardingView.SetStatus(string.Empty);
            try
            {
                // "" => brojTelefona, slikaProfila, korIme nisu obavezni za onboarding
                var result = await userProfileService.SaveAsync(currentSub, currentEmail, e.Ime, e.Prezime, e.BrojSobe,
                    "", "", "",
                    currentRole);

                if (!result.isSuccess)
                {
                    requiresOnboarding = true;
                    onboardingView.SetStatus(result.Error ?? "Greška kod spremanja profila");
                    return;
                }

                requiresOnboarding = false;
            }
            catch (Exception)
            {
                requiresOnboarding = true;
                onboardingView.SetStatus("Greška kod spremanja profila");
            }
            finally
            {
                SetBusy(false);
            }
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

            if (requiresOnboarding)
            {
                RoleContent.Content = onboardingView;
                return;
            }

            /*
            RoleContent.Content = currentRole switch
            {
                "osoblje" => staffView,
                "student" => studentView,
                _ => "student"
            };
            */

            //TODO: treba maknuti, samo privremeno, do navigacije, i vratiti dio iznad
            RoleContent.Content = kvaroviView;
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
                        SetIdentity(result.Token?.Sub, result.Token?.Email);
                        await RefreshOnboardingStateAsync();
                        await EnsureTriggersStartedAsync();
                        break;
                    case AuthSessionRestoreState.ExpiredNoRefreshToken:
                        isAuthenticated = false;
                        requiresOnboarding = false;
                        SetStatus("Sesija je istekla (nema refresh tokena).");
                        break;
                    case AuthSessionRestoreState.RefreshFailed:
                        isAuthenticated = false;
                        requiresOnboarding = false;
                        SetStatus($"Sesija je istekla, greška kod osvjezavanja: {result.Error}");
                        break;
                    default:
                        isAuthenticated = false;
                        requiresOnboarding = false;
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
                requiresOnboarding = false;
                currentSub = null;
                currentEmail = null;
                currentId = 0;
                staffView.KorisnikId = 0;
                studentView.KorisnikId = 0;
                SetStatus("Uspješna odjava");
            }
            catch (Exception ex)
            {
                isAuthenticated = false;
                requiresOnboarding = false;
                SetStatus($"Greška kod odjave: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
                triggerTimer.Stop();
                triggersStarted = false;
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
                    requiresOnboarding = false;
                    SetStatus($"Prijava nije uspjela: {result.Error}");
                }
                else
                {
                    isAuthenticated = true;
                    SetRoleFromToken(result.Role);
                    SetIdentity(result.Sub, result.Email);
                    await RefreshOnboardingStateAsync();
                    await EnsureTriggersStartedAsync();
                    SetStatus(string.Empty);
                }
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task RefreshOnboardingStateAsync()
        {
            requiresOnboarding = false;
            onboardingView.SetStatus(string.Empty);

            if (!isAuthenticated) return;
            if (string.IsNullOrWhiteSpace(currentSub) || string.IsNullOrWhiteSpace(currentEmail))
            {
                isAuthenticated = false;
                requiresOnboarding = false;
                currentSub = null;
                currentEmail = null;
                currentId = 0;
                staffView.KorisnikId = 0;
                studentView.KorisnikId = 0;
                SetStatus("Nedostaju podaci iz Auth0 profila");
                return;
            }

            var profile = await userProfileService.GetBySubAsync(currentSub);
            currentId = profile?.Id ?? 0;

            //TODO: treba maknuti, samo privremeno, do navigacije
            kvaroviView.PostaviKorisnika(currentId);
            if (!categoryWindowShown)
            {
                categoryWindowShown = true;
                var window = new CategorySelectionView(currentId)
                {
                    Owner = this
                };
                window.Show();
            }

            staffView.KorisnikId = currentId;
            studentView.KorisnikId = currentId;
            if (profile is null || !profile.IsOnboardingComplete)
            {
                requiresOnboarding = true;
                onboardingView.SetInitialValues(profile?.Ime, profile?.Prezime, currentEmail, profile?.BrojSobe);
            }
        }

        private void SetIdentity(string? sub, string? email)
        {
            currentSub = string.IsNullOrWhiteSpace(sub) ? null : sub.Trim();
            currentEmail = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        }

        private void SetRoleFromToken(string? role) =>
            currentRole = string.IsNullOrEmpty(role) ? "student" : role.Trim().ToLowerInvariant();

        private void SetBusy(bool busy)
        {
            isBusy = busy;
            onboardingView.SetBusy(busy);
            ApplyUiState();
        }

        private void BtnProfilKorisnika_OnClick(object sender, MouseButtonEventArgs e)
        {
            var profileView = new UserProfileView(currentSub)
            {
                Owner = this
            };
            profileView.Show();
        }

        private async Task StartTriggersAsync()
        {
            try
            {
                await http.PostAsync("api/triggers/start", content: null);
            }
            catch
            {
                MessageBox.Show("Greska kod pokretanja triggera");
            }
        }

        private async Task EnsureTriggersStartedAsync()
        {
            if (triggersStarted) return;

            await StartTriggersAsync();
            triggerTimer.Start();
            triggersStarted = true;
        }
    }
}