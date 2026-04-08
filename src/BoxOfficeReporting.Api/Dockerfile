FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BoxOfficeReporting.Api/BoxOfficeReporting.Api.csproj", "src/BoxOfficeReporting.Api/"]
RUN dotnet restore "src/BoxOfficeReporting.Api/BoxOfficeReporting.Api.csproj"

COPY . .
WORKDIR "/src/src/BoxOfficeReporting.Api"
RUN dotnet publish "BoxOfficeReporting.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS="https://+:8081;http://+:8080" \
    ASPNETCORE_HTTPS_PORTS=8081

ENTRYPOINT ["dotnet", "BoxOfficeReporting.Api.dll"]
