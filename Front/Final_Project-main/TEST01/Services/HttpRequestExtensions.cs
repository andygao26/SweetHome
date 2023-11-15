using Microsoft.AspNetCore.Http;

public static class HttpRequestExtensions
{

    //EmailList搜尋用
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException("request");
        }

        if (request.Headers != null)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        return false;
    }
}