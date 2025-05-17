using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MyAvaloniaApp.Memory_Layer;

namespace PersistenceLayer
{
    public class RequestExecutedRepositoryStrategy : IRepositoryStrategy<RequestExecuted>
    {
        private readonly PersistenceManager PM = PersistenceManager.GetInstance();

        public bool add_item(RequestExecuted item)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string insertQuery = @"
                    INSERT INTO RequestExecuted(Request_id, Assigned_worker_id, Client_id, Actual_start_time, Actual_end_time,
                                                Status, Client_rate, Worker_rate, Client_feedback, Worker_feedback)
                    VALUES (@Request_id, @Assigned_worker_id, @Client_id, @Actual_start_time, @Actual_end_time,
                            @Status, @Client_rate, @Worker_rate, @Client_feedback, @Worker_feedback);";

                    using var cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Request_id", item.GetRequest().Id);
                    cmd.Parameters.AddWithValue("@Assigned_worker_id", item.GetWorker().GetId());
                    cmd.Parameters.AddWithValue("@Client_id", item.GetClient().get_user_ID());
                    cmd.Parameters.AddWithValue("@Actual_start_time", item.GetActalStartTime());
                    cmd.Parameters.AddWithValue("@Actual_end_time", item.GetActalEndTime());
                    cmd.Parameters.AddWithValue("@Status", item.GetStatus().ToString());
                    cmd.Parameters.AddWithValue("@Client_rate", item.GetClientRate());
                    cmd.Parameters.AddWithValue("@Worker_rate", item.GetWorkerRate());
                    cmd.Parameters.AddWithValue("@Client_feedback", item.GetClientFeedback());
                    cmd.Parameters.AddWithValue("@Worker_feedback", item.GetWorkerFeedback());

                    // Update users' rating
                    UpdateAvgRatings(item.GetWorker().get_user_ID(), item.GetClient().get_user_ID(), conn);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding task: {ex.Message}");
                return false;
            }
        }

        public List<RequestExecuted> get_items(int user_id)
        {
            var RequestsExecuted = new List<RequestExecuted>();
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    var requestRepo = new RequestRepoStrategy();

                    string selectQuery = @"
                    SELECT Request_id, Assigned_worker_id, Client_id, Actual_start_time, Actual_end_time,
                           Status, Client_rate, Worker_rate, Client_feedback, Worker_feedback
                    FROM RequestExecuted
                    WHERE Assigned_worker_id = @worker_id";

                    using var cmd = new SqlCommand(selectQuery, conn);


                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        RequestExecuted RE = new RequestExecuted();
                        Request r = requestRepo.get_item(user_id, reader.GetInt32(0));
                        RE.SetRequest(r);
                        RE.SetActalStartTime(reader.GetDateTime(2));
                        RE.SetActalEndTime(reader.GetDateTime(3));
                        if (Enum.TryParse(reader.GetString(4), out RequestStatus status))
                        {
                            RE.SetStatus(status);
                        }
                        RE.SetClientRate(Convert.ToDouble(reader.GetDecimal(5)));
                        RE.SetWorkerRate(Convert.ToDouble(reader.GetDecimal(6)));
                        RE.SetClientFeedback(reader.GetString(7));
                        RE.SetWorkerFeedback(reader.GetString(8));

                        RequestsExecuted.Add(RE);
                    }
                }
                return RequestsExecuted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tasks: {ex.Message}");
                return RequestsExecuted;
            }
        }

        public bool update_item(RequestExecuted new_item, RequestExecuted old_item)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string updateQuery = @"
                        UPDATE Request
                        SET Actual_start_time = @Actual_start_time, 
                            Actual_end_time = @Actual_end_time, 
                            Status = @Status, 
                            Client_rate = @Client_rate, 
                            Worker_rate = @Worker_rate, 
                            Client_feedback = @Client_feedback
                            Worker_feedback = @Worker_feedback
                        WHERE Request_id = @Request_id AND Assigned_worker_id = @Assigned_worker_id AND Client_id = @Client_id";

                    using var cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@Actual_start_time", new_item.GetActalStartTime());
                    cmd.Parameters.AddWithValue("@Actual_end_time", new_item.GetActalEndTime());
                    cmd.Parameters.AddWithValue("@Status", new_item.GetStatus());
                    cmd.Parameters.AddWithValue("@Client_rate", new_item.GetClientRate());
                    cmd.Parameters.AddWithValue("@Worker_rate", new_item.GetWorkerRate());
                    cmd.Parameters.AddWithValue("@Client_feedback", new_item.GetClientFeedback());
                    cmd.Parameters.AddWithValue("@Worker_feedback", new_item.GetWorkerFeedback());
                    cmd.Parameters.AddWithValue("@Request_id", old_item.GetRequest().Id);
                    cmd.Parameters.AddWithValue("@Assigned_worker_id", old_item.GetWorker().GetId());
                    cmd.Parameters.AddWithValue("@Client_id", old_item.GetClient().get_user_ID());

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

        public bool delete_item(RequestExecuted item)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string deleteQuery = @"
                        DELETE FROM Request
                        WHERE Request_id = @Request_id AND Assigned_worker_id = @Assigned_worker_id AND Client_id = @Client_id";

                    using var cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@Request_id", item.GetRequest().Id);
                    cmd.Parameters.AddWithValue("@Assigned_worker_id", item.GetWorker().GetId());
                    cmd.Parameters.AddWithValue("@Client_id", item.GetClient().get_user_ID());

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

        public double GetAvgRating(int user_id, bool isWorker, SqlConnection conn)
        {
            try
            {
                if (!isWorker)
                {
                    string AVGQuery = @"
                    SELECT AVG(Client_rate) FROM RequestExecuted
                    WHERE Client_id = @Client_id";

                    using var cmd = new SqlCommand(AVGQuery, conn);
                    cmd.Parameters.AddWithValue("@Client_id", user_id);

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        return Convert.ToDouble(result);
                    else
                        return 0.0;
                }
                else
                {
                    string AVGQuery = @"
                    SELECT AVG(Worker_feedback) FROM RequestExecuted
                    WHERE Assigned_worker_id = @Assigned_worker_id";

                    using var cmd = new SqlCommand(AVGQuery, conn);
                    cmd.Parameters.AddWithValue("@Assigned_worker_id", user_id);

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        return Convert.ToDouble(result);
                    else
                        return 0.0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting average rating: {ex.Message}");
                return 0.0;
            }
        }

        private void UpdateAvgRatings(int worker_id, int client_id, SqlConnection conn)
        {
            double client_rating = GetAvgRating(client_id, false, conn);
            double worker_rating = GetAvgRating(worker_id, true, conn);

            string updateClientQuery = @"
                UPDATE Client
                SET Overall_rating = @Overall_rating
                WHERE Id = @Id";

            using var cmdClient = new SqlCommand(updateClientQuery, conn);
            cmdClient.Parameters.AddWithValue("@Overall_rating", client_rating);
            cmdClient.Parameters.AddWithValue("@Id", client_id);

            cmdClient.ExecuteNonQuery();

            string updateWorkerQuery = @"
                UPDATE Client
                SET Overall_rating = @Overall_rating
                WHERE Id = @Id";

            using var cmdWorker = new SqlCommand(updateWorkerQuery, conn);
            cmdWorker.Parameters.AddWithValue("@Overall_rating", worker_rating);
            cmdWorker.Parameters.AddWithValue("@Id", worker_id);

            cmdWorker.ExecuteNonQuery();
        }
    }
}