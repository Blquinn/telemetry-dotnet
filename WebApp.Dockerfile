FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY . .

WORKDIR /App

# Restore as distinct layers
RUN dotnet restore

WORKDIR /App/WebApp

# Build and publish a release
ENV ASPNETCORE_ENVIRONMENT=Development
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/WebApp/out .
ENTRYPOINT ["dotnet", "WebApp.dll"]
