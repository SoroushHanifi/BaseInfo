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
    ) : IRequest<int>;

    public class CreateServiceFeatureCommandHandler : IRequestHandler<CreateServiceFeatureCommand, int>
    {
        private readonly DarooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateServiceFeatureCommandHandler(
            DarooDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ISSOClient sSOClient,
            IOptions<AppSettingsOption> appSettingOption)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
        }

        public async Task<int> Handle(CreateServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            // دریافت اطلاعات کاربر
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var serviceFeature = new ServiceFeature
            {
                Name = request.Name,
                Description = request.Description,
                Code = request.Code,
                Icon = request.Icon,
                Color = request.Color,
                DisplayOrder = request.DisplayOrder,
                CreateUserId = result.Data.NationalCode,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            _context.ServiceFeatures.Add(serviceFeature);
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
        private readonly DarooDbContext _context;

        public UpdateServiceFeatureCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            var serviceFeature = await _context.ServiceFeatures
                .FirstOrDefaultAsync(sf => sf.Id == request.Id && !sf.IsDelete, cancellationToken);

            if (serviceFeature == null)
                return false;

            serviceFeature.Name = request.Name;
            serviceFeature.Description = request.Description;
            serviceFeature.Code = request.Code;
            serviceFeature.Icon = request.Icon;
            serviceFeature.Color = request.Color;
            serviceFeature.DisplayOrder = request.DisplayOrder;
            serviceFeature.IsActive = request.IsActive;
            serviceFeature.ModifyDate = DateTime.Now;

            _context.ServiceFeatures.Update(serviceFeature);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== DELETE SERVICEFEATURE COMMAND =====
    public record DeleteServiceFeatureCommand(int Id) : IRequest<bool>;

    public class DeleteServiceFeatureCommandHandler : IRequestHandler<DeleteServiceFeatureCommand, bool>
    {
        private readonly DarooDbContext _context;

        public DeleteServiceFeatureCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            var serviceFeature = await _context.ServiceFeatures
                .FirstOrDefaultAsync(sf => sf.Id == request.Id && !sf.IsDelete, cancellationToken);

            if (serviceFeature == null)
                return false;

            serviceFeature.IsDelete = true;
            serviceFeature.ModifyDate = DateTime.Now;

            _context.ServiceFeatures.Update(serviceFeature);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== ASSIGN SERVICEFEATURE TO MAINTITLE COMMAND =====
    public record AssignServiceFeatureToMainTitleCommand(
        int MainTitleId,
        int ServiceFeatureId,
        bool IsActive = true,
        int DisplayOrder = 0,
        string? Notes = null
    ) : IRequest<int>;

    public class AssignServiceFeatureToMainTitleCommandHandler : IRequestHandler<AssignServiceFeatureToMainTitleCommand, int>
    {
        private readonly DarooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public AssignServiceFeatureToMainTitleCommandHandler(
            DarooDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ISSOClient sSOClient,
            IOptions<AppSettingsOption> appSettingOption)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
        }

        public async Task<int> Handle(AssignServiceFeatureToMainTitleCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود رابطه قبلی (حذف نشده)
            var existingRelation = await _context.MainTitleServiceFeatures
                .FirstOrDefaultAsync(mtsf => mtsf.MainTitleId == request.MainTitleId &&
                                            mtsf.ServiceFeatureId == request.ServiceFeatureId &&
                                            !mtsf.IsDelete, cancellationToken);

            if (existingRelation != null)
                throw new AppException("این ویژگی قبلاً به این عنوان اصلی اختصاص داده شده است");

            // دریافت اطلاعات کاربر
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var relation = new MainTitleServiceFeature
            {
                MainTitleId = request.MainTitleId,
                ServiceFeatureId = request.ServiceFeatureId,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder,
                Notes = request.Notes,
                ActivatedDate = request.IsActive ? DateTime.Now : null,
                CreateUserId = result.Data.NationalCode,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            _context.MainTitleServiceFeatures.Add(relation);
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
        private readonly DarooDbContext _context;

        public UpdateMainTitleServiceFeatureCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMainTitleServiceFeatureCommand request, CancellationToken cancellationToken)
        {
            var relation = await _context.MainTitleServiceFeatures
                .FirstOrDefaultAsync(mtsf => mtsf.Id == request.Id && !mtsf.IsDelete, cancellationToken);

            if (relation == null)
                return false;

            // اگر وضعیت تغییر کرد، تاریخ‌ها رو آپدیت کن
            if (relation.IsActive != request.IsActive)
            {
                if (request.IsActive)
                {
                    relation.ActivatedDate = DateTime.Now;
                    relation.DeactivatedDate = null;
                }
                else
                {
                    relation.DeactivatedDate = DateTime.Now;
                }
            }

            relation.IsActive = request.IsActive;
            relation.DisplayOrder = request.DisplayOrder;
            relation.Notes = request.Notes;
            relation.ModifyDate = DateTime.Now;

            _context.MainTitleServiceFeatures.Update(relation);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== REMOVE SERVICEFEATURE FROM MAINTITLE COMMAND =====
    public record RemoveServiceFeatureFromMainTitleCommand(int Id) : IRequest<bool>;

    public class RemoveServiceFeatureFromMainTitleCommandHandler : IRequestHandler<RemoveServiceFeatureFromMainTitleCommand, bool>
    {
        private readonly DarooDbContext _context;

        public RemoveServiceFeatureFromMainTitleCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveServiceFeatureFromMainTitleCommand request, CancellationToken cancellationToken)
        {
            var relation = await _context.MainTitleServiceFeatures
                .FirstOrDefaultAsync(mtsf => mtsf.Id == request.Id && !mtsf.IsDelete, cancellationToken);

            if (relation == null)
                return false;

            relation.IsDelete = true;
            relation.DeactivatedDate = DateTime.Now;
            relation.ModifyDate = DateTime.Now;

            _context.MainTitleServiceFeatures.Update(relation);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}