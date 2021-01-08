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
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class QNameValueExtractorFixture
	{
		[Fact]
		public void Equality()
		{
			new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name)
				.Should().Be(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name));
		}

		[Fact]
		public void ExecuteDemotesLocalNameValueInContext()
		{
			var messageContextMock = new MessageContextMock();
			messageContextMock.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation)).Returns("new-value");
			var newValue = string.Empty;

			var sut = new QNameValueExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/to", ExtractionMode.Demote, QNameValueExtractionMode.LocalName);
			sut.Execute(messageContextMock.Object, "value", ref newValue);

			messageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.OutboundTransportLocation, It.IsAny<string>()), Times.Never);
			newValue.Should().Be("new-value");
		}

		[Fact]
		public void ExecuteDemotesLocalNameValueInContextAndKeepOriginalPrefix()
		{
			var messageContextMock = new MessageContextMock();
			messageContextMock.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation)).Returns("new-value");
			var newValue = string.Empty;

			var sut = new QNameValueExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/to", ExtractionMode.Demote, QNameValueExtractionMode.LocalName);
			sut.Execute(messageContextMock.Object, "ns:value", ref newValue);

			messageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.OutboundTransportLocation, It.IsAny<string>()), Times.Never);
			newValue.Should().Be("ns:new-value");
		}

		[Fact]
		public void ExecutePromotesOrWritesLocalNameValueInContext()
		{
			var messageContextMock = new MessageContextMock();
			var newValue = string.Empty;

			var sut = new QNameValueExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/to", ExtractionMode.Promote, QNameValueExtractionMode.LocalName);
			sut.Execute(messageContextMock.Object, "ns:value", ref newValue);

			messageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.OutboundTransportLocation, "value"));
			newValue.Should().BeEmpty();
		}

		[Fact]
		public void InequalityOfExtractionMode()
		{
			new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name)
				.Should().NotBe(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote, QNameValueExtractionMode.Name));
		}

		[Fact]
		public void InequalityOfProperty()
		{
			new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name)
				.Should().NotBe(new QNameValueExtractor(new XmlQualifiedName("prop2", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name));
		}

		[Fact]
		public void InequalityOfQNameValueExtractionMode()
		{
			new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name)
				.Should().NotBe(
					new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.LocalName));
		}

		[Fact]
		public void InequalityOfType()
		{
			new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote, QNameValueExtractionMode.Name)
				.Should().NotBe(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote));

			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote)
				.Should().NotBe(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote, QNameValueExtractionMode.Name));
		}

		[Fact]
		public void InequalityOfXPathExpression()
		{
			new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name)
				.Should().NotBe(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'another']", ExtractionMode.Write, QNameValueExtractionMode.Name));
		}
	}
}
