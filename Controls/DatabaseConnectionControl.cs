using System;
using System.Drawing;
using System.Windows.Forms;
using MultiDbScriptDeployer.Models;

namespace MultiDbScriptDeployer.Controls
{
    public class DatabaseConnectionControl : UserControl
    {
        private TextBox txtServerName;
        private TextBox txtServer;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtDatabase;
        private Button btnRemove;
        private Button btnTest;
        private Label lblServerName;
        private Label lblServer;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblDatabase;
        private Panel pnlStatus;

        public event EventHandler RemoveClicked;
        public DatabaseConnection Connection { get; private set; }

        public DatabaseConnectionControl()
        {
            Connection = new DatabaseConnection();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Set control properties
            this.Height = 140;  // CHANGED from 110
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Padding = new Padding(10);
            this.BackColor = Color.White;

            // Server Name Label and TextBox (Optional)
            lblServerName = new Label
            {
                Text = "Name:",
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            txtServerName = new TextBox
            {
                Location = new Point(100, 8),
                Width = 250,
                PlaceholderText = "(Optional) Friendly name for this server"
            };
            txtServerName.TextChanged += (s, e) => Connection.ServerName = txtServerName.Text;


            // Server Label and TextBox
            lblServer = new Label
            {
                Text = "Server:",
                Location = new Point(10, 38),
                AutoSize = true
            };

            txtServer = new TextBox
            {
                Location = new Point(100, 36),
                Width = 250,
                PlaceholderText = "server.database.windows.net or localhost"
            };
            txtServer.TextChanged += (s, e) => Connection.Server = txtServer.Text;

            // Username Label and TextBox
            lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(10, 66),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(100, 64),
                Width = 250,
                PlaceholderText = "SQL Server username"
            };
            txtUsername.TextChanged += (s, e) => Connection.Username = txtUsername.Text;

            // Password Label and TextBox
            lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(10, 94),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(100, 92),
                Width = 250,
                UseSystemPasswordChar = true,
                PlaceholderText = "SQL Server password"
            };
            txtPassword.TextChanged += (s, e) => Connection.Password = txtPassword.Text;

            // Database Label and TextBox
            lblDatabase = new Label
            {
                Text = "Database:",
                Location = new Point(370, 38),
                AutoSize = true
            };

            txtDatabase = new TextBox
            {
                Location = new Point(450, 36),
                Width = 150,
                Text = "SMSCPhoenix",
                PlaceholderText = "Database name"
            };
            txtDatabase.TextChanged += (s, e) => Connection.Database = txtDatabase.Text;

            // Test Button
            btnTest = new Button
            {
                Text = "Test",
                Location = new Point(370, 64),
                Width = 80,
                Height = 25
            };
            btnTest.Click += BtnTest_Click;

            // Remove Button
            btnRemove = new Button
            {
                Text = "Remove",
                Location = new Point(460, 64),
                Width = 80,
                Height = 25,
                BackColor = Color.LightCoral
            };
            btnRemove.Click += (s, e) => RemoveClicked?.Invoke(this, EventArgs.Empty);

            // Status Panel
            pnlStatus = new Panel
            {
                Location = new Point(370, 94),
                Width = 230,
                Height = 25,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            // Add controls
            this.Controls.AddRange(new Control[]
            {
                lblServerName, txtServerName,
                lblServer, txtServer,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblDatabase, txtDatabase,
                btnTest, btnRemove, pnlStatus
            });

            this.ResumeLayout();
        }

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            if (!Connection.IsValid())
            {
                ShowStatus("Please fill in all fields", Color.Orange);
                return;
            }

            btnTest.Enabled = false;
            btnTest.Text = "Testing...";
            ShowStatus("Testing connection...", Color.LightBlue);

            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var conn = new System.Data.SqlClient.SqlConnection(Connection.GetConnectionString()))
                    {
                        conn.Open();
                    }
                });

                ShowStatus("✓ Connection successful", Color.LightGreen);
            }
            catch (Exception ex)
            {
                ShowStatus($"✗ Failed: {ex.Message}", Color.LightCoral);
            }
            finally
            {
                btnTest.Enabled = true;
                btnTest.Text = "Test";
            }
        }

        private void ShowStatus(string message, Color color)
        {
            pnlStatus.BackColor = color;
            pnlStatus.Visible = true;

            // Remove existing label if any
            pnlStatus.Controls.Clear();

            var label = new Label
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 8)
            };
            pnlStatus.Controls.Add(label);
        }

        public void SetConnection(DatabaseConnection connection)
        {
            Connection = connection;
            txtServerName.Text = connection.ServerName;
            txtServer.Text = connection.Server;
            txtUsername.Text = connection.Username;
            txtPassword.Text = connection.Password;
            txtDatabase.Text = connection.Database;
        }
    }
}
