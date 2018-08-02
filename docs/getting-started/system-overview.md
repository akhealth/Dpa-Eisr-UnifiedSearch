# System Overview

The search system consists of two services, an **API** and a **web server**, that enable users to search multiple benefits systems simultaneously.

In the `development` environment configuration we use several other services to aid development.

### Service Addresses

The service addresses are configurable using the `.env` file in the project root, or by editing the root `docker-compose.yml` file. Standard environment addresses are listed below.

#### `Development`

| Service               | Address                       |
|:----------------------|:------------------------------|
| Search API            | http://localhost:5000         |
| Search Site           | http://localhost:5001         |
| BrowserSync           | http://localhost:5002         |
| BrowserSync Config    | http://localhost:5003         |
| Documentation         | http://localhost:5004         |
| Swagger Documentation | http://localhost:5000/swagger |

## High-Level Architecture

Below is the current architecture for the `Test` environment:

![System Context](../resources/search-architecture.png?raw=true "Overview")