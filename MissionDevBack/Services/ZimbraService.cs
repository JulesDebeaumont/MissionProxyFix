using System.Net;
using System.Text;
using System.Text.Json;
using MissionDevBack.Models;

namespace MissionDevBack.Services;

public class ZimbraService
{
    private readonly string ZimbraDomainUrl = "https://mail.chu-reims.fr/";
    private readonly string ZimbraLoginCsrf = "ZM_LOGIN_CSRF";
    private readonly string ZimbraAuthToken = "ZM_AUTH_TOKEN";

    public Task SynchronyseWithUserAcount()
    {

    }

    public async Task<ZimbraServiceResponseMails> GetLastMailsBySender(string senderEmail, int mailCount, string crsfToken)
    {
        var responseZimbra = new ZimbraServiceResponseMails();
        if (mailCount > 30)
        {
            mailCount = 30;
        }
        try
        {
            using HttpClientHandler handler = new();
            handler.CookieContainer = new CookieContainer();
            handler.CookieContainer.Add(new Cookie(ZimbraAuthToken, crsfToken));
            using (var client = new HttpClient())
            {
                var searchPayload = new StringContent(GetPayloadMailSearch(senderEmail, mailCount), Encoding.UTF8, "application/xml");
                var responseMail = await client.PostAsync($"{ZimbraDomainUrl}/service/soap/SearchRequest", searchPayload);
                responseMail.EnsureSuccessStatusCode();
                var stringResponse = await responseMail.Content.ReadAsStringAsync();
                
                JsonSerializer.Deserialize<object>(stringResponse);
                responseZimbra.Mails = [];
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

    private async Task<ZimbraServiceResponseToken> GetZimbraLoginCrsfToken()
    {
        var responseServiceZimbra = new ZimbraServiceResponseToken();
        try
        {
            using HttpClientHandler handler = new();
            handler.CookieContainer = new CookieContainer();
            using (var client = new HttpClient(handler))
            {
                var responseZimbra = await client.GetAsync($"{ZimbraDomainUrl}");
                responseZimbra.EnsureSuccessStatusCode();
                var cookies = handler.CookieContainer.GetCookies(new Uri($"{ZimbraDomainUrl}"));
                var crsfCookie = cookies.FirstOrDefault(cookie => cookie.Name == ZimbraLoginCsrf);
                if (crsfCookie == null)
                {
                    responseServiceZimbra.Errors.Add("Cookie for login not found");
                    return responseServiceZimbra;
                }
                responseServiceZimbra.CrsfToken = crsfCookie.Value;
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

    private async Task<ZimbraServiceResponseToken> GetUserZimbraCrsfToken(string email, string password)
    {
        var responseServiceZimbra = new ZimbraServiceResponseToken();
        var responseLoginCrsf = await GetZimbraLoginCrsfToken();
        if (!responseLoginCrsf.IsSuccess)
        {
            responseServiceZimbra.Errors = responseLoginCrsf.Errors;
            return responseServiceZimbra;
        }
        try
        {
            using HttpClientHandler handler = new();
            handler.CookieContainer = new CookieContainer();
            using (var client = new HttpClient(handler))
            {
                var XmlContent = new StringContent($"loginOp=login&login_crsf={responseLoginCrsf.CrsfToken}&username={email}&password={password}&client=preferred", Encoding.UTF8, "application/xml");
                var responseZimbra = await client.PostAsync($"{ZimbraDomainUrl}", XmlContent);
                responseZimbra.EnsureSuccessStatusCode();
                var cookies = handler.CookieContainer.GetCookies(new Uri($"{ZimbraDomainUrl}"));
                var crsfCookie = cookies.FirstOrDefault(cookie => cookie.Name == ZimbraLoginCsrf);
                if (crsfCookie == null)
                {
                    responseServiceZimbra.Errors.Add("Cookie for user crsf not found");
                    return responseServiceZimbra;
                }
                responseServiceZimbra.CrsfToken = crsfCookie.Value;
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

    private string GetPayloadMailSearch(string senderEmail, int mailCount)
    {
        var listOfSenderString = $"from:{senderEmail}";
        var objectPayload = new
        {
            Body = new
            {
                SearchRequest = new
                {
                    _jsns = "urn:zimbraMail",
                    sortBy = "dateDesc",
                    header = new object[3] {
                                new {
                                    n = "List-ID"
                                },
                                new {
                                    n = "X-Zimbra-DL"
                                },
                                new {
                                    n = "IN-REPLY-TO"
                                }
                            },
                    tz = new
                    {
                        id = "Europe/Brussels"
                    },
                    locale = new
                    {
                        _content = "fr_FR"
                    },
                    offset = 0,
                    mailCount = 100,
                    query = listOfSenderString,
                    types = "message",
                    recip = "0",
                    needExp = 1
                }
            }
        };
        return JsonSerializer.Serialize(objectPayload);
    }
}