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
using Be.Stateless.BizTalk.Unit;
using FluentAssertions;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class XPathExtractorFixture
	{
		[Fact]
		public void DemoteDoesNotThrowIfNewValueIsNullOrEmpty()
		{
			var messageMock = new MessageMock();
			var sut = new XPathExtractor(BizTalkFactoryProperties.CorrelationId.QName, "//value1", ExtractionMode.Demote);
			string newValue = null;
			Action(() => sut.Execute(messageMock.Object.Context, "old", ref newValue)).Should().NotThrow();
			newValue.Should().BeNull();
		}

		[Fact]
		public void Equality()
		{
			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)
				.Should().Be(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write));
		}

		[Fact]
		public void InequalityOfExtractionMode()
		{
			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)
				.Should().NotBe(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote));
		}

		[Fact]
		public void InequalityOfProperty()
		{
			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)
				.Should().NotBe(new XPathExtractor(new XmlQualifiedName("prop2", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write));
		}

		[Fact]
		public void InequalityOfType()
		{
			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)
				.Should().NotBe(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote));

			new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote)
				.Should().NotBe(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write));
		}

		[Fact]
		public void InequalityOfXPathExpression()
		{
			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)
				.Should().NotBe(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'another']", ExtractionMode.Write));
		}
	}
}
