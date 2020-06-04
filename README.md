# infohub
(A centralised group of Azure Functions that feed an infoscreen)

## The APIs

### GetSunTimes

#### Description
Get the sunset and sunrise times for a given lattitude and longitude.

#### Usage
Provide the Query Params _lat_ and _lng_ with numerical values for for lattitude and longitude.

[TODO: Add example]

#### To Do
Take the undelying API URL out into env var.




## The Code

C# .net Core 3.1 Azure Functions developed using VS Code.

## Deployment

Currently deployed via an Azure Pipeline to an Azure Functions deployment slot.

[TODO: to write post deployment tests and, if passing, swap slots to take live.]

[TODO: Azure Function IaC code]