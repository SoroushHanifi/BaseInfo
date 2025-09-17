using Application.Common;
using Domain.Entities.Daroo;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS
{
    // ===== DTOs =====
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsDelete { get; set; }
    }

    // ===== GetAll Query =====
    public record GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>;

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllDepartmentsQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(d => !d.IsDelete)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate
                })
                .ToListAsync(cancellationToken);
        }
    }
    public record GetAllDepartmentsPaginationQuery(int PageIndex = 1, int PageSize = 10) : IRequest<PagedData<DepartmentDto>>;

    public class GetAllDepartmentsPaginationQueryHandler : IRequestHandler<GetAllDepartmentsPaginationQuery, PagedData<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public GetAllDepartmentsPaginationQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<DepartmentDto>> Handle(GetAllDepartmentsPaginationQuery request, CancellationToken cancellationToken)
        {
            // تنظیم مقادیر پیش‌فرض
            int pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            // دریافت تعداد کل رکوردها
            int totalCount = await _context.Departments
                .Where(d => !d.IsDelete)
                .CountAsync(cancellationToken);

            // محاسبه تعداد صفحات
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // دریافت داده‌های صفحه‌بندی‌شده
            var items = await _context.Departments
                .Where(d => !d.IsDelete)
                .OrderBy(d => d.Name) // مرتب‌سازی بر اساس نام (اختیاری)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserId = d.CreateUserId,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDelete = d.IsDelete
                })
                .ToListAsync(cancellationToken);

            return new PagedData<DepartmentDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                IndexFrom = (pageIndex - 1) * pageSize,
                Items = items,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }
    }

    // ===== GetById Query =====
    public record GetDepartmentByIdQuery(int Id) : IRequest<DepartmentDto?>;

    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
    {
        private readonly DarooDbContext _context;

        public GetDepartmentByIdQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<DepartmentDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(d => d.Id == request.Id && !d.IsDelete)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDelete = d.IsDelete
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    // ===== GetActive Query (اختیاری - فقط رکوردهای فعال) =====
    public record GetActiveDepartmentsQuery : IRequest<List<DepartmentDto>>;

    public class GetActiveDepartmentsQueryHandler : IRequestHandler<GetActiveDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public GetActiveDepartmentsQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> Handle(GetActiveDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments
                .Where(d => !d.IsDelete)
                .OrderBy(d => d.Id)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDelete = d.IsDelete
                })
                .ToListAsync(cancellationToken);
        }
    }

    public record GetActiveDepartmentsPaginationQuery(int PageIndex = 1, int PageSize = 10) : IRequest<PagedData<DepartmentDto>>;
    public class GetActiveDepartmentsPaginationQueryHandler : IRequestHandler<GetActiveDepartmentsPaginationQuery, PagedData<DepartmentDto>>
    {
        private readonly DarooDbContext _context;

        public GetActiveDepartmentsPaginationQueryHandler(DarooDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<DepartmentDto>> Handle(GetActiveDepartmentsPaginationQuery request, CancellationToken cancellationToken)
        {
            int pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            int totalCount = await _context.Departments
                .Where(d => !d.IsDelete)
                .CountAsync(cancellationToken);

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await _context.Departments
                .Where(d => !d.IsDelete)
                .OrderBy(d => d.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    CreateUserId = d.CreateUserId,
                    CreateDate = d.CreateDate,
                    ModifyDate = d.ModifyDate,
                    IsDelete = d.IsDelete
                })
                .ToListAsync(cancellationToken);

            return new PagedData<DepartmentDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                IndexFrom = (pageIndex - 1) * pageSize,
                Items = items,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }
    }
}