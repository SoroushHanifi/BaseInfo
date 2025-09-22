using Application.Common;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    public class ScopeDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public long? CreateUserID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int FinalEnt { get; set; }
        public long BaCreatedTime { get; set; }
        public Guid BaGuid { get; set; }

        public DateTime? BaCreatedDateTime => BaCreatedTime > 0
            ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(BaCreatedTime)
            : null;
    }

    public class ScopeSimpleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long? DepartementId { get; set; }
    }

    // ===== GET ALL SCOPES QUERY =====
    public record GetAllScopesQuery : IRequest<List<ScopeDto>>;

    public class GetAllScopesQueryHandler : IRequestHandler<GetAllScopesQuery, List<ScopeDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllScopesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScopeDto>> Handle(GetAllScopesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Scopes
                .Include(s => s.Department)
                .Where(s => s.IsDeleted != true)
                .OrderBy(s => s.Department.Name)
                .ThenBy(s => s.Name)
                .Select(s => new ScopeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    CreateUserID = s.CreateUserID,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    IsDeleted = s.IsDeleted,
                    FinalEnt = s.FinalEnt,
                    BaCreatedTime = s.BaCreatedTime,
                    BaGuid = s.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }


    public record GetAllScopesPaginationQuery(int PageIndex = 1, int PageSize = 10) : IRequest<PagedData<ScopeDto>>;

    public class GetAllScopespaginationQueryHandler : IRequestHandler<GetAllScopesPaginationQuery, PagedData<ScopeDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllScopespaginationQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<ScopeDto>> Handle(GetAllScopesPaginationQuery request, CancellationToken cancellationToken)
        {

            int pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            int totalCount = await _context.Scopes
                .Include(s => s.Department)
                .Where(s => s.IsDeleted != true)
                .CountAsync(cancellationToken);

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _context.Scopes
                .Where(d => d.IsDeleted != true)
                .OrderBy(d => d.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new ScopeDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserID = d.CreateUserID,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDeleted = d.IsDeleted,
                    FinalEnt = d.FinalEnt,
                    BaCreatedTime = d.BaCreatedTime,
                    BaGuid = d.BaGuid
                })
                .ToListAsync(cancellationToken);

            return new PagedData<ScopeDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                IndexFrom = (pageIndex - 1) * pageSize,
                Items = items,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }
    }

    public record GetScopeByIdQuery(long Id) : IRequest<ScopeDto?>;

    public class GetScopeByIdQueryHandler : IRequestHandler<GetScopeByIdQuery, ScopeDto?>
    {
        private readonly DarooDbContext _context;

        public GetScopeByIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<ScopeDto?> Handle(GetScopeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Scopes
                .Include(s => s.Department)
                .Where(s => s.Id == request.Id && s.IsDeleted != true)
                .Select(s => new ScopeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    CreateUserID = s.CreateUserID,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    IsDeleted = s.IsDeleted,
                    FinalEnt = s.FinalEnt,
                    BaCreatedTime = s.BaCreatedTime,
                    BaGuid = s.BaGuid
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    public record GetScopesByDepartmentIdQuery(int PageIndex = 1, int PageSize = 10, long DepartmentId = 0) : IRequest<PagedData<ScopeSimpleDto>>;

    public class GetScopesByDepartmentIdQueryHandler : IRequestHandler<GetScopesByDepartmentIdQuery, PagedData<ScopeSimpleDto>>
    {
        private readonly DarooDbContext _context;

        public GetScopesByDepartmentIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<ScopeSimpleDto>> Handle(GetScopesByDepartmentIdQuery request, CancellationToken cancellationToken)
        {
            int pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            int totalCount = await _context.Scopes
                .Where(s => s.DepartmentId == request.DepartmentId && s.IsDeleted != true)
                .OrderBy(s => s.Name)
                .CountAsync(cancellationToken);

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _context.Scopes
                .Where(s => s.DepartmentId == request.DepartmentId && s.IsDeleted != true)
                .OrderBy(s => s.Name)
                .Select(s => new ScopeSimpleDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartementId = s.DepartmentId
                })
                .ToListAsync(cancellationToken);

            return new PagedData<ScopeSimpleDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                IndexFrom = (pageIndex - 1) * pageSize,
                Items = items,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }
    }

    public record GetActiveScopesQuery : IRequest<List<ScopeDto>>;

    public class GetActiveScopesQueryHandler : IRequestHandler<GetActiveScopesQuery, List<ScopeDto>>
    {
        private readonly DarooDbContext _context;

        public GetActiveScopesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScopeDto>> Handle(GetActiveScopesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Scopes
                .Include(s => s.Department)
                .Where(s => s.IsDeleted != true)
                .OrderBy(s => s.Department.Name)
                .ThenBy(s => s.Name)
                .Select(s => new ScopeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    CreateUserID = s.CreateUserID,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    IsDeleted = s.IsDeleted,
                    FinalEnt = s.FinalEnt,
                    BaCreatedTime = s.BaCreatedTime,
                    BaGuid = s.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record SearchScopesQuery(string SearchTerm, long? DepartmentId = null) : IRequest<List<ScopeDto>>;

    public class SearchScopesQueryHandler : IRequestHandler<SearchScopesQuery, List<ScopeDto>>
    {
        private readonly DarooDbContext _context;

        public SearchScopesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScopeDto>> Handle(SearchScopesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Scopes
                .Include(s => s.Department)
                .Where(s => s.IsDeleted != true);

            // اعمال فیلتر جستجو
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(s => s.Name.Contains(request.SearchTerm));
            }

            // اعمال فیلتر اداره کل
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == request.DepartmentId.Value);
            }

            return await query
                .OrderBy(s => s.Department.Name)
                .ThenBy(s => s.Name)
                .Select(s => new ScopeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    CreateUserID = s.CreateUserID,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    IsDeleted = s.IsDeleted,
                    FinalEnt = s.FinalEnt,
                    BaCreatedTime = s.BaCreatedTime,
                    BaGuid = s.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

}