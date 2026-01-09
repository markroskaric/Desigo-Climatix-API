using System.Drawing;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DesigoClimatixApi.Tests;

public class ConnectionTest
{

    private const string username = "user";
    private const string  password = "pass";
    private const string  url = "http://10.0.0.1";
    private const string pin = "1234";
    private const string base64Id = "HAWDIKDAW=";
    private const string value = "32";
    readonly Connection con = new (username, password, url, pin);

    [Fact]
    public void UrlBuilder_ShouldReturnCorrectFormat()
    {
        string expectedStart = "http://10.0.0.1/JSONGEN.HTML";

        string resultUrl = con.GetBaseUrl(); 

        Assert.Contains(expectedStart, resultUrl);
    }

    [Fact]
    public void ValidateBase64Transformation()
    {
        string expectedBase64String =  Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

        string resultBase64 = con.GetAuthHeaderValue();

        Assert.Contains(expectedBase64String,resultBase64);
    }
    [Fact]
    public void ValidateWriteUrlString()
    {
        string expectedWriteUrl = $"{con.GetBaseUrl()}Write&OA={base64Id};{value}&PIN={pin}";

        string resultBuildWriteUrl = con.BuildWriteUrl(base64Id,value);
        
        Assert.Contains(expectedWriteUrl,resultBuildWriteUrl);
    }
    [Fact]
    public void ValidateReadUrlString()
    {
        string expectedReadUrl = $"{con.GetBaseUrl()}Read&OA={base64Id}&PIN={pin}";

        string resultBuildReadUrl = con.BuildReadUrl(base64Id);
        
        Assert.Contains(expectedReadUrl,resultBuildReadUrl);
    }
    // [Fact]
    // public void SendRequest_ShouldParseResponse_WhenEverythingIsOk()
    // {
       

    //     var con = new Connection("ADMIN", "SBTAdmin!", "http://192.168.1.113", "4000");

    //     string expected = ("[-2.38,-2.38]");
    //     object result = con.ReadValue("AyJaMNclAAE=");
        
    //     Assert.Equal(expected, result.ToString());
    // }

}
public class FakeSiemensHandler : HttpMessageHandler
{
    private readonly string _fakeResponse;
    private readonly HttpStatusCode _code;

    public FakeSiemensHandler(string response, HttpStatusCode code = HttpStatusCode.OK)
    {
        _fakeResponse = response;
        _code = code;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _code,
            Content = new StringContent(_fakeResponse)
        });
    }
}