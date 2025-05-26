using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
namespace SmartPharma5.Model
{
    public class LogModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ObjectType { get; set; }
        public int ObjectId { get; set; }
        public string ActionType { get; set; }
        public DateTime Date { get; set; }
        public string IP { get; set; }
        public string MacAddress { get; set; }
        public string MachineName { get; set; }
        public string GPS { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; private set; }


        public int Priority { get; set; }
        public string Utilisateur { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string LogType { get; set; }
        public string Operation { get; set; }
        public DateTime OrderDate { get; set; }
        //ajouter le champ is_read
        public bool IsRead { get; set; }
        public bool IsMessage => LogType == "message";
        public string ReadIcon => IsRead ? "read_icon.png" : "unread_icon.png";

        public string FullName => $"{FirstName} {LastName}".Trim();
        public string FullDate => Date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("fr-FR"));
        public string OperationLogType => $"{Operation} {LogType}".Trim();
        public bool IsGpsVisible => !string.IsNullOrEmpty(GPS);

        public static async Task<List<LogModel>> GetLogsAsync(int moduleId, string moduleType)
        {
            var logs = new List<LogModel>();

            try
            {
                DbConnection.Connecter();

                string sql = @"
            SELECT 1 as priority_, concat(atooerp_person_1.first_name,' ',atooerp_person_1.last_name) AS utilisateur,
            CASE
                WHEN DATE(atooerp_activity.due_date) = CURDATE() THEN
                    CONCAT('Aujourd''hui :  ', '""', atooerp_activity.summary, '""  pour ', concat(atooerp_person_1.first_name,' ',atooerp_person_1.last_name))
                WHEN DATE(atooerp_activity.due_date) < CURDATE() THEN
                    CONCAT('', DATEDIFF(CURDATE(), DATE(atooerp_activity.due_date)), ' jours de retard : ', '""', atooerp_activity.summary, '"" pour ', concat(atooerp_person_1.first_name,' ',atooerp_person_1.last_name))
                WHEN DATE(atooerp_activity.due_date) > CURDATE() THEN
                    CONCAT('Venant à échéance dans ', DATEDIFF(DATE(atooerp_activity.due_date), CURDATE()), ' jours :  ', '""', atooerp_activity.summary, '"" pour ',concat(atooerp_person_1.first_name,' ',atooerp_person_1.last_name))
                ELSE
                    atooerp_activity.summary
            END AS name,
            CAST(NULL AS CHAR) AS subject, COALESCE(CONCAT(
                'Type: ', COALESCE(atooerp_activity_type.name, ''), '\n',
                'Créé    ', COALESCE(atooerp_activity.create_date, ''), ' par: ', 
                COALESCE(atooerp_person_1.first_name, ''), ' ', COALESCE(atooerp_person_1.last_name, ''), '\n',
                'Autheur: ', COALESCE(atooerp_person_1.first_name, ''), ' ', COALESCE(atooerp_person_1.last_name, ''), '\n',
                'Attribué à  ', COALESCE(atooerp_person.first_name, ''), ' ', COALESCE(atooerp_person.last_name, ''), '\n',
                'Dûe le   ', COALESCE(atooerp_activity.due_date, ''), '\n',
                COALESCE(atooerp_activity.memo, '')
            ), '') AS content
            , atooerp_activity.create_date AS create_date, atooerp_activity.object_type, 'activity' AS log_type, 'insert' AS operation,atooerp_activity.due_date as order_date,
            NULL AS gps, NULL AS is_read, NULL AS read_date
            FROM atooerp_activity  
            INNER JOIN atooerp_activity_state ON atooerp_activity_state.Id = atooerp_activity.state  and atooerp_activity_state.Id=1 
            INNER JOIN atooerp_person atooerp_person_1 ON atooerp_activity.author = atooerp_person_1.Id 
            LEFT OUTER JOIN atooerp_person ON atooerp_activity.assigned_employee = atooerp_person.Id 
            LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity_type.Id = atooerp_activity.type
            WHERE (atooerp_activity.object_type LIKE CONCAT(@object_type, '%')) AND (atooerp_activity.object= @piece)

            UNION
            SELECT 2 as priority_, concat(atooerp_person_1.first_name,' ',atooerp_person_1.last_name) AS utilisateur,
            CONCAT(
              atooerp_person_1.first_name, ' ', atooerp_person_1.last_name,
              '  - Il y a ',
              CASE
                WHEN DATEDIFF(NOW(), atooerp_activity.done_date) = 0 THEN 
                  CASE
                    WHEN TIMESTAMPDIFF(MINUTE, atooerp_activity.done_date, NOW()) < 60 THEN 
                      CONCAT(TIMESTAMPDIFF(MINUTE, atooerp_activity.done_date, NOW()), ' minutes')
                    ELSE 
                      CONCAT(TIMESTAMPDIFF(HOUR, atooerp_activity.done_date, NOW()), ' heures')
                  END
                ELSE 
                  CONCAT(DATEDIFF(NOW(), atooerp_activity.done_date), ' jours')
              END
            ) AS name,
            CAST(NULL AS CHAR) AS subject, CONCAT(atooerp_activity_type.name, ' terminée : ', atooerp_activity.summary, '\n', 'Note originale   \n ',atooerp_activity.memo) AS content
            , atooerp_activity.create_date AS create_date, atooerp_activity.object_type, 'activity' AS log_type, 'insert' AS operation,atooerp_activity.done_date as order_date,
            NULL AS gps, NULL AS is_read, NULL AS read_date
            FROM atooerp_activity  
            INNER JOIN atooerp_activity_state ON atooerp_activity_state.Id = atooerp_activity.state  and atooerp_activity_state.Id=2 
            INNER JOIN atooerp_person atooerp_person_1 ON atooerp_activity.author = atooerp_person_1.Id 
            LEFT OUTER JOIN atooerp_person ON atooerp_activity.assigned_employee = atooerp_person.Id 
            LEFT OUTER JOIN atooerp_activity_type ON atooerp_activity_type.Id = atooerp_activity.type
            WHERE (atooerp_activity.object_type LIKE CONCAT(@object_type, '%')) AND (atooerp_activity.object= @piece)

            UNION 
            SELECT 3 as priority_, atooerp_user_4.login AS utilisateur, CONCAT('envoie ', CONCAT('email a ', atooerp_mailer_email.recipient)) AS name, atooerp_mailer_email.subject, atooerp_mailer_email.body AS content, atooerp_log.date AS create_date, 
            atooerp_log.object_type, 'email' AS log_type, atooerp_log.type AS operation, atooerp_mailer_email.create_date as order_date,
            atooerp_log.gps, NULL AS is_read, NULL AS read_date
            FROM atooerp_log 
            INNER JOIN atooerp_mailer_email ON atooerp_mailer_email.Id = atooerp_log.object AND atooerp_mailer_email.piece_type = @object_type AND atooerp_mailer_email.piece= @piece 
            LEFT OUTER JOIN atooerp_user atooerp_user_4 ON atooerp_user_4.Id = atooerp_log.user
            WHERE (atooerp_log.object_type = 'AtooERP.Mailer.Email') 

            UNION
            SELECT 5 as priority_, atooerp_user_3.login AS utilisateur, CONCAT(atooerp_log_2.type, CONCAT(' document : ', atooerp_document.name)) AS name, CAST(NULL AS CHAR) AS subject, atooerp_document.content, atooerp_log_2.date AS create_date, 
            atooerp_log_2.object_type, 'document' AS log_type, atooerp_log_2.type AS operation,atooerp_log_2.`date`as order_date,
            atooerp_log_2.gps, NULL AS is_read, NULL AS read_date
            FROM atooerp_log atooerp_log_2 
            INNER JOIN atooerp_document ON atooerp_document.Id = atooerp_log_2.object AND atooerp_document.piece= @piece AND atooerp_document.piece_type LIKE CONCAT(@object_type, '%') 
            LEFT OUTER JOIN atooerp_user atooerp_user_3 ON atooerp_user_3.Id = atooerp_log_2.user
            WHERE (atooerp_log_2.object_type = 'AtooERP.Document.Document') 

            UNION
            SELECT 4 as priority_, atooerp_user_2.login AS utilisateur, CONCAT('Création note ', atooerp_note.name) AS name, CAST(NULL AS CHAR) AS subject, atooerp_note.description AS content, atooerp_note.create_date, atooerp_note.piece_type AS object_type, 
            'note' AS log_type, 'insert' AS operation,atooerp_note.create_date as order_date,
            NULL AS gps, NULL AS is_read, NULL AS read_date
            FROM atooerp_note 
            INNER JOIN atooerp_user atooerp_user_2 ON atooerp_note.create_user = atooerp_user_2.Id
            WHERE (atooerp_note.piece= @piece) AND (atooerp_note.piece_type = @object_type)

            UNION
            SELECT 4 as priority_, atooerp_user_5.login AS utilisateur, CONCAT('Mise à jour note', atooerp_note_1.name) AS name, CAST(NULL AS CHAR) AS subject, atooerp_note_1.description AS content, atooerp_note_1.modify_date AS create_date, 
            atooerp_note_1.piece_type AS object_type, 'note' AS log_type, 'update' AS operation, atooerp_note_1.modify_date as order_date,
            NULL AS gps, NULL AS is_read, NULL AS read_date
            FROM atooerp_note atooerp_note_1 
            INNER JOIN atooerp_user atooerp_user_5 ON atooerp_note_1.modify_user = atooerp_user_5.Id
            WHERE (atooerp_note_1.piece= @piece) AND (atooerp_note_1.piece_type = @object_type)

            UNION
            SELECT 7 as priority_, atooerp_user_1.login AS utilisateur, CONCAT(atooerp_log_1.type, CONCAT(' ', atooerp_log_1.name)) AS name, CAST(NULL AS CHAR) AS subject, CONCAT(atooerp_log_1.type, ' ', atooerp_log_1.name) AS content, 
            atooerp_log_1.date AS create_date, atooerp_log_1.object_type, 'log' AS log_type, atooerp_log_1.type AS operation, atooerp_log_1.date as order_date,
            atooerp_log_1.gps, NULL AS is_read, NULL AS read_date
            FROM atooerp_log atooerp_log_1 
            LEFT OUTER JOIN atooerp_user atooerp_user_1 ON atooerp_user_1.Id = atooerp_log_1.user
            WHERE (atooerp_log_1.object= @piece) AND (atooerp_log_1.object_type = @object_type)

            UNION 
            SELECT 6 as priority_, atooerp_user_9.login AS utilisateur,
            cast(CONCAT(' envoie un Message a : ', atooerp_user_10.login, '  le: ', atooerp_message.create_date) as char) AS name,
            CAST(NULL AS CHAR) AS subject,
            CASE
                WHEN atooerp_message_reciever.is_read = 1 THEN
                    CONCAT(atooerp_message.text, '<p style=""color: green;"">✓✓ Lu le  ', atooerp_message_reciever.read_date,'</p>')
                ELSE
                    CONCAT(atooerp_message.text, '<p>✓ Non lu</p>')
            END as content,
            atooerp_message.create_date AS create_date,
            atooerp_message.piece_type,
            'message' AS log_type,
            'insert' AS operation,
            atooerp_message.create_date as order_date,
            NULL AS gps, atooerp_message_reciever.is_read AS is_read, atooerp_message_reciever.read_date AS read_date
            FROM atooerp_messages atooerp_message
            INNER JOIN atooerp_messages_receiver atooerp_message_reciever ON atooerp_message_reciever.message = atooerp_message.Id
            INNER JOIN atooerp_user atooerp_user_9 ON atooerp_message.sender = atooerp_user_9.Id
            INNER JOIN atooerp_user atooerp_user_10 ON atooerp_message_reciever.receiver = atooerp_user_10.Id
            WHERE (atooerp_message.piece_type= @object_type AND (atooerp_message.piece= @piece))
            ORDER BY priority_ ASC, order_date DESC";

                var cmd = new MySqlCommand(sql, DbConnection.con);
                cmd.Parameters.AddWithValue("@piece", moduleId);
                cmd.Parameters.AddWithValue("@object_type", moduleType);

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    logs.Add(new LogModel
                    {
                        Priority = reader["priority_"] == DBNull.Value ? 0 : Convert.ToInt32(reader["priority_"]),
                        Utilisateur = reader["utilisateur"] == DBNull.Value ? null : reader["utilisateur"].ToString(),
                        Name = reader["name"] == DBNull.Value ? null : reader["name"].ToString(),
                        Subject = reader["subject"] == DBNull.Value ? null : reader["subject"].ToString(),
                        Content = reader["content"] == DBNull.Value ? null : reader["content"].ToString(),
                        CreateDate = reader["create_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["create_date"]),
                        ObjectType = reader["object_type"] == DBNull.Value ? null : reader["object_type"].ToString(),
                        LogType = reader["log_type"] == DBNull.Value ? null : reader["log_type"].ToString(),
                        Operation = reader["operation"] == DBNull.Value ? null : reader["operation"].ToString(),
                        OrderDate = reader["order_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["order_date"]),
                        GPS = reader["gps"] == DBNull.Value ? null : reader["gps"].ToString(),
                        // Pour compatibilité UI
                        FirstName = reader["utilisateur"] == DBNull.Value ? "" : reader["utilisateur"].ToString().Split(' ').FirstOrDefault() ?? "",
                        LastName = reader["utilisateur"] == DBNull.Value ? "" : reader["utilisateur"].ToString().Split(' ').Skip(1).FirstOrDefault() ?? "",
                        Date = reader["create_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["create_date"]),
                        ActionType = reader["name"] == DBNull.Value ? null : reader["name"].ToString(),
                        Description = reader["content"] == DBNull.Value ? null : reader["content"].ToString(),
                        IsRead = reader["is_read"] == DBNull.Value ? false : Convert.ToBoolean(reader["is_read"]),
                        ReadDate = reader["read_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["read_date"]),
                    });
                }
                await reader.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la récupération des logs : " + ex.Message);
            }
            finally
            {
                DbConnection.Deconnecter();
            }

            return logs;
        }

    public string Initials =>
        string.IsNullOrWhiteSpace(FullName) ? "??" :
        FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries) is var parts && parts.Length >= 2 ?
            $"{parts[0][0]}{parts[1][0]}".ToUpper() :
            FullName.Length >= 2 ? FullName[..2].ToUpper() : FullName.ToUpper();

        public string RelativeDate
        {
            get
            {
                var diff = DateTime.Now - Date;
                return diff.TotalDays > 1 ? $"il y a {(int)diff.TotalDays} jours" :
                       diff.TotalHours > 1 ? $"il y a {(int)diff.TotalHours} heures" :
                       $"il y a {(int)diff.TotalMinutes} minutes";
            }
        }

        public DateTime? ReadDate { get; set; }
        public string ReadDateString => ReadDate.HasValue && ReadDate.Value > DateTime.MinValue
            ? ReadDate.Value.ToString("dd/MM/yyyy HH:mm")
            : string.Empty;

        //public static async Task<List<LogModel>> GetLogsAsync(int moduleId, string moduleType)
        //{
        //    var logs = new List<LogModel>();

        //    try
        //    {
        //        DbConnection.Connecter();

        //        string sql = @"
        //        SELECT 
        //            l.Id,
        //            l.user,
        //            l.object_type,
        //            l.object,
        //            l.type,
        //            l.date,
        //            l.ip,
        //            l.mac_adress,
        //            l.machine_name,
        //            l.gps,
        //            l.description,
        //            l.name,
        //            p.first_name,
        //            p.last_name
        //        FROM atooerp_log l
        //        LEFT JOIN hr_employe e ON e.user = l.user
        //        LEFT JOIN atooerp_person p ON p.Id = e.Id
        //        WHERE l.object = @moduleId 
        //          AND l.object_type = @moduleType
        //        ORDER BY l.date DESC";

        //        var cmd = new MySqlCommand(sql, DbConnection.con);
        //        cmd.Parameters.AddWithValue("@moduleId", moduleId);
        //        cmd.Parameters.AddWithValue("@moduleType", moduleType);

        //        var reader = await cmd.ExecuteReaderAsync();

        //        while (await reader.ReadAsync())
        //        {
        //            logs.Add(new LogModel
        //            {
        //                Id = reader.GetInt32("Id"),
        //                UserId = reader.GetInt32("user"),
        //                ObjectType = reader.GetString("object_type"),
        //                ObjectId = reader.GetInt32("object"),
        //                ActionType = reader.GetString("type"),
        //                Date = reader.GetDateTime("date"),
        //                IP = reader.IsDBNull(reader.GetOrdinal("ip")) ? null : reader.GetString("ip"),
        //                MacAddress = reader.IsDBNull(reader.GetOrdinal("mac_adress")) ? null : reader.GetString("mac_adress"),
        //                MachineName = reader.IsDBNull(reader.GetOrdinal("machine_name")) ? null : reader.GetString("machine_name"),
        //                GPS = reader.IsDBNull(reader.GetOrdinal("gps")) ? null : reader.GetString("gps"),
        //                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
        //                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString("name"),
        //                FirstName = reader.IsDBNull(reader.GetOrdinal("first_name")) ? "" : reader.GetString("first_name"),
        //                LastName = reader.IsDBNull(reader.GetOrdinal("last_name")) ? "" : reader.GetString("last_name")
        //            });
        //        }

        //        await reader.CloseAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Erreur lors de la récupération des logs : " + ex.Message);
        //    }
        //    finally
        //    {
        //        DbConnection.Deconnecter();
        //    }

        //    return logs;
        //}


    }
}