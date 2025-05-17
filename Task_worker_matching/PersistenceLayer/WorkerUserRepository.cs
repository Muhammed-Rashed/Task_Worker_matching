using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Task_worker_matching.Memory_Layer;

namespace PersistenceLayer
{
    public class WorkerUserRepository : IUserRepository
    {
        private readonly PersistenceManager PM = PersistenceManager.GetInstance();
        public bool add_user(User new_user)
        {
            if (new_user is Worker worker)
            {
                using (var conn = PM.GetOpenConnection())
                {
                    SqlTransaction tx = conn.BeginTransaction();

                    try
                    {
                        string insertQuery = @"
                            INSERT INTO Worker (Name, Email, PhoneNum, Available_locations, Overall_rating, Available_start_time, Available_end_time)
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Email, @PhoneNum, @Available_locations, @Overall_rating, @Available_start_time, @Available_end_time);";

                        using var cmd = new SqlCommand(insertQuery, conn, tx);
                        cmd.Parameters.AddWithValue("@Name", worker.get_name());
                        cmd.Parameters.AddWithValue("@Email", worker.get_email());
                        cmd.Parameters.AddWithValue("@PhoneNum", worker.get_phone_number());
                        cmd.Parameters.AddWithValue("@Available_locations", worker.GetAvailableLocations());
                        cmd.Parameters.AddWithValue("@Overall_rating", worker.get_overall_rating());
                        cmd.Parameters.AddWithValue("@Available_start_time", worker.GetAvailableStartTime());
                        cmd.Parameters.AddWithValue("@Available_end_time", worker.GetAvailableEndTime());

                        int newId = Convert.ToInt32(cmd.ExecuteScalar());

                        worker.set_user_ID(newId);

                        // Add specialites
                        List<string> specialites = worker.GetSpecialities();

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
                            string insertLink = "INSERT INTO WorkerSpeciality (Worker_id, Speciality_id) VALUES (@WorkerId, @SpecId)";
                            SqlCommand insertLinkCmd = new SqlCommand(insertLink, conn, tx);
                            insertLinkCmd.Parameters.AddWithValue("@WorkerId", worker.get_user_ID());
                            insertLinkCmd.Parameters.AddWithValue("@SpecId", specId);
                            insertLinkCmd.ExecuteNonQuery();
                        }

                        tx.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding task: {ex.Message}");
                        tx.Rollback();
                        return false;
                    }
                }
            }
            else
            {
                Console.Out.WriteLine("User is not a worker, you goofy lil monkey :)");
                return false;
            }
        }

        public List<string> GetSpecialities(int worker_id, SqlConnection conn)
        {
            List<string> specialities = new();

            string query = @"
                SELECT S.Speciality_name
                FROM WorkerSpeciality WS
                JOIN Speciality S ON WS.Speciality_id = S.Id
                WHERE WS.Worker_id = @WorkerId;";

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@WorkerId", worker_id);

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

        public List<User> get_all_users()
        {
            var workers = new List<User>();
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string selectQuery = @"
                        SELECT Id, Name, Email, PhoneNum, Available_locations, Overall_rating, Available_start_time, Available_end_time
                        FROM Worker;";

                    using var cmd = new SqlCommand(selectQuery, conn);

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Worker worker = new();

                        worker.set_user_ID(reader.GetInt32(0));
                        worker.set_name(reader.GetString(1));
                        worker.set_email(reader.GetString(2));
                        worker.set_phone_number(reader.GetString(3));
                        worker.SetAvailableLocations(reader.GetString(4));
                        worker.set_overall_rating(Convert.ToDouble(reader.GetDecimal(5)));
                        worker.SetAvailableStartTime(reader.GetTimeSpan(7));
                        worker.SetAvailableEndTime(reader.GetTimeSpan(8));

                        // Set specialites
                        worker.SetSpecialities(GetSpecialities(worker.get_user_ID(), conn));

                        workers.Add(worker);
                    }
                }
                return workers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving requests: {ex.Message}");
                return workers;
            }
        }

        public User get_user(int userID)
        {
            Worker worker = new();
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string selectQuery = @"
                        SELECT Id, Name, Email, PhoneNum, Available_locations, Overall_rating, Available_start_time, Available_end_time
                        FROM Worker
                        WHERE Id = @Id;";

                    using var cmd = new SqlCommand(selectQuery, conn);

                    cmd.Parameters.AddWithValue("@Id", userID);

                    using var reader = cmd.ExecuteReader();

                    worker.set_user_ID(reader.GetInt32(0));
                    worker.set_name(reader.GetString(1));
                    worker.set_email(reader.GetString(2));
                    worker.set_phone_number(reader.GetString(3));
                    worker.SetAvailableLocations(reader.GetString(4));
                    worker.set_overall_rating(Convert.ToDouble(reader.GetDecimal(5)));
                    worker.SetAvailableStartTime(reader.GetTimeSpan(7));
                    worker.SetAvailableEndTime(reader.GetTimeSpan(8));

                    // Set specialites
                    worker.SetSpecialities(GetSpecialities(worker.get_user_ID(), conn));

                }
                return worker;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving requests: {ex.Message}");
                return worker;
            }
        }

        public bool update_user(User new_user, User old_user)
        {
            if (new_user is Worker new_worker && old_user is Worker old_worker)
            {
                using (var conn = PM.GetOpenConnection())
                {
                    SqlTransaction tx = conn.BeginTransaction();
                    try
                    {
                        string updateQuery = @"
                            UPDATE Worker
                            SET Name = @Name,
                                Email = @Email,
                                PhoneNum = @PhoneNum,
                                Available_locations @Available_locations,
                                Overall_rating = @Overall_rating,
                                Available_start_time = @Available_start_time,
                                Available_end_time = @Available_end_time
                            WHERE Id = @Id";

                        using var cmd = new SqlCommand(updateQuery, conn, tx);
                        cmd.Parameters.AddWithValue("@Name", new_worker.get_name());
                        cmd.Parameters.AddWithValue("@Email", new_worker.get_email());
                        cmd.Parameters.AddWithValue("@PhoneNum", new_worker.get_phone_number());
                        cmd.Parameters.AddWithValue("@Available_locations", new_worker.GetAvailableLocations());
                        cmd.Parameters.AddWithValue("@Overall_rating", new_worker.get_overall_rating());
                        cmd.Parameters.AddWithValue("@Available_start_time", new_worker.GetAvailableStartTime());
                        cmd.Parameters.AddWithValue("@Available_end_time", new_worker.GetAvailableEndTime());
                        cmd.Parameters.AddWithValue("@Id", old_worker.get_user_ID());
                        cmd.ExecuteNonQuery();

                        // Delete old worker specialities
                        string deleteOldLinks = "DELETE FROM WorkerSpeciality WHERE Worker_id = @Worker_id";
                        using var deleteCmd = new SqlCommand(deleteOldLinks, conn, tx);
                        deleteCmd.Parameters.AddWithValue("@Worker_id", old_worker.get_user_ID());
                        deleteCmd.ExecuteNonQuery();

                        // Insert new task specialities
                        foreach (var spec in new_worker.GetSpecialities())
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
                            string insertLink = "INSERT INTO WorkerSpeciality (Worker_id, Speciality_id) VALUES (@Worker_id, @SpecId)";
                            using var insertLinkCmd = new SqlCommand(insertLink, conn, tx);
                            insertLinkCmd.Parameters.AddWithValue("@Worker_id", old_worker.get_user_ID());
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
            else
            {
                Console.Out.WriteLine("NOT A WORKER");
                return false;
            }
        }

        public bool delete_user(User user)
        {
            using (var conn = PM.GetOpenConnection())
            {
                SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    string deleteQuery = @"
                        DELETE FROM Worker
                        WHERE Id = @Id";

                    using var cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", user.get_user_ID());

                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Delete task specialities
                    string deleteOldLinks = "DELETE FROM WorkerSpeciality WHERE Worker_id = @Worker_id";
                    using var deleteCmd = new SqlCommand(deleteOldLinks, conn, tx);
                    deleteCmd.Parameters.AddWithValue("@Worker_id", user.get_user_ID());
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
}