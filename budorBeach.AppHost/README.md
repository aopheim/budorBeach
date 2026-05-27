# BudorBeach Aspire AppHost

Local development orchestration with SQL Server, rpiDaemon backend, and budorWeb frontend.

## Quick Start

```bash
dotnet run --project BudorBeach.AppHost
```

Visit: https://localhost:18888 (Aspire Dashboard)

## Prerequisites

- .NET 8.0 SDK
- Docker Desktop (running)
- Aspire workload: `dotnet workload install aspire`

## Services

| Service | Port | Purpose |
|---------|------|---------|
| Aspire Dashboard | 18888 | Monitoring hub |
| SQL Server | 1433 | Development DB |
| rpiDaemon | 5001 | Backend (motion detection, uploads) |
| budorWeb | 5000 | Web frontend |

## Features

✅ Real-time logging dashboard  
✅ Video upload job (every 2 minutes)  
✅ Service health monitoring  
✅ Persistent SQL database  
✅ Hot reload support  

## Setup

1. **Navigate to AppHost folder**
   ```bash
   cd BudorBeach.AppHost
   ```

2. **Run**
   ```bash
   dotnet run
   ```

3. **Open dashboard**
   - https://localhost:18888

## Configuration

Edit `Program.cs` to:
- Change ports
- Add environment variables
- Modify service references

### Using Program.Advanced.cs

```bash
cp Program.Advanced.cs Program.cs
dotnet run
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Docker not running | Start Docker Desktop |
| Port in use | Edit ports in Program.cs |
| SQL won't connect | Wait 30s, verify `docker ps` |
| Dashboard won't load | Use https:// (not http://) |
| Build fails | `dotnet clean && dotnet build` |

## Common Commands

```bash
# Run with hot reload
dotnet watch run --project BudorBeach.AppHost

# Build
dotnet build BudorBeach.AppHost

# View containers
docker ps

# View SQL logs
docker logs -f sqlserver

# Clean Docker
docker system prune -a
```

## Files

- `BudorBeach.AppHost.csproj` - Project config
- `Program.cs` - Main orchestration
- `Program.Advanced.cs` - Enhanced variant
- `launchSettings.json` - Debug settings

## Architecture

```
┌──────────────────────────────┐
│  Aspire Dashboard (18888)    │
└──────────────┬───────────────┘
    ┌──────────┼──────────┐
    ▼          ▼          ▼
┌────────┐ ┌────────┐ ┌────────┐
│  SQL   │ │rpiD    │ │budorWeb│
│ :1433  │ │:5001   │ │:5000   │
└────────┘ └────────┘ └────────┘
```

## Testing

### Verify Services
- Dashboard shows all 4 services green ✓

### Test Web UI
- http://localhost:5000 (budorWeb)
- http://localhost:5001 (rpiDaemon)

### Test Video Upload
1. Create test video in: `%APPDATA%\budorBeach\motionRecordings\`
2. Job runs every 2 minutes
3. Check rpiDaemon logs in dashboard
4. Query: `SELECT * FROM VideoUploads`

## Development Workflow

```bash
# Start with watch
dotnet watch run --project BudorBeach.AppHost

# Edit code
# Services auto-rebuild
# Dashboard shows changes
```

## Stop

Press `Ctrl+C` in terminal. Data persists (SQL volume).

## Learn More

- [Aspire Docs](https://learn.microsoft.com/en-us/dotnet/aspire)
- [Dashboard Guide](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/dashboard)
- [Docker Docs](https://docs.docker.com/desktop/)

