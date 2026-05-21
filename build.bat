@echo off
echo ========================================
echo Multi-Database Script Deployer Builder
echo ========================================
echo.

REM Check if .NET 9 is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)

echo Step 1: Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo.
echo Step 2: Building application (Release configuration)...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo Step 3: Publishing self-contained executable...
dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false
if %errorlevel% neq 0 (
    echo ERROR: Publish failed
    pause
    exit /b 1
)

echo.
echo ========================================
echo BUILD SUCCESSFUL!
echo ========================================
echo.
echo Executable location:
echo bin\Release\net9.0-windows\win-x64\publish\MultiDbScriptDeployer.exe
echo.
echo You can now run the application!
echo.
pause
