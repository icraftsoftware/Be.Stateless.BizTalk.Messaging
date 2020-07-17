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
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Resources.Schema
{
	[Schema(@"urn:schemas.stateless.be:biztalk:any:2020:02", @"Root")]
	[SchemaRoots(new[] { @"Root" })]
	[SchemaType(SchemaTypeEnum.Document)]
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	internal class RootedSchema : SchemaBase
	{
		static RootedSchema()
		{
			_xmlContent = @"<?xml version='1.0' encoding='utf-16'?>
<xs:schema elementFormDefault='qualified' targetNamespace='urn:schemas.stateless.be:biztalk:any:2020:02' xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01' xmlns:b='http://schemas.microsoft.com/BizTalk/2003' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
   <xs:annotation>
     <xs:appinfo>
       <b:schemaInfo is_envelope='yes' />
     </xs:appinfo>
   </xs:annotation>
   <xs:element name='Root'>
     <xs:annotation>
       <xs:appinfo>
         <b:recordInfo body_xpath=""/*[local-name()='Root' and namespace-uri()='urn:schemas.stateless.be:biztalk:any:2020:02']"" />
         <san:Properties xmlns:bf='urn:schemas.stateless.be:biztalk:properties:system:2012:04'>
           <bf:CorrelationId xpath=""/*[local-name()='Root']//*[local-name()='Id']"" />
         </san:Properties>
       </xs:appinfo>
     </xs:annotation>
     <xs:complexType>
       <xs:sequence>
         <xs:any minOccurs='0' maxOccurs='unbounded' namespace='##any' processContents='lax' />
       </xs:sequence>
     </xs:complexType>
   </xs:element>
</xs:schema>";
		}

		#region Base Class Member Overrides

		protected override object RawSchema { get; set; }

		public override string[] RootNodes => new[] { "Root" };

		public override string XmlContent => _xmlContent;

		#endregion

		private static readonly string _xmlContent;
	}
}
