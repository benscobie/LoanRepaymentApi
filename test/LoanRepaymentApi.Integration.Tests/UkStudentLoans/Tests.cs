namespace LoanRepaymentApi.Integration.Tests.UkStudentLoans;

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;

public class Tests
{
    [Fact]
    public async Task Calculate_WithSingleLoan_ShouldReturnSuccessResponseWithCorrectData()
    {
        await using var application = new Application();

        // Arrange
        var client = application.CreateClient();

        var jsonRequest = await File.ReadAllTextAsync("UkStudentLoans/calculate-request.json");
        var expectedJsonResponse = JToken.Parse(await File.ReadAllTextAsync("UkStudentLoans/calculate-response.json"));

        // Act
        var response = await client.PostAsync("/ukstudentloans/calculate",
            new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = JToken.Parse(await response.Content.ReadAsStringAsync());

        responseBody.Should().BeEquivalentTo(expectedJsonResponse);
    }

    [Fact]
    public async Task Calculate_WithInvalidRequest_ShouldReturnBadRequestAndErrors()
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