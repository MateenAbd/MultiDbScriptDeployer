# Quick Start Guide

## Prerequisites
1. Install .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0
2. Ensure you have access to SQL Server instances

## Building the Application

### Option 1: Using the Build Script (Easiest)
1. Open Command Prompt or PowerShell
2. Navigate to the project folder
3. Run: `build.bat`
4. The executable will be created in: `bin\Release\net9.0-windows\win-x64\publish\`

### Option 2: Manual Build
```batch
dotnet restore
dotnet build --configuration Release
dotnet run
```

### Option 3: Create Standalone Executable
```batch
dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true
```

## First Time Usage

### Step 1: Launch the Application
- Run `MultiDbScriptDeployer.exe` from the publish folder
- Or use `dotnet run` from the project directory

### Step 2: Add Database Connections
1. The application starts with one connection panel
2. Fill in:
   - **Server**: Your SQL Server address (e.g., `localhost`, `(local)`, or `server.database.windows.net`)
   - **Username**: SQL authentication username (e.g., `sa`)
   - **Password**: SQL authentication password
   - **Database**: Target database name (e.g., `master`, `TestDB`)
3. Click **Test** button to verify connection
4. Click **+ Add Database Connection** to add more databases

### Step 3: Load or Enter SQL Script
**Option A: Load from file**
- Click **Load from File**
- Select your .sql file
- A sample script is provided: `SampleScript.sql`

**Option B: Type/Paste directly**
- Type or paste SQL script into the text box
- Scripts can include GO statements for batching

### Step 4: Deploy
1. Click the **▶ DEPLOY SCRIPT** button
2. Confirm the deployment
3. Watch the log window for real-time progress
4. Review the summary when complete

## Sample Test

1. **Use the provided sample files:**
   - Load `sample_connections.json` (Click "Load Connections")
   - Update the passwords in each connection
   - Load `SampleScript.sql` (Click "Load from File")
   - Click **Test** on each connection
   - Click **▶ DEPLOY SCRIPT**

2. **The sample script will:**
   - Create a DeploymentLog table
   - Insert a test record
   - Display recent deployments

## Tips

### Connection Testing
- Always test connections before deploying
- Green status = Connection successful
- Red status = Connection failed (check credentials and network)

### Save/Load Connections
- Save frequently used connections using **Save Connections**
- Load saved configurations using **Load Connections**
- **⚠️ Warning**: Passwords are saved in plain text - keep files secure!

### Log Files
- Logs are automatically saved to the `Logs` folder
- Each deployment creates a new timestamped log file
- Click **Open Log File** to view the current log in your text editor

### Script Writing
- Use GO statements to separate batches
- Include error handling in your scripts
- Test scripts on a development database first

## Common Connection Strings

### Local SQL Server (Windows Authentication)
*Note: This app uses SQL Authentication, so you need to enable mixed mode authentication*

### Local SQL Server (SQL Authentication)
- Server: `localhost` or `(local)` or `.`
- Username: `sa` (or your SQL user)
- Password: Your SQL password
- Database: `master` or your database name

### Azure SQL Database
- Server: `yourserver.database.windows.net`
- Username: `sqladmin@yourserver`
- Password: Your Azure SQL password
- Database: Your database name

### SQL Server with Instance Name
- Server: `localhost\SQLEXPRESS`
- Username: `sa`
- Password: Your password
- Database: Your database name

### SQL Server with Port
- Server: `192.168.1.100,1433`
- Username: `sa`
- Password: Your password
- Database: Your database name

## Troubleshooting

### Cannot connect to SQL Server
1. Check if SQL Server is running
2. Verify SQL Server Authentication is enabled (not just Windows Auth)
3. Check firewall allows SQL Server port (default: 1433)
4. Verify credentials are correct

### Script execution fails
1. Check the log window for specific error messages
2. Verify the database user has appropriate permissions
3. Test the script manually in SSMS first
4. Check for syntax errors in the script

### Application won't start
1. Ensure .NET 9 is installed: Run `dotnet --version` in command prompt
2. Try running from command line to see error messages: `dotnet run`
3. Check if any antivirus is blocking the application

## Next Steps

1. **Test with sample data**: Use the provided sample files
2. **Create your own scripts**: Write deployment scripts for your databases
3. **Save connection profiles**: Create different connection files for dev/test/prod
4. **Review logs**: Check the Logs folder after deployments

## Security Reminders

- Never commit connection files with real passwords to source control
- Use service accounts with minimal required permissions
- Test scripts in development environment first
- Always backup databases before running deployment scripts
- Store connection JSON files securely

## Need Help?

Check the main README.md file for comprehensive documentation and troubleshooting tips.
