using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    // ===== DTOs =====
    public class ServiceFeatureDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CreateUserId { get; set; } = string.Empty;
    }

    public class ServiceFeatureSimpleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; }
    }

    public class MainTitleServiceFeatureDto
    {
        public long Id { get; set; }
        public long MainTitleId { get; set; }
        public long ServiceFeature { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public string? Notes { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    // ===== GET ALL SERVICEFEATURES QUERY =====
    public record GetAllServiceFeaturesQuery : IRequest<List<ServiceFeatureDto>>;

    public class GetAllServiceFeaturesQueryHandler : IRequestHandler<GetAllServiceFeaturesQuery, List<ServiceFeatureDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllServiceFeaturesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceFeatureDto>> Handle(GetAllServiceFeaturesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ServiceFeature
                .Where(sf => !sf.IsDeleted)
                .OrderBy(sf => sf.Id)
                .ThenBy(sf => sf.Name)
                .Select(sf => new ServiceFeatureDto
                {
                    Id = sf.Id,
                    Name = sf.Name,
                    IsActive = sf.IsActive,
                    IsDelete = sf.IsDeleted,
                    CreateDate = (DateTime)sf.CreateDate,
                    ModifyDate = (DateTime)sf.ModifyDate,
                    CreateUserId = sf.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET ACTIVE SERVICEFEATURES QUERY =====
    public record GetActiveServiceFeaturesQuery : IRequest<List<ServiceFeatureSimpleDto>>;

    public class GetActiveServiceFeaturesQueryHandler : IRequestHandler<GetActiveServiceFeaturesQuery, List<ServiceFeatureSimpleDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetActiveServiceFeaturesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceFeatureSimpleDto>> Handle(GetActiveServiceFeaturesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ServiceFeature
                .Where(sf => !sf.IsDeleted && sf.IsActive)
                .OrderBy(sf => sf.Id)
                .ThenBy(sf => sf.Name)
                .Select(sf => new ServiceFeatureSimpleDto
                {
                    Id = sf.Id,
                    Name = sf.Name,
                    IsActive = sf.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET SERVICEFEATURE BY ID QUERY =====
    public record GetServiceFeatureByIdQuery(int Id) : IRequest<ServiceFeatureDto?>;

    public class GetServiceFeatureByIdQueryHandler : IRequestHandler<GetServiceFeatureByIdQuery, ServiceFeatureDto?>
    {
        private readonly ApplicationDbContext _context;

        public GetServiceFeatureByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceFeatureDto?> Handle(GetServiceFeatureByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.ServiceFeature
                .Where(sf => sf.Id == request.Id && !sf.IsDeleted)
                .Select(sf => new ServiceFeatureDto
                {
                    Id = sf.Id,
                    Name = sf.Name,
                    IsActive = sf.IsActive,
                    IsDelete = sf.IsDeleted,
                    CreateDate = (DateTime)sf.CreateDate,
                    ModifyDate = (DateTime)sf.ModifyDate,
                    CreateUserId = sf.CreateUserId
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GET SERVICEFEATURES BY MAINTITLE QUERY =====
    public record GetServiceFeaturesByMainTitleQuery(int MainTitleId, bool ActiveOnly = false) : IRequest<List<MainTitleServiceFeatureDto>>;

    public class GetServiceFeaturesByMainTitleQueryHandler : IRequestHandler<GetServiceFeaturesByMainTitleQuery, List<MainTitleServiceFeatureDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetServiceFeaturesByMainTitleQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleServiceFeatureDto>> Handle(GetServiceFeaturesByMainTitleQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MainTitleServiceFeature
                .Include(mtsf => mtsf.MainTitle)
                .Include(mtsf => mtsf.ServiceFeature)
                .Where(mtsf => mtsf.MainTitle == request.MainTitleId && !mtsf.IsDeleted);

            if (request.ActiveOnly)
            {
                query = query.Where(mtsf => mtsf.IsActive);
            }

            return await query
                .OrderBy(mtsf => mtsf.Id)
                .ThenBy(mtsf => mtsf.ServiceFeature)
                .Select(mtsf => new MainTitleServiceFeatureDto
                {
                    Id = mtsf.Id,
                    MainTitleId = mtsf.MainTitle,
                    ServiceFeature = mtsf.ServiceFeature,
                    IsActive = mtsf.IsActive,
                    CreateDate = (DateTime)mtsf.CreateDate
                })
                .ToListAsync(cancellationToken);
        }
    }




}