## Build into Docker

Clone the docker file in the repository:

```bash
docker build -t n3-rosetta .
docker run -p 10332:10332 -p 10333:10333 -p 10335:10335 --name=n3-rosetta-mainnet n3-rosetta
```

After start the container successfully, use the following scripts to open neo-cli interactive window:

```bash
docker exec -it n3-rosetta-mainnet /bin/bash
screen -r node
```
