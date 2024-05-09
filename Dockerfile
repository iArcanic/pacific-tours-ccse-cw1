# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set Config argument to toggle
ARG Config=Debug

# Set working directory to a new directory named 'source'
WORKDIR /source

# Copy .csproj, .sln, and restore as distinct layers
COPY asp-net-core-web-app-authentication-authorisation.sln .
COPY asp-net-core-web-app-authentication-authorisation.csproj .
RUN dotnet restore

# Copy entire project
COPY . .

# Publish app with the 'debug' build configuration
RUN dotnet publish -c ${Config} -o /app --no-restore

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "asp-net-core-web-app-authentication-authorisation.dll"]
