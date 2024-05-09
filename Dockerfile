# Base Microsoft ASP.NET Core SDK version 7.0 image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Copy .csproj and restore as distinct layers
COPY *.sln .
COPY *.csproj ./PacificToursAspDotnetCoreWebApp/
RUN dotnet restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "asp-net-core-web-app-authentication-authorisation.dll"]
