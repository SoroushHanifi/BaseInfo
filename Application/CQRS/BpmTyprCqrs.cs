using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    // ===================== QUERIES =====================

    #region Get All Query
    public record GetAllBpmTypeQuery : IRequest<List<BpmType>>;

    public class GetAllBpmTypeQueryHandler : IRequestHandler<GetAllBpmTypeQuery, List<BpmType>>
    {
        private readonly DarooDbContext _context;

        public GetAllBpmTypeQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<BpmType>> Handle(GetAllBpmTypeQuery request, CancellationToken cancellationToken)
        {
            return await _context.BpmTypes
                .Where(pt => pt.IsDeleted != true)
                .OrderBy(pt => pt.Name)
                .Select(pt => new BpmType
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    CreateDate = pt.CreateDate,
                    ModifyDate = pt.ModifyDate,
                    IsDeleted = pt.IsDeleted,
                    FinalEnt = pt.FinalEnt,
                    BaCreatedTime = pt.BaCreatedTime,
                    BaGuid = pt.BaGuid
                })
                .ToListAsync(cancellationToken);



        }
    }
    #endregion

    #region Get By Id Query
    public record GetBpmTypeByIdQuery(long Id) : IRequest<BpmType?>;

    public class GetBpmTypeByIdQueryHandler : IRequestHandler<GetBpmTypeByIdQuery, BpmType?>
    {
        private readonly DarooDbContext _context;

        public GetBpmTypeByIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<BpmType?> Handle(GetBpmTypeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.BpmTypes
                .Where(pt => pt.Id == request.Id && pt.IsDeleted != true)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
    #endregion

    // ===================== COMMANDS =====================

    #region Create Command
    public record CreateBpmTypeCommand(string Name) : IRequest<BpmType>;

    public class CreateBpmTypeCommandHandler : IRequestHandler<CreateBpmTypeCommand, BpmType>
    {
        private readonly DarooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimHelper _claimHelper;
        private readonly ISSOClient _sSOClient;
        private readonly AppSettingsOption _appSettingsOption;

        public CreateBpmTypeCommandHandler(
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

        public async Task<BpmType> Handle(CreateBpmTypeCommand request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_appSettingsOption.Settings.CookieInfo.Name];
            var result = await _sSOClient.GetCurrentUser($"{_appSettingsOption.Settings.CookieInfo.Name}=" + token);

            if (result.IsSuccess is false || result.Data is null)
                throw new AppException(Messages.UserNotFound);
            // Check for duplicate name
            var exists = await _context.BpmTypes
                .AnyAsync(pt => pt.Name == request.Name && pt.IsDeleted != true, cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException($"نوع فرایند با نام '{request.Name}' قبلاً ثبت شده است");
            }

            var bpmType = new BpmType
            {
                Id = _context.GetLastId<BpmType>() + 1,
                Name = request.Name,
                CreateDate = DateTime.Now,
                CreateUserID = result.Data.UserName,
                IsDeleted = false
            };

            _context.BpmTypes.Add(bpmType);
            await _context.SaveChangesAsync(cancellationToken);

            return bpmType;
        }
    }
    #endregion

    #region Update Command
    public record UpdateBpmTypeCommand(long Id, string Name) : IRequest<BpmType>;

    public class UpdateBpmTypeCommandHandler : IRequestHandler<UpdateBpmTypeCommand, BpmType>
    {
        private readonly DarooDbContext _context;

        public UpdateBpmTypeCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<BpmType> Handle(UpdateBpmTypeCommand request, CancellationToken cancellationToken)
        {
            var bpmType = await _context.BpmTypes
                .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsDeleted != true, cancellationToken);

            if (bpmType == null)
            {
                throw new KeyNotFoundException($"نوع فرایند با شناسه {request.Id} یافت نشد");
            }

            // Check for duplicate name (excluding current record)
            var duplicateExists = await _context.BpmTypes
                .AnyAsync(pt => pt.Name == request.Name && pt.Id != request.Id && pt.IsDeleted != true, cancellationToken);

            if (duplicateExists)
            {
                throw new InvalidOperationException($"نوع فرایند با نام '{request.Name}' قبلاً ثبت شده است");
            }

            bpmType.Name = request.Name;
            bpmType.ModifyDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return bpmType;
        }
    }
    #endregion

    #region Delete Command
    public record DeleteBpmTypeCommand(long Id) : IRequest<bool>;

    public class DeleteBpmTypeCommandHandler : IRequestHandler<DeleteBpmTypeCommand, bool>
    {
        private readonly DarooDbContext _context;

        public DeleteBpmTypeCommandHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteBpmTypeCommand request, CancellationToken cancellationToken)
        {
            var bpmType = await _context.BpmTypes
                .FirstOrDefaultAsync(pt => pt.Id == request.Id && pt.IsDeleted != true, cancellationToken);

            if (bpmType == null)
            {
                throw new KeyNotFoundException($"نوع فرایند با شناسه {request.Id} یافت نشد");
            }

            // Soft delete
            bpmType.IsDeleted = true;
            bpmType.ModifyDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
    #endregion
}
