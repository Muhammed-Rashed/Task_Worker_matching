using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PersistenceLayer;
using MyAvaloniaApp.Memory_Layer;


public class RequestRepoStrategy : IRepositoryStrategy<Request>
{
    private readonly PersistenceManager _db_connection;

    public RequestRepoStrategy()
    {
        _db_connection = PersistenceManager.GetInstance();
    }

    public bool add_item(Request request)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string insertQuery = @"
                    INSERT INTO Request (Client_id, Task_id, Status, preferred_date, Address, Location, Placement_time, IsPrivate, Description)
                    OUTPUT INSERTED.Id
                    VALUES (@ClientId, @TaskId, @Status, @PreferredDate, @Address, @Location, @PlacementTime, @IsPrivate, @Description);
                    SELECT SCOPE_IDENTITY();";

                using var cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@ClientId", request.ClientId);
                cmd.Parameters.AddWithValue("@TaskId", request.TaskId);
                cmd.Parameters.AddWithValue("@Status", request.Status);
                cmd.Parameters.AddWithValue("@PreferredDate", request.PreferredDate);
                cmd.Parameters.AddWithValue("@Address", request.Address);
                cmd.Parameters.AddWithValue("@Location", request.Location);
                cmd.Parameters.AddWithValue("@PlacementTime", request.PlacementTime);
                cmd.Parameters.AddWithValue("@IsPrivate", request.IsPrivate);
                cmd.Parameters.AddWithValue("@Description", request.Description);

                request.Id = (int)cmd.ExecuteScalar();

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                request.Id = newId;
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding request: {ex.Message}");
            return false;
        }
    }

    public Request get_item(int user_id, int id)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string selectQuery = @"
                    SELECT Id, Client_id, Task_id, Status, preferred_date, Address, Location, Placement_time, IsPrivate, Description
                    FROM Request
                    WHERE Id = @Id";

                using var cmd = new SqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Request
                    {
                        Id = reader.GetInt32(0),
                        ClientId = reader.GetInt32(1),
                        TaskId = reader.GetInt32(2),
                        Status = Enum.Parse<RequestStatus>(reader.GetString(3)),
                        PreferredDate = reader.GetDateTime(4),
                        Address = reader.GetString(5),
                        Location = reader.GetString(6),
                        PlacementTime = reader.GetDateTime(7),
                        IsPrivate = reader.GetBoolean(8),
                        Description = reader.GetString(9)
                    };
                }
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving request: {ex.Message}");
            return null;
        }
    }

    public List<Request> get_items(int user_id)
    {
        var requests = new List<Request>();
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string selectQuery = @"
                    SELECT Id, Client_id, Task_id, Status, preferred_date, Address, Location, Placement_time, IsPrivate, Description
                    FROM Request
                    WHERE Client_id = @ClientId";

                using var cmd = new SqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@ClientId", user_id);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    requests.Add(new Request
                    {
                        Id = reader.GetInt32(0),
                        ClientId = reader.GetInt32(1),
                        TaskId = reader.GetInt32(2),
                        Status = Enum.Parse<RequestStatus>(reader.GetString(3)),
                        PreferredDate = reader.GetDateTime(4),
                        Address = reader.GetString(5),
                        Location = reader.GetString(6),
                        PlacementTime = reader.GetDateTime(7),
                        IsPrivate = reader.GetBoolean(8),
                        Description = reader.GetString(9)
                    });
                }
            }
            return requests;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving requests: {ex.Message}");
            return requests;
        }
    }

    public bool update_item(Request new_item, Request old_item)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string updateQuery = @"
                    UPDATE Request
                    SET Task_id = @TaskId, 
                        Status = @Status, 
                        preferred_date = @PreferredDate, 
                        Address = @Address, 
                        Location = @Location, 
                        IsPrivate = @IsPrivate, 
                        Description = @Description
                    WHERE Id = @Id AND Client_id = @ClientId";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@TaskId", new_item.TaskId);
                cmd.Parameters.AddWithValue("@Status", new_item.Status);
                cmd.Parameters.AddWithValue("@PreferredDate", new_item.PreferredDate);
                cmd.Parameters.AddWithValue("@Address", new_item.Address);
                cmd.Parameters.AddWithValue("@Location", new_item.Location);
                cmd.Parameters.AddWithValue("@IsPrivate", new_item.IsPrivate);
                cmd.Parameters.AddWithValue("@Description", new_item.Description);
                cmd.Parameters.AddWithValue("@Id", old_item.Id);
                cmd.Parameters.AddWithValue("@ClientId", old_item.ClientId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating request: {ex.Message}");
            return false;
        }
    }

    public bool delete_item(Request item)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string deleteQuery = @"
                    DELETE FROM Request
                    WHERE Id = @Id AND Client_id = @ClientId";

                using var cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.Parameters.AddWithValue("@ClientId", item.ClientId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting request: {ex.Message}");
            return false;
        }
    }
}