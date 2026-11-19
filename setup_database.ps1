# CyberArena Database Setup Script
# This script creates the CyberArena database in LocalDB and applies the schema

# Get the SQL script content
$sqlScriptPath = "App_Data\CreateDatabase_Complete.sql"
$connectionString = "Server=(LocalDb)\MSSQLLocalDB;Integrated Security=true;"

Write-Host "================================"
Write-Host "CyberArena Database Setup"
Write-Host "================================"
Write-Host ""

# Check if SQL script exists
if (-not (Test-Path $sqlScriptPath)) {
	Write-Host "ERROR: SQL script not found at $sqlScriptPath"
	exit 1
}

# Read the SQL script
Write-Host "Reading SQL schema from: $sqlScriptPath"
$sqlContent = Get-Content -Path $sqlScriptPath -Raw

# First, create a master connection to create the database
Write-Host "Connecting to LocalDB server..."
$masterConnectionString = $connectionString

try {
	# Check if database already exists
	$checkDbQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = 'CyberArena'"
	$connection = New-Object System.Data.SqlClient.SqlConnection($masterConnectionString)
	$connection.Open()

	$command = New-Object System.Data.SqlClient.SqlCommand($checkDbQuery, $connection)
	$result = $command.ExecuteScalar()

	if ($result -gt 0) {
		Write-Host "Database 'CyberArena' already exists. Proceeding to update schema..."
	} else {
		Write-Host "Creating database 'CyberArena'..."
		$createDbQuery = "CREATE DATABASE CyberArena"
		$command = New-Object System.Data.SqlClient.SqlCommand($createDbQuery, $connection)
		$command.ExecuteNonQuery()
		Write-Host "✓ Database created successfully"
	}

	$connection.Close()
}
catch {
	Write-Host "ERROR creating database: $_"
	exit 1
}

# Now connect to the CyberArena database and execute the schema script
Write-Host ""
Write-Host "Applying schema and seed data..."

$dbConnectionString = "Server=(LocalDb)\MSSQLLocalDB;Initial Catalog=CyberArena;Integrated Security=true;"

try {
	$connection = New-Object System.Data.SqlClient.SqlConnection($dbConnectionString)
	$connection.Open()

	# Split the SQL script by GO statements
	$sqlScripts = $sqlContent -split "\nGO\n" | Where-Object { $_.Trim() -ne "" }

	foreach ($script in $sqlScripts) {
		if ($script.Trim() -ne "") {
			$command = New-Object System.Data.SqlClient.SqlCommand($script, $connection)
			$command.CommandTimeout = 300
			$command.ExecuteNonQuery()
		}
	}

	$connection.Close()
	Write-Host "✓ Schema and seed data applied successfully"
}
catch {
	Write-Host "ERROR applying schema: $_"
	exit 1
}

Write-Host ""
Write-Host "================================"
Write-Host "Setup Complete!"
Write-Host "================================"
Write-Host "Database: CyberArena"
Write-Host "Server: (LocalDb)\MSSQLLocalDB"
Write-Host "Connection String: $dbConnectionString"
Write-Host ""
Write-Host "You can now use CRUD operations via the application."
Write-Host "================================"
