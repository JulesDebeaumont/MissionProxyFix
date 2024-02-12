using MissionDevBack.Models;

namespace MissionDevBack.Services;

public class NoyauSihService
{
    private readonly string CasUrl = "https://cas.chu-reims.fr";
    private readonly string NoyauSihUrl = "http://noyausih.domchurs.ad";
    private readonly string ResponseCasOk = "yes";
    private readonly string ApplicationCI = "CI0841";

    async public Task<NoyauServiceResponseCas> VerifyTicketFromCas(string ticket, string service)
    {
        var responseServiceNoyau = new NoyauServiceResponseCas();
        try
        {
            using (var client = new HttpClient())
            {
                var responseCas = await client.GetAsync($"{CasUrl}/validate?service={service}&ticket={ticket}");
                responseCas.EnsureSuccessStatusCode();
                var responseCasArray = responseCas.Content.ToString().Split("\n");
                if (responseCasArray.First() != ResponseCasOk)
                {
                    responseServiceNoyau.Errors.Add("CAS did not authorize");
                    return responseServiceNoyau;
                }
                responseServiceNoyau.IsSuccess = true;
                responseServiceNoyau.UserIdRes = responseCasArray.Last();
            }

            return responseServiceNoyau;
        }
        catch (Exception exception)
        {
            responseServiceNoyau.Errors.Add(exception.Message);
            return responseServiceNoyau;
        }
    }

    async public Task<NoyauServiceResponseUser> GetUserFromNoyauSih(string userIdRes)
    {
        var responseServiceNoyau = new NoyauServiceResponseUser();
        try
        {
            using (var client = new HttpClient())
            {
                var responseNoyau = await client.GetAsync($"{NoyauSihUrl}/iam{userIdRes}/{ApplicationCI}/habilitations.json");
                responseNoyau.EnsureSuccessStatusCode();
                responseServiceNoyau.NoyauSihUser = await responseNoyau.Content.ReadFromJsonAsync<NoyauSihUser>();
            }

            responseServiceNoyau.IsSuccess = true;
            return responseServiceNoyau;
        }
        catch (Exception exception)
        {
            responseServiceNoyau.Errors.Add(exception.Message);
            return responseServiceNoyau;
        }
    }
}
