namespace LoanRepaymentApi.Integration.Tests.UkStudentLoans;

using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;

public class Tests
{
    [Fact]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
    {
        await using var application = new Application();
        
        // Arrange
        var client = application.CreateClient();

        var jsonRequest = await File.ReadAllTextAsync("UkStudentLoans/calculate-request.json");
        var expectedJsonResponse = JToken.Parse(await File.ReadAllTextAsync("UkStudentLoans/calculate-response.json"));

        // Act
        var response = await client.PostAsync("/ukstudentloans", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = JToken.Parse(await response.Content.ReadAsStringAsync());
    
        responseBody.Should().BeEquivalentTo(expectedJsonResponse);
    }
}