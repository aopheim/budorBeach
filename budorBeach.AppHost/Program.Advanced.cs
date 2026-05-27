using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// SQL Server for development database
var sqlPassword = builder.AddParameter("sql-password", secret: true);
var sql = builder
    .AddSqlServer("sqlserver", password: sqlPassword, port: 1433)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("MSSQL_SA_PASSWORD", p => sqlPassword.Resource.Value);

var sqldb = sql.AddDatabase("budordb", "budordb");

// rpiDaemon service - main backend orchestrator
var rpiDaemon = builder
    .AddProject<Projects.rpiDaemon>("rpidaemon")
    .WithReference(sqldb)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://0.0.0.0:80")
    .WithHttpEndpoint(port: 5001, targetPort: 80, name: "http");

// BudorWeb service - frontend web application
var web = builder
    .AddProject<Projects.budorWeb>("budorweb")
    .WithReference(rpiDaemon)
    .WithHttpEndpoint(port: 5000, targetPort: 80, name: "http");

builder.Build().Run();
