#!/bin/sh
echo "running ${NETWORK} node in ${MODE} mode"

cp config.${NETWORK}.${MODE}.json config.json
cp Plugins/N3RosettaAPI/config.${NETWORK}.json Plugins/N3RosettaAPI/config.json

# run app in background
screen -dmS node dotnet neo-cli.dll

# keep container runnning 
tail -f /dev/null
# sleep infinitys
