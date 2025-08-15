namespace Accounting.Dtos
{
    public class ClientDetailDto : ClientDto
    {
        public string DisplayName => $"{Login} | {FullName} | ID: {Id}";
    }
}
