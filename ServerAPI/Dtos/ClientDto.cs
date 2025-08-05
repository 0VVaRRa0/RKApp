namespace ServerAPI.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
