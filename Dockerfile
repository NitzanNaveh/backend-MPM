# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore dependencies
COPY ProjectManager.Api/ProjectManager.Api.csproj ./ProjectManager.Api/
RUN dotnet restore ProjectManager.Api/ProjectManager.Api.csproj

# Copy the rest of the source code and publish
COPY ProjectManager.Api/. ./ProjectManager.Api/
WORKDIR /src/ProjectManager.Api
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProjectManager.Api.dll"]
