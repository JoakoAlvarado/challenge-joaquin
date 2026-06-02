FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ChallengeApi.API/ChallengeApi.API.csproj", "ChallengeApi.API/"]
COPY ["ChallengeApi.Application/ChallengeApi.Application.csproj", "ChallengeApi.Application/"]
COPY ["ChallengeApi.Domain/ChallengeApi.Domain.csproj", "ChallengeApi.Domain/"]
COPY ["ChallengeApi.Infrastructure/ChallengeApi.Infrastructure.csproj", "ChallengeApi.Infrastructure/"]

RUN dotnet restore "ChallengeApi.API/ChallengeApi.API.csproj"

COPY . .

WORKDIR "/src/ChallengeApi.API"
RUN dotnet build "ChallengeApi.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChallengeApi.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChallengeApi.API.dll"]