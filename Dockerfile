FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LocalFunctionProj.csproj", "./"]
RUN dotnet restore "LocalFunctionProj.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "LocalFunctionProj.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LocalFunctionProj.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LocalFunctionProj.dll"]
