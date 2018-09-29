# Blip.HttpClient [![Build status](https://ci.appveyor.com/api/projects/status/xg52i4obk27h92g9/branch/master?svg=true)](https://ci.appveyor.com/project/lfmundim/blip-httpclient/branch/master) [![codecov](https://codecov.io/gh/lfmundim/Blip.HttpClient/branch/master/graph/badge.svg)](https://codecov.io/gh/lfmundim/Blip.HttpClient) [![Nuget](https://img.shields.io/nuget/v/Blip.Httpclient.svg)](https://www.nuget.org/packages/blip.httpclient)
Take.BLiP.Client implementation to use Http calls instead of instantiating an actual client.

Full documentation of supported methods can be found [here](https://docs.blip.ai)

# Methods
## `ProcessCommandAsync`
Sends a LIME Command to the `/commands` endpoint and receives another LIME Command response with the fetched data. Examples below.

HTTP:
```http
POST https://msging.net/commands HTTP/1.1
Content-Type: application/json
Authorization: Key {YOUR_TOKEN}

{  
  "id": "2",
  "method": "get",
  "uri": "/contacts"
}
```

CSharp (Using this package):
```csharp
// {...}
    var factory = new BlipHttpClientFactory();
        _client = factory.BuildBlipHttpClient("{YOUR_TOKEN}");
        var command = new Command
        {
            Method = CommandMethod.Get,
            Uri = new LimeUri(commandSuffix)
        };
        var resp = await _client.ProcessCommandAsync(command, CancellationToken.None);
// {...}
```
> Gets all contacts from a Bot's contact list

## `SendCommandAsync`
Sends a LIME Command to the `/commands` endpoint without expecting any return.

## `SendMessageAsync`
Sends a message to a set user on behalf of the Bot.

## `SendNotificationAsync`
