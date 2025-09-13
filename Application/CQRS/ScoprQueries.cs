using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    // ===== DTOs =====
    public class ScopeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string CreateUserId { get; set; } = string.Empty;
    }

    public class ScopeSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool IsActive { get; set; }
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
                .Where(s => !s.IsDelete)
                .OrderBy(s => s.Department.Name)
                .ThenBy(s => s.Name)
                .Select(s => new ScopeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    IsDelete = s.IsDelete,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    CreateUserId = s.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET SCOPE BY ID QUERY =====
    public record GetScopeByIdQuery(int Id) : IRequest<ScopeDto?>;

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
                .Where(s => s.Id == request.Id && !s.IsDelete)
                .Select(s => new ScopeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    IsDelete = s.IsDelete,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    CreateUserId = s.CreateUserId
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GET SCOPES BY DEPARTMENT ID QUERY =====
    public record GetScopesByDepartmentIdQuery(int DepartmentId) : IRequest<List<ScopeSimpleDto>>;

    public class GetScopesByDepartmentIdQueryHandler : IRequestHandler<GetScopesByDepartmentIdQuery, List<ScopeSimpleDto>>
    {
        private readonly DarooDbContext _context;

        public GetScopesByDepartmentIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScopeSimpleDto>> Handle(GetScopesByDepartmentIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Scopes
                .Where(s => s.DepartmentId == request.DepartmentId && !s.IsDelete)
                .OrderBy(s => s.Name)
                .Select(s => new ScopeSimpleDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET ACTIVE SCOPES QUERY =====
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
                .Where(s => !s.IsDelete)
                .OrderBy(s => s.Department.Name)
                .ThenBy(s => s.Name)
                .Select(s => new ScopeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DepartmentId = s.DepartmentId,
                    DepartmentName = s.Department.Name,
                    IsDelete = s.IsDelete,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    CreateUserId = s.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET ACTIVE SCOPES BY DEPARTMENT QUERY =====
    public record GetActiveScopesByDepartmentQuery(int DepartmentId) : IRequest<List<ScopeSimpleDto>>;

    public class GetActiveScopesByDepartmentQueryHandler : IRequestHandler<GetActiveScopesByDepartmentQuery, List<ScopeSimpleDto>>
    {
        private readonly DarooDbContext _context;

        public GetActiveScopesByDepartmentQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScopeSimpleDto>> Handle(GetActiveScopesByDepartmentQuery request, CancellationToken cancellationToken)
        {
            return await _context.Scopes
                .Where(s => s.DepartmentId == request.DepartmentId && !s.IsDelete)
                .OrderBy(s => s.Name)
                .Select(s => new ScopeSimpleDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== SEARCH SCOPES QUERY =====
    public record SearchScopesQuery(string SearchTerm, int? DepartmentId = null) : IRequest<List<ScopeDto>>;

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
                .Where(s => !s.IsDelete);

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
                    IsDelete = s.IsDelete,
                    CreateDate = s.CreateDate,
                    ModifyDate = s.ModifyDate,
                    CreateUserId = s.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }
}