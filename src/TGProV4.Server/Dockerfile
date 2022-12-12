FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ENV ASPNETCORE_URLS=https://+:5002;http://+:5003
WORKDIR /app
EXPOSE 5002
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY ["src/TGProV4.Server/TGProV4.Server.csproj", "src/TGProV4.Server/"]
COPY ["src/TGProV4.Application/TGProV4.Application.csproj", "src/TGProV4.Application/"]
COPY ["src/TGProV4.Client/TGProV4.Client.csproj", "src/TGProV4.Client/"]
COPY ["src/TGProV4.Domain/TGProV4.Domain.csproj", "src/TGProV4.Domain/"]
COPY ["src/TGProV4.Infrastructure/TGProV4.Infrastructure.csproj", "src/TGProV4.Infrastructure/"]
COPY ["src/TGProV4.Shared/TGProV4.Shared.csproj", "src/TGProV4.Shared/"]
RUN dotnet restore "src/TGProV4.Server/TGProV4.Server.csproj"
COPY . .
WORKDIR "src/TGProV4.Server"
RUN dotnet build "TGProV4.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TGProV4.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
WORKDIR /app/Files
WORKDIR /app
ENTRYPOINT ["dotnet", "TGProV4.Server.dll"]