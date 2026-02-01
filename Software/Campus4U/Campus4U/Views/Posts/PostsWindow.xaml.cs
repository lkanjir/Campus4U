using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Client.Application.Images;
using Client.Application.Posts;
using Client.Data.Auth;
using Client.Data.Images;
using Client.Data.Posts;
using Client.Domain.Posts;
using Microsoft.Extensions.Configuration;

namespace Client.Presentation.Views.Posts;

//Luka Kanjir
public partial class PostsWindow
{
    private readonly int _userId;
    private readonly bool _isStaff;
    private readonly PostsService _postsService;
    private readonly InterestsService _interestsService;
    private readonly IImageService imageService;

    private PostDetail? _currentPost;
    private bool _isInterested;

    public ObservableCollection<PostListItem> Posts { get; } = [];

    public PostsWindow(int userId, bool isStaff)
    {
        InitializeComponent();
        DataContext = this;
        this._userId = userId;
        this._isStaff = isStaff;

        var postsRepository = new PostsRepository();
        _postsService = new PostsService(postsRepository);
        _interestsService = new InterestsService(new InterestsRepository(), postsRepository);
        UpdateActionButtons();

        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
        var apiBaseUrl = config["Api:BaseUrl"];
        if (string.IsNullOrWhiteSpace(apiBaseUrl))
            throw new InvalidOperationException("API: BaseUrl nije definiran u appsettings.json");

        var tokenStore = new SecureTokenStore();
        imageService = new ImageService(new ImageApiClient(apiBaseUrl, tokenStore), new ImageCache(),
            TimeSpan.FromMinutes(20));

        ImgEvent.Visibility = Visibility.Collapsed;
    }

    private void UpdateActionButtons()
    {
        var canEditDelete = _currentPost is not null && (_isStaff || _currentPost.AutorId == _userId);
        BtnEdit.Visibility = canEditDelete ? Visibility.Visible : Visibility.Collapsed;
        BtnDelete.Visibility = canEditDelete ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void PostsWindow_OnLoaded(object sender, RoutedEventArgs e) => await LoadPostsAsync();

    private async Task LoadPostsAsync()
    {
        try
        {
            var items = await _postsService.GetAllAsync();
            Posts.Clear();
            foreach (var item in items)
            {
                Posts.Add(item);
            }

            SelectPost(_currentPost?.Id);
            if (PostsList.SelectedItem is null) ClearDetails();
        }
        catch (Exception)
        {
            MessageBox.Show("Greška kod dohvaćanja objava");
        }
    }

    private void SelectPost(int? postId)
    {
        if (postId is null) return;
        foreach (var item in Posts)
        {
            if (item.Id == postId.Value)
            {
                PostsList.SelectedItem = item;
                break;
            }
        }
    }

    private async void BtnRefresh_OnClick(object sender, RoutedEventArgs e) => await LoadPostsAsync();

    private async void PostsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = PostsList.SelectedItem as PostListItem;
        if (selected is null)
        {
            ClearDetails();
            return;
        }

        try
        {
            var selectedId = selected.Id;
            _currentPost = await _postsService.GetByIdAsync(selectedId);
            if (PostsList.SelectedItem is not PostListItem currentSelection ||
                currentSelection.Id != selectedId) return;
            if (_currentPost is null)
            {
                ClearDetails();
                return;
            }

            TxtTitle.Text = _currentPost.Naslov;
            TxtAuthor.Text = string.IsNullOrWhiteSpace(_currentPost.AutorIme)
                ? "Autor: -"
                : $"Autor: {_currentPost.AutorIme}";
            var eventDateText = _currentPost.DatumVrijemeDogadaja == default
                ? "-"
                : _currentPost.DatumVrijemeDogadaja.ToString("dd.MM.yyyy. HH:mm");
            TxtDates.Text = $"Objava: {_currentPost.DatumVrijemeObjave:dd.MM.yyyy.} | Događaj: {eventDateText}";
            TxtBody.Text = _currentPost.Sadrzaj;

            ShowDetails();
            await UpdateInterestsStateAsync();
            await UpdateEventImageAsync(selectedId);
            await UpdateCommentsAsync();
            UpdateActionButtons();
        }
        catch (Exception)
        {
            MessageBox.Show($"Greska kod učitavanja objave");
        }
    }

    private async Task UpdateEventImageAsync(int selectedId)
    {
        if (_currentPost is null) return;

        try
        {
            var payload = await imageService.GetEventImageAsync(_currentPost.Id);
            if (_currentPost is null || _currentPost.Id != selectedId) return;
            if (payload is null)
            {
                ImgEvent.Visibility = Visibility.Collapsed;
                ImgEvent.Source = null;
                return;
            }

            ImgEvent.Source = CreateImageSource(payload.Bytes);
            ImgEvent.Visibility = Visibility.Visible;
        }
        catch
        {
            ImgEvent.Visibility = Visibility.Collapsed;
            ImgEvent.Source = null;
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

    private async Task UpdateCommentsAsync()
    {
        if (_currentPost is null || _currentPost.DatumVrijemeDogadaja == default)
        {
            EventFeedbackControl.Clear();
            EventFeedbackControl.IsEnabled = false;
            return;
        }
        
        EventFeedbackControl.IsEnabled = true;
        await EventFeedbackControl.LoadAsync(_currentPost.Id, _userId);
    }

    private async Task UpdateInterestsStateAsync()
    {
        if (_currentPost is null)
        {
            BtnToggleInterests.IsEnabled = false;
            TxtInterestStatus.Text = string.Empty;
            return;
        }

        if (_currentPost.DatumVrijemeDogadaja == default)
        {
            BtnToggleInterests.IsEnabled = false;
            TxtInterestStatus.Text = "Neispravan datum događaja";
            return;
        }

        _isInterested = await _interestsService.IsInterestedAsync(_currentPost.Id, _userId);
        BtnToggleInterests.IsEnabled = true;
        BtnToggleInterests.Content = _isInterested ? "Ne zanima me" : "Zanima me";
        TxtInterestStatus.Text = _isInterested ? "Interes označen" : "Interes nije označen";
    }

    private void ClearDetails()
    {
        _currentPost = null;
        TxtTitle.Text = "Odaberi objavu";
        TxtAuthor.Text = string.Empty;
        TxtDates.Text = string.Empty;
        TxtBody.Text = string.Empty;
        EventFeedbackControl.Clear();
        UpdateActionButtons();
        ImgEvent.Source = null;
        HideDetails();
    }

    private async void BtnToggleInterests_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentPost is null) return;

        var result = await _interestsService.ToggleAsync(_currentPost.Id, _userId);
        if (!result.IsSuccess)
        {
            MessageBox.Show(result.Error ?? "Greška kod ažuriranja interesa");
            return;
        }

        _isInterested = result.IsInterested;
        BtnToggleInterests.Content = _isInterested ? "Ne zanima me" : "Zanima me";
        TxtInterestStatus.Text = _isInterested ? "Interes označen" : "Interes nije označen";
    }

    private void BtnNew_OnClick(object sender, RoutedEventArgs e)
    {
        var window = new PostEditWindow(_postsService, _userId, _isStaff)
        {
            Owner = this
        };
        if (window.ShowDialog() == true)
        {
            _ = LoadPostsAsync();
        }
    }

    private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentPost is null)
        {
            MessageBox.Show("Odaberite objavu");
            return;
        }

        if (!_isStaff && _currentPost.AutorId != _userId)
        {
            MessageBox.Show("Nemate pravo uređivanja ove objave.");
            return;
        }

        var window = new PostEditWindow(_postsService, _userId, _isStaff, _currentPost)
        {
            Owner = this
        };
        if (window.ShowDialog() == true)
        {
            _ = LoadPostsAsync();
        }
    }

    private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentPost is null)
        {
            MessageBox.Show("Odaberite objavu");
            return;
        }

        if (!_isStaff && _currentPost.AutorId != _userId)
        {
            MessageBox.Show("Nemate pravo brisanja objave");
            return;
        }

        var confirm = MessageBox.Show("Jeste li sigurni da želite obrisati objavu?", "Potvrda brisanja",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        var result = await _postsService.DeleteAsync(_currentPost.Id, _userId, _isStaff);
        if (!result.IsSuccess)
        {
            MessageBox.Show(result.Error ?? "Greška kod brisanja objave");
            return;
        }

        await LoadPostsAsync();
        ClearDetails();
    }

    private void ShowDetails()
    {
        SpMainContent.Visibility = Visibility.Visible;
        EventFeedbackControl.Visibility = Visibility.Visible;
        SpInterests.Visibility = Visibility.Visible;
    }

    private void HideDetails()
    {
        SpMainContent.Visibility = Visibility.Collapsed;
        EventFeedbackControl.Visibility = Visibility.Collapsed;
        SpInterests.Visibility = Visibility.Collapsed;
        ImgEvent.Visibility = Visibility.Collapsed;
        ImgEvent.Source = null;
    }
}