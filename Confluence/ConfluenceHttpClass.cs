namespace AdformMcp.Confluence;

using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;


public static class ConfluenceHttpClient
{
    public static async Task<string> GetPageById(string id, PrimaryBodyRepresentation? bodyFormat = null)
    {
        var url = ConfluenceUrl.GetPageById(id, bodyFormat);

        using var client = CreateClient();

        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient();
        var authenticationString = $"{ConfluenceSecret.Email}:{ConfluenceSecret.Token}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        return client;
    }
}
