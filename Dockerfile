# Use the official .NET 8 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files
COPY . ./

# Build the application
RUN dotnet publish -c Release -o /out

# Use the official .NET 8 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /out .

# Expose the port the application runs on
EXPOSE 5000

# Set the entry point for the container
ENTRYPOINT ["dotnet", "BookAPI.dll"]
