using AutoFixture;
using Microsoft.AspNetCore.Mvc;

namespace budorWeb.Api.Controllers
{
    [ApiController]
    public class BaseBudorController : ControllerBase
    {
        protected Fixture Fixture => new Fixture();
    }
}