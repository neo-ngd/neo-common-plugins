# How to use this Rosetta plugin

## Clone the whole project

```bash
git clone https://github.com/neo-ngd/neo-common-plugins.git
```

## Enter the dockerfile location

```bash
cd neo-common-plugins/RosettaAPI
```

## Build the docker image

```bash
docker build --tag rosetta:1.0 .
```

## Run the image in a container

```bash
docker run -dit -p 10332:10332 --name rosetta rosetta:1.0
```

## Test the container locally

```bash
curl 127.0.0.1:10332
```

You should receive a json response shown below after this step:

```json
{"jsonrpc":"2.0","id":null,"error":{"code":-32700,"message":"Parse error"}}
```

## Enter the container

```bash
docker exec -it rosetta /bin/bash
```

## Enter the screen

Since the neo-cli node is running in a screen process, use the following command to check its status:

```bash
screen -r node
```

## Check neo-cli status

After you enter the screen, you should see neo-cli is running normally with a command prompt "neo>". Use the following command to check its status:

```bash
show state
```

You should notice that neo-cli is synchronizing all the blocks, and this will take quite a moment. When all the blocks are synced, you can start using this plugin by sending commands through the rpc port, all the available APIs are listed [here](DataAPI.md) and [here](ConstructionAPI.md).
