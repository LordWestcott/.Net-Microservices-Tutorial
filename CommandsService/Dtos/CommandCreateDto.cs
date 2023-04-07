namespace CommandsService.Dtos
{
    public class CommandCreateDto
    {
        //PlatformId is acquired from the route
        public string HowTo { get; set; }
        public string CommandLine { get; set; }
    }
}