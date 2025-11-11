using Application.OptionPatternModel;
using Application.Refits;
using Domain;
using Domain.Entities.Daroo;
using Infrastructure;
using Infrastructure.Exceptions;
using Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.CQRS
{
    // ===== CREATE SERVICEFEATURE COMMAND =====
    public record CreateServiceFeatureCommand(
        string Name,
        string? Description = null,
        string? Code = null,
        string? Icon = null,
        string? Color = null,
        int DisplayOrder = 0
    ) : IRequest<long>;

    public class CreateServiceFeatureCommandHandler : IRequestHandler<CreateServiceFeatureCommand, long>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateServiceFeatureCommandHandler(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ISSOClient sSOClient,
            IOptions<AppSettingsOption> appSettingOption)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
        }

        public async Task<long> Handle(CreateServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            // دریافت اطلاعات کاربر
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var serviceFeature = new ServiceFeature
            {
                Id = _context.GetLastId<ServiceFeature>() + 1,
                Name = request.Name,
                Description = request.Description,
                CreateUserId = result.Data.NationalCode,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            _context.ServiceFeature.Add(serviceFeature);
            await _context.SaveChangesAsync(cancellationToken);

            return serviceFeature.Id;
        }
    }

    // ===== UPDATE SERVICEFEATURE COMMAND =====
    public record UpdateServiceFeatureCommand(
        int Id,
        string Name,
        string? Description = null,
        string? Code = null,
        string? Icon = null,
        string? Color = null,
        int DisplayOrder = 0,
        bool IsActive = true
    ) : IRequest<bool>;

    public class UpdateServiceFeatureCommandHandler : IRequestHandler<UpdateServiceFeatureCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateServiceFeatureCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            var serviceFeature = await _context.ServiceFeature
                .FirstOrDefaultAsync(sf => sf.Id == request.Id && !sf.IsDeleted, cancellationToken);

            if (serviceFeature == null)
                return false;

            serviceFeature.Name = request.Name;
            serviceFeature.IsActive = request.IsActive;
            serviceFeature.ModifyDate = DateTime.Now;

            _context.ServiceFeature.Update(serviceFeature);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== DELETE SERVICEFEATURE COMMAND =====
    public record DeleteServiceFeatureCommand(int Id) : IRequest<bool>;

    public class DeleteServiceFeatureCommandHandler : IRequestHandler<DeleteServiceFeatureCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteServiceFeatureCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            var serviceFeature = await _context.ServiceFeature
                .FirstOrDefaultAsync(sf => sf.Id == request.Id && !sf.IsDeleted, cancellationToken);

            if (serviceFeature == null)
                return false;

            serviceFeature.IsDeleted = true;
            serviceFeature.ModifyDate = DateTime.Now;

            _context.ServiceFeature.Update(serviceFeature);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== ASSIGN SERVICEFEATURE TO MAINTITLE COMMAND =====
    public record AssignServiceFeatureToMainTitleCommand(
        int MainTitleId,
        int ServiceFeature,
        bool IsActive = true,
        int DisplayOrder = 0,
        string? Notes = null
    ) : IRequest<long>;

    public class AssignServiceFeatureToMainTitleCommandHandler : IRequestHandler<AssignServiceFeatureToMainTitleCommand, long>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public AssignServiceFeatureToMainTitleCommandHandler(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ISSOClient sSOClient,
            IOptions<AppSettingsOption> appSettingOption)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
        }

        public async Task<long> Handle(AssignServiceFeatureToMainTitleCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود رابطه قبلی (حذف نشده)
            var existingRelation = await _context.MainTitleServiceFeature
                .FirstOrDefaultAsync(mtsf => mtsf.MainTitle == request.MainTitleId &&
                                            mtsf.ServiceFeature == request.ServiceFeature &&
                                            !mtsf.IsDeleted, cancellationToken);

            if (existingRelation != null)
                throw new AppException("این ویژگی قبلاً به این عنوان اصلی اختصاص داده شده است");

            // دریافت اطلاعات کاربر
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var relation = new MainTitleServiceFeature
            {
                MainTitle = request.MainTitleId,
                ServiceFeature = request.ServiceFeature,
                IsActive = request.IsActive,
                CreateUserId = result.Data.NationalCode,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            _context.MainTitleServiceFeature.Add(relation);
            await _context.SaveChangesAsync(cancellationToken);

            return relation.Id;
        }
    }

    // ===== UPDATE MAINTITLE-SERVICEFEATURE RELATION COMMAND =====
    public record UpdateMainTitleServiceFeatureCommand(
        int Id,
        bool IsActive,
        int DisplayOrder = 0,
        string? Notes = null
    ) : IRequest<bool>;

    public class UpdateMainTitleServiceFeatureCommandHandler : IRequestHandler<UpdateMainTitleServiceFeatureCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateMainTitleServiceFeatureCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMainTitleServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            var relation = await _context.MainTitleServiceFeature
                .FirstOrDefaultAsync(mtsf => mtsf.Id == request.Id && !mtsf.IsDeleted, cancellationToken);

            if (relation == null)
                return false;

         

            relation.IsActive = request.IsActive;
            relation.ModifyDate = DateTime.Now;

            _context.MainTitleServiceFeature.Update(relation);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== REMOVE SERVICEFEATURE FROM MAINTITLE COMMAND =====
    public record RemoveServiceFeatureFromMainTitleCommand(int Id) : IRequest<bool>;

    public class RemoveServiceFeatureFromMainTitleCommandHandler : IRequestHandler<RemoveServiceFeatureFromMainTitleCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public RemoveServiceFeatureFromMainTitleCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveServiceFeatureFromMainTitleCommand request, CancellationToken cancellationToken)
        {
            var relation = await _context.MainTitleServiceFeature
                .FirstOrDefaultAsync(mtsf => mtsf.Id == request.Id && !mtsf.IsDeleted, cancellationToken);

            if (relation == null)
                return false;

            relation.IsDeleted = true;
            relation.ModifyDate = DateTime.Now;

            _context.MainTitleServiceFeature.Update(relation);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}