FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.slnx .
COPY .editorconfig .
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY src/Soundmates.Api/*.csproj ./src/Soundmates.Api/

RUN dotnet restore

COPY src/ ./src/

RUN dotnet publish ./src/Soundmates.Api/Soundmates.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080
EXPOSE 8443

ENTRYPOINT ["dotnet", "Soundmates.Api.dll"]
