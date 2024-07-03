using SQLite;
using People.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace People
{
    public class PersonRepository
    {
        private string _dbPath;
        private SQLiteAsyncConnection conn;

        public string StatusMessage { get; set; }

        public PersonRepository(string dbPath)
        {
            _dbPath = dbPath;
        }

        private async Task Init()
        {
            if (conn != null)
                return;

            conn = new SQLiteAsyncConnection(_dbPath);
            await conn.CreateTableAsync<PersonEA>();
        }

        public async Task AddNewPerson(string name)
        {
            int result = 0;
            try
            {
                // Call Init()
                await Init();

                // Basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(name))
                    throw new Exception("Valid name required");

                // Insert the new person asynchronously
                result = await conn.InsertAsync(new PersonEA { Name = name });

                StatusMessage = string.Format("{0} record(s) added (Name: {1})", result, name);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", name, ex.Message);
            }
        }

        public async Task<List<PersonEA>> GetAllPeople()
        {
            try
            {
                await Init();
                return await conn.Table<PersonEA>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<PersonEA>();
        }
    }
}
