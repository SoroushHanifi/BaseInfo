using Application.Models;
using Application.OptionPatternModel;
using Application.Refits;
using Domain;
using Domain.Entities.Daroo;
using FluentValidation;
using Infrastructure;
using Infrastructure.Exceptions;
using Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Application.CQRS
{
    // ===== CREATE COMMAND =====
    public record CreateMainTitleCommand(
     string Name,
     string? Description,
     long ScopeId,
     string? DisplayOrder = "0",
     long? BpmType = null,
     long? ProduceType = null,
     List<MainTitleServiceFeatureInput>? ServiceFeatures = null  // 👈 اضافه شد
 ) : IRequest<long>;

    public class CreateMainTitleCommandHandler : IRequestHandler<CreateMainTitleCommand, long>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimHelper _claimHelper;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateMainTitleCommandHandler(
            IClaimHelper claimHelper,
            ISSOClient sSOClient,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettingsOption> appSettingOption)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _claimHelper = claimHelper;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
        }

        public async Task<long> Handle(CreateMainTitleCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود حوزه
            var scopeExists = await _context.Scopes
                .AnyAsync(s => s.Id == request.ScopeId && s.IsDeleted != true, cancellationToken);

            if (!scopeExists)
                throw new AppException("حوزه مورد نظر یافت نشد");

            // بررسی تکراری نبودن نام در همین حوزه
            var nameExists = await _context.MainTitles
                .AnyAsync(mt => mt.Name == request.Name &&
                               mt.ScopesId == request.ScopeId &&
                               mt.IsDeleted != true, cancellationToken);

            if (nameExists)
                throw new AppException("نام عنوان اصلی در این حوزه تکراری است");


            // 👇 بررسی معتبر بودن ServiceFeatureها
            if (request.ServiceFeatures != null && request.ServiceFeatures.Any())
            {
                var serviceFeatureIds = request.ServiceFeatures.Select(sf => sf.ServiceFeature).ToList();
                var existingServiceFeaturesCount = await _context.ServiceFeature
                    .Where(sf => serviceFeatureIds.Contains(sf.Id) && !sf.IsDeleted)
                    .CountAsync(cancellationToken);

                if (existingServiceFeaturesCount != serviceFeatureIds.Count)
                    throw new AppException("برخی از ویژگی‌های خدمات وارد شده معتبر نیستند");
            }

            // دریافت اطلاعات کاربر
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            // ساخت MainTitle
            var mainTitle = new MainTitle
            {
                Id = _context.GetLastId<MainTitle>() + 1,
                Name = request.Name,
                Description = request.Description,
                ScopesId = request.ScopeId,
                ProduceType = request.ProduceType,
                DisplayOrder = request.DisplayOrder,
                BpmType = request.BpmType,
                CreateUserID = result.Data.NationalCode
            };

            mainTitle.PrepareForCreation();
            _context.MainTitles.Add(mainTitle);

            var restetste = await _context.MainTitleServiceFeature.ToListAsync();

            // 👇 اضافه کردن ServiceFeature‌ها
            if (request.ServiceFeatures != null && request.ServiceFeatures.Any())
            {
                try
                {
                    int counter = 1;
                    foreach (var sfInput in request.ServiceFeatures)
                    {
                        var mainTitleServiceFeature = new MainTitleServiceFeature
                        {
                            Id = _context.GetLastId<MainTitleServiceFeature>() + counter,
                            MainTitle = (int)mainTitle.Id,
                            ServiceFeature = sfInput.ServiceFeature,
                            IsActive = true,
                            CreateUserId = result.Data.NationalCode,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            IsDeleted = false
                        };

                        _context.MainTitleServiceFeature.Add(mainTitleServiceFeature);
                        counter++;
                    }
                }
                catch (Exception ex)
                {

                    var test = ex.Message;
                    throw new Exception(ex.Message);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return mainTitle.Id;
        }
    }

    public record UpdateMainTitleCommand(
        long Id,
        string Name,
        string? Description,
        decimal Amount,
        string? DisplayOrder,
        long? BpmType = null
    ) : IRequest<bool>;

    public class UpdateMainTitleCommandHandler : IRequestHandler<UpdateMainTitleCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public UpdateMainTitleCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMainTitleCommand request, CancellationToken cancellationToken)
        {
            var mainTitle = await _context.MainTitles
                .FirstOrDefaultAsync(mt => mt.Id == request.Id && mt.IsDeleted != true, cancellationToken);

            if (mainTitle == null)
                return false;

            // بررسی تکراری نبودن نام در همین حوزه (جز خودش)
            var nameExists = await _context.MainTitles
                .AnyAsync(mt => mt.Name == request.Name &&
                               mt.ScopesId == mainTitle.ScopesId &&
                               mt.Id != request.Id &&
                               mt.IsDeleted != true, cancellationToken);

            if (nameExists)
                throw new AppException("نام عنوان اصلی در این حوزه تکراری است");

            // بررسی مبلغ
            if (request.Amount < 0)
                throw new AppException("مبلغ نمی‌تواند منفی باشد");

            mainTitle.Name = request.Name;
            mainTitle.Description = request.Description;
            mainTitle.DisplayOrder = request.DisplayOrder;
            mainTitle.BpmType = request.BpmType;
            mainTitle.PrepareForUpdate();

            _context.MainTitles.Update(mainTitle);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public record DeleteMainTitleCommand(long Id) : IRequest<bool>;

    public class DeleteMainTitleCommandHandler : IRequestHandler<DeleteMainTitleCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteMainTitleCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMainTitleCommand request, CancellationToken cancellationToken)
        {
            var mainTitle = await _context.MainTitles
                .FirstOrDefaultAsync(mt => mt.Id == request.Id && mt.IsDeleted != true, cancellationToken);

            if (mainTitle == null)
                return false;

            mainTitle.SoftDelete();

            _context.MainTitles.Update(mainTitle);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }


    public class UpdateMainTitleValidator : AbstractValidator<UpdateMainTitleCommand>
    {
        public UpdateMainTitleValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه عنوان اصلی باید عددی مثبت باشد");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام عنوان اصلی الزامی است")
                .MinimumLength(2).WithMessage("نام عنوان اصلی باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام عنوان اصلی نباید بیشتر از 50 کاراکتر باشد");

            RuleFor(x => x.Description)
                .MaximumLength(150).WithMessage("توضیحات نباید بیشتر از 150 کاراکتر باشد")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("مبلغ نمی‌تواند منفی باشد");

            RuleFor(x => x.DisplayOrder)
                .MaximumLength(50).WithMessage("ترتیب نمایش نباید بیشتر از 50 کاراکتر باشد")
                .When(x => !string.IsNullOrWhiteSpace(x.DisplayOrder));
        }
    }
}