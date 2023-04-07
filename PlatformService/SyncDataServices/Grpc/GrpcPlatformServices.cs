using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
                /* 
                    Be aware that code completion will not work for
                    GrpcPlatform.GrpcPlatformBase until you have
                    built the project with the .proto file implemented. 
                    This is because it is a generated class.
                */
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _repo.GetAllPlatforms();
            
            foreach(var plat in platforms)
            {
                response.Platforms.Add(_mapper.Map<GrpcPlatformModel>(plat));
            }

            return Task.FromResult(response);
        }

    }
}