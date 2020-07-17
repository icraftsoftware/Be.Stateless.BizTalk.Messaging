#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
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

using System.Xml;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	public class PropertyExtractorFixture
	{
		[Fact]
		public void Equality()
		{
			new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear)
				.Should().Be(new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear));
		}

		[Fact]
		public void InequalityOfExtractionMode()
		{
			new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear)
				.Should().NotBe(new PropertyExtractor(new("prop", "urn"), ExtractionMode.Ignore));
		}

		[Fact]
		public void InequalityOfProperty()
		{
			new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear)
				.Should().NotBe(new PropertyExtractor(new("prop2", "urn"), ExtractionMode.Clear));
		}

		[Fact]
		public void InequalityOfType()
		{
			new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear)
				.Should().NotBe(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Clear));

			new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Clear)
				.Should().NotBe(new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear));

			new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear)
				.Should().NotBe(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "*/node", ExtractionMode.Clear));

			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "*/node", ExtractionMode.Clear)
				.Should().NotBe(new PropertyExtractor(new("prop", "urn"), ExtractionMode.Clear));
		}
	}
}
