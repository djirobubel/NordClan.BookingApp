FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore "NordClan.BookingApp.Api/NordClan.BookingApp.Api.csproj"

RUN dotnet build "NordClan.BookingApp.Api/NordClan.BookingApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NordClan.BookingApp.Api/NordClan.BookingApp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NordClan.BookingApp.Api.dll"]