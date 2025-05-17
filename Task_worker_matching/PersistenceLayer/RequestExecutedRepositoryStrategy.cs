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
                    INSERT INTO RequestExecuted(Request_id, Assigned_worker_id, Actual_start_time, Actual_end_time,
                                                Status, Client_rate, Worker_rate, Client_feedback, Worker_feedback)
                    VALUES (@Request_id, @Assigned_worker_id, @Actual_start_time, @Actual_end_time,
                            @Status, @Client_rate, @Worker_rate, @Client_feedback, @Worker_feedback);";

                using var cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@Request_id", item.GetRequest().Id);
                cmd.Parameters.AddWithValue("@Assigned_worker_id", item.GetWorker().GetId());
                cmd.Parameters.AddWithValue("@Actual_start_time", item.GetActalStartTime());
                cmd.Parameters.AddWithValue("@Actual_end_time", item.GetActalEndTime());
                cmd.Parameters.AddWithValue("@Status", item.GetStatus().ToString());
                cmd.Parameters.AddWithValue("@Client_rate", item.GetClientRate());
                cmd.Parameters.AddWithValue("@Worker_rate", item.GetWorkerRate());
                cmd.Parameters.AddWithValue("@Client_feedback", item.GetClientFeedback());
                cmd.Parameters.AddWithValue("@Worker_feedback", item.GetWorkerFeedback());


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
                    SELECT Request_id, Assigned_worker_id, Actual_start_time, Actual_end_time,
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
                        RE.SetActualStartTime(reader.GetDateTime(2));
                        RE.SetActualEndTime(reader.GetDateTime(3));
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
                        WHERE Request_id = @Request_id AND Assigned_worker_id = @Assigned_worker_id";

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

        public bool delete_item(RequestExecuted item_id)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string deleteQuery = @"
                        DELETE FROM Request
                        WHERE Request_id = @Request_id AND Assigned_worker_id = @Assigned_worker_id";

                    using var cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@Request_id", item_id.GetRequest().Id);
                    cmd.Parameters.AddWithValue("@Assigned_worker_id", item_id.GetWorker().GetId());

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
}