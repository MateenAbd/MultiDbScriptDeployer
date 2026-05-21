-- Sample SQL Script for Testing Multi-Database Deployment
-- This script creates a deployment log table and inserts a test record

-- Create deployment log table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeploymentLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DeploymentLog] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [DeploymentDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ScriptName] NVARCHAR(255) NOT NULL,
        [DeployedBy] NVARCHAR(100) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [Comments] NVARCHAR(MAX) NULL
    )
    PRINT 'DeploymentLog table created successfully'
END
ELSE
BEGIN
    PRINT 'DeploymentLog table already exists'
END
GO

-- Insert a test deployment record
INSERT INTO [dbo].[DeploymentLog] (ScriptName, DeployedBy, Status, Comments)
VALUES ('SampleScript.sql', SYSTEM_USER, 'Success', 'Test deployment from Multi-Database Script Deployer')
GO

-- Display the latest deployment records
SELECT TOP 5 * 
FROM [dbo].[DeploymentLog] 
ORDER BY DeploymentDate DESC
GO

PRINT 'Sample script executed successfully!'
GO
