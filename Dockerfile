# Base Microsoft ASP.NET Core SDK version 7.0 image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory to a new directory named 'app'
WORKDIR /app

# Set environment variable to disable NuGet package signature verification
# ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

# Copy the entire project into the 'app' directory
COPY . .

# estore as distinct layers
RUN dotnet restore asp-net-core-web-app-authentication-authorisation.sln

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime

# Set working directory to 'app'
WORKDIR /app

# Copy necessary files from the build stage
COPY --from=build /app/asp-net-core-web-app-authentication-authorisation.csproj ./
COPY --from=build /app/*.sln ./

# Restore packages
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Run the application
ENTRYPOINT ["dotnet", "asp-net-core-web-app-authentication-authorisation.dll"]
