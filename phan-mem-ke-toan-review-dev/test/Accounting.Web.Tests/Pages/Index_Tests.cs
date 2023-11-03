using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Accounting.Pages;

public class Index_Tests : AccountingWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
