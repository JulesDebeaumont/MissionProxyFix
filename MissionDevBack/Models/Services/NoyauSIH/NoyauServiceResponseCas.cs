
namespace MissionDevBack.Models;

public class NoyauServiceResponseCase
{
    public bool IsSuccess { get ; set; } = false;
    public List<string> Errors { get; set; } = [];
}