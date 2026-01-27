using Npgsql;

// Database initialization for example-web-api-with-database
// Following Zitadel's three-phase pattern (Phase 1: Init)
// Creates the database and service user using admin credentials

// Require all environment variables to be explicitly set
var requiredVars = new Dictionary<string, string?>
{
    { "POSTGRES_HOST", Environment.GetEnvironmentVariable("POSTGRES_HOST") },
    { "POSTGRES_PORT", Environment.GetEnvironmentVariable("POSTGRES_PORT") },
    { "POSTGRES_ADMIN_USER", Environment.GetEnvironmentVariable("POSTGRES_ADMIN_USER") },
    { "POSTGRES_ADMIN_PASSWORD", Environment.GetEnvironmentVariable("POSTGRES_ADMIN_PASSWORD") },
    { "COMMENTARY_DATABASE", Environment.GetEnvironmentVariable("COMMENTARY_DATABASE") },
    { "COMMENTARY_USER", Environment.GetEnvironmentVariable("COMMENTARY_USER") },
    { "COMMENTARY_PASSWORD", Environment.GetEnvironmentVariable("COMMENTARY_PASSWORD") }
};

// Assert all required environment variables are set
var missingVars = requiredVars
    .Where(kv => string.IsNullOrWhiteSpace(kv.Value))
    .Select(kv => kv.Key)
    .ToList();

if (missingVars.Any())
{
    Console.WriteLine($"ERROR: Missing required environment variables: {string.Join(", ", missingVars)}");
    Environment.Exit(1);
}

// Parse port
if (!int.TryParse(requiredVars["POSTGRES_PORT"], out var port) || port <= 0 || port > 65535)
{
    Console.WriteLine("ERROR: POSTGRES_PORT must be a valid integer between 1 and 65535");
    Environment.Exit(1);
}

var host = requiredVars["POSTGRES_HOST"]!;
var adminUser = requiredVars["POSTGRES_ADMIN_USER"]!;
var adminPassword = requiredVars["POSTGRES_ADMIN_PASSWORD"]!;
var dbName = requiredVars["COMMENTARY_DATABASE"]!;
var dbUser = requiredVars["COMMENTARY_USER"]!;
var dbPassword = requiredVars["COMMENTARY_PASSWORD"]!;
Console.WriteLine($"Configuration loaded:");


// Build admin connection string (connects to postgres database)
var adminConnectionString = $"Host={host};Port={port};Database=postgres;Username={adminUser};Password={adminPassword}";

Console.WriteLine("Waiting for PostgreSQL to be ready...");

// Wait for PostgreSQL to be ready
var maxRetries = 30;
var retryCount = 0;
while (retryCount < maxRetries)
{
    try
    {
        await using var testConn = new NpgsqlConnection(adminConnectionString);
        await testConn.OpenAsync();
        break;
    }
    catch (Exception)
    {
        retryCount++;
        if (retryCount >= maxRetries)
        {
            Console.WriteLine("ERROR: Could not connect to PostgreSQL after 30 retries");
            Environment.Exit(1);
        }
        await Task.Delay(1000);
    }
}

Console.WriteLine("PostgreSQL is ready. Initializing database...");

try
{
    // Connect as admin to create database and user
    await using var adminConn = new NpgsqlConnection(adminConnectionString);
    await adminConn.OpenAsync();

    // Create database if it doesn't exist
    var createDbCommand = new NpgsqlCommand(
        $"""
        SELECT 'CREATE DATABASE {dbName}'
        WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '{dbName}')
        """,
        adminConn);

    var createDbResult = await createDbCommand.ExecuteScalarAsync();
    if (createDbResult != null && createDbResult.ToString()!.StartsWith("CREATE DATABASE"))
    {
        var createDbSql = createDbResult.ToString()!;
        await using var createDbCmd = new NpgsqlCommand(createDbSql, adminConn);
        await createDbCmd.ExecuteNonQueryAsync();
        Console.WriteLine($"Database '{dbName}' created.");
    }
    else
    {
        Console.WriteLine($"Database '{dbName}' already exists.");
    }

    // Create user if it doesn't exist
    var checkUserCommand = new NpgsqlCommand(
        $"SELECT 1 FROM pg_user WHERE usename = '{dbUser}'",
        adminConn);

    var userExists = await checkUserCommand.ExecuteScalarAsync() != null;

    if (!userExists)
    {
        var createUserCommand = new NpgsqlCommand(
            $"CREATE USER {dbUser} WITH PASSWORD '{dbPassword}'",
            adminConn);
        await createUserCommand.ExecuteNonQueryAsync();
        Console.WriteLine($"User '{dbUser}' created.");
    }
    else
    {
        Console.WriteLine($"User '{dbUser}' already exists.");
    }

    // Grant privileges
    var grantDbCommand = new NpgsqlCommand(
        $"GRANT ALL PRIVILEGES ON DATABASE {dbName} TO {dbUser}",
        adminConn);
    await grantDbCommand.ExecuteNonQueryAsync();

    // Connect to the new database to grant schema privileges
    var serviceConnectionString = $"Host={host};Port={port};Database={dbName};Username={adminUser};Password={adminPassword}";
    await using var serviceConn = new NpgsqlConnection(serviceConnectionString);
    await serviceConn.OpenAsync();

    var grantSchemaCommand = new NpgsqlCommand(
        $"GRANT ALL PRIVILEGES ON SCHEMA public TO {dbUser}",
        serviceConn);
    await grantSchemaCommand.ExecuteNonQueryAsync();

    Console.WriteLine("Database initialization completed successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: Database initialization failed: {ex.Message}");
    Environment.Exit(1);
}
