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
                .OrderBy(d => d.Name)
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
}