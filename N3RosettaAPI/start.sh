#!/bin/sh

# run app in background
screen -dmS node dotnet neo-cli.dll

# keep container runnning 
tail -f /dev/null
# sleep infinitys