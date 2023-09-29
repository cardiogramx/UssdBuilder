using Microsoft.AspNetCore.Mvc;
using UssdBuilder.Models;
using UssdBuilder.Services;

namespace SampleApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefaultController : ControllerBase
    {
        private readonly IUssdServer<UssdRequest> server;

        public DefaultController(IUssdServer<UssdRequest> server)
        {
            this.server = server;

            AddRequiredRoutes();
            AddRequiredHandlers();
        }

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


        private void AddRequiredRoutes()
        {
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
        }

        private void AddRequiredHandlers()
        {
            this.server.AddHandlers("000", new()
            {
                 {"welcome", async (UssdScreen current, UssdRequest request) => {
                    
                    //do some async work
                    await DoSomeWork();

                    return new UssdResponse
                    {
                        Status = true,
                        Message = "CON Welcome to Sample Ussd.\nEnter \n1. To say hello \n2. To say goodbye \n3. To repeat"
                    };
                 }},

                 {"sayhello", async (UssdScreen current, UssdRequest request) => {
                    
                    //do some async work
                    await DoSomeWork();
                    
                    return new UssdResponse
                    {
                        Status = true,
                        Message = $"END Hello world. User input was {request.Text}"
                    };
                 }},
                 
                 {"goodbye", async (UssdScreen current, UssdRequest request) => {
                    
                    //do some async work
                    await DoSomeWork();
                    
                    return new UssdResponse
                    {
                        Status = true,
                        Message = $"END Goodbye. User input was {request.Text}"
                    };
                 }},
            });
        }
    
        private async Task DoSomeWork()
        {
            await Task.CompletedTask;
        }
    }
}