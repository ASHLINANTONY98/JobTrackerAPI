# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY JobTrackerApp.sln .
COPY Business/*.csproj ./Business/
COPY Common/*.csproj ./Common/
COPY DataAccess/*.csproj ./DataAccess/
COPY JobTrackerApp/*.csproj ./JobTrackerApp/

RUN dotnet restore JobTrackerApp/JobTrackerApp.csproj

COPY Business ./Business
COPY Common ./Common
COPY DataAccess ./DataAccess
COPY JobTrackerApp ./JobTrackerApp
COPY NativeLibs/libwkhtmltox.dll /app/libwkhtmltox.dll


WORKDIR /app/JobTrackerApp
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/JobTrackerApp/out ./
ENTRYPOINT ["dotnet", "JobTrackerApp.dll"]
