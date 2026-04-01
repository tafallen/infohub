# infohub

(A centralised group of Azure Functions that feed an infoscreen)

## The APIs

### GetSunTimes

Get the sunset and sunrise times for a given lattitude and longitude.

#### Configuration

To use this you'll need to set a environment variable containing the URI of the sunrise-sunset.org API. It's a free service. They're good people.

##### Local config

In your local.settings.json file add this in the values section:
```"sunrisesunsetapi" : "https://api.sunrise-sunset.org/json"```

##### Server side setup

In the Application Settings area of the Azure Functions where you're deploying this you'll need to add a new setting with the name _sunrisesunsetapi_ and the URI as shown above.

We'll be using this for all the API URLs and API Keys as it's bad practice to put them into config files.
Ideally we'd secure them in KeyVault but that's for another day.

#### Usage

Provide the Query Params _lat_ and _lng_ with numerical values for for lattitude and longitude. So you'd call it like this from within Postman or from your code:
```https://<<your service URI>>/api/GetSunTimes?lat=<<your lattitude>>&lng=-<<your longitude>>```
Change the details within the <<>> to suit your service and location. And get rid of the <<>> of course.
Returned is a small scrap of JSON with the sunrise and sunset times for the chosen location.

### GetWeather

Gets the weather for a given location.

#### Configuration

This uses the Accuweather weather API from [https://developer.accuweather.com/](https://developer.accuweather.com/) so an account with them and an API key is required. A location key is required to get a forecast for the chosen area. Location keys can be determined using this api: [https://developer.accuweather.com/accuweather-locations-api/apis](https://developer.accuweather.com/accuweather-locations-api/apis).

##### Local config

In your local.settings.json file add this in the values section:
```"weatherapi" : "http://dataservice.accuweather.com/forecasts/v1/daily/5day/"```
```"weatherapikey":"<<PUT YOUR API KEY HERE>>"```

##### Server side setup

In the Application Settings area of the Azure Functions where you're deploying this you'll need to add a new setting with the name _weatherapi_ and the URI as shown above. You'll also need to add _weatherapikey_ and the api key.

#### Usage

Provide the Query Param _location__ with the location key and you'd call it like this from within Postman or from your code:
```https://<<your service URI>>/api/GetWeather?location=<<Your Location Key>>```

Returned is a JSON object containing a basic 5 day weather forecast, maximum and minimum temperatures and a summary forecast.

#### To Do
[TODO: Cache results for 12 hours]

### GetNews

Gets the latest top ten news headlines from the BBC via the NewsAPI service (newsapi.org).

#### Configuration
Once again, this requires an API key which can be obtained from NewsAPI. The service is currently hardcoded to select the top 10 headlines from the BBC but in later updates I will add functionality to get news about specific, and from other sources.

##### Local config

In your local.settings.json file add this in the values section:
```"news_key" : "<<PUT YOUR API KEY HERE>>"```

##### Server side setup

In the Application Settings area of the Azure Functions where you're deploying this you'll need to add a new setting with the name _news_key_ and the api key.

#### Usage

Just call the service endpoint and look at the JSON returned.
```https://<<your service URI>>/api/GetNews```

#### To Do
[Add details of the JSON returned]
[Add the ability to customise the news feed]

### GetHeating

Gets the current status of your Hive heating.

#### Configuration

To use this you'll need to set environment variables for your Hive credentials and the thermostat node ID.

##### Local config

In your local.settings.json file add these in the values section:
```"hive_username" : "<<YOUR HIVE USERNAME>>"```
```"hive_password" : "<<YOUR HIVE PASSWORD>>"```
```"hive_thermo_node" : "<<YOUR THERMOSTAT NODE ID>>"```
```"hive_base_url" : "https://api.prod.bgchprod.info:443/omnia"``` (Optional)

##### Server side setup

In the Application Settings area of the Azure Functions where you're deploying this you'll need to add new settings with the following names:
- _hive_username_
- _hive_password_
- _hive_thermo_node_
- _hive_base_url_ (Optional)

#### Usage

Just call the service endpoint and look at the JSON returned.
```https://<<your service URI>>/api/GetHeating```

Returned is a JSON object containing the current state of the thermostat.
```json
{
  "heat": "OFF",
  "mode": "HEAT",
  "target": "20",
  "temperature": "18.5",
  "name": "MyThermostat",
  "id": "123",
  "href": "http://test"
}
```

#### To Do

## The Code

C# .net Core 3.1 Azure Functions developed using VS Code.

## Why?

Well, I have an infoscren (see different github repo) and was looking to develop some more in different form factors. Rather than duplicating the Python code I wrote for that I thought that I should centralise them and then I can apply come centralised caching to reduce the load on the downstream services.

## Why Azure Functions?

I've not deployed an Azure Function for years so I thought that it might be nice to see what's new, plus it's got a fairly generous free tier.

## Why Github?

Well, why not make the code available to everyone. I'm not looking to build a platform but if anyone can sugest improvements, or if this code can help someone else, then why not?

## Where to next?

Well, I'd like to recode these in Python and Node.js as they're both interesting languages to develop for.