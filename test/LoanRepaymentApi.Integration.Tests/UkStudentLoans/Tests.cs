namespace LoanRepaymentApi.Integration.Tests.UkStudentLoans;

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using AwesomeAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;

public class Tests
{
    [Theory]
    [InlineData("calculate-type1", "Salary adjustment should modify amounts.")]
    [InlineData("calculate-type1-type2", "The type 2 interest rate should increase in the 2nd year, and paying off type 2 in the 2nd year should rollover a sufficient amount to pay off the type 1 in the same year.")]
    [InlineData("calculate-type1-not-enough-salary-at-start", "The first should not have any paid amount as not enough salary, further years have enough salary to start repaying.")]
    public async Task Calculate_WithHappyPath_ShouldReturnSuccessResponseWithCorrectData(string filenamePrefix, string because)
    {
        await using var application = new Application();

        // Arrange
        var client = application.CreateClient();

        var jsonRequest = await File.ReadAllTextAsync($"UkStudentLoans/{filenamePrefix}-request.json");
        var expectedJsonResponse = JToken.Parse(await File.ReadAllTextAsync($"UkStudentLoans/{filenamePrefix}-response.json"));

        // Act
        var response = await client.PostAsync("/ukstudentloans/calculate",
            new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

        // Assert
        var responseBody = JToken.Parse(await response.Content.ReadAsStringAsync());

        responseBody.Should().BeEquivalentTo(expectedJsonResponse, because);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Calculate_WithSadPath_ShouldReturnBadRequestAndErrors()
    {
        await using var application = new Application();

        // Arrange
        var client = application.CreateClient();

        var jsonRequest = await File.ReadAllTextAsync("UkStudentLoans/calculate-invalid-request.json");
        var expectedJsonResponse =
            JToken.Parse(await File.ReadAllTextAsync("UkStudentLoans/calculate-invalid-response.json"));
        expectedJsonResponse.Children<JProperty>().Single(x => x.Name == "traceId").Remove();

        // Act
        var response = await client.PostAsync("/ukstudentloans/calculate",
            new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseBody = JToken.Parse(await response.Content.ReadAsStringAsync());
        responseBody.Children<JProperty>().Single(x => x.Name == "traceId").Remove();

        responseBody.Should().BeEquivalentTo(expectedJsonResponse);
    }
}