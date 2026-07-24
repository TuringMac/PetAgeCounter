using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAgeCounter
{
    public class PetItemDatabase
    {
        SQLiteAsyncConnection database;

        async Task Init()
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await database.CreateTableAsync<PetItem>();
        }

        public async Task<List<PetItem>> GetItemsAsync()
        {
            await Init();
            return await database.Table<PetItem>().ToListAsync();
        }

        public async Task<PetItem> GetItemAsync(int id)
        {
            await Init();
            return await database.Table<PetItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(PetItem item)
        {
            await Init();
            if (item.Id != 0)
                return await database.UpdateAsync(item);
            else
                return await database.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(PetItem item)
        {
            await Init();
            return await database.DeleteAsync(item);
        }
    }
}
