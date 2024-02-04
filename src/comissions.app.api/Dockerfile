FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
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
