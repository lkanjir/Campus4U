using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Client.Application.Auth;
using Duende.IdentityModel.OidcClient.Browser;

namespace Client.Data.Auth;

//Luka Kanjir
public sealed class SystemBrowser : IBrowser
{
    private HttpListener httpListener;

    private void StartSystemBrowser(string startUrl) =>
        Process.Start(new ProcessStartInfo(startUrl) { UseShellExecute = true });

    public async Task<BrowserResult> InvokeAsync(BrowserOptions options,
        CancellationToken cancellationToken = default)
    {
        var result = new BrowserResult();

        httpListener?.Abort();
        using (httpListener = new HttpListener())
        {
            var url = options.EndUrl;
            if (!url.EndsWith("/")) url += "/";

            httpListener.Prefixes.Add(url);
            httpListener.Start();
            StartSystemBrowser(options.StartUrl);
            await using (cancellationToken.Register(() => { httpListener?.Abort(); }))
            {
                HttpListenerContext context;
                try
                {
                    context = await httpListener.GetContextAsync();
                }
                catch (HttpListenerException)
                {
                    result.ResultType = BrowserResultType.UnknownError;
                    return result;
                }

                result.Response = context.Request.Url.AbsoluteUri;
                var isLogoutFlow = options.StartUrl.Contains("/logout", StringComparison.OrdinalIgnoreCase);
                var isSuccess = context.Request.QueryString.Get("code") != null ||
                                (isLogoutFlow && context.Request.Url.AbsoluteUri == options.EndUrl);
                result.ResultType = isSuccess ? BrowserResultType.Success : BrowserResultType.UnknownError;

                var html = await BuildHtml(isLogoutFlow, isSuccess);
                var buffer = Encoding.UTF8.GetBytes(html);
                context.Response.ContentType = "text/html";
                await context.Response.OutputStream.WriteAsync(buffer, cancellationToken);
                context.Response.OutputStream.Close();
                context.Response.Close();

                var drainUntil = DateTimeOffset.UtcNow.AddSeconds(2);
                while (DateTimeOffset.UtcNow < drainUntil)
                {
                    try
                    {
                        var delay = Task.Delay(200, cancellationToken);
                        var ctxTask = httpListener.GetContextAsync();
                        var completed = await Task.WhenAny(ctxTask, delay);
                        if (completed != ctxTask) continue;

                        var extra = ctxTask.Result;
                        extra.Response.StatusCode = (int)HttpStatusCode.NoContent;
                        extra.Response.Close();
                    }
                    catch (HttpListenerException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }
                httpListener.Stop();
            }
        }

        return result;
    }

    private static async Task<string> BuildHtml(bool isLogoutFlow, bool isSuccess)
    {
        string title, description;
        if (isLogoutFlow)
        {
            title = isSuccess ? "Uspješna odjava" : "Greška prilikom odjave";
            description = isSuccess
                ? "Uspješno ste se odjavili."
                : "Došlo je do greške prilikom odjave. Kako bi ponovno koristili aplikaciju morate se ponovno prijaviti.";
        }
        else
        {
            title = isSuccess ? "Uspješna prijava" : "Greška kod prijave";
            description = isSuccess
                ? "Uspješno ste se prijavili u aplikaciju Campus4U. Možete se vratiti u aplikaciju"
                : "Došlo je do greške prilikom prijave. Kako bi koristili aplikaciju morate se ponovno prijaviti.";
        }

        var template = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Auth", "Template.html"),
            Encoding.UTF8);
        template = template.Replace("{{title}}", title).Replace("{{description}}", description);
        return template;
    }
}