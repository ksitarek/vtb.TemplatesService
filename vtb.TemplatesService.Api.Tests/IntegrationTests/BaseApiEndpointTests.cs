﻿using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using vtb.Auth.Jwt;
using vtb.TemplatesService.Api.Responses;
using vtb.TemplatesService.Api.Tests.IntegrationTests.ExpectedResults;

namespace vtb.TemplatesService.Api.Tests.IntegrationTests
{
    public abstract class BaseApiEndpointTests
    {
        internal ApiFactory _factory;
        protected HttpClient _httpClient;

        private ITokenGenerator _tokenGenerator;

        private ITokenGenerator TokenGenerator
        {
            get
            {
                if (_tokenGenerator == null)
                {
                    var sp = _factory.Services;

                    ISystemClock systemClock = sp.GetService<ISystemClock>();
                    IOptions<JwtSettings> jwtOptions = sp.GetService<IOptions<JwtSettings>>();

                    _tokenGenerator = new TokenGenerator(systemClock, jwtOptions);
                }

                return _tokenGenerator;
            }
        }

        [OneTimeSetUp]
        public void SetUpHarness()
        {
            _factory = new ApiFactory();
        }

        [SetUp]
        public void SetUpHttpClient()
        {
            _httpClient = _factory.CreateClient();
        }

        [TearDown]
        public void TearDownHttpClient()
        {
            _httpClient?.Dispose();
        }

        [OneTimeTearDown]
        public void TearDownHarness()
        {
            _factory?.Dispose();
        }

        protected void Authorize(Guid userId, Guid tenantId, string[] roles, string[] permissions)
        {
            var token = TokenGenerator.GetJwt(userId, tenantId, roles, permissions);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token.Token}");
        }

        protected void AuthorizeSystem(Guid tenantId)
        {
            var token = TokenGenerator.GetSystemJwt(tenantId, Guid.NewGuid().ToString());

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token.Token}");
        }

        internal async Task TestPagination<TListItem, TExpectedListItem>(Func<int, int, ValueTask<ListPage<TListItem>>> func, List<TExpectedListItem> allRecords)
        {
            var totalCount = allRecords.Count;
            var pageSize = (int)Math.Ceiling(totalCount / 2M);

            var expectedResults = new[]
            {
                new ExpectedListPage<TExpectedListItem>(2, allRecords.Take(pageSize).ToList()),
                new ExpectedListPage<TExpectedListItem>(2, allRecords.Skip(pageSize).Take(pageSize).ToList()),
                new ExpectedListPage<TExpectedListItem>(2, allRecords),
                new ExpectedListPage<TExpectedListItem>(2, allRecords),
                new ExpectedListPage<TExpectedListItem>(2, new List<TExpectedListItem>() { })
            };

            var tasks = new[]
            {
                func.Invoke(1, pageSize),
                func.Invoke(2, pageSize),
                func.Invoke(1, totalCount),
                func.Invoke(1, totalCount * 2),
                func.Invoke(3, pageSize)
            };

            var results = await Task.WhenAll(tasks.Select(t => t.AsTask()));

            for (var i = 0; i < results.Length; i++)
            {
                results[i].Should().BeEquivalentTo(expectedResults[i]);
            }
        }
    }
}