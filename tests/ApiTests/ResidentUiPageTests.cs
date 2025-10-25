using System.Threading.Tasks;
using HouseHelp.ApiTests.Fixtures;
using Xunit;

namespace HouseHelp.ApiTests;

public class ResidentUiPageTests : IClassFixture<ApiDatabaseFixture>
{
    private readonly ApiDatabaseFixture _fixture;

    public ResidentUiPageTests(ApiDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ResidentDashboard_StaticAssetServed()
    {
        var client = _fixture.Factory.CreateClient();
        var response = await client.GetAsync("/ui/resident-dashboard.html");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Resident Booking Dashboard", html);
        Assert.Contains("resident-dashboard.js", html);
    }
}
