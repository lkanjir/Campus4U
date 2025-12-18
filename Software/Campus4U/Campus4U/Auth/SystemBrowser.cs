using System.Diagnostics;
using System.Net;
using Duende.IdentityModel.OidcClient.Browser;

namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed class SystemBrowser : IBrowser
{
    private const string ErrorMsg = "Greška";
    private const string SuccessMsg = "Uspješna prijava";
    private const string SuccessLoguotMsg = "Uspješna odjava";

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
            using (cancellationToken.Register(() => { httpListener?.Abort(); }))
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
                    msg = SuccessLoguotMsg;
                    result.ResultType = BrowserResultType.Success;
                }
                else
                {
                    msg = ErrorMsg;
                    result.ResultType = BrowserResultType.UnknownError;
                }

                Byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                context.Response.Close();
                httpListener.Stop();
            }
        }
        return result;
    }
}