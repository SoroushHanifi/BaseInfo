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
using Microsoft.Extensions.Options;
using Refit;

namespace Application.CQRS
{
    // ===== CREATE COMMAND =====
    public record CreateDepartmentCommand(string Name) : IRequest<long>;

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, long>
    {
        private readonly DarooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimHelper _claimHelper;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateDepartmentCommandHandler(
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

        public async Task<long> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var department = new Department
            {
                Name = request.Name,
                CreateUserID = int.Parse(result.Data.NationalCode), // Convert to int
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                FinalEnt = 10008, // Bizagi default
                BaGuid = Guid.NewGuid(),
                IsDeleted = false
            };

            // Set BaCreatedTime to current Unix timestamp in milliseconds
            department.SetBaCreatedTimeToNow();

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }

    // ===== UPDATE COMMAND =====
    public record UpdateDepartmentCommand(long Id, string Name) : IRequest<bool>;

    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
    {
        private readonly DarooDbContext _context;

        public UpdateDepartmentCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _context.Departments.FindAsync(request.Id, cancellationToken);

            if (department == null || department.IsDeleted == true)
                return false;

            department.Name = request.Name;
            department.ModifyDate = DateTime.Now;

            _context.Departments.Update(department);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== DELETE COMMAND (Soft Delete) =====
    public record DeleteDepartmentCommand(long Id) : IRequest<bool>;

    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly DarooDbContext _context;

        public DeleteDepartmentCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _context.Departments.FindAsync(request.Id, cancellationToken);

            if (department == null || department.IsDeleted == true)
                return false;

            department.IsDeleted = true;
            department.ModifyDate = DateTime.Now;

            _context.Departments.Update(department);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // ===== VALIDATORS =====
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام اداره کل الزامی است")
                .MinimumLength(2).WithMessage("نام اداره کل باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام اداره کل نباید بیشتر از 50 کاراکتر باشد"); // Updated to match Bizagi limit
        }
    }

    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه اداره کل باید عددی مثبت باشد");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام اداره کل الزامی است")
                .MinimumLength(2).WithMessage("نام اداره کل باید حداقل 2 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام اداره کل نباید بیشتر از 50 کاراکتر باشد"); // Updated to match Bizagi limit
        }
    }
}