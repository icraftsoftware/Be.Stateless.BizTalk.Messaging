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

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.ContextProperties.Extensions;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using FluentAssertions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Unit.Message
{
	public class MockFixture
	{
		[Fact]
		public void AnyFunctionCanBeUsedToSetupExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.AckRequired)).Returns(true);
			message.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Action(() => message.Object.GetProperty(BtsProperties.AckRequired)).Should().NotThrow();
			message.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			Action(() => message.Object.GetProperty(BtsProperties.ActualRetryCount)).Should().NotThrow();
			message.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			Action(() => message.Object.GetProperty(BtsProperties.SendPortName)).Should().NotThrow();
			message.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");
		}

		[Fact]
		public void AnyPredicateCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Setup(m => m.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 == 0)));
			context.Setup(m => m.Promote(BtsProperties.SendPortName, It.Is<string>(s => s.IsQName())));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 12)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "ns:name")).Should().NotThrow();
		}

		[Fact]
		public void AnyPredicateCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => !b)));
			context.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 != 0)));
			context.Setup(m => m.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => !s.IsQName())));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, false)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "any name")).Should().NotThrow();
		}

		[Fact]
		public void AnyValueCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(m => m.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(m => m.Promote(BtsProperties.SendPortName, It.IsAny<string>()));

			Action(() => context.Object.Promote(BtsProperties.AckRequired, false)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.ActualRetryCount, 11)).Should().NotThrow();
			Action(() => context.Object.Promote(BtsProperties.SendPortName, "any-send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void AnyValueCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(m => m.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));

			Action(() => context.Object.SetProperty(BtsProperties.AckRequired, false)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11)).Should().NotThrow();
			Action(() => context.Object.SetProperty(BtsProperties.SendPortName, "any-send-port-name")).Should().NotThrow();
		}

		[Fact]
		public void AnyValueOrPredicateCanBeUsedToVerifyPromoteExtensionMethod()
		{
			var context = new Message.Mock<IBaseMessage>();

			context.Object.Context.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Context.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Context.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(m => m.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(m => m.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(m => m.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(m => m.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(m => m.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(m => m.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(m => m.Promote(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(m => m.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(m => m.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Fact]
		public void AnyValueOrPredicateCanBeUsedToVerifySetPropertyExtensionMethod()
		{
			var context = new Message.Mock<IBaseMessage>();

			context.Object.Context.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Context.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Context.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(m => m.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(m => m.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(m => m.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(m => m.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(m => m.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(m => m.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Fact]
		public void BodyPartContentTypeCanBeAssigned()
		{
			var message = new Message.Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };

			message.Object.BodyPart.ContentType = "application/test";

			message.Object.BodyPart.ContentType.Should().Be("application/test");
		}

		[Fact]
		public void BodyPartDataSetupImplicitlySetupOriginalDataStream()
		{
			const string content = "<s1:letter xmlns:s1='urn-one'>" +
				"<s1:headers><s1:subject>inquiry</s1:subject></s1:headers>" +
				"<s1:body><s1:paragraph>paragraph-one</s1:paragraph></s1:body>" +
				"</s1:letter>";
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);

				message.Object.BodyPart.Data = inputStream;

				message.Object.BodyPart.GetOriginalDataStream().Should().BeSameAs(inputStream);
			}
		}

		[Fact]
		public void BodyPartNameCanBeSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);

			message.Setup(m => m.BodyPartName).Returns("implicit");

			message.Object.BodyPartName.Should().Be("implicit");
		}

		[Fact]
		public void ContextMockIsImplicitlySetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);

			message.Object.Context.Should().NotBeNull();
			Mock.Get(message.Object.Context).Behavior.Should().Be(MockBehavior.Strict);
		}

		[Fact]
		public void ContextPropertyValueDefaultsToNullWithoutSetupIfMockBehaviorIsLoose()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Loose);

			message.Object.GetProperty(ErrorReportProperties.ErrorType).Should().BeNull();
			message.Object.HasFailed().Should().BeFalse();
		}

		[Fact]
		public void DeletePropertyExtensionMethodCanVerifyContextDeletePropertyExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.AckRequired));
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.SendPortName));

			message.Object.DeleteProperty(BtsProperties.AckRequired);
			message.Object.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.DeleteProperty(BtsProperties.SendPortName);

			message.Verify(m => m.DeleteProperty(BtsProperties.AckRequired));
			message.Verify(m => m.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.DeleteProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void DeletePropertyExtensionMethodCanVerifyVerifiableContextDeletePropertyExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.ActualRetryCount)).Verifiable();
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.AckRequired)).Verifiable();
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.SendPortName)).Verifiable();

			message.Object.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.DeleteProperty(BtsProperties.AckRequired);
			message.Object.DeleteProperty(BtsProperties.SendPortName);

			message.Verify();
		}

		[Fact]
		public void DeletePropertyExtensionMethodSetupCanBeVerifiedByContextDeletePropertyExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.DeleteProperty(BtsProperties.AckRequired));
			message.Setup(m => m.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Setup(m => m.DeleteProperty(BtsProperties.SendPortName));

			message.Object.Context.DeleteProperty(BtsProperties.AckRequired);
			message.Object.Context.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.Context.DeleteProperty(BtsProperties.SendPortName);

			message.Verify(m => m.Context.DeleteProperty(BtsProperties.AckRequired));
			message.Verify(m => m.Context.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.Context.DeleteProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void DeletePropertyExtensionMethodVerifiableSetupCanBeVerifiedByContextDeletePropertyExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.DeleteProperty(BtsProperties.ActualRetryCount)).Verifiable();
			message.Setup(m => m.DeleteProperty(BtsProperties.AckRequired)).Verifiable();
			message.Setup(m => m.DeleteProperty(BtsProperties.SendPortName)).Verifiable();

			message.Object.Context.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.Context.DeleteProperty(BtsProperties.AckRequired);
			message.Object.Context.DeleteProperty(BtsProperties.SendPortName);

			message.Verify();
		}

		[Fact]
		public void GetPropertyExtensionMethodCanVerifyContextGetPropertyExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.GetProperty(BtsProperties.AckRequired)).Returns(true);
			message.Setup(m => m.Context.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			message.Setup(m => m.Context.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			message.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			message.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			message.Verify(m => m.GetProperty(BtsProperties.AckRequired));
			message.Verify(m => m.GetProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.GetProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void GetPropertyExtensionMethodCanVerifyVerifiableContextGetPropertyExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.GetProperty(BtsProperties.AckRequired)).Returns(true).Verifiable();
			message.Setup(m => m.Context.GetProperty(BtsProperties.ActualRetryCount)).Returns(10).Verifiable();
			message.Setup(m => m.Context.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name").Verifiable();

			message.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			message.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			message.Verify();
		}

		[Fact]
		public void GetPropertyExtensionMethodSetupCanBeVerifiedByContextGetPropertyExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.AckRequired)).Returns(true);
			message.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			message.Object.Context.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.Context.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			message.Object.Context.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			message.Verify(m => m.Context.GetProperty(BtsProperties.AckRequired));
			message.Verify(m => m.Context.GetProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.Context.GetProperty(BtsProperties.SendPortName));
		}

		[Fact]
		public void GetPropertyExtensionMethodVerifiableSetupCanBeVerifiedByContextGetPropertyExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.AckRequired)).Returns(true).Verifiable();
			message.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount)).Returns(10).Verifiable();
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name").Verifiable();

			message.Object.Context.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.Context.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			message.Object.Context.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			message.Verify();
		}

		[Fact]
		public void IsPromotedExtensionMethodFailsWithEitherContextOrMessageIsPromotedExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Loose);
			message.Setup(m => m.Context.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			message.Setup(m => m.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			message.Setup(m => m.Context.IsPromoted(BtsProperties.SendPortName)).Returns(true);
			message.Setup(m => m.IsPromoted(BtsProperties.TransmitWorkId)).Returns(true);

			message.Object.IsPromoted(BtsProperties.AckRequired).Should().BeFalse();
			message.Object.Context.IsPromoted(BtsProperties.ActualRetryCount).Should().BeFalse();
			message.Object.Context.IsPromoted(BtsProperties.SendPortName).Should().BeFalse();
			message.Object.IsPromoted(BtsProperties.TransmitWorkId).Should().BeFalse();

			Action(() => message.Verify(m => m.IsPromoted(BtsProperties.AckRequired))).Should().Throw<MockException>();
			Action(() => message.Verify(m => m.IsPromoted(BtsProperties.ActualRetryCount))).Should().Throw<MockException>();
			Action(() => message.Verify(m => m.IsPromoted(BtsProperties.SendPortName))).Should().Throw<MockException>();
			Action(() => message.Verify(m => m.IsPromoted(BtsProperties.TransmitWorkId))).Should().Throw<MockException>();
		}

		[Fact]
		public void IsPromotedExtensionMethodRequiresEitherContextOrMessagePromoteExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.Promote(BtsProperties.AckRequired, true));
			message.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"));
			message.Setup(m => m.Promote(BtsProperties.TransmitWorkId, "work-id"));

			message.Object.IsPromoted(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.Context.IsPromoted(BtsProperties.ActualRetryCount).Should().BeTrue();
			message.Object.Context.IsPromoted(BtsProperties.SendPortName).Should().BeTrue();
			message.Object.IsPromoted(BtsProperties.TransmitWorkId).Should().BeTrue();

			message.Verify(m => m.IsPromoted(BtsProperties.AckRequired));
			message.Verify(m => m.IsPromoted(BtsProperties.ActualRetryCount));
			message.Verify(m => m.IsPromoted(BtsProperties.SendPortName));
			message.Verify(m => m.IsPromoted(BtsProperties.TransmitWorkId));
		}

		[Fact]
		public void MoqBugWhereRecursiveMockingOverwritesExplicitSetupIsAscertained()
		{
			var message = new Mock<IBaseMessage> { DefaultValue = DefaultValue.Empty };
			var context = new Context.Mock<IBaseMessageContext> { DefaultValue = DefaultValue.Empty };

			message.Setup(m => m.Context).Returns(context.Object);
			message.Object.Context.Should().BeSameAs(context.Object);

			message.Setup(m => m.Context.CountProperties).Returns(0);
			message.Object.Context.Should().BeSameAs(context.Object, "Moq bug has resurfaced as explicit setup being overwritten by recursive mocking feature.");
		}

		[Fact]
		public void MoqBugWhereRecursiveMockingOverwritesExplicitSetupIsCircumvented()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			message.Object.GetProperty(BtsProperties.InboundTransportLocation).Should().NotBeNullOrEmpty();
			var c = message.Object.Context;

			message.Setup(m => m.Context.IsPromoted(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
			message.Object.Context.Should().BeSameAs(c);
			message.Object.GetProperty(BtsProperties.InboundTransportLocation).Should().NotBeNullOrEmpty();
		}

		[Fact]
		public void PromoteExtensionMethodCanVerifyContextPromoteExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.Promote(BtsProperties.AckRequired, true));
			message.Setup(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"));

			Action(() => message.Object.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => message.Object.Promote(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => message.Object.Promote(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();

			// notice Promote() setup sets up a context.Read() setup too, i.e. GetProperty() extension method
			message.Object.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			message.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			// notice Promote() setup sets up a context.IsPromoted() setup too, i.e. IsPromoted() extension method
			message.Object.IsPromoted(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.IsPromoted(BtsProperties.ActualRetryCount).Should().BeTrue();
			message.Object.IsPromoted(BtsProperties.SendPortName).Should().BeTrue();

			message.Verify(m => m.Promote(BtsProperties.AckRequired, true));
			message.Verify(m => m.Promote(BtsProperties.AckRequired, true), Times.Once);
			message.Verify(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.Promote(BtsProperties.ActualRetryCount, 10), Times.Once);
			message.Verify(m => m.Promote(BtsProperties.SendPortName, "send-port-name"));
			message.Verify(m => m.Promote(BtsProperties.SendPortName, "send-port-name"), Times.Once);
		}

		[Fact]
		public void PromoteExtensionMethodCanVerifyVerifiableContextPromoteExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.Context.Promote(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name")).Verifiable();
			message.Setup(m => m.Context.Promote(BtsProperties.ReceivePortName, "receive-port-name")).Verifiable();

			message.Object.Promote(BtsProperties.ActualRetryCount, 10);
			message.Object.Promote(BtsProperties.AckRequired, true);
			message.Object.Promote(BtsProperties.SendPortName, "send-port-name");

			Action(() => message.Verify())
				.Should().Throw<MockException>()
				.Where(
					e => Regex.IsMatch(
						e.Message,
						"This mock failed verification due to the following:.+"
						+ $"{nameof(IBaseMessageContext)} context => context\\.Promote\\(\"{BtsProperties.ReceivePortName.Name}\", \"{BtsProperties.ReceivePortName.Namespace}\", \"receive\\-port\\-name\"\\):.+"
						+ "This setup was not matched\\.",
						RegexOptions.Singleline));
		}

		[Fact]
		public void PromoteExtensionMethodSetupCanBeVerifiedByContextPromoteExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Promote(BtsProperties.AckRequired, true));
			message.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Promote(BtsProperties.SendPortName, "send-port-name"));

			Action(() => message.Object.Context.Promote(BtsProperties.AckRequired, true)).Should().NotThrow();
			Action(() => message.Object.Context.Promote(BtsProperties.ActualRetryCount, 10)).Should().NotThrow();
			Action(() => message.Object.Context.Promote(BtsProperties.SendPortName, "send-port-name")).Should().NotThrow();

			// notice Promote() setup sets up a context.Read() setup too, i.e. GetProperty() extension method
			message.Object.Context.GetProperty(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.Context.GetProperty(BtsProperties.ActualRetryCount).Should().Be(10);
			message.Object.Context.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			// notice Promote() setup sets up a context.IsPromoted() setup too, i.e. IsPromoted() extension method
			message.Object.Context.IsPromoted(BtsProperties.AckRequired).Should().BeTrue();
			message.Object.Context.IsPromoted(BtsProperties.ActualRetryCount).Should().BeTrue();
			message.Object.Context.IsPromoted(BtsProperties.SendPortName).Should().BeTrue();

			message.Verify(m => m.Context.Promote(BtsProperties.AckRequired, true));
			message.Verify(m => m.Context.Promote(BtsProperties.AckRequired, true), Times.Once);
			message.Verify(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10), Times.Once);
			message.Verify(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"));
			message.Verify(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"), Times.Once);
		}

		[Fact]
		public void PromoteExtensionMethodVerifiableSetupCanBeVerifiedByContextPromoteExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.Promote(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.Promote(BtsProperties.SendPortName, "send-port-name")).Verifiable();
			message.Setup(m => m.Promote(BtsProperties.ReceivePortName, "receive-port-name")).Verifiable();

			message.Object.Context.Promote(BtsProperties.ActualRetryCount, 10);
			message.Object.Context.Promote(BtsProperties.AckRequired, true);
			message.Object.Context.Promote(BtsProperties.SendPortName, "send-port-name");

			Action(() => message.Verify()).Should().Throw<MockException>().Where(
				e => Regex.IsMatch(
					e.Message,
					"This mock failed verification due to the following:.+"
					+ $"{nameof(IBaseMessageContext)} context => context\\.Promote\\(\"{BtsProperties.ReceivePortName.Name}\", \"{BtsProperties.ReceivePortName.Namespace}\", \"receive\\-port\\-name\"\\):.+"
					+ "This setup was not matched\\.",
					RegexOptions.Singleline));
		}

		[Fact]
		public void RegularMethodsAndPropertiesSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message
				.Setup(m => m.PartCount)
				.Returns(2);
			message
				.Setup(m => m.GetPart("part"))
				.Returns((IBaseMessagePart) null);

			message.Object.PartCount.Should().Be(2);
			message.Object.GetPart("part").Should().BeNull();
		}

		[Fact]
		public void SetPropertyExtensionMethodCanVerifyContextSetPropertyExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.SetProperty(BtsProperties.AckRequired, true));
			message.Setup(m => m.Context.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Context.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			message.Object.SetProperty(BtsProperties.AckRequired, true);
			message.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify(m => m.SetProperty(BtsProperties.AckRequired, true));
			message.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void SetPropertyExtensionMethodCanVerifyVerifiableContextSetPropertyExtensionMethodSetup()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.SetProperty(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.Context.SetProperty(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.Context.SetProperty(BtsProperties.SendPortName, "send-port-name")).Verifiable();

			message.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.SetProperty(BtsProperties.AckRequired, true);
			message.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify();
		}

		[Fact]
		public void SetPropertyExtensionMethodSetupCanBeVerifiedByContextSetPropertyExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.SetProperty(BtsProperties.AckRequired, true));
			message.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			message.Object.Context.SetProperty(BtsProperties.AckRequired, true);
			message.Object.Context.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.Context.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify(m => m.Context.SetProperty(BtsProperties.AckRequired, true));
			message.Verify(m => m.Context.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.Context.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Fact]
		public void SetPropertyExtensionMethodVerifiableSetupCanBeVerifiedByContextSetPropertyExtensionMethod()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.SetProperty(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name")).Verifiable();

			message.Object.Context.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.Context.SetProperty(BtsProperties.AckRequired, true);
			message.Object.Context.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify();
		}

		[Fact]
		public void VerifyAllAgainstLooseMock()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Loose);
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			message.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			message.VerifyAll();
		}

		[Fact]
		public void VerifyAllAgainstStrictMock()
		{
			var message = new Message.Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			message.Object.GetProperty(BtsProperties.SendPortName).Should().Be("send-port-name");

			message.VerifyAll();
		}
	}
}
