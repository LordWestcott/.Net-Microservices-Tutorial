using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        //get all commands for a platform
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Getting Commands for Platform: {platformId}");

            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        //get a single command for a platform
        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Getting Command: {commandId} for Platform: {platformId}");

            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _repo.GetCommand(platformId, commandId);
            if (command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        //post - Create a command for a platform
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Creating Command for Platform: {platformId}");

            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandCreateDto);
            _repo.CreateCommand(platformId, command);
            _repo.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            //The reason we gave the GetCommandForPlatform method a name is so that we can use it here.
            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}