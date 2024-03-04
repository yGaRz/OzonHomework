FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ozon.Route256.Practice.OrdersService.csproj", "src/Ozon.Route256.Practice.OrdersService/"]
RUN dotnet restore "src/Ozon.Route256.Practice.OrdersService/Ozon.Route256.Practice.OrdersService.csproj"
WORKDIR "/src/Ozon.Route256.Practice.OrdersService"
COPY . .
RUN dotnet build "Ozon.Route256.Practice.OrdersService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ozon.Route256.Practice.OrdersService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.Route256.Practice.OrdersService.dll"]