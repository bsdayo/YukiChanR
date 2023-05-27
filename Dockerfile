# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build

WORKDIR /repo

# Copy all project files and restore first
COPY YukiChanR.sln ./

COPY deps/Flandre/src/Flandre.Core/Flandre.Core.csproj \
    deps/Flandre/src/Flandre.Core/
COPY deps/Flandre/src/Flandre.Framework/Flandre.Framework.csproj \
    deps/Flandre/src/Flandre.Framework/
COPY deps/Flandre/src/Flandre.Adapters.OneBot/Flandre.Adapters.OneBot.csproj \
    deps/Flandre/src/Flandre.Adapters.OneBot/
COPY deps/Flandre/src/Flandre.Adapters.OneBot.Extensions/Flandre.Adapters.OneBot.Extensions.csproj \
    deps/Flandre/src/Flandre.Adapters.OneBot.Extensions/

COPY src/YukiChanR/YukiChanR.csproj \
    src/YukiChanR/
COPY src/YukiChanR.Core/YukiChanR.Core.csproj \
    src/YukiChanR.Core/
COPY src/YukiChanR.Plugins.Arcaea/YukiChanR.Plugins.Arcaea.csproj \
    src/YukiChanR.Plugins.Arcaea/

RUN dotnet restore

# Build project
COPY . .
RUN dotnet build src/YukiChanR/YukiChanR.csproj \
    -c Release \
    --no-restore \
    -p:UseAppHost=false

# Publish project
RUN dotnet publish src/YukiChanR/YukiChanR.csproj \
    -c Release \
    --no-build \
    -o publish \
    -p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine AS runtime
COPY --from=build /repo/publish /app
WORKDIR /data
ENTRYPOINT ["dotnet", "/app/YukiChanR.dll"]
