using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS
{
    // ===== DTOs =====
    public class ServiceFeatureDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string CreateUserId { get; set; } = string.Empty;
    }

    public class ServiceFeatureSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; }
    }

    public class MainTitleServiceFeatureDto
    {
        public int Id { get; set; }
        public int MainTitleId { get; set; }
        public string MainTitleName { get; set; } = string.Empty;
        public int ServiceFeatureId { get; set; }
        public string ServiceFeatureName { get; set; } = string.Empty;
        public string? ServiceFeatureIcon { get; set; }
        public string? ServiceFeatureColor { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public string? Notes { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public DateTime CreateDate { get; set; }
    }

    // ===== GET ALL SERVICEFEATURES QUERY =====
    public record GetAllServiceFeaturesQuery : IRequest<List<ServiceFeatureDto>>;

    public class GetAllServiceFeaturesQueryHandler : IRequestHandler<GetAllServiceFeaturesQuery, List<ServiceFeatureDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllServiceFeaturesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceFeatureDto>> Handle(GetAllServiceFeaturesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ServiceFeatures
                .Where(sf => !sf.IsDelete)
                .OrderBy(sf => sf.DisplayOrder)
                .ThenBy(sf => sf.Name)
                .Select(sf => new ServiceFeatureDto
                {
                    Id = sf.Id,
                    Name = sf.Name,
                    Description = sf.Description,
                    Code = sf.Code,
                    Icon = sf.Icon,
                    Color = sf.Color,
                    DisplayOrder = sf.DisplayOrder,
                    IsActive = sf.IsActive,
                    IsDelete = sf.IsDelete,
                    CreateDate = sf.CreateDate,
                    ModifyDate = sf.ModifyDate,
                    CreateUserId = sf.CreateUserId
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET ACTIVE SERVICEFEATURES QUERY =====
    public record GetActiveServiceFeaturesQuery : IRequest<List<ServiceFeatureSimpleDto>>;

    public class GetActiveServiceFeaturesQueryHandler : IRequestHandler<GetActiveServiceFeaturesQuery, List<ServiceFeatureSimpleDto>>
    {
        private readonly DarooDbContext _context;

        public GetActiveServiceFeaturesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceFeatureSimpleDto>> Handle(GetActiveServiceFeaturesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ServiceFeatures
                .Where(sf => !sf.IsDelete && sf.IsActive)
                .OrderBy(sf => sf.DisplayOrder)
                .ThenBy(sf => sf.Name)
                .Select(sf => new ServiceFeatureSimpleDto
                {
                    Id = sf.Id,
                    Name = sf.Name,
                    Icon = sf.Icon,
                    Color = sf.Color,
                    IsActive = sf.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET SERVICEFEATURE BY ID QUERY =====
    public record GetServiceFeatureByIdQuery(int Id) : IRequest<ServiceFeatureDto?>;

    public class GetServiceFeatureByIdQueryHandler : IRequestHandler<GetServiceFeatureByIdQuery, ServiceFeatureDto?>
    {
        private readonly DarooDbContext _context;

        public GetServiceFeatureByIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceFeatureDto?> Handle(GetServiceFeatureByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.ServiceFeatures
                .Where(sf => sf.Id == request.Id && !sf.IsDelete)
                .Select(sf => new ServiceFeatureDto
                {
                    Id = sf.Id,
                    Name = sf.Name,
                    Description = sf.Description,
                    Code = sf.Code,
                    Icon = sf.Icon,
                    Color = sf.Color,
                    DisplayOrder = sf.DisplayOrder,
                    IsActive = sf.IsActive,
                    IsDelete = sf.IsDelete,
                    CreateDate = sf.CreateDate,
                    ModifyDate = sf.ModifyDate,
                    CreateUserId = sf.CreateUserId
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GET SERVICEFEATURES BY MAINTITLE QUERY =====
    public record GetServiceFeaturesByMainTitleQuery(int MainTitleId, bool ActiveOnly = false) : IRequest<List<MainTitleServiceFeatureDto>>;

    public class GetServiceFeaturesByMainTitleQueryHandler : IRequestHandler<GetServiceFeaturesByMainTitleQuery, List<MainTitleServiceFeatureDto>>
    {
        private readonly DarooDbContext _context;

        public GetServiceFeaturesByMainTitleQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleServiceFeatureDto>> Handle(GetServiceFeaturesByMainTitleQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MainTitleServiceFeatures
                .Include(mtsf => mtsf.MainTitle)
                .Include(mtsf => mtsf.ServiceFeature)
                .Where(mtsf => mtsf.MainTitleId == request.MainTitleId && !mtsf.IsDelete);

            if (request.ActiveOnly)
            {
                query = query.Where(mtsf => mtsf.IsActive);
            }

            return await query
                .OrderBy(mtsf => mtsf.DisplayOrder)
                .ThenBy(mtsf => mtsf.ServiceFeature.Name)
                .Select(mtsf => new MainTitleServiceFeatureDto
                {
                    Id = mtsf.Id,
                    MainTitleId = mtsf.MainTitleId,
                    MainTitleName = mtsf.MainTitle.Name,
                    ServiceFeatureId = mtsf.ServiceFeatureId,
                    ServiceFeatureName = mtsf.ServiceFeature.Name,
                    ServiceFeatureIcon = mtsf.ServiceFeature.Icon,
                    ServiceFeatureColor = mtsf.ServiceFeature.Color,
                    IsActive = mtsf.IsActive,
                    DisplayOrder = mtsf.DisplayOrder,
                    Notes = mtsf.Notes,
                    ActivatedDate = mtsf.ActivatedDate,
                    DeactivatedDate = mtsf.DeactivatedDate,
                    CreateDate = mtsf.CreateDate
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET MAINTITLES BY SERVICEFEATURE QUERY =====
    public record GetMainTitlesByServiceFeatureQuery(int ServiceFeatureId, bool ActiveOnly = false) : IRequest<List<MainTitleServiceFeatureDto>>;

    public class GetMainTitlesByServiceFeatureQueryHandler : IRequestHandler<GetMainTitlesByServiceFeatureQuery, List<MainTitleServiceFeatureDto>>
    {
        private readonly DarooDbContext _context;

        public GetMainTitlesByServiceFeatureQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleServiceFeatureDto>> Handle(GetMainTitlesByServiceFeatureQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MainTitleServiceFeatures
                .Include(mtsf => mtsf.MainTitle)
                .Include(mtsf => mtsf.ServiceFeature)
                .Where(mtsf => mtsf.ServiceFeatureId == request.ServiceFeatureId && !mtsf.IsDelete);

            if (request.ActiveOnly)
            {
                query = query.Where(mtsf => mtsf.IsActive);
            }

            return await query
                .OrderBy(mtsf => mtsf.MainTitle.Name)
                .Select(mtsf => new MainTitleServiceFeatureDto
                {
                    Id = mtsf.Id,
                    MainTitleId = mtsf.MainTitleId,
                    MainTitleName = mtsf.MainTitle.Name,
                    ServiceFeatureId = mtsf.ServiceFeatureId,
                    ServiceFeatureName = mtsf.ServiceFeature.Name,
                    ServiceFeatureIcon = mtsf.ServiceFeature.Icon,
                    ServiceFeatureColor = mtsf.ServiceFeature.Color,
                    IsActive = mtsf.IsActive,
                    DisplayOrder = mtsf.DisplayOrder,
                    Notes = mtsf.Notes,
                    ActivatedDate = mtsf.ActivatedDate,
                    DeactivatedDate = mtsf.DeactivatedDate,
                    CreateDate = mtsf.CreateDate
                })
                .ToListAsync(cancellationToken);
        }
    }

    // ===== GET ALL MAINTITLE-SERVICEFEATURE RELATIONS QUERY =====
    public record GetAllMainTitleServiceFeaturesQuery : IRequest<List<MainTitleServiceFeatureDto>>;

    public class GetAllMainTitleServiceFeaturesQueryHandler : IRequestHandler<GetAllMainTitleServiceFeaturesQuery, List<MainTitleServiceFeatureDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllMainTitleServiceFeaturesQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<MainTitleServiceFeatureDto>> Handle(GetAllMainTitleServiceFeaturesQuery request, CancellationToken cancellationToken)
        {
            return await _context.MainTitleServiceFeatures
                .Include(mtsf => mtsf.MainTitle)
                .Include(mtsf => mtsf.ServiceFeature)
                .Where(mtsf => !mtsf.IsDelete)
                .OrderBy(mtsf => mtsf.MainTitle.Name)
                .ThenBy(mtsf => mtsf.DisplayOrder)
                .ThenBy(mtsf => mtsf.ServiceFeature.Name)
                .Select(mtsf => new MainTitleServiceFeatureDto
                {
                    Id = mtsf.Id,
                    MainTitleId = mtsf.MainTitleId,
                    MainTitleName = mtsf.MainTitle.Name,
                    ServiceFeatureId = mtsf.ServiceFeatureId,
                    ServiceFeatureName = mtsf.ServiceFeature.Name,
                    ServiceFeatureIcon = mtsf.ServiceFeature.Icon,
                    ServiceFeatureColor = mtsf.ServiceFeature.Color,
                    IsActive = mtsf.IsActive,
                    DisplayOrder = mtsf.DisplayOrder,
                    Notes = mtsf.Notes,
                    ActivatedDate = mtsf.ActivatedDate,
                    DeactivatedDate = mtsf.DeactivatedDate,
                    CreateDate = mtsf.CreateDate
                })
                .ToListAsync(cancellationToken);
        }
    }
}