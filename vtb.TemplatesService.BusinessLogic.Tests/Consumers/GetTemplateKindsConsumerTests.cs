namespace vtb.TemplatesService.BusinessLogic.Tests.Consumers
{
    //public class GetTemplateKindsConsumerTests
    //{
    //    private InMemoryTestHarness _harness;
    //    private ConsumerTestHarness<GetTemplateKindsConsumer> _consumerHarness;
    //    private Mock<ITemplateKindsRepository> _repositoryMock;

    //    [SetUp]
    //    public async Task SetUpAsync()
    //    {
    //        _repositoryMock = new Mock<ITemplateKindsRepository>();

    //        _harness = new InMemoryTestHarness();
    //        _consumerHarness = _harness.Consumer(() => new GetTemplateKindsConsumer(_repositoryMock.Object));
    //        await _harness.Start();
    //    }

    //    [TearDown]
    //    public async Task TearDownAsync()
    //    {
    //        await _harness.Stop();
    //    }

    //    [Test]
    //    public async Task WillRespondWithPagedResult()
    //    {
    //        var returnedEntities = new List<TemplateKind>(new[] {
    //            new TemplateKind() { TemplateKindKey = "tk1" },
    //            new TemplateKind() { TemplateKindKey = "tk2" }
    //        });

    //        _repositoryMock.Setup(x => x.GetTemplateKindsPage(1, 10))
    //            .ReturnsAsync(new Page<TemplateKind>(2, returnedEntities));

    //        await _harness.InputQueueSendEndpoint.Send<IPagedRequest<TemplateKind>>(new
    //        {
    //            Page = 1,
    //            PageSize = 10
    //        });

    //        (await _consumerHarness.Consumed.Any<IPagedRequest<TemplateKind>>()).Should().BeTrue();
    //        (await _harness.Consumed.Any<IPagedRequest<TemplateKind>>()).Should().BeTrue();
    //        (await _harness.Published.Any<IPaginatedResult<TemplateKind>>(x =>
    //            x.Context.Message.Entities.Count() == 2 &&
    //            x.Context.Message.Entities.ElementAt(0) == returnedEntities[0] &&
    //            x.Context.Message.Entities.ElementAt(1) == returnedEntities[1] &&
    //            x.Context.Message.TotalCount == 2 &&
    //            x.Context.Message.Page == 1 &&
    //            x.Context.Message.PageSize == 10
    //        )).Should().BeTrue();
    //    }

    //    [TestCase(1, 0)]
    //    [TestCase(0, 1)]
    //    public async Task WontRespondIfParametersInvalid(int page, int pageSize)
    //    {
    //        await _harness.InputQueueSendEndpoint.Send<IPagedRequest<TemplateKind>>(new
    //        {
    //            Page = page,
    //            PageSize = pageSize
    //        });

    //        (await _consumerHarness.Consumed.Any<IPagedRequest<TemplateKind>>()).Should().BeTrue();
    //        (await _harness.Consumed.Any<IPagedRequest<TemplateKind>>()).Should().BeTrue();
    //        (await _harness.Published.Any<IPaginatedResult<TemplateKind>>()).Should().BeFalse();
    //    }

    //    [Test]
    //    public async Task WillRespondWithEmptyPagedResultIfRepositoryFailed()
    //    {
    //        _repositoryMock.Setup(x => x.GetTemplateKindsPage(1, 10))
    //            .Throws(new Exception());

    //        await _harness.InputQueueSendEndpoint.Send<IPagedRequest<TemplateKind>>(new
    //        {
    //            Page = 1,
    //            PageSize = 10
    //        });

    //        (await _consumerHarness.Consumed.Any<IPagedRequest<TemplateKind>>()).Should().BeTrue();
    //        (await _harness.Consumed.Any<IPagedRequest<TemplateKind>>()).Should().BeTrue();
    //        (await _harness.Published.Any<IPaginatedResult<TemplateKind>>(x =>
    //            x.Context.Message.Entities.Count() == 0 &&
    //            x.Context.Message.TotalCount == 0 &&
    //            x.Context.Message.Page == 1 &&
    //            x.Context.Message.PageSize == 10
    //        )).Should().BeTrue();
    //    }
    //}
}