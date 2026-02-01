using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ObjectiveC;
using System.Windows;
using System.Windows.Controls;
using Client.Application.Posts;
using Client.Data.Posts;
using Client.Domain.Posts;

namespace Client.Presentation.Views.Posts;

//Luka Kanjir
public partial class PostsWindow : Window
{
    private readonly int userId;
    private readonly bool isStaff;
    private readonly PostsService postsService;
    private readonly InterestsService interestsService;

    private PostDetail? currentPost;
    private bool isInterested;

    public ObservableCollection<PostListItem> Posts { get; } = [];

    public PostsWindow(int userId, bool isStaff)
    {
        InitializeComponent();
        DataContext = this;
        this.userId = userId;
        this.isStaff = isStaff;

        var postsRepository = new PostsRepository();
        postsService = new PostsService(postsRepository);
        interestsService = new InterestsService(new InterestsRepository(), postsRepository);
        UpdateActionButtons();
    }

    private void UpdateActionButtons()
    {
        var canEditDelete = currentPost is not null && (isStaff || currentPost.AutorId == userId);
        BtnEdit.Visibility = canEditDelete ? Visibility.Visible : Visibility.Collapsed;
        BtnDelete.Visibility = canEditDelete ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void PostsWindow_OnLoaded(object sender, RoutedEventArgs e) => await LoadPostsAsync();

    private async Task LoadPostsAsync()
    {
        try
        {
            var items = await postsService.GetAllAsync();
            Posts.Clear();
            foreach (var item in items)
            {
                Posts.Add(item);
            }

            SelectPost(currentPost?.Id);
            if (PostsList.SelectedItem is null) ClearDetails();
        }
        catch (Exception ex)
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
            currentPost = await postsService.GetByIdAsync(selectedId);
            if (PostsList.SelectedItem is not PostListItem currentSelection ||
                currentSelection.Id != selectedId) return;
            if (currentPost is null)
            {
                ClearDetails();
                return;
            }

            TxtTitle.Text = currentPost.Naslov;
            TxtAuthor.Text = string.IsNullOrWhiteSpace(currentPost.AutorIme)
                ? "Autor: -"
                : $"Autor: {currentPost.AutorIme}";
            var eventDateText = currentPost.DatumVrijemeDogadaja == default
                ? "-"
                : currentPost.DatumVrijemeDogadaja.ToString("dd.MM.yyyy. HH:mm");
            TxtDates.Text = $"Objava: {currentPost.DatumVrijemeObjave:dd.MM.yyyy.} | Događaj: {eventDateText}";
            TxtBody.Text = currentPost.Sadrzaj;

            await UpdateInterestsStateAsync();
            await UpdateCommentsAsync();
            UpdateActionButtons();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greska kod učitavanja objave");
        }
    }

    private async Task UpdateCommentsAsync()
    {
        if (currentPost is null || currentPost.DatumVrijemeDogadaja == default)
        {
            EventFeedbackControl.Clear();
            EventFeedbackControl.IsEnabled = false;
            EventFeedbackControl.Visibility = Visibility.Collapsed;
            return;
        }

        EventFeedbackControl.Visibility = Visibility.Visible;
        EventFeedbackControl.IsEnabled = true;
        await EventFeedbackControl.LoadAsync(currentPost.Id, userId);
    }

    private async Task UpdateInterestsStateAsync()
    {
        if (currentPost is null)
        {
            BtnToggleInterests.IsEnabled = false;
            TxtInterestStatus.Text = string.Empty;
            return;
        }

        if (currentPost.DatumVrijemeDogadaja == default)
        {
            BtnToggleInterests.IsEnabled = false;
            TxtInterestStatus.Text = "Neispravan datum događaja";
            return;
        }

        isInterested = await interestsService.IsInterestedAsync(currentPost.Id, userId);
        BtnToggleInterests.IsEnabled = true;
        BtnToggleInterests.Content = isInterested ? "Ne zanima me" : "Zanima me";
        TxtInterestStatus.Text = isInterested ? "Interes označen" : "Interes nije označen";
    }

    private void ClearDetails()
    {
        currentPost = null;
        TxtTitle.Text = "Odaberi objavu";
        TxtAuthor.Text = string.Empty;
        TxtDates.Text = string.Empty;
        TxtBody.Text = string.Empty;
        BtnToggleInterests.IsEnabled = false;
        TxtInterestStatus.Text = string.Empty;
        EventFeedbackControl.Clear();
        EventFeedbackControl.IsEnabled = false;
        EventFeedbackControl.Visibility = Visibility.Collapsed;
        UpdateActionButtons();
    }

    private async void BtnToggleInterests_OnClick(object sender, RoutedEventArgs e)
    {
        if (currentPost is null) return;

        var result = await interestsService.ToggleAsync(currentPost.Id, userId);
        if (!result.IsSuccess)
        {
            MessageBox.Show(result.Error ?? "Greška kod ažuriranja interesa");
            return;
        }

        isInterested = result.IsInterested;
        BtnToggleInterests.Content = isInterested ? "Ne zanima me" : "Zanima me";
        TxtInterestStatus.Text = isInterested ? "Interes označen" : "Interes nije označen";
    }

    private void BtnNew_OnClick(object sender, RoutedEventArgs e)
    {
        var window = new PostEditWindow(postsService, userId, isStaff)
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
        if (currentPost is null)
        {
            MessageBox.Show("Odaberite objavu");
            return;
        }

        if (!isStaff && currentPost.AutorId != userId)
        {
            MessageBox.Show("Nemate pravo uređivanja ove objave.");
            return;
        }

        var window = new PostEditWindow(postsService, userId, isStaff, currentPost)
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
        if (currentPost is null)
        {
            MessageBox.Show("Odaberite objavu");
            return;
        }

        if (!isStaff && currentPost.AutorId != userId)
        {
            MessageBox.Show("Nemate pravo brisanja objave");
            return;
        }

        var confirm = MessageBox.Show("Jeste li sigurni da želite obrisati objavu?", "Potvrda brisanja",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        var result = await postsService.DeleteAsync(currentPost.Id, userId, isStaff);
        if (!result.IsSuccess)
        {
            MessageBox.Show(result.Error ?? "Greška kod brisanja objave");
            return;
        }

        await LoadPostsAsync();
        ClearDetails();
    }
}