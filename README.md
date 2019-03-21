# Blip.HttpClient [![Build status](https://ci.appveyor.com/api/projects/status/xg52i4obk27h92g9/branch/master?svg=true)](https://ci.appveyor.com/project/lfmundim/blip-httpclient/branch/master) [![codecov](https://codecov.io/gh/lfmundim/Blip.HttpClient/branch/master/graph/badge.svg)](https://codecov.io/gh/lfmundim/Blip.HttpClient) [![Nuget](https://img.shields.io/nuget/v/Blip.Httpclient.svg)](https://www.nuget.org/packages/blip.httpclient)
Take.BLiP.Client implementation to use Http calls instead of a TCP client.

Full documentation of supported methods can be found [here](https://docs.blip.ai), to be sent using the `ProcessCommandAsync`, `SendMessageAsync` and `SendNotificationAsync` by building the `Command` using the given Http Structure.

---

## Extra Services
Alongside with the simple `HttpClient`, this package provides you with implementations of BLiP's own extensions, such as the `IContactExtension`, with *overloads* for every method adding the support for *Serilog's* `ILogger` logging interface to enrich your debugging and logging experience.

Note that the logging structure (as of the `2.0.0` release) is set to use `Seq`s synthax:
```csharp
logger.Information("[DeleteContact] Trying delete contact using {@identity}", identity);
```

### Currently available services
|   BLiP                |   Package           |
|:---------------------:|:-------------------:|
| `IContactExtension`   | `IContactService`   |
| `IBroadcastExtension` | `IBroadcastService` |
| `IBucketExtension`    | `IBucketService`    |
| --------------------- | `IThreadService`    |
| `IEventTrackExtension`| `IEventTrackService`|

### Roadmap 
* `IContactExtension` ✅ (v2.0.0)
* `IBroadcastExtension` ✅ (v2.1.0)
* `IBucketExtension` ✅ (v2.2.0)
* Chat History Service (`/threads`) ✅ (v2.2.0)
* `IEventTrackExtension`✅ (v2.3.0) - PR by @[matheus-almeida-rosa](https://github.com/matheus-almeida-rosa)
* `IResourceExtension`
* `ISchedulerExtension`

Note that these are the Extensions BLiP provides that I am currently working on implementing with Logs. There are a few others not planned, but they may become available later.

### Issues
* #2 ✅ (v2.3.0)
