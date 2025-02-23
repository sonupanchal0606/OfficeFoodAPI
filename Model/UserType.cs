using System.ComponentModel.DataAnnotations;

namespace OfficeFoodAPI.Model
{
    public class UserType
    {
        [Key]
        public Guid usertypeid { get; set; } 
        public string usertype { get; set; } // companyHR, employee, vendor, Admin
        public Permission? permission { get; set; } = Permission.ReadWrite;
        public DateTime createdat { get; set; }
        public DateTime upatedat { get; set; }
    }


    public class UserType_post
    {
        public string? usertype { get; set; } // companyHR, employee, vendor, Admin
        public Permission? permission { get; set; } = Permission.ReadWrite;
    }

    public enum Permission
    {
        ReadOnly = 0,
        ReadWrite = 1,
        NoDelete = 2
    }

}
