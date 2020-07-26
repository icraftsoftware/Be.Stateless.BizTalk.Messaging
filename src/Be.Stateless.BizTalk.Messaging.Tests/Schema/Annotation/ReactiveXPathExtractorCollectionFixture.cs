#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.BizTalk.Unit.Message;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class ReactiveXPathExtractorCollectionFixture
	{
		[Fact]
		public void Match()
		{
			var extractors = new[] {
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
				new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/to", ExtractionMode.Promote),
				new XPathExtractor(SBMessagingProperties.ContentType.QName, "/letter/*/subject", ExtractionMode.Write),
				new XPathExtractor(SBMessagingProperties.CorrelationId.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(SBMessagingProperties.Label.QName, "/letter/*/salutations", ExtractionMode.Write),
				new XPathExtractor(BtsProperties.Operation.QName, "/letter/*/signature", ExtractionMode.Write)
			};

			using (var stream = XPathMutatorStreamFactory.Create(new StringStream(UNQUALIFIED_LETTER), extractors, () => MessageContextMock.Object))
			{
				stream.Drain();
			}

			MessageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.SenderName, "info@world.com"));
			MessageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.ReceiverName, "francois.chabot@gmail.com"));
			MessageContextMock.Verify(c => c.SetProperty(SBMessagingProperties.ContentType, "inquiry"));
			MessageContextMock.Verify(c => c.SetProperty(SBMessagingProperties.CorrelationId, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(SBMessagingProperties.Label, "King regards,"));
			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.Operation, "John Doe"));
		}

		[Fact]
		public void MatchAndDemote()
		{
			var extractors = new[] {
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/paragraph[1]", ExtractionMode.Demote),
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/paragraph[2]", ExtractionMode.Demote),
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/paragraph[3]", ExtractionMode.Demote)
			};

			using (var stream = XPathMutatorStreamFactory.Create(new StringStream(UNQUALIFIED_LETTER), extractors, () => MessageContextMock.Object))
			using (var reader = new StreamReader(stream))
			{
				MessageContextMock.Setup(c => c.GetProperty(BizTalkFactoryProperties.SenderName)).Returns("same-paragraph");
				reader.ReadToEnd()
					.Should().Be(
						UNQUALIFIED_LETTER
							.Replace("paragraph-one", "same-paragraph")
							.Replace("paragraph-two", "same-paragraph")
							.Replace("paragraph-six", "same-paragraph")
					);
			}
		}

		[Fact]
		public void MatchForGroup()
		{
			var extractors = new[] {
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.EnvironmentTag.QName, "/letter/*/paragraph", ExtractionMode.Write)
			};

			using (var stream = XPathMutatorStreamFactory.Create(new StringStream(UNQUALIFIED_LETTER), extractors, () => MessageContextMock.Object))
			{
				stream.Drain();
			}

			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.SenderName, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.ReceiverName, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.EnvironmentTag, "paragraph-one"));
		}

		[Fact]
		public void MatchQualified()
		{
			var extractors = new[] {
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/*[local-name()='letter']/*/*[local-name()='subject']", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph']", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.EnvironmentTag.QName, "/*[local-name()='letter']/*/*[local-name()='signature']", ExtractionMode.Write)
			};

			using (var stream = XPathMutatorStreamFactory.Create(new StringStream(QUALIFIED_LETTER), extractors, () => MessageContextMock.Object))
			{
				stream.Drain();
			}

			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.SenderName, "inquiry"));
			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.ReceiverName, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.EnvironmentTag, "John Doe"));
		}

		[Fact]
		public void MatchWithPositionWhenQualified()
		{
			var extractors = new[] {
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph'][1]", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph'][2]", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.EnvironmentTag.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph'][3]", ExtractionMode.Write)
			};

			using (var stream = XPathMutatorStreamFactory.Create(new StringStream(QUALIFIED_LETTER), extractors, () => MessageContextMock.Object))
			{
				stream.Drain();
			}

			// !!IMPORTANT!! XPathMutatorStream does not support such expressions and henceforth does not perform any succeeding match
			MessageContextMock.Verify(c => c.Promote(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
			MessageContextMock.Verify(c => c.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
		}

		[Fact]
		public void MatchWithPositionWhenUnqualified()
		{
			var extractors = new[] {
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/paragraph[1]", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/paragraph[2]", ExtractionMode.Write),
				new XPathExtractor(BizTalkFactoryProperties.EnvironmentTag.QName, "/letter/*/paragraph[3]", ExtractionMode.Write)
			};

			using (var stream = XPathMutatorStreamFactory.Create(new StringStream(UNQUALIFIED_LETTER), extractors, () => MessageContextMock.Object))
			{
				stream.Drain();
			}

			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.SenderName, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.ReceiverName, "paragraph-six"));
			MessageContextMock.Verify(c => c.SetProperty(BizTalkFactoryProperties.EnvironmentTag, "paragraph-two"));
		}

		public ReactiveXPathExtractorCollectionFixture()
		{
			MessageContextMock = new MessageContextMock();
		}

		private MessageContextMock MessageContextMock { get; }

		private const string QUALIFIED_LETTER = @"<s1:letter xmlns:s1='urn-one' xmlns:s2='urn-two' xmlns:s6='urn-six' xmlns:s0='urn-ten'>
  <s2:headers>
    <s6:subject>inquiry</s6:subject>
    <s6:from>info@world.com</s6:from>
    <s6:to>francois.chabot@gmail.com</s6:to>
  </s2:headers>
  <s2:body>
    <s0:paragraph>paragraph-one</s0:paragraph>
    <s0:paragraph>paragraph-two</s0:paragraph>
    <s0:paragraph>paragraph-six</s0:paragraph>
  </s2:body>
  <s2:footers>
    <s6:salutations>King regards,</s6:salutations>
    <s6:signature>John Doe</s6:signature>
  </s2:footers>
</s1:letter>";

		private const string UNQUALIFIED_LETTER = @"<letter>
	<body>
		<paragraph>paragraph-one</paragraph>
		<paragraph>paragraph-six</paragraph>
		<paragraph>paragraph-two</paragraph>
	</body>
	<footers>
		<salutations>King regards,</salutations>
		<signature>John Doe</signature>
	</footers>
	<headers>
		<from>info@world.com</from>
		<subject>inquiry</subject>
		<to>francois.chabot@gmail.com</to>
	</headers>
</letter>";
	}
}
