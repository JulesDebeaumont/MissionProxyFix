using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MissionDevBack.Models;

namespace MissionDevBack.Services;

public partial class MailBoxService
{
    private readonly string ZimbraUrl = "https://mail.chu-reims.fr/";
    private readonly string ZimbraDomain = "mail.chu-reims.fr";
    private readonly string ZimbraAuthToken = "ZM_AUTH_TOKEN";


    public async Task<ZimbraServiceResponseToken> GetMailBoxAuthToken(string email, string password)
    {
        var responseServiceZimbra = new ZimbraServiceResponseToken();
        try
        {
            using HttpClientHandler handler = new();
            var cookies = handler.CookieContainer = new CookieContainer();
            using (var client = new HttpClient(handler))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{ZimbraUrl}/service/soap");
                var formDataContent = new StringContent("{\"Header\": {\"context\": {\"_jsns\":\"urn:zimbra\",\"userAgent\":{\"name\":\"curl\",\"version\":\"7.54.0\"}}},\"Body\": {\"AuthRequest\": {\"_jsns\":\"urn:zimbraAccount\",\"account\":{\"_content\":\"" + email + "\",\"by\":\"name\"},\"password\": \"" + password + "\"}}}", Encoding.UTF8, "application/json");
                request.Content = formDataContent;
                var responseZimbra = await client.SendAsync(request);
                responseZimbra.EnsureSuccessStatusCode();
                var authTokenCookie = cookies.GetCookies(new Uri(ZimbraUrl)).FirstOrDefault(cookie => cookie.Name == ZimbraAuthToken);
                if (authTokenCookie == null)
                {
                    responseServiceZimbra.Errors.Add("Cookie for user crsf not found");
                    return responseServiceZimbra;
                }
                responseServiceZimbra.Token = authTokenCookie.Value;
            }
            responseServiceZimbra.IsSuccess = true;
            return responseServiceZimbra;
        }
        catch (Exception exception)
        {
            responseServiceZimbra.Errors.Add(exception.Message);
            return responseServiceZimbra;
        }
    }

    public async Task<ZimbraServiceResponseMailsPreview> GetLastMailsByFolderName(string mailFolderName, int mailCount, string authToken)
    {
        var responseZimbra = new ZimbraServiceResponseMailsPreview();
        var responseCrsf = await GetZimbraSessionCrsfToken(authToken);
        if (!responseCrsf.IsSuccess)
        {
            responseZimbra.Errors = responseCrsf.Errors;
            return responseZimbra;
        }
        if (mailCount > 100)
        {
            mailCount = 100;
        }
        try
        {
            var cookies = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookies })
            using (var client = new HttpClient(handler) { BaseAddress = new Uri(ZimbraUrl) })
            {
                var content = new StringContent(GetPayloadMailSearchInFolder(mailFolderName, mailCount, responseCrsf.Token), Encoding.UTF8, "application/soap+xml");
                cookies.Add(new Cookie(ZimbraAuthToken, authToken) { Domain = ZimbraDomain });
                var responseMail = await client.PostAsync("/service/soap/SearchRequest", content);
                responseMail.EnsureSuccessStatusCode();
                var stringResponse = await responseMail.Content.ReadAsStringAsync();
                responseZimbra.MailPreviews = JsonSerializer.Deserialize<ZimbraResponseMailPreview>(stringResponse).Body.SearchResponse.m;
            }
            responseZimbra.IsSuccess = true;
            return responseZimbra;
        }
        catch (Exception exception)
        {
            responseZimbra.Errors.Add(exception.Message);
            return responseZimbra;
        }
    }

    public async Task<ZimbraServiceResponseBlob> GetMailAttachmentBlob(string mailId, string part, string authToken)
    {
        var responseZimbra = new ZimbraServiceResponseBlob();
        try
        {
            var cookies = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookies })
            using (var client = new HttpClient(handler) { BaseAddress = new Uri(ZimbraUrl) })
            {
                cookies.Add(new Cookie(ZimbraAuthToken, authToken) { Domain = ZimbraDomain });
                var responseMail = await client.GetAsync($"service/home/~/?auth=co&loc=fr_FR&id={mailId}&part={part}");
                responseMail.EnsureSuccessStatusCode();
                responseZimbra.Blob = await responseMail.Content.ReadAsByteArrayAsync();
            }
            responseZimbra.IsSuccess = true;
            return responseZimbra;
        }
        catch (Exception exception)
        {
            responseZimbra.Errors.Add(exception.Message);
            return responseZimbra;
        }
    }
    
    public async Task<ZimbraServiceResponseMail> GetMailById(string mailId, string authToken)
    {
        var responseZimbra = new ZimbraServiceResponseMail();
        var responseCrsf = await GetZimbraSessionCrsfToken(authToken);
        if (!responseCrsf.IsSuccess)
        {
            responseZimbra.Errors = responseCrsf.Errors;
            return responseZimbra;
        }
        try
        {
            var cookies = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookies })
            using (var client = new HttpClient(handler) { BaseAddress = new Uri(ZimbraUrl) })
            {
                var content = new StringContent(GetPayloadMailFind(mailId, responseCrsf.Token), Encoding.UTF8, "application/soap+xml");
                cookies.Add(new Cookie(ZimbraAuthToken, authToken) { Domain = ZimbraDomain });
                var responseMail = await client.PostAsync("/service/soap/GetMsgRequest", content);
                responseMail.EnsureSuccessStatusCode();
                var stringResponse = await responseMail.Content.ReadAsStringAsync();
                responseZimbra.Mail = JsonSerializer.Deserialize<ZimbraResponseMail>(stringResponse).Body.GetMsgResponse.m.First();
            }
            responseZimbra.IsSuccess = true;
            return responseZimbra;
        }
        catch (Exception exception)
        {
            responseZimbra.Errors.Add(exception.Message);
            return responseZimbra;
        }
    }

    private async Task<ZimbraServiceResponseToken> GetZimbraSessionCrsfToken(string authToken)
    {
        var responseServiceZimbra = new ZimbraServiceResponseToken();
        try
        {
            var cookies = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookies })
            using (var client = new HttpClient(handler))
            {
                cookies.Add(new Cookie(ZimbraAuthToken, authToken) { Domain = ZimbraDomain });
                var responseZimbra = await client.GetAsync($"{ZimbraUrl}");
                responseZimbra.EnsureSuccessStatusCode();
                var responseString = await responseZimbra.Content.ReadAsStringAsync();
                var crsfToken = CrsfTokenRegex().Match(responseString).Groups?[0].Value.Split("\"")?[1] ?? null;
                if (crsfToken is null)
                {
                    responseServiceZimbra.Errors.Add("crsfToken not found in Zimbra response");
                    return responseServiceZimbra;
                }
                responseServiceZimbra.Token = crsfToken;
            }
            responseServiceZimbra.IsSuccess = true;
            return responseServiceZimbra;
        }
        catch (Exception exception)
        {
            responseServiceZimbra.Errors.Add(exception.Message);
            return responseServiceZimbra;
        }
    }

    private string GetPayloadMailSearchInFolder(string mailFolderName, int mailCount, string crsfToken)
    {
        var mailInFolderFilter = "in:" + mailFolderName;
        var stringTruc = "{\"Header\":{\"context\":{\"_jsns\":\"urn:zimbra\",\"csrfToken\":\"" + crsfToken + "\"}},\"Body\":{\"SearchRequest\":{\"_jsns\":\"urn:zimbraMail\",\"sortBy\":\"dateDesc\",\"header\":[{\"n\":\"List-ID\"},{\"n\":\"X-Zimbra-DL\"},{\"n\":\"IN-REPLY-TO\"}],\"tz\":{\"id\":\"Europe/Brussels\"},\"locale\":{\"_content\":\"fr_FR\"},\"offset\":0,\"limit\":" + mailCount + ",\"query\":\"" + mailInFolderFilter + "\",\"types\":\"message\",\"recip\":\"0\",\"needExp\":1}}}";
        return stringTruc;
    }

    private string GetPayloadMailFind(string mailId, string crsfToken)
    {
        var stringTruc = "{\"Header\":{\"context\":{\"_jsns\":\"urn:zimbra\",\"csrfToken\":\"" + crsfToken + "\"}},\"Body\":{\"GetMsgRequest\":{\"_jsns\":\"urn:zimbraMail\",\"m\":{\"id\":\"" + mailId + "\",\"html\":1,\"needExp\":1,\"header\":[{\"n\":\"List-ID\"},{\"n\":\"X-Zimbra-DL\"},{\"n\":\"IN-REPLY-TO\"}],\"max\":250000}}}}";
        return stringTruc;
    }

    [GeneratedRegex(@"window.csrfToken\s*=\s*""(.*?)""")]
    private static partial Regex CrsfTokenRegex();
}