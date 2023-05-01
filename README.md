# EventNotifier
## Getting Started
1. Install [.NET SDK 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. Create appsetting.json:
  ```json
  {
  "Logging": {
    "LogLevel": {
      "Default": "...",
      "Microsoft.AspNetCore": "..."
    }
  },
  "Audience": {
    "Secret": "<YOUR SECRET WORD FOR JWT AUTH>",
    "Iss": "<ISSUER NAME>",
    "Aud": "<AUDIENCE NAME>"
  },
  "EmailSettings": {
    "Username": "...",
    "Password": "...",
    "SmtpHost": "...",
    "SmtpPort": "...",
    "EmailFrom": "..."
  },
  "ConnectionStrings": {
    "DefaultConnection": "<YOUR PSQL CONNECTION>"
  },
  "AllowedHosts": "*"
}  
  ```
3. Install EF tool and init db:
  ```cmd
  cd ../EventNotifier/EventNotifier/
  dotnet tool install --global dotnet-ef
  dotnet ef database update
  ```
4. Build and run app:
  ```cmd
  cd ../EventNotifier/
  dotnet run .
  ```
5. Check browser on [http://localhost:5146](http://localhost:5146) or [https://localhost:7171](https://localhost:7171)
6. Run with docker:
  ```cmd
  Comming soon...
  ```

