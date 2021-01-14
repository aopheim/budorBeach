using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace budorWeb.Pages
{
    public class SignalRTestModel : PageModel
    {
        private readonly ILogger<SignalRTestModel> _logger;

        public SignalRTestModel(ILogger<SignalRTestModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}