using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Model;

namespace OfficeFoodAPI.Handlers
{
    public class UserTypeHandler
    {
        private FoodDbContext _context;

        public UserTypeHandler(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserType>> GetUserTypes()
        {
            var list = await _context.usertype_mstr.ToListAsync();
            if (list == null)
            {
                throw new Exception("No records found");
            }
            return list;
        }

        public async Task<UserType> GetUserTypeById(Guid usertypeid)
        {
            var record = await _context.usertype_mstr.Where(u => u.usertypeid == usertypeid).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new Exception("No records found");
            }

            return record;
        }


        public async Task<UserType> PostUserType(UserType_post value)
        {
            UserType usertype = new UserType()
            {
                usertype = value.usertype,
                permission = value.permission.Value,
                createdat = DateTime.UtcNow,
                upatedat = DateTime.UtcNow,
            };
            await _context.usertype_mstr.AddAsync(usertype);
            await _context.SaveChangesAsync();

            var record = await _context.usertype_mstr.Where(u => u.usertypeid == usertype.usertypeid).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new Exception("Error in saving records");
            }
            return record;
        }


        public async Task<UserType> PutUserType(Guid usertypeid, UserType_post value)
        {
            var data = await _context.usertype_mstr.Where(c => c.usertypeid == usertypeid).FirstOrDefaultAsync();
            if (data == null)
            {
                throw new Exception("No record to update");
            }
            if (value.usertype != null)
            {
                data.usertype = value.usertype;
            }
            if (value.permission != null)
            {
                data.permission = value.permission.Value;
            }
            data.upatedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<JsonResult> DeleteUserType(Guid usertypeid)
        {
            var record = await _context.usertype_mstr.Where(u => u.usertypeid == usertypeid).FirstOrDefaultAsync();
            _context.usertype_mstr.Remove(record);
            await _context.SaveChangesAsync();

            return new JsonResult("User Type deleted :" + record.usertype);
        }
    }
}
