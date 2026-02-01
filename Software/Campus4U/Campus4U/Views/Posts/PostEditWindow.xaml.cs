using System.Globalization;
using System.Windows;
using Client.Application.Posts;
using Client.Domain.Posts;

namespace Client.Presentation.Views.Posts;

//Luka Kanjir
public partial class PostEditWindow : Window
{
    private readonly PostsService postsService;
    private readonly int userId;
    private readonly bool isStaff;
    private readonly PostDetail? editingPost;

    public PostEditWindow(PostsService postsService, int userId, bool isStaff)
    {
        InitializeComponent();
        this.postsService = postsService;
        this.userId = userId;
        this.isStaff = isStaff;
        Title = "Nova objava";
        DpDate.SelectedDate = DateTime.Today;
        TxtTime.Text = DateTime.Now.ToString("HH:mm");
    }

    public PostEditWindow(PostsService postsService, int userId, bool isStaff, PostDetail post) : this(postsService,
        userId, isStaff)
    {
        editingPost = post;
        Title = "Uredi objavu";
        TxtTitle.Text = post.Naslov;
        TxtBody.Text = post.Sadrzaj;
        DpDate.SelectedDate = post.DatumVrijemeDogadaja.Date;
        TxtTime.Text = post.DatumVrijemeDogadaja.ToString("HH:mm");
    }

    private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
    {
        var title = TxtTitle.Text?.Trim() ?? string.Empty;
        var body = TxtBody.Text?.Trim() ?? string.Empty;
        var date = DpDate.SelectedDate;
        if (date is null)
        {
            MessageBox.Show("Datum događaja je obavezan");
            return;
        }

        if (!TryParseTime(TxtTime.Text, out var time))
        {
            MessageBox.Show("Vrijeme mora biti u formatu sati:minute s vodećim nulama, npr. 05:32");
            return;
        }

        var eventDate = date.Value.Date + time;
        if (editingPost is null)
        {
            var result = await postsService.CreateAsync(new PostCreateRequest(title, body, eventDate), userId);
            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Error ?? "Greška kod spremanja objave");
                return;
            }
        }
        else
        {
            var result = await postsService.UpdateAsync(editingPost.Id, new PostUpdateRequest(title, body, eventDate),
                userId, isStaff);
            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Error ?? "Greška kod ažuriranja objave");
                return;
            }
        }

        DialogResult = true;
    }

    private static bool TryParseTime(string? value, out TimeSpan time) =>
        TimeSpan.TryParseExact(value?.Trim(), ["h\\:mm", "hh\\:mm", "hh\\:m", "h\\:m"], CultureInfo.InvariantCulture,
            out time);
}