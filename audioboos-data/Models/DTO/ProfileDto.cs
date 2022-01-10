namespace AudioBoos.Data.Models.DTO;

public record ProfileDto {
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string Email { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }
};
