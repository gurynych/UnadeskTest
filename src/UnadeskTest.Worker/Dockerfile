FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/UnadeskTest.Shared/UnadeskTest.Shared.csproj", "src/UnadeskTest.Shared/"]
COPY ["src/UnadeskTest.Worker/UnadeskTest.Worker.csproj", "src/UnadeskTest.Worker/"]
RUN dotnet restore "src/UnadeskTest.Worker/UnadeskTest.Worker.csproj"
COPY src/ src/
WORKDIR /src/src/UnadeskTest.Worker
RUN dotnet publish "UnadeskTest.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UnadeskTest.Worker.dll"]
