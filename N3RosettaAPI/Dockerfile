FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
RUN apt-get update && apt-get install -y \
  unzip \
  wget 

WORKDIR /tmp
RUN git clone https://github.com/neo-ngd/neo-common-plugins.git
WORKDIR /tmp/neo-common-plugins/N3RosettaAPI
RUN dotnet restore && dotnet publish -c Release -o /api

WORKDIR /app
RUN git clone https://github.com/neo-project/neo-node.git
WORKDIR /app/neo-node
RUN git checkout v3.1.0
RUN dotnet publish neo-cli/neo-cli.csproj -o /app/neo-cli

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS final
RUN apt-get update && apt-get install -y \
  screen \
  libleveldb-dev \
  sqlite3
RUN rm -rf /var/lib/apt/lists/*

WORKDIR /neo-cli
COPY --from=build /app/neo-cli ./

WORKDIR /neo-cli/Plugins
COPY --from=build /api/N3RosettaAPI.dll ./
COPY --from=build /tmp/neo-common-plugins/N3RosettaAPI/Dependencies/LevelDBStore.dll ./

WORKDIR /neo-cli/Plugins/N3RosettaAPI
COPY --from=build /api/N3RosettaAPI/config.*.json ./

WORKDIR /neo-cli
COPY start.sh ./
COPY neo-cli-conf/* ./
RUN chmod +x start.sh

ENTRYPOINT ["./start.sh", "NETWORK", "MODE"]
