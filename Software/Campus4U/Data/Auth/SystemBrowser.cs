using System.Diagnostics;
using System.Net;
using Client.Application.Auth;
using Duende.IdentityModel.OidcClient.Browser;

namespace Client.Data.Auth;

//Luka Kanjir
public sealed class SystemBrowser : IBrowser, ISystemBrowser
{
    private const string ErrorMsg = "Greška";
    private const string SuccessMsg = "Uspješna prijava";
    private const string SuccessLogoutMsg = "Uspješna odjava";

    private HttpListener httpListener;

    private void StartSystemBrowser(string startUrl) =>
        Process.Start(new ProcessStartInfo(startUrl) { UseShellExecute = true });

    public async Task<BrowserResult> InvokeAsync(BrowserOptions options,
        CancellationToken cancellationToken = default)
    {
        StartSystemBrowser(options.StartUrl);
        var result = new BrowserResult();

        httpListener?.Abort();
        using (httpListener = new HttpListener())
        {
            var url = options.EndUrl;
            if (!url.EndsWith("/")) url += "/";

            httpListener.Prefixes.Add(url);
            httpListener.Start();
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

                string msg;
                if (context.Request.QueryString.Get("code") != null)
                {
                    msg = SuccessMsg;
                    result.ResultType = BrowserResultType.Success;
                }
                else if (options.StartUrl.Contains("/logout") && context.Request.Url.AbsoluteUri == options.EndUrl)
                {
                    msg = SuccessLogoutMsg;
                    result.ResultType = BrowserResultType.Success;
                }
                else
                {
                    msg = ErrorMsg;
                    result.ResultType = BrowserResultType.UnknownError;
                }

                Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, cancellationToken);
                context.Response.OutputStream.Close();
                context.Response.Close();
                httpListener.Stop();
            }
        }

        return result;
    }

    public async Task<SystemBrowserResult> OpenAsync(string startUrl, string endUrl, CancellationToken ct = default)
    {
        var result = await InvokeAsync(new BrowserOptions(startUrl, endUrl), ct);
        return result.ResultType == BrowserResultType.Success
            ? new SystemBrowserResult(true, result.Response, null)
            : new SystemBrowserResult(false, result.Response, result.Error ?? result.ResultType.ToString());
    }
}