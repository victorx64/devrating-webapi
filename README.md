# devrating-webapi
Dev Rating Web API

## Run SQL Server

```
docker run --name mssql -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=65awe_5f651323d' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest
```

## Build Web API

```
docker build -t webapi .
```

## Run Web API

```
docker run --name webapi -p 8000:80 -e 'ConnectionString="Server=mssql;User Id=sa;Password=65awe_5f651323d;"' --link=mssql -d webapi:latest
```