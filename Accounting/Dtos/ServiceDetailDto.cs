namespace Accounting.Dtos
{
    public class ServiceDetailDto : ServiceDto
    {
        public string DisplayName => $"{Name} | ID: {Id}";
    }
}
