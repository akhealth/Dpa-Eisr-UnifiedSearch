# Environment Name
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:NODE_ENV="development"
$env:WEBSEAL_AUTH_DISABLED="true"

# Search API Configuration
$env:SEARCH_API_PORT=5000
$env:SEARCH_API_ASPNETCORE_URLS="http://*:$env:SEARCH_API_PORT"
$env:ESB_URL="https://esbtest.dhss.alaska.gov"

# Search Web Configuration
$env:SEARCH_WEB_PORT=5001
$env:SEARCH_API_URL="http://localhost:$env:SEARCH_API_PORT"
$env:SEARCH_WEB_URL="http://localhost:$env:SEARCH_WEB_PORT"
$env:SEARCH_WEB_ASPNETCORE_URLS="http://*:$env:SEARCH_WEB_PORT"
