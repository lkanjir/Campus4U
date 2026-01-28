using Client.Application.Users;
using Client.Data.Users;
using DomainUserProfile = Client.Domain.Users.UserProfile;
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
        private DomainUserProfile? _profil;
        private string? _selectedImagePath;

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
                _profil = profil;
                string imePrezime = (profil.Ime ?? "") + " " + (profil.Prezime ?? "");
                txtImePrezime.Text = imePrezime;
                txtKorIme.Text = "Korisnicko ime:" + (string.IsNullOrWhiteSpace(profil.KorisnickoIme) ? " -" : " " + profil.KorisnickoIme);
                txtEmail.Text = "E-mail:" + (string.IsNullOrWhiteSpace(profil.Email) ? " -" : " " + profil.Email);
                txtBrojSobe.Text = "Broj sobe:" + (string.IsNullOrWhiteSpace(profil.BrojSobe) ? " -" : " " + profil.BrojSobe);
                txtBrojTelefona.Text = "Broj telefona:" + (string.IsNullOrWhiteSpace(profil.BrojTelefona) ? " -" : " " + profil.BrojTelefona);
                txtUloga.Text = "Uloga:" + (profil.UlogaId == 1 ? " Student" : " Administrator");
            }
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
            var FileExplorer = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Odaberi sliku profila",
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Multiselect = false
            };

            if(FileExplorer.ShowDialog() == true)
            {
                _selectedImagePath = FileExplorer.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(_selectedImagePath));
                ProfileImageBrush.ImageSource = bitmap;
            }
        }
    }
}
