using ECSina.Core.Base;
using ECSina.Db;
using ECSina.Db.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECSina.Core.Features.Entities;

public sealed class GetEntities : QueryBase<List<DataEntity>>
{
    public sealed class Handler : IRequestHandler<GetEntities, List<DataEntity>>
    {
        #region Constructor and dependencies

        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        public async Task<List<DataEntity>> Handle(
            GetEntities request,
            CancellationToken cancellationToken
        )
        {
            var dataEntities = await _dbContext
                .Set<DataEntity>()
                .Include(x => x.Components)
                .ToListAsync(cancellationToken);

            return dataEntities;
        }
    }
}
