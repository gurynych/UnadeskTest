FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/UnadeskTest.Shared/UnadeskTest.Shared.csproj", "src/UnadeskTest.Shared/"]
COPY ["src/UnadeskTest.Api/UnadeskTest.Api.csproj", "src/UnadeskTest.Api/"]
RUN dotnet restore "src/UnadeskTest.Api/UnadeskTest.Api.csproj"
COPY src/ src/
WORKDIR /src/src/UnadeskTest.Api
RUN dotnet publish "UnadeskTest.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "UnadeskTest.Api.dll"]
