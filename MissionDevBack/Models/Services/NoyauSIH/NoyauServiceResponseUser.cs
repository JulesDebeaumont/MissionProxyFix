namespace MissionDevBack.Models;

public class NoyauServiceResponseUser
{
    public bool IsSuccess { get; set; } = false;

    public List<string> Errors { get; set; } = [];

    public NoyauSihUser NoyauSihUser { get; set; }
}