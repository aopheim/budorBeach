# budorBeach

Værstasjon og fuglekassekamera på hytta

# Pre-requisuites to run the daemon on a Raspberry Pi

- Install [yarn](https://yarnpkg.com/) as a package manager
- Install [docker](https://docs.docker.com/engine/install/debian/)

# Running in Docker

From the root directory, run ´yarn docker:compose:dev`or`yarn docker:compose:prod` for running in developmen/production mode.

# Applying database migrations

If you experience issues with applying database migrations through Entity Framework on the Production database, even though the ASPNETCORE_ENVIRONMENT in launchSettings.json is set to Production, one workaround is to remove the if statement in BudorDbContextDesignTimeFactory.cs, and hard-code in the production db: `optionsBuilder.UseSqlServer(
                config[GlobalConstants.ProductionDb]
            );`

# GitGuardian pre-commit hook

To setup a pre-commit hook for preventing secrets being committed, [follow this guide](https://blog.gitguardian.com/setting-up-a-pre-commit-git-hook-with-gitguardian-shield-to-scan-for-secrets/).

- You may need to add the ggshield install location to PATH environment variable. It might on a location like
  `C:\Users\<user-name>\AppData\Local\Packages\PythonSoftwareFoundation.Python.3.10_qbz5n2kfra8p0\LocalCache\local-packages\Python310\Scripts`
