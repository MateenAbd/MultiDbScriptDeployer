using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MultiDbScriptDeployer.Models;

namespace MultiDbScriptDeployer.Services
{
    public class DeploymentService
    {
        private readonly Logger _logger;
        public event EventHandler<DeploymentProgress> ProgressChanged;

        public DeploymentService(Logger logger)
        {
            _logger = logger;
        }

        public async Task<DeploymentResult> DeployScriptAsync(List<DatabaseConnection> connections, string script)
        {
            var result = new DeploymentResult
            {
                StartTime = DateTime.Now,
                TotalConnections = connections.Count
            };

            _logger.LogInfo("========================================");
            _logger.LogInfo("DEPLOYMENT STARTED");
            _logger.LogInfo($"Total databases: {connections.Count}");
            _logger.LogInfo($"Script length: {script.Length} characters");
            _logger.LogInfo("========================================");

            int currentIndex = 0;

            foreach (var connection in connections)
            {
                currentIndex++;

                // Report progress
                ProgressChanged?.Invoke(this, new DeploymentProgress
                {
                    CurrentIndex = currentIndex,
                    TotalCount = connections.Count,
                    CurrentServer = connection.ToString(),
                    IsComplete = false
                });

                try
                {
                    _logger.LogInfo($"Processing [{currentIndex}/{connections.Count}]: {connection}");
                    await ExecuteScriptAsync(connection, script);

                    result.SuccessfulDeployments++;
                    _logger.LogSuccess($"✓ Successfully deployed to {connection}");
                }
                catch (Exception ex)
                {
                    result.FailedDeployments++;
                    _logger.LogError($"✗ Failed to deploy to {connection}", ex);
                }
                finally
                {
                    _logger.LogInfo("----------------------------------------");
                }
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;

            _logger.LogInfo("========================================");
            _logger.LogInfo("DEPLOYMENT COMPLETED");
            _logger.LogInfo($"Total: {result.TotalConnections}");
            _logger.LogSuccess($"Success: {result.SuccessfulDeployments}");
            _logger.LogError($"Failed: {result.FailedDeployments}");
            _logger.LogInfo($"Duration: {result.Duration.TotalSeconds:F2} seconds");
            _logger.LogInfo("========================================");

            ProgressChanged?.Invoke(this, new DeploymentProgress
            {
                CurrentIndex = connections.Count,
                TotalCount = connections.Count,
                CurrentServer = "Completed",
                IsComplete = true
            });

            return result;
        }

        private async Task ExecuteScriptAsync(DatabaseConnection connection, string script)
        {
            if (!connection.IsValid())
            {
                throw new InvalidOperationException("Invalid connection parameters");
            }

            string connectionString = connection.GetConnectionString();
            
            _logger.LogInfo($"Connecting to {connection.Server}...");

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                _logger.LogInfo("Connection established");

                // Split script by GO statements
                var batches = SplitScriptIntoBatches(script);
                _logger.LogInfo($"Script split into {batches.Count} batch(es)");

                int batchNumber = 1;
                foreach (var batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch))
                        continue;

                    _logger.LogInfo($"Executing batch {batchNumber}/{batches.Count}...");

                    using (var command = new SqlCommand(batch, sqlConnection))
                    {
                        command.CommandTimeout = 3600;
                        
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        _logger.LogInfo($"Batch {batchNumber} completed. Rows affected: {rowsAffected}");
                    }

                    batchNumber++;
                }

                _logger.LogInfo("All batches executed successfully");
            }
        }

        private List<string> SplitScriptIntoBatches(string script)
        {
            // Split by GO statements (case-insensitive, must be on its own line)
            var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                               .Where(b => !string.IsNullOrWhiteSpace(b))
                               .Select(b => b.Trim())
                               .ToList();

            return batches;
        }
    }

    public class DeploymentResult
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int TotalConnections { get; set; }
        public int SuccessfulDeployments { get; set; }
        public int FailedDeployments { get; set; }

        public bool AllSuccessful => FailedDeployments == 0 && TotalConnections > 0;
    }

    public class DeploymentProgress
    {
        public int CurrentIndex { get; set; }
        public int TotalCount { get; set; }
        public string CurrentServer { get; set; }
        public bool IsComplete { get; set; }

        public int PercentComplete => TotalCount > 0 ? (CurrentIndex * 100) / TotalCount : 0;
    }
}
