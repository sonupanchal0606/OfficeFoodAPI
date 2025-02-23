using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Model;

namespace OfficeFoodAPI.Handlers
{
    public class MenuItemHandler
    {
        private FoodDbContext _context;

        public MenuItemHandler(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetMenuItem()
        {
            var list = await _context.menuitem_mstr.ToListAsync();
            if (list == null)
            {
                throw new Exception("No records found");
            }
            return list;
        }

        public async Task<MenuItem> GetMenuById(Guid menuitemid)
        {
            var record = await _context.menuitem_mstr.FindAsync(menuitemid);
            if (record == null)
            {
                throw new Exception("No records found");
            }

            return record;
        }


        public async Task<MenuItem> PostMenuItem(MenuItem_post value)
        {
            MenuItem menuitem = new MenuItem()
            {
                itemname = value.itemname,
                price = value.price.Value,
                vendorid = value.vendorid.Value,
                createdat = DateTime.UtcNow,
                upatedat = DateTime.UtcNow,
            };
            await _context.menuitem_mstr.AddAsync(menuitem);
            await _context.SaveChangesAsync();

            var record = await _context.menuitem_mstr.FindAsync(menuitem.menuitemid);
            if (record == null)
            {
                throw new Exception("Error in saving records");
            }

            return record;
        }


        public async Task<MenuItem> PutMenuItem(Guid menuitemid, MenuItem_post value)
        {
            var data = await _context.menuitem_mstr.Where(c => c.menuitemid == menuitemid).FirstOrDefaultAsync();
            if (data == null)
            {
                throw new Exception("No record to update");
            }
            if (value.itemname != null)
            {
                data.itemname = value.itemname;
            }
            if (value.price != null)
            {
                data.price = value.price.Value  ;
            }
            if (value.vendorid != null)
            {
                data.vendorid = value.vendorid.Value;
            }
            data.upatedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return data;
        }
        
        public async Task<JsonResult> DeleteMenuItem(Guid menuitemid)
        {
            var record = await _context.menuitem_mstr.FindAsync(menuitemid);
            _context.menuitem_mstr.Remove(record);
            await _context.SaveChangesAsync();
            
            return new JsonResult("Menu Item deleted :" + record.itemname);
        }
    }
}
