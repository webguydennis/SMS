﻿# SMS Gate Keeper

There are three projects which uses C#.net core 8 and Microsoft SQL Server. I am developing in Windows Operating Systems. 

## Projects

- [TextGateKeeper](#textgatekeeper)
- [TextGateKeeper.Tests](#textgatekeepertests)
- [TextGateKeeper.Worker](#textgatekeeperworker)

## TextGateKeeper

This project pretends to send SMS Text Messages with Twilio. Before it sends the SMS Text Message, some gate keeper rules are executed. This project uses Cors Policy, Repository Design Pattern, Entity Framework, and Controllers.
<br />
In this project, there are Web API Endpoints. 
- Health is a Get Request to return success. This tells the web api is running.
- Runs the Gate Keeper Rules before sending SMS Text Messages. It checks if the max limit is hit by Account or Phone on the last second. The max limit can be configure in the Appsettings.
- Removes the Inactive Text Messages. For this purpose, we considers the all SMS Text Messages from the day before as inactive. This can be configure in the Appsetting. 
<br />
The Gate Keeper Limits are configured in Appsetting.json. MaxLimitAccountPerSecond tells maximum number of text messages in an account from the last second. There is also MaxLimitPhonePerSecond which tells the maximum number of text messages per Phone on the last second. For the CutOffInDays, it tells which text message can be inactive. For example, anything from day before (current date - 1 CutOffInDays) can be considered as inactive. The inactive can be removed from the database.
<br /><br />
"MaxLimitAccountPerSecond": "2000",<BR />
"MaxLimitPhonePerSecond": "1000",<BR />
"CutOffInDays": "1"<BR />
 
### Prerequisites
 
We need a few nuget packages.<br />
```
dotnet add package Automapper
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Swashbuckle.AspNetCore
```

A Microsoft SQL Server is installed locally. In my case, the username and password is sa and Password_123# respectively. The database can be any name. In this project, the database name is TextGateKeeper. The database and tables 
will be created when you run the Entity Framework Database Migration. 
<br />
Set the database connection strings in the Appsettings.json. Feel free to change to Windows Authentication. In my case, the SQL Server is running in Docker. So, the username and password is specified in the Appsettings.
<br />
```
"DefaultConnection": "Server=localhost;Database=TextGateKeeper;TrustServerCertificate=true;User Id=sa;Password=Password_123#"
```
 
Install the Entity Framework CLI Tools if not installed yet.
<br />
```
dotnet tool install --global dotnet-ef
```

Create the database and tables by running the command in the terminal. This runs all the database migrations.
<br />
```
dotnet ef database update 
```

### Configuring the max limit on Account and Phone, and cut off days.
The cut off days tells when a Text Message is considered as inactive.

### Running Locally

Navigate to the TextGateKeeper Project from the Terminal. See example to navigate when you in sms folder below. 
<br />
```
cd .\TextGateKeeper\
```

Run the web api
<br />
```
dotnet run
```

Check if the web api is running by hitting Health API Endpoint.
<br />
```
curl -X 'GET' \
  'http://localhost:5000/Sms/Health' \
  -H 'accept: */*'
```

Send a text message via SendText API Endpoint.
curl -X 'POST' \
  'http://localhost:5000/Sms/SendText' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "body": "test 133",
  "phoneNumberFrom": "604-123-1234",
  "phoneNumberTo": "604-654-3210",
  "createdDate": "2024-11-07T00:53:14.440Z"
}'
```

Remove inactive text message via RemoveInActiveSms API Endpoint.
<br />
```
curl -X 'DELETE' \
  'http://localhost:5000/Sms/RemoveInActiveSms' \
  -H 'accept: */*'
```

## TextGateKeeper.Tests

The test uses NUnit for the Unit Test and moq for Mock.
 
### List of Test
- Able to get the max limit for an account on the last second of Appsettings.
- Able to get the max limit for an account on the last second of Appsettings.
- Able to get cut off days of Appsettings. In this project, we assume that any text message from the day before as inactive.
- Exceeds the max limit for an account on the last second and returns bad requests.
- Exceeds the max limit for a phone on the last second and returns bad requests.
- Valid requests and returns Ok.

### Prerequisites

We need a few nuget package
<br />
```
dotnet add package coverlet.collector
dotnet add package Microsoft.AspNetCore.Mvc
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package moq
dotnet add package NUnit
dotnet add package NUnit.Analyzers
dotnet add package NUnit3TestAdapter
dotnet add package NUnit.Framework
```

### Steps to run the tests

Navigate to the TextGateKeeper.Tests Project from the Terminal. See example to navigate when you in sms folder below. 
<br />
```
cd .\TextGateKeeper.Tests\
```

Run the tests.
<br />
```
dotnet test
```
 
## TextGateKeeper.Worker

This is a background worker service that removes inactive text messages by hitting an remove inactive endpoint from web api. So, the [TextGateKeeper](#textgatekeeper) must be running at the same time. It executes immediately. Then, it runs every 2am next day. 

### Prerequisites

We need a few nuget packages.
<br />
```
dotnet add package 
```

### Steps to run the background worker service.

Navigate to the TextGateKeeper.Worker Project from the Terminal. See example to navigate when you in sms folder below. 
<br />
```
cd .\TextGateKeeper.Worker\
```

Run the service.
<br />
```
dotnet run
```
