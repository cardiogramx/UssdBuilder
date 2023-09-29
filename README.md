
# UssdBuilder

A lightweight and lightning-fast ussd library for advocates of clean and scalable code.

### INTRODUCTION

A ussd application is one of hardest api to build. Although the implementation starts fairly simple, it could quickly degenerate into a messy and complicated code of if-else webs.

This library intends to solve the issue above, enabling you build lightning-fast ussd apps quicker, while keeping it clean, readable, and scalable.


### SETUP

1. Install the [Nuget library](https://www.nuget.org/packages/UssdBuilder)
2. Go to your project's `Startup.cs` or `Program.cs` and add the code below

```javascript
builder.Services.AddDistributedMemoryCache(); //any implementation of IDistributedCache works, visit https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-6.0

builder.Services.AddUssdServer();
```

### USAGE

1. Resolve the injected ``IUssdServer`` instance
2. Declare your ussd routes

```javascript
this.server.AddRoute(new UssdRoute
{
    Code = "000",
    Prev = null, //no previous screen means this is a new ussd session
    Regx = (_, _) => true, //use this if you want your request to proceed regardless of user input
    Goto = "welcome"
});

//Go to "sayhello" if (ussdCode == "000" && prevScreen == "welcome" && userInput == 1)
this.server.AddRoute(new UssdRoute
{
    Code = "000",
    Prev = "welcome",
    Regx = (_, req) => req.Text == "1",
    Goto = "sayhello"
});

//Go to "goodbye" if (ussdCode == "000" && prevScreen == "welcome" && userInput == 2)
this.server.AddRoute(new UssdRoute
{
    Code = "000",
    Prev = "welcome",
    Regx = (_, req) => req.Text == "2",
    Goto = "goodbye"
});

// //Go to "goodbye" if (ussdCode == "000" && prevScreen == "welcome", regardless of user input)
// this.server.AddRoute(new UssdRoute
// {
//     Code = "000",
//     Prev = "welcome",
//     Regx = (_, _) => true, //use this if you want your request to proceed regardless of user input
//     Goto = "goodbye"
// });

//Go to "welcome" if (ussdCode == "000" && prevScreen == "welcome" && userInput != 1 && userInput != 2)
this.server.AddRoute(new UssdRoute
{
    Code = "000",
    Prev = "welcome",
    Regx = (_, req) => req.Text != "1" && req.Text != "2",  //Regx could also be used for input validation like input length check, etc.
    Goto = "welcome"
});
```

3. Add your route handlers

```javascript
this.server.AddHandlers("000", new()
{
     {"welcome", async (UssdScreen current, UssdRequest request) => {
         // do some async work
         return new UssdResponse
         {
            Status = true,
            Message = "CON Welcome.\nEnter \n1. To say hello \n2. To say goodbye \n3. To repeat"
         };
     }},

     {"sayhello", async (UssdScreen current, UssdRequest request) => {
         // do some async work
         return new UssdResponse
         {
            Status = true,
            Message = $"END Hello world. User input was {request.Text}"
         };
     }},
     
     {"goodbye", async (UssdScreen current, UssdRequest request) => {
         // do some async work
         return new UssdResponse
         {
            Status = true,
            Message = $"END Goodbye. User input was {request.Text}"
         };
     }},
});
```

4. Handle incoming request in your controller action

```javascript
[HttpPost("Handle")]
public async Task<IActionResult> Handle([FromBody] UssdRequest model)
{
    try
    {
        var result = await server.HandleAsync(model);
        return Ok(result);
    }
    catch (Exception ex)
    {
        await server.DeleteAsync(model.SessionId);
        return Ok($"END An error occurred: {ex.Message}");
    }
}
```
  
### Important note

1. Complex input (e.g \*0\*0*2#) is handled by default using `string.Split(...)` and iteration over splitted inputs. To disable this behaviour, use

```javascript
builder.Services.AddUssdServer(opt =>
{
    opt.EnableInputSplit = false;
});

```

2. The default sliding expiration of a ussd session 40 seconds. To override this session behaviour, use

```javascript
builder.Services.AddUssdServer(opt =>
{
    opt.CacheEntryOptions = new DistributedCacheEntryOptions
    {
        //...add your cache session options here
    };
});

```

3. A single ussd server or endpoint can serve multiple ussd codes, you just have to add routes and handlers for the different codes you want the server to process.

4. For the sake of clean code, `Route.Regx` is not meant to do more than input validation, do your complex work in your handler.

5. For more details, see [sample projects](https://github.com/cardiogramx/UssdBuilder/tree/master/sample)

6. PRs are welcome. 

7. Having a problem? Verify that you are implementing rightly, check sample projects, and if the problem persists, create an issue.

8. Want to hire competent engineer(s) for a ussd application? [Shoot me an email](mailto:kolawole.ox@gmail.com)