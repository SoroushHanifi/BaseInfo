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

namespace Application.CQRS
{
    // ===== CREATE COMMAND =====
    public record CreateMainTitleCommand(
        string Name,
        string? Description,
        decimal Amount,
        int ScopeId,
        int DisplayOrder = 0
    ) : IRequest<int>;

    public class CreateMainTitleCommandHandler : IRequestHandler<CreateMainTitleCommand, int>
    {
        private readonly DarooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimHelper _claimHelper;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateMainTitleCommandHandler(
            IClaimHelper claimHelper,
            ISSOClient sSOClient,
            DarooDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettingsOption> appSettingOption)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _claimHelper = claimHelper;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
        }

        public async Task<int> Handle(CreateMainTitleCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود حوزه
            var scopeExists = await _context.Scopes
                .AnyAsync(s => s.Id == request.ScopeId && !s.IsDelete, cancellationToken);

            if (!scopeExists)
                throw new AppException("حوزه مورد نظر یافت نشد");

            // بررسی تکراری نبودن نام در همین حوزه
            var nameExists = await _context.MainTitles
                .AnyAsync(mt => mt.Name == request.Name &&
                               mt.ScopeId == request.ScopeId &&
                               !mt.IsDelete, cancellationToken);

            if (nameExists)
                throw new AppException("نام عنوان اصلی در این حوزه تکراری است");

            // بررسی مبلغ (نباید منفی باشد)
            if (request.Amount < 0)
                throw new AppException("مبلغ نمی‌تواند منفی باشد");

            // دریافت اطلاعات کاربر
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var mainTitle = new MainTitle
            {
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                ScopeId = request.ScopeId,
                DisplayOrder = request.DisplayOrder,
                CreateUserId = result.Data.NationalCode,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            _context.MainTitles.Add(mainTitle);
            await _context.SaveChangesAsync(cancellationToken);

            return mainTitle.Id;
        }
    }

    // ===== UPDATE COMMAND =====
    public record UpdateMainTitleCommand(
        int Id,
        string Name,
        string? Description,
        decimal Amount,
        int DisplayOrder
    ) : IRequest<bool>;

    public class UpdateMainTitleCommandHandler : IRequestHandler<UpdateMainTitleCommand, bool>
    {
        private readonly DarooDbContext _context;

        public UpdateMainTitleCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMainTitleCommand request, CancellationToken cancellationToken)
        {
            var mainTitle = await _context.MainTitles
                .FirstOrDefaultAsync(mt => mt.Id == request.Id && !mt.IsDelete, cancellationToken);

            if (mainTitle == null)
                return false;

            // بررسی تکراری نبودن نام در همین حوزه (جز خودش)
            var nameExists = await _context.MainTitles
                .AnyAsync(mt => mt.Name == request.Name &&
                               mt.ScopeId == mainTitle.ScopeId &&
                               mt.Id != request.Id &&
                               !mt.IsDelete, cancellationToken);

            if (nameExists)
                throw new AppException("نام عنوان اصلی در این حوزه تکراری است");

            // بررسی مبلغ
            if (request.Amount < 0)
                throw new AppException("مبلغ نمی‌تواند منفی باشد");

            mainTitle.Name = request.Name;
            mainTitle.Description = request.Description;
            mainTitle.Amount = request.Amount;
            mainTitle.DisplayOrder = request.DisplayOrder;
            mainTitle.ModifyDate = DateTime.Now;

            _context.MainTitles.Update(mainTitle);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== DELETE COMMAND (Soft Delete) =====
    public record DeleteMainTitleCommand(int Id) : IRequest<bool>;

    public class DeleteMainTitleCommandHandler : IRequestHandler<DeleteMainTitleCommand, bool>
    {
        private readonly DarooDbContext _context;

        public DeleteMainTitleCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMainTitleCommand request, CancellationToken cancellationToken)
        {
            var mainTitle = await _context.MainTitles
                .FirstOrDefaultAsync(mt => mt.Id == request.Id && !mt.IsDelete, cancellationToken);

            if (mainTitle == null)
                return false;

            mainTitle.IsDelete = true;
            mainTitle.ModifyDate = DateTime.Now;

            _context.MainTitles.Update(mainTitle);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== UPDATE DISPLAY ORDER COMMAND =====
    public record UpdateMainTitleDisplayOrderCommand(int Id, int DisplayOrder) : IRequest<bool>;

    public class UpdateMainTitleDisplayOrderCommandHandler : IRequestHandler<UpdateMainTitleDisplayOrderCommand, bool>
    {
        private readonly DarooDbContext _context;

        public UpdateMainTitleDisplayOrderCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMainTitleDisplayOrderCommand request, CancellationToken cancellationToken)
        {
            var mainTitle = await _context.MainTitles
                .FirstOrDefaultAsync(mt => mt.Id == request.Id && !mt.IsDelete, cancellationToken);

            if (mainTitle == null)
                return false;

            mainTitle.DisplayOrder = request.DisplayOrder;
            mainTitle.ModifyDate = DateTime.Now;

            _context.MainTitles.Update(mainTitle);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== BULK UPDATE DISPLAY ORDER COMMAND =====
    public record BulkUpdateMainTitleDisplayOrderCommand(List<MainTitleDisplayOrderItem> Items) : IRequest<bool>;

    public class MainTitleDisplayOrderItem
    {
        public int Id { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class BulkUpdateMainTitleDisplayOrderCommandHandler : IRequestHandler<BulkUpdateMainTitleDisplayOrderCommand, bool>
    {
        private readonly DarooDbContext _context;

        public BulkUpdateMainTitleDisplayOrderCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(BulkUpdateMainTitleDisplayOrderCommand request, CancellationToken cancellationToken)
        {
            if (!request.Items.Any())
                return false;

            var ids = request.Items.Select(x => x.Id).ToList();
            var mainTitles = await _context.MainTitles
                .Where(mt => ids.Contains(mt.Id) && !mt.IsDelete)
                .ToListAsync(cancellationToken);

            if (!mainTitles.Any())
                return false;

            foreach (var mainTitle in mainTitles)
            {
                var orderItem = request.Items.FirstOrDefault(x => x.Id == mainTitle.Id);
                if (orderItem != null)
                {
                    mainTitle.DisplayOrder = orderItem.DisplayOrder;
                    mainTitle.ModifyDate = DateTime.Now;
                }
            }

            _context.MainTitles.UpdateRange(mainTitles);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== VALIDATORS =====
    public class CreateMainTitleValidator : AbstractValidator<CreateMainTitleCommand>
    {
        public CreateMainTitleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام عنوان اصلی الزامی است")
                .MinimumLength(2).WithMessage("نام عنوان اصلی باید حداقل 2 کاراکتر باشد")
                .MaximumLength(300).WithMessage("نام عنوان اصلی نباید بیشتر از 300 کاراکتر باشد");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("توضیحات نباید بیشتر از 1000 کاراکتر باشد")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("مبلغ نمی‌تواند منفی باشد")
                .ScalePrecision(2, 18).WithMessage("مبلغ باید حداکثر 18 رقم کل و 2 رقم اعشار داشته باشد");

            RuleFor(x => x.ScopeId)
                .GreaterThan(0).WithMessage("شناسه حوزه باید عددی مثبت باشد");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("ترتیب نمایش نمی‌تواند منفی باشد");
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
                .MaximumLength(300).WithMessage("نام عنوان اصلی نباید بیشتر از 300 کاراکتر باشد");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("توضیحات نباید بیشتر از 1000 کاراکتر باشد")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("مبلغ نمی‌تواند منفی باشد")
                .ScalePrecision(2, 18).WithMessage("مبلغ باید حداکثر 18 رقم کل و 2 رقم اعشار داشته باشد");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("ترتیب نمایش نمی‌تواند منفی باشد");
        }
    }
}