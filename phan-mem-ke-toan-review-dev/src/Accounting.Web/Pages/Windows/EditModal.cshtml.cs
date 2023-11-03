using Accounting.Windows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Accounting.Web.Pages.Windows
{
    public class EditModalModel : AccountingPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty]
        public CrudWindowDto Window { get; set; }

        private readonly IWindowAppService _windowAppService;

        public EditModalModel(IWindowAppService windowAppService)
        {
            _windowAppService = windowAppService;
        }

        public async Task OnGetAsync()
        {
            var windowDto = await _windowAppService.GetByIdAsync(Id);
            Window = ObjectMapper.Map<WindowDto, CrudWindowDto>(windowDto);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _windowAppService.UpdateAsync(Id, Window);
            return NoContent();
        }
    }
}
