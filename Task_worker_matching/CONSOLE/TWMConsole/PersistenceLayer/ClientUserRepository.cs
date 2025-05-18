using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;
using Microsoft.Data.SqlClient;
using System;

namespace PersistenceLayer
{
    public class ClientUserRepository : IUserRepository
    {
        private readonly PersistenceManager PM = PersistenceManager.GetInstance();
        public bool add_user(User new_user)
        {
            if (new_user is Client client)
            {
                try
                {
                    using (var conn = PM.GetOpenConnection())
                    {
                        string insertQuery = @"
                            INSERT INTO Client (Name, Email, PhoneNum, Address, Overall_rating, Payment_info)
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Email, @PhoneNum, @Address, @Overall_rating, @Payment_info);
                            SELECT SCOPE_IDENTITY();";

                        using var cmd = new SqlCommand(insertQuery, conn);
                        cmd.Parameters.AddWithValue("@Name", client.get_name());
                        cmd.Parameters.AddWithValue("@Email", client.get_email());
                        cmd.Parameters.AddWithValue("@PhoneNum", client.get_phone_number());
                        cmd.Parameters.AddWithValue("@Address", client.get_address());
                        cmd.Parameters.AddWithValue("@Overall_rating", client.get_overall_rating());
                        cmd.Parameters.AddWithValue("@Payment_info", client.get_payment_info());

                        // Get Id to put into memory
                        int newId = Convert.ToInt32(cmd.ExecuteScalar());
                        client.set_user_ID(newId);
                        
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding task: {ex.Message}");
                    return false;
                }
            }
            else
            {
                Console.Out.WriteLine("User is not a client, stupid :(");
                return false;
            }
        }

        public List<User> get_all_users()
        {
            var clients = new List<User>();
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string selectQuery = @"
                        SELECT Id, Name, Email, PhoneNum, Address, Overall_rating, Payment_info
                        FROM Client;";

                    using var cmd = new SqlCommand(selectQuery, conn);

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Client client = new();

                        client.set_user_ID(reader.GetInt32(0));
                        client.set_name(reader.GetString(1));
                        client.set_email(reader.GetString(2));
                        client.set_phone_number(reader.GetString(3));
                        client.set_address(reader.GetString(4));
                        client.set_overall_rating(Convert.ToDouble(reader.GetDecimal(5)));
                        client.set_payment_info(reader.GetString(6));

                        clients.Add(client);
                    }
                }
                return clients;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving requests: {ex.Message}");
                return clients;
            }
        }

        public User get_user(string email)
        {
            Client client = new();
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string selectQuery = @"
                        SELECT Id, Name, Email, PhoneNum, Address, Overall_rating, Payment_info
                        FROM Client
                        WHERE Email = @Email;";

                    using var cmd = new SqlCommand(selectQuery, conn);

                    cmd.Parameters.AddWithValue("@Email", email);

                    using var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        client.set_user_ID(reader.GetInt32(0));
                        client.set_name(reader.GetString(1));
                        client.set_email(reader.GetString(2));
                        client.set_phone_number(reader.GetString(3));
                        client.set_address(reader.GetString(4));
                        client.set_overall_rating(Convert.ToDouble(reader.GetDecimal(5)));
                        client.set_payment_info(reader.GetString(6));
                    }
                    else
                    {
                        return null; // No user found with this email
                    }

                }
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving requests: {ex.Message}");
                return null;
            }
        }

        public bool update_user(User new_user, User old_user)
        {
            try
            {
                if (new_user is Client new_client && old_user is Client old_client)
                {
                    using (var conn = PM.GetOpenConnection())
                    {
                        string updateQuery = @"
                            UPDATE Client
                            SET Name = @Name,
                                Email = @Email,
                                PhoneNum = @PhoneNum,
                                Address = @Address,
                                Overall_rating = @Overall_rating,
                                Payment_info = @Payment_info
                            WHERE Id = @Id";

                        using var cmd = new SqlCommand(updateQuery, conn);
                        cmd.Parameters.AddWithValue("@Name", new_client.get_name());
                        cmd.Parameters.AddWithValue("@Email", new_client.get_email());
                        cmd.Parameters.AddWithValue("@PhoneNum", new_client.get_phone_number());
                        cmd.Parameters.AddWithValue("@Address", new_client.get_address());
                        cmd.Parameters.AddWithValue("@Overall_rating", new_client.get_overall_rating());
                        cmd.Parameters.AddWithValue("@Payment_info", new_client.get_payment_info());
                        cmd.Parameters.AddWithValue("@Id", old_client.get_user_ID());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                else
                {
                    Console.Out.WriteLine("THIS IS NOT A CLIENT HOW DID YOU END UP HERE??");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating worker offer: {ex.Message}");
                return false;
            }
        }

        public bool delete_user(User user)
        {
            try
            {
                using (var conn = PM.GetOpenConnection())
                {
                    string deleteQuery = @"
                        DELETE FROM Client
                        WHERE Id = @Id";

                    using var cmd = new SqlCommand(deleteQuery, conn);
                    cmd.Parameters.AddWithValue("@Id", user.get_user_ID());

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
}