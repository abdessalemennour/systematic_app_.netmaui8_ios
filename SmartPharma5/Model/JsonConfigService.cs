    using System;
    using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Xml;

    namespace SmartPharma5.Model
    {
        public class ServerSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _customName;
        public string CustomName
        {
            get => _customName;
            set
            {
                _customName = value;
                OnPropertyChanged(nameof(CustomName));
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }
        public string Address { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
            public string Database { get; set; }
            public int Port { get; set; }
            public int ConnectionId { get; set; }
            public int CurrentConnection { get; set; }
            public DateTime ConnectionDate { get; set; } = DateTime.Now;
            public string Login { get; set; }
            public string PasswordLogin { get; set; }
            //public string CustomName { get; set; }
            public bool IsNotCurrentConnection =>
            Preferences.Get("current_connection_id", 0) != ConnectionId;
        protected void OnPropertyChanged(string name) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Nouvelle classe pour stocker la liste des configurations
    public class ServerSettingsHistory
        {
            public List<ServerSettings> Connections { get; set; } = new List<ServerSettings>();
            public int ConnectionCounter { get; set; } = 0;
            public int TotalConnections => Connections?.Count ?? 0; // Nouvelle propriété en lecture seule



        // Méthode pour initialiser le compteur en fonction des connexions existantes
        public void InitializeCounter()
        {
            if (Connections != null && Connections.Count > 0)
            {
                // Trouver la valeur maximale de ConnectionId dans la liste existante
                ConnectionCounter = Connections.Max(c => c.ConnectionId);
            }
        }
        // Méthode utilitaire pour ajouter une connexion
        public void AddConnection(ServerSettings settings)
        {
            // Initialiser le compteur avec les valeurs déjà présentes
            InitializeCounter();

            bool connectionExists = Connections.Any(c =>
                c.Address == settings.Address &&
                c.Database == settings.Database &&
                c.Name == settings.Name &&
                c.Port == settings.Port &&
                c.Password == settings.Password);

            if (!connectionExists)
            {
                ConnectionCounter++; // Incrémenter ici avant d'assigner l'id
                settings.ConnectionId = ConnectionCounter;
                settings.CustomName = $"connexion{ConnectionCounter}";

                settings.ConnectionDate = DateTime.Now;
                Connections.Add(settings);
            }
            else
            {
                var existingConnection = Connections.First(c =>
                    c.Address == settings.Address &&
                    c.Database == settings.Database &&
                    c.Name == settings.Name &&
                    c.Port == settings.Port &&
                    c.Password == settings.Password);

                existingConnection.ConnectionDate = DateTime.Now;
                // Si jamais le nom n'était pas défini (cas rare)
                if (string.IsNullOrEmpty(existingConnection.CustomName))
                {
                    existingConnection.CustomName = $"connexion{existingConnection.ConnectionId}";
                }
            }
        }
    }

    public class JsonConfigService
        {
            private readonly string _filePath;
            private readonly string _historyFilePath;

            public JsonConfigService()
            {
                _filePath = Path.Combine(FileSystem.AppDataDirectory, "server_settings.json");
                _historyFilePath = Path.Combine(FileSystem.AppDataDirectory, "server_settings_history.json");
            }
public async Task<bool> UpdateConnectionName(string connectionId, string newCustomName)
{
    try
    {
        var history = await LoadHistoryAsync();

        if (history?.Connections != null)
        {
            if (!int.TryParse(connectionId, out int parsedId))
                return false;

            var connection = history.Connections.FirstOrDefault(c => c.ConnectionId == parsedId);

            if (connection != null)
            {
                connection.CustomName = newCustomName;

                // Sauvegarder les changements
                await SaveHistoryAsync(history);
                return true;
            }
        }

        return false;
    }
    catch (Exception ex)
    {
        // Log ou traitement d’erreur si nécessaire
        return false;
    }
}


        public async Task SaveHistoryAsync(ServerSettingsHistory history)
        {
            try
            {
                string json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_historyFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sauvegarde historique: {ex.Message}");
            }
        }
        // Sauvegarder les données dans le fichier JSON (dernière connexion)
        public async Task SaveSettingsAsync(ServerSettings settings)
            {
                try
                {
                    string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(_filePath, json);

                    // Ajouter aussi à l'historique
                    await AddToHistoryAsync(settings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la sauvegarde du fichier JSON : {ex.Message}");
                }
            }

        // Ajouter une connexion à l'historique
        public async Task AddToHistoryAsync(ServerSettings settings)
        {
            try
            {
                var history = await LoadHistoryAsync() ?? new ServerSettingsHistory();
                history.InitializeCounter();

                if (settings.ConnectionId == 0) // Nouvelle connexion
                {
                    // NE PAS incrémenter ici, ce sera fait dans AddConnection
                    settings.CustomName = $"connexion{history.ConnectionCounter + 1}"; // Prévision du prochain ID
                }
                else if (string.IsNullOrEmpty(settings.CustomName))
                {
                    settings.CustomName = $"connexion{settings.ConnectionId}";
                }

                history.AddConnection(settings);

                string json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_historyFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout à l'historique : {ex.Message}");
            }
        }
        // Charger l'historique des connexions
        public async Task<ServerSettingsHistory> LoadHistoryAsync()
            {
                try
                {
                    if (File.Exists(_historyFilePath))
                    {
                        string json = await File.ReadAllTextAsync(_historyFilePath);
                        return JsonSerializer.Deserialize<ServerSettingsHistory>(json);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement de l'historique : {ex.Message}");
                }
                return new ServerSettingsHistory();
            }

            // Charger les données de la dernière connexion depuis le fichier JSON
            public async Task<ServerSettings> LoadSettingsAsync()
            {
                try
                {
                    if (File.Exists(_filePath))
                    {
                        string json = await File.ReadAllTextAsync(_filePath);
                        return JsonSerializer.Deserialize<ServerSettings>(json);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la lecture du fichier JSON : {ex.Message}");
                }
                return null;
            }

            // Supprimer le fichier 
            public void DeleteSettingsFile()
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
            }

            // Supprimer l'historique 
            public void DeleteHistoryFile()
            {
                if (File.Exists(_historyFilePath))
                {
                    File.Delete(_historyFilePath);
                }
            }

            // Retourner le chemin complet (utile pour debug)
            public string GetSettingsFilePath() => _filePath;

            // Retourner le chemin de l'historique
            public string GetHistoryFilePath() => _historyFilePath;

            // Méthode sécurisée pour charger les paramètres
            public async Task<ServerSettings> SafeLoadSettingsAsync()
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        Console.WriteLine("Le fichier JSON n'existe pas.");
                        return null;
                    }
                    string json = await File.ReadAllTextAsync(_filePath);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        Console.WriteLine("Le fichier est vide.");
                        return null;
                    }
                    var settings = JsonSerializer.Deserialize<ServerSettings>(json);
                    if (settings == null)
                    {
                        Console.WriteLine("Les données JSON sont invalides ou corrompues.");
                    }
                    return settings;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement du fichier JSON : {ex.Message}");
                    return null;
                }
            }

            // Méthode pour obtenir le contenu brut du fichier JSON
            public async Task<string> GetRawJsonContentAsync()
            {
                try
                {
                    if (File.Exists(_filePath))
                    {
                        return await File.ReadAllTextAsync(_filePath);
                    }
                    return "Le fichier n'existe pas.";
                }
                catch (Exception ex)
                {
                    return $"Erreur lors de la lecture du fichier : {ex.Message}";
                }
            }

        // Méthode pour obtenir tout l'historique formaté
        public async Task<string> GetFormattedHistoryAsync()
        {
            try
            {
                var history = await LoadHistoryAsync();
                if (history == null || history.Connections.Count == 0)
                {
                    return "Aucun historique de connexion n'est disponible.";
                }

                // Trier par date, du plus récent au plus ancien
                var sortedConnections = history.Connections.OrderByDescending(c => c.ConnectionDate).ToList();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("===== Historique des connexions =====");
                sb.AppendLine($"Total de connexions enregistrées : {history.TotalConnections}");
                sb.AppendLine("--------------------------------------");
                sb.AppendLine($"Dernier ID utilisé : {history.ConnectionCounter}");
                foreach (var conn in sortedConnections)
                {
                    sb.AppendLine($"nom  : {conn.CustomName}");

                    sb.AppendLine($"Connection ID : {conn.ConnectionId}");
                    sb.AppendLine($"Login: {conn.Login}");
                    sb.AppendLine($"Mot de passe login: {conn.PasswordLogin}");
                    sb.AppendLine($"Date: {conn.ConnectionDate:dd/MM/yyyy HH:mm:ss}");
                    sb.AppendLine($"Serveur: {conn.Address}:{conn.Port}");
                    sb.AppendLine($"Base de données: {conn.Database}");
                    sb.AppendLine($"Utilisateur: {conn.Name}");
                    sb.AppendLine($"Mot de passe: {conn.Password}");
                    sb.AppendLine("--------------------------------------");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Erreur lors de la récupération de l'historique : {ex.Message}";
            }
        }


        // Méthode pour obtenir le contenu brut de l'historique
        public async Task<string> GetRawHistoryJsonContentAsync()
            {
                try
                {
                    if (File.Exists(_historyFilePath))
                    {
                        return await File.ReadAllTextAsync(_historyFilePath);
                    }
                    return "Le fichier d'historique n'existe pas.";
                }
                catch (Exception ex)
                {
                    return $"Erreur lors de la lecture du fichier d'historique : {ex.Message}";
                }
            }
        public async Task<bool> RemoveConnectionAsync(int connectionId)
        {
            try
            {
                var history = await LoadHistoryAsync();
                if (history != null && history.Connections != null)
                {
                    var connectionToRemove = history.Connections.FirstOrDefault(c => c.ConnectionId == connectionId);
                    if (connectionToRemove != null)
                    {
                        history.Connections.Remove(connectionToRemove);
                        // Sauvegarder l'historique après suppression
                        string json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                        await File.WriteAllTextAsync(_historyFilePath, json);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression de la connexion : {ex.Message}");
            }

            return false;
        }
    }
}