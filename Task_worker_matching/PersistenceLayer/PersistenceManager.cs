using System;
using Microsoft.Data.SqlClient;

namespace PersistenceLayer
{
    public class PersistenceManager
    {
        private static PersistenceManager PM;

        private PersistenceManager()
        {
            Initialize_tables();
        }

        public static PersistenceManager GetInstance()
        {
            if (PM != null)
            {
                return PM;
            }
            else
            {
                PM = new PersistenceManager();
                return PM;
            }
        }

        public void Initialize_tables()
        {

            // Create the DB if it doesn't exist
            string masterConnectionString = @"Server=localhost\SQLEXPRESS;Database=database;Trusted_Connection=True;";
            using (var masterConn = new SqlConnection(masterConnectionString))
            {
                masterConn.Open();
                using var createDbCmd = new SqlCommand($@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'database')
                    CREATE DATABASE [database];", masterConn);
                createDbCmd.ExecuteNonQuery();
            }

            string connectionString = @"Server=localhost\SQLEXPRESS;Database=database;Trusted_Connection=True";

            string createClientTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Client' AND xtype='U')
            CREATE TABLE Client (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50),
                Email VARCHAR(100),
                PhoneNum VARCHAR(15),
                Address VARCHAR(100),
                Overall_rating DECIMAL(2,1),
                Payment_info VARCHAR(100)
            );";

            string createWorkerTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Worker' AND xtype='U')
            CREATE TABLE Worker (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50),
                Email VARCHAR(100),
                PhoneNum VARCHAR(15),
                Available_locations VARCHAR(500),
                Overall_rating DECIMAL(2,1),
                Available_start_time TIME,
                Available_end_time TIME
            );";

            // Because it's a multivalued attribute
            string createSpecialityTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Speciality' AND xtype='U')
            CREATE TABLE Speciality (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Speciality_name VARCHAR(50)
            );";

            // A join table to assign specialities to workers
            string createWorkerSpecialityTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WorkerSpeciality' AND xtype='U')
            CREATE TABLE WorkerSpeciality (
                Worker_id INT FOREIGN KEY REFERENCES Worker(Id),
                Speciality_id INT FOREIGN KEY REFERENCES Speciality(Id),
                PRIMARY KEY (Worker_id, Speciality_id)
            );";

            // AVG_time is in seconds and don't ask why :_)
            string createTaskTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Task' AND xtype='U')
            CREATE TABLE Task (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50),
                AVG_time BIGINT,
                AVG_fee MONEY
            );";

            // A join table to assign specialities to Tasks
            string createTaskSpecialityTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskSpeciality' AND xtype='U')
            CREATE TABLE TaskSpeciality (
                Task_id INT FOREIGN KEY REFERENCES Task(Id),
                Speciality_id INT FOREIGN KEY REFERENCES Speciality(Id),
                PRIMARY KEY (Task_id, Speciality_id)
            );";


            string createRequestTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Request' AND xtype='U')
            CREATE TABLE Request (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Client_id INT FOREIGN KEY REFERENCES Client(Id),
                Task_id INT FOREIGN KEY REFERENCES Task(Id),
                Status VARCHAR(50),
                preferred_date DATETIME,
                Address VARCHAR(100),
                Location VARCHAR(100),
                Placement_time DATETIME,
                IsPrivate BIT,
                Description VARCHAR(1000),
                Fee MONEY
            );";

            string createWorkerOfferTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WorkerOffer' AND xtype='U')
            CREATE TABLE WorkerOffer (
                Id INT PRIMARY KEY IDENTITY(1,1),
                worker_id INT FOREIGN KEY REFERENCES Worker(Id),
                Request_id INT FOREIGN KEY REFERENCES Request(Id),
                Expiration_time DATETIME,
                Time DATETIME,
                Fee MONEY,
                Message VARCHAR(500)
            );";

            string createRequestExecutedTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RequestExecuted' AND xtype='U')
            CREATE TABLE RequestExecuted (
                Request_id INT FOREIGN KEY REFERENCES Request(Id),
                Assigned_worker_id INT FOREIGN KEY REFERENCES Worker(Id),
                Client_id INT FOREIGN KEY REFERENCES Client(Id),
                Actual_start_time DATETIME,
                Actual_end_time DATETIME,
                Status VARCHAR(50),
                Client_rate DECIMAL(2,1),
                Worker_rate DECIMAL(2,1),
                Client_feedback VARCHAR(500),
                Worker_feedback VARCHAR(500),
                PRIMARY KEY (Request_id, Assigned_worker_id, Client_id)
            );";

            // Asker type is 0 for client and 1 for worker
            string createQuestionTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Question' AND xtype='U')
            CREATE TABLE Question (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Asker_id INT,
                Asker_type BIT,
                Question_text VARCHAR(1000),
                Question_time DATETIME,
                Question_rate DECIMAL(2,1)
            );";

            // Answerer type is 0 for client and 1 for worker
            string createAnswerTable = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Answer' AND xtype='U')
            CREATE TABLE Answer (
                Question_id INT FOREIGN KEY REFERENCES Question(Id),
                Answer_text VARCHAR(1000),
                Answerer_id INT,
                Answerer_type BIT,
                Answer_time DATETIME,
                Answer_rate DECIMAL(2,1),
                PRIMARY KEY (Question_id, Answerer_id, Answer_time)
            );";

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                var commands = new[]
                {
                        createClientTable,
                        createWorkerTable,
                        createSpecialityTable,
                        createWorkerSpecialityTable,
                        createTaskTable,
                        createTaskSpecialityTable,
                        createRequestTable,
                        createWorkerOfferTable,
                        createRequestExecutedTable,
                        createQuestionTable,
                        createAnswerTable
                    };

                foreach (var cmdText in commands)
                {
                    using var cmd = new SqlCommand(cmdText, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Database initialization failed: " + ex.Message);
            }

        }

        public SqlConnection GetOpenConnection()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=database;Trusted_Connection=True;";
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}