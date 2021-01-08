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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using BTS;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Message
{
	public class MessageBodyFactoryFixture
	{
		[Fact]
		public void CreatingMessageBodyBySchemaTypeNeverThrows()
		{
			Action act = () => MessageBodyFactory.Create<soap_envelope_1__2.Envelope>();
			act.Should().NotThrow();
		}

		[Fact]
		[SuppressMessage("ReSharper", "StringLiteralTypo")]
		public void CreatingMessageBodyForNonSchemaTypeThrows()
		{
			Action act = () => MessageBodyFactory.Create(typeof(int));
			act.Should().Throw<ArgumentException>().WithMessage("System.Int32 does not derive from Microsoft.XLANGs.BaseTypes.SchemaBase.*");
		}

		[Fact]
		public void CreatingMessageBodyWithInvalidContentThrows()
		{
			var content = MessageBodyFactory.Create<soap_envelope_1__1.Envelope>().OuterXml;
			Action act = () => MessageBodyFactory.Create<soap_envelope_1__2.Envelope>(content);
			act.Should().Throw<XmlSchemaValidationException>();
		}

		[Fact]
		public void CreatingMessageBodyWithValidContentDoesNotThrow()
		{
			var content = MessageBodyFactory.Create<soap_envelope_1__2.Envelope>().OuterXml;
			Action act = () => MessageBodyFactory.Create<soap_envelope_1__2.Envelope>(content);
			act.Should().NotThrow();
		}
	}
}
