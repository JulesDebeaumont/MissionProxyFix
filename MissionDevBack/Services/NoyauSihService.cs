using MissionDevBack.Models;

namespace MissionDevBack.Services;

public class NoyauSihService
{
    private readonly string CasUrl = "";
    private readonly string NoyauSihUrl = "";

    async public Task<NoyauServiceResponseCase> VerifyTicketFromCas(string ticket, string service)
    {
        var responseServiceNoyau = new NoyauServiceResponseCase();
        try
        {
            using HttpClient client = new();
            var responseCas = await client.GetAsync($"{CasUrl}/validate?service={service}&ticket={ticket}");
            responseCas.EnsureSuccessStatusCode();
            if (responseCas.Content.ToString() != "yes")
            {
                responseServiceNoyau.Errors.Add("Cas did not authorize");
                return responseServiceNoyau;
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
    
    async public Task<NoyauServiceResponseUser> GetUserFromNoyauSih(string userIdRes)
    {
        var responseServiceNoyau = new NoyauServiceResponseUser();
        try
        {
            using HttpClient client = new();
            var responseNoyau = await client.GetAsync($"{NoyauSihUrl}/TODO{userIdRes}");
            responseNoyau.EnsureSuccessStatusCode();
            responseServiceNoyau.NoyauSihUser = await responseNoyau.Content.ReadFromJsonAsync<NoyauSihUser>();
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
