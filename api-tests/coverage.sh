dotnet add package opencover --version 4.6.519 --package-directory ../tools

../tools/opencover/4.6.519/tools/OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test api-tests.csproj" -register:user -output:coverage-report.xml -oldStyle -filter:"+[API]SearchApi.* -[API]SearchApi.SearchApi -[API]SearchApi.Startup"