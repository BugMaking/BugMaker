﻿using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Abp.HtmlSanitizer.HtmlSanitizer;
using AbpAspNetCoreDemo.IntegrationTests.Data;
using AbpAspNetCoreDemo.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

public class SanitizerControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;
    private const string BaseUrl = "/SanitizerTest/";

    public SanitizerControllerTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [MemberData(nameof(SanitizerControllerData.SanitizerTestData), MemberType = typeof(SanitizerControllerData))]
    public async Task Sanitize_Html_Test(MyModel myInputModel, MyModel myExpectedModel)
    {
        // Arrange
        var client = _factory.CreateClient();

        var content = JsonContent.Create(new
        {
            myInputModel.HtmlInput, myInputModel.SecondInput,
        });

        // Act
        var response = await client.PostAsync(BaseUrl + "SanitizeHtmlTest", content);

        // Assert

        var json = await response.Content.ReadAsStringAsync();

        var myResult = JsonConvert.DeserializeObject<MyModel>(json);

        myResult.ShouldBeEquivalentTo(myExpectedModel);
    }

    [Theory]
    [MemberData(nameof(SanitizerControllerData.SanitizerTestPropertyData),
        MemberType = typeof(SanitizerControllerData))]
    public async Task Sanitize_Html_Property_Test(string firstInput, string secondInput, object myExpectedModel)
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.PostAsync(BaseUrl + "sanitizeHtmlPropertyTest", new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("firstInput", firstInput),
            new KeyValuePair<string, string>("secondInput", secondInput),
        }));

        // Assert

        var json = await response.Content.ReadAsStringAsync();

        var myResult = JsonConvert.DeserializeAnonymousType(json, new { firstInput = "", secondInput = "" } );

        myResult.ShouldBeEquivalentTo(myExpectedModel);
    }

    [Theory]
    [MemberData(nameof(SanitizerControllerData.SanitizerTestKeepChildNodesData),
        MemberType = typeof(SanitizerControllerData))]
    public async Task Sanitize_Html_Keep_Child_Nodes_Test(MyModel myInputModel, MyModel myExpectedModel)
    {
        // Arrange
        var client = _factory.CreateClient();

        var content = JsonContent.Create(new
        {
            myInputModel.HtmlInput,
            myInputModel.SecondInput
        });

        // Act
        var response = await client.PostAsync(BaseUrl + "sanitizeHtmlKeepChildNodesTest", content);

        // Assert

        var json = await response.Content.ReadAsStringAsync();

        var myResult = JsonConvert.DeserializeObject<MyModel>(json);

        myResult.ShouldBeEquivalentTo(myExpectedModel);
    }
    
    [Theory]
    [MemberData(nameof(SanitizerControllerData.SanitizerTestInnerModelData),
        MemberType = typeof(SanitizerControllerData))]
    public async Task Sanitize_Inner_Model_Test(MyModel myInputModel, MyModel myExpectedModel)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl + "sanitizeInnerModelTest", myInputModel);

        // Assert

        var json = await response.Content.ReadAsStringAsync();

        var myResult = JsonConvert.DeserializeObject<MyModel>(json);

        myResult.ShouldBeEquivalentTo(myExpectedModel);
    }
    
    [Theory]
    [MemberData(nameof(SanitizerControllerData.SanitizerTestAttributedModelData),
        MemberType = typeof(SanitizerControllerData))]
    public async Task Sanitize_Attributed_Model_Test(MyAttributedModel myInputModel, MyAttributedModel myExpectedModel)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl + "sanitizeAttributedModelTest", myInputModel);

        // Assert

        var json = await response.Content.ReadAsStringAsync();

        var myResult = JsonConvert.DeserializeObject<MyAttributedModel>(json);

        myResult.ShouldBeEquivalentTo(myExpectedModel);
    }
    
    [Theory]
    [MemberData(nameof(SanitizerControllerData.SanitizerTestAttributedPropertyModelData),
        MemberType = typeof(SanitizerControllerData))]
    public async Task Sanitize_Attributed_Property_Model_Test(MyAttributedPropertyModel myInputModel, MyAttributedPropertyModel myExpectedModel)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(BaseUrl + "sanitizeAttributedPropertyModelTest", myInputModel);

        // Assert

        var json = await response.Content.ReadAsStringAsync();

        var myResult = JsonConvert.DeserializeObject<MyAttributedPropertyModel>(json);

        myResult.ShouldBeEquivalentTo(myExpectedModel);
    }
}