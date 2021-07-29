namespace Catalog.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString
        {
            //using environmen variables
             // dotnet user-secrets init -> to create the secret files in <projectname>.csproj
             //dotnet user-secrets set MongoDbSettings:Password Pass#word1
             get
             {
                 if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
                     return $@"mongodb://{Host}:{Port}";
                 return $@"mongodb://{User}:{Password}@{Host}:{Port}";
             }
           // get { return $"mongodb://{Host}:{Port}"; }
            //get { return $"mongodb://{User}:{Password}@{Host}:{Port}"; }
        }
    }
}