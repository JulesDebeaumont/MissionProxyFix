namespace MissionDevBack.Models;

public class GetProjectsDTOOut
{
    public int Id { get; set; }

    public string Title { get; set; }

    public DateTime? Deadline { get; set; }

    public EProjectState State { get; set; }

    public ICollection<UserDTO> Users { get; set; }

    public class UserDTO
    {
        public string Id { get; set; }

        public string Fullname { get; set; }
    }
}