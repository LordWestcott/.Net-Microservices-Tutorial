using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public interface ICommandDataClient
    {
        // This method will send the platform to the command service using the platform DTO - id.
        Task SendPlatformToCommand(PlatformReadDto plat);
    }
}