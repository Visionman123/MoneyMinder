# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . /app

# Set the environment variable for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the port specified in the app configuration
EXPOSE 80

# Install necessary tools and libraries
RUN apt-get update \
    && apt-get install -y libgdiplus \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/

# Start the application
ENTRYPOINT ["dotnet", "lifechartapi.dll"]