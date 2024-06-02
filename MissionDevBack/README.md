Code générator : `dotnet tool install --global dotnet-aspnet-codegenerator`  
Controllers scaffold : `dotnet aspnet-codegenerator controller -name UsersController -async -api -m User -dc IntranetContext -outDir Controllers`  
EF scaffold (scaffold from Db) : `dotnet ef dbcontext scaffold`  
dotnet ef database drop  
dotnet ef database update  
dotnet ef database create 
dotnet ef database reset
dotnet ef migrations add InitialCreate  
dotnet ef migrations add  

dotnet user-secrets init
dotnet user-secrets set "MissionDevDbCredentials" "Host=localhost;Database=mission_dev;Username=mission_dev;Password="

https://learn.microsoft.com/fr-fr/aspnet/core/performance/caching/overview?view=aspnetcore-8.0