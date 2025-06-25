using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using MvvmHelpers;
using MySqlConnector;
using SQLite;
using System.ComponentModel;
using System.Data;
using System;
using System.ComponentModel;
using SmartPharma5.ModelView;  // Référence à l'espace de noms où se trouve CustomNavigationDrawerViewModel

namespace SmartPharma5.Model
{
    public class NoteActivityChat
    {
    }
    public class Memo : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int? Piece { get; set; }
        public DateTime ModifyDate { get; set; }
        public int? Modify_user { get; set; }
        public int? Create_user { get; set; }
        public string CreateUserLogin { get; set; }
        public string PieceType { get; set; }

        // Constructeur pour l'ajout d'un nouveau Memo
        public Memo(string name, string description, int createUserId)
        {
            Name = name;
            Description = description;
            CreateDate = DateTime.Now;
            Piece = 5;
            PieceType = "Memo";
            ModifyDate = DateTime.Now;
            Modify_user = 1;  // Tu peux garder cette valeur ou la modifier dynamiquement si nécessaire
            Create_user = createUserId; // Utilisation de l'ID dynamique
        }

        public Memo(string name, string description, int createUserId, object linkedObject)
        {
            Name = name;
            Description = description;
            CreateDate = DateTime.Now;
            Piece = (int?)linkedObject?.GetType().GetProperty("Id")?.GetValue(linkedObject);
            PieceType = linkedObject?.GetType().Name;
            ModifyDate = DateTime.Now;
            Modify_user = 1;  // Tu peux garder cette valeur ou la modifier dynamiquement si nécessaire
            Create_user = createUserId; // Utilisation de l'ID dynamique
        }
        public static async Task<bool> UpdateMemoInDatabase(Memo memo)
        {
            if (memo == null)
            {
                Console.WriteLine("Le mémo est null.");
                return false;
            }

            if (memo.Id == 0)
            {
                Console.WriteLine("L'Id du mémo est invalide.");
                return false;
            }

            const string sqlCmd = @"UPDATE atooerp_note 
                            SET name = @Name, 
                                description = @Description, 
                                modify_date = @ModifyDate, 
                                modify_user = @ModifyUser 
                            WHERE id = @Id;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    if (DbConnection.con == null)
                    {
                        Console.WriteLine("La connexion à la base de données est null.");
                        return false;
                    }

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        // Ajouter les paramètres
                        cmd.Parameters.AddWithValue("@Name", memo.Name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", memo.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifyDate", memo.ModifyDate);
                        cmd.Parameters.AddWithValue("@ModifyUser", memo.Modify_user ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id", memo.Id);

                        // Log des paramètres
                        Console.WriteLine($"Mise à jour du mémo : Id = {memo.Id}, Name = {memo.Name}, Description = {memo.Description}, ModifyDate = {memo.ModifyDate}, ModifyUser = {memo.Modify_user}");

                        // Exécuter la commande
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        DbConnection.Deconnecter();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Mémo mis à jour avec succès.");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Aucune ligne mise à jour. Vérifiez l'Id du mémo.");
                            return false;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Erreur MySQL lors de la mise à jour du mémo : {ex.Message}");
                    DbConnection.Deconnecter();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur inattendue lors de la mise à jour du mémo : {ex.Message}");
                    DbConnection.Deconnecter();
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
                return false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        // Méthode pour enregistrer un Memo dans la base de données
        public async static Task<bool> SaveToDatabase(Memo memo)
        {
            if (memo == null)
                throw new ArgumentNullException(nameof(memo));

            const string sqlCmd = @"INSERT INTO atooerp_note 
            (name, description, piece, piece_type, create_date, create_user, modify_user) 
            VALUES 
            (@Name, @Description, @Piece, @PieceType, @CreateDate, @CreateUser, @ModifyUser);";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        // Ajouter les paramètres
                        cmd.Parameters.AddWithValue("@Name", memo.Name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", memo.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreateDate", memo.CreateDate);
                        cmd.Parameters.AddWithValue("@Piece", memo.Piece ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PieceType", memo.PieceType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreateUser", memo.Create_user);
                        cmd.Parameters.AddWithValue("@ModifyUser", memo.Modify_user ?? (object)DBNull.Value);

                        // Exécuter la commande
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        // Déconnexion de la base de données
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'enregistrement du mémo : {ex.Message}");
                    DbConnection.Deconnecter();
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
                return false;
            }
        }

        public async static Task<bool> DeleteMemoFromDatabase(int memoId)
        {
            const string sqlCmd = "DELETE FROM atooerp_note WHERE id = @MemoId;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@MemoId", memoId);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la suppression du mémo : {ex.Message}");
                    DbConnection.Deconnecter();
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
                return false;
            }
        }


        public async static Task<List<Memo>> GetAllMemosForDrawerControl(int entityId, string entityType)
        {
            List<Memo> memos = new List<Memo>();

            const string sqlCmd = @"SELECT n.*, u.login as creator_login 
                           FROM atooerp_note n
                           LEFT JOIN atooerp_user u ON n.create_user = u.id
                           WHERE n.piece = @entityId AND n.piece_type = @entityType 
                           ORDER BY n.create_date DESC;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var linkedObject = new
                                {
                                    Id = reader["piece"] != DBNull.Value ? Convert.ToInt32(reader["piece"]) : (int?)null,
                                    Name = reader["piece_type"] != DBNull.Value ? reader["piece_type"].ToString() : null
                                };

                                var memo = new Memo(
                                    reader["name"].ToString(),
                                    reader["description"].ToString(),
                                    Convert.ToInt32(reader["create_user"]),
                                    linkedObject
                                )
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    CreateDate = Convert.ToDateTime(reader["create_date"]),
                                    CreateUserLogin = reader["creator_login"] != DBNull.Value ?
                                                      reader["creator_login"].ToString() : "Inconnu"
                                };

                                memos.Add(memo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des mémos : {ex.Message}");
                }
                finally
                {
                    DbConnection.Deconnecter();
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
            }

            return memos;
        }

        // Méthode pour récupérer tous les mémos associés à un DrawerControl
        //public async static Task<List<Memo>> GetAllMemosForDrawerControl(int entityId, string entityType)
        //{
        //    List<Memo> memos = new List<Memo>();

        //    const string sqlCmd = @"SELECT * FROM atooerp_note 
        //                    WHERE piece = @entityId AND piece_type = @entityType 
        //                    ORDER BY create_date DESC;";

        //    DbConnection.Deconnecter();
        //    if (DbConnection.Connecter())
        //    {
        //        try
        //        {
        //            using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
        //            {
        //                cmd.Parameters.AddWithValue("@entityId", entityId);
        //                cmd.Parameters.AddWithValue("@entityType", entityType);

        //                using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var linkedObject = new
        //                        {
        //                            Id = reader["piece"] != DBNull.Value ? Convert.ToInt32(reader["piece"]) : (int?)null,
        //                            Name = reader["piece_type"] != DBNull.Value ? reader["piece_type"].ToString() : null
        //                        };

        //                        var memo = new Memo(
        //                            reader["name"].ToString(),
        //                            reader["description"].ToString(),
        //                            Convert.ToInt32(reader["create_user"]),
        //                            linkedObject // Passage de l'objet lié
        //                        )
        //                        {
        //                            Id = Convert.ToInt32(reader["id"]),
        //                            CreateDate = Convert.ToDateTime(reader["create_date"])
        //                        };

        //                        memos.Add(memo);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Erreur lors de la récupération des mémos : {ex.Message}");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Échec de la connexion à la base de données.");
        //    }

        //    return memos;
        //}



    }
    /* public class Activity : INotifyPropertyChanged
     {
         private DateTime? _doneDate;

         public DateTime CreateDate { get; set; }
         public string Summary { get; set; }
         public DateTime DueDate { get; set; }
         public string ActivityMemo { get; set; }
         public bool IsDone => DoneDate.HasValue;

         public DateTime? DoneDate
         {
             get => _doneDate;
             set
             {
                 _doneDate = value;
                 OnPropertyChanged(nameof(DoneDate)); // Notifie le changement
             }
         }

         public event PropertyChangedEventHandler PropertyChanged;
         protected void OnPropertyChanged(string propertyName)
         {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
         }
     }*/
    public class UserchatModel : UserModel
    {
        public string ImageSource { get; set; }
        public string Name { get; set; }
    }
    public class MessageModel : INotifyPropertyChanged
    {
        public LayoutOptions Alignment { get; set; }  // Pour l'alignement à gauche ou à droite
        public Color BackgroundColor { get; set; }   // Couleur de fond du message
        public int Id { get; set; }
        public string Text { get; set; }         // Le texte du message
        public DateTime CreateDate { get; set; }
        public int? Piece { get; set; }
        public string PieceType { get; set; }
        public int? Sender { get; set; }
        public string PieceInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(PieceType) && Piece.HasValue)
                {
                    return $"{PieceType} #{Piece.Value}";
                }
                return string.Empty;
            }
        }
        // Utilise le même alignement que le message
        public LayoutOptions PieceInfoAlignment
        {
            get
            {
                return Alignment; // Utilise le même alignement que le message
            }
        }
        //public bool ShowUnreadIndicator => IsLastSentMessage && !IsRead; // Affiche unread.png si non lu
        //public bool ShowReadIndicator => IsLastSentMessage && IsRead;    // Affiche read.png si lu
        public bool IsSent => Sender == Preferences.Get("iduser", 0);
        public bool ShowUnreadIndicator => IsSent && !IsRead;
        public bool ShowReadIndicator => IsSent && IsRead;
        private bool _isRead;
        public bool IsRead
        {
            get => _isRead;
            set
            {
                if (_isRead != value)
                {
                    _isRead = value;
                    OnPropertyChanged(nameof(IsRead));
                    OnPropertyChanged(nameof(ShowUnreadIndicator)); // Notifie le changement
                    OnPropertyChanged(nameof(ShowReadIndicator));    // Notifie le changement
                    OnPropertyChanged(nameof(ImageSource));
                }
            }
        }
        public string ImageSource
        {
            get
            {
                // Affiche l'image seulement si le message a été envoyé par l'utilisateur actuel (Sender == utilisateur actuel)
                if (Sender == Preferences.Get("iduser", 0))
                {
                    return IsRead ? "read_icon.png" : "unread_icon.png";
                }
                else
                {
                    // Aucun icône si c'est un message reçu
                    return null;
                }
            }
        }
        private int? _receiver;
        public int? Receiver
        {
            get => _receiver;
            set
            {
                if (_receiver != value)
                {
                    _receiver = value;
                    OnPropertyChanged(nameof(Receiver));
                }
            }
        }
        public DateTime ReadDate { get; internal set; }
        private bool _isLastSentMessage;
        public bool IsLastSentMessage
        {
            get => _isLastSentMessage;
            set
            {
                if (_isLastSentMessage != value)
                {
                    _isLastSentMessage = value;
                    OnPropertyChanged(nameof(IsLastSentMessage));
                    OnPropertyChanged(nameof(ShowUnreadIndicator)); // Notifie le changement
                    OnPropertyChanged(nameof(ShowReadIndicator));   // Notifie le changement                }
                }
            }
        }
        public async static Task<bool> InsertMessageAsync(int piece, string pieceType, string text, int sender, int receiver)
        {
            const string insertMessageSql = @"
    INSERT INTO atooerp_messages (create_date, text, piece, piece_type, sender)
    VALUES (@createDate, @text, @piece, @pieceType, @sender);
    SELECT LAST_INSERT_ID();";

            const string insertReceiverSql = @"
    INSERT INTO atooerp_messages_receiver (message, is_read, read_date, receiver)
    VALUES (@message, @isRead, @readDate, @receiver);";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();

                    int messageId;
                    using (MySqlCommand cmd = new MySqlCommand(insertMessageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@createDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@text", text);
                        cmd.Parameters.AddWithValue("@piece", piece);
                        cmd.Parameters.AddWithValue("@pieceType", pieceType);
                        cmd.Parameters.AddWithValue("@sender", sender);

                        messageId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    using (MySqlCommand cmd = new MySqlCommand(insertReceiverSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@message", messageId);
                        cmd.Parameters.AddWithValue("@isRead", false);
                        cmd.Parameters.AddWithValue("@readDate", DBNull.Value);
                        cmd.Parameters.AddWithValue("@receiver", receiver); // Utiliser le receiver dynamique

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                return false;
            }
        }
        public async static Task<bool> InsertnotifMessageAsync(string text, int sender, int receiver)
        {
            const string insertMessageSql = @"
    INSERT INTO atooerp_messages (create_date, text, sender)
    VALUES (@createDate, @text, @sender);
    SELECT LAST_INSERT_ID();";

            const string insertReceiverSql = @"
    INSERT INTO atooerp_messages_receiver (message, is_read, read_date, receiver)
    VALUES (@message, @isRead, @readDate, @receiver);";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();

                    int messageId;
                    using (MySqlCommand cmd = new MySqlCommand(insertMessageSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@createDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@text", text);
                        cmd.Parameters.AddWithValue("@sender", sender);

                        messageId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    using (MySqlCommand cmd = new MySqlCommand(insertReceiverSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@message", messageId);
                        cmd.Parameters.AddWithValue("@isRead", false);
                        cmd.Parameters.AddWithValue("@readDate", DBNull.Value);
                        cmd.Parameters.AddWithValue("@receiver", receiver); // Utiliser le receiver dynamique

                        await cmd.ExecuteNonQueryAsync();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                return false;
            }
        }


        //public async static Task<List<MessageModel>> GetMessagesAsync(int piece, string pieceType, int receiver)
        //{
        //    List<MessageModel> messages = new List<MessageModel>();

        //    const string sqlCmd = @"
        //    SELECT m.*, mr.is_read 
        //    FROM atooerp_messages m
        //    LEFT JOIN atooerp_messages_receiver mr ON m.id = mr.message
        //    WHERE m.piece = @piece 
        //    AND m.piece_type = @pieceType
        //    AND (m.sender = @receiver OR m.id IN (
        //        SELECT message FROM atooerp_messages_receiver WHERE receiver = @receiver
        //    ))
        //    ORDER BY m.create_date ASC;";

        //    try
        //    {

        //        using (var connection = new MySqlConnection(DbConnection.ConnectionString))
        //        {
        //            await connection.OpenAsync();
        //            using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@piece", piece);
        //                cmd.Parameters.AddWithValue("@pieceType", pieceType);
        //                cmd.Parameters.AddWithValue("@receiver", receiver);

        //                using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var message = new MessageModel
        //                        {
        //                            Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
        //                            CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
        //                            Text = reader["text"] != DBNull.Value ? reader["text"].ToString() : string.Empty,
        //                            Sender = reader["sender"] != DBNull.Value ? Convert.ToInt32(reader["sender"]) : 0,
        //                            IsRead = reader["is_read"] != DBNull.Value && Convert.ToBoolean(reader["is_read"]), // 🔥 Important
        //                            Alignment = reader["sender"] != DBNull.Value && Convert.ToInt32(reader["sender"]) == Preferences.Get("iduser", 0)
        //                             ? LayoutOptions.End
        //                             : LayoutOptions.Start,
        //                            BackgroundColor = reader["sender"] != DBNull.Value && Convert.ToInt32(reader["sender"]) == Preferences.Get("iduser", 0)
        //                             ? Color.FromArgb("#D0E8FF")
        //                             : Color.FromArgb("#88e388")
        //                                                    };

        //                        messages.Add(message);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erreur lors de la récupération des messages : {ex.Message}");
        //    }

        //    return messages;
        //}
        public async static Task<List<MessageModel>> GetMessagesAsync(int piece, string pieceType, int receiverId, int senderId)
        {
            List<MessageModel> messages = new List<MessageModel>();

            const string sqlCmd = @"
            SELECT m.*, mr.is_read, mr.receiver
            FROM atooerp_messages m
            LEFT JOIN atooerp_messages_receiver mr ON m.id = mr.message
            WHERE m.piece = @piece 
            AND m.piece_type = @pieceType
            AND (
                (m.sender = @senderId AND mr.receiver = @receiverId)  -- Messages envoyés par sender à receiver
                OR
                (m.sender = @receiverId AND (  -- Messages envoyés par receiver à sender
                    mr.receiver = @senderId
                    OR
                    m.id IN (
                        SELECT message 
                        FROM atooerp_messages_receiver 
                        WHERE receiver = @senderId
                    )
                ))
            )
            ORDER BY m.create_date ASC;";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@piece", piece);
                        cmd.Parameters.AddWithValue("@pieceType", pieceType);
                        cmd.Parameters.AddWithValue("@senderId", senderId);
                        cmd.Parameters.AddWithValue("@receiverId", receiverId);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var message = new MessageModel
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Text = reader["text"] != DBNull.Value ? reader["text"].ToString() : string.Empty,
                                    Sender = reader["sender"] != DBNull.Value ? Convert.ToInt32(reader["sender"]) : 0,
                                    IsRead = reader["is_read"] != DBNull.Value && Convert.ToBoolean(reader["is_read"]),
                                    Alignment = reader["sender"] != DBNull.Value && Convert.ToInt32(reader["sender"]) == senderId
                                        ? LayoutOptions.End
                                        : LayoutOptions.Start,
                                    BackgroundColor = reader["sender"] != DBNull.Value && Convert.ToInt32(reader["sender"]) == senderId
                                        ? Color.FromArgb("#D0E8FF")
                                        : Color.FromArgb("#88e388")
                                };

                                messages.Add(message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des messages : {ex.Message}");
            }

            return messages;
        }
        public async static Task<List<MessageModel>> GetAllMessagesAsync(int receiverId, int senderId)
        {
            List<MessageModel> messages = new List<MessageModel>();

            const string sqlCmd = @"
    SELECT 
        m.*, 
        mr.is_read, 
        mr.receiver,
        m.piece_type,
        m.piece
    FROM atooerp_messages m
    LEFT JOIN atooerp_messages_receiver mr ON m.id = mr.message
    WHERE (
        (m.sender = @senderId AND mr.receiver = @receiverId)  -- Messages envoyés par sender à receiver
        OR
        (m.sender = @receiverId AND mr.receiver = @senderId)  -- Messages envoyés par receiver à sender
    )
    ORDER BY m.create_date ASC;";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@senderId", senderId);
                        cmd.Parameters.AddWithValue("@receiverId", receiverId);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var message = new MessageModel
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Text = reader["text"] != DBNull.Value ? reader["text"].ToString() : string.Empty,
                                    Sender = reader["sender"] != DBNull.Value ? Convert.ToInt32(reader["sender"]) : 0,
                                    IsRead = reader["is_read"] != DBNull.Value && Convert.ToBoolean(reader["is_read"]),
                                    Alignment = reader["sender"] != DBNull.Value && Convert.ToInt32(reader["sender"]) == senderId
                                        ? LayoutOptions.End
                                        : LayoutOptions.Start,
                                    BackgroundColor = reader["sender"] != DBNull.Value && Convert.ToInt32(reader["sender"]) == senderId
                                        ? Color.FromArgb("#D0E8FF")
                                        : Color.FromArgb("#88e388"),
                                    // Ajout de la propriété PieceType
                                    PieceType = reader["piece_type"] != DBNull.Value ? reader["piece_type"].ToString() : null,
                                    Piece = reader["piece"] != DBNull.Value ? Convert.ToInt32(reader["piece"]) : (int?)null
                                };

                                messages.Add(message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des messages : {ex.Message}");
            }

            return messages;
        }
        public static async Task<bool> UpdateMessageAsReadAsync(int messageId, int? receiverId = null)
        {
            string updateReceiverSql = @"
        UPDATE atooerp_messages_receiver
        SET is_read = 1, read_date = @readDate
        WHERE message = @messageId AND is_read = 0";

            if (receiverId.HasValue)
            {
                updateReceiverSql += " AND receiver = @receiverId";
            }

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(updateReceiverSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@messageId", messageId);
                        cmd.Parameters.AddWithValue("@readDate", DateTime.Now);

                        if (receiverId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@receiverId", receiverId.Value);
                        }

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du message : {ex.Message}");
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Méthode pour notifier les changements de propriétés
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ActivityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        // ✅ Constructeur par défaut
        public ActivityType() { }

        // ✅ Constructeur avec paramètres
        public ActivityType(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public ActivityType(int id, string name,string summary)
        {
            Id = id;
            Name = name;
            Summary = summary;
        }

        public static async Task<List<ActivityType>> GetActivityTypes()
        {
            const string sql = "SELECT id, name, summary FROM atooerp_activity_type ORDER BY name";
            var types = new List<ActivityType>();

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                await connection.OpenAsync();
                using (var cmd = new MySqlCommand(sql, connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        types.Add(new ActivityType
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Summary = reader.IsDBNull("summary") ? string.Empty : reader.GetString("summary")
                        });
                    }
                }
            }

            return types;
        }
    }
    public class Activity : INotifyPropertyChanged

    {

        public class ActivityState
        {
            public int Id { get; set; } // Valeur numérique (1, 2, 3)
            public string Name { get; set; } // Libellé (In Progress, Done, Cancelled)

            public ActivityState(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public ActivityState()
            {

            }

            // Liste statique des états disponibles
            public static List<ActivityState> States => new List<ActivityState>
                {
                    new ActivityState(1, "In Progress"),
                    new ActivityState(2, "Done"),
                    new ActivityState(3, "Cancelled")
                };
            public static List<ActivityState> selectStates => new List<ActivityState>
                {
                    new ActivityState(4, "All"),
                    new ActivityState(1, "In Progress"),
                    new ActivityState(2, "Done"),
                    new ActivityState(3, "Cancelled")
                };


            public static async Task<List<ActivityState>> GetActivityStates()
            {
                List<ActivityState> activityStates = new List<ActivityState>();

                const string sqlCmd = "SELECT id, name FROM atooerp_activity_state";

                DbConnection.Deconnecter();
                if (DbConnection.Connecter())
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                        {
                            using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (reader.Read())
                                {
                                    activityStates.Add(new ActivityState
                                    {
                                        Id = reader.GetInt32("id"),
                                        Name = reader.GetString("name")
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de la récupération des états d'activité : {ex.Message}");
                    }
                    finally
                    {
                        DbConnection.Deconnecter();
                    }
                }
                return activityStates;
            }
        }
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public int Type { get; set; }
        public string Type_all { get; set; }  

        public string Summary { get; set; }
        public DateTime DueDate { get; set; }
        private DateTime? _doneDate;
        public DateTime? DoneDate
        {
            get => _doneDate;
            set
            {
                if (_doneDate != value)
                {
                    _doneDate = value;
                    OnPropertyChanged(nameof(DoneDate)); // Notifier le changement de DoneDate
                }
            }
        }
        public string Gps { get; set; }

        public int AssignedEmployee { get; set; }
        public int Author { get; set; }
        public string Memo { get; set; }
        private readonly CustomNavigationDrawerViewModel _modelView;
        public Activity(CustomNavigationDrawerViewModel modelView)
        {
            _modelView = modelView;
        }
        public event EventHandler<bool> DoneFormVisibilityChanged;

        private int _state;
        public int State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State)); // Notifier le changement de State
                    OnPropertyChanged(nameof(StateColor)); // Notifier le changement de StateColor

                    UpdateStateName(); // Mettre à jour le nom de l'état

                    // Mettre à jour SelectedState en fonction du nouveau State
                    SelectedState = States.FirstOrDefault(s => s.Id == value) ?? States.First();

                    // Si l'état est "Done", déclencher l'événement pour ouvrir le formulaire
                    if (value == 2)
                    {
                        ActivityStateChanged?.Invoke(this, this);
                    }
                    else
                    {
                        // Pour les autres états, mettre à jour la base de données
                        _ = UpdateStateInDatabaseAsync();
                    }
                }
            }
        }
        private string _stateName;
        public string StateName
        {
            get => _stateName;
            set
            {
                if (_stateName != value)
                {
                    _stateName = value;
                    OnPropertyChanged(nameof(StateName)); // Notifier le changement de StateName
                }
            }
        }
        private async void UpdateStateName()
        {
            StateName = await GetStateNameFromDatabase(State);
        }
        public int? Parent { get; set; }
        public string ObjectType { get; set; }
        public int? Object { get; set; }
        public DateTime Date { get; set; }
        public string Form { get; set; }


        private string _assignedEmployeeName;
        public string AssignedEmployeeName
        {
            get => _assignedEmployeeName;
            set
            {
                _assignedEmployeeName = value;
                OnPropertyChanged(nameof(AssignedEmployeeName));
            }
        }
        public static List<ActivityState> States => ActivityState.States;
        public static List<ActivityState> selectStates => ActivityState.selectStates;


        private ActivityState _selectedState;
        //public ActivityState SelectedState
        //{
        //    get => _selectedState;
        //    set
        //    {
        //        if (_selectedState != value)
        //        {
        //            _selectedState = value;
        //            OnPropertyChanged(nameof(SelectedState));

        //            // Si l'état est "Done" (id=2), on ne met pas à jour immédiatement
        //            if (value?.Id == 2)
        //            {
        //                // Déclencher l'événement pour ouvrir le formulaire
        //                ActivityStateChanged?.Invoke(this, this);
        //            }
        //            else
        //            {
        //                // Pour les autres états, mettre à jour immédiatement
        //                State = value?.Id ?? 0;
        //                _ = UpdateStateInDatabaseAsync();
        //            }
        //        }
        //    }
        //}
        public ActivityState SelectedState
        {
            get => _selectedState;
            set
            {
                if (_selectedState != value)
                {
                    _selectedState = value;
                    OnPropertyChanged(nameof(SelectedState));

                    // Mettre à jour la propriété State avec la valeur Id de l'état sélectionné
                    State = value?.Id ?? 0;

                    // Déclencher l'événement seulement si l'état change vers "Done"
                    if (value?.Id == 2) // État "Done"
                    {
                        ActivityStateChanged?.Invoke(this, this);
                    }

                    // Mettre à jour la base de données
                    _ = UpdateStateInDatabaseAsync();
                }
            }
        }

        public Color StateColor
        {
            get
            {
                switch (State)
                {
                    case 1: return Color.FromArgb("#f59042"); // Couleur orange pour l'état 1
                    case 2: return Color.FromArgb("#31f55f"); // Couleur verte pour l'état 2
                    case 3: return Color.FromArgb("#f03824"); // Couleur rouge pour l'état 3
                    default: return Color.FromArgb("#F0F0F0"); ; // Couleur par défaut
                }
            }
        }

        // Propriété simple pour la couleur de la date d'échéance
        public Color DueDateColor
        {
            get
            {
                // Seulement pour les activités "In Progress"
                if (State != 1) return Color.FromArgb("#959aa0");

                var today = DateTime.Today;
                var dueDate = DueDate.Date;

                if (dueDate < today)
                    return Color.FromArgb("#FF6F61"); // Rouge pour en retard
                else if (dueDate == today)
                    return Color.FromArgb("#ffe000"); // Jaune pour aujourd'hui
                else
                    return Color.FromArgb("#4CAF50"); // Vert pour future
            }
        }

        public string Icon { get; private set; }
        public string Employee { get; private set; }
        public string PieceAcronym { get; private set; }
        public string PieceCode { get; private set; }
        public string ProductName { get; private set; }
        // public string PartnerName { get; private set; }
        private string _partnerName;
        public string PartnerName
        {
            get => _partnerName;
            set
            {
                _partnerName = value;
                OnPropertyChanged(nameof(PartnerName));
                OnPropertyChanged(nameof(ShowPartnerInfo)); // Notifier le changement de visibilité
            }
        }

        public bool ShowPartnerInfo => !string.IsNullOrEmpty(PartnerName);

        public bool ShowGpsInfo => !string.IsNullOrEmpty(Gps);

        public bool IsGpsVisible => !string.IsNullOrEmpty(Gps);

        public string State_name { get; private set; }
        public string Author_name { get; private set; }
        public string Parent_name { get; private set; }
        public string PieceDisplay => $"{PieceAcronym}/{PieceCode}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Activity()
        {

            // Initialiser SelectedState en fonction de State
            SelectedState = States.FirstOrDefault(s => s.Id == State) ?? States.First();
            OnPropertyChanged(nameof(SelectedState));

        }


        public static async Task<bool> UpdateAllActivity(Activity activity)
        {
            const string sqlCmd = @"UPDATE atooerp_activity 
                          SET summary = @Summary, 
                              memo = @Memo
                          WHERE id = @Id;";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@Summary", activity.Summary ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Memo", activity.Memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id", activity.Id);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la mise à jour de l'activité : {ex.Message}");
                    return false;
                }
            }
        }

        public static async Task<bool> UpdateActivity(Activity activity)
        {
            const string sqlCmd = @"UPDATE atooerp_activity 
                          SET summary = @Summary, 
                              memo = @Memo,
                              assigned_employee = @AssignedEmployee
                          WHERE id = @Id;";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@Summary", activity.Summary ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Memo", activity.Memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssignedEmployee", activity.AssignedEmployee);
                        cmd.Parameters.AddWithValue("@Id", activity.Id);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la mise à jour de l'activité : {ex.Message}");
                    return false;
                }
            }
        }

        public async static Task<bool> DeleteActivityFromDatabase(int activityId)
        {
            const string sqlCmd = "DELETE FROM atooerp_activity WHERE id = @ActivityId;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@ActivityId", activityId);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la suppression de l'activité : {ex.Message}");
                    DbConnection.Deconnecter();
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
                return false;
            }
        }

        public static event EventHandler<Activity> ActivityStateChanged;

        public async Task<bool> UpdateStateInDatabaseAsync()
        {
            string sqlCmd;
            DateTime? doneDateParam = null;

            if (this.State == 2 && this.DoneDate == null)
            {
                this.DoneDate = DateTime.Now;
                doneDateParam = this.DoneDate;

                sqlCmd = @"UPDATE atooerp_activity 
                  SET state = @State, done_date = @DoneDate, summary = @Summary, memo = @Memo
                  WHERE id = @Id;";
            }
            else
            {
                sqlCmd = @"UPDATE atooerp_activity 
                  SET state = @State, summary = @Summary, memo = @Memo
                  WHERE id = @Id;";
            }

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@State", this.State);
                        cmd.Parameters.AddWithValue("@Id", this.Id);
                        cmd.Parameters.AddWithValue("@Summary", this.Summary);
                        cmd.Parameters.AddWithValue("@Memo", this.Memo);

                        if (this.State == 2 && doneDateParam != null)
                        {
                            cmd.Parameters.AddWithValue("@DoneDate", doneDateParam);
                        }

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            // L'événement ActivityStateChanged est maintenant géré dans la propriété SelectedState
                            // ActivityStateChanged?.Invoke(this, this);
                        }
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la mise à jour de l'état de l'activité : {ex.Message}");
                    return false;
                }
            }
        }
        private async Task<string> GetStateNameFromDatabase(int stateId)
        {
            const string query = "SELECT name FROM atooerp_activity_state WHERE id = @StateId;";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StateId", stateId);

                        var result = await cmd.ExecuteScalarAsync();
                        return result?.ToString() ?? "Unknown State"; // Retourne le nom ou une valeur par défaut
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération du nom de l'état : {ex.Message}");
                    return "Unknown State";
                }
            }
        }
        public static async Task<int> GetEmployeeIdByUserId(int userId)
        {
            const string sqlQuery = "SELECT Id FROM hr_employe WHERE user = @UserId;";
            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result); // Retourne l'Id de l'employé
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération de l'Id de l'employé : {ex.Message}");
                }
                finally
                {
                    DbConnection.Deconnecter();
                }
            }
            return 0; // Retourne 0 si l'employé n'est pas trouvé
        }

        //    public static async Task<bool> SaveactivityToDatabase(Activity activity)
        //    {
        //        if (activity == null)
        //            throw new ArgumentNullException(nameof(activity));

        //        if (string.IsNullOrWhiteSpace(activity.Summary))
        //            throw new InvalidOperationException("Le résumé de l'activité ne peut pas être vide.");

        //        const string sqlCmd = @"
        //INSERT INTO atooerp_activity 
        //(create_date, type, summary, due_date, done_date, assigned_employee, author, memo, state, parent, object_type, object, date, form) 
        //VALUES 
        //(@CreateDate, @Type, @Summary, @DueDate, @DoneDate, @AssignedEmployee, @Author, @Memo, @State, @Parent, @ObjectType, @Object, @Date, @Form);";

        //        using (var connection = new MySqlConnection(DbConnection.ConnectionString))
        //        {
        //            try
        //            {
        //                await connection.OpenAsync();

        //                using (var cmd = new MySqlCommand(sqlCmd, connection))
        //                {
        //                    cmd.Parameters.AddWithValue("@CreateDate", activity.CreateDate);
        //                    cmd.Parameters.AddWithValue("@Type", activity.Type);
        //                    cmd.Parameters.AddWithValue("@Summary", activity.Summary ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@DueDate", activity.DueDate);
        //                    cmd.Parameters.AddWithValue("@DoneDate", activity.DoneDate ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@AssignedEmployee", activity.AssignedEmployee);
        //                    cmd.Parameters.AddWithValue("@Author", activity.Author);
        //                    cmd.Parameters.AddWithValue("@Memo", activity.Memo ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@State", activity.State);
        //                    cmd.Parameters.AddWithValue("@Parent", activity.Parent ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@ObjectType", activity.ObjectType);
        //                    cmd.Parameters.AddWithValue("@Object", activity.Object);
        //                    cmd.Parameters.AddWithValue("@Date", activity.Date);
        //                    cmd.Parameters.AddWithValue("@Form", activity.Form);

        //                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
        //                    return rowsAffected > 0;
        //                }
        //            }
        //            catch (MySqlException ex)
        //            {
        //                Console.WriteLine($"❌ Erreur MySQL : {ex.Message}");
        //                return false;
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"❌ Erreur inattendue : {ex.Message}");
        //                return false;
        //            }
        //        }
        //    }
        public static async Task<bool> SaveactivityToDatabase(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            if (string.IsNullOrWhiteSpace(activity.Summary))
                throw new InvalidOperationException("Le résumé de l'activité ne peut pas être vide.");

            const string sqlCmd = @"
INSERT INTO atooerp_activity 
(create_date, type, summary, due_date, done_date, assigned_employee, author, memo, state, parent, object_type, object, date, form, gps) 
VALUES 
(@CreateDate, @Type, @Summary, @DueDate, @DoneDate, @AssignedEmployee, @Author, @Memo, @State, @Parent, @ObjectType, @Object, @Date, @Form, @Gps);";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@CreateDate", activity.CreateDate);
                        cmd.Parameters.AddWithValue("@Type", activity.Type);
                        cmd.Parameters.AddWithValue("@Summary", activity.Summary ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DueDate", activity.DueDate);
                        cmd.Parameters.AddWithValue("@DoneDate", activity.DoneDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssignedEmployee", activity.AssignedEmployee);
                        cmd.Parameters.AddWithValue("@Author", activity.Author);
                        cmd.Parameters.AddWithValue("@Memo", activity.Memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@State", activity.State);
                        cmd.Parameters.AddWithValue("@Parent", activity.Parent ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ObjectType", activity.ObjectType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Object", activity.Object ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", activity.Date);
                        cmd.Parameters.AddWithValue("@Form", activity.Form ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gps", activity.Gps ?? (object)DBNull.Value);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"❌ Erreur MySQL : {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur inattendue : {ex.Message}");
                    return false;
                }
            }
        }
        public static async Task<bool> SaveAllactivityToDatabase(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            if (string.IsNullOrWhiteSpace(activity.Summary))
                throw new InvalidOperationException("Le résumé de l'activité ne peut pas être vide.");

            const string sqlCmd = @"
INSERT INTO atooerp_activity 
(create_date, type, summary, due_date, done_date, assigned_employee, author, memo, state, parent, object_type, object, date, form, gps) 
VALUES 
(@CreateDate, @Type, @Summary, @DueDate, @DoneDate, @AssignedEmployee, @Author, @Memo, @State, @Parent, @ObjectType, @Object, @Date, @Form, @Gps)";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@CreateDate", activity.CreateDate);
                        cmd.Parameters.AddWithValue("@Type", activity.Type);
                        cmd.Parameters.AddWithValue("@Summary", activity.Summary ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DueDate", activity.DueDate);
                        cmd.Parameters.AddWithValue("@DoneDate", activity.DoneDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssignedEmployee", activity.AssignedEmployee);
                        cmd.Parameters.AddWithValue("@Author", activity.Author);
                        cmd.Parameters.AddWithValue("@Memo", activity.Memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@State", activity.State);
                        cmd.Parameters.AddWithValue("@Parent", activity.Parent ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ObjectType", activity.ObjectType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Object", activity.Object ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", activity.Date);
                        cmd.Parameters.AddWithValue("@Form", activity.Form ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gps", activity.Gps ?? (object)DBNull.Value);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"❌ Erreur MySQL : {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur inattendue : {ex.Message}");
                    return false;
                }
            }
        }
        public static async Task<List<EmployeeDto>> GetEmployeesWithFullNamesAsync()
        {
            const string sqlQuery = @"
        SELECT 
            hr_employe.*,
            CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS name_employee
        FROM 
            hr_employe 
        INNER JOIN 
            atooerp_person ON hr_employe.Id = atooerp_person.Id;";

            var employees = new List<EmployeeDto>();

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new MySqlCommand(sqlQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var employee = new EmployeeDto
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    NameEmployee = reader["name_employee"] != DBNull.Value ? reader["name_employee"].ToString() : string.Empty
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des employés : {ex.Message}");
                }
            }

            return employees;
        }

        public class EmployeeDto
        {
            public int Id { get; set; }
            public string NameEmployee { get; set; }
        }

        private static async Task<string> GetPieceTypeFromCommercialDialing(string objectType)
        {
            const string query = "SELECT piece_type FROM commercial_dialing WHERE name = @Name;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@Name", objectType);

                        // Exécuter la requête et récupérer le résultat
                        var result = await cmd.ExecuteScalarAsync();
                        DbConnection.Deconnecter();

                        return result?.ToString(); // Retourne le piece_type ou null si non trouvé
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération du piece_type : {ex.Message}");
                    DbConnection.Deconnecter();
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
                return null;
            }
        }
        public async static Task<List<Activity>> GetAllActivities(int userId)
        {
            List<Activity> activities = new List<Activity>();
            const string sqlCmd = @"
        SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.form AS Form,
          atooerp_activity.memo AS Memo,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          -- New Columns: Piece Acronym (Dynamic Logic)
          CASE
          -- Purchase
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'

          -- Sale
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
          WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
          WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
          WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'

          -- POS
          WHEN atooerp_activity.object_type LIKE 'POS.Order%' THEN 'POSO'
          WHEN atooerp_activity.object_type LIKE 'POS.Credit_order%' THEN 'POSC'

          -- Commercial
          WHEN atooerp_activity.object_type LIKE 'Commercial.Payment%' THEN 'CP'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_out%' THEN 'CSO'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_entry%' THEN 'CSE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%' THEN 'CSM'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Need_expression%' THEN 'CNE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Product%' THEN 'PROD'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN 'PAR'
  
          -- CRM
          WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'CRMO'

          ELSE NULL
        END AS piece_acronym,
          -- New Column: Piece Code (from joined table)
          COALESCE(
          -- Purchase Tables
          purchase_quotation.code,
          purchase_order.code,
          purchase_shipping.code,
          purchase_invoice.code,
          purchase_credit_invoice.code,
          purchase_quotation_request.code,
          purchase_shipping_return.code,

          -- Sale Tables
          sale_quotation.code,
          sale_order.code,
          sale_shipping.code,
          sale_invoice.code,
          sale_credit_invoice.code,
          sale_shipping_return.code,

          -- POS Tables
          pos_order.code,
          pos_credit_order.code,

          -- Commercial Tables
          commercial_payment.code,
          commercial_stock_out.code,
          commercial_stock_entry.code,
          commercial_stock_mouvement.code,
          commercial_need_expression.code,
  
          -- Commercial Partner (Added for partner case)
          CASE WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN commercial_partner_direct.name ELSE NULL END,

          -- CRM Table
          crm_opportunity.code
        ) AS piece_code,
          -- New Column: Partner Name (via commercial_partner)
          commercial_partner.name AS partner_name,
          -- Product Name (only populated for Commercial.Product%)
          commercial_product.name AS product_name
        FROM
          atooerp_activity
          -- Existing Joins (unchanged)
          LEFT OUTER JOIN hr_employe ON atooerp_activity.author = hr_employe.Id
          LEFT OUTER JOIN hr_employe hr_employe_1 ON atooerp_activity.assigned_employee = hr_employe_1.Id
          LEFT OUTER JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
          LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
          LEFT OUTER JOIN atooerp_person ON hr_employe.Id = atooerp_person.Id
          LEFT OUTER JOIN atooerp_person atooerp_person_1 ON hr_employe_1.Id = atooerp_person_1.Id
          LEFT OUTER JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
          -- Purchase Tables
        LEFT OUTER JOIN purchase_quotation 
          ON atooerp_activity.object = purchase_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation%'
        LEFT OUTER JOIN purchase_order 
          ON atooerp_activity.object = purchase_order.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Order%'
        LEFT OUTER JOIN purchase_shipping 
          ON atooerp_activity.object = purchase_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping%'
        LEFT OUTER JOIN purchase_invoice 
          ON atooerp_activity.object = purchase_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Invoice%'
        LEFT OUTER JOIN purchase_credit_invoice 
          ON atooerp_activity.object = purchase_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%'
        LEFT OUTER JOIN purchase_quotation_request 
          ON atooerp_activity.object = purchase_quotation_request.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation_request%'
        LEFT OUTER JOIN purchase_shipping_return 
          ON atooerp_activity.object = purchase_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping_return%'

        -- Sale Tables
        LEFT OUTER JOIN sale_quotation 
          ON atooerp_activity.object = sale_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Quotation%'
        LEFT OUTER JOIN sale_order 
          ON atooerp_activity.object = sale_order.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Order%'
        LEFT OUTER JOIN sale_shipping 
          ON atooerp_activity.object = sale_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping%'
        LEFT OUTER JOIN sale_invoice 
          ON atooerp_activity.object = sale_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Invoice%'
        LEFT OUTER JOIN sale_credit_invoice 
          ON atooerp_activity.object = sale_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Credit_invoice%'
        LEFT OUTER JOIN sale_shipping_return 
          ON atooerp_activity.object = sale_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping_return%'

        -- POS Tables
        LEFT OUTER JOIN pos_order 
          ON atooerp_activity.object = pos_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Order%'
        LEFT OUTER JOIN pos_credit_order 
          ON atooerp_activity.object = pos_credit_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Credit_order%'

        -- Commercial Tables
        LEFT OUTER JOIN commercial_payment 
          ON atooerp_activity.object = commercial_payment.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Payment%'
        LEFT OUTER JOIN commercial_stock_out 
          ON atooerp_activity.object = commercial_stock_out.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_out%'
        LEFT OUTER JOIN commercial_stock_entry 
          ON atooerp_activity.object = commercial_stock_entry.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_entry%'
        LEFT OUTER JOIN commercial_stock_mouvement 
          ON atooerp_activity.object = commercial_stock_mouvement.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%'
        LEFT OUTER JOIN commercial_need_expression 
          ON atooerp_activity.object = commercial_need_expression.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Need_expression%'

        -- New join for Commercial.Partner
        LEFT OUTER JOIN commercial_partner AS commercial_partner_direct
          ON atooerp_activity.object = commercial_partner_direct.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Partner%'

        -- CRM Table
        LEFT OUTER JOIN crm_opportunity
          ON atooerp_activity.object = crm_opportunity.Id 
          AND atooerp_activity.object_type LIKE 'CRM.Opportunity%'
         -- New Join for Commercial.Product
          LEFT OUTER JOIN commercial_product 
            ON atooerp_activity.object = commercial_product.Id 
            AND atooerp_activity.object_type LIKE 'Commercial.Product%'
          -- Add joins for ALL other object_type cases here
          -- Join commercial_partner (for partner name)
          LEFT OUTER JOIN commercial_partner
          ON COALESCE(
            -- Purchase Tables
            purchase_quotation.partner,
            purchase_order.partner,
            purchase_shipping.partner,
            purchase_invoice.partner,
            purchase_credit_invoice.partner,
            purchase_shipping_return.partner,

            -- Sale Tables
            sale_quotation.partner,
            sale_order.partner,
            sale_shipping.partner,
            sale_invoice.partner,
            sale_credit_invoice.partner,
            sale_shipping_return.partner,

            -- POS Tables
            pos_order.partner,
            pos_credit_order.partner,

            -- Commercial Tables
            commercial_payment.partner,

            -- CRM Table
            crm_opportunity.partner
          ) = commercial_partner.Id
        WHERE hr_employe_1.user = @userId
        ORDER BY (atooerp_activity.state = 1) DESC, atooerp_activity.due_date ASC;";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    // State_name = reader["State_name"] != DBNull.Value ? reader["State_name"].ToString() : string.Empty,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 0,

                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Gps = reader["Gps"] != DBNull.Value ? reader["Gps"].ToString() : string.Empty,
                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync(); // Fermer la connexion de manière asynchrone
                }
            }

            // Trier les activités par state (1, 2, 3) puis par due_date (du plus proche au plus éloigné)
            var sortedActivities = activities
                .OrderBy(a => a.State) // Trier par state
                .ThenBy(a => a.DueDate) // Ensuite trier par due_date
                .ToList();

            return sortedActivities;
        }
        public async static Task<List<Activity>> GetAllActivitiesForDrawerControl(int entityId, string entityType)
        {
            List<Activity> activities = new List<Activity>();

            //const string sqlCmd = @"
            //    SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) as assigned_employee_name
            //    FROM atooerp_activity a
            //    LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            //    LEFT JOIN atooerp_person p ON e.Id = p.Id
            //    WHERE a.object = @entityId AND a.object_type = @entityType 
            //    ORDER BY a.create_date DESC;";
            const string sqlCmd = @"
            SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) AS assigned_employee_name
            FROM atooerp_activity a
            LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            LEFT JOIN atooerp_person p ON e.Id = p.Id
            WHERE a.object = @entityId AND a.object_type = @entityType
            ORDER BY 
            FIELD(a.state, 1, 2, 3), 
            a.create_date DESC;  ;";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        // Ajouter les paramètres pour éviter les injections SQL
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Type = reader["type"] != DBNull.Value ? Convert.ToInt32(reader["type"]) : 0,
                                    Summary = reader["summary"] != DBNull.Value ? reader["summary"].ToString() : string.Empty,
                                    DueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : DateTime.MinValue,
                                    DoneDate = reader["done_date"] != DBNull.Value ? Convert.ToDateTime(reader["done_date"]) : (DateTime?)null,
                                    AssignedEmployee = reader["assigned_employee"] != DBNull.Value ? Convert.ToInt32(reader["assigned_employee"]) : 0,
                                    Author = reader["author"] != DBNull.Value ? Convert.ToInt32(reader["author"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    State = reader["state"] != DBNull.Value ? Convert.ToInt32(reader["state"]) : 0,
                                    Parent = reader["parent"] != DBNull.Value ? Convert.ToInt32(reader["parent"]) : (int?)null,
                                    ObjectType = reader["object_type"] != DBNull.Value ? reader["object_type"].ToString() : string.Empty,
                                    Object = reader["object"] != DBNull.Value ? Convert.ToInt32(reader["object"]) : 0,
                                    Date = reader["date"] != DBNull.Value ? Convert.ToDateTime(reader["date"]) : DateTime.MinValue,
                                    Form = reader["form"] != DBNull.Value ? reader["form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    AssignedEmployeeName = reader["assigned_employee_name"] != DBNull.Value ? reader["assigned_employee_name"].ToString() : "Non assigné"
                                };

                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync(); // Fermer la connexion de manière asynchrone
                }
            }

            // Trier les activités par state (1, 2, 3) puis par due_date (du plus proche au plus éloigné)
            var sortedActivities = activities
                .OrderBy(a => a.State) // Trier par state
                .ThenBy(a => a.DueDate) // Ensuite trier par due_date
                .ToList();

            return sortedActivities;
        }
        public async static Task<List<Activity>> GetInProgressActivities(int entityId, string entityType)
        {
            List<Activity> activities = new List<Activity>();

            const string sqlCmd = @"
            SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) AS assigned_employee_name
            FROM atooerp_activity a
            LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            LEFT JOIN atooerp_person p ON e.Id = p.Id
            WHERE a.object = @entityId 
              AND a.object_type = @entityType
              AND a.state = 1  
            ORDER BY a.create_date DESC; ";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Type = reader["type"] != DBNull.Value ? Convert.ToInt32(reader["type"]) : 0,
                                    Summary = reader["summary"] != DBNull.Value ? reader["summary"].ToString() : string.Empty,
                                    DueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : DateTime.MinValue,
                                    DoneDate = reader["done_date"] != DBNull.Value ? Convert.ToDateTime(reader["done_date"]) : (DateTime?)null,
                                    AssignedEmployee = reader["assigned_employee"] != DBNull.Value ? Convert.ToInt32(reader["assigned_employee"]) : 0,
                                    Author = reader["author"] != DBNull.Value ? Convert.ToInt32(reader["author"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    State = reader["state"] != DBNull.Value ? Convert.ToInt32(reader["state"]) : 0,
                                    Parent = reader["parent"] != DBNull.Value ? Convert.ToInt32(reader["parent"]) : (int?)null,
                                    ObjectType = reader["object_type"] != DBNull.Value ? reader["object_type"].ToString() : string.Empty,
                                    Object = reader["object"] != DBNull.Value ? Convert.ToInt32(reader["object"]) : 0,
                                    Date = reader["date"] != DBNull.Value ? Convert.ToDateTime(reader["date"]) : DateTime.MinValue,
                                    Form = reader["form"] != DBNull.Value ? reader["form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    AssignedEmployeeName = reader["assigned_employee_name"] != DBNull.Value ? reader["assigned_employee_name"].ToString() : "Non assigné"
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités In Progress : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return activities.OrderBy(a => a.DueDate).ToList();
        }

        public async static Task<List<Activity>> GetInProgressActivitiesToday(int entityId, string entityType)
        {
            List<Activity> activities = new List<Activity>();

            const string sqlCmd = @"
            SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) AS assigned_employee_name
            FROM atooerp_activity a
            LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            LEFT JOIN atooerp_person p ON e.Id = p.Id
            WHERE a.object = @entityId 
              AND a.object_type = @entityType
              AND a.state = 1  
              AND DATE(a.due_date) = CURDATE()
            ORDER BY a.create_date DESC; ";

        using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Type = reader["type"] != DBNull.Value ? Convert.ToInt32(reader["type"]) : 0,
                                    Summary = reader["summary"] != DBNull.Value ? reader["summary"].ToString() : string.Empty,
                                    DueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : DateTime.MinValue,
                                    DoneDate = reader["done_date"] != DBNull.Value ? Convert.ToDateTime(reader["done_date"]) : (DateTime?)null,
                                    AssignedEmployee = reader["assigned_employee"] != DBNull.Value ? Convert.ToInt32(reader["assigned_employee"]) : 0,
                                    Author = reader["author"] != DBNull.Value ? Convert.ToInt32(reader["author"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    State = reader["state"] != DBNull.Value ? Convert.ToInt32(reader["state"]) : 0,
                                    Parent = reader["parent"] != DBNull.Value ? Convert.ToInt32(reader["parent"]) : (int?)null,
                                    ObjectType = reader["object_type"] != DBNull.Value ? reader["object_type"].ToString() : string.Empty,
                                    Object = reader["object"] != DBNull.Value ? Convert.ToInt32(reader["object"]) : 0,
                                    Date = reader["date"] != DBNull.Value ? Convert.ToDateTime(reader["date"]) : DateTime.MinValue,
                                    Form = reader["form"] != DBNull.Value ? reader["form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    AssignedEmployeeName = reader["assigned_employee_name"] != DBNull.Value ? reader["assigned_employee_name"].ToString() : "Non assigné"
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités In Progress : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetInProgressActivitiesOverdue(int entityId, string entityType)
        {
            List<Activity> activities = new List<Activity>();

            const string sqlCmd = @"
            SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) AS assigned_employee_name
            FROM atooerp_activity a
            LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            LEFT JOIN atooerp_person p ON e.Id = p.Id
            WHERE a.object = @entityId 
              AND a.object_type = @entityType
              AND a.state = 1  
              AND DATE(a.due_date) < CURDATE()
            ORDER BY a.create_date DESC; ";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Type = reader["type"] != DBNull.Value ? Convert.ToInt32(reader["type"]) : 0,
                                    Summary = reader["summary"] != DBNull.Value ? reader["summary"].ToString() : string.Empty,
                                    DueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : DateTime.MinValue,
                                    DoneDate = reader["done_date"] != DBNull.Value ? Convert.ToDateTime(reader["done_date"]) : (DateTime?)null,
                                    AssignedEmployee = reader["assigned_employee"] != DBNull.Value ? Convert.ToInt32(reader["assigned_employee"]) : 0,
                                    Author = reader["author"] != DBNull.Value ? Convert.ToInt32(reader["author"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    State = reader["state"] != DBNull.Value ? Convert.ToInt32(reader["state"]) : 0,
                                    Parent = reader["parent"] != DBNull.Value ? Convert.ToInt32(reader["parent"]) : (int?)null,
                                    ObjectType = reader["object_type"] != DBNull.Value ? reader["object_type"].ToString() : string.Empty,
                                    Object = reader["object"] != DBNull.Value ? Convert.ToInt32(reader["object"]) : 0,
                                    Date = reader["date"] != DBNull.Value ? Convert.ToDateTime(reader["date"]) : DateTime.MinValue,
                                    Form = reader["form"] != DBNull.Value ? reader["form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    AssignedEmployeeName = reader["assigned_employee_name"] != DBNull.Value ? reader["assigned_employee_name"].ToString() : "Non assigné"
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités In Progress : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetInProgressActivitiesFuture(int entityId, string entityType)
        {
            List<Activity> activities = new List<Activity>();

            const string sqlCmd = @"
            SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) AS assigned_employee_name
            FROM atooerp_activity a
            LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            LEFT JOIN atooerp_person p ON e.Id = p.Id
            WHERE a.object = @entityId 
              AND a.object_type = @entityType
              AND a.state = 1  
              AND DATE(a.due_date) > CURDATE()
            ORDER BY a.create_date DESC; ";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Type = reader["type"] != DBNull.Value ? Convert.ToInt32(reader["type"]) : 0,
                                    Summary = reader["summary"] != DBNull.Value ? reader["summary"].ToString() : string.Empty,
                                    DueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : DateTime.MinValue,
                                    DoneDate = reader["done_date"] != DBNull.Value ? Convert.ToDateTime(reader["done_date"]) : (DateTime?)null,
                                    AssignedEmployee = reader["assigned_employee"] != DBNull.Value ? Convert.ToInt32(reader["assigned_employee"]) : 0,
                                    Author = reader["author"] != DBNull.Value ? Convert.ToInt32(reader["author"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    State = reader["state"] != DBNull.Value ? Convert.ToInt32(reader["state"]) : 0,
                                    Parent = reader["parent"] != DBNull.Value ? Convert.ToInt32(reader["parent"]) : (int?)null,
                                    ObjectType = reader["object_type"] != DBNull.Value ? reader["object_type"].ToString() : string.Empty,
                                    Object = reader["object"] != DBNull.Value ? Convert.ToInt32(reader["object"]) : 0,
                                    Date = reader["date"] != DBNull.Value ? Convert.ToDateTime(reader["date"]) : DateTime.MinValue,
                                    Form = reader["form"] != DBNull.Value ? reader["form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    AssignedEmployeeName = reader["assigned_employee_name"] != DBNull.Value ? reader["assigned_employee_name"].ToString() : "Non assigné"
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités In Progress : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return activities.OrderBy(a => a.DueDate).ToList();
        }


        public async static Task<List<Activity>> GetDoneActivities(int entityId, string entityType)
        {
            List<Activity> activities = new List<Activity>();

            const string sqlCmd = @"
            SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) AS assigned_employee_name
            FROM atooerp_activity a
            LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            LEFT JOIN atooerp_person p ON e.Id = p.Id
            WHERE a.object = @entityId 
              AND a.object_type = @entityType
              AND a.state = 2  
            ORDER BY a.create_date DESC; ";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Type = reader["type"] != DBNull.Value ? Convert.ToInt32(reader["type"]) : 0,
                                    Summary = reader["summary"] != DBNull.Value ? reader["summary"].ToString() : string.Empty,
                                    DueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : DateTime.MinValue,
                                    DoneDate = reader["done_date"] != DBNull.Value ? Convert.ToDateTime(reader["done_date"]) : (DateTime?)null,
                                    AssignedEmployee = reader["assigned_employee"] != DBNull.Value ? Convert.ToInt32(reader["assigned_employee"]) : 0,
                                    Author = reader["author"] != DBNull.Value ? Convert.ToInt32(reader["author"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    State = reader["state"] != DBNull.Value ? Convert.ToInt32(reader["state"]) : 0,
                                    Parent = reader["parent"] != DBNull.Value ? Convert.ToInt32(reader["parent"]) : (int?)null,
                                    ObjectType = reader["object_type"] != DBNull.Value ? reader["object_type"].ToString() : string.Empty,
                                    Object = reader["object"] != DBNull.Value ? Convert.ToInt32(reader["object"]) : 0,
                                    Date = reader["date"] != DBNull.Value ? Convert.ToDateTime(reader["date"]) : DateTime.MinValue,
                                    Form = reader["form"] != DBNull.Value ? reader["form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    AssignedEmployeeName = reader["assigned_employee_name"] != DBNull.Value ? reader["assigned_employee_name"].ToString() : "Non assigné"
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités In Progress : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetCancelledActivities(int entityId, string entityType)
        {
            List<Activity> activities = new List<Activity>();

            const string sqlCmd = @"
            SELECT a.*, CONCAT(p.first_name, ' ', p.last_name) AS assigned_employee_name
            FROM atooerp_activity a
            LEFT JOIN hr_employe e ON a.assigned_employee = e.Id
            LEFT JOIN atooerp_person p ON e.Id = p.Id
            WHERE a.object = @entityId 
              AND a.object_type = @entityType
              AND a.state = 3  
            ORDER BY a.create_date DESC; ";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@entityId", entityId);
                        cmd.Parameters.AddWithValue("@entityType", entityType);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                                    CreateDate = reader["create_date"] != DBNull.Value ? Convert.ToDateTime(reader["create_date"]) : DateTime.MinValue,
                                    Type = reader["type"] != DBNull.Value ? Convert.ToInt32(reader["type"]) : 0,
                                    Summary = reader["summary"] != DBNull.Value ? reader["summary"].ToString() : string.Empty,
                                    DueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : DateTime.MinValue,
                                    DoneDate = reader["done_date"] != DBNull.Value ? Convert.ToDateTime(reader["done_date"]) : (DateTime?)null,
                                    AssignedEmployee = reader["assigned_employee"] != DBNull.Value ? Convert.ToInt32(reader["assigned_employee"]) : 0,
                                    Author = reader["author"] != DBNull.Value ? Convert.ToInt32(reader["author"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    State = reader["state"] != DBNull.Value ? Convert.ToInt32(reader["state"]) : 0,
                                    Parent = reader["parent"] != DBNull.Value ? Convert.ToInt32(reader["parent"]) : (int?)null,
                                    ObjectType = reader["object_type"] != DBNull.Value ? reader["object_type"].ToString() : string.Empty,
                                    Object = reader["object"] != DBNull.Value ? Convert.ToInt32(reader["object"]) : 0,
                                    Date = reader["date"] != DBNull.Value ? Convert.ToDateTime(reader["date"]) : DateTime.MinValue,
                                    Form = reader["form"] != DBNull.Value ? reader["form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    AssignedEmployeeName = reader["assigned_employee_name"] != DBNull.Value ? reader["assigned_employee_name"].ToString() : "Non assigné"
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités In Progress : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetCancelledAllActivities(int userId)
        {
            List<Activity> activities = new List<Activity>();
            const string sqlCmd = @"
        SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.form AS Form,
          atooerp_activity.gps AS Gps,
          atooerp_activity.memo AS Memo,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          -- New Columns: Piece Acronym (Dynamic Logic)
          CASE
          -- Purchase
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'

          -- Sale
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
          WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
          WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
          WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'

          -- POS
          WHEN atooerp_activity.object_type LIKE 'POS.Order%' THEN 'POSO'
          WHEN atooerp_activity.object_type LIKE 'POS.Credit_order%' THEN 'POSC'

          -- Commercial
          WHEN atooerp_activity.object_type LIKE 'Commercial.Payment%' THEN 'CP'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_out%' THEN 'CSO'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_entry%' THEN 'CSE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%' THEN 'CSM'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Need_expression%' THEN 'CNE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Product%' THEN 'PROD'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN 'PAR'
  
          -- CRM
          WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'CRMO'

          ELSE NULL
        END AS piece_acronym,
          -- New Column: Piece Code (from joined table)
          COALESCE(
          -- Purchase Tables
          purchase_quotation.code,
          purchase_order.code,
          purchase_shipping.code,
          purchase_invoice.code,
          purchase_credit_invoice.code,
          purchase_quotation_request.code,
          purchase_shipping_return.code,

          -- Sale Tables
          sale_quotation.code,
          sale_order.code,
          sale_shipping.code,
          sale_invoice.code,
          sale_credit_invoice.code,
          sale_shipping_return.code,

          -- POS Tables
          pos_order.code,
          pos_credit_order.code,

          -- Commercial Tables
          commercial_payment.code,
          commercial_stock_out.code,
          commercial_stock_entry.code,
          commercial_stock_mouvement.code,
          commercial_need_expression.code,
  
          -- Commercial Partner (Added for partner case)
          CASE WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN commercial_partner_direct.name ELSE NULL END,

          -- CRM Table
          crm_opportunity.code
        ) AS piece_code,
          -- New Column: Partner Name (via commercial_partner)
          commercial_partner.name AS partner_name,
          -- Product Name (only populated for Commercial.Product%)
          commercial_product.name AS product_name
        FROM
          atooerp_activity
          -- Existing Joins (unchanged)
          LEFT OUTER JOIN hr_employe ON atooerp_activity.author = hr_employe.Id
          LEFT OUTER JOIN hr_employe hr_employe_1 ON atooerp_activity.assigned_employee = hr_employe_1.Id
          LEFT OUTER JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
          LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
          LEFT OUTER JOIN atooerp_person ON hr_employe.Id = atooerp_person.Id
          LEFT OUTER JOIN atooerp_person atooerp_person_1 ON hr_employe_1.Id = atooerp_person_1.Id
          LEFT OUTER JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
          -- Purchase Tables
        LEFT OUTER JOIN purchase_quotation 
          ON atooerp_activity.object = purchase_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation%'
        LEFT OUTER JOIN purchase_order 
          ON atooerp_activity.object = purchase_order.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Order%'
        LEFT OUTER JOIN purchase_shipping 
          ON atooerp_activity.object = purchase_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping%'
        LEFT OUTER JOIN purchase_invoice 
          ON atooerp_activity.object = purchase_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Invoice%'
        LEFT OUTER JOIN purchase_credit_invoice 
          ON atooerp_activity.object = purchase_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%'
        LEFT OUTER JOIN purchase_quotation_request 
          ON atooerp_activity.object = purchase_quotation_request.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation_request%'
        LEFT OUTER JOIN purchase_shipping_return 
          ON atooerp_activity.object = purchase_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping_return%'

        -- Sale Tables
        LEFT OUTER JOIN sale_quotation 
          ON atooerp_activity.object = sale_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Quotation%'
        LEFT OUTER JOIN sale_order 
          ON atooerp_activity.object = sale_order.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Order%'
        LEFT OUTER JOIN sale_shipping 
          ON atooerp_activity.object = sale_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping%'
        LEFT OUTER JOIN sale_invoice 
          ON atooerp_activity.object = sale_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Invoice%'
        LEFT OUTER JOIN sale_credit_invoice 
          ON atooerp_activity.object = sale_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Credit_invoice%'
        LEFT OUTER JOIN sale_shipping_return 
          ON atooerp_activity.object = sale_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping_return%'

        -- POS Tables
        LEFT OUTER JOIN pos_order 
          ON atooerp_activity.object = pos_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Order%'
        LEFT OUTER JOIN pos_credit_order 
          ON atooerp_activity.object = pos_credit_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Credit_order%'

        -- Commercial Tables
        LEFT OUTER JOIN commercial_payment 
          ON atooerp_activity.object = commercial_payment.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Payment%'
        LEFT OUTER JOIN commercial_stock_out 
          ON atooerp_activity.object = commercial_stock_out.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_out%'
        LEFT OUTER JOIN commercial_stock_entry 
          ON atooerp_activity.object = commercial_stock_entry.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_entry%'
        LEFT OUTER JOIN commercial_stock_mouvement 
          ON atooerp_activity.object = commercial_stock_mouvement.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%'
        LEFT OUTER JOIN commercial_need_expression 
          ON atooerp_activity.object = commercial_need_expression.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Need_expression%'

        -- New join for Commercial.Partner
        LEFT OUTER JOIN commercial_partner AS commercial_partner_direct
          ON atooerp_activity.object = commercial_partner_direct.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Partner%'

        -- CRM Table
        LEFT OUTER JOIN crm_opportunity
          ON atooerp_activity.object = crm_opportunity.Id 
          AND atooerp_activity.object_type LIKE 'CRM.Opportunity%'
         -- New Join for Commercial.Product
          LEFT OUTER JOIN commercial_product 
            ON atooerp_activity.object = commercial_product.Id 
            AND atooerp_activity.object_type LIKE 'Commercial.Product%'
          -- Add joins for ALL other object_type cases here
          -- Join commercial_partner (for partner name)
          LEFT OUTER JOIN commercial_partner
          ON COALESCE(
            -- Purchase Tables
            purchase_quotation.partner,
            purchase_order.partner,
            purchase_shipping.partner,
            purchase_invoice.partner,
            purchase_credit_invoice.partner,
            purchase_shipping_return.partner,

            -- Sale Tables
            sale_quotation.partner,
            sale_order.partner,
            sale_shipping.partner,
            sale_invoice.partner,
            sale_credit_invoice.partner,
            sale_shipping_return.partner,

            -- POS Tables
            pos_order.partner,
            pos_credit_order.partner,

            -- Commercial Tables
            commercial_payment.partner,

            -- CRM Table
            crm_opportunity.partner
          ) = commercial_partner.Id
         WHERE hr_employe_1.user = @userId 
              AND atooerp_activity.state = 3  -- Filtre pour n'avoir que les activités annulées
            ORDER BY atooerp_activity.due_date ASC;";
            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 3, // Forcé à 3 pour cohérence
                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités annulées : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            // Trier par due_date (du plus proche au plus éloigné)
            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetDoneAllActivities(int userId)
        {
            List<Activity> activities = new List<Activity>();
            const string sqlCmd = @"
        SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.form AS Form,
          atooerp_activity.memo AS Memo,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          -- New Columns: Piece Acronym (Dynamic Logic)
          CASE
          -- Purchase
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'

          -- Sale
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
          WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
          WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
          WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'

          -- POS
          WHEN atooerp_activity.object_type LIKE 'POS.Order%' THEN 'POSO'
          WHEN atooerp_activity.object_type LIKE 'POS.Credit_order%' THEN 'POSC'

          -- Commercial
          WHEN atooerp_activity.object_type LIKE 'Commercial.Payment%' THEN 'CP'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_out%' THEN 'CSO'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_entry%' THEN 'CSE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%' THEN 'CSM'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Need_expression%' THEN 'CNE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Product%' THEN 'PROD'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN 'PAR'
  
          -- CRM
          WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'CRMO'

          ELSE NULL
        END AS piece_acronym,
          -- New Column: Piece Code (from joined table)
          COALESCE(
          -- Purchase Tables
          purchase_quotation.code,
          purchase_order.code,
          purchase_shipping.code,
          purchase_invoice.code,
          purchase_credit_invoice.code,
          purchase_quotation_request.code,
          purchase_shipping_return.code,

          -- Sale Tables
          sale_quotation.code,
          sale_order.code,
          sale_shipping.code,
          sale_invoice.code,
          sale_credit_invoice.code,
          sale_shipping_return.code,

          -- POS Tables
          pos_order.code,
          pos_credit_order.code,

          -- Commercial Tables
          commercial_payment.code,
          commercial_stock_out.code,
          commercial_stock_entry.code,
          commercial_stock_mouvement.code,
          commercial_need_expression.code,
  
          -- Commercial Partner (Added for partner case)
          CASE WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN commercial_partner_direct.name ELSE NULL END,

          -- CRM Table
          crm_opportunity.code
        ) AS piece_code,
          -- New Column: Partner Name (via commercial_partner)
          commercial_partner.name AS partner_name,
          -- Product Name (only populated for Commercial.Product%)
          commercial_product.name AS product_name
        FROM
          atooerp_activity
          -- Existing Joins (unchanged)
          LEFT OUTER JOIN hr_employe ON atooerp_activity.author = hr_employe.Id
          LEFT OUTER JOIN hr_employe hr_employe_1 ON atooerp_activity.assigned_employee = hr_employe_1.Id
          LEFT OUTER JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
          LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
          LEFT OUTER JOIN atooerp_person ON hr_employe.Id = atooerp_person.Id
          LEFT OUTER JOIN atooerp_person atooerp_person_1 ON hr_employe_1.Id = atooerp_person_1.Id
          LEFT OUTER JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
          -- Purchase Tables
        LEFT OUTER JOIN purchase_quotation 
          ON atooerp_activity.object = purchase_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation%'
        LEFT OUTER JOIN purchase_order 
          ON atooerp_activity.object = purchase_order.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Order%'
        LEFT OUTER JOIN purchase_shipping 
          ON atooerp_activity.object = purchase_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping%'
        LEFT OUTER JOIN purchase_invoice 
          ON atooerp_activity.object = purchase_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Invoice%'
        LEFT OUTER JOIN purchase_credit_invoice 
          ON atooerp_activity.object = purchase_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%'
        LEFT OUTER JOIN purchase_quotation_request 
          ON atooerp_activity.object = purchase_quotation_request.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation_request%'
        LEFT OUTER JOIN purchase_shipping_return 
          ON atooerp_activity.object = purchase_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping_return%'

        -- Sale Tables
        LEFT OUTER JOIN sale_quotation 
          ON atooerp_activity.object = sale_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Quotation%'
        LEFT OUTER JOIN sale_order 
          ON atooerp_activity.object = sale_order.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Order%'
        LEFT OUTER JOIN sale_shipping 
          ON atooerp_activity.object = sale_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping%'
        LEFT OUTER JOIN sale_invoice 
          ON atooerp_activity.object = sale_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Invoice%'
        LEFT OUTER JOIN sale_credit_invoice 
          ON atooerp_activity.object = sale_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Credit_invoice%'
        LEFT OUTER JOIN sale_shipping_return 
          ON atooerp_activity.object = sale_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping_return%'

        -- POS Tables
        LEFT OUTER JOIN pos_order 
          ON atooerp_activity.object = pos_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Order%'
        LEFT OUTER JOIN pos_credit_order 
          ON atooerp_activity.object = pos_credit_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Credit_order%'

        -- Commercial Tables
        LEFT OUTER JOIN commercial_payment 
          ON atooerp_activity.object = commercial_payment.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Payment%'
        LEFT OUTER JOIN commercial_stock_out 
          ON atooerp_activity.object = commercial_stock_out.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_out%'
        LEFT OUTER JOIN commercial_stock_entry 
          ON atooerp_activity.object = commercial_stock_entry.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_entry%'
        LEFT OUTER JOIN commercial_stock_mouvement 
          ON atooerp_activity.object = commercial_stock_mouvement.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%'
        LEFT OUTER JOIN commercial_need_expression 
          ON atooerp_activity.object = commercial_need_expression.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Need_expression%'

        -- New join for Commercial.Partner
        LEFT OUTER JOIN commercial_partner AS commercial_partner_direct
          ON atooerp_activity.object = commercial_partner_direct.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Partner%'

        -- CRM Table
        LEFT OUTER JOIN crm_opportunity
          ON atooerp_activity.object = crm_opportunity.Id 
          AND atooerp_activity.object_type LIKE 'CRM.Opportunity%'
         -- New Join for Commercial.Product
          LEFT OUTER JOIN commercial_product 
            ON atooerp_activity.object = commercial_product.Id 
            AND atooerp_activity.object_type LIKE 'Commercial.Product%'
          -- Add joins for ALL other object_type cases here
          -- Join commercial_partner (for partner name)
          LEFT OUTER JOIN commercial_partner
          ON COALESCE(
            -- Purchase Tables
            purchase_quotation.partner,
            purchase_order.partner,
            purchase_shipping.partner,
            purchase_invoice.partner,
            purchase_credit_invoice.partner,
            purchase_shipping_return.partner,

            -- Sale Tables
            sale_quotation.partner,
            sale_order.partner,
            sale_shipping.partner,
            sale_invoice.partner,
            sale_credit_invoice.partner,
            sale_shipping_return.partner,

            -- POS Tables
            pos_order.partner,
            pos_credit_order.partner,

            -- Commercial Tables
            commercial_payment.partner,

            -- CRM Table
            crm_opportunity.partner
          ) = commercial_partner.Id
         WHERE hr_employe_1.user = @userId 
              AND atooerp_activity.state = 2  -- Filtre pour n'avoir que les activités annulées
            ORDER BY atooerp_activity.due_date ASC;";
            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 2, // Forcé à 2 pour cohérence
                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités annulées : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            // Trier par due_date (du plus proche au plus éloigné)
            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetInProgressAllActivities(int userId)
        {
            List<Activity> activities = new List<Activity>();
            const string sqlCmd = @"
        SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.memo AS Memo,
          atooerp_activity.form AS Form,
          atooerp_activity.gps AS Gps,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          -- New Columns: Piece Acronym (Dynamic Logic)
          CASE
          -- Purchase
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'

          -- Sale
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
          WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
          WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
          WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'

          -- POS
          WHEN atooerp_activity.object_type LIKE 'POS.Order%' THEN 'POSO'
          WHEN atooerp_activity.object_type LIKE 'POS.Credit_order%' THEN 'POSC'

          -- Commercial
          WHEN atooerp_activity.object_type LIKE 'Commercial.Payment%' THEN 'CP'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_out%' THEN 'CSO'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_entry%' THEN 'CSE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%' THEN 'CSM'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Need_expression%' THEN 'CNE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Product%' THEN 'PROD'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN 'PAR'
  
          -- CRM
          WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'CRMO'

          ELSE NULL
        END AS piece_acronym,
          -- New Column: Piece Code (from joined table)
          COALESCE(
          -- Purchase Tables
          purchase_quotation.code,
          purchase_order.code,
          purchase_shipping.code,
          purchase_invoice.code,
          purchase_credit_invoice.code,
          purchase_quotation_request.code,
          purchase_shipping_return.code,

          -- Sale Tables
          sale_quotation.code,
          sale_order.code,
          sale_shipping.code,
          sale_invoice.code,
          sale_credit_invoice.code,
          sale_shipping_return.code,

          -- POS Tables
          pos_order.code,
          pos_credit_order.code,

          -- Commercial Tables
          commercial_payment.code,
          commercial_stock_out.code,
          commercial_stock_entry.code,
          commercial_stock_mouvement.code,
          commercial_need_expression.code,
  
          -- Commercial Partner (Added for partner case)
          CASE WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN commercial_partner_direct.name ELSE NULL END,

          -- CRM Table
          crm_opportunity.code
        ) AS piece_code,
          -- New Column: Partner Name (via commercial_partner)
          commercial_partner.name AS partner_name,
          -- Product Name (only populated for Commercial.Product%)
          commercial_product.name AS product_name
        FROM
          atooerp_activity
          -- Existing Joins (unchanged)
          LEFT OUTER JOIN hr_employe ON atooerp_activity.author = hr_employe.Id
          LEFT OUTER JOIN hr_employe hr_employe_1 ON atooerp_activity.assigned_employee = hr_employe_1.Id
          LEFT OUTER JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
          LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
          LEFT OUTER JOIN atooerp_person ON hr_employe.Id = atooerp_person.Id
          LEFT OUTER JOIN atooerp_person atooerp_person_1 ON hr_employe_1.Id = atooerp_person_1.Id
          LEFT OUTER JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
          -- Purchase Tables
        LEFT OUTER JOIN purchase_quotation 
          ON atooerp_activity.object = purchase_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation%'
        LEFT OUTER JOIN purchase_order 
          ON atooerp_activity.object = purchase_order.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Order%'
        LEFT OUTER JOIN purchase_shipping 
          ON atooerp_activity.object = purchase_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping%'
        LEFT OUTER JOIN purchase_invoice 
          ON atooerp_activity.object = purchase_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Invoice%'
        LEFT OUTER JOIN purchase_credit_invoice 
          ON atooerp_activity.object = purchase_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%'
        LEFT OUTER JOIN purchase_quotation_request 
          ON atooerp_activity.object = purchase_quotation_request.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation_request%'
        LEFT OUTER JOIN purchase_shipping_return 
          ON atooerp_activity.object = purchase_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping_return%'

        -- Sale Tables
        LEFT OUTER JOIN sale_quotation 
          ON atooerp_activity.object = sale_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Quotation%'
        LEFT OUTER JOIN sale_order 
          ON atooerp_activity.object = sale_order.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Order%'
        LEFT OUTER JOIN sale_shipping 
          ON atooerp_activity.object = sale_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping%'
        LEFT OUTER JOIN sale_invoice 
          ON atooerp_activity.object = sale_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Invoice%'
        LEFT OUTER JOIN sale_credit_invoice 
          ON atooerp_activity.object = sale_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Credit_invoice%'
        LEFT OUTER JOIN sale_shipping_return 
          ON atooerp_activity.object = sale_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping_return%'

        -- POS Tables
        LEFT OUTER JOIN pos_order 
          ON atooerp_activity.object = pos_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Order%'
        LEFT OUTER JOIN pos_credit_order 
          ON atooerp_activity.object = pos_credit_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Credit_order%'

        -- Commercial Tables
        LEFT OUTER JOIN commercial_payment 
          ON atooerp_activity.object = commercial_payment.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Payment%'
        LEFT OUTER JOIN commercial_stock_out 
          ON atooerp_activity.object = commercial_stock_out.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_out%'
        LEFT OUTER JOIN commercial_stock_entry 
          ON atooerp_activity.object = commercial_stock_entry.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_entry%'
        LEFT OUTER JOIN commercial_stock_mouvement 
          ON atooerp_activity.object = commercial_stock_mouvement.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%'
        LEFT OUTER JOIN commercial_need_expression 
          ON atooerp_activity.object = commercial_need_expression.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Need_expression%'

        -- New join for Commercial.Partner
        LEFT OUTER JOIN commercial_partner AS commercial_partner_direct
          ON atooerp_activity.object = commercial_partner_direct.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Partner%'

        -- CRM Table
        LEFT OUTER JOIN crm_opportunity
          ON atooerp_activity.object = crm_opportunity.Id 
          AND atooerp_activity.object_type LIKE 'CRM.Opportunity%'
         -- New Join for Commercial.Product
          LEFT OUTER JOIN commercial_product 
            ON atooerp_activity.object = commercial_product.Id 
            AND atooerp_activity.object_type LIKE 'Commercial.Product%'
          -- Add joins for ALL other object_type cases here
          -- Join commercial_partner (for partner name)
          LEFT OUTER JOIN commercial_partner
          ON COALESCE(
            -- Purchase Tables
            purchase_quotation.partner,
            purchase_order.partner,
            purchase_shipping.partner,
            purchase_invoice.partner,
            purchase_credit_invoice.partner,
            purchase_shipping_return.partner,

            -- Sale Tables
            sale_quotation.partner,
            sale_order.partner,
            sale_shipping.partner,
            sale_invoice.partner,
            sale_credit_invoice.partner,
            sale_shipping_return.partner,

            -- POS Tables
            pos_order.partner,
            pos_credit_order.partner,

            -- Commercial Tables
            commercial_payment.partner,

            -- CRM Table
            crm_opportunity.partner
          ) = commercial_partner.Id
         WHERE hr_employe_1.user = @userId 
              AND atooerp_activity.state = 1  -- Filtre pour n'avoir que les activités annulées
            ORDER BY atooerp_activity.due_date ASC;";
            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 1, // Forcé à 1 pour cohérence
                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités annulées : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            // Trier par due_date (du plus proche au plus éloigné)
            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetInProgressAllActivitiesToday(int userId)
        {
            List<Activity> activities = new List<Activity>();
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

            const string sqlCmd = @"
        SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.memo AS Memo,
          atooerp_activity.form AS Form,
          atooerp_activity.gps AS Gps,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          -- New Columns: Piece Acronym (Dynamic Logic)
          CASE
          -- Purchase
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'

          -- Sale
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
          WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
          WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
          WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'

          -- POS
          WHEN atooerp_activity.object_type LIKE 'POS.Order%' THEN 'POSO'
          WHEN atooerp_activity.object_type LIKE 'POS.Credit_order%' THEN 'POSC'

          -- Commercial
          WHEN atooerp_activity.object_type LIKE 'Commercial.Payment%' THEN 'CP'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_out%' THEN 'CSO'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_entry%' THEN 'CSE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%' THEN 'CSM'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Need_expression%' THEN 'CNE'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Product%' THEN 'PROD'
          WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN 'PAR'
  
          -- CRM
          WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'CRMO'

          ELSE NULL
        END AS piece_acronym,
          -- New Column: Piece Code (from joined table)
          COALESCE(
          -- Purchase Tables
          purchase_quotation.code,
          purchase_order.code,
          purchase_shipping.code,
          purchase_invoice.code,
          purchase_credit_invoice.code,
          purchase_quotation_request.code,
          purchase_shipping_return.code,

          -- Sale Tables
          sale_quotation.code,
          sale_order.code,
          sale_shipping.code,
          sale_invoice.code,
          sale_credit_invoice.code,
          sale_shipping_return.code,

          -- POS Tables
          pos_order.code,
          pos_credit_order.code,

          -- Commercial Tables
          commercial_payment.code,
          commercial_stock_out.code,
          commercial_stock_entry.code,
          commercial_stock_mouvement.code,
          commercial_need_expression.code,
  
          -- Commercial Partner (Added for partner case)
          CASE WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN commercial_partner_direct.name ELSE NULL END,

          -- CRM Table
          crm_opportunity.code
        ) AS piece_code,
          -- New Column: Partner Name (via commercial_partner)
          commercial_partner.name AS partner_name,
          -- Product Name (only populated for Commercial.Product%)
          commercial_product.name AS product_name
        FROM
          atooerp_activity
          -- Existing Joins (unchanged)
          LEFT OUTER JOIN hr_employe ON atooerp_activity.author = hr_employe.Id
          LEFT OUTER JOIN hr_employe hr_employe_1 ON atooerp_activity.assigned_employee = hr_employe_1.Id
          LEFT OUTER JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
          LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
          LEFT OUTER JOIN atooerp_person ON hr_employe.Id = atooerp_person.Id
          LEFT OUTER JOIN atooerp_person atooerp_person_1 ON hr_employe_1.Id = atooerp_person_1.Id
          LEFT OUTER JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
          -- Purchase Tables
        LEFT OUTER JOIN purchase_quotation 
          ON atooerp_activity.object = purchase_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation%'
        LEFT OUTER JOIN purchase_order 
          ON atooerp_activity.object = purchase_order.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Order%'
        LEFT OUTER JOIN purchase_shipping 
          ON atooerp_activity.object = purchase_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping%'
        LEFT OUTER JOIN purchase_invoice 
          ON atooerp_activity.object = purchase_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Invoice%'
        LEFT OUTER JOIN purchase_credit_invoice 
          ON atooerp_activity.object = purchase_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%'
        LEFT OUTER JOIN purchase_quotation_request 
          ON atooerp_activity.object = purchase_quotation_request.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Quotation_request%'
        LEFT OUTER JOIN purchase_shipping_return 
          ON atooerp_activity.object = purchase_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Purchase.Shipping_return%'

        -- Sale Tables
        LEFT OUTER JOIN sale_quotation 
          ON atooerp_activity.object = sale_quotation.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Quotation%'
        LEFT OUTER JOIN sale_order 
          ON atooerp_activity.object = sale_order.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Order%'
        LEFT OUTER JOIN sale_shipping 
          ON atooerp_activity.object = sale_shipping.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping%'
        LEFT OUTER JOIN sale_invoice 
          ON atooerp_activity.object = sale_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Invoice%'
        LEFT OUTER JOIN sale_credit_invoice 
          ON atooerp_activity.object = sale_credit_invoice.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Credit_invoice%'
        LEFT OUTER JOIN sale_shipping_return 
          ON atooerp_activity.object = sale_shipping_return.Id 
          AND atooerp_activity.object_type LIKE 'Sale.Shipping_return%'

        -- POS Tables
        LEFT OUTER JOIN pos_order 
          ON atooerp_activity.object = pos_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Order%'
        LEFT OUTER JOIN pos_credit_order 
          ON atooerp_activity.object = pos_credit_order.Id 
          AND atooerp_activity.object_type LIKE 'POS.Credit_order%'

        -- Commercial Tables
        LEFT OUTER JOIN commercial_payment 
          ON atooerp_activity.object = commercial_payment.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Payment%'
        LEFT OUTER JOIN commercial_stock_out 
          ON atooerp_activity.object = commercial_stock_out.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_out%'
        LEFT OUTER JOIN commercial_stock_entry 
          ON atooerp_activity.object = commercial_stock_entry.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_entry%'
        LEFT OUTER JOIN commercial_stock_mouvement 
          ON atooerp_activity.object = commercial_stock_mouvement.Id 
          AND atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%'
        LEFT OUTER JOIN commercial_need_expression 
          ON atooerp_activity.object = commercial_need_expression.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Need_expression%'

        -- New join for Commercial.Partner
        LEFT OUTER JOIN commercial_partner AS commercial_partner_direct
          ON atooerp_activity.object = commercial_partner_direct.Id
          AND atooerp_activity.object_type LIKE 'Commercial.Partner%'

        -- CRM Table
        LEFT OUTER JOIN crm_opportunity
          ON atooerp_activity.object = crm_opportunity.Id 
          AND atooerp_activity.object_type LIKE 'CRM.Opportunity%'
         -- New Join for Commercial.Product
          LEFT OUTER JOIN commercial_product 
            ON atooerp_activity.object = commercial_product.Id 
            AND atooerp_activity.object_type LIKE 'Commercial.Product%'
          -- Add joins for ALL other object_type cases here
          -- Join commercial_partner (for partner name)
          LEFT OUTER JOIN commercial_partner
          ON COALESCE(
            -- Purchase Tables
            purchase_quotation.partner,
            purchase_order.partner,
            purchase_shipping.partner,
            purchase_invoice.partner,
            purchase_credit_invoice.partner,
            purchase_shipping_return.partner,

            -- Sale Tables
            sale_quotation.partner,
            sale_order.partner,
            sale_shipping.partner,
            sale_invoice.partner,
            sale_credit_invoice.partner,
            sale_shipping_return.partner,

            -- POS Tables
            pos_order.partner,
            pos_credit_order.partner,

            -- Commercial Tables
            commercial_payment.partner,

            -- CRM Table
            crm_opportunity.partner
          ) = commercial_partner.Id
         WHERE hr_employe_1.user = @userId 
              AND atooerp_activity.state = 1  -- Filtre pour n'avoir que les activités annulées
              AND DATE(atooerp_activity.due_date) = CURDATE()
            ORDER BY atooerp_activity.due_date ASC;";
            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 1, // Forcé à 1 pour cohérence
                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités annulées : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                
                }
            }

            // Trier par due_date (du plus proche au plus éloigné)
            return activities.OrderBy(a => a.DueDate).ToList();
        }
        public async static Task<List<Activity>> GetInProgressAllActivitiesOverDue(int userId)
        {
            List<Activity> activities = new List<Activity>();
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

            const string sqlCmd = @"
       SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.memo AS Memo,
          atooerp_activity.form AS Form,
          atooerp_activity.gps AS Gps,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          CASE
            WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'
            WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
            WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
            WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
            WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
            WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
            WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'
            WHEN atooerp_activity.object_type LIKE 'POS.Order%' THEN 'POSO'
            WHEN atooerp_activity.object_type LIKE 'POS.Credit_order%' THEN 'POSC'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Payment%' THEN 'CP'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_out%' THEN 'CSO'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_entry%' THEN 'CSE'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%' THEN 'CSM'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Need_expression%' THEN 'CNE'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Product%' THEN 'PROD'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN 'PAR'
            WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'CRMO'
            ELSE NULL
          END AS piece_acronym,
          COALESCE(
            purchase_quotation.code,
            purchase_order.code,
            purchase_shipping.code,
            purchase_invoice.code,
            purchase_credit_invoice.code,
            purchase_quotation_request.code,
            purchase_shipping_return.code,
            sale_quotation.code,
            sale_order.code,
            sale_shipping.code,
            sale_invoice.code,
            sale_credit_invoice.code,
            sale_shipping_return.code,
            pos_order.code,
            pos_credit_order.code,
            commercial_payment.code,
            commercial_stock_out.code,
            commercial_stock_entry.code,
            commercial_stock_mouvement.code,
            commercial_need_expression.code,
            CASE WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN commercial_partner_direct.name ELSE NULL END,
            crm_opportunity.code
          ) AS piece_code,
          commercial_partner.name AS partner_name,
          commercial_product.name AS product_name
        FROM
          atooerp_activity
          LEFT OUTER JOIN hr_employe ON atooerp_activity.author = hr_employe.Id
          LEFT OUTER JOIN hr_employe hr_employe_1 ON atooerp_activity.assigned_employee = hr_employe_1.Id
          LEFT OUTER JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
          LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
          LEFT OUTER JOIN atooerp_person ON hr_employe.Id = atooerp_person.Id
          LEFT OUTER JOIN atooerp_person atooerp_person_1 ON hr_employe_1.Id = atooerp_person_1.Id
          LEFT OUTER JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
          LEFT OUTER JOIN purchase_quotation ON atooerp_activity.object = purchase_quotation.Id AND atooerp_activity.object_type LIKE 'Purchase.Quotation%'
          LEFT OUTER JOIN purchase_order ON atooerp_activity.object = purchase_order.Id AND atooerp_activity.object_type LIKE 'Purchase.Order%'
          LEFT OUTER JOIN purchase_shipping ON atooerp_activity.object = purchase_shipping.Id AND atooerp_activity.object_type LIKE 'Purchase.Shipping%'
          LEFT OUTER JOIN purchase_invoice ON atooerp_activity.object = purchase_invoice.Id AND atooerp_activity.object_type LIKE 'Purchase.Invoice%'
          LEFT OUTER JOIN purchase_credit_invoice ON atooerp_activity.object = purchase_credit_invoice.Id AND atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%'
          LEFT OUTER JOIN purchase_quotation_request ON atooerp_activity.object = purchase_quotation_request.Id AND atooerp_activity.object_type LIKE 'Purchase.Quotation_request%'
          LEFT OUTER JOIN purchase_shipping_return ON atooerp_activity.object = purchase_shipping_return.Id AND atooerp_activity.object_type LIKE 'Purchase.Shipping_return%'
          LEFT OUTER JOIN sale_quotation ON atooerp_activity.object = sale_quotation.Id AND atooerp_activity.object_type LIKE 'Sale.Quotation%'
          LEFT OUTER JOIN sale_order ON atooerp_activity.object = sale_order.Id AND atooerp_activity.object_type LIKE 'Sale.Order%'
          LEFT OUTER JOIN sale_shipping ON atooerp_activity.object = sale_shipping.Id AND atooerp_activity.object_type LIKE 'Sale.Shipping%'
          LEFT OUTER JOIN sale_invoice ON atooerp_activity.object = sale_invoice.Id AND atooerp_activity.object_type LIKE 'Sale.Invoice%'
          LEFT OUTER JOIN sale_credit_invoice ON atooerp_activity.object = sale_credit_invoice.Id AND atooerp_activity.object_type LIKE 'Sale.Credit_invoice%'
          LEFT OUTER JOIN sale_shipping_return ON atooerp_activity.object = sale_shipping_return.Id AND atooerp_activity.object_type LIKE 'Sale.Shipping_return%'
          LEFT OUTER JOIN pos_order ON atooerp_activity.object = pos_order.Id AND atooerp_activity.object_type LIKE 'POS.Order%'
          LEFT OUTER JOIN pos_credit_order ON atooerp_activity.object = pos_credit_order.Id AND atooerp_activity.object_type LIKE 'POS.Credit_order%'
          LEFT OUTER JOIN commercial_payment ON atooerp_activity.object = commercial_payment.Id AND atooerp_activity.object_type LIKE 'Commercial.Payment%'
          LEFT OUTER JOIN commercial_stock_out ON atooerp_activity.object = commercial_stock_out.Id AND atooerp_activity.object_type LIKE 'Commercial.Stock_out%'
          LEFT OUTER JOIN commercial_stock_entry ON atooerp_activity.object = commercial_stock_entry.Id AND atooerp_activity.object_type LIKE 'Commercial.Stock_entry%'
          LEFT OUTER JOIN commercial_stock_mouvement ON atooerp_activity.object = commercial_stock_mouvement.Id AND atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%'
          LEFT OUTER JOIN commercial_need_expression ON atooerp_activity.object = commercial_need_expression.Id AND atooerp_activity.object_type LIKE 'Commercial.Need_expression%'
          LEFT OUTER JOIN commercial_partner AS commercial_partner_direct ON atooerp_activity.object = commercial_partner_direct.Id AND atooerp_activity.object_type LIKE 'Commercial.Partner%'
          LEFT OUTER JOIN crm_opportunity ON atooerp_activity.object = crm_opportunity.Id AND atooerp_activity.object_type LIKE 'CRM.Opportunity%'
          LEFT OUTER JOIN commercial_product ON atooerp_activity.object = commercial_product.Id AND atooerp_activity.object_type LIKE 'Commercial.Product%'
          LEFT OUTER JOIN commercial_partner ON COALESCE(
            purchase_quotation.partner,
            purchase_order.partner,
            purchase_shipping.partner,
            purchase_invoice.partner,
            purchase_credit_invoice.partner,
            purchase_shipping_return.partner,
            sale_quotation.partner,
            sale_order.partner,
            sale_shipping.partner,
            sale_invoice.partner,
            sale_credit_invoice.partner,
            sale_shipping_return.partner,
            pos_order.partner,
            pos_credit_order.partner,
            commercial_payment.partner,
            crm_opportunity.partner
          ) = commercial_partner.Id
        WHERE hr_employe_1.user = 85
          AND atooerp_activity.state = 1
          AND DATE(atooerp_activity.due_date) < CURRENT_DATE()
        ORDER BY atooerp_activity.due_date ASC;";
            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 1, // Forcé à 1 pour cohérence
                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités annulées : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            // Trier par due_date (du plus proche au plus éloigné)
            return activities.OrderBy(a => a.DueDate).ToList();
        }

        public async static Task<List<Activity>> GetInProgressAllActivitiesFuture(int userId)
        {
            List<Activity> activities = new List<Activity>();
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

            const string sqlCmd = @"
       SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.memo AS Memo,
          atooerp_activity.form AS Form,
          atooerp_activity.gps AS Gps,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          CASE
            WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
            WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'
            WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
            WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
            WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
            WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
            WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
            WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'
            WHEN atooerp_activity.object_type LIKE 'POS.Order%' THEN 'POSO'
            WHEN atooerp_activity.object_type LIKE 'POS.Credit_order%' THEN 'POSC'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Payment%' THEN 'CP'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_out%' THEN 'CSO'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_entry%' THEN 'CSE'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%' THEN 'CSM'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Need_expression%' THEN 'CNE'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Product%' THEN 'PROD'
            WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN 'PAR'
            WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'CRMO'
            ELSE NULL
          END AS piece_acronym,
          COALESCE(
            purchase_quotation.code,
            purchase_order.code,
            purchase_shipping.code,
            purchase_invoice.code,
            purchase_credit_invoice.code,
            purchase_quotation_request.code,
            purchase_shipping_return.code,
            sale_quotation.code,
            sale_order.code,
            sale_shipping.code,
            sale_invoice.code,
            sale_credit_invoice.code,
            sale_shipping_return.code,
            pos_order.code,
            pos_credit_order.code,
            commercial_payment.code,
            commercial_stock_out.code,
            commercial_stock_entry.code,
            commercial_stock_mouvement.code,
            commercial_need_expression.code,
            CASE WHEN atooerp_activity.object_type LIKE 'Commercial.Partner%' THEN commercial_partner_direct.name ELSE NULL END,
            crm_opportunity.code
          ) AS piece_code,
          commercial_partner.name AS partner_name,
          commercial_product.name AS product_name
        FROM
          atooerp_activity
          LEFT OUTER JOIN hr_employe ON atooerp_activity.author = hr_employe.Id
          LEFT OUTER JOIN hr_employe hr_employe_1 ON atooerp_activity.assigned_employee = hr_employe_1.Id
          LEFT OUTER JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
          LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
          LEFT OUTER JOIN atooerp_person ON hr_employe.Id = atooerp_person.Id
          LEFT OUTER JOIN atooerp_person atooerp_person_1 ON hr_employe_1.Id = atooerp_person_1.Id
          LEFT OUTER JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
          LEFT OUTER JOIN purchase_quotation ON atooerp_activity.object = purchase_quotation.Id AND atooerp_activity.object_type LIKE 'Purchase.Quotation%'
          LEFT OUTER JOIN purchase_order ON atooerp_activity.object = purchase_order.Id AND atooerp_activity.object_type LIKE 'Purchase.Order%'
          LEFT OUTER JOIN purchase_shipping ON atooerp_activity.object = purchase_shipping.Id AND atooerp_activity.object_type LIKE 'Purchase.Shipping%'
          LEFT OUTER JOIN purchase_invoice ON atooerp_activity.object = purchase_invoice.Id AND atooerp_activity.object_type LIKE 'Purchase.Invoice%'
          LEFT OUTER JOIN purchase_credit_invoice ON atooerp_activity.object = purchase_credit_invoice.Id AND atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%'
          LEFT OUTER JOIN purchase_quotation_request ON atooerp_activity.object = purchase_quotation_request.Id AND atooerp_activity.object_type LIKE 'Purchase.Quotation_request%'
          LEFT OUTER JOIN purchase_shipping_return ON atooerp_activity.object = purchase_shipping_return.Id AND atooerp_activity.object_type LIKE 'Purchase.Shipping_return%'
          LEFT OUTER JOIN sale_quotation ON atooerp_activity.object = sale_quotation.Id AND atooerp_activity.object_type LIKE 'Sale.Quotation%'
          LEFT OUTER JOIN sale_order ON atooerp_activity.object = sale_order.Id AND atooerp_activity.object_type LIKE 'Sale.Order%'
          LEFT OUTER JOIN sale_shipping ON atooerp_activity.object = sale_shipping.Id AND atooerp_activity.object_type LIKE 'Sale.Shipping%'
          LEFT OUTER JOIN sale_invoice ON atooerp_activity.object = sale_invoice.Id AND atooerp_activity.object_type LIKE 'Sale.Invoice%'
          LEFT OUTER JOIN sale_credit_invoice ON atooerp_activity.object = sale_credit_invoice.Id AND atooerp_activity.object_type LIKE 'Sale.Credit_invoice%'
          LEFT OUTER JOIN sale_shipping_return ON atooerp_activity.object = sale_shipping_return.Id AND atooerp_activity.object_type LIKE 'Sale.Shipping_return%'
          LEFT OUTER JOIN pos_order ON atooerp_activity.object = pos_order.Id AND atooerp_activity.object_type LIKE 'POS.Order%'
          LEFT OUTER JOIN pos_credit_order ON atooerp_activity.object = pos_credit_order.Id AND atooerp_activity.object_type LIKE 'POS.Credit_order%'
          LEFT OUTER JOIN commercial_payment ON atooerp_activity.object = commercial_payment.Id AND atooerp_activity.object_type LIKE 'Commercial.Payment%'
          LEFT OUTER JOIN commercial_stock_out ON atooerp_activity.object = commercial_stock_out.Id AND atooerp_activity.object_type LIKE 'Commercial.Stock_out%'
          LEFT OUTER JOIN commercial_stock_entry ON atooerp_activity.object = commercial_stock_entry.Id AND atooerp_activity.object_type LIKE 'Commercial.Stock_entry%'
          LEFT OUTER JOIN commercial_stock_mouvement ON atooerp_activity.object = commercial_stock_mouvement.Id AND atooerp_activity.object_type LIKE 'Commercial.Stock_mouvement%'
          LEFT OUTER JOIN commercial_need_expression ON atooerp_activity.object = commercial_need_expression.Id AND atooerp_activity.object_type LIKE 'Commercial.Need_expression%'
          LEFT OUTER JOIN commercial_partner AS commercial_partner_direct ON atooerp_activity.object = commercial_partner_direct.Id AND atooerp_activity.object_type LIKE 'Commercial.Partner%'
          LEFT OUTER JOIN crm_opportunity ON atooerp_activity.object = crm_opportunity.Id AND atooerp_activity.object_type LIKE 'CRM.Opportunity%'
          LEFT OUTER JOIN commercial_product ON atooerp_activity.object = commercial_product.Id AND atooerp_activity.object_type LIKE 'Commercial.Product%'
          LEFT OUTER JOIN commercial_partner ON COALESCE(
            purchase_quotation.partner,
            purchase_order.partner,
            purchase_shipping.partner,
            purchase_invoice.partner,
            purchase_credit_invoice.partner,
            purchase_shipping_return.partner,
            sale_quotation.partner,
            sale_order.partner,
            sale_shipping.partner,
            sale_invoice.partner,
            sale_credit_invoice.partner,
            sale_shipping_return.partner,
            pos_order.partner,
            pos_credit_order.partner,
            commercial_payment.partner,
            crm_opportunity.partner
          ) = commercial_partner.Id
        WHERE hr_employe_1.user = 85
          AND atooerp_activity.state = 1
          AND DATE(atooerp_activity.due_date) > CURRENT_DATE()
        ORDER BY atooerp_activity.due_date ASC;";
            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 1, // Forcé à 1 pour cohérence
                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,
                                    Memo = reader["memo"] != DBNull.Value ? reader["memo"].ToString() : string.Empty,
                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités annulées : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            // Trier par due_date (du plus proche au plus éloigné)
            return activities.OrderBy(a => a.DueDate).ToList();
        }


        public async static Task<List<Activity>> GetLateActivities(int userId)
        {
            List<Activity> activities = new List<Activity>();
            const string sqlCmd = @"
        SELECT
          atooerp_activity.Id,
          atooerp_activity_type.icon AS Icon,
          atooerp_activity.create_date AS CreateDate,
          atooerp_activity.date AS Date,
          atooerp_activity_type.name AS Type,
          atooerp_activity.summary AS Summary,
          atooerp_activity.due_date AS DueDate,
          atooerp_activity.done_date AS DoneDate,
          atooerp_activity_state.Id AS State,
          atooerp_activity.object_type AS ObjectType,
          atooerp_activity.object AS Object,
          atooerp_activity.memo AS Memo,
          atooerp_activity.form AS Form,
          atooerp_activity.gps AS Gps,
          CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name) AS Author,
          CONCAT(atooerp_person_1.first_name, ' ', atooerp_person_1.last_name) AS Employee,
          atooerp_activity_1.summary AS Parent,
          -- New Columns: Piece Acronym (Dynamic Logic)
          CASE
          -- Purchase
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN 'PQ'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN 'PO'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN 'PS'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN 'PIN'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN 'PCI'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN 'PQR'
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN 'PSR'

          -- Sale
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN 'SQ'
          WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN 'SO'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN 'SS'
          WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN 'SIN'
          WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN 'SCI'
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation_request%' THEN 'SQR'
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN 'SSR'

          -- Inventory
          WHEN atooerp_activity.object_type LIKE 'Inventory.Stock%' THEN 'ST'
          WHEN atooerp_activity.object_type LIKE 'Inventory.Stock_Move%' THEN 'SM'
          WHEN atooerp_activity.object_type LIKE 'Inventory.Stock_Adjustment%' THEN 'SA'
          WHEN atooerp_activity.object_type LIKE 'Inventory.Stock_Count%' THEN 'SC'

          -- Manufacturing
          WHEN atooerp_activity.object_type LIKE 'Manufacturing.Production%' THEN 'PR'
          WHEN atooerp_activity.object_type LIKE 'Manufacturing.Bill_of_Materials%' THEN 'BOM'
          WHEN atooerp_activity.object_type LIKE 'Manufacturing.Routing%' THEN 'RT'

          -- Quality
          WHEN atooerp_activity.object_type LIKE 'Quality.Control%' THEN 'QC'
          WHEN atooerp_activity.object_type LIKE 'Quality.Inspection%' THEN 'QI'
          WHEN atooerp_activity.object_type LIKE 'Quality.Test%' THEN 'QT'

          -- Maintenance
          WHEN atooerp_activity.object_type LIKE 'Maintenance.Equipment%' THEN 'EQ'
          WHEN atooerp_activity.object_type LIKE 'Maintenance.Maintenance%' THEN 'MT'
          WHEN atooerp_activity.object_type LIKE 'Maintenance.Preventive%' THEN 'PM'

          -- Project
          WHEN atooerp_activity.object_type LIKE 'Project.Project%' THEN 'PJ'
          WHEN atooerp_activity.object_type LIKE 'Project.Task%' THEN 'TSK'
          WHEN atooerp_activity.object_type LIKE 'Project.Milestone%' THEN 'MS'

          -- HR
          WHEN atooerp_activity.object_type LIKE 'HR.Employee%' THEN 'EMP'
          WHEN atooerp_activity.object_type LIKE 'HR.Contract%' THEN 'CTR'
          WHEN atooerp_activity.object_type LIKE 'HR.Attendance%' THEN 'ATT'
          WHEN atooerp_activity.object_type LIKE 'HR.Leave%' THEN 'LV'
          WHEN atooerp_activity.object_type LIKE 'HR.Payroll%' THEN 'PYR'

          -- Finance
          WHEN atooerp_activity.object_type LIKE 'Finance.Account%' THEN 'ACC'
          WHEN atooerp_activity.object_type LIKE 'Finance.Journal%' THEN 'JNL'
          WHEN atooerp_activity.object_type LIKE 'Finance.Payment%' THEN 'PMT'
          WHEN atooerp_activity.object_type LIKE 'Finance.Receipt%' THEN 'RCP'
          WHEN atooerp_activity.object_type LIKE 'Finance.Tax%' THEN 'TAX'

          -- CRM
          WHEN atooerp_activity.object_type LIKE 'CRM.Lead%' THEN 'LD'
          WHEN atooerp_activity.object_type LIKE 'CRM.Opportunity%' THEN 'OPP'
          WHEN atooerp_activity.object_type LIKE 'CRM.Campaign%' THEN 'CMP'
          WHEN atooerp_activity.object_type LIKE 'CRM.Contact%' THEN 'CNT'

          -- Other
          ELSE 'OT'
          END AS piece_acronym,
          -- Piece Code (Dynamic Logic)
          CASE
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Purchase.Order%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Purchase.Invoice%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Purchase.Credit_invoice%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Purchase.Quotation_request%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Purchase.Shipping_return%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Sale.Order%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Sale.Invoice%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Sale.Credit_invoice%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Sale.Quotation_request%' THEN atooerp_activity.object
          WHEN atooerp_activity.object_type LIKE 'Sale.Shipping_return%' THEN atooerp_activity.object
          ELSE NULL
          END AS piece_code,
          -- Partner Name (Dynamic Logic)
          CASE
          WHEN atooerp_activity.object_type LIKE 'Purchase%' THEN (
              SELECT CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name)
              FROM atooerp_purchase_quotation
              LEFT JOIN atooerp_person ON atooerp_purchase_quotation.partner = atooerp_person.Id
              WHERE atooerp_purchase_quotation.Id = atooerp_activity.object
              LIMIT 1
          )
          WHEN atooerp_activity.object_type LIKE 'Sale%' THEN (
              SELECT CONCAT(atooerp_person.first_name, ' ', atooerp_person.last_name)
              FROM atooerp_sale_quotation
              LEFT JOIN atooerp_person ON atooerp_sale_quotation.partner = atooerp_person.Id
              WHERE atooerp_sale_quotation.Id = atooerp_activity.object
              LIMIT 1
          )
          ELSE NULL
          END AS partner_name,
          -- Product Name (Dynamic Logic)
          CASE
          WHEN atooerp_activity.object_type LIKE 'Purchase%' THEN (
              SELECT atooerp_product.name
              FROM atooerp_purchase_quotation_line
              LEFT JOIN atooerp_product ON atooerp_purchase_quotation_line.product = atooerp_product.Id
              WHERE atooerp_purchase_quotation_line.quotation = atooerp_activity.object
              LIMIT 1
          )
          WHEN atooerp_activity.object_type LIKE 'Sale%' THEN (
              SELECT atooerp_product.name
              FROM atooerp_sale_quotation_line
              LEFT JOIN atooerp_product ON atooerp_sale_quotation_line.product = atooerp_product.Id
              WHERE atooerp_sale_quotation_line.quotation = atooerp_activity.object
              LIMIT 1
          )
          ELSE NULL
          END AS product_name
        FROM atooerp_activity
        LEFT JOIN atooerp_activity_type ON atooerp_activity.type = atooerp_activity_type.Id
        LEFT JOIN atooerp_activity_state ON atooerp_activity.state = atooerp_activity_state.Id
        LEFT JOIN atooerp_person ON atooerp_activity.author = atooerp_person.Id
        LEFT JOIN atooerp_person atooerp_person_1 ON atooerp_activity.assigned_employee = atooerp_person_1.Id
        LEFT JOIN atooerp_activity atooerp_activity_1 ON atooerp_activity.parent = atooerp_activity_1.Id
        WHERE atooerp_activity.assigned_employee = @userId
          AND atooerp_activity.state = 1  -- En cours
          AND atooerp_activity.due_date < CURDATE()  -- Date d'échéance dépassée
        ORDER BY atooerp_activity.due_date ASC;";

            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var activity = new Activity
                                {

                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Icon = reader["Icon"] != DBNull.Value ? reader["Icon"].ToString() : string.Empty,
                                    CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue,
                                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                                    Type_all = reader["Type"] != DBNull.Value ? reader["Type"].ToString() : string.Empty,
                                    Summary = reader["Summary"] != DBNull.Value ? reader["Summary"].ToString() : string.Empty,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    DoneDate = reader["DoneDate"] != DBNull.Value ? Convert.ToDateTime(reader["DoneDate"]) : (DateTime?)null,
                                    State = reader["State"] != DBNull.Value ? Convert.ToInt32(reader["State"]) : 1,
                                    ObjectType = reader["ObjectType"] != DBNull.Value ? reader["ObjectType"].ToString() : string.Empty,
                                    Object = reader["Object"] != DBNull.Value ? Convert.ToInt32(reader["Object"]) : 0,

                                    Author_name = reader["Author"] != DBNull.Value ? reader["Author"].ToString() : string.Empty,
                                    Employee = reader["Employee"] != DBNull.Value ? reader["Employee"].ToString() : string.Empty,
                                    Parent_name = reader["Parent"] != DBNull.Value ? reader["Parent"].ToString() : string.Empty,
                                    PieceAcronym = reader["piece_acronym"] != DBNull.Value ? reader["piece_acronym"].ToString() : string.Empty,
                                    PieceCode = reader["piece_code"] != DBNull.Value ? reader["piece_code"].ToString() : string.Empty,
                                    PartnerName = reader["partner_name"] != DBNull.Value ? reader["partner_name"].ToString() : string.Empty,
                                    Form = reader["Form"] != DBNull.Value ? reader["Form"].ToString() : string.Empty,
                                    Gps = reader["gps"] != DBNull.Value ? reader["gps"].ToString() : string.Empty,
                                    ProductName = reader["product_name"] != DBNull.Value ? reader["product_name"].ToString() : string.Empty
                                };
                                activities.Add(activity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des activités en retard : {ex.Message}");
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return activities.OrderBy(a => a.DueDate).ToList();
        }

    }

    public class UserModel : INotifyPropertyChanged
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string Avatar { get; set; }
        private bool _isSelected;
        public static int TotalUnreadMessages { get; private set; }
        public int? PieceId { get; set; }
        public string PieceType { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private int _unreadMessagesCount;
        public int UnreadMessagesCount
        {
            get => _unreadMessagesCount;
            set
            {
                _unreadMessagesCount = value;
                OnPropertyChanged(nameof(UnreadMessagesCount));
                OnPropertyChanged(nameof(HasUnreadMessages)); // Met à jour également HasUnreadMessages

            }
        }
        // public bool HasUnreadMessages => UnreadMessagesCount > 0;
        private bool _hasUnreadMessages;
        public bool HasUnreadMessages
        {
            get => _hasUnreadMessages;
            set
            {
                if (_hasUnreadMessages != value)
                {
                    _hasUnreadMessages = value;
                    OnPropertyChanged(nameof(HasUnreadMessages));
                }
            }
        }
        public void UpdateUnreadMessagesCount(int newUnreadMessagesCount)
        {
            UnreadMessagesCount = newUnreadMessagesCount;
        }


        public static async Task<List<UserModel>> LoadUsersAsync(int userId, int? piece, string pieceType)
        {
            List<UserModel> usersList = new List<UserModel>();
            TotalUnreadMessages = 0;

            string query = @"
SELECT 
    u.id, 
    u.login,
    lastComm.piece,
    lastComm.piece_type,

    -- Nombre de messages non lus
    (SELECT COUNT(*)
     FROM atooerp_messages_receiver mr
     JOIN atooerp_messages m2 ON m2.id = mr.message
     WHERE mr.receiver = @UserId 
       AND mr.is_read = 0 
       AND m2.sender = u.id
       AND (@Piece IS NULL OR m2.piece = @Piece)
       AND (@PieceType IS NULL OR m2.piece_type = @PieceType)) AS unreadMessages,

    -- Nombre de messages envoyés par ce user vers moi
    (SELECT COUNT(*)
     FROM atooerp_messages_receiver mr
     JOIN atooerp_messages m2 ON m2.id = mr.message
     WHERE mr.receiver = @UserId 
       AND m2.sender = u.id
       AND (@Piece IS NULL OR m2.piece = @Piece)
       AND (@PieceType IS NULL OR m2.piece_type = @PieceType)) AS sentMessages,

    lastComm.last_date AS lastCommunication

FROM 
    atooerp_user u

-- Jointure avec la dernière communication de l'utilisateur
LEFT JOIN (
    SELECT 
        CASE 
            WHEN m.sender = @UserId THEN mr.receiver
            ELSE m.sender
        END AS other_user,
        MAX(m.create_date) AS last_date,
        m.piece,
        m.piece_type
    FROM 
        atooerp_messages m
    JOIN 
        atooerp_messages_receiver mr ON mr.message = m.id
    WHERE 
        (m.sender = @UserId OR mr.receiver = @UserId)
        AND (@Piece IS NULL OR m.piece = @Piece)
        AND (@PieceType IS NULL OR m.piece_type = @PieceType)
    GROUP BY 
        other_user, m.piece, m.piece_type
) AS lastComm ON lastComm.other_user = u.id

WHERE 
    u.id != @UserId

ORDER BY 
    lastComm.last_date DESC;
";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@Piece", piece ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PieceType", pieceType ?? (object)DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int unreadMessages = reader.GetInt32("unreadMessages");

                                usersList.Add(new UserModel
                                {
                                    Id = reader.GetInt32("id"),
                                    Login = reader.GetString("login"),
                                    Avatar = "user.png",
                                    UnreadMessagesCount = unreadMessages,
                                    PieceId = reader.IsDBNull("piece") ? (int?)null : reader.GetInt32("piece"),
                                    PieceType = reader.IsDBNull("piece_type") ? null : reader.GetString("piece_type")
                                });

                                TotalUnreadMessages += unreadMessages;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des utilisateurs : {ex.Message}");
            }

            return usersList;
        }
        public static async Task<List<UserModel>> LoadUsersnotifAsync(int userId)
        {
            List<UserModel> usersList = new List<UserModel>();
            TotalUnreadMessages = 0;

            string query = @"
SELECT 
    u.id, 
    u.login,

    -- Nombre de messages non lus
    (SELECT COUNT(*)
     FROM atooerp_messages_receiver mr
     JOIN atooerp_messages m2 ON m2.id = mr.message
     WHERE mr.receiver = @UserId 
       AND mr.is_read = 0 
       AND m2.sender = u.id) AS unreadMessages,

    -- Nombre de messages envoyés par ce user vers moi
    (SELECT COUNT(*)
     FROM atooerp_messages_receiver mr
     JOIN atooerp_messages m2 ON m2.id = mr.message
     WHERE mr.receiver = @UserId 
       AND m2.sender = u.id) AS sentMessages,

    lastComm.last_date AS lastCommunication

FROM 
    atooerp_user u

-- Jointure avec la dernière communication de l'utilisateur
LEFT JOIN (
    SELECT 
        CASE 
            WHEN m.sender = @UserId THEN mr.receiver
            ELSE m.sender
        END AS other_user,
        MAX(m.create_date) AS last_date
    FROM 
        atooerp_messages m
    JOIN 
        atooerp_messages_receiver mr ON mr.message = m.id
    WHERE 
        (m.sender = @UserId OR mr.receiver = @UserId)
    GROUP BY 
        other_user
) AS lastComm ON lastComm.other_user = u.id

WHERE 
    u.id != @UserId

ORDER BY 
    lastComm.last_date DESC;";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int unreadMessages = reader.GetInt32("unreadMessages");

                                usersList.Add(new UserModel
                                {
                                    Id = reader.GetInt32("id"),
                                    Login = reader.GetString("login"),
                                    Avatar = "user.png",
                                    UnreadMessagesCount = unreadMessages,
                                    PieceId = null,  // Maintenant toujours null
                                    PieceType = null // Maintenant toujours null
                                });

                                TotalUnreadMessages += unreadMessages;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des utilisateurs : {ex.Message}");
            }

            return usersList;
        }


        //    public static async Task<List<UserModel>> LoadUsersAsync(int userId, int? piece, string pieceType )
        //    {
        //        List<UserModel> usersList = new List<UserModel>();
        //        TotalUnreadMessages = 0;

        //        string query = @"
        //SELECT 
        //    u.id, 
        //    u.login,
        //    m.piece,
        //    m.piece_type,
        //    (SELECT COUNT(*)
        //     FROM atooerp_messages_receiver mr
        //     JOIN atooerp_messages m2 ON m2.id = mr.message
        //     WHERE mr.receiver = @UserId 
        //       AND mr.is_read = 0 
        //       AND m2.sender = u.id
        //       AND (m2.piece = @Piece OR @Piece IS NULL)
        //       AND (m2.piece_type = @PieceType OR @PieceType IS NULL)) AS unreadMessages,

        //    (SELECT COUNT(*)
        //     FROM atooerp_messages_receiver mr
        //     JOIN atooerp_messages m2 ON m2.id = mr.message
        //     WHERE mr.receiver = @UserId 
        //       AND m2.sender = u.id
        //       AND (m2.piece = @Piece OR @Piece IS NULL)
        //       AND (m2.piece_type = @PieceType OR @PieceType IS NULL)) AS sentMessages
        //FROM 
        //    atooerp_user u
        //LEFT JOIN 
        //    atooerp_messages m ON m.sender = u.id
        //WHERE 
        //    u.id != @UserId
        //GROUP BY 
        //    u.id, u.login
        //ORDER BY 
        //    MAX(m.create_date) DESC;";

        //        try
        //        {
        //            using (var connection = new MySqlConnection(DbConnection.ConnectionString))
        //            {
        //                await connection.OpenAsync();
        //                using (var command = new MySqlCommand(query, connection))
        //                {
        //                    command.Parameters.AddWithValue("@UserId", userId);
        //                    command.Parameters.AddWithValue("@Piece", piece ?? (object)DBNull.Value);
        //                    command.Parameters.AddWithValue("@PieceType", pieceType ?? (object)DBNull.Value);

        //                    using (var reader = await command.ExecuteReaderAsync())
        //                    {
        //                        while (await reader.ReadAsync())
        //                        {
        //                            int unreadMessages = reader.GetInt32("unreadMessages");

        //                            usersList.Add(new UserModel
        //                            {
        //                                Id = reader.GetInt32("id"),
        //                                Login = reader.GetString("login"),
        //                                Avatar = "user.png",
        //                                UnreadMessagesCount = unreadMessages,
        //                                PieceId = reader.IsDBNull("piece") ? (int?)null : reader.GetInt32("piece"),
        //                                PieceType = reader.IsDBNull("piece_type") ? null : reader.GetString("piece_type")
        //                            });

        //                            TotalUnreadMessages += unreadMessages;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Erreur lors du chargement des utilisateurs : {ex.Message}");
        //        }

        //        return usersList;
        //    }


        public static async Task<int> GetUnreadMessagesCountAsync(int userId, int? piece , string pieceType )
        {
            const string query = @"
        SELECT COUNT(*) as UnreadCount
        FROM atooerp_messages_receiver mr
        JOIN atooerp_messages m ON m.id = mr.message
        WHERE mr.receiver = @UserId
        AND mr.is_read = 0
        AND (m.piece = @Piece OR @Piece IS NULL)
        AND (m.piece_type = @PieceType OR @PieceType IS NULL)";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@Piece", piece ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PieceType", pieceType ?? (object)DBNull.Value);

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetUnreadMessagesCountAsync: {ex.Message}");
                return 0;
            }
        }

        public static async Task<int> GetAllUnreadMessagesCountAsync(int userId)
        {
            const string query = @"
        SELECT COUNT(*) as UnreadCount
        FROM atooerp_messages_receiver mr
        JOIN atooerp_messages m ON m.id = mr.message
        WHERE mr.receiver = @UserId
        AND mr.is_read = 0";

            try
            {
                using (var connection = new MySqlConnection(DbConnection.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetUnreadMessagesCountAsync: {ex.Message}");
                return 0;
            }
        }

        //public static async Task<List<UserModel>> LoadUsersAsync(int userId)
        //{
        //    List<UserModel> usersList = new List<UserModel>();
        //    TotalUnreadMessages = 0; // Réinitialise le compteur

        //    const string query = @"
        //    SELECT u.id, u.login,
        //        (SELECT COUNT(*)
        //         FROM atooerp_messages_receiver mr
        //         JOIN atooerp_messages m ON m.id = mr.message
        //         WHERE mr.receiver = @UserId AND mr.is_read = 0 AND m.sender = u.id) AS unreadMessages,
        //        (SELECT COUNT(*)
        //         FROM atooerp_messages_receiver mr
        //         JOIN atooerp_messages m ON m.id = mr.message
        //         WHERE mr.receiver = @UserId AND m.sender = u.id) AS sentMessages
        //    FROM atooerp_user u
        //    LEFT JOIN atooerp_messages m ON m.sender = u.id
        //    WHERE u.id != @UserId
        //    GROUP BY u.id, u.login
        //    ORDER BY MAX(m.create_date) DESC;";

        //    try
        //    {
        //        using (var connection = new MySqlConnection(DbConnection.ConnectionString))
        //        {
        //            await connection.OpenAsync();
        //            using (var command = new MySqlCommand(query, connection))
        //            {
        //                // Ajoute les paramètres pour l'utilisateur et les dates de filtrage
        //                command.Parameters.AddWithValue("@UserId", userId);


        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        int unreadMessages = reader.GetInt32("unreadMessages");
        //                        int sentMessages = reader.GetInt32("sentMessages");

        //                        usersList.Add(new UserModel
        //                        {
        //                            Id = reader.GetInt32("id"),
        //                            Login = reader.GetString("login"),
        //                            Avatar = "user.png",
        //                            UnreadMessagesCount = unreadMessages,
        //                            // SentMessagesCount = sentMessages // Assurez-vous d'ajouter cette propriété dans UserModel si nécessaire
        //                        });

        //                        TotalUnreadMessages += unreadMessages; // Mise à jour du total
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erreur lors du chargement des utilisateurs : {ex.Message}");
        //    }

        //    return usersList;
        //}


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
