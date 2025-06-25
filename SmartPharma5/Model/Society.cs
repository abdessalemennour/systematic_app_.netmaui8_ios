using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class Society
    {
        #region Attributes
        public int Id { get; set; }
        public string name { get; set; }
        public static int Count { get; set; } = 0;

        public ObservableCollection<Society> SocietyList { get; set; }
        public static List<Society> CachedSocieties { get; private set; } = null;

        #endregion

        #region Constructors
        public Society()
        {
        }

        public Society(int id, string name)
        {
            Id = id;
            this.name = name;
        }
        public Society(int id, string name, int count)  
        {
            Id = id;
            this.name = name;
            Count = count;
        }
       // public static int LoadedSocietyCount => CachedSocieties?.FirstOrDefault()?.Count ?? 0;

        #endregion

        #region Methods
        public static async Task<Society> GetSocietyById(int id)
        {
            var societies = await GetAllSocietiesAsync();
            return societies?.FirstOrDefault(s => s.Id == id);
        }

        // Nouvelle méthode avec userId en paramètre
        public static async Task<List<Society>> GetSocietiesByUserIdAsync(int userId)
        {
            if (userId <= 0)
                return new List<Society>();

            List<Society> societies = new List<Society>();
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(DbConnection.ConnectionString);
                await connection.OpenAsync();

                string query = @"
                    SELECT s.id, s.name 
                    FROM atooerp_socity s
                    INNER JOIN atooerp_socity_user su ON s.id = su.socity
                    WHERE su.user = @userId
                    ORDER BY s.name";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<(int id, string name)> rawList = new();

                        while (await reader.ReadAsync())
                        {
                            rawList.Add((
                                Convert.ToInt32(reader["id"]),
                                reader["name"]?.ToString() ?? ""
                            ));
                        }

                        int count = rawList.Count;

                        foreach (var item in rawList)
                        {
                            societies.Add(new Society(item.id, item.name, count));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting societies for user {userId}: {ex.Message}");
                return new List<Society>();
            }
            finally
            {
                if (connection != null)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
            }

            return societies;
        }

        // Ancienne méthode conservée sans modification
        public static async Task<List<Society>> GetAllSocietiesAsync()
        {
            int userId = Preferences.Get("iduser", 0);
            
            // Vérifier si le cache existe et s'il correspond au bon utilisateur
            if (CachedSocieties != null && CachedSocieties.Count > 0)
            {
                // Si le cache existe, on peut le retourner
                return CachedSocieties;
            }

            List<Society> societies = new List<Society>();
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(DbConnection.ConnectionString);
                await connection.OpenAsync();

                string query = @"
                    SELECT s.id, s.name 
                    FROM atooerp_socity s
                    INNER JOIN atooerp_socity_user su ON s.id = su.socity
                    WHERE su.user = @userId
                    ORDER BY s.name";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<(int id, string name)> rawList = new();

                        while (await reader.ReadAsync())
                        {
                            rawList.Add((
                                Convert.ToInt32(reader["id"]),
                                reader["name"]?.ToString() ?? ""
                            ));
                        }
                        int count = rawList.Count;

                        foreach (var item in rawList)
                        {
                            societies.Add(new Society(item.id, item.name, count));
                        }
                    }
                }

                CachedSocieties = societies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user societies: {ex.Message}");
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
            }

            return CachedSocieties;
        }

        public static async Task ClearSocietiesCache()
        {
            CachedSocieties = null;
            Count = 0;
        }
        #endregion
    }
}