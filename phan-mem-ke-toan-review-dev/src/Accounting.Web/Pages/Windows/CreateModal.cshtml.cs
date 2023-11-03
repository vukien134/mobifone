using Accounting.Windows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.Windows
{
    public class CreateModalModel : AccountingPageModel
    {
        [BindProperty]
        public CrudWindowDto Window { get; set; }
        private readonly IWindowAppService _windowAppService;
        public CreateModalModel(IWindowAppService windowAppService)
        {
            _windowAppService = windowAppService;
        }
        public void OnGet()
        {
            Window = new CrudWindowDto();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _windowAppService.CreateAsync(Window);
            return NoContent();
        }

    }
}
