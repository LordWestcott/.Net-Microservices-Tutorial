using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    /*
        We have added a /c/ here as this controller shares a name with a controller in the platformservice.
        This is fine, as the platformservice is a separate microservice, and the path will be different.
        However, this may lead to trouble one we get to the API Gateway.

        c - denotes commands.
    */
    [Route("api/c/[controller]")] 
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms from CommandService");      
            var platformItems = _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }


        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # CommandsService");
            return Ok("Inbound test of from Platforms Controller");
        }
    }
}