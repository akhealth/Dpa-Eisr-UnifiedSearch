# Code Style and Formatting

There are a number of tools used to ensure consistent style and formatting. The easiest way to take advantage of them all is to use Visual Studio Code plus the relevant extensions.

## Omnisharp

The `C#` extension for VS Code uses Omnisharp, which contains a C# formatting engine. Once the extension is installed, it will pick up configuration from the `omnisharp.json` file in the root directory of the repository.

In addition, we have committed the `.vscode/settings.json` file, which enables autoformatting after saving a file.

## Editorconfig

Editorconfig helps keep simple formatting rules, such as tabs/spaces, indentation, etc., consistent across all project files. You can change the settings by editing the `.editorconfig` file in the root directory of the repository.

There are also quite a few C#-specific options in the `.editorconfig` file, but most are only applicable in Visual Studio. They have been left in place in case VS Code adds support for these options in the future.

## ESLint

For client-side JavaScript, ESLint is used to format and lint code. The VS Code extension will analyze code on the fly to help prevent trivial issues, and it also provides formatting function which is run after saving JavaScript files.

You can change ESLint configuration by editing the relevant sections in `web/package.json` and `web-tests/package.json`.