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
        string expectedStart = "http://10.0.0.1/JSONGEN.HTML?FN=";

        string resultUrl = con.GetBaseUrl(); 

        Assert.Equal(expectedStart, resultUrl);
    }

    [Fact]
    public void ValidateBase64Transformation()
    {
        string expectedBase64String =  Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

        string resultBase64 = con.GetAuthHeaderValue();

        Assert.Equal(expectedBase64String,resultBase64);
    }
    
    [Fact]
    public void ValidateWriteUrlString()
    {
        string expectedWriteUrl = $"{con.GetBaseUrl()}Write&OA={base64Id};{value}&PIN={pin}";

        string resultBuildWriteUrl = con.BuildWriteUrl(base64Id,value);
        
        Assert.Equal(expectedWriteUrl,resultBuildWriteUrl);
    }
    
    [Fact]
    public void ValidateReadUrlString()
    {
        string expectedReadUrl = $"{con.GetBaseUrl()}Read&OA={base64Id}&PIN={pin}";

        string resultBuildReadUrl = con.BuildReadUrl(base64Id);
        
        Assert.Equal(expectedReadUrl,resultBuildReadUrl);
    }
    
    [Fact]
    public void ValidateFormatResultRead()
    {
        string[] formats = [
            """{"values":{"AyJaMNclDQE=":".IO.P.AI.TZS.Val"}}""",
            """{"values":{"AyJaMNclDQE=":"°C"}}""",
            """{"values":{"AyJaMNclDQE=":[5,5]}}""",
            """{"values":{"AyJaMNclDQE=":[100,100]}}""",
            """{"values":{"AyJaMNclDQE=":"100"}}"""
        ];
        
        string[] results = [".IO.P.AI.TZS.Val", "°C", "5", "100", "100"];
        string base64Id = "AyJaMNclDQE=";
        
        ApiResponse response = new ApiResponse();
        
        for (int i = 0; i < formats.Length; i++)
        {
            response.Content = formats[i];
            string expected = results[i];

            var formatedResponse = response.ToFormattedResult(false, base64Id, "test-url", ApiOperation.Read);

            Assert.Equal(expected, formatedResponse.ToString());
        }
    }

    [Fact]
    public void ValidateFormatResultWrite()
    {

        ApiResponse response = new();
        
        response.IsSuccess = true;
        
        var formatedResponse1 = response.ToFormattedResult(false, base64Id, "test-url", ApiOperation.Write);

        Assert.Equal("Success", formatedResponse1.ToString());



        response.IsSuccess = false;

        var formatedResponse2 = response.ToFormattedResult(false, base64Id, "test-url", ApiOperation.Write);

        Assert.Equal("Write Failed", formatedResponse2.ToString());
    }

}
