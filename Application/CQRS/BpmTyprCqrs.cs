using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Daroo;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    public record GetAllBpmTypeQuery : IRequest<List<BpmType>>;

    public class GetAllBpmTypeQueryHandler : IRequestHandler<GetAllBpmTypeQuery, List<BpmType>>
    {
        private readonly DarooDbContext _context;

        public GetAllBpmTypeQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<BpmType>> Handle(GetAllBpmTypeQuery request, CancellationToken cancellationToken)
        {
            return await _context.BpmTypes
                .Where(pt => pt.IsDeleted != true)
                .OrderBy(pt => pt.Name)
                .Select(pt => new BpmType
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    CreateDate = pt.CreateDate,
                    ModifyDate = pt.ModifyDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }
}
