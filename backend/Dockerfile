FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FertileNotify.API/FertileNotify.API.csproj", "FertileNotify.API/"]
COPY ["FertileNotify.Application/FertileNotify.Application.csproj", "FertileNotify.Application/"]
COPY ["FertileNotify.Domain/FertileNotify.Domain.csproj", "FertileNotify.Domain/"]
COPY ["FertileNotify.Infrastructure/FertileNotify.Infrastructure.csproj", "FertileNotify.Infrastructure/"]

RUN dotnet restore "FertileNotify.API/FertileNotify.API.csproj"

COPY . .

WORKDIR "/src/FertileNotify.API"
RUN dotnet publish "FertileNotify.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# GC: memory-conserving workstation mode, hard heap cap ~143MB, tiered PGO for hot paths
ENV DOTNET_GCConserveMemory=7
ENV DOTNET_GCHeapHardLimit=150000000
ENV DOTNET_TieredPGO=1

EXPOSE 8080

ENTRYPOINT ["dotnet", "FertileNotify.API.dll"]