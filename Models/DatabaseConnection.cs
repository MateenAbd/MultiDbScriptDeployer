using System;

namespace MultiDbScriptDeployer.Models
{
    public class DatabaseConnection
    {
        //public Guid Id { get; set; }
        public string ServerName { get; set; }
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public DatabaseConnection()
        {
            //Id = Guid.NewGuid();
            ServerName = string.Empty;
            Server = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Database = "SMSCPhoenix"; // Default database
        }

        public string GetConnectionString()
        {
            return $"Server={Server};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=True;Connection Timeout=3600;";
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Server) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(ServerName))
                return $"{ServerName} - {Server} ({Username})";
            return $"{Server} ({Username})";
        }
    }
}
