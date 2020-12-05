using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using vtb.TemplatesService.BusinessLogic.Exceptions;
using vtb.TemplatesService.DomainModel;

namespace vtb.TemplatesService.BusinessLogic.Tests.Managers
{
    public class TemplateManager_Create : TemplateManagerTests
    {
        [Test]
        public async Task Will_Build_And_Insert_New_Template_To_Repository()
        {
            var templateId = Guid.NewGuid();
            var templateKindKey = "tk1";
            var templateLabel = "First Template";
            var templateContent = "Lorem ipsum, dolor sit amet.";

            var ct = CancellationToken.None;

            var expectedTemplate = new Template()
            {
                TemplateId = templateId,
                TemplateKindKey = templateKindKey,
                Label = templateLabel,
                Versions = new List<TemplateVersion>()
                {
                    new TemplateVersion()
                    {
                        TemplateVersionId = It.IsAny<Guid>(),
                        CreatedAt = _utcNow,
                        Version = 1,
                        Content = templateContent,
                        IsActive = true
                    }
                },
            };

            _templatesRepositoryMock.Setup(x => x.AddTemplate(It.IsAny<Template>(), ct));

            await _manager.CreateTemplate(templateId, templateKindKey, templateLabel, templateContent, ct);

            _templatesRepositoryMock.Verify(x => x.AddTemplate(
                It.Is<Template>(t =>
                    t.TemplateId != Guid.Empty &&
                    t.TemplateKindKey == templateKindKey &&
                    t.Label == templateLabel &&
                    t.Versions.Count == 1 &&

                    t.ActiveVersion.TemplateVersionId != Guid.Empty &&
                    t.ActiveVersion.CreatedAt == _utcNow &&
                    t.ActiveVersion.Version == 1 &&
                    t.ActiveVersion.Content == templateContent &&
                    t.ActiveVersion.IsActive
                ),
                ct
            ));
        }

        [Test]
        public void Will_Throw_TemplateCreationFailedException_When_Add_To_Repository_Failed()
        {
            var templateId = Guid.NewGuid();
            var templateKindKey = "tk1";
            var templateLabel = "First Template";
            var templateContent = "Lorem ipsum, dolor sit amet.";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.AddTemplate(It.IsAny<Template>(), ct)).Throws(new Exception());

            Assert.ThrowsAsync<TemplateCreationFailedException>(async () => await _manager.CreateTemplate(templateId, templateKindKey, templateLabel, templateContent, CancellationToken.None));
        }

        [Test]
        public void Will_Throw_TemplateLabelAlreadyTakenException_When_Conflicting_Label()
        {
            var templateId = Guid.NewGuid();
            var templateKindKey = "tk1";
            var templateLabel = "First Template";
            var templateContent = "Lorem ipsum, dolor sit amet.";
            var ct = CancellationToken.None;

            _templatesRepositoryMock.Setup(x => x.TemplateLabelTaken(templateLabel, ct)).ReturnsAsync(true);

            Assert.ThrowsAsync<TemplateLabelAlreadyTakenException>(async () => await _manager.CreateTemplate(templateId, templateKindKey, templateLabel, templateContent, CancellationToken.None));
        }

        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", "", "a", "b")]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", null, "a", "b")]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", " ", "a", "b")]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", "a", "", "b")]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", "a", null, "b")]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", "a", " ", "b")]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", "a", "b", "")]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", "a", "b", null)]
        [TestCase("b560bed1-bd48-44c5-8345-7ed77fc991ae", "a", "b", " ")]
        [TestCase("00000000-0000-0000-0000-000000000000", "a", "b", "c")]
        public void Will_Throw_ArgumentException_On_Invalid_Input(string guidString, string templateKindKey, string label, string content)
        {
            var templateId = Guid.Parse(guidString);
            Assert.ThrowsAsync<ArgumentException>(async () => await _manager.CreateTemplate(templateId, templateKindKey, label, content, CancellationToken.None));
        }
    }
}