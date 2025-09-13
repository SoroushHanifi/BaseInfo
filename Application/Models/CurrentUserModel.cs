using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class CurrentUserModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NationalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string CompanyName { get; set; }
        public string AvatarFilePath { get; set; }
        public string Mobile { get; set; }
        public int Type { get; set; }
        public int? DarooUserType { get; set; }
        public List<UnitRoleDto> UnitRoles { get; set; }
        public List<UserSystemDto> UserSystems { get; set; }
    }
    public class UniversityInfoDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class UnitRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class UserSystemDto
    {
        public Guid Id { get; set; }
        public string UnitRoleCode { get; set; }
        public string ProvinceCode { get; set; }
        public string CityCode { get; set; }
    }

    public class GetAvatarsByNationalCodes
    {
        public string NationalCode { get; set; }
        public string AvatarPath { get; set; }
    }

    public class SignFileData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SignFilePath { get; set; }
        public string EnglishName { get; set; }

    }
}
