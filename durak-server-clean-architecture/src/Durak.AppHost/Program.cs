var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.Durak_Api>("durakapi");
builder.Build().Run();