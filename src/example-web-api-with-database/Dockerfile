ARG DOTNET_VERSION=10.0

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /source

COPY . .
# Restore dependencies
RUN dotnet restore

RUN dotnet publish \
    --configuration Release \
    --output /app/Init \
    --no-restore \
    Init

RUN dotnet publish \
    --configuration Release \
    --output /app/Api \
    --no-restore \
    Api

# Init runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS init-runtime
WORKDIR /app
COPY --from=build /app/Init ./

ENTRYPOINT ["dotnet", "Init.dll"]

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS api-runtime
WORKDIR /app
COPY --from=build /app/Api ./

# Standardized ports for the service
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80
# ENV ASPNETCORE_HTTPS_PORTS=443 # Needs a certificate to be set up
# EXPOSE 443

ENTRYPOINT ["dotnet", "Api.dll"]
