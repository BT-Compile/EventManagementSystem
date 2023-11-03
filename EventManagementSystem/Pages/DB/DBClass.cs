using EventManagementSystem.Pages.DataClasses;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace EventManagementSystem.Pages.DB
{
    public class DBClass
    {
        // Connection Object at the class level
        public static SqlConnection DBConnection = new SqlConnection();

        // Connection String
        public static readonly string CapstoneDBConnString = "Server=Localhost;Database=CAPSTONE;Trusted_Connection=True";
        // For Hashed Passwords
        private static readonly string? AuthDBConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True";

        // Can run and return results for any "ExecuteReader query", if results exist.
        // Query is passed from the invoking code.
        public static SqlDataReader GeneralReaderQuery(string sqlQuery)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = new SqlConnection(CapstoneDBConnString);
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

            return tempReader;
        }

        // Can run and return results for any ExecuteNonQuery command, if results exist.
        // Query is passed from the invoking code.
        // This Query is mainly used for interactions to the CAPSTONE database
        public static void GeneralQuery(string sqlQuery)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = DBConnection;
            cmdProductRead.Connection.ConnectionString = CapstoneDBConnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();

        }

        public static SqlDataReader SingleUserReader(int UserID)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = new SqlConnection();
            cmdProductRead.Connection.ConnectionString = CapstoneDBConnString;
            cmdProductRead.CommandText = "SELECT * FROM \"User\" WHERE UserID = " + UserID;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader SingleEventReader(int EventID)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = new SqlConnection();
            cmdProductRead.Connection.ConnectionString = CapstoneDBConnString;
            cmdProductRead.CommandText = "SELECT * FROM Event WHERE EventID = " + EventID;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader SingleActivityReader(int ActivityID)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = new SqlConnection();
            cmdProductRead.Connection.ConnectionString = CapstoneDBConnString;
            cmdProductRead.CommandText = "SELECT * FROM Activity WHERE ActivityID = " + ActivityID;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader SingleRoomReader(int RoomID)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = new SqlConnection();
            cmdProductRead.Connection.ConnectionString = CapstoneDBConnString;
            cmdProductRead.CommandText = "SELECT * FROM Room WHERE RoomID = " + RoomID;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

            return tempReader;
        }

        public static int MaxCapacityGet(int ActivityID)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = new SqlConnection();
            cmdProductRead.Connection.ConnectionString = CapstoneDBConnString;
            cmdProductRead.CommandText = "SELECT Count(UserID) FROM ActivityAttendance WHERE ActivityID = " + ActivityID;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();
            tempReader.Read();
            int result = Int32.Parse(tempReader["Count(UserID)"].ToString());

            return result;
        }

        public static SqlDataReader AssignRoomReader(int activityid)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = new SqlConnection();
            cmdProductRead.Connection.ConnectionString = CapstoneDBConnString;
            cmdProductRead.CommandText = "SELECT * FROM Room WHERE " +
                "Room.Capacity>= " + MaxCapacityGet(activityid);
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();

            return tempReader;
        }

        // Method to retrieve the EventName attribute from the current Event that was selected
        // when viewing the schedule
        public static string GetEventName(int eventID)
        {
            string eventName = null;

            using (SqlConnection connection = new SqlConnection(CapstoneDBConnString))
            {
                connection.Open();

                string sqlQuery = "SELECT EventName FROM Event WHERE EventID = @EventID";

                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                {
                    sqlCommand.Parameters.AddWithValue("@EventID", eventID);
                    var result = sqlCommand.ExecuteScalar();

                    if (result != null)
                    {
                        eventName = result.ToString();
                    }
                }
            }

            return eventName;
        }

        // Method to retrieve the full name from the current User
        public static string GetUserName(int userID)
        {
            string userName = null;

            using (SqlConnection connection = new SqlConnection(CapstoneDBConnString))
            {
                connection.Open();

                string sqlQuery = "SELECT CONCAT(FirstName, ' ', LastName) AS \"Full Name\" FROM \"User\" WHERE UserID = @UserID";

                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                {
                    sqlCommand.Parameters.AddWithValue("@UserID", userID);
                    var result = sqlCommand.ExecuteScalar();

                    if (result != null)
                    {
                        userName = result.ToString();
                    }
                }
            }

            return userName;
        }

        // Method to create a user, while also PREVENTING SQL INJECTION
        public static void SecureUserCreation(string firstName, string lastName, string email,
            string phoneNumber, string username, string allergyNote, string accessibility, bool isActive)
        {
            // set all null variables to an empty string
            if (email == null)
            {
                email = "";
            }
            if (phoneNumber == null)
            {
                phoneNumber = "";
            }

            string creationQuery = "INSERT INTO \"User\" (FirstName, LastName, Email, PhoneNumber, Username, AllergyNote, Accessibility) VALUES (" +
                "@FirstName," +
                "@LastName," +
                "@Email," +
                "@PhoneNumber," +
                "@Username," +
                "@AllergyNote," +
                "@Accessibility)";


            SqlCommand cmdCreation = new SqlCommand();
            cmdCreation.Connection = DBConnection;
            cmdCreation.Connection.ConnectionString = CapstoneDBConnString;

            cmdCreation.CommandText = creationQuery;
            cmdCreation.Parameters.AddWithValue("@FirstName", firstName);
            cmdCreation.Parameters.AddWithValue("@LastName", lastName);
            cmdCreation.Parameters.AddWithValue("@Email", email);
            cmdCreation.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmdCreation.Parameters.AddWithValue("@Username", username);
            cmdCreation.Parameters.AddWithValue("@AllergyNote", allergyNote);
            cmdCreation.Parameters.AddWithValue("@Accessibility", accessibility);

            cmdCreation.Connection.Open();

            cmdCreation.ExecuteNonQuery();
        }

        // Creation of hashed credentials corresponding to the newly created User account.
        // A stored procedure is used in this method.
        public static void CreateHashedUser(string Username, string Password)
        {
            string loginQuery = "INSERT INTO HashedCredentials (Username, HashedPassword) values (@Username, @HashedPassword)";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = DBConnection;
            cmdLogin.Connection.ConnectionString = AuthDBConnString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.Parameters.AddWithValue("@HashedPassword", PasswordHash.HashPassword(Password));

            cmdLogin.Connection.Open();

            cmdLogin.ExecuteNonQuery();
        }

        // User login that uses the hashed password to compare
        // A stored procedure is used in this method.
        public static bool HashedParameterLogin(string Username, string Password)
        {
            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = new SqlConnection();
            cmdLogin.Connection.ConnectionString = AuthDBConnString;
            cmdLogin.CommandType = System.Data.CommandType.StoredProcedure;
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.CommandText = "sp_Login";
            cmdLogin.Connection.Open();

            SqlDataReader hashReader = cmdLogin.ExecuteReader();
            if (hashReader.Read())
            {
                string correctHash = hashReader["HashedPassword"].ToString();

                if (PasswordHash.ValidatePassword(Password, correctHash))
                {
                    return true;
                }
            }

            return false;
        }

        public static string EncryptedPasswordReader(int UserID)
        {
            string passwordQuery = "SELECT HashedPassword FROM HashedCredentials WHERE UserID = @UserID";
            SqlCommand cmdPassword = new SqlCommand();
            cmdPassword.Connection = DBConnection;
            cmdPassword.Connection.ConnectionString = AuthDBConnString;
            cmdPassword.CommandText = passwordQuery;

            cmdPassword.Parameters.AddWithValue("@UserID", UserID);

            cmdPassword.Connection.Open();

            string password = "";

            SqlDataReader passwordReader = cmdPassword.ExecuteReader();
            if (passwordReader.Read())
            {
                password = passwordReader["HashedPassword"].ToString();
            }

            return password;
        }

        // This Query is mainly used for interactions to the AUTH database
        public static void AuthGeneralQuery(string sqlQuery)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = DBConnection;
            cmdProductRead.Connection.ConnectionString = AuthDBConnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            cmdProductRead.ExecuteNonQuery();
        }

    }
}
