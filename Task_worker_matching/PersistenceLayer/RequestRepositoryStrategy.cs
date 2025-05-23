﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PersistenceLayer;
using Task_worker_matching.Memory_Layer;


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
                    INSERT INTO Request (Client_id, Task_id, Status, preferred_date, Address, Location, Placement_time, IsPrivate, Description, Fee)
                    OUTPUT INSERTED.Id
                    VALUES (@ClientId, @TaskId, @Status, @PreferredDate, @Address, @Location, @PlacementTime, @IsPrivate, @Description, @Fee);";

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
                cmd.Parameters.AddWithValue("@Fee", request.Fee);

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

    public Request get_item(int id)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string selectQuery = @"
                    SELECT Id, Client_id, Task_id, Status, preferred_date, Address, Location, Placement_time, IsPrivate, Description, Fee
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
                        Description = reader.GetString(9),
                        Fee = reader.GetDecimal(10)
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
                    SELECT Id, Client_id, Task_id, Status, preferred_date, Address, Location, Placement_time, IsPrivate, Description, Fee
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
                        Description = reader.GetString(9),
                        Fee = reader.GetDecimal(10)
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
                        Fee = @Fee
                    WHERE Id = @Id AND Client_id = @ClientId";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@TaskId", new_item.TaskId);
                cmd.Parameters.AddWithValue("@Status", new_item.Status);
                cmd.Parameters.AddWithValue("@PreferredDate", new_item.PreferredDate);
                cmd.Parameters.AddWithValue("@Address", new_item.Address);
                cmd.Parameters.AddWithValue("@Location", new_item.Location);
                cmd.Parameters.AddWithValue("@IsPrivate", new_item.IsPrivate);
                cmd.Parameters.AddWithValue("@Description", new_item.Description);
                cmd.Parameters.AddWithValue("@Fee", new_item.Fee);
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

    // The most requested is at list[0] and the least requested is at list[1]
    public List<Task> MostAndLeastRequestedTasks()
    {
        List<Task> MostAndLeast = new();
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string query = @"
                WITH TaskRequestCounts AS (
                    SELECT Task_id, COUNT(*) AS RequestCount
                    FROM Request
                    GROUP BY Task_id
                )
                SELECT TOP 1 Task.Id, Task.Name, Task.AVG_time, Task.AVG_fee, TRC.RequestCount, 'Most Requested' AS RequestType
                FROM TaskRequestCounts TRC
                JOIN Task ON TRC.Task_id = Task.Id
                ORDER BY TRC.RequestCount DESC

                UNION ALL

                SELECT TOP 1 Task.Id, Task.Name, Task.AVG_time, Task.AVG_fee, TRC.RequestCount, 'Least Requested' AS RequestType
                FROM TaskRequestCounts TRC
                JOIN Task ON TRC.Task_id = Task.Id
                ORDER BY TRC.RequestCount ASC;";

                using var cmd = new SqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Task task = new();

                    task.Id = reader.GetInt32(0);
                    task.Name = reader.GetString(1);
                    task.Avg_Time = reader.GetInt64(2);
                    task.AVG_Fee = reader.GetDecimal(3);

                    MostAndLeast.Add(task);
                }
                return MostAndLeast;

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting request: {ex.Message}");
            return null;
        }
    }
}