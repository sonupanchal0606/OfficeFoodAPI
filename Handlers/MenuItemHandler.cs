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
            var list = await _context.menuitem_mstr.Include(c => c.vendor).ToListAsync();
            
       
            if (list == null)
            {
                throw new Exception("No records found");
            }
            return list;
        }


        public async Task<MenuItem> GetMenuById(Guid menuitemid)
        {
            var record = await _context.menuitem_mstr.Where( m=> m.menuitemid == menuitemid).Include(c => c.vendor).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new Exception("No records found");
            }

            return record;
        }
        

        public async Task<MenuItem> GetMenuByVendorId(Guid vendorid, Guid menuitemid)
        {
            var record = await _context.menuitem_mstr.Where(m=> m.vendorid == vendorid && m.menuitemid == menuitemid).FirstOrDefaultAsync();
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
                updatedat = DateTime.UtcNow,
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
            var data = await _context.menuitem_mstr.Where(c => c.menuitemid == menuitemid && c.vendorid == value.vendorid).FirstOrDefaultAsync();
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

            data.updatedat = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return data;
        }
        
        // vendor can delete the menu item
        public async Task<JsonResult> DeleteMenuItem(Guid vendorid, Guid menuitemid)
        {
            var record = await _context.menuitem_mstr.Where(m => m.vendorid == vendorid && m.menuitemid == menuitemid).FirstOrDefaultAsync();
            _context.menuitem_mstr.Remove(record);
            await _context.SaveChangesAsync();
            
            return new JsonResult("Menu Item deleted :" + record.itemname);
        }
    }
}
