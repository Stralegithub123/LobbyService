# 1. faza: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiramo .csproj i radimo restore paketa
COPY ["LobbyService.csproj", "./"]
RUN dotnet restore "LobbyService.csproj"

# Kopiramo ostatak koda i pravimo objavljenu verziju
COPY . .
RUN dotnet publish "LobbyService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. faza: Runtime (Finalna slika)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Railway ce sam dodeliti port, .NET 8 to slusa automatski na portu 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LobbyService.dll"]