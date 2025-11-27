using System.Net;
using System.Net.Http.Headers;
using DotNetUnknown.Tests.Support;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DotNetUnknown.Tests.Security;

[TestFixture]
internal sealed class SecurityTests : MvcTestSupport
{
    [Test]
    public async Task TestWithoutJwtToken()
    {
        // Given
        HttpClient.DefaultRequestHeaders.Authorization = null;
        // When
        var response = await HttpClient.GetAsync("security/admin");
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task TestWithNormalJwtToken_Forbidden()
    {
        // Given
        var jwtTokenTestSupport = GetRequiredService<JwtTokenTestSupport>();
        var normalUserToken = jwtTokenTestSupport.NormalUserToken;
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, normalUserToken);
        // When
        var response = await HttpClient.GetAsync("security/admin");
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }


    [Test]
    public async Task TestWithAdminJwtToken_Ok()
    {
        // Given
        var jwtTokenTestSupport = GetRequiredService<JwtTokenTestSupport>();
        var adminUserToken = jwtTokenTestSupport.AdminUserToken;
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, adminUserToken);
        // When
        var response = await HttpClient.GetAsync("security/admin");
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Is.EqualTo("this is admin user data"));
    }
}