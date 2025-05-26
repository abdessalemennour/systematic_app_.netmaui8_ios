using SmartPharma5.Model;
using SQLite;

namespace SmartPharma5.Services
{
    public static class User_Module_Groupe_Services
    {

        /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
        Avant :
                static SQLiteAsyncConnection db;

                static async Task Init()
        Après :
                static SQLiteAsyncConnection db;

                static async Task Init()
        */
        static SQLiteAsyncConnection db;

        static async Task Init()
        {
            if (db != null)
                return;
           //var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MyData.db");
           var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");

            // db = new SQLiteAsyncConnection(databasePath);
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<User_module_groupe>();





        }
        public static async Task Adddb(User_module_groupe umg)
        {
            await Init();
            var id = await db.InsertAsync(umg);
            var customeradd = await db.Table<User_module_groupe>().ToListAsync();

        }
        public static async Task Remove(int id)
        {
            await Init();
            await db.DeleteAsync<User_module_groupe>(id);
        }

        public static async Task<User_module_groupe> GetGroupeCRM(int iduser)
        {
            await Init();
            var c = await db.Table<User_module_groupe>().CountAsync();
            var umg = await db.Table<User_module_groupe>().Where(g => g.IdUser == iduser && g.IdModule == 14).FirstOrDefaultAsync();
            return umg;
        }

        public static async Task<User_module_groupe> GetGroupeComp(int iduser)
        {
            await Init();
            var c = await db.Table<User_module_groupe>().CountAsync();
            var umg = await db.Table<User_module_groupe>().Where(g => g.IdUser == iduser && g.IdModule == 8).FirstOrDefaultAsync();
            return umg;
        }

        public static async Task<int> UMGcount()
        {
            int c = 0;
            try
            {
                c = await db.Table<User_module_groupe>().CountAsync();
            }
            catch (Exception ex)
            {

            }
            return c;
        }
        public static async Task DeleteAll()
        {
            await Init();
            try
            {
                await db.DeleteAllAsync<User_module_groupe>();
            }
            catch (Exception ex)
            {

            }


        }
        //public static async Task Update(int id, decimal val)
        //{
        //    await Init();

        //    Opportunity_Line existingopp = await (db.Table<Opportunity_Line>().Where(
        //            a => a.Id == id)).FirstOrDefaultAsync();
        //    if (existingopp != null)
        //    {
        //        existingopp.quantity = val;
        //        int success = await db.UpdateAsync(existingopp);
        //    }

        //}
    }
}
