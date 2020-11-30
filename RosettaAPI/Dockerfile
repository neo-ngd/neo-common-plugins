FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
RUN apt-get update && apt-get install -y \
  unzip \
  wget 

WORKDIR /mytmp
RUN git clone https://github.com/neo-ngd/neo-common-plugins.git
WORKDIR /mytmp/neo-common-plugins/RosettaAPI
RUN dotnet restore && dotnet publish -c Release -o /api

WORKDIR /app
RUN wget https://github.com/neo-project/neo-node/releases/download/v2.12.2/neo-cli-linux-x64.zip 
RUN unzip neo-cli-linux-x64.zip

WORKDIR /app/neo-cli
RUN rm config.json
RUN wget https://raw.githubusercontent.com/joeqian10/Rosetta_config/master/config.json
RUN cat config.json

FROM mcr.microsoft.com/dotnet/core/runtime:2.1 AS final
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