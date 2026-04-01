FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BoxOfficeReporting.Api.csproj", "./"]
RUN dotnet restore "BoxOfficeReporting.Api.csproj"

COPY . .
RUN dotnet publish "BoxOfficeReporting.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .

RUN mkdir -p /app/data
RUN mkdir -p /app/keys

EXPOSE 8080

ENTRYPOINT ["dotnet", "BoxOfficeReporting.Api.dll"]
