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
    public record CreateScopeCommand(string Name, long DepartmentId) : IRequest<long>;

    public class CreateScopeCommandHandler : IRequestHandler<CreateScopeCommand, long>
    {
        private readonly DarooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimHelper _claimHelper;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateScopeCommandHandler(
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

        public async Task<long> Handle(CreateScopeCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود اداره کل
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == request.DepartmentId && d.IsDeleted != true, cancellationToken);

            if (!departmentExists)
                throw new AppException("اداره کل مورد نظر یافت نشد");

            // بررسی تکراری نبودن نام در همین اداره کل
            var nameExists = await _context.Scopes
                .AnyAsync(s => s.Name == request.Name &&
                              s.DepartmentId == request.DepartmentId &&
                              s.IsDeleted != true, cancellationToken);

            if (nameExists)
                throw new AppException("نام حوزه در این اداره کل تکراری است");

            // دریافت اطلاعات کاربر
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var scope = new Scope
            {
                Name = request.Name,
                DepartmentId = request.DepartmentId,
                CreateUserID = long.Parse(result.Data.NationalCode)
            };

            scope.PrepareForCreation();

            _context.Scopes.Add(scope);
            await _context.SaveChangesAsync(cancellationToken);

            return scope.Id;
        }
    }

    public record UpdateScopeCommand(long Id, string Name) : IRequest<bool>;

    public class UpdateScopeCommandHandler : IRequestHandler<UpdateScopeCommand, bool>
    {
        private readonly DarooDbContext _context;

        public UpdateScopeCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateScopeCommand request, CancellationToken cancellationToken)
        {
            var scope = await _context.Scopes
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

            if (scope == null)
                return false;

            // بررسی تکراری نبودن نام در همین اداره کل (جز خودش)
            var nameExists = await _context.Scopes
                .AnyAsync(s => s.Name == request.Name &&
                              s.DepartmentId == scope.DepartmentId &&
                              s.Id != request.Id &&
                              s.IsDeleted != true, cancellationToken);

            if (nameExists)
                throw new AppException("نام حوزه در این اداره کل تکراری است");

            scope.Name = request.Name;
            scope.PrepareForUpdate();

            _context.Scopes.Update(scope);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public record DeleteScopeCommand(long Id) : IRequest<bool>;

    public class DeleteScopeCommandHandler : IRequestHandler<DeleteScopeCommand, bool>
    {
        private readonly DarooDbContext _context;

        public DeleteScopeCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteScopeCommand request, CancellationToken cancellationToken)
        {
            var scope = await _context.Scopes
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsDeleted != true, cancellationToken);

            if (scope == null)
                return false;

            scope.SoftDelete();

            _context.Scopes.Update(scope);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }


    // Validator برای CreateScopeCommand
    public class CreateScopeValidator : AbstractValidator<CreateScopeCommand>
    {
        public CreateScopeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام حوزه الزامی است")
                .MinimumLength(2).WithMessage("نام حوزه باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام حوزه نباید بیشتر از 50 کاراکتر باشد");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("شناسه اداره کل باید عددی مثبت باشد");
        }
    }

    public class UpdateScopeValidator : AbstractValidator<UpdateScopeCommand>
    {
        public UpdateScopeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه حوزه باید عددی مثبت باشد");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام حوزه الزامی است")
                .MinimumLength(2).WithMessage("نام حوزه باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام حوزه نباید بیشتر از 50 کاراکتر باشد");
        }
    }

}