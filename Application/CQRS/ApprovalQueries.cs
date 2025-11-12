using Application.Common;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    // ===== DTOs =====
    public class ApprovalDto
    {
        public long Id { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public DateTime? TariffStartDate { get; set; }
        public DateTime? TariffEndDate { get; set; }
        public long? MainTitleId { get; set; }
        public string MainTitleName { get; set; } = string.Empty;
        public decimal? Amount { get; set; } // جدید
        public bool? IsActive { get; set; }

        // از پراپرتی‌های خود Approval استفاده می‌کنیم
        public bool IsCurrentlyActive { get; set; }
        public bool IsLatestTariff { get; set; }
        public bool IsTariffExpired { get; set; }

        public int FinalEnt { get; set; }
        public long BaCreatedTime { get; set; }
        public Guid BaGuid { get; set; }

        public DateTime? BaCreatedDateTime => BaCreatedTime > 0
            ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(BaCreatedTime)
            : null;

        public int? DaysUntilEnd => IsLatestTariff ? null : (int?)(TariffEndDate!.Value - DateTime.Now).TotalDays;
    }

    public class ApprovalSimpleDto
    {
        public long Id { get; set; }
        public DateTime? TariffStartDate { get; set; }
        public DateTime? TariffEndDate { get; set; }
        public decimal? Amount { get; set; }
        public bool? IsActive { get; set; }
        public bool IsCurrentlyActive { get; set; }
        public bool IsLatestTariff { get; set; }
    }

    // ===== GET ALL APPROVALS =====
    public record GetAllApprovalsQuery : IRequest<List<ApprovalDto>>;

    public class GetAllApprovalsQueryHandler : IRequestHandler<GetAllApprovalsQuery, List<ApprovalDto>>
    {
        private readonly ApplicationDbContext _context;
        public GetAllApprovalsQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<List<ApprovalDto>> Handle(GetAllApprovalsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Approvals
                .AsNoTracking()
                .Include(a => a.MainTitle)
                .OrderByDescending(a => a.BaCreatedTime)
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    ExecutionDate = a.ExecutionDate,
                    TariffStartDate = a.TariffStartDate,
                    TariffEndDate = a.TariffEndDate,
                    MainTitleId = a.MainTitleId,
                    MainTitleName = a.MainTitle != null ? a.MainTitle.Name : "",
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    IsCurrentlyActive = a.IsCurrentlyActive,
                    IsLatestTariff = a.IsLatestTariff,
                    IsTariffExpired = a.TariffEndDate.HasValue && a.TariffEndDate.Value < DateTime.Now,
                    FinalEnt = a.FinalEnt,
                    BaCreatedTime = a.BaCreatedTime,
                    BaGuid = a.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== PAGINATION =====
    public record GetAllApprovalsPaginationQuery(
        int PageIndex = 1,
        int PageSize = 10,
        long? MainTitleId = null,
        bool? IsActive = null,
        bool? OnlyCurrent = null,   // فقط تعرفه‌های در حال اجرا
        bool? OnlyLatest = null     // فقط آخرین تعرفه هر عنوان
    ) : IRequest<PagedData<ApprovalDto>>;

    public class GetAllApprovalsPaginationQueryHandler : IRequestHandler<GetAllApprovalsPaginationQuery, PagedData<ApprovalDto>>
    {
        private readonly ApplicationDbContext _context;
        public GetAllApprovalsPaginationQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<PagedData<ApprovalDto>> Handle(GetAllApprovalsPaginationQuery request, CancellationToken cancellationToken)
        {
            // اعتبارسنجی ورودی
            var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize > 100 ? 100 : request.PageSize;

            var query = _context.Approvals
                .AsNoTracking()
                .Include(a => a.MainTitle)
                .AsQueryable();

            // فیلترها
            if (request.MainTitleId.HasValue)
                query = query.Where(a => a.MainTitleId == request.MainTitleId.Value);

            if (request.IsActive.HasValue)
                query = query.Where(a => a.IsActive == request.IsActive.Value);

            if (request.OnlyCurrent == true)
                query = query.Where(a => a.IsCurrentlyActive);

            if (request.OnlyLatest == true)
                query = query.Where(a => a.IsLatestTariff);

            // شمارش کل
            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // گرفتن آیتم‌ها
            var items = await query
                .OrderByDescending(a => a.BaCreatedTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    ExecutionDate = a.ExecutionDate,
                    TariffStartDate = a.TariffStartDate,
                    TariffEndDate = a.TariffEndDate,
                    MainTitleId = a.MainTitleId,
                    MainTitleName = a.MainTitle != null ? a.MainTitle.Name : "",
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    IsCurrentlyActive = a.IsCurrentlyActive,
                    IsLatestTariff = a.IsLatestTariff,
                    IsTariffExpired = a.TariffEndDate.HasValue && a.TariffEndDate.Value < DateTime.Now,
                    FinalEnt = a.FinalEnt,
                    BaCreatedTime = a.BaCreatedTime,
                    BaGuid = a.BaGuid
                })
                .ToListAsync(cancellationToken);

            // ساختن نتیجه با همه پراپرتی‌های مورد نیاز
            return new PagedData<ApprovalDto>
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

    // ===== GET CURRENTLY ACTIVE APPROVALS (در حال اجرا در همین لحظه) =====
    public record GetActiveApprovalsQuery : IRequest<List<ApprovalDto>>;

    public class GetActiveApprovalsQueryHandler : IRequestHandler<GetActiveApprovalsQuery, List<ApprovalDto>>
    {
        private readonly ApplicationDbContext _context;
        public GetActiveApprovalsQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<List<ApprovalDto>> Handle(GetActiveApprovalsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Approvals
                .AsNoTracking()
                .Include(a => a.MainTitle)
                .Where(a => a.IsCurrentlyActive) // دقیقاً همون چیزی که تو Approval تعریف کردی
                .OrderBy(a => a.MainTitle!.Name)
                .ThenByDescending(a => a.TariffStartDate)
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    ExecutionDate = a.ExecutionDate,
                    TariffStartDate = a.TariffStartDate,
                    TariffEndDate = a.TariffEndDate,
                    MainTitleId = a.MainTitleId,
                    MainTitleName = a.MainTitle != null ? a.MainTitle.Name : "",
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    IsCurrentlyActive = true,
                    IsLatestTariff = a.IsLatestTariff,
                    FinalEnt = a.FinalEnt,
                    BaCreatedTime = a.BaCreatedTime,
                    BaGuid = a.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET BY ID =====
    public record GetApprovalByIdQuery(long Id) : IRequest<ApprovalDto?>;

    public class GetApprovalByIdQueryHandler : IRequestHandler<GetApprovalByIdQuery, ApprovalDto?>
    {
        private readonly ApplicationDbContext _context;
        public GetApprovalByIdQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<ApprovalDto?> Handle(GetApprovalByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Approvals
                .AsNoTracking()
                .Include(a => a.MainTitle)
                .Where(a => a.Id == request.Id)
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    ExecutionDate = a.ExecutionDate,
                    TariffStartDate = a.TariffStartDate,
                    TariffEndDate = a.TariffEndDate,
                    MainTitleId = a.MainTitleId,
                    MainTitleName = a.MainTitle != null ? a.MainTitle.Name : "",
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    IsCurrentlyActive = a.IsCurrentlyActive,
                    IsLatestTariff = a.IsLatestTariff,
                    IsTariffExpired = a.TariffEndDate.HasValue && a.TariffEndDate.Value < DateTime.Now,
                    FinalEnt = a.FinalEnt,
                    BaCreatedTime = a.BaCreatedTime,
                    BaGuid = a.BaGuid
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GET BY MAINTITLE =====
    public record GetApprovalsByMainTitleQuery(long MainTitleId, bool OnlyCurrent = false, bool OnlyLatest = false)
        : IRequest<List<ApprovalDto>>;

    public class GetApprovalsByMainTitleQueryHandler : IRequestHandler<GetApprovalsByMainTitleQuery, List<ApprovalDto>>
    {
        private readonly ApplicationDbContext _context;
        public GetApprovalsByMainTitleQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<List<ApprovalDto>> Handle(GetApprovalsByMainTitleQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Approvals
                .AsNoTracking()
                .Include(a => a.MainTitle)
                .Where(a => a.MainTitleId == request.MainTitleId);

            return await query
                .OrderByDescending(a => a.TariffStartDate)
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    ExecutionDate = a.ExecutionDate,
                    TariffStartDate = a.TariffStartDate,
                    TariffEndDate = a.TariffEndDate,
                    MainTitleId = a.MainTitleId,
                    MainTitleName = a.MainTitle != null ? a.MainTitle.Name : "",
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    IsCurrentlyActive = a.IsCurrentlyActive,
                    IsLatestTariff = a.IsLatestTariff,
                    IsTariffExpired = a.TariffEndDate.HasValue && a.TariffEndDate.Value < DateTime.Now,
                    FinalEnt = a.FinalEnt,
                    BaCreatedTime = a.BaCreatedTime,
                    BaGuid = a.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET CURRENT ACTIVE APPROVAL FOR MAINTITLE (مهم‌ترین کوئری!) =====
    public record GetCurrentApprovalForMainTitleQuery(long MainTitleId) : IRequest<ApprovalDto?>;

    public class GetCurrentApprovalForMainTitleQueryHandler : IRequestHandler<GetCurrentApprovalForMainTitleQuery, ApprovalDto?>
    {
        private readonly ApplicationDbContext _context;
        public GetCurrentApprovalForMainTitleQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<ApprovalDto?> Handle(GetCurrentApprovalForMainTitleQuery request, CancellationToken cancellationToken)
        {
            return await _context.Approvals
                .AsNoTracking()
                .Include(a => a.MainTitle)
                .Where(a => a.MainTitleId == request.MainTitleId && a.IsCurrentlyActive)
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    ExecutionDate = a.ExecutionDate,
                    TariffStartDate = a.TariffStartDate,
                    TariffEndDate = a.TariffEndDate,
                    MainTitleId = a.MainTitleId,
                    MainTitleName = a.MainTitle != null ? a.MainTitle.Name : "",
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    IsCurrentlyActive = true,
                    IsLatestTariff = a.IsLatestTariff,
                    FinalEnt = a.FinalEnt,
                    BaCreatedTime = a.BaCreatedTime,
                    BaGuid = a.BaGuid
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GET EXPIRING SOON =====
    public record GetExpiringSoonApprovalsQuery(int DaysThreshold = 30) : IRequest<List<ApprovalDto>>;

    public class GetExpiringSoonApprovalsQueryHandler : IRequestHandler<GetExpiringSoonApprovalsQuery, List<ApprovalDto>>
    {
        private readonly ApplicationDbContext _context;
        public GetExpiringSoonApprovalsQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<List<ApprovalDto>> Handle(GetExpiringSoonApprovalsQuery request, CancellationToken cancellationToken)
        {
            var threshold = DateTime.Now.AddDays(request.DaysThreshold);

            return await _context.Approvals
                .AsNoTracking()
                .Include(a => a.MainTitle)
                .Where(a => a.IsCurrentlyActive && a.TariffEndDate <= threshold)
                .OrderBy(a => a.TariffEndDate)
                .Select(a => new ApprovalDto
                {
                    Id = a.Id,
                    ExecutionDate = a.ExecutionDate,
                    TariffStartDate = a.TariffStartDate,
                    TariffEndDate = a.TariffEndDate,
                    MainTitleId = a.MainTitleId,
                    MainTitleName = a.MainTitle != null ? a.MainTitle.Name : "",
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    IsCurrentlyActive = true,
                    IsLatestTariff = a.IsLatestTariff,
                    
                    FinalEnt = a.FinalEnt,
                    BaCreatedTime = a.BaCreatedTime,
                    BaGuid = a.BaGuid
                })
                .ToListAsync(cancellationToken);
        }
    }
}