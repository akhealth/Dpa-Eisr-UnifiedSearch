# EIS Unified Search

The unified search system is a tool developed to simplify the process of finding benefits program application information for Alaskan individuals and families.

The system fetches and provides a unified view of data from the Master Client Index (MCI), the Eligibility Information System (EIS), and Alaska's Resource for Integrated Eligibility Services (ARIES).

## Getting Started

Follow the [Installation and Setup](docs/getting-started/installation-and-setup.md) document, followed by the [Development Quickstart](docs/development/quickstart.md) guide.

After running `docker-compose up`, you should be able to access services on the following URLs using the development configuration:

| Service            | Address               |
|:-------------------|:----------------------|
| Search API         | http://localhost:5000 |
| Search Site        | http://localhost:5001 |
| BrowserSync        | http://localhost:5002 |
| BrowserSync Config | http://localhost:5003 |
| Documentation      | http://localhost:5004 |

## Important Documentation

* [Introduction](docs/README.md)
* [Installation and Setup](docs/getting-started/installation-and-setup.md)
* [System Overview](docs/getting-started/system-overview.md)
* [Development Quickstart](docs/development/quickstart.md)
* [Collaboration Workflow](docs/development/collaboration-workflow.md)
* [Testing and Coverage](docs/development/testing-and-coverage.md)
* [Code Style and Formatting](docs/development/code-style-and-formatting.md)
* [FAQ](docs/more-information/faq.md)

## Contributing

Contributions are welcome, and appreciated! See [CONTRIBUTING.md](CONTRIBUTING.md) for more info about how to contribute.

## License

As a work of the State of Alaska, this project is in the public domain within the United States.

Additionally, we waive copyright and related rights in the work worldwide through the CC0 1.0 Universal public domain dedication.

See [LICENSE.md](LICENSE.md) for the complete license text.

[travis-url]: https://travis-ci.org/
[travis-image]: https://img.shields.io/travis/