using Application.Common;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    // ===== DTOs =====
    public class MainTitleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? ScopesId { get; set; }
        public string ScopeName { get; set; } = string.Empty;
        public long? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? DisplayOrder { get; set; }
        public long? BpmType { get; set; }
        public long? ProduceType { get; set; }
        public List<MainTitleServiceFeatureDto> MainTitleServiceFeatureDtos { get; set; }
        public string? CreateUserID { get; set; }
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

    public class MainTitleSimpleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string? DisplayOrder { get; set; }
    }

    public class MainTitleSummaryDto
    {
        public long ScopeId { get; set; }
        public string ScopeName { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
    }

    // ===== GET ALL MAIN TITLES QUERY =====
    public record GetAllMainTitlesQuery : IRequest<List<MainTitleDto>>;

    public class GetAllMainTitlesQueryHandler : IRequestHandler<GetAllMainTitlesQuery, List<MainTitleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllMainTitlesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(GetAllMainTitlesQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.IsDeleted != true)
                .OrderBy(mt => mt.Scope.Department.Name)
                .ThenBy(mt => mt.Scope.Name)
                .ThenBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    ScopesId = mt.ScopesId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    BpmType = mt.BpmType,
                    CreateUserID = mt.CreateUserID,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    IsDeleted = mt.IsDeleted,
                    ProduceType = mt.ProduceType,
                    FinalEnt = mt.FinalEnt,
                    BaCreatedTime = mt.BaCreatedTime,
                    BaGuid = mt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }


    public record GetAllMainTitlesPaginationQuery(int PageIndex = 1, int PageSize = 10, long? scopeId = 0, long? bpmType = 0) : IRequest<PagedData<MainTitleDto>>;

    public class GetAllMainTitlesQueryPaginationHandler : IRequestHandler<GetAllMainTitlesPaginationQuery, PagedData<MainTitleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllMainTitlesQueryPaginationHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<MainTitleDto>> Handle(GetAllMainTitlesPaginationQuery request, CancellationToken cancellationToken)
        {
            int pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.MainTitles
            .Include(mt => mt.Scope)
                .ThenInclude(s => s.Department)
            .Where(mt => mt.IsDeleted != true);

            if (request.scopeId != 0)
            {
                query = query.Where(mt => mt.ScopesId == request.scopeId);
            }

            
            if (request.bpmType != 0)
            {
                query = query.Where(mt => mt.BpmType == request.bpmType);
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.IsDeleted != true)
                .OrderBy(mt => mt.Scope.Department.Name)
                .ThenBy(mt => mt.Scope.Name)
                .ThenBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    ScopesId = mt.ScopesId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    BpmType = mt.BpmType,
                    ProduceType = mt.ProduceType,   
                    CreateUserID = mt.CreateUserID,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    IsDeleted = mt.IsDeleted,
                    FinalEnt = mt.FinalEnt,
                    BaCreatedTime = mt.BaCreatedTime,
                    BaGuid = mt.BaGuid
                })
                .ToListAsync(cancellationToken);

            return new PagedData<MainTitleDto>
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



    public record GetMainTitleByIdQuery(long Id) : IRequest<MainTitleDto?>;

    public class GetMainTitleByIdQueryHandler : IRequestHandler<GetMainTitleByIdQuery, MainTitleDto?>
    {
        private readonly ApplicationDbContext _context;

        public GetMainTitleByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MainTitleDto?> Handle(GetMainTitleByIdQuery request, CancellationToken cancellationToken)
        {

            var mainTitleServiceFeatures = await _context.MainTitleServiceFeature
                .Where(q => q.MainTitle == request.Id).ToListAsync();
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.Id == request.Id && mt.IsDeleted != true)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    ScopesId = mt.ScopesId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    BpmType = mt.BpmType,
                    ProduceType = mt.ProduceType,   
                    MainTitleServiceFeatureDtos =  mainTitleServiceFeatures.Select(s => new MainTitleServiceFeatureDto 
                    {
                        Id = s.Id,
                        MainTitleId = s.MainTitle,
                        ServiceFeature = s.ServiceFeature,
                        IsActive = s.IsActive,
                    }).ToList(),
                    CreateUserID = mt.CreateUserID,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    IsDeleted = mt.IsDeleted,
                    FinalEnt = mt.FinalEnt,
                    BaCreatedTime = mt.BaCreatedTime,
                    BaGuid = mt.BaGuid
                })
                .FirstOrDefaultAsync(cancellationToken);

            

        }
    }

    public record GetMainTitlesByScopeQuery(long ScopeId) : IRequest<List<MainTitleSimpleDto>>;

    public class GetMainTitlesByScopeQueryHandler : IRequestHandler<GetMainTitlesByScopeQuery, List<MainTitleSimpleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetMainTitlesByScopeQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleSimpleDto>> Handle(GetMainTitlesByScopeQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Where(mt => mt.ScopesId == request.ScopeId && mt.IsDeleted != true)
                .OrderBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleSimpleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    DisplayOrder = mt.DisplayOrder
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record GetMainTitlesByDepartmentQuery(long DepartmentId) : IRequest<List<MainTitleDto>>;

    public class GetMainTitlesByDepartmentQueryHandler : IRequestHandler<GetMainTitlesByDepartmentQuery, List<MainTitleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetMainTitlesByDepartmentQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(GetMainTitlesByDepartmentQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.Scope.DepartmentId == request.DepartmentId && mt.IsDeleted != true)
                .OrderBy(mt => mt.Scope.Name)
                .ThenBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    ScopesId = mt.ScopesId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    BpmType = mt.BpmType,
                    ProduceType = mt.ProduceType,   
                    CreateUserID = mt.CreateUserID,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    IsDeleted = mt.IsDeleted,
                    FinalEnt = mt.FinalEnt,
                    BaCreatedTime = mt.BaCreatedTime,
                    BaGuid = mt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record SearchMainTitlesQuery(
        string? SearchTerm = null,
        long? ScopeId = null,
        long? DepartmentId = null
    ) : IRequest<List<MainTitleDto>>;

    public class SearchMainTitlesQueryHandler : IRequestHandler<SearchMainTitlesQuery, List<MainTitleDto>>
    {
        private readonly ApplicationDbContext _context;

        public SearchMainTitlesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(SearchMainTitlesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.IsDeleted != true);

            // فیلتر جستجوی متنی
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                query = query.Where(mt =>
                    mt.Name.Contains(searchTerm) ||
                    (mt.Description != null && mt.Description.Contains(searchTerm)) ||
                    mt.Scope.Name.Contains(searchTerm) ||
                    mt.Scope.Department.Name.Contains(searchTerm)
                );
            }

            // فیلتر حوزه
            if (request.ScopeId.HasValue)
            {
                query = query.Where(mt => mt.ScopesId == request.ScopeId.Value);
            }

            // فیلتر اداره کل
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(mt => mt.Scope.DepartmentId == request.DepartmentId.Value);
            }



            return await query
                .OrderBy(mt => mt.Scope.Department.Name)
                .ThenBy(mt => mt.Scope.Name)
                .ThenBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    ScopesId = mt.ScopesId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    BpmType = mt.BpmType,
                    ProduceType = mt.ProduceType,   
                    CreateUserID = mt.CreateUserID,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    IsDeleted = mt.IsDeleted,
                    FinalEnt = mt.FinalEnt,
                    BaCreatedTime = mt.BaCreatedTime,
                    BaGuid = mt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record GetMainTitlesSummaryQuery : IRequest<List<MainTitleSummaryDto>>;

    public class GetMainTitlesSummaryQueryHandler : IRequestHandler<GetMainTitlesSummaryQuery, List<MainTitleSummaryDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetMainTitlesSummaryQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleSummaryDto>> Handle(GetMainTitlesSummaryQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                .Where(mt => mt.IsDeleted != true)
                .GroupBy(mt => new { mt.ScopesId, mt.Scope.Name })
                .Select(g => new MainTitleSummaryDto
                {
                    ScopeId = g.Key.ScopesId ?? 0,
                    ScopeName = g.Key.Name ?? "",
                    TotalCount = g.Count()
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToListAsync(cancellationToken);
        }
    }

    public record GetTopExpensiveMainTitlesQuery(int Top = 10) : IRequest<List<MainTitleDto>>;

    public class GetTopExpensiveMainTitlesQueryHandler : IRequestHandler<GetTopExpensiveMainTitlesQuery, List<MainTitleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetTopExpensiveMainTitlesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(GetTopExpensiveMainTitlesQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.IsDeleted != true)
                .Take(Math.Max(1, Math.Min(request.Top, 100)))
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    ScopesId = mt.ScopesId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    BpmType = mt.BpmType,
                    ProduceType = mt.ProduceType,
                    CreateUserID = mt.CreateUserID,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    IsDeleted = mt.IsDeleted,
                    FinalEnt = mt.FinalEnt,
                    BaCreatedTime = mt.BaCreatedTime,
                    BaGuid = mt.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

}