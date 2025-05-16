using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PersistenceLayer;
using Task_worker_matching.Memory_Layer;


public class TaskRepoStrategy : IRepositoryStrategy<Task>
{
    private readonly PersistenceManager _db_connection;

    public TaskRepoStrategy()
    {
        _db_connection = PersistenceManager.GetInstance();
    }

    public bool add_item(int user_id, int new_item_id, Task task)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string insertQuery = @"
                    INSERT INTO Task (Name, AVG_time, AVG_fee)
                    VALUES (@Name, @AvgTime, @AvgFee);
                    SELECT SCOPE_IDENTITY();";

                using var cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@Name", task.Name);
                cmd.Parameters.AddWithValue("@AvgTime", task.AVG_Time);
                cmd.Parameters.AddWithValue("@AvgFee", task.AVG_Fee);

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                task.Id = newId;
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding task: {ex.Message}");
            return false;
        }
    }

    public Task get_item(int user_id, int id)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
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
                        Name = reader.GetString(1),
                        AVG_Time = reader.GetInt64(2),
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
            using (var conn = _db_connection.GetOpenConnection())
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
                        Name = reader.GetString(1),
                        AVG_Time = reader.GetInt64(2),
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

    public bool update_item(int user_id, int new_item_id, Task old_item)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string updateQuery = @"
                    UPDATE Task
                    SET Name = @Name,
                        AVG_time = @AvgTime,
                        AVG_fee = @AvgFee
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@Name", old_item.Name);
                cmd.Parameters.AddWithValue("@AvgTime", old_item.AVG_Time);
                cmd.Parameters.AddWithValue("@AvgFee", old_item.AVG_Fee);
                cmd.Parameters.AddWithValue("@Id", old_item.Id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task: {ex.Message}");
            return false;
        }
    }

    public bool delete_item(int user_id, int item_id)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string deleteQuery = @"
                    DELETE FROM Task
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@Id", item_id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting task: {ex.Message}");
            return false;
        }
    }
}