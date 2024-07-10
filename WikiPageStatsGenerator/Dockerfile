FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.303 AS build
WORKDIR /src
COPY ["WikiPageStatsGenerator.csproj", "./"]
RUN dotnet restore "WikiPageStatsGenerator.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "WikiPageStatsGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WikiPageStatsGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WikiPageStatsGenerator.dll"]
