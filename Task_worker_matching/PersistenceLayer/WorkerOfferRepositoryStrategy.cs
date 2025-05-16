using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PersistenceLayer;
using Task_worker_matching.Memory_Layer;

public class WorkerOfferRepoStrategy : IRepositoryStrategy<Offer>
{
    private readonly PersistenceManager _db_connection;

    public WorkerOfferRepoStrategy()
    {
        _db_connection = PersistenceManager.GetInstance();
    }

    public bool add_item(int user_id, int new_item_id, Offer offer)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string insertQuery = @"
                    INSERT INTO WorkerOffer (worker_id, Request_id, Expiration_time, Time, Fee, Message)
                    VALUES (@WorkerId, @RequestId, @ExpirationTime, @Time, @Fee, @Message);
                    SELECT SCOPE_IDENTITY();";

                using var cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@WorkerId", offer.GetWorker().GetId());
                cmd.Parameters.AddWithValue("@RequestId", offer.GetRequest().Id);
                cmd.Parameters.AddWithValue("@ExpirationTime", offer.GetExpirationTime());
                cmd.Parameters.AddWithValue("@Time", offer.GetTime());
                cmd.Parameters.AddWithValue("@Fee", offer.GetFee());
                cmd.Parameters.AddWithValue("@Message", offer.GetMessage());

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                offer.SetId(newId); // Set the generated ID
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding worker offer: {ex.Message}");
            return false;
        }
    }

    public Offer get_item(int user_id, int id)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string selectQuery = @"
                    SELECT Id, worker_id, Request_id, Expiration_time, Time, Fee, Message
                    FROM WorkerOffer
                    WHERE Id = @Id AND worker_id = @WorkerId";

                using var cmd = new SqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@WorkerId", user_id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var offer = new Offer();
                    offer.SetId(reader.GetInt32(0));

                    var worker = new Worker();
                    worker.SetId(reader.GetInt32(1));
                    offer.SetWorker(worker);

                    var request = new Request();
                    request.Id = reader.GetInt32(2);
                    offer.SetRequest(request);

                    offer.SetExpirationTime(reader.GetDateTime(3));
                    offer.SetTime(reader.GetTimeSpan(4));
                    offer.SetFee((double)reader.GetDecimal(5));
                    offer.SetMessage(reader.GetString(6));

                    return offer;
                }
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving worker offer: {ex.Message}");
            return null;
        }
    }

    public List<Offer> get_items(int user_id)
    {
        var offers = new List<Offer>();
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string selectQuery = @"
                    SELECT Id, worker_id, Request_id, Expiration_time, Time, Fee, Message
                    FROM WorkerOffer
                    WHERE worker_id = @WorkerId";

                using var cmd = new SqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@WorkerId", user_id);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var offer = new Offer();
                    offer.SetId(reader.GetInt32(0));

                    var worker = new Worker();
                    worker.SetId(reader.GetInt32(1));
                    offer.SetWorker(worker);

                    var request = new Request();
                    request.Id = reader.GetInt32(2);
                    offer.SetRequest(request);

                    offer.SetExpirationTime(reader.GetDateTime(3));
                    offer.SetTime(reader.GetTimeSpan(4));
                    offer.SetFee((double)reader.GetDecimal(5));
                    offer.SetMessage(reader.GetString(6));

                    offers.Add(offer);
                }
            }
            return offers;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving worker offers: {ex.Message}");
            return offers;
        }
    }

    public bool update_item(int user_id, int new_item_id, Offer offer)
    {
        try
        {
            using (var conn = _db_connection.GetOpenConnection())
            {
                string updateQuery = @"
                    UPDATE WorkerOffer
                    SET Expiration_time = @ExpirationTime,
                        Time = @Time,
                        Fee = @Fee,
                        Message = @Message
                    WHERE Id = @Id AND worker_id = @WorkerId";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@ExpirationTime", offer.GetExpirationTime());
                cmd.Parameters.AddWithValue("@Time", offer.GetTime());
                cmd.Parameters.AddWithValue("@Fee", offer.GetFee());
                cmd.Parameters.AddWithValue("@Message", offer.GetMessage());
                cmd.Parameters.AddWithValue("@Id", offer.GetId());
                cmd.Parameters.AddWithValue("@WorkerId", user_id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating worker offer: {ex.Message}");
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
                    DELETE FROM WorkerOffer
                    WHERE Id = @Id AND worker_id = @WorkerId";

                using var cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@Id", item_id);
                cmd.Parameters.AddWithValue("@WorkerId", user_id);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting worker offer: {ex.Message}");
            return false;
        }
    }
}
