
namespace MissionDevBack.Models;

public class NoyauServiceResponseCas
{
    public bool IsSuccess { get ; set; } = false;

    public string UserIdRes { get; set; }

    public List<string> Errors { get; set; } = [];
}