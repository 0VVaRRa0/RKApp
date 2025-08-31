namespace Accounting.Dtos;

public class ClientDto
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;
}
