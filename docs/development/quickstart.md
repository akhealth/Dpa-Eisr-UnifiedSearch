# Development Quickstart

First, follow the steps in the [Installation and Setup](../getting-started/installation-and-setup.md) document, then continue below.

## Using Docker

[Docker](https://www.docker.com/what-docker) is a tool used to containerize applications (like VMs, but smaller and faster). This makes deployments and development more similar, and Docker offers [many other benefits](https://www.docker.com/what-container).

You can build and run the entire search system using the following Docker commands:

```
docker-compose build
docker-compose up
```

During development, docker will mount the local source code directories as a volumes in the containers. 

The default configuration will watch for changes to the source code and restart the corresponding services automatically.

If you change other files, such as adding new project dependencies, you may need to rebuild and restart the containers:

```
docker-compose build
docker-compose up
```

You can also run the command `docker ps` to view currently running containers. Sometimes Docker runs into a problem and is unable to build or run the project - doing a hard restart of Docker usually fixes the problem.

## Not Using Docker

If you prefer not to use docker, or need to run tests, you'll also need a few other things:

- [Install Microsoft Dotnet Core 2.0](https://www.microsoft.com/net/download)
- [Install Current Node.js](https://nodejs.org/en/download/)

Follow the commands in the dockerfiles in each project to see the build and run process. 

Essentially, you need to use `dotnet` in your local terminal to build and run the server processes. For the web project, you also need to bundle the client-side assets:

```
cd web
npm install
npx webpack -w
```

This will install client-side dependencies, then run Webpack in watch mode to recompile and reload any changed files.

