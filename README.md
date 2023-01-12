# TGPro Version 4

TGPro is a .NET project based on Clean Architecture that fully adheres to the principles and design
patterns such as
Dependency Inversion, Inversion of Control, Dependency Injection, ...

## Features

Update soon...

## Tech Stack

- [.NET 6.0](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6) - An open-source
  developer platform for
  building many different types of applications
- [EF Core 6.0](https://docs.microsoft.com/en-us/ef/core/) - EF Core can serve as an
  object-relational mapper (O/RM)
- [Angular 14](https://angular.io/start) - Angular is a development platform, built
  on [TypeScript](https://www.typescriptlang.org/)

## Building

Build project using the .NET Core CLI, which is installed with the .NET Core SDK. Then run these
commands from the CLI
in the root directory:

```bash
dotnet restore
cd src
dotnet build
dotnet run
```

## Migration

```bash
dotnet restore
cd src
dotnet ef migrations add InitialCreate -s TGProV4.Server -p TGProV4.Infrastructure -c ApplicationDbContext
dotnet ef database update -s TGProV4.Server -p TGProV4.Infrastructure -c ApplicationDbContext
```

## License

MIT

