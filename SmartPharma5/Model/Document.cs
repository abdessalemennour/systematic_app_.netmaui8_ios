using MvvmHelpers.Commands;
using MySqlConnector;
using System.ComponentModel;
using SmartPharma5.View;
using System.Threading.Tasks;
using System.Data;

namespace SmartPharma5.Model
{
    public class Document
    {
        #region Attributs

        public int Id { get; set; }
        public string name { get; set; }
        public DateTime create_date { get; set; }
        public string memo { get; set; }
        public string description { get; set; }
        public DateTime? date { get; set; }
        public DateTime? date_validity { get; set; }
        public string extension { get; set; }
        public uint? type_document { get; set; }
        public byte[] content { get; set; }
        public bool check => content != null && content.Length > 0;
        //public long size => content?.LongLength ?? 0;
        public long size { get; set; }

        public int? piece { get; set; }
        public string piece_type { get; set; }
        public string label { get; set; }
        public DateTime? return_date { get; set; }
        #endregion
        private string _typeDocumentName;
        public string TypeDocumentName
        {
            get => _typeDocumentName ?? "Unknown Type";
            private set => _typeDocumentName = value;
        }
        public async Task LoadTypeDocumentNameAsync()
        {
            if (type_document.HasValue)
            {
                TypeDocumentName = await GetDocumentTypeNameByIdAsync(type_document.Value);
            }
        }

        #region Constructeurs
        public Document()
        {
            create_date = DateTime.Now;
            content = Array.Empty<byte>();
        }

        public Document(int? piece, string piece_type, string label) : this()
        {
            this.piece = piece;
            this.piece_type = piece_type;
            this.label = label;
        }

        public Document(int id, string name, DateTime create_date, string memo, string description, DateTime? date,
            DateTime? date_validity, string extension, uint? type_document, byte[] content, int? piece,
            string piece_type, DateTime? return_date)
        {
            Id = id;
            this.name = name;
            this.create_date = create_date;
            this.memo = memo;
            this.description = description;
            this.date = date;
            this.date_validity = date_validity;
            this.extension = extension;
            this.type_document = type_document;
            this.content = content;
            this.piece = piece;
            this.piece_type = piece_type;
            this.return_date = return_date;
        }
        public Document(string name, string extension, byte[] content, string memo, string description, int? selectedTypeId)
        {
            this.name = name;
            this.extension = extension;
            this.content = content;
            this.create_date = DateTime.Now;
            this.date = DateTime.Now;
            this.memo = memo;
            this.description = description;
            this.type_document = selectedTypeId.HasValue ? (uint?)selectedTypeId.Value : null;
            this.size = content?.LongLength ?? 0;
        }
        #endregion
        /**********opportunity save document**************/
        #region Méthodes
        public async static Task<bool> SaveToDatabase(Document document, int opportunityId)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (document.content == null || document.content.Length == 0 || string.IsNullOrWhiteSpace(document.extension))
                throw new InvalidOperationException("Le document est vide ou invalide.");

            // Récupérer piece_type depuis la table commercial_dialing
            string pieceType = await GetPieceTypeFromCommercialDialingAsync();
            if (string.IsNullOrWhiteSpace(pieceType))
            {
                throw new InvalidOperationException("Impossible de récupérer piece_type depuis commercial_dialing.");
            }

            // Assigner piece_type au document
            document.piece_type = pieceType;

            const string sqlCmd = @"INSERT INTO atooerp_document 
    (name, create_date, memo, description, date, date_validity, extension, type_document, content, piece, piece_type, return_date, size, `check`) 
    VALUES 
    (@Name, @CreateDate, @Memo, @Description, @Date, @DateValidity, @Extension, @TypeDocument, @Content, @Piece, @PieceType, @ReturnDate, @Size, @Check);";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        // Ajouter les paramètres
                        cmd.Parameters.AddWithValue("@Name", document.name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreateDate", document.create_date);
                        cmd.Parameters.AddWithValue("@Memo", document.memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", document.description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", document.date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateValidity", document.date_validity ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Extension", document.extension ?? (object)DBNull.Value);
                        //cmd.Parameters.AddWithValue("@TypeDocument", document.type_document ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TypeDocument", document.type_document.HasValue ? (object)document.type_document.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Content", document.content ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Piece", opportunityId); // Utiliser l'Id de l'opportunité
                        cmd.Parameters.AddWithValue("@PieceType", document.piece_type ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReturnDate", document.return_date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Size", document.size);
                        cmd.Parameters.AddWithValue("@Check", document.check);

                        // Exécuter la commande
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        // Déconnexion de la base de données
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'enregistrement du document : {ex.Message}");
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
        /*******************partner save document***************************/

        public async static Task<bool> SaveToDatabasePartner(Document document, int partnerId)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (document.content == null || document.content.Length == 0 || string.IsNullOrWhiteSpace(document.extension))
                throw new InvalidOperationException("Le document est vide ou invalide.");

            // Assigner manuellement piece_type
            document.piece_type = "Commercial.Partner, Commercial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

            const string sqlCmd = @"INSERT INTO atooerp_document 
(name, create_date, memo, description, date, date_validity, extension, type_document, content, piece, piece_type, return_date, size, `check`) 
VALUES 
(@Name, @CreateDate, @Memo, @Description, @Date, @DateValidity, @Extension, @TypeDocument, @Content, @Piece, @PieceType, @ReturnDate, @Size, @Check);";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        // Ajouter les paramètres
                        cmd.Parameters.AddWithValue("@Name", document.name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreateDate", document.create_date);
                        cmd.Parameters.AddWithValue("@Memo", document.memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", document.description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", document.date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateValidity", document.date_validity ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Extension", document.extension ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TypeDocument", document.type_document ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Content", document.content ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Piece", partnerId); // Utiliser l'Id de l'opportunité
                        cmd.Parameters.AddWithValue("@PieceType", document.piece_type ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReturnDate", document.return_date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Size", document.size);
                        cmd.Parameters.AddWithValue("@Check", document.check);

                        // Exécuter la commande
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        // Déconnexion de la base de données
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'enregistrement du document : {ex.Message}");
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
        /*******************partner form save document***************************/

        public async static Task<bool> SaveToDatabasePartnerForm(Document document, int partnerId)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (document.content == null || document.content.Length == 0 || string.IsNullOrWhiteSpace(document.extension))
                throw new InvalidOperationException("Le document est vide ou invalide.");

            // Assigner manuellement piece_type                                   
            document.piece_type = "Marketing.Quiz.Partners_Forms, Marketing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

            const string sqlCmd = @"INSERT INTO atooerp_document 
(name, create_date, memo, description, date, date_validity, extension, type_document, content, piece, piece_type, return_date, size, `check`) 
VALUES 
(@Name, @CreateDate, @Memo, @Description, @Date, @DateValidity, @Extension, @TypeDocument, @Content, @Piece, @PieceType, @ReturnDate, @Size, @Check);";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        // Ajouter les paramètres
                        cmd.Parameters.AddWithValue("@Name", document.name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreateDate", document.create_date);
                        cmd.Parameters.AddWithValue("@Memo", document.memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", document.description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", document.date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateValidity", document.date_validity ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Extension", document.extension ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TypeDocument", document.type_document ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Content", document.content ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Piece", partnerId); // Utiliser l'Id de l'opportunité
                        cmd.Parameters.AddWithValue("@PieceType", document.piece_type ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReturnDate", document.return_date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Size", document.size);
                        cmd.Parameters.AddWithValue("@Check", document.check);

                        // Exécuter la commande
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        // Déconnexion de la base de données
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'enregistrement du document : {ex.Message}");
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

        /************payment save document**************/
        public async static Task<bool> SaveToDatabasePayment(Document document, int opportunityId)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (document.content == null || document.content.Length == 0 || string.IsNullOrWhiteSpace(document.extension))
                throw new InvalidOperationException("Le document est vide ou invalide.");

            // Récupérer piece_type depuis la table commercial_dialing
            string pieceType = await GetPieceTypeFromCommercialDialingPaymentAsync();
            if (string.IsNullOrWhiteSpace(pieceType))
            {
                throw new InvalidOperationException("Impossible de récupérer piece_type depuis commercial_dialing.");
            }

            // Assigner piece_type au document
            document.piece_type = pieceType;

            const string sqlCmd = @"INSERT INTO atooerp_document 
    (name, create_date, memo, description, date, date_validity, extension, type_document, content, piece, piece_type, return_date, size, `check`) 
    VALUES 
    (@Name, @CreateDate, @Memo, @Description, @Date, @DateValidity, @Extension, @TypeDocument, @Content, @Piece, @PieceType, @ReturnDate, @Size, @Check);";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        // Ajouter les paramètres
                        cmd.Parameters.AddWithValue("@Name", document.name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreateDate", document.create_date);
                        cmd.Parameters.AddWithValue("@Memo", document.memo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", document.description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", document.date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateValidity", document.date_validity ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Extension", document.extension ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TypeDocument", document.type_document ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Content", document.content ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Piece", opportunityId); // Utiliser l'Id de l'opportunité
                        cmd.Parameters.AddWithValue("@PieceType", document.piece_type ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReturnDate", document.return_date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Size", document.size);
                        cmd.Parameters.AddWithValue("@Check", document.check);

                        // Exécuter la commande
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        // Déconnexion de la base de données
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'enregistrement du document : {ex.Message}");
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

        /*******************delete file*****************************/
        public static async Task<bool> DeleteDocumentAsync(int documentId)
        {
            const string sqlCmd = @"DELETE FROM atooerp_document WHERE Id = @DocumentId;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        // Ajouter le paramètre DocumentId
                        cmd.Parameters.AddWithValue("@DocumentId", documentId);

                        // Exécuter la commande
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        // Déconnexion de la base de données
                        DbConnection.Deconnecter();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la suppression du document : {ex.Message}");
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
        /*************recuperer document opportunity**************/
        public static async Task<List<Document>> GetDocumentsByOpportunityIdAsync(int opportunityId)
        {
            //c.name = 'Opportunity' : Cela s'assure que seuls les documents liés aux piece_type qui appartiennent à "Opportunity" dans commercial_dialing sont récupérés.
            const string sqlCmd = @"
    SELECT d.Id, d.name, d.extension, d.create_date, d.date, d.memo, d.description, 
           d.type_document, d.piece_type, d.piece, d.size, t.name AS TypeDocumentName 
    FROM atooerp_document d
    LEFT JOIN atooerp_type_document t ON d.type_document = t.id
    INNER JOIN commercial_dialing c ON c.piece_type = d.piece_type
    WHERE d.piece = @OpportunityId AND c.name = 'Opportunity';"; 
        
    DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@OpportunityId", opportunityId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var documents = new List<Document>();

                            while (await reader.ReadAsync())
                            {
                                var document = new Document
                                {
                                    Id = reader.GetInt32("Id"),
                                    name = reader.GetString("name"),
                                    extension = reader.GetString("extension"),
                                    create_date = reader.GetDateTime("create_date"),
                                    date = reader.GetDateTime("date"),
                                    memo = reader.IsDBNull(reader.GetOrdinal("memo")) ? null : reader.GetString("memo"),
                                    description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                                    type_document = reader.IsDBNull(reader.GetOrdinal("type_document")) ? (uint?)null : reader.GetUInt32("type_document"),
                                    piece_type = reader.GetString("piece_type"),
                                    piece = reader.GetInt32("piece"),
                                    size = reader.GetInt64("size"), // Récupération de la taille
                                    TypeDocumentName = reader.IsDBNull(reader.GetOrdinal("TypeDocumentName")) ? "Unknown Type" : reader.GetString("TypeDocumentName"),
                                    content = null // Ne charge pas `content` ici pour éviter de charger trop de données
                                };

                                documents.Add(document);
                            }

                            DbConnection.Deconnecter();
                            return documents;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des documents : {ex.Message}");
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
        /***************partner profil get************************************/
        public static async Task<List<Document>> GetDocumentsByPartnerIdAsync(int partnerId)
        {
            const string sqlCmd = @"
SELECT d.Id, d.name, d.extension, d.create_date, d.date, d.memo, d.description, 
       d.type_document, d.piece_type, d.piece, d.size, t.name AS TypeDocumentName 
FROM atooerp_document d
LEFT JOIN atooerp_type_document t ON d.type_document = t.id
WHERE d.piece = @PartnerId 
  AND d.piece_type = 'Commercial.Partner, Commercial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null';";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@PartnerId", partnerId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var documents = new List<Document>();

                            while (await reader.ReadAsync())
                            {
                                var document = new Document
                                {
                                    Id = reader.GetInt32("Id"),
                                    name = reader.GetString("name"),
                                    extension = reader.GetString("extension"),
                                    create_date = reader.GetDateTime("create_date"),
                                    date = reader.GetDateTime("date"),
                                    memo = reader.IsDBNull(reader.GetOrdinal("memo")) ? null : reader.GetString("memo"),
                                    description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                                    type_document = reader.IsDBNull(reader.GetOrdinal("type_document")) ? (uint?)null : reader.GetUInt32("type_document"),
                                    piece_type = reader.GetString("piece_type"),
                                    piece = reader.GetInt32("piece"),
                                    size = reader.GetInt64("size"), 
                                    TypeDocumentName = reader.IsDBNull(reader.GetOrdinal("TypeDocumentName")) ? "Unknown Type" : reader.GetString("TypeDocumentName"),
                                    content = null
                                };

                                documents.Add(document);
                            }

                            DbConnection.Deconnecter();
                            return documents;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des documents : {ex.Message}");
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

        public static async Task<List<Document>> GetDocumentsByPartnerFormIdAsync(int partnerId)
        {
            const string sqlCmd = @"
SELECT d.Id, d.name, d.extension, d.create_date, d.date, d.memo, d.description, 
       d.type_document, d.piece_type, d.piece, d.size, t.name AS TypeDocumentName 
FROM atooerp_document d
LEFT JOIN atooerp_type_document t ON d.type_document = t.id
WHERE d.piece = @PartnerId 
  AND d.piece_type = 'Marketing.Quiz.Partners_Forms, Marketing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null';";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@PartnerId", partnerId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var documents = new List<Document>();

                            while (await reader.ReadAsync())
                            {
                                var document = new Document
                                {
                                    Id = reader.GetInt32("Id"),
                                    name = reader.GetString("name"),
                                    extension = reader.GetString("extension"),
                                    create_date = reader.GetDateTime("create_date"),
                                    date = reader.GetDateTime("date"),
                                    memo = reader.IsDBNull(reader.GetOrdinal("memo")) ? null : reader.GetString("memo"),
                                    description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                                    type_document = reader.IsDBNull(reader.GetOrdinal("type_document")) ? (uint?)null : reader.GetUInt32("type_document"),
                                    piece_type = reader.GetString("piece_type"),
                                    piece = reader.GetInt32("piece"),
                                    size = reader.GetInt64("size"),
                                    TypeDocumentName = reader.IsDBNull(reader.GetOrdinal("TypeDocumentName")) ? "Unknown Type" : reader.GetString("TypeDocumentName"),
                                    content = null
                                };

                                documents.Add(document);
                            }

                            DbConnection.Deconnecter();
                            return documents;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des documents : {ex.Message}");
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
        /********************************************************************/

        public static async Task<byte[]> GetDocumentContentAsync(int documentId)
        {
            const string sqlCmd = @"
            SELECT content 
            FROM atooerp_document 
            WHERE Id = @DocumentId;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@DocumentId", documentId);

                        var content = await cmd.ExecuteScalarAsync() as byte[];
                        DbConnection.Deconnecter();
                        return content;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération du contenu du document : {ex.Message}");
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

        /**********************opportunity**************************/
        public static async Task<string> GetPieceTypeFromCommercialDialingAsync()
        {
            const string query = "SELECT piece_type FROM commercial_dialing WHERE name = 'Opportunity' LIMIT 1";

            try
            {
                // Connexion à la base de données
                DbConnection.Deconnecter();
                if (DbConnection.Connecter())
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, DbConnection.con))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        DbConnection.Deconnecter();

                        if (result != null)
                        {
                            return result.ToString(); // Retourne la valeur de piece_type
                        }
                    }
                }
                DbConnection.Deconnecter();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de piece_type : {ex.Message}");
            }

            return null;
        }
        /**********************Payment**************************/
        public static async Task<string> GetPieceTypeFromCommercialDialingPaymentAsync()
        {
            const string query = "SELECT piece_type FROM commercial_dialing WHERE name = 'Payement' LIMIT 1";

            try
            {
                if (DbConnection.Connecter())
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, DbConnection.con))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            return result.ToString(); // Retourne la valeur de piece_type
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de piece_type : {ex.Message}");
                return null;
            }
            finally
            {
                DbConnection.Deconnecter();
            }
        }
        /**********************Partner**************************/
     /*   public static async Task<string> GetPieceTypeFromCommercialDialingPartnerAsync()
        {
            const string query = "SELECT piece_type FROM commercial_dialing WHERE name = 'Avoir Caise' LIMIT 1";

            try
            {
                if (DbConnection.Connecter())   
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, DbConnection.con))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            return result.ToString(); // Retourne la valeur de piece_type
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de piece_type : {ex.Message}");
                return null;
            }
            finally
            {
                DbConnection.Deconnecter();
            }
        }*/

        public static async Task<string> GetDocumentTypeNameByIdAsync(uint typeDocumentId)
        {
            const string query = "SELECT name FROM atooerp_type_document WHERE id = @TypeDocumentId;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@TypeDocumentId", typeDocumentId);

                        var result = await cmd.ExecuteScalarAsync();
                        DbConnection.Deconnecter();

                        if (result != null)
                        {
                            Console.WriteLine($"Type document name found: {result}");
                            return result.ToString();
                        }
                        else
                        {
                            Console.WriteLine("No type document name found for the given ID.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération du nom du type de document : {ex.Message}");
                    DbConnection.Deconnecter();
                }
            }
            else
            {
                Console.WriteLine("Échec de la connexion à la base de données.");
            }

            return "Unknown Type";
        }
         public static async Task<Dictionary<int, string>> GetDocumentTypesAsync()
         {
             const string query = "SELECT id, name FROM atooerp_type_document";

             try
             {
                 var documentTypes = new Dictionary<int, string>();
                 DbConnection.Deconnecter();
                 if (DbConnection.Connecter())
                 {
                     using (MySqlCommand cmd = new MySqlCommand(query, DbConnection.con))
                     {
                         using (var reader = await cmd.ExecuteReaderAsync())
                         {
                             while (await reader.ReadAsync())
                             {
                                 int id = reader.GetInt32("id");
                                 string name = reader.GetString("name");
                                 documentTypes.Add(id, name);
                             }
                         }
                     }
                 }
                 DbConnection.Deconnecter();
                 return documentTypes;
                // return null;
            }
             catch (Exception ex)
             {
                 Console.WriteLine($"Erreur lors de la récupération des types de documents : {ex.Message}");
                 return null;
             }
         }
      


        public string GetTempPath()
        {
            if (content == null || content.Length == 0 || string.IsNullOrWhiteSpace(extension))
                throw new InvalidOperationException("Le document est vide ou invalide.");

            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + extension);
            File.WriteAllBytes(tempPath, content);
            return tempPath;
        }

        public static BindingList<string> GetFileList(string path, BindingList<string> fileList)
        {
            foreach (var file in Directory.GetFiles(path))
                fileList.Add(file);

            foreach (var subdirectory in Directory.GetDirectories(path))
                GetFileList(subdirectory, fileList);

            return fileList;
        }
        /*****************payment get document*******************/
        public static async Task<List<Document>> GetDocumentsByPaymentIdAsync(int paymentId)
        {
            const string sqlCmd = @"
    SELECT d.Id, d.name, d.extension, d.create_date, d.date, d.memo, d.description, 
           d.type_document, d.piece_type, d.piece, d.size, t.name AS TypeDocumentName 
    FROM atooerp_document d
    LEFT JOIN atooerp_type_document t ON d.type_document = t.id
    INNER JOIN commercial_dialing c ON c.piece_type = d.piece_type
    WHERE d.piece = @PaymentId AND c.name = 'Payement';";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@PaymentId", paymentId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var documents = new List<Document>();

                            while (await reader.ReadAsync())
                            {
                                var document = new Document
                                {
                                    Id = reader.GetInt32("Id"),
                                    name = reader.GetString("name"),
                                    extension = reader.GetString("extension"),
                                    create_date = reader.GetDateTime("create_date"),
                                    date = reader.GetDateTime("date"),
                                    memo = reader.IsDBNull(reader.GetOrdinal("memo")) ? null : reader.GetString("memo"),
                                    description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                                    type_document = reader.IsDBNull(reader.GetOrdinal("type_document")) ? (uint?)null : reader.GetUInt32("type_document"),
                                    piece_type = reader.GetString("piece_type"),
                                    piece = reader.GetInt32("piece"),
                                    size = reader.GetInt64("size"), // Récupération de la taille
                                    TypeDocumentName = reader.IsDBNull(reader.GetOrdinal("TypeDocumentName")) ? "Unknown Type" : reader.GetString("TypeDocumentName"),
                                    content = null // Ne chargez pas content ici
                                };

                                documents.Add(document);
                            }

                            DbConnection.Deconnecter();
                            return documents;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération des documents : {ex.Message}");
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


        public static async Task<byte[]> GetDocumentPaymentContentAsync(int documentId)
        {
            const string sqlCmd = @"
            SELECT content 
            FROM atooerp_document 
            WHERE Id = @DocumentId;";

            DbConnection.Deconnecter();
            if (DbConnection.Connecter())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sqlCmd, DbConnection.con))
                    {
                        cmd.Parameters.AddWithValue("@DocumentId", documentId);

                        var content = await cmd.ExecuteScalarAsync() as byte[];
                        DbConnection.Deconnecter();
                        return content;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la récupération du contenu du document : {ex.Message}");
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

        /************************************************************/
        public string GetInsertTitle() => "Ajouter un nouveau document";
        public string GetUpdateTitle() => $"Nom de document [{name}]";
        public string GetListeTitle() => "Liste des documents";
        #endregion
    }
}