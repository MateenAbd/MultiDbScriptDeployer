# Multi-Database Script Deployer

A .NET 9 Windows Forms application for deploying SQL scripts to multiple SQL Server databases simultaneously with comprehensive logging.

## Features

- **Multiple Database Connections**: Add unlimited database connections with individual server, username, password, and database settings
- **Connection Testing**: Test each connection before deployment
- **Script Management**: Load SQL scripts from files or paste directly
- **Batch Execution**: Automatically handles GO statements and executes scripts in batches
- **Comprehensive Logging**: 
  - Real-time log display in the application
  - Persistent log files saved to disk (Logs folder)
  - Detailed error tracking with stack traces
- **Connection Management**: Save and load connection configurations as JSON files
- **Progress Tracking**: Visual progress indicators during deployment
- **Error Handling**: Continues deployment even if one connection fails

## Prerequisites

- .NET 9 SDK (https://dotnet.microsoft.com/download/dotnet/9.0)
- Windows OS (Windows 10 or later recommended)
- SQL Server access with appropriate credentials

## Building the Application

1. Open a command prompt or PowerShell window
2. Navigate to the project directory:
   ```
   cd MultiDbScriptDeployer
   ```

3. Restore NuGet packages:
   ```
   dotnet restore
   ```

4. Build the application:
   ```
   dotnet build --configuration Release
   ```

5. Run the application:
   ```
   dotnet run
   ```

   Or navigate to the output folder and run the executable:
   ```
   cd bin\Release\net9.0-windows
   MultiDbScriptDeployer.exe
   ```

## Publishing as Standalone Executable

To create a self-contained executable that doesn't require .NET runtime to be installed:

```bash
dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false
```

The executable will be in: `bin\Release\net9.0-windows\win-x64\publish\`

## Usage

### Adding Database Connections

1. Click the **"+ Add Database Connection"** button to add a new connection panel
2. Fill in the connection details:
   - **Server**: SQL Server address (e.g., `localhost`, `server.database.windows.net`)
   - **Username**: SQL Server authentication username
   - **Password**: SQL Server authentication password
   - **Database**: Target database name (default: `master`)
3. Click **"Test"** to verify the connection
4. Repeat for all target databases

### Loading SQL Script

**Option 1: Load from File**
- Click **"Load from File"** button
- Select your .sql file

**Option 2: Paste Directly**
- Paste your SQL script directly into the text box

### Deploying Script

1. Ensure all connections are configured and tested
2. Verify your SQL script is loaded
3. Click the **"▶ DEPLOY SCRIPT"** button
4. Confirm the deployment in the dialog
5. Monitor progress in the log window

### Saving/Loading Connections

**Save Connections:**
- Click **"Save Connections"**
- Choose a location and filename
- Connections are saved as JSON

**Load Connections:**
- Click **"Load Connections"**
- Select a previously saved JSON file
- All connections will be restored

### Log Management

- **Clear Log**: Clears the on-screen log display
- **Open Log File**: Opens the persistent log file in your default text editor
- Log files are saved in the `Logs` subfolder with timestamps

## SQL Script Features

- **GO Statement Support**: Scripts are automatically split by GO statements
- **Batch Execution**: Each batch is executed separately
- **Transaction Handling**: Ensure your script includes appropriate transaction logic
- **Timeout**: 5-minute timeout per batch (configurable in code)

## Log File Location

Log files are automatically created in:
```
<Application Directory>\Logs\deployment_YYYYMMDD_HHMMSS.log
```

## Connection String Format

The application uses SQL Server Authentication with the following connection string format:
```
Server={Server};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=True;Connection Timeout=3600;
```

## Error Handling

- Each database deployment is independent
- If one deployment fails, the process continues to the next database
- All errors are logged with full stack traces
- Summary shows successful and failed deployments

## Security Notes

⚠️ **Important Security Considerations:**

1. **Credentials Storage**: Saved connection files contain passwords in plain text. Store them securely.
2. **Network Security**: Ensure proper network security between the application and SQL Servers
3. **SQL Injection**: This tool executes SQL directly - only use trusted scripts
4. **Least Privilege**: Use database accounts with minimum required permissions

## Example SQL Script

```sql
-- Create a table
CREATE TABLE TestTable (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE()
)
GO

-- Insert sample data
INSERT INTO TestTable (Name) VALUES ('Test 1')
INSERT INTO TestTable (Name) VALUES ('Test 2')
GO

-- Query the data
SELECT * FROM TestTable
GO
```

## Troubleshooting

### "Connection Failed" Errors
- Verify SQL Server is accessible from your network
- Check firewall rules allow SQL Server port (default: 1433)
- Confirm credentials are correct
- Ensure SQL Server Authentication is enabled (not just Windows Authentication)

### "Trust Server Certificate" Errors
- The application uses `TrustServerCertificate=True` by default
- For production, configure proper SSL certificates

### "Timeout Expired" Errors
- Increase the timeout in `DeploymentService.cs` (line: `command.CommandTimeout`)
- Optimize long-running scripts

## Project Structure

```
MultiDbScriptDeployer/
├── Program.cs                          # Application entry point
├── MainForm.cs                         # Main UI form
├── Models/
│   └── DatabaseConnection.cs          # Connection model
├── Services/
│   ├── Logger.cs                      # Logging service
│   └── DeploymentService.cs           # Deployment logic
├── Controls/
│   └── DatabaseConnectionControl.cs   # Connection UI control
└── Logs/                              # Log files (created at runtime)
```

## Version

- Application Version: 1.0.0
- .NET Version: 9.0
- Target Framework: net9.0-windows

## License

This is a utility application. Use at your own risk. Always test scripts in a development environment first.

## Support

For issues or questions, check the log files in the Logs folder for detailed error information.
