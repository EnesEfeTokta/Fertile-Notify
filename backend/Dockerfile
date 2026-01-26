FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FertileNotify.API/FertileNotify.API.csproj", "FertileNotify.API/"]
COPY ["FertileNotify.Application/FertileNotify.Application.csproj", "FertileNotify.Application/"]
COPY ["FertileNotify.Domain/FertileNotify.Domain.csproj", "FertileNotify.Domain/"]
COPY ["FertileNotify.Infrastructure/FertileNotify.Infrastructure.csproj", "FertileNotify.Infrastructure/"]

RUN dotnet restore "FertileNotify.API/FertileNotify.API.csproj"

COPY . .

WORKDIR "/src/FertileNotify.API"
RUN dotnet publish "FertileNotify.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FertileNotify.API.dll"]