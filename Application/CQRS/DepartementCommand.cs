using Application.OptionPatternModel;
using Application.Refits;
using Domain;
using Domain.Entities.Daroo;
using Infrastructure;
using Infrastructure.Exceptions;
using Infrastructure.Utility;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Refit;

namespace Application.CQRS
{
    public record CreateDepartmentCommand(string Name) : IRequest<int>;

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
    {
        private readonly DarooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimHelper _claimHelper;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateDepartmentCommandHandler(IClaimHelper claimHelper, ISSOClient sSOClient,DarooDbContext context, IHttpContextAccessor httpContextAccessor, IOptions<AppSettingsOption> appSettingOption)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _claimHelper = claimHelper;
            _sSOClient = sSOClient;
            _appSettingsOption = appSettingOption.Value;
        }

        public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);

            var department = new Department
            {
                CreateUserId = result.Data.NationalCode,
                Name = request.Name,
                CreateDate = DateTime.UtcNow,
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }

    // Update Command
    public record UpdateDepartmentCommand(int Id, string Name) : IRequest<bool>;

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

            if (department == null)
                return false;

            department.Name = request.Name;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // Delete Command
    public record DeleteDepartmentCommand(int Id) : IRequest<bool>;

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

            if (department == null || department.IsDelete)
                return false;

            department.IsDelete = true;
            department.ModifyDate = DateTime.UtcNow; // اضافه کردن ModifyDate

            _context.Departments.Update(department);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
