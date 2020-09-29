namespace vtb.TemplatesService.BusinessLogic
{
    public class BusConfiguration
    {
        public bool InMemory { get; set; } = false;
        public string Host { get; set; } = "localhost";
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}