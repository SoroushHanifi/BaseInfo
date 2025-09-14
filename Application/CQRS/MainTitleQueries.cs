using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    // ===== DTOs =====
    public class MainTitleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public int ScopeId { get; set; }
        public string ScopeName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string CreateUserId { get; set; } = string.Empty;
    }

    public class MainTitleSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class MainTitleSummaryDto
    {
        public int ScopeId { get; set; }
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
        private readonly DarooDbContext _context;

        public GetAllMainTitlesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(GetAllMainTitlesQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => !mt.IsDelete)
                .OrderBy(mt => mt.Scope.Department.Name)
                .ThenBy(mt => mt.Scope.Name)
                .ThenBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    Amount = mt.Amount,
                    ScopeId = mt.ScopeId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    IsDelete = mt.IsDelete,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    CreateUserId = mt.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET MAIN TITLE BY ID QUERY =====
    public record GetMainTitleByIdQuery(int Id) : IRequest<MainTitleDto?>;

    public class GetMainTitleByIdQueryHandler : IRequestHandler<GetMainTitleByIdQuery, MainTitleDto?>
    {
        private readonly DarooDbContext _context;

        public GetMainTitleByIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<MainTitleDto?> Handle(GetMainTitleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.Id == request.Id && !mt.IsDelete)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    Amount = mt.Amount,
                    ScopeId = mt.ScopeId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    IsDelete = mt.IsDelete,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    CreateUserId = mt.CreateUserId
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GET MAIN TITLES BY SCOPE QUERY =====
    public record GetMainTitlesByScopeQuery(int ScopeId) : IRequest<List<MainTitleSimpleDto>>;

    public class GetMainTitlesByScopeQueryHandler : IRequestHandler<GetMainTitlesByScopeQuery, List<MainTitleSimpleDto>>
    {
        private readonly DarooDbContext _context;

        public GetMainTitlesByScopeQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleSimpleDto>> Handle(GetMainTitlesByScopeQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Where(mt => mt.ScopeId == request.ScopeId && !mt.IsDelete)
                .OrderBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleSimpleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Amount = mt.Amount,
                    DisplayOrder = mt.DisplayOrder
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET MAIN TITLES BY DEPARTMENT QUERY =====
    public record GetMainTitlesByDepartmentQuery(int DepartmentId) : IRequest<List<MainTitleDto>>;

    public class GetMainTitlesByDepartmentQueryHandler : IRequestHandler<GetMainTitlesByDepartmentQuery, List<MainTitleDto>>
    {
        private readonly DarooDbContext _context;

        public GetMainTitlesByDepartmentQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(GetMainTitlesByDepartmentQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => mt.Scope.DepartmentId == request.DepartmentId && !mt.IsDelete)
                .OrderBy(mt => mt.Scope.Name)
                .ThenBy(mt => mt.DisplayOrder)
                .ThenBy(mt => mt.Name)
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    Amount = mt.Amount,
                    ScopeId = mt.ScopeId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    IsDelete = mt.IsDelete,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    CreateUserId = mt.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== SEARCH MAIN TITLES QUERY =====
    public record SearchMainTitlesQuery(
        string? SearchTerm = null,
        int? ScopeId = null,
        int? DepartmentId = null,
        decimal? MinAmount = null,
        decimal? MaxAmount = null
    ) : IRequest<List<MainTitleDto>>;

    public class SearchMainTitlesQueryHandler : IRequestHandler<SearchMainTitlesQuery, List<MainTitleDto>>
    {
        private readonly DarooDbContext _context;

        public SearchMainTitlesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(SearchMainTitlesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => !mt.IsDelete);

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
                query = query.Where(mt => mt.ScopeId == request.ScopeId.Value);
            }

            // فیلتر اداره کل
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(mt => mt.Scope.DepartmentId == request.DepartmentId.Value);
            }

            // فیلتر مبلغ حداقل
            if (request.MinAmount.HasValue)
            {
                query = query.Where(mt => mt.Amount >= request.MinAmount.Value);
            }

            // فیلتر مبلغ حداکثر
            if (request.MaxAmount.HasValue)
            {
                query = query.Where(mt => mt.Amount <= request.MaxAmount.Value);
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
                    Amount = mt.Amount,
                    ScopeId = mt.ScopeId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    IsDelete = mt.IsDelete,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    CreateUserId = mt.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET MAIN TITLES SUMMARY BY SCOPE QUERY =====
    public record GetMainTitlesSummaryQuery : IRequest<List<MainTitleSummaryDto>>;

    public class GetMainTitlesSummaryQueryHandler : IRequestHandler<GetMainTitlesSummaryQuery, List<MainTitleSummaryDto>>
    {
        private readonly DarooDbContext _context;

        public GetMainTitlesSummaryQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleSummaryDto>> Handle(GetMainTitlesSummaryQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                .Where(mt => !mt.IsDelete)
                .GroupBy(mt => new { mt.ScopeId, mt.Scope.Name })
                .Select(g => new MainTitleSummaryDto
                {
                    ScopeId = g.Key.ScopeId,
                    ScopeName = g.Key.Name,
                    TotalCount = g.Count(),
                    TotalAmount = g.Sum(mt => mt.Amount),
                    AverageAmount = g.Average(mt => mt.Amount),
                    MinAmount = g.Min(mt => mt.Amount),
                    MaxAmount = g.Max(mt => mt.Amount)
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET TOP EXPENSIVE MAIN TITLES QUERY =====
    public record GetTopExpensiveMainTitlesQuery(int Top = 10) : IRequest<List<MainTitleDto>>;

    public class GetTopExpensiveMainTitlesQueryHandler : IRequestHandler<GetTopExpensiveMainTitlesQuery, List<MainTitleDto>>
    {
        private readonly DarooDbContext _context;

        public GetTopExpensiveMainTitlesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleDto>> Handle(GetTopExpensiveMainTitlesQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitles
                .Include(mt => mt.Scope)
                    .ThenInclude(s => s.Department)
                .Where(mt => !mt.IsDelete)
                .OrderByDescending(mt => mt.Amount)
                .Take(Math.Max(1, Math.Min(request.Top, 100))) // محدود به 1-100
                .Select(mt => new MainTitleDto
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Description = mt.Description,
                    Amount = mt.Amount,
                    ScopeId = mt.ScopeId,
                    ScopeName = mt.Scope.Name,
                    DepartmentId = mt.Scope.DepartmentId,
                    DepartmentName = mt.Scope.Department.Name,
                    DisplayOrder = mt.DisplayOrder,
                    IsDelete = mt.IsDelete,
                    CreateDate = mt.CreateDate,
                    ModifyDate = mt.ModifyDate,
                    CreateUserId = mt.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }
}