## What is it
A set of plugins that can be used inside the NEO core library. Check [here](https://docs.neo.org/docs/en-us/node/cli/setup.html) for the official documentation.

## Using plugins
Plugins can be used to increase functionality, as well as providing policies definitions of the network.

To configure a plugin, do the following:
- Compile from source
    - Clone this repository;
    - Select the plugin you want to enable and type the command `dotnet publish` \(compile it using Release configuration\)
    - Create the Plugins folder in neo-cli / neo-gui (where the binary is run from, like `/neo-cli/bin/Debug/netcoreapp3.0/Plugins`)
 - Copy the .dll and the folder with the configuration files into this Plugin folder.

 Note: you should also paste the dependencies of the plugin to the binary directory of the neo-cli.

The resulting folder structure is going to be like this:

```BASH
./neo-cli.dll
./Plugins/RestServer.dll
./Plugins/RestServer/config.json
```

## Existing plugins
### RestServer
Add this plugin to enable the restful service. It also provides the swagger-ui for users to query information with ease.

### GraphQL
GraphQL provides a complete and understandable description of the data in your API, gives clients the power to ask for exactly what they need and nothing more, makes it easier to evolve APIs over time, and enables powerful developer tools.

