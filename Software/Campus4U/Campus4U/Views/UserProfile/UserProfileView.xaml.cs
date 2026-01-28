using Client.Application.Users;
using Client.Data.Users;
using Microsoft.IdentityModel.Tokens;
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

namespace Client.Presentation.Views.UserProfile
{
    /// <summary>
    /// Interaction logic for UserProfileView.xaml
    /// </summary>
    public partial class UserProfileView : Window
    {
        private readonly string? _korisnikSub;

        private UserProfileService userProfileService;
        public UserProfileView(string? korisnikSub)
        {
            InitializeComponent();
            _korisnikSub = korisnikSub;
            Loaded += UserProfileView_Loaded;
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

        private async Task UcitajPodatkeProfilaAsync(string korisnikSub)
        {
            IUserProfileRepository userProfileRepository = new UserProfileProfileRepository();
            userProfileService = new UserProfileService(userProfileRepository);

            var profil = await userProfileService.GetBySubAsync(korisnikSub);

            if (profil != null)
            {
                string imePrezime = (profil.Ime ?? "") + " " + (profil.Prezime ?? "");
                txtImePrezime.Text = imePrezime;
                txtEmail.Text = profil.Email;
                txtBrojSobe.Text += profil.BrojSobe.IsNullOrEmpty() ? " -" : " " + profil.BrojSobe;
                txtBrojTelefona.Text = profil.BrojTelefona;
                txtUloga.Text = profil.UlogaId == 1 ? "Student" : "Administrator";
            }
        }

    }
}
