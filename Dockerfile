# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy project file first (better caching)
COPY src/BoxOfficeReporting.Api/BoxOfficeReporting.Api.csproj src/BoxOfficeReporting.Api/

RUN dotnet restore src/BoxOfficeReporting.Api/BoxOfficeReporting.Api.csproj

# copy everything else
COPY . .

RUN dotnet publish src/BoxOfficeReporting.Api/BoxOfficeReporting.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BoxOfficeReporting.Api.dll"]
