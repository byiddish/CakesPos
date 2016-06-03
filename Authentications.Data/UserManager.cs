using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentications.Data
{
    public class UserManager
    {
        private readonly string _connectionString;

        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(string userName, string password, string name)
        {
            User user = new User();
            string salt = PasswordHelper.GenerateRandomSalt();
            user.PasswordHash = PasswordHelper.HashPassword(password, salt);
            user.Salt = salt;
            user.UserName = userName;
            user.Name = name;

            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO Users (Username, Name, PasswordHash, Salt) VALUES (@username, @name, @password, @salt)";
                cmd.Parameters.AddWithValue("@username", user.UserName);
                cmd.Parameters.AddWithValue("@password", user.PasswordHash);
                cmd.Parameters.AddWithValue("@salt", user.Salt);
                cmd.Parameters.AddWithValue("@name", user.Name);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public User GetUser(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Username = @username";
                cmd.Parameters.AddWithValue("@username", username);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null; //username not found....
                }

                User user = new User();
                user.Id = (int)reader["Id"];
                user.PasswordHash = (string)reader["PasswordHash"];
                user.Salt = (string)reader["Salt"];
                user.UserName = (string)reader["UserName"];
                user.Name = (string)reader["Name"];

                bool success = PasswordHelper.PasswordMatch(password, user.PasswordHash, user.Salt);
                return success ? user : null;
            }
        }

        public User GetUserByUserName(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Username = @username";
                cmd.Parameters.AddWithValue("@username", username);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null; //username not found....
                }

                User user = new User();
                user.Id = (int)reader["Id"];
                user.PasswordHash = (string)reader["PasswordHash"];
                user.Salt = (string)reader["Salt"];
                user.UserName = (string)reader["UserName"];
                user.Name = (string)reader["Name"];

                return user;
            }
        }

    }
}
