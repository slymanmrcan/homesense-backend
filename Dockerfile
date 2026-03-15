FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# root olmayan kullanıcı oluştur
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HomeSense.Api.csproj", "./"]
RUN dotnet restore "HomeSense.Api.csproj"
COPY . .
RUN dotnet publish "HomeSense.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# sahipliği appuser'a ver, sonra geç
RUN chown -R appuser:appgroup /app
USER appuser

ENTRYPOINT ["dotnet", "HomeSense.Api.dll"]
