# EatOut
Find food trucks neary by in san francisco

This is the solution to the challenge https://github.com/timfpark/take-home-engineering-challenge. We have to find the location of the nearest food trucks in san francisco.

This is the api https://eatout20210629183754.azurewebsites.net/api/GetFoodNearby and you can call a post on this endpoint using the following json in the body.
{
"Latitude": "37.74171783074393",
"Longitude":"-122.40541797744821"
}

This will return 5 nearest locations of food truck based on the current coordinates. You can add more results by adding a result size
{
"Latitude": "37.74171783074393",
"Longitude":"-122.40541797744821",
"ResultSize":"10"
}

This service uses multi tenant service to service authentication and checks the claims with a valid set of whitelisted apps.
Ex I have whitelisted this app id e2e0854a-440c-41e8-9a2f-5800b83568d7 and i use the following curl commands to get the access token for the app.

$baseurl =  'https://eatout20210629183754.azurewebsites.net'

$clientSecret = '<Put Secret Here>'; echo $clientSecret

$access_token=$(curl.exe -d "grant_type=client_credentials" -d "client_id=e2e0854a-440c-41e8-9a2f-5800b83568d7" --data-urlencode "client_secret=$clientSecret" --data-urlencode "resource=8ae23107-f283-4447-b4a4-2b911e12a524" -v https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token | ConvertFrom-Json).access_token ; echo $access_token

Now i use the access token to call the api.

curl.exe -s -w "${base_url}:%{time_connect}s:%{time_starttransfer}s:%{time_total}s\n" -v -H "Content-Type: application/json" -H "Authorization: Bearer $access_token" -X POST $baseurl/api/GetFoodNearby --data '{\"Latitude\":\"37.74171783074393\",\"Longitude\":\"-122.40541797744821\"}' -v
