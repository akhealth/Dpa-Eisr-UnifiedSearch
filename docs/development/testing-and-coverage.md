# Testing

Before running tests and coverage, make sure you load the environment variables as described in the installation and setup documents.

## Server Side

You can easily run `dotnet` tests for the API and Web projects. In the corresponding test project, simply run one of the valid test commands, e.g.,

```
cd web-tests
dotnet test
```

You can also run code coverage analysis using the `coverage` scripts in the test projects:

```
cd web-tests
sh coverage.sh
```


Currently, the code coverage tool OpenCover only supports Windows, but MacOS and Linux support [may be coming soon](https://github.com/OpenCover/opencover/issues/703).

### Cross-Platform Coverage

The API uses `coverlet` to generate cross-platform coverage metrics. To perform analysis and generate an output, use the following command:

```
cd api-tests
dotnet test /p:CollectCoverage=true
```

If you're using Git Bash, you may need to add an extra slash: `//p:CollectCoverage=true`.

The output from `coverlet` is pretty sparse, and the `coverage.json` output is difficult to analyze. You can use a tool `coverage-viewer` to more easily review coverage statistics:

```
cd api-tests
npm install -g coverage-viewer httpserver
coverage-viewer coverage.json -s ../api -o ./coverage
cd coverage
httpserver
```

## Client Side

Javascript tests currently run within the `web-tests` project:

```
cd web-tests
npm test
```

Client-side coverage analysis can also be executed within the `web-tests` project:

```
cd web-tests
npm run coverage
```

If the javascript tests fail, the problem can sometimes be fixed by running `npm install`.

### `react-mocha-setup.js`

In order to test the React components and other UI features, we use `jsdom` to simulate a browser-like environment that allows checking various DOM states. Some functionality also requires global objects and methods that are available in the browser but aren't normally in Node.js.

All UI tests will typically include `react-mocha-setup.js` to provide `jsdom`, global browser objects like `document` or `window`, and general setup code for other client-side tests. Any common setup and global values, such as environment variables, should be added to the `react-mocha-setup.js` file for use in automated tests.

