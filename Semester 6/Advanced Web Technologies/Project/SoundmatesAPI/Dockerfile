# Etap 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Kopiujemy pliki
COPY . ./

# Przywracamy zależności
RUN dotnet restore

# Budujemy i publikujemy
RUN dotnet publish SoundmatesAPI/SoundmatesAPI.csproj -c Release -o /app/publish

# Etap 2: run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Kopiujemy opublikowaną aplikację z etapu build
COPY --from=build /app/publish .

# Ustawiamy zmienną środowiskową dla trybu Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Otwieramy port 5000
EXPOSE 5000

# Ustawiamy punkt wejścia dla aplikacji
ENTRYPOINT ["dotnet", "SoundmatesAPI.dll"]
