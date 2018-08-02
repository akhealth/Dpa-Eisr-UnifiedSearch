# This script sets ENV variables for `web` and `api` in the Azure staging environment.
# Note: this script requires AzureRM support and login ability.  It can't be run in CI yet.

$apiName = "DPAEisrUnifiedSearch-API"
$webName = "DPAEisrUnifiedSearch"
$port = 80
$slot = "staging"

# ENV variables for API/Staging
$apiSettings = @{}
$apiSettings["ASPNETCORE_ENVIRONMENT"] = "Development"
$apiSettings["NODE_ENV"] = "development"
$apiSettings["SEARCH_API_PORT"] = $port
$apiSettings["SEARCH_WEB_URL"] = "http://stage.myaries.alaska.gov/search-web-stage"
$apiSettings["SEARCH_WEB_SECURE_URL"] = "https://stage.myaries.alaska.gov/search-web-stage"
$apiSettings["SEARCH_API_ASPNETCORE_URLS"] = "http://*:$port"
$apiSettings["ESB_URL"] = "https://esbtest.dhss.alaska.gov"

# Both are in the same resource group with the name $web
Set-AzureRmWebAppSlot -ResourceGroupName $webName -Name $apiName -AppSettings $apiSettings -Slot $slot

# ENV variables for Web/Staging
$webSettings = @{}
$webSettings["ASPNETCORE_ENVIRONMENT"] = "Development"
$webSettings["NODE_ENV"] = "development"
$webSettings["SEARCH_WEB_PORT"] = $port
$webSettings["SEARCH_API_URL"] = "https://stage.myaries.alaska.gov/search-api-stage"
$webSettings["SEARCH_WEB_URL"] = "https://stage.myaries.alaska.gov/search-web-stage"
$webSettings["SEARCH_WEB_ASPNETCORE_URLS"] = "http://*:$port"

Set-AzureRmWebAppSlot -ResourceGroupName $webName -Name $webName -AppSettings $webSettings -Slot $slot