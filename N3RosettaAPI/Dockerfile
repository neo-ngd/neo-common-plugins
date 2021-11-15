FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN apt-get update && apt-get install -y \
  unzip \
  wget 

WORKDIR /mytmp
RUN git clone https://github.com/neo-ngd/neo-common-plugins.git
WORKDIR /mytmp/neo-common-plugins/N3RosettaAPI
RUN dotnet restore && dotnet publish -c Release -o /api

WORKDIR /app
RUN wget https://github.com/neo-project/neo-node/releases/download/v3.0.3/neo-cli-linux-x64.zip
RUN unzip neo-cli-linux-x64.zip

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
RUN apt-get update && apt-get install -y \
  screen \
  libleveldb-dev \
  sqlite3
RUN rm -rf /var/lib/apt/lists/*

WORKDIR /neo-cli/Plugins
COPY --from=build /api/RosettaAPI.dll .

WORKDIR /neo-cli/Plugins/RosettaAPI
COPY --from=build /api/RosettaAPI/config.json .

WORKDIR /neo-cli
COPY  --from=Build /app/neo-cli .

RUN ls

WORKDIR /neo-cli
ENTRYPOINT ["screen","-DmS","node","dotnet","neo-cli.dll","-r"]