using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// SQL Server for development
var sqlPassword = builder.AddParameter("sql-password", secret: true);
var sql = builder
    .AddSqlServer("sqlserver", password: sqlPassword, port: 1433)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var sqldb = sql.AddDatabase("budordb");

// rpiDaemon service
var rpiDaemon = builder
    .AddProject<Projects.rpiDaemon>("rpidaemon")
    .WithReference(sqldb)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithHttpEndpoint(port: 5001, targetPort: 80, name: "http");

// BudorWeb service
var web = builder
    .AddProject<Projects.budorWeb>("budorweb")
    .WithReference(rpiDaemon)
    .WithHttpEndpoint(port: 5000, targetPort: 80, name: "http");

builder.Build().Run();
