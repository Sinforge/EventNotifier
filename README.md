# EventNotifier
## About
This project was developed as a test task for RTUITLab. The API can store information about upcoming events, notify users about them, form recommendations for users and much more.

## Getting Started locally
1. Install [.NET SDK 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. Install [PostgreSQL](https://www.postgresql.org)
3. Install PostGIS and create extension
  ```sql
    CREATE EXTENSION postgis;
  ```
4. Install [Redis](https://redis.io/docs/getting-started/installation/)

5. Create appsetting.json:
  ```json
  {
  "Logging": {
    "LogLevel": {
      "Default": "...",
      "Microsoft.AspNetCore": "..."
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning" //disable logs for sending sql queries
    }
  },
  "Audience": {
    "Secret": "<YOUR SECRET WORD FOR JWT AUTH>",
    "Iss": "<ISSUER NAME>",
    "Aud": "<AUDIENCE NAME>"
  },
  "EmailSettings": {
    //setting  of your email service
    "Username": "...",
    "Password": "...",
    "SmtpHost": "...",
    "SmtpPort": "...",
    "EmailFrom": "..."
  },
  "ConnectionStrings": {
    "RedisConnection": "<YOUR REDIS CONNECTION>",
    "DefaultConnection": "<YOUR PSQL CONNECTION>"
  },
  "AllowedHosts": "*"
}
  ```
6. Build and run app:
  ```cmd
  cd ../EventNotifier/
  dotnet run .
  ```
7. Check browser on [http://localhost:5146](http://localhost:5146) or [https://localhost:7171](https://localhost:7171)
## Getting started with Docker
 1. Create appsettings.json from <i><strong>Getting started 5.</i></strong>.
 2. Create .env file
  ```env
  POSTGRES_PASSWORD=<Password>
  POSTGRES_USER=<USER>
  POSTGRES_DB=<Database name>
  ``` 
 3. Build images and run containers
 ```cmd
 docker compose --env-file .env up --build
 ```
 4. Check browser on [http://localhost:5146](http://localhost:5146)
 ## Usage
Check [http://localhost:5146/swagger](http://localhost:5146/swagger) to check API documentation.