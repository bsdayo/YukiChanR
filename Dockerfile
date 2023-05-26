FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish src/YukiChanR/YukiChanR.csproj \
    --no-restore \
    --no-self-contained \
    -c Release \
    -o /app \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine AS runtime
COPY --from=build /app /app
WORKDIR /data
ENTRYPOINT ["dotnet", "/app/YukiChanR.dll"]
