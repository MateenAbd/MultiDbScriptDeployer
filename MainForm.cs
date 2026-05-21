using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MultiDbScriptDeployer.Controls;
using MultiDbScriptDeployer.Models;
using MultiDbScriptDeployer.Services;

namespace MultiDbScriptDeployer
{
    public class MainForm : Form
    {
        private Panel pnlConnections;
        private Button btnAddConnection;
        private Button btnTestAll;
        private TextBox txtScript;
        private Button btnDeploy;
        private Button btnLoadScript;
        private Button btnSaveConnections;
        private Button btnLoadConnections;
        private RichTextBox txtLog;
        private Button btnClearLog;
        private Button btnOpenLogFile;
        private ProgressBar progressBar;
        private Label lblConnectionsTitle;
        private Label lblScriptTitle;
        private Label lblLogTitle;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        private Logger _logger;
        private DeploymentService _deploymentService;
        private List<DatabaseConnectionControl> _connectionControls;

        public MainForm()
        {
            _logger = new Logger();
            _deploymentService = new DeploymentService(_logger);
            _connectionControls = new List<DatabaseConnectionControl>();

            InitializeComponents();

            _logger.LogAdded += Logger_LogAdded;
            _deploymentService.ProgressChanged += DeploymentService_ProgressChanged;
            _logger.LogInfo("Application started");
        }

        private void InitializeComponents()
        {
            // Form properties
            this.Text = "Multi-Database Script Deployer - .NET 9";
            this.Size = new Size(1200, 800);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Connections Panel (Top Left)
            lblConnectionsTitle = new Label
            {
                Text = "Database Connections",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
            };

            pnlConnections = new Panel
            {
                Location = new Point(10, 35),
                Size = new Size(650, 250),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke
            };

            btnAddConnection = new Button
            {
                Text = "+ Add Database Connection",
                Location = new Point(10, 290),
                Size = new Size(200, 30),
                BackColor = Color.LightGreen
            };
            btnAddConnection.Click += BtnAddConnection_Click;

            btnSaveConnections = new Button
            {
                Text = "Save Connections",
                Location = new Point(220, 290),
                Size = new Size(140, 30)
            };
            btnSaveConnections.Click += BtnSaveConnections_Click;

            btnLoadConnections = new Button
            {
                Text = "Load Connections",
                Location = new Point(370, 290),
                Size = new Size(140, 30)
            };
            btnLoadConnections.Click += BtnLoadConnections_Click;

            btnTestAll = new Button
            {
                Text = "Test All Connections",
                Location = new Point(520, 290),
                Size = new Size(140, 30),
                BackColor = Color.LightSkyBlue
            };
            btnTestAll.Click += BtnTestAll_Click;

            // Script Panel (Top Right)
            lblScriptTitle = new Label
            {
                Text = "SQL Script",
                Location = new Point(670, 10),
                AutoSize = true,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
            };

            txtScript = new TextBox
            {
                Location = new Point(670, 35),
                Size = new Size(500, 250),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9),
                PlaceholderText = "Enter your SQL script here...",
                MaxLength = 0
            };

            btnLoadScript = new Button
            {
                Text = "Load from File",
                Location = new Point(670, 290),
                Size = new Size(120, 30)
            };
            btnLoadScript.Click += BtnLoadScript_Click;

            btnDeploy = new Button
            {
                Text = "▶ DEPLOY SCRIPT",
                Location = new Point(800, 290),
                Size = new Size(370, 30),
                BackColor = Color.LightBlue,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
            };
            btnDeploy.Click += BtnDeploy_Click;

            // Progress Bar
            progressBar = new ProgressBar
            {
                Location = new Point(10, 330),
                Size = new Size(1160, 20),
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100,
                Visible = false
            };

            // Log Panel (Bottom)
            lblLogTitle = new Label
            {
                Text = "Deployment Log",
                Location = new Point(10, 360),
                AutoSize = true,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
            };

            txtLog = new RichTextBox
            {
                Location = new Point(10, 385),
                Size = new Size(1160, 300),
                ReadOnly = true,
                Font = new Font("Consolas", 8),
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnClearLog = new Button
            {
                Text = "Clear Log",
                Location = new Point(10, 690),
                Size = new Size(100, 25)
            };
            btnClearLog.Click += (s, e) => txtLog.Clear();

            btnOpenLogFile = new Button
            {
                Text = "Open Log File",
                Location = new Point(120, 690),
                Size = new Size(120, 25)
            };
            btnOpenLogFile.Click += BtnOpenLogFile_Click;

            // Status Strip
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready");
            statusStrip.Items.Add(statusLabel);

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                lblConnectionsTitle, pnlConnections,
                btnAddConnection, btnSaveConnections, btnLoadConnections, btnTestAll,
                lblScriptTitle, txtScript, btnLoadScript, btnDeploy,
                progressBar,
                lblLogTitle, txtLog, btnClearLog, btnOpenLogFile,
                statusStrip
            });

            // Add initial connection control
            AddConnectionControl();
        }

        private void AddConnectionControl()
        {
            var control = new DatabaseConnectionControl
            {
                Location = new Point(5, 5 + (_connectionControls.Count * 145)),
                Width = pnlConnections.Width - 25
            };
            control.RemoveClicked += ConnectionControl_RemoveClicked;

            _connectionControls.Add(control);
            pnlConnections.Controls.Add(control);

            UpdateConnectionsLayout();
        }

        private void ConnectionControl_RemoveClicked(object sender, EventArgs e)
        {
            var control = sender as DatabaseConnectionControl;
            if (control != null && _connectionControls.Count > 1)
            {
                _connectionControls.Remove(control);
                pnlConnections.Controls.Remove(control);
                UpdateConnectionsLayout();
            }
            else
            {
                MessageBox.Show("You must have at least one connection.", "Cannot Remove",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateConnectionsLayout()
        {
            for (int i = 0; i < _connectionControls.Count; i++)
            {
                _connectionControls[i].Location = new Point(5, 5 + (i * 145));
            }
        }

        private void BtnAddConnection_Click(object sender, EventArgs e)
        {
            AddConnectionControl();
            _logger.LogInfo("New connection control added");
        }

        private void BtnLoadScript_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQL Files (*.sql)|*.sql|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.Title = "Select SQL Script File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        txtScript.Text = File.ReadAllText(openFileDialog.FileName);
                        _logger.LogInfo($"Script loaded from: {openFileDialog.FileName}");
                        statusLabel.Text = $"Loaded: {Path.GetFileName(openFileDialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _logger.LogError("Failed to load script file", ex);
                    }
                }
            }
        }

        private async void BtnDeploy_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtScript.Text))
            {
                MessageBox.Show("Please enter a SQL script to deploy.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var connections = _connectionControls.Select(c => c.Connection).ToList();
            var invalidConnections = connections.Where(c => !c.IsValid()).ToList();

            if (invalidConnections.Any())
            {
                MessageBox.Show("Please fill in all connection details for all database connections.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm
            var result = MessageBox.Show(
                $"Are you sure you want to deploy this script to {connections.Count} database(s)?",
                "Confirm Deployment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Disable controls
            SetControlsEnabled(false);
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0; 
            progressBar.Maximum = connections.Count;
            statusLabel.Text = "Deployment in progress...";

            try
            {
                var deploymentResult = await _deploymentService.DeployScriptAsync(connections, txtScript.Text);

                // Show summary
                string message = $"Deployment Completed!\n\n" +
                                $"Total: {deploymentResult.TotalConnections}\n" +
                                $"Successful: {deploymentResult.SuccessfulDeployments}\n" +
                                $"Failed: {deploymentResult.FailedDeployments}\n" +
                                $"Duration: {deploymentResult.Duration.TotalSeconds:F2} seconds";

                MessageBox.Show(message, "Deployment Complete",
                    MessageBoxButtons.OK,
                    deploymentResult.AllSuccessful ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                statusLabel.Text = deploymentResult.AllSuccessful ?
                    "Deployment completed successfully" :
                    "Deployment completed with errors";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Deployment failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError("Deployment process failed", ex);
                statusLabel.Text = "Deployment failed";
            }
            finally
            {
                SetControlsEnabled(true);
                progressBar.Visible = false;
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            btnAddConnection.Enabled = enabled;
            btnTestAll.Enabled = enabled;
            btnDeploy.Enabled = enabled;
            btnLoadScript.Enabled = enabled;
            btnSaveConnections.Enabled = enabled;
            btnLoadConnections.Enabled = enabled;
            txtScript.ReadOnly = !enabled;

            foreach (var control in _connectionControls)
            {
                control.Enabled = enabled;
            }
        }

        private void Logger_LogAdded(object sender, LogEntry logEntry)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Logger_LogAdded(sender, logEntry)));
                return;
            }

            // Set color based on log level
            Color textColor = logEntry.Level switch
            {
                LogLevel.Error => Color.Red,
                LogLevel.Success => Color.LightGreen,
                LogLevel.Warning => Color.Yellow,
                LogLevel.Info => Color.White,
                _ => Color.White
            };

            // Append colored text
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = textColor;
            txtLog.AppendText(logEntry.Message + Environment.NewLine);
            txtLog.SelectionColor = txtLog.ForeColor; // Reset to default
            txtLog.ScrollToCaret();
        }

        private void BtnOpenLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                string logFilePath = _logger.GetLogFilePath();
                if (File.Exists(logFilePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = logFilePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Log file not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening log file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTestAll_Click(object sender, EventArgs e)
        {
            if (_connectionControls.Count == 0)
            {
                MessageBox.Show("No connections to test.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Disable button during testing
            btnTestAll.Enabled = false;
            btnTestAll.Text = "Testing...";
            statusLabel.Text = "Testing all connections...";

            int successCount = 0;
            int failCount = 0;
            int totalConnections = _connectionControls.Count;

            _logger.LogInfo("========================================");
            _logger.LogInfo("TESTING ALL CONNECTIONS");
            _logger.LogInfo($"Total connections: {totalConnections}");
            _logger.LogInfo("========================================");

            foreach (var control in _connectionControls)
            {
                var connection = control.Connection;

                if (!connection.IsValid())
                {
                    _logger.LogWarning($"Skipping invalid connection: {connection}");
                    failCount++;
                    continue;
                }

                _logger.LogInfo($"Testing: {connection}");

                try
                {
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        using (var conn = new System.Data.SqlClient.SqlConnection(connection.GetConnectionString()))
                        {
                            conn.Open();
                        }
                    });

                    successCount++;
                    _logger.LogSuccess($"✓ Connection successful: {connection}");
                }
                catch (Exception ex)
                {
                    failCount++;
                    _logger.LogError($"✗ Connection failed: {connection}", ex);
                }
            }

            _logger.LogInfo("========================================");
            _logger.LogInfo("TEST ALL COMPLETED");
            _logger.LogSuccess($"Successful: {successCount}");
            _logger.LogError($"Failed: {failCount}");
            _logger.LogInfo("========================================");

            // Show summary
            string message = $"Connection Test Results:\n\n" +
                            $"Total: {totalConnections}\n" +
                            $"Successful: {successCount}\n" +
                            $"Failed: {failCount}";

            MessageBox.Show(message, "Test All Connections",
                MessageBoxButtons.OK,
                failCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            // Restore button
            btnTestAll.Enabled = true;
            btnTestAll.Text = "Test All Connections";
            statusLabel.Text = failCount == 0 ?
                "All connections tested successfully" :
                $"Connection testing completed - {failCount} failed";
        }

        private void BtnSaveConnections_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON Files (*.json)|*.json";
                saveFileDialog.Title = "Save Connections";
                saveFileDialog.FileName = "connections.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var connections = _connectionControls.Select(c => c.Connection).ToList();
                        string json = System.Text.Json.JsonSerializer.Serialize(connections, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(saveFileDialog.FileName, json);

                        MessageBox.Show("Connections saved successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _logger.LogInfo($"Connections saved to: {saveFileDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving connections: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _logger.LogError("Failed to save connections", ex);
                    }
                }
            }
        }

        private void BtnLoadConnections_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                openFileDialog.Title = "Load Connections";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(openFileDialog.FileName);
                        var connections = System.Text.Json.JsonSerializer.Deserialize<List<DatabaseConnection>>(json);

                        if (connections != null && connections.Any())
                        {
                            // Clear existing controls
                            foreach (var control in _connectionControls.ToList())
                            {
                                pnlConnections.Controls.Remove(control);
                            }
                            _connectionControls.Clear();

                            // Add loaded connections
                            foreach (var connection in connections)
                            {
                                var control = new DatabaseConnectionControl
                                {
                                    Location = new Point(5, 5 + (_connectionControls.Count * 145)),
                                    Width = pnlConnections.Width - 25
                                };
                                control.SetConnection(connection);
                                control.RemoveClicked += ConnectionControl_RemoveClicked;

                                _connectionControls.Add(control);
                                pnlConnections.Controls.Add(control);
                            }

                            MessageBox.Show($"Loaded {connections.Count} connection(s) successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _logger.LogInfo($"Connections loaded from: {openFileDialog.FileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading connections: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _logger.LogError("Failed to load connections", ex);
                    }
                }
            }
        }
        private void DeploymentService_ProgressChanged(object sender, DeploymentProgress progress)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => DeploymentService_ProgressChanged(sender, progress)));
                return;
            }

            // Update progress bar
            progressBar.Value = progress.CurrentIndex;

            // Update status label
            if (progress.IsComplete)
            {
                statusLabel.Text = "Deployment completed";
            }
            else
            {
                statusLabel.Text = $"Deploying to server {progress.CurrentIndex}/{progress.TotalCount}: {progress.CurrentServer}";
            }
        }
    }
}
