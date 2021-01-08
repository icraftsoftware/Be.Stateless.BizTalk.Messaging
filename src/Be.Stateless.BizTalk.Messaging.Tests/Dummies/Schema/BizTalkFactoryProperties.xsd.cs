namespace Be.Stateless.BizTalk.Schemas.BizTalkFactory {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Property)]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"CorrelationId", @"EnvironmentTag", @"ReceiverName", @"SenderName"})]
    public sealed class Properties : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""urn:schemas.stateless.be:biztalk:properties:system:2012:04"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" targetNamespace=""urn:schemas.stateless.be:biztalk:properties:system:2012:04"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:annotation>
    <xs:documentation><![CDATA[
Copyright © 2012 - 2020 François Chabot

Licensed under the Apache License, Version 2.0 (the ""License"");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an ""AS IS"" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
]]></xs:documentation>
    <xs:appinfo>
      <b:schemaInfo schema_type=""property"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" />
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""CorrelationId"" type=""xs:string"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""5206a0a0-40e5-44f2-b6b0-069d0293e37a"" propSchFieldBase=""MessageContextPropertyBase"" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""EnvironmentTag"" type=""xs:string"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""dce26d06-9a64-49ac-8651-03286a5cba2e"" propSchFieldBase=""MessageContextPropertyBase"" notes=""To be used when one application has to be connected to several distinct sets of interacting parties and cannot leak messages from one set of parties into another. In concrete terms, to be used in pub/sub of messages in order to keep them strictly insulated within an individual set of such interacting parties."" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""ReceiverName"" type=""xs:string"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""268786e1-7638-4e02-ad9c-c69b0deca23f"" propSchFieldBase=""MessageContextPropertyBase"" notes=""Name of the intended receiver of the current message."" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
  <xs:element name=""SenderName"" type=""xs:string"">
    <xs:annotation>
      <xs:appinfo>
        <b:fieldInfo propertyGuid=""1b669871-b481-4e4f-9a6a-d182c24c462e"" propSchFieldBase=""MessageContextPropertyBase"" notes=""Name of the initiating sender of the current message."" />
      </xs:appinfo>
    </xs:annotation>
  </xs:element>
</xs:schema>";
        
        public Properties() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [4];
                _RootElements[0] = "CorrelationId";
                _RootElements[1] = "EnvironmentTag";
                _RootElements[2] = "ReceiverName";
                _RootElements[3] = "SenderName";
                return _RootElements;
            }
        }
        
        protected override object RawSchema {
            get {
                return _rawSchema;
            }
            set {
                _rawSchema = value;
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"CorrelationId",@"urn:schemas.stateless.be:biztalk:properties:system:2012:04","string","System.String")]
    [PropertyGuidAttribute(@"5206a0a0-40e5-44f2-b6b0-069d0293e37a")]
    public sealed class CorrelationId : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"CorrelationId", @"urn:schemas.stateless.be:biztalk:properties:system:2012:04");
        
        private static string PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(string);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"EnvironmentTag",@"urn:schemas.stateless.be:biztalk:properties:system:2012:04","string","System.String")]
    [PropertyGuidAttribute(@"dce26d06-9a64-49ac-8651-03286a5cba2e")]
    public sealed class EnvironmentTag : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"EnvironmentTag", @"urn:schemas.stateless.be:biztalk:properties:system:2012:04");
        
        private static string PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(string);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"ReceiverName",@"urn:schemas.stateless.be:biztalk:properties:system:2012:04","string","System.String")]
    [PropertyGuidAttribute(@"268786e1-7638-4e02-ad9c-c69b0deca23f")]
    public sealed class ReceiverName : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"ReceiverName", @"urn:schemas.stateless.be:biztalk:properties:system:2012:04");
        
        private static string PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(string);
            }
        }
    }
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [System.SerializableAttribute()]
    [PropertyType(@"SenderName",@"urn:schemas.stateless.be:biztalk:properties:system:2012:04","string","System.String")]
    [PropertyGuidAttribute(@"1b669871-b481-4e4f-9a6a-d182c24c462e")]
    public sealed class SenderName : Microsoft.XLANGs.BaseTypes.MessageContextPropertyBase {
        
        [System.NonSerializedAttribute()]
        private static System.Xml.XmlQualifiedName _QName = new System.Xml.XmlQualifiedName(@"SenderName", @"urn:schemas.stateless.be:biztalk:properties:system:2012:04");
        
        private static string PropertyValueType {
            get {
                throw new System.NotSupportedException();
            }
        }
        
        public override System.Xml.XmlQualifiedName Name {
            get {
                return _QName;
            }
        }
        
        public override System.Type Type {
            get {
                return typeof(string);
            }
        }
    }
}
