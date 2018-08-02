# Installation and Setup

## Supported Platforms

- Windows 7 and above
- macOS
- Linux

We recommend using recent versions of MacOS or Linux distributions.

The system has been tested on the following platforms:

- macOS High Sierra
- Ubuntu 17.10, kernel version 4.14.0
- Windows 7
- Windows 10

We prefer using Bash over Powershell, but there are powershell workflows for all processes as well.

## Requirements

To run these projects, the following tools are required to be installed on your system:

- [Install Docker Community Edition](https://www.docker.com/community-edition#/download)
- [Install Docker Compose](https://docs.docker.com/compose/install/)

### Windows 7

Windows 7 will require a couple special steps:

- use [Docker Toolbox](https://docs.docker.com/toolbox/toolbox_install_windows/)
- place the project under `C:/Users/Public`

You may also need to use the Docker Quickstart terminal for docker-related operations.


## Running the System

### Environment

Once you have the tools listed above installed, first, you need to populate your environment with the needed environment variables:

```
source .env.example.bash
```

### Docker

Now, you can build and run the system using Docker:

```
docker-compose build
docker-compose up
```

The system will output startup messages from each process in your terminal, and you should now be able to access the system in a browser.

Proceed to the [System Overview](system-overview.md) for a list of valid URLs.