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
using System.Collections.Generic;

namespace Be.Stateless.BizTalk.Schema
{
	public class SchemaAnnotationCollection : ISchemaAnnotationCollection
	{
		internal static ISchemaAnnotationCollection Create(ISchemaMetadata schemaMetadata)
		{
			if (schemaMetadata == null) throw new ArgumentNullException(nameof(schemaMetadata));
			return new SchemaAnnotationCollection(() => SchemaAnnotationReader.Create(schemaMetadata));
		}

		private SchemaAnnotationCollection(Func<ISchemaAnnotationReader> schemaAnnotationReaderFactory)
		{
			_schemaAnnotationReaderFactory = schemaAnnotationReaderFactory;
			_annotationObjects = new(3);
		}

		#region ISchemaAnnotationCollection Members

		public T Find<T>() where T : ISchemaAnnotation<T>, new()
		{
			if (_annotationObjects.TryGetValue(typeof(T), out var annotationObject)) return (T) annotationObject;
			lock (_annotationObjects)
			{
				if (_annotationObjects.TryGetValue(typeof(T), out annotationObject)) return (T) annotationObject;
				var annotation = new T().Build(_schemaAnnotationReaderFactory());
				_annotationObjects.Add(typeof(T), annotation);
				return annotation;
			}
		}

		#endregion

		private readonly Dictionary<Type, object> _annotationObjects;
		private readonly Func<ISchemaAnnotationReader> _schemaAnnotationReaderFactory;
	}
}
