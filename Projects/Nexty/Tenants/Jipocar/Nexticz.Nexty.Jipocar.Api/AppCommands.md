
# Marten
## How to regenerate Marten prebuilt classes
dotnet run --launch-profile "development" -- codegen delete
dotnet run --launch-profile "development" -- codegen write

# Docker

## Docker compose commands
### Run services without api
docker compose -f docker-compose.nexty.jipocar.dev.yaml up -d mssql postgres ftp rabbitmq elasticsearch
docker compose -f docker-compose.nexty.jipocar.dev.yaml down mssql postgres ftp rabbitmq elasticsearch

### Run services with api
docker compose -f docker-compose.nexty.jipocar.dev.yaml up -d
docker compose -f docker-compose.nexty.jipocar.dev.yaml down

# Db migrations

## Postgres Auth migrations

You need to go to the Api project and run:

```
dotnet ef migrations add Initial_Auth --context AuthDataContext --project "./../../../../../Modules/Auth/Nexticz.Module.Auth/Nexticz.Module.Auth.csproj" --startup-project "./Nexticz.Nexty.Jipocar.Api.csproj" --output-dir "Infrastructure/Dbs/Migrations"
dotnet ef database update --project "./../../../../../Modules/Auth/Nexticz.Module.Auth/Nexticz.Module.Auth.csproj" --context AuthDataContext
```

## Vh migrations

```
dotnet ef migrations add Vh_Depositor_Test --context Nexticz.Module.Vh.Infrastructure.Common.Persistence.DataContext --project "./../../../../../Modules/Vh/Nexticz.Module.Vh.Infrastructure/Nexticz.Module.Vh.Infrastructure.csproj" --startup-project "./Nexticz.Nexty.Jipocar.Api.csproj" --output-dir "Migrations"
dotnet ef database update --project "./Nexticz.Nexty.Jipocar.Api.csproj" --context Nexticz.Module.Vh.Infrastructure.Common.Persistence.DataContext
```
