using PlatformService.Models;

namespace PlatformService.Data 
{
    public interface IPlatformRepo
    {
        bool SaveChanges(); // Save changes to the DB

        IEnumerable<Platform> GetAllPlatforms();
        Platform GetPlatformById(int id);
        void CreatePlatform(Platform plat);
        
    }
}