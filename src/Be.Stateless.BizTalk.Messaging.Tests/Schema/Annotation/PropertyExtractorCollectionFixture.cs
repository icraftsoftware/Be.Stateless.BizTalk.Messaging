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

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using FluentAssertions;
using Microsoft.BizTalk.XPath;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Schema.Annotation
{
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class PropertyExtractorCollectionFixture
	{
		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "AccessToModifiedClosure")]
		public void PropertyExtractorCollectionEmptyIsImmutable()
		{
			var sut = PropertyExtractorCollection.Empty;

			sut.Any().Should().BeFalse();
			sut.Should().BeSameAs(PropertyExtractorCollection.Empty);
			using (var reader = XmlReader.Create(new StringReader("")))
			{
				Invoking(() => sut.ReadXml(reader)).Should().Throw<NotSupportedException>();
			}

			sut = new PropertyExtractor[] { new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write) };
			sut.Should().NotBeSameAs(PropertyExtractorCollection.Empty);
		}

		[Fact]
		public void ReadXml()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 xpath='*/some-node'/>"
				+ "<s0:Property2 promoted='true' xpath='*/other-node'/>"
				+ "<s0:Property3 mode='write' value='constant'/>"
				+ "<s0:Property4 mode='clear'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(
					new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
					new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote),
					new ConstantExtractor(new XmlQualifiedName("Property3", "urn"), "constant", ExtractionMode.Write),
					new PropertyExtractor(new XmlQualifiedName("Property4", "urn"), ExtractionMode.Clear)
				);
			}
		}

		[Fact]
		public void ReadXmlForConstantExtractor()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 value='constant'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new ConstantExtractor(new XmlQualifiedName("Property1", "urn"), "constant", ExtractionMode.Write));
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForConstantExtractorThrowsWhenModeAttributeIsInvalid()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 mode='demote' value='constant'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader)).Should().Throw<ArgumentException>().WithMessage("ExtractionMode 'Demote' is not supported by ConstantExtractor.*");
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForConstantExtractorThrowsWhenValueIsEmpty()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 value=''/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader)).Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "value");
			}
		}

		[Fact]
		public void ReadXmlForConstantExtractorWithModeAttribute()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 mode='promote' value='constant'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new ConstantExtractor(new XmlQualifiedName("Property1", "urn"), "constant", ExtractionMode.Promote));
			}
		}

		[Fact]
		public void ReadXmlForConstantExtractorWithPromotedAttribute()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 promoted='true' value='constant'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new ConstantExtractor(new XmlQualifiedName("Property1", "urn"), "constant", ExtractionMode.Promote));
			}
		}

		[Fact]
		public void ReadXmlForExtractorPrecedence()
		{
			var xml = $"<san:Properties precedence='schemaOnly' xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 mode='clear'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Precedence.Should().Be(ExtractorPrecedence.SchemaOnly);
			}
		}

		[Fact]
		public void ReadXmlForPropertyExtractor()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 mode='clear'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new PropertyExtractor(new XmlQualifiedName("Property1", "urn"), ExtractionMode.Clear));
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForPropertyExtractorThrowsWhenModeAttributeIsInvalid()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 mode='promote'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ArgumentException>()
					.WithMessage("Invalid ExtractionMode, only Clear and Ignore are supported for PropertyExtractor without a Value or an XPath.*");
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForPropertyExtractorThrowsWhenModeAttributeIsMissing()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>().WithMessage("ExtractionMode is missing for PropertyExtractor without a Value or an XPath.*");
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForPropertyExtractorThrowsWhenPromotedAttributeIsPresent()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 promoted='true'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>().WithMessage("ExtractionMode is missing for PropertyExtractor without a Value or an XPath.*");
			}
		}

		[Fact]
		public void ReadXmlForQNameValueExtractor()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property3 mode='promote' qnameValue='localName' xpath='*/extra-node'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(
					new QNameValueExtractor(
						new XmlQualifiedName("Property3", "urn"),
						"*/extra-node",
						ExtractionMode.Promote,
						QNameValueExtractionMode.LocalName));
			}
		}

		[Fact]
		public void ReadXmlForQNameValueExtractorFallsBackOnXPathExtractorWhenQNameValueExtractionModeIsDefault()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property3 mode='promote' qnameValue='name' xpath='*/extra-node'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new XPathExtractor(new XmlQualifiedName("Property3", "urn"), "*/extra-node", ExtractionMode.Promote));
			}
		}

		[Fact]
		public void ReadXmlForXPathExtractor()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property1 xpath='*/some-node'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write));
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForXPathExtractorThrowsWhenXPathIsEmpty()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:PropertyName xpath=''/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>()
					.WithInnerException<XPathException>()
					// ReSharper disable once StringLiteralTypo
					.WithMessage("Bad Query string encoundered in XPath:*");
			}
		}

		[Fact]
		public void ReadXmlForXPathExtractorWithModeAttribute()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property3 mode='demote' xpath='*/extra-node'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new XPathExtractor(new XmlQualifiedName("Property3", "urn"), "*/extra-node", ExtractionMode.Demote));
			}
		}

		[Fact]
		public void ReadXmlForXPathExtractorWithPromotedAttribute()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:Property2 promoted='true' xpath='*/other-node'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEquivalentTo(new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote));
			}
		}

#if DEBUG
		[Fact]
#else
		[Fact(Skip = "Only to be run in DEBUG configuration.")]
#endif
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenDuplicatePropertyToExtract()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<s0:PropertyName xpath='*'/><s0:PropertyName xpath='*'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>()
					.WithInnerException<XmlException>()
					.WithMessage("The following properties are declared multiple times: [urn:PropertyName].");
			}
		}

#if DEBUG
		[Fact]
#else
		[Fact(Skip = "Only to be run in DEBUG configuration.")]
#endif
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenPropertyToExtractHasNoNamespace()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}'>"
				+ "<PropertyName xpath='*'/>"
				+ "</san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>()
					.WithInnerException<XmlException>()
					.WithMessage("The following properties are not associated with the target namespace URI of some property schema: [PropertyName].");
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenRootElementMissesNamespace()
		{
			const string xml = "<Properties xmlns:s0='urn'><s0:PropertyName xpath='*'/></Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>()
					.WithInnerException<XmlException>()
					.WithMessage($"Element 'Properties' with namespace name '{SchemaAnnotationCollection.NAMESPACE}' was not found. Line 1, position 2.");
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenRootElementNameIsInvalid()
		{
			const string xml = "<san:Extractors xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'><s0:PropertyName xpath='*'/></san:Extractors>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>()
					.WithInnerException<XmlException>()
					.WithMessage($"Element 'Properties' with namespace name '{SchemaAnnotationCollection.NAMESPACE}' was not found. Line 1, position 2.");
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenRootElementNamespaceIsInvalid()
		{
			const string xml = "<san:Properties xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:2012:12:extractors'><s0:PropertyName xpath='*'/></san:Properties>";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Invoking(() => sut.ReadXml(reader))
					.Should().Throw<ConfigurationErrorsException>()
					.WithInnerException<XmlException>()
					.WithMessage($"Element 'Properties' with namespace name '{SchemaAnnotationCollection.NAMESPACE}' was not found. Line 1, position 2.");
			}
		}

		[Fact]
		public void ReadXmlWithoutProperties()
		{
			var xml = $"<san:Properties xmlns:s0='urn' xmlns:san='{SchemaAnnotationCollection.NAMESPACE}' />";

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				sut.Should().BeEmpty();
			}
		}

		[Fact]
		public void UnionWithPipelineOnlyPrecedenceOfEmptySchemaAndPipelineExtractors()
		{
			var schemaExtractors = PropertyExtractorCollection.Empty;

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.PipelineOnly,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(pipelineExtractors);
		}

		[Fact]
		public void UnionWithPipelineOnlyPrecedenceOfSchemaAndEmptyPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(ExtractorPrecedence.PipelineOnly);

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(schemaExtractors);
		}

		[Fact]
		public void UnionWithPipelineOnlyPrecedenceOfSchemaAndPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.PipelineOnly,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(pipelineExtractors);
		}

		[Fact]
		public void UnionWithPipelinePrecedenceOfEmptySchemaAndPipelineExtractors()
		{
			var schemaExtractors = PropertyExtractorCollection.Empty;

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Pipeline,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(pipelineExtractors);
		}

		[Fact]
		public void UnionWithPipelinePrecedenceOfSchemaAndEmptyPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(ExtractorPrecedence.Pipeline);

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(schemaExtractors);
		}

		[Fact]
		public void UnionWithPipelinePrecedenceOfSchemaAndPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Pipeline,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			var expectedExtractors = pipelineExtractors.Concat(
				new PropertyExtractor[] {
					new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
					new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write)
				});

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(expectedExtractors);
		}

		[Fact]
		public void UnionWithPipelinePrecedenceOfSchemaAndPipelineExtractorsHavingPipelinePropertyExtractorsToBeIgnored()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Clear),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Clear),
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Pipeline,
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Ignore),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Ignore),
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			var expectedExtractors = pipelineExtractors
				.Where(pe => pe.ExtractionMode != ExtractionMode.Ignore)
				.Concat(
					new PropertyExtractor[] {
						new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
						new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write)
					});

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(expectedExtractors);
		}

		[Fact]
		public void UnionWithPipelinePrecedenceOfSchemaAndPipelineExtractorsHavingSchemaPropertyExtractorsToBeIgnored()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Ignore),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Ignore),
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Pipeline,
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Clear),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Clear),
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			var expectedExtractors = pipelineExtractors.Concat(
				new PropertyExtractor[] {
					new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
					new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write)
				});

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(expectedExtractors);
		}

		[Fact]
		public void UnionWithSchemaOnlyPrecedenceOfEmptySchemaAndPipelineExtractors()
		{
			var schemaExtractors = PropertyExtractorCollection.Empty;

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.SchemaOnly,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(pipelineExtractors);
		}

		[Fact]
		public void UnionWithSchemaOnlyPrecedenceOfSchemaAndEmptyPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(ExtractorPrecedence.SchemaOnly);

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(schemaExtractors);
		}

		[Fact]
		public void UnionWithSchemaOnlyPrecedenceOfSchemaAndPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.SchemaOnly,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(schemaExtractors);
		}

		[Fact]
		public void UnionWithSchemaPrecedenceOfEmptySchemaAndPipelineExtractors()
		{
			var schemaExtractors = PropertyExtractorCollection.Empty;

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Schema,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(pipelineExtractors);
		}

		[Fact]
		public void UnionWithSchemaPrecedenceOfSchemaAndEmptyPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(ExtractorPrecedence.Schema);

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(schemaExtractors);
		}

		[Fact]
		public void UnionWithSchemaPrecedenceOfSchemaAndPipelineExtractors()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Schema,
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			var expectedExtractors = schemaExtractors.Concat(
				new PropertyExtractor[] {
					new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
					new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote)
				});

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(expectedExtractors);
		}

		[Fact]
		public void UnionWithSchemaPrecedenceOfSchemaAndPipelineExtractorsHavingPipelinePropertyExtractorsToBeIgnored()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Clear),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Clear),
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Schema,
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Ignore),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Ignore),
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			var expectedExtractors = schemaExtractors.Concat(
				new PropertyExtractor[] {
					new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
					new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote)
				});

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(expectedExtractors);
		}

		[Fact]
		public void UnionWithSchemaPrecedenceOfSchemaAndPipelineExtractorsHavingSchemaPropertyExtractorsToBeIgnored()
		{
			var schemaExtractors = new PropertyExtractorCollection(
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Ignore),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Ignore),
				new ConstantExtractor(new XmlQualifiedName("cso-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("xso-prop", "urn"), "*/other-node", ExtractionMode.Write),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Write));

			var pipelineExtractors = new PropertyExtractorCollection(
				ExtractorPrecedence.Schema,
				new PropertyExtractor(new XmlQualifiedName("prop1", "urn"), ExtractionMode.Clear),
				new PropertyExtractor(new XmlQualifiedName("prop2", "urn"), ExtractionMode.Clear),
				new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote),
				new ConstantExtractor(new XmlQualifiedName("c-prop", "urn"), "constant", ExtractionMode.Promote),
				new XPathExtractor(new XmlQualifiedName("x-prop", "urn"), "*/other-node", ExtractionMode.Promote));

			var expectedExtractors = schemaExtractors
				.Where(pe => pe.ExtractionMode != ExtractionMode.Ignore)
				.Concat(
					new PropertyExtractor[] {
						new ConstantExtractor(new XmlQualifiedName("cpo-prop", "urn"), "constant", ExtractionMode.Promote),
						new XPathExtractor(new XmlQualifiedName("xpo-prop", "urn"), "*/other-node", ExtractionMode.Promote)
					});

			schemaExtractors.Union(pipelineExtractors).Should().BeEquivalentTo(expectedExtractors);
		}

		[Fact]
		public void UnionWithWhateverPrecedenceOfEmptySchemaAndEmptyPipelineExtractors()
		{
			PropertyExtractorCollection.Empty.Union(new PropertyExtractorCollection(ExtractorPrecedence.PipelineOnly)).Should().BeEmpty();
			PropertyExtractorCollection.Empty.Union(new PropertyExtractorCollection(ExtractorPrecedence.Pipeline)).Should().BeEmpty();
			PropertyExtractorCollection.Empty.Union(new PropertyExtractorCollection(ExtractorPrecedence.SchemaOnly)).Should().BeEmpty();
			PropertyExtractorCollection.Empty.Union(new PropertyExtractorCollection(ExtractorPrecedence.Schema)).Should().BeEmpty();
		}

		[Fact]
		public void WriteXml()
		{
			var xml = $"<s0:Properties xmlns:s0=\"{SchemaAnnotationCollection.NAMESPACE}\" xmlns:s1=\"urn\">"
				+ "<s1:Property1 xpath=\"*/some-node\" />"
				+ "<s1:Property2 mode=\"promote\" xpath=\"*/other-node\" />"
				+ "<s1:Property3 mode=\"promote\" value=\"constant\" />"
				+ "<s1:Property4 mode=\"clear\" />"
				+ "</s0:Properties>";

			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = new PropertyExtractorCollection(
					new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
					new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote),
					new ConstantExtractor(new XmlQualifiedName("Property3", "urn"), "constant", ExtractionMode.Promote),
					new PropertyExtractor(new XmlQualifiedName("Property4", "urn"), ExtractionMode.Clear));
				sut.WriteXml(writer!);
			}

			builder.ToString().Should().Be(xml);
		}

		[Fact]
		public void WriteXmlForExtractorPrecedence()
		{
			var xml = $"<s0:Properties precedence=\"pipelineOnly\" xmlns:s0=\"{SchemaAnnotationCollection.NAMESPACE}\" xmlns:s1=\"urn\">"
				+ "<s1:Property1 xpath=\"*/some-node\" />"
				+ "</s0:Properties>";

			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = new PropertyExtractorCollection(
					ExtractorPrecedence.PipelineOnly,
					new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write));
				sut.WriteXml(writer!);
			}

			builder.ToString().Should().Be(xml);
		}
	}
}
