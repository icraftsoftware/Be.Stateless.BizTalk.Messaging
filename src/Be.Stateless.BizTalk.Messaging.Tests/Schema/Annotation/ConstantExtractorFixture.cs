﻿#region Copyright & License

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
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class ConstantExtractorFixture
	{
		[Fact]
		public void Equality()
		{
			new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write)
				.Should().Be(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write));
		}

		[Fact]
		public void InequalityOfExtractionMode()
		{
			new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write)
				.Should().NotBe(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Promote));
		}

		[Fact]
		public void InequalityOfProperty()
		{
			new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write)
				.Should().NotBe(new ConstantExtractor(new XmlQualifiedName("prop2", "urn"), "value", ExtractionMode.Write));
		}

		[Fact]
		public void InequalityOfType()
		{
			new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write)
				.Should().NotBe(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "*/node", ExtractionMode.Promote));

			new XPathExtractor(new XmlQualifiedName("prop", "urn"), "*/node", ExtractionMode.Promote)
				.Should().NotBe(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write));
		}

		[Fact]
		public void InequalityOfValue()
		{
			new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write)
				.Should().NotBe(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value2", ExtractionMode.Write));
		}
	}
}
