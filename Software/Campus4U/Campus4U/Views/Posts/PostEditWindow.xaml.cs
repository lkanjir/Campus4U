using System.Globalization;
using System.IO;
using System.Windows;
using Client.Application.Images;
using Client.Application.Posts;
using Client.Data.Auth;
using Client.Data.Images;
using Client.Domain.Posts;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace Client.Presentation.Views.Posts;

//Luka Kanjir
public partial class PostEditWindow
{
    private readonly PostsService _postsService;
    private readonly int _userId;
    private readonly bool _isStaff;
    private readonly PostDetail? _editingPost;
    private readonly IImageService _imageService;
    private string? _selectedImagePath;

    public PostEditWindow(PostsService postsService, int userId, bool isStaff)
    {
        InitializeComponent();
        this._postsService = postsService;
        this._userId = userId;
        this._isStaff = isStaff;
        Title = "Nova objava";
        DpDate.SelectedDate = DateTime.Today;
        TxtTime.Text = DateTime.Now.ToString("HH:mm");

        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
        var apiBaseUrl = config["Api:BaseUrl"];
        if (string.IsNullOrWhiteSpace(apiBaseUrl))
            throw new InvalidOperationException("API: BaseUrl nije definiran u appsettings.json");

        var tokenStore = new SecureTokenStore();
        _imageService = new ImageService(new ImageApiClient(apiBaseUrl, tokenStore), new ImageCache(),
            TimeSpan.FromMinutes(20));
    }

    public PostEditWindow(PostsService postsService, int userId, bool isStaff, PostDetail post) : this(postsService,
        userId, isStaff)
    {
        _editingPost = post;
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
        var title = TxtTitle.Text.Trim();
        var body = TxtBody.Text.Trim();
        var date = DpDate.SelectedDate;
        if (date is null)
        {
            MessageBox.Show("Datum događaja je obavezan");
            return;
        }

        if (!TryParseTime(TxtTime.Text, out var time))
        {
            MessageBox.Show("Vrijeme mora biti u formatu sati:minute, npr. 05:32");
            return;
        }

        var eventDate = date.Value.Date + time;
        int? savedId;
        if (_editingPost is null)
        {
            var result = await _postsService.CreateAsync(new PostCreateRequest(title, body, eventDate), _userId);
            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Error ?? "Greška kod spremanja objave");
                return;
            }

            savedId = result.PostId;
        }
        else
        {
            var result = await _postsService.UpdateAsync(_editingPost.Id, new PostUpdateRequest(title, body, eventDate),
                _userId, _isStaff);
            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Error ?? "Greška kod ažuriranja objave");
                return;
            }

            savedId = _editingPost.Id;
        }

        if (savedId is not null) await TryUploadSelectedImageAsync(savedId.Value);

        DialogResult = true;
    }

    private async Task TryUploadSelectedImageAsync(int eventId)
    {
        if (string.IsNullOrWhiteSpace(_selectedImagePath)) return;

        try
        {
            var fileInfo = new FileInfo(_selectedImagePath);
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

            var bytes = await File.ReadAllBytesAsync(_selectedImagePath);
            await using var stream = new MemoryStream(bytes, writable: false);
            var upload = new ImageUpload(stream, contentType, bytes.LongLength, fileInfo.Name);
            await _imageService.UploadEventImageAsync(eventId, upload);
        }
        catch
        {
            MessageBox.Show("Greška pri uploadu slike.");
        }
    }

    private static string? GetContentType(string extension) => extension.Trim().ToLowerInvariant() switch
    {
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        _ => null
    };

    private static bool TryParseTime(string? value, out TimeSpan time) =>
        TimeSpan.TryParseExact(value?.Trim(), ["h\\:mm", "hh\\:mm", "hh\\:m", "h\\:m"], CultureInfo.InvariantCulture,
            out time);

    private void BtnSelectImage_OnClick(object sender, RoutedEventArgs e)
    {
        var fileExplorer = new OpenFileDialog
        {
            Title = "Odaberi sliku događaja",
            Filter = "Image files (*.png;*.jpeg;*.jpg;*.webp)|*.png;*.jpeg;*.jpg;*.webp",
            Multiselect = false
        };

        if (fileExplorer.ShowDialog() != true) return;
        _selectedImagePath = fileExplorer.FileName;
        TxtSelectedImage.Text = Path.GetFileName(_selectedImagePath);
    }
}