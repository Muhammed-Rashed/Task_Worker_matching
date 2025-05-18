using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PersistenceLayer;
using MyAvaloniaApp.Memory_Layer;


public class TaskRepoStrategy : IRepositoryStrategy<Task>
{
    private readonly PersistenceManager _dbConnection;

    public TaskRepoStrategy()
    {
        _dbConnection = PersistenceManager.GetInstance();
    }

    public bool add_item(Task task)
    {
        using (var conn = _dbConnection.GetOpenConnection())
        {
            SqlTransaction tx = conn.BeginTransaction();

            try
            {
                string insertQuery = @"
                    INSERT INTO Task (Name, AVG_time, AVG_fee)
                    OUTPUT INSERTED.Id
                    VALUES (@Name, @AvgTime, @AvgFee);";

                using var cmd = new SqlCommand(insertQuery, conn, tx);
                cmd.Parameters.AddWithValue("@Name", task.Name);
                cmd.Parameters.AddWithValue("@AvgTime", task.Avg_Time);
                cmd.Parameters.AddWithValue("@AvgFee", task.AVG_Fee);

                task.Id = (int)cmd.ExecuteScalar();

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                task.Id = newId;

                // Add specialites
                List<string> specialites = task.Specialties;

                foreach (var spec in specialites)
                {
                    // Ensure speciality exists
                    string insertSpeciality = @"
                        IF NOT EXISTS (SELECT * FROM Speciality WHERE Speciality_name = @SpecName)
                            INSERT INTO Speciality (Speciality_name) VALUES (@SpecName);";

                    SqlCommand insertSpecCmd = new SqlCommand(insertSpeciality, conn, tx);
                    insertSpecCmd.Parameters.AddWithValue("@SpecName", spec);
                    insertSpecCmd.ExecuteNonQuery();

                    // Get Speciality ID
                    string getSpecId = "SELECT Id FROM Speciality WHERE Speciality_name = @SpecName";
                    SqlCommand getSpecIdCmd = new SqlCommand(getSpecId, conn, tx);
                    getSpecIdCmd.Parameters.AddWithValue("@SpecName", spec);
                    int specId = (int)getSpecIdCmd.ExecuteScalar();

                    // Insert into WorkerSpeciality
                    string insertLink = "INSERT INTO TaskSpeciality (Task_id, Speciality_id) VALUES (@Task_id, @SpecId)";
                    SqlCommand insertLinkCmd = new SqlCommand(insertLink, conn, tx);
                    insertLinkCmd.Parameters.AddWithValue("@Task_id", task.Id);
                    insertLinkCmd.Parameters.AddWithValue("@SpecId", specId);
                    insertLinkCmd.ExecuteNonQuery();
                }

                tx.Commit();

                return true;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Error adding task: {ex.Message}");
                return false;
            }
        }
    }

    public List<string> GetSpecialities(int task_id, SqlConnection conn)
    {
        List<string> specialities = [];

        string query = @"
            SELECT S.Speciality_name
            FROM TaskSpeciality TS
            JOIN Speciality S ON TS.Speciality_id = S.Id
            WHERE TS.Task_id = @Task_id;";

        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Task_id", task_id);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    specialities.Add(reader.GetString(0));
                }
            }
        }

        return specialities;
    }

    public Task get_item(int user_id, int id)
    {
        try
        {
            using (var conn = _dbConnection.GetOpenConnection())
            {
                string selectQuery = @"
                    SELECT Id, Name, AVG_time, AVG_fee
                    FROM Task
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Task
                    {
                        Id = reader.GetInt32(0),
                        Specialties = GetSpecialities(reader.GetInt32(0), conn),
                        Name = reader.GetString(1),
                        Avg_Time = reader.GetInt64(2),
                        AVG_Fee = reader.GetDecimal(3)
                    };
                }
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving task: {ex.Message}");
            return null;
        }
    }

    public List<Task> get_items(int user_id)
    {
        var tasks = new List<Task>();
        try
        {
            using (var conn = _dbConnection.GetOpenConnection())
            {
                string selectQuery = @"
                    SELECT Id, Name, AVG_time, AVG_fee
                    FROM Task";

                using var cmd = new SqlCommand(selectQuery, conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task
                    {
                        Id = reader.GetInt32(0),
                        Specialties = GetSpecialities(reader.GetInt32(0), conn),
                        Name = reader.GetString(1),
                        Avg_Time = reader.GetInt64(2),
                        AVG_Fee = reader.GetDecimal(3)
                    });
                }
            }
            return tasks;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving tasks: {ex.Message}");
            return tasks;
        }
    }

    public bool update_item(Task new_item, Task old_item)
    {
        using (var conn = _dbConnection.GetOpenConnection())
        {
            SqlTransaction tx = conn.BeginTransaction();
            try
            {
                string updateQuery = @"
                    UPDATE Task
                    SET Name = @Name,
                        AVG_time = @AvgTime,
                        AVG_fee = @AvgFee
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(updateQuery, conn, tx);
                cmd.Parameters.AddWithValue("@Name", new_item.Name);
                cmd.Parameters.AddWithValue("@AvgTime", new_item.Avg_Time);
                cmd.Parameters.AddWithValue("@AvgFee", new_item.AVG_Fee);
                cmd.Parameters.AddWithValue("@Id", old_item.Id);
                cmd.ExecuteNonQuery();

                // Delete old task specialities
                string deleteOldLinks = "DELETE FROM TaskSpeciality WHERE Task_id = @TaskId";
                using var deleteCmd = new SqlCommand(deleteOldLinks, conn, tx);
                deleteCmd.Parameters.AddWithValue("@TaskId", old_item.Id);
                deleteCmd.ExecuteNonQuery();

                // Insert new task specialities
                foreach (var spec in new_item.Specialties)
                {
                    // Check if speciality exists
                    string insertSpeciality = @"
                        IF NOT EXISTS (SELECT * FROM Speciality WHERE Speciality_name = @SpecName)
                            INSERT INTO Speciality (Speciality_name) VALUES (@SpecName);";

                    using var insertSpecCmd = new SqlCommand(insertSpeciality, conn, tx);
                    insertSpecCmd.Parameters.AddWithValue("@SpecName", spec);
                    insertSpecCmd.ExecuteNonQuery();

                    // Get ID
                    string getSpecId = "SELECT Id FROM Speciality WHERE Speciality_name = @SpecName";
                    using var getSpecIdCmd = new SqlCommand(getSpecId, conn, tx);
                    getSpecIdCmd.Parameters.AddWithValue("@SpecName", spec);
                    int specId = (int)getSpecIdCmd.ExecuteScalar();

                    // Insert link
                    string insertLink = "INSERT INTO TaskSpeciality (Task_id, Speciality_id) VALUES (@TaskId, @SpecId)";
                    using var insertLinkCmd = new SqlCommand(insertLink, conn, tx);
                    insertLinkCmd.Parameters.AddWithValue("@TaskId", old_item.Id);
                    insertLinkCmd.Parameters.AddWithValue("@SpecId", specId);
                    insertLinkCmd.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Error updating task: {ex.Message}");
                return false;
            }
        }
    }


    public bool delete_item(Task item)
    {
        using (var conn = _dbConnection.GetOpenConnection())
        {
            SqlTransaction tx = conn.BeginTransaction();
            try
            {
                string deleteQuery = @"
                    DELETE FROM Task
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@Id", item.Id);

                int rowsAffected = cmd.ExecuteNonQuery();

                // Delete task specialities
                string deleteOldLinks = "DELETE FROM TaskSpeciality WHERE Task_id = @TaskId";
                using var deleteCmd = new SqlCommand(deleteOldLinks, conn, tx);
                deleteCmd.Parameters.AddWithValue("@TaskId", item.Id);
                deleteCmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task: {ex.Message}");
                return false;
            }
        }
    }
}