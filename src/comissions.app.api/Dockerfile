FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# Patch base-image OS CVEs flagged by the image scan gate (libgnutls30:
# CVE-2026-33845 et al., fixed in 3.7.9-2+deb12u7). Done as root before
# dropping to the non-root app user.
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends --only-upgrade libgnutls30 \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
COPY ["/src/comissions.app.api/comissions.app.api.csproj", "comissions.app.api/"]
COPY ["/src/comissions.app.database/comissions.app.database.csproj", "comissions.app.database/"]
RUN dotnet restore "comissions.app.api/comissions.app.api.csproj"
COPY . .
WORKDIR "/src/comissions.app.api"
RUN dotnet build "comissions.app.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "comissions.app.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "comissions.app.api.dll"]
