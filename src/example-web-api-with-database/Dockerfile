ARG DOTNET_VERSION=10.0

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /source

COPY . .
# Restore dependencies
RUN dotnet restore

RUN dotnet publish \
    --configuration Release \
    --output /app \
    --no-restore \
    Api

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app
COPY --from=build /app ./

# Standardized ports for the service
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80
# ENV ASPNETCORE_HTTPS_PORTS=443 # Needs a certificate to be set up
# EXPOSE 443

ENTRYPOINT ["dotnet", "Api.dll"]
