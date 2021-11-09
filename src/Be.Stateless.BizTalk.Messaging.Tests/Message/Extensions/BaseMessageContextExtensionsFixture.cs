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

using Be.Stateless.BizTalk.ContextProperties;
using FluentAssertions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public class BaseMessageContextExtensionsFixture
	{
		[Fact]
		public void SerializeContextToXml()
		{
			var context = new Mock<IBaseMessageContext>();

			var (name, ns) = (BtsProperties.OutboundTransportLocation.Name, BtsProperties.OutboundTransportLocation.Namespace);
			context.Setup(c => c.ReadAt(0, out name, out ns)).Returns(@"file://c:\folder\ports\out");

			(name, ns) = (BtsProperties.OutboundTransportType.Name, BtsProperties.OutboundTransportType.Namespace);
			context.Setup(c => c.ReadAt(1, out name, out ns)).Returns("FILE");
			context.Setup(c => c.IsPromoted(name, ns)).Returns(true);

			(name, ns) = (FileProperties.Password.Name, FileProperties.Password.Namespace);
			context.Setup(c => c.ReadAt(2, out name, out ns)).Returns("p@ssw0rd");

			(name, ns) = (new WCF.SharedAccessKey().Name.Name, new WCF.SharedAccessKey().Name.Namespace);
			context.Setup(c => c.ReadAt(3, out name, out ns)).Returns("sh@r3d@cc3k3y");

			(name, ns) = (new WCF.IssuerSecret().Name.Name, new WCF.IssuerSecret().Name.Namespace);
			context.Setup(c => c.ReadAt(4, out name, out ns)).Returns("s3cr3t");

			(name, ns) = (WcfProperties.HttpHeaders.Name, WcfProperties.HttpHeaders.Namespace);
			context.Setup(c => c.ReadAt(5, out name, out ns)).Returns(
				@"Content-Type: application/xml
Authorization: Bearer b3@r3r
Accept: application/xml");

			name = "/*[local-name()='order' and namespace-uri()='urn:schemas.stateless.be:biztalk']/*[local-name()='id' and namespace-uri()='urn:schemas.stateless.be:biztalk']";
			ns = "http://schemas.microsoft.com/BizTalk/2003/btsDistinguishedFields";
			context.Setup(c => c.ReadAt(6, out name, out ns)).Returns("123456789");

			context.Setup(c => c.CountProperties).Returns(7);

			context.Object.ToXml().Should().Be(
				@"<context xmlns:s0=""http://schemas.microsoft.com/BizTalk/2003/system-properties"" xmlns:s1=""http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties"" xmlns:s2=""http://schemas.microsoft.com/BizTalk/2003/btsDistinguishedFields"">"
				+ @"<s0:p n=""OutboundTransportLocation"">file://c:\folder\ports\out</s0:p>"
				+ @"<s0:p n=""OutboundTransportType"" promoted=""true"">FILE</s0:p>"
				+ @"<s1:p n=""HttpHeaders"">Content-Type: application/xml
Accept: application/xml</s1:p>"
				+ @"<s2:p n=""/*[local-name()='order' and namespace-uri()='urn:schemas.stateless.be:biztalk']/*[local-name()='id' and namespace-uri()='urn:schemas.stateless.be:biztalk']"">123456789</s2:p>"
				+ "</context>"
			);

			context.VerifyAll();
		}
	}
}
