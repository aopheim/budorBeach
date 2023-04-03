# budorBeach

Værstasjon og fuglekassekamera på hytta

# Running in Docker

From the root directory, run ´yarn docker:compose:dev`or`yarn docker:compose:prod` for running in developmen/production mode.

# Applying database migrations

If you experience issues with applying database migrations through Entity Framework on the Production database, even though the ASPNETCORE_ENVIRONMENT in launchSettings.json is set to Production, one workaround is to remove the if statement in BudorDbContextDesignTimeFactory.cs, and hard-code in the production db: `optionsBuilder.UseSqlServer(
                config[GlobalConstants.ProductionDb]
            );`
