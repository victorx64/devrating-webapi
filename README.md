# devrating-webapi
Dev Rating Web API

## Run SQL Server

```
docker run --name mssql -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=65awe_5f651323d' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest
```

## Run on localhost

```
cd WebApi
dotnet user-secrets set "ConnectionString" "Server=localhost;User Id=sa;Password=65awe_5f651323d;"
dotnet run
```

## Run in Docker container

```
docker build -t webapi .
docker run --name webapi -p 8000:80 -e 'ConnectionString="Server=mssql;User Id=sa;Password=65awe_5f651323d;"' --link=mssql -d webapi:latest
```

## Seed DB

```
curl 'http://localhost:5000/api/v1/db/domain' -i
curl 'http://localhost:5000/api/v1/db/web' -i
```

## Check db status

```
curl 'http://localhost:5000/api/v1/statuses/domain' | jq
curl 'http://localhost:5000/api/v1/statuses/web' | jq
```
