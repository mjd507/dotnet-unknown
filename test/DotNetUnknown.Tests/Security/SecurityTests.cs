using System.Net;
using System.Net.Http.Headers;
using DotNetUnknown.Tests.Support;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DotNetUnknown.Tests.Security;

internal sealed class SecurityTests
{
    [OneTimeTearDown]
    public void ResetJwtToken()
    {
        TestProgram.SetDefaultJwtToken();
    }

    [Test]
    public async Task TestWithoutJwtToken()
    {
        // Given
        TestProgram.HttpClient.DefaultRequestHeaders.Authorization = null;
        // When
        var response = await TestProgram.HttpClient.GetAsync("security/admin");
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task TestWithNormalJwtToken_Forbidden()
    {
        // Given
        var jwtTokenTestSupport = TestProgram.GetRequiredService<JwtTokenTestSupport>();
        var normalUserToken = jwtTokenTestSupport.NormalUserToken;
        TestProgram.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, normalUserToken);
        // When
        var response = await TestProgram.HttpClient.GetAsync("security/admin");
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }


    [Test]
    public async Task TestWithAdminJwtToken_Ok()
    {
        // Given
        var jwtTokenTestSupport = TestProgram.GetRequiredService<JwtTokenTestSupport>();
        var adminUserToken = jwtTokenTestSupport.AdminUserToken;
        TestProgram.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, adminUserToken);
        // When
        var response = await TestProgram.HttpClient.GetAsync("security/admin");
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Is.EqualTo("this is admin user data"));
    }
}