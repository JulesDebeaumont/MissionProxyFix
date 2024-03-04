namespace MissionDevBack.Models;

public class GetProjectDTOOut
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public EProjectState State { get; set; }

    public DateTime? Deadline { get; set; }

    public ICollection<ProjectUserDTO> ProjectUsers { get; set; }

    public class ProjectUserDTO
    {
        public string Id { get; set; }

        public string Fullname { get; set; }
    }
}