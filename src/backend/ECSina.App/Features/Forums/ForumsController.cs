using AutoMapper;
using ECSina.App.ApiModel;
using ECSina.Core.Features.Forums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECSina.App.Features.Forums;

[ApiController]
[Route("[controller]")]
public class ForumsController : ControllerBase
{
    #region Constructor and dependencies

    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ForumsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    #endregion

    public sealed class CreateForumRequestDto
    {
        public required string Name { get; set; }
        public Guid? ParentId { get; set; }
    }

    [Authorize]
    [HttpPost]
    public async Task<ApiDataEntity> CreateForum(CreateForumRequestDto dto)
    {
        var response = await _mediator.Send(
            new CreateForum { Name = dto.Name, ParentId = dto.ParentId }
        );

        return _mapper.Map<ApiDataEntity>(response.Entity);
    }
}
