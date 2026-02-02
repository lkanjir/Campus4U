using Client.Application.Users;
using System.Windows;
using Client.Application.Notifications;
using Client.Data.Notifications;
using Client.Domain.Notifications;
using DomainUserProfile = Client.Domain.Users.UserProfile;

namespace Client.Presentation.Views.UserProfile
{
    /// <summary>
    /// Interaction logic for UpdateUserProfile.xaml
    /// </summary>
    /// Nikola Kihas
    public partial class UpdateUserProfile : Window
    {
        private readonly DomainUserProfile _profile;
        private readonly UserProfileService _userProfileService;

        private readonly NotificationPreferenceService _notificationPreferencesService;
        private NotificationPreferences _preferences = NotificationPreferences.Default;

        public UpdateUserProfile()
        {
            InitializeComponent();
        }

        public UpdateUserProfile(DomainUserProfile profile, UserProfileService userProfileService)
            : this()
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
            _notificationPreferencesService = new NotificationPreferenceService(new NotificationPreferenceRepository());

            TxtIme.Text = profile.Ime ?? string.Empty;
            TxtPrezime.Text = profile.Prezime ?? string.Empty;
            TxtKorIme.Text = profile.KorisnickoIme ?? string.Empty;
            TxtEmail.Text = profile.Email ?? string.Empty;
            TxtBrojTelefona.Text = profile.BrojTelefona ?? string.Empty;
            TxtBrojSobe.Text = profile.BrojSobe ?? string.Empty;
        }

        private async void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
            var preferences = BuildPreferences();
            var prefResult = await _notificationPreferencesService.SaveAsync(_profile.Id, preferences);
            if (!prefResult.IsSuccess)
            {
                MessageBox.Show(prefResult.Error ?? "Greška kod spremanja postavki obavijesti.");
                return;
            }

            var rez = await _userProfileService.AzurirajProfilAsync(_profile,
                TxtIme.Text,
                TxtPrezime.Text,
                TxtKorIme.Text,
                TxtBrojSobe.Text,
                TxtBrojTelefona.Text);
            if (rez.isSuccess)
            {
                DialogResult = true;
                return;
            }

            MessageBox.Show(rez.Error ?? "Neuspjesno spremanje profila.");
        }

        private NotificationPreferences BuildPreferences() => new(
            ChkNotifyPosts.IsChecked == true,
            ChkNotifyFaults.IsChecked == true,
            ChkNotifyReservations.IsChecked == true);

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void UpdateUserProfile_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _preferences = await _notificationPreferencesService.GetByUserIdAsync(_profile.Id);
            }
            catch (Exception ex)
            {
                _preferences = NotificationPreferences.Default;
            }

            ChkNotifyPosts.IsChecked = _preferences.Posts;
            ChkNotifyFaults.IsChecked = _preferences.Faults;
            ChkNotifyReservations.IsChecked = _preferences.Reservations;
        }
    }
}