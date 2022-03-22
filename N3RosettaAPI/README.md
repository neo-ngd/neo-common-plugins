## Build into Docker image

Clone the docker file in the repository:

```bash
docker build -t n3-rosetta .
```

## Run

For `mainnet`, 10333 is default p2p port and 10335 is default rosetta api port.
**mainnet:online**
```bash
docker run -d -e NETWORK=mainnet -e MODE=online -p 10333:10333 -p 10335:10335 --name=n3-rosetta-mainnet n3-rosetta
```
**mainnet:offline**
```bash
docker run -d -e NETWORK=mainnet -e MODE=offline -p 10333:10333 -p 10335:10335 --name=n3-rosetta-mainnet n3-rosetta
```

For `testnet`, 20333 is default p2p port and 20335 is default rosetta api port.

**testnet:online**
```bash
docker run -d -e NETWORK=testnet -e MODE=online -p 20333:20333 -p 20335:20335 --name=n3-rosetta-testnet n3-rosetta
```
**testnet:offline**
```bash
docker run -d -e NETWORK=testnet -e MODE=offline -p 20333:20333 -p 20335:20335 --name=n3-rosetta-testnet n3-rosetta
```

After start the container successfully, use the following scripts to open neo-cli interactive window:

```bash
docker exec -it n3-rosetta-mainnet /bin/bash
screen -r node
```
