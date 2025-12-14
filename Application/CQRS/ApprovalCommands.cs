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
    // ===== CREATE APPROVAL COMMAND =====
    /// <summary>
    /// دستور ایجاد تعرفه جدید
    /// ExecutionDate به صورت خودکار با تاریخ الان پر می‌شود
    /// TariffEndDate NULL است و با ساخت تعرفه بعدی پر می‌شود
    /// </summary>
    public record CreateApprovalCommand(
        DateTime TariffStartDate,
        long MainTitleId,
        long Amount,
        bool IsActive = true
    ) : IRequest<CreateApprovalResult>;

    public class CreateApprovalResult
    {
        public long ApprovalId { get; set; }
        public List<long> UpdatedApprovalIds { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class CreateApprovalCommandHandler : IRequestHandler<CreateApprovalCommand, CreateApprovalResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimHelper _claimHelper;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;
        private readonly IMediator _mediator;

        public CreateApprovalCommandHandler(
            IClaimHelper claimHelper,
            ISSOClient sSOClient,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettingsOption> appSettingOption,
            IMediator mediator)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _claimHelper = claimHelper;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
            _mediator = mediator;
        }

        public async Task<CreateApprovalResult> Handle(CreateApprovalCommand request, CancellationToken cancellationToken)
        {
            var result = new CreateApprovalResult();

            var mainTitleExists = await _context.MainTitles
                .AnyAsync(mt => mt.Id == request.MainTitleId && mt.IsDeleted != true, cancellationToken);

            if (!mainTitleExists)
                throw new AppException("عنوان اصلی مورد نظر یافت نشد");

            var existingApprovals = await _context.Approvals
                .Where(a => a.MainTitleId == request.MainTitleId &&
                           a.IsActive == true &&
                           a.TariffStartDate.HasValue)
                .OrderBy(a => a.TariffStartDate)
                .ToListAsync(cancellationToken);

            var newTariffStart = request.TariffStartDate;

            // پردازش تعرفه‌های همپوشان
            foreach (var existingApproval in existingApprovals)
            {
                if (newTariffStart <= existingApproval.TariffStartDate)
                {
                    existingApproval.TariffEndDate = DateTime.Now;
                    existingApproval.IsActive = false;
                    _context.Approvals.Update(existingApproval);
                    result.UpdatedApprovalIds.Add(existingApproval.Id);
                }
                else if (!existingApproval.TariffEndDate.HasValue)
                {
                    existingApproval.TariffEndDate = newTariffStart;
                    _context.Approvals.Update(existingApproval);
                    result.UpdatedApprovalIds.Add(existingApproval.Id);
                }
                else if(existingApproval.TariffEndDate <= DateTime.Now) 
                {
                    existingApproval.IsActive = false;
                    _context.Approvals.Update(existingApproval);
                    result.UpdatedApprovalIds.Add(existingApproval.Id);
                }

            }

            // ساخت تعرفه جدید
            var approval = new Approval
            {
                Id = _context.GetLastId<Approval>() + 1,
                ExecutionDate = DateTime.Now, // همیشه زمان ساخت
                TariffStartDate = request.TariffStartDate,
                TariffEndDate = null, // همیشه NULL - با تعرفه بعدی پر می‌شود
                MainTitleId = request.MainTitleId,
                Amount = request.Amount,
                IsActive = request.IsActive,
                FinalEnt = 10021,
                BaGuid = Guid.NewGuid()
            };

            // Set BaCreatedTime
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            approval.BaCreatedTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync(cancellationToken);

            result.ApprovalId = approval.Id;

            try
            {
                await _mediator.Send(new CreateCasePaymentDifferenceCommand(approval.MainTitleId, approval.TariffStartDate ?? DateTime.Now, approval.TariffEndDate ?? DateTime.Now, approval.Amount));

            }
            catch (Exception ex)
            {
                var test = ex.InnerException.Message;
                throw ex;
            }

            if (result.UpdatedApprovalIds.Any())
            {
                result.Message = $"تعرفه جدید ایجاد شد و {result.UpdatedApprovalIds.Count} تعرفه قبلی به‌روزرسانی شد";
            }
            else
            {
                result.Message = "تعرفه جدید با موفقیت ایجاد شد";
            }

            return result;
        }
    }

    // ===== DELETE APPROVAL COMMAND =====
    /// <summary>
    /// حذف تعرفه - تنها عملیات مجاز بعد از ایجاد
    /// Update مجاز نیست
    /// </summary>
    public record DeleteApprovalCommand(long Id, bool AdjustPreviousTariff = true) : IRequest<DeleteApprovalResult>;

    public class DeleteApprovalResult
    {
        public bool Success { get; set; }
        public long? AdjustedApprovalId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DeleteApprovalCommandHandler : IRequestHandler<DeleteApprovalCommand, DeleteApprovalResult>
    {
        private readonly ApplicationDbContext _context;

        public DeleteApprovalCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeleteApprovalResult> Handle(DeleteApprovalCommand request, CancellationToken cancellationToken)
        {
            var result = new DeleteApprovalResult();

            var approval = await _context.Approvals
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (approval == null)
            {
                result.Success = false;
                result.Message = "تعرفه یافت نشد";
                return result;
            }

            var mainTitleId = approval.MainTitleId;
            var tariffStartDate = approval.TariffStartDate;

            // حذف تعرفه
            _context.Approvals.Remove(approval);

            // اگر نیاز به تنظیم مجدد تعرفه قبلی است
            if (request.AdjustPreviousTariff && tariffStartDate.HasValue)
            {
                // پیدا کردن آخرین تعرفه‌ای که قبل از تعرفه حذف شده شروع شده
                var previousApproval = await _context.Approvals
                    .Where(a => a.MainTitleId == mainTitleId &&
                               a.TariffStartDate < tariffStartDate &&
                               a.Id != request.Id)
                    .OrderByDescending(a => a.TariffStartDate)
                    .FirstOrDefaultAsync(cancellationToken);

                if (previousApproval != null)
                {
                    // اگر تعرفه قبلی TariffEndDate داشت که برابر با شروع تعرفه حذف شده بود
                    if (previousApproval.TariffEndDate.HasValue &&
                        previousApproval.TariffEndDate.Value == tariffStartDate.Value)
                    {
                        // پیدا کردن تعرفه بعدی
                        var nextApproval = await _context.Approvals
                            .Where(a => a.MainTitleId == mainTitleId &&
                                       a.TariffStartDate > tariffStartDate &&
                                       a.Id != request.Id)
                            .OrderBy(a => a.TariffStartDate)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (nextApproval != null)
                        {
                            // TariffEndDate تعرفه قبلی را روی شروع تعرفه بعدی قرار می‌دهیم
                            previousApproval.TariffEndDate = nextApproval.TariffStartDate;
                        }
                        else
                        {
                            // تعرفه بعدی وجود ندارد، پس TariffEndDate را NULL می‌کنیم
                            previousApproval.TariffEndDate = null;
                        }

                        _context.Approvals.Update(previousApproval);
                        result.AdjustedApprovalId = previousApproval.Id;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Message = result.AdjustedApprovalId.HasValue
                ? "تعرفه حذف شد و تعرفه قبلی تنظیم شد"
                : "تعرفه با موفقیت حذف شد";

            return result;
        }
    }

    // ===== TOGGLE ACTIVE STATUS COMMAND =====
    /// <summary>
    /// تغییر وضعیت فعال/غیرفعال تعرفه
    /// تنها خاصیت قابل تغییر بعد از ایجاد
    /// </summary>
    public record ToggleApprovalActiveStatusCommand(long Id) : IRequest<bool>;

    public class ToggleApprovalActiveStatusCommandHandler : IRequestHandler<ToggleApprovalActiveStatusCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public ToggleApprovalActiveStatusCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ToggleApprovalActiveStatusCommand request, CancellationToken cancellationToken)
        {
            var approval = await _context.Approvals
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (approval == null)
                return false;

            approval.IsActive = !approval.IsActive;

            _context.Approvals.Update(approval);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== VALIDATORS =====
    public class CreateApprovalValidator : AbstractValidator<CreateApprovalCommand>
    {
        public CreateApprovalValidator()
        {
            RuleFor(x => x.MainTitleId)
                .GreaterThan(0).WithMessage("شناسه عنوان اصلی باید عددی مثبت باشد");

            RuleFor(x => x.TariffStartDate)
                .NotEmpty().WithMessage("تاریخ شروع تعرفه الزامی است");

            // می‌توان تاریخ گذشته هم انتخاب کرد
            // RuleFor(x => x.TariffStartDate)
            //     .GreaterThanOrEqualTo(DateTime.Today)
            //     .WithMessage("تاریخ شروع تعرفه نمی‌تواند در گذشته باشد");
        }
    }
}