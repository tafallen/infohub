# infohub
(A centralised group of Azure Functions that feed an infoscreen)

## The APIs

### GetSunTimes

#### Description
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



### GetWeather

#### Description

#### Usage

[TODO: Add example]

#### To Do


### GetNews

#### Description

#### Usage

[TODO: Add example]

#### To Do


### GetHeating

#### Description

#### Usage

[TODO: Add example]

#### To Do




## The Code

C# .net Core 3.1 Azure Functions developed using VS Code.

## Deployment

Currently deployed via an Azure Pipeline to an Azure Functions deployment slot.

[TODO: to write post deployment tests and, if passing, swap slots to take live.]

[TODO: Azure Function IaC code]