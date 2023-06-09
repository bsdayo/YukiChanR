# Build stage
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-preview-alpine AS build

ARG TARGETARCH
ARG BUILD_REF_NAME=unknown
ARG BUILD_COMMIT_HASH=0000000000000000000000000000000000000000

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
COPY src/YukiChanR.Plugins.Monitor/YukiChanR.Plugins.Monitor.csproj \
    src/YukiChanR.Plugins.Monitor/

RUN dotnet restore -a $TARGETARCH

# Build project
COPY . .
RUN dotnet build src/YukiChanR/YukiChanR.csproj \
    -c Release \
    -a $TARGETARCH \
    --no-restore \
    -p:UseAppHost=false \
    -p:BuildRefName=$BUILD_REF_NAME \
    -p:BuildCommitHash=$BUILD_COMMIT_HASH \
    -p:BuildExtraTags=docker

# Publish project
RUN dotnet publish src/YukiChanR/YukiChanR.csproj \
    -c Release \
    -a $TARGETARCH \
    --no-build \
    -o publish \
    -p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine AS runtime
COPY --from=build /repo/publish /app
WORKDIR /data
ENTRYPOINT ["dotnet", "/app/YukiChanR.dll"]
