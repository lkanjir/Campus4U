using Client.Application.Auth;
using Client.Application.Favorites;
using Client.Application.Users;
using Client.Data.Auth;
using Client.Data.Favorites;
using Client.Data.Users;
using Client.Domain.Spaces;
using Client.Domain.Templates;
using Client.Presentation.Views.Spaces;
using Client.Presentation.Views.Templates;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.Application.Images;
using Client.Data.Images;
using Client.Presentation.Views.Spaces;
using DomainUserProfile = Client.Domain.Users.UserProfile;

namespace Client.Presentation.Views.UserProfile
{
    /// <summary>
    /// Interaction logic for UserProfileView.xaml
    /// </summary>
    /// Nikola Kihas
    public partial class UserProfileView : Window
    {
        private readonly string? _korisnikSub;
        private DomainUserProfile? _profil;
        private string? _selectedImagePath;
        private readonly IImageService imageService;

        private static readonly ImageSource DefaultProfileImage = new BitmapImage(
            new Uri("pack://application:,,,/Images/Profile/default-profile.png",
                UriKind.Absolute));

        private UserProfileService userProfileService;
        private readonly ISpacesFavoritesService spacesFavoritesService;
        private readonly IPasswordResetService passwordResetService;
        public ObservableCollection<FavoriteEventItem> FavoriteEvents { get; } = new();
        public ObservableCollection<FavoriteSpaceItem> FavoriteSpaces { get; } = new();

        public UserProfileView(string? korisnikSub)
        {
            InitializeComponent();
            DataContext = this;
            _korisnikSub = korisnikSub;
            Loaded += UserProfileView_Loaded;
            Activated += UserProfileView_Activated;
            userProfileService = new UserProfileService(new UserProfileProfileRepository());
            spacesFavoritesService = new SpacesFavoritesService(new SpacesFavoritesRepository());

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var options = config.GetSection("Auth0").Get<AuthOptions>();
            if (options is null)
                throw new InvalidOperationException("Problem u konfiguracijama");

            var apiBaseUrl = config["Api:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                throw new InvalidOperationException("API: BaseUrl nije definiran u appsettings.json");

            var tokenStore = new SecureTokenStore();
            imageService = new ImageService(new ImageApiClient(apiBaseUrl, tokenStore), new ImageCache(),
                TimeSpan.FromMinutes(20));

            passwordResetService = new Auth0PasswordResetService(options);
            FavoriteEvents.Add(new FavoriteEventItem
            {
                Title = "Noć muzeja",
                Date = "26.01.2026",
                Description = "Posebne izložbe i slobodan ulaz u muzeje diljem grada."
            });
            FavoriteEvents.Add(new FavoriteEventItem
            {
                Title = "Studentski hackathon",
                Date = "02.02.2026",
                Description = "48-satno timsko natjecanje u razvoju softverskih rješenja."
            });
            FavoriteEvents.Add(new FavoriteEventItem
            {
                Title = "Predavanje: UI/UX trendovi",
                Date = "05.02.2026",
                Description = "Pregled novih trendova u dizajnu sučelja i korisničkog iskustva."
            });
        }

        private async void UserProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_korisnikSub))
            {
                txtImePrezime.Text = "Nepoznati korisnik";
                return;
            }

            await UcitajPodatkeProfilaAsync(_korisnikSub);
        }

        private async void UserProfileView_Activated(object? sender, EventArgs e)
        {
            if (_profil is null) return;
            await DohvatiFavoriteProstorijaAsync(_profil.Id);
        }

        private async Task UcitajPodatkeProfilaAsync(string korisnikSub)
        {
            IUserProfileRepository userProfileRepository = new UserProfileProfileRepository();
            userProfileService = new UserProfileService(userProfileRepository);

            var profil = await userProfileService.GetBySubAsync(korisnikSub);

            if (profil != null)
            {
                _profil = profil;
                await LoadProfileImage(profil.Id);

                string imePrezime = (profil.Ime ?? "") + " " + (profil.Prezime ?? "");
                txtImePrezime.Text = imePrezime;
                txtKorIme.Text = "Korisnicko ime:" +
                                 (string.IsNullOrWhiteSpace(profil.KorisnickoIme) ? " -" : " " + profil.KorisnickoIme);
                txtEmail.Text = "E-mail:" + (string.IsNullOrWhiteSpace(profil.Email) ? " -" : " " + profil.Email);
                txtBrojSobe.Text = "Broj sobe:" +
                                   (string.IsNullOrWhiteSpace(profil.BrojSobe) ? " -" : " " + profil.BrojSobe);
                txtBrojTelefona.Text = "Broj telefona:" +
                                       (string.IsNullOrWhiteSpace(profil.BrojTelefona)
                                           ? " -"
                                           : " " + profil.BrojTelefona);
                txtUloga.Text = "Uloga:" + (profil.UlogaId == 1 ? " Student" : "Osoblje");
                await DohvatiFavoriteProstorijaAsync(profil.Id);
            }
        }

        private async Task DohvatiFavoriteProstorijaAsync(int korisnikId)
        {
            try
            {
                List<Space> prostoriFavoriti = await spacesFavoritesService.DohvatiFavoriteKorisnikaAsync(korisnikId);
                FavoriteSpaces.Clear();
                foreach (var prostor in prostoriFavoriti)
                {
                    FavoriteSpaces.Add(new FavoriteSpaceItem
                    {
                        Title = prostor.Naziv,
                        Capacity = prostor.Kapacitet.ToString(),
                        Description = prostor.Opis,
                        ImagePath = prostor.SlikaPutanja ?? string.Empty,
                        Space = prostor,
                        KorisnikId = _profil?.Id
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju favorita prostorija: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProfileImage(int userId)
        {
            try
            {
                var payload = await imageService.GetProfileImageAsync(userId);
                if (payload is null)
                {
                    ProfileImageBrush.ImageSource = DefaultProfileImage;
                    return;
                }

                ProfileImageBrush.ImageSource = CreateImageSource(payload.Bytes);
            }
            catch
            {
                ProfileImageBrush.ImageSource = DefaultProfileImage;
            }
        }

        private static ImageSource CreateImageSource(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private async void BtnUrediProfil_OnClick(object sender, RoutedEventArgs e)
        {
            if (_profil is null)
            {
                MessageBox.Show("Profil nije ucitan.");
                return;
            }

            var updateView = new UpdateUserProfile(_profil, userProfileService)
            {
                Owner = this
            };
            var dialogResult = updateView.ShowDialog();
            if (dialogResult == true && !string.IsNullOrWhiteSpace(_korisnikSub))
            {
                await UcitajPodatkeProfilaAsync(_korisnikSub);
            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_profil is null) return;

            var FileExplorer = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Odaberi sliku profila",
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Multiselect = false
            };

            if (FileExplorer.ShowDialog() == true)
            {
                _selectedImagePath = FileExplorer.FileName;
                _ = UploadProfileImageAsync(_selectedImagePath);
            }
        }

        private async Task UploadProfileImageAsync(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                const long maxBytes = 10 * 1024 * 1024;
                if (fileInfo.Length is <= 0 or > maxBytes)
                {
                    MessageBox.Show("Slika je prevelika ili nema slike (max 10MB).");
                    return;
                }

                var contentType = GetContentType(fileInfo.Extension);
                if (contentType is null)
                {
                    MessageBox.Show("Nepodržan format slike.");
                    return;
                }

                var bytes = await File.ReadAllBytesAsync(filePath);
                await using var stream = new MemoryStream(bytes, writable: false);
                var upload = new ImageUpload(stream, contentType, bytes.LongLength, fileInfo.Name);
                await imageService.UploadProfileImageAsync(_profil!.Id, upload);

                ProfileImageBrush.ImageSource = CreateImageSource(bytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri uploadu slike.");
            }
        }

        private static string? GetContentType(string extension) =>
            extension.Trim().ToLowerInvariant() switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => null
            };

        private async void BtnPromijeniLozinku_OnClick(object sender, RoutedEventArgs e)
        {
            if (_profil is null)
            {
                MessageBox.Show("Profil nije učitan.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_profil.Email))
            {
                MessageBox.Show("Email nije naveden.");
                return;
            }

            var button = sender as Button;
            if (button is not null) button.IsEnabled = false;
            try
            {
                var result = await passwordResetService.SendPasswordResetEmailAsync(_profil.Email);
                if (result.IsSuccess)
                    MessageBox.Show("Poslan je email za promjenu lozinke.");
                else
                    MessageBox.Show($"Slanje emaila nije uspjelo: {result.Error}");
            }
            finally
            {
                if (button is not null)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private async void SpaceCard_ReserveRequested(object sender, SpaceCardReserveEventArgs e)
        {
            if (e.Prostor == null || e.KorisnikId is null || e.KorisnikId <= 0) return;
            var pogled = new ReservationView(e.Prostor, e.KorisnikId.Value)
            {
                Owner = this
            };
            pogled.ShowDialog();
            if (_profil is not null)
            {
                await DohvatiFavoriteProstorijaAsync(_profil.Id);
            }
        }
        private void BtnMojeRezervacije_OnClick(object sender, RoutedEventArgs e)
        {
            if (_korisnikSub == null)
            {
                return;
            }

            MyReservationView myReservationView = new MyReservationView(_korisnikSub)
            {
                Owner = this
            };
            myReservationView.ShowDialog();
        }
    }
}