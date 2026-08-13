using zogo.Application.Common;

namespace zogo.UnitTests.Application;

public class ApiResponseTests
{
    [Fact]
    public void Ok_ShouldReturnSuccessResponse()
    {
        var response = ApiResponse<string>.Ok("data", "Success");

        Assert.True(response.Success);
        Assert.Equal("data", response.Data);
        Assert.Equal("Success", response.Message);
    }

    [Fact]
    public void Fail_ShouldReturnFailureResponse()
    {
        var errors = new List<string> { "Error 1" };
        var response = ApiResponse<string>.Fail("Failed", errors);

        Assert.False(response.Success);
        Assert.Equal("Failed", response.Message);
        Assert.Equal(errors, response.Errors);
    }
}
