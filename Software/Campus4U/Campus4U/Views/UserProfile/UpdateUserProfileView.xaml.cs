using Client.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DomainUserProfile = Client.Domain.Users.UserProfile;

namespace Client.Presentation.Views.UserProfile
{
    /// <summary>
    /// Interaction logic for UpdateUserProfile.xaml
    /// </summary>
    public partial class UpdateUserProfile : Window
    {
        private readonly DomainUserProfile _profile;
        private readonly UserProfileService _userProfileService;
        public UpdateUserProfile()
        {
            InitializeComponent();
        }

        public UpdateUserProfile(DomainUserProfile profile, UserProfileService userProfileService)
            : this()
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));

            TxtIme.Text = profile.Ime ?? string.Empty;
            TxtPrezime.Text = profile.Prezime ?? string.Empty;
            TxtKorIme.Text = profile.KorisnickoIme ?? string.Empty;
            TxtEmail.Text = profile.Email ?? string.Empty;
            TxtBrojTelefona.Text = profile.BrojTelefona ?? string.Empty;
            TxtBrojSobe.Text = profile.BrojSobe ?? string.Empty;
        }

        private async void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
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

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

