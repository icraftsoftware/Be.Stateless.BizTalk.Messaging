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
using System.Linq.Expressions;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.Message.Message.Language;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message.Message.Context
{
	/// <summary>
	/// <see cref="Mock"/> overloads to support the direct setup of the <see cref="BaseMessageContext"/>'s extension methods to
	/// read, write and promote <see cref="IBaseMessageContext"/> properties in a shorter and <b>type-safe</b> way.
	/// </summary>
	/// <typeparam name="TMock">
	/// Type to mock, which can be an interface or a class; in this case, <see cref="IBaseMessageContext"/>.
	/// </typeparam>
	/// <seealso cref="BaseMessageContext.GetProperty{T}(IBaseMessageContext,MessageContextProperty{T,string})"/>
	/// <seealso cref="BaseMessageContext.GetProperty{T,TResult}(IBaseMessageContext,MessageContextProperty{T,TResult})"/>
	/// <seealso cref="BaseMessageContext.SetProperty{T}(IBaseMessageContext,MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessageContext.SetProperty{T,TV}(IBaseMessageContext,MessageContextProperty{T,TV},TV)"/>
	/// <seealso cref="BaseMessageContext.Promote{T}(IBaseMessageContext,MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessageContext.Promote{T,TV}(IBaseMessageContext,MessageContextProperty{T,TV},TV)"/>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public class Mock<TMock> : Moq.Mock<TMock> where TMock : class, IBaseMessageContext
	{
		public Mock() : this(MockBehavior.Default) { }

		public Mock(MockBehavior behavior) : base(behavior) { }

		[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
		public ISetup<TMock> Setup(Expression<Action<IBaseMessageContext>> expression)
		{
			// intercept setup
			return expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContext)
				// rewrite setup
				? SetupCoreMethodCallExpression(methodCallExpression)
				// let base class handle all other setup
				: Setup((Expression<Action<TMock>>) (Expression) expression);
		}

		internal ISetup<TMock> SetupCoreMethodCallExpression(MethodCallExpression methodCallExpression)
		{
			// rewrite setup
			var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
			switch (methodCallExpression.Method.Name)
			{
				case nameof(BaseMessageContext.DeleteProperty):
					return base.Setup(context => context.Write(qualifiedName.Name, qualifiedName.Namespace, null));
				case nameof(BaseMessageContext.SetProperty):
					// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
					return Setup(RewriteExpression(methodCallExpression));
				case nameof(BaseMessageContext.Promote):
					// setup IsPromoted() as well
					base.Setup(context => context.IsPromoted(qualifiedName.Name, qualifiedName.Namespace)).Returns(true);
					// to mock how a promoted property actually behaves Read() must also be setup
					// pass expression's argument verbatim should it be It.IsAny<>
					var promotedValue = Expression.Lambda(methodCallExpression.Arguments[2]).Compile().DynamicInvoke();
					base.Setup(context => context.Read(qualifiedName.Name, qualifiedName.Namespace)).Returns(promotedValue);
					// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
					return Setup(RewriteExpression(methodCallExpression));
				default:
					throw new InvalidOperationException($"Unexpected call of extension method: '{methodCallExpression.Method.Name}'.");
			}
		}

		public ISetup<TMock, string> Setup(Expression<Func<IBaseMessageContext, string>> expression)
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContext)
				&& methodCallExpression.Method.Name == nameof(BaseMessageContext.GetProperty))
			{
				// rewrite setup
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return this.SetupContextReadPropertyString(qualifiedName.Name, qualifiedName.Namespace);
			}
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, string>>) (Expression) expression);
		}

		public ISetup<TMock, bool> Setup(Expression<Func<IBaseMessageContext, bool>> expression)
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContext)
				&& methodCallExpression.Method.Name == nameof(BaseMessageContext.IsPromoted))
			{
				// rewrite setup
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return base.Setup(context => context.IsPromoted(qualifiedName.Name, qualifiedName.Namespace));
			}
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, bool>>) (Expression) expression);
		}

		public ISetup<TMock, object> Setup<TResult>(Expression<Func<IBaseMessageContext, TResult?>> expression) where TResult : struct
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContext)
				&& methodCallExpression.Method.Name == nameof(BaseMessageContext.GetProperty))
			{
				// rewrite setup
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return base.Setup(context => context.Read(qualifiedName.Name, qualifiedName.Namespace));
			}
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, object>>) (Expression) expression);
		}

		public ISetup<TMock, TResult> Setup<TResult>(Expression<Func<IBaseMessageContext, TResult>> expression)
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContext))
				throw new InvalidOperationException($"Unexpected call of extension method: '{methodCallExpression.Method.Name}'.");
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, TResult>>) (Expression) expression);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression)
		{
			Verify(expression, Times.AtLeastOnce());
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, Func<Times> times)
		{
			Verify(expression, times());
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, string failMessage)
		{
			Verify(expression, Times.AtLeastOnce(), failMessage);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, Func<Times> times, string failMessage)
		{
			Verify(expression, times(), failMessage);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, Times times, string failMessage = null)
		{
			// intercept and rewrite IBaseMessage Verify calls against IBaseMessageContext
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContext))
			{
				// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
				var rewrittenExpression = RewriteExpression(methodCallExpression);
				Verify(rewrittenExpression, times, failMessage);
			}
			else
			{
				// let base Moq class handle all other Verify calls
				Verify((Expression<Action<TMock>>) (Expression) expression, times, failMessage);
			}
		}

		[SuppressMessage("Performance", "CA1822:Mark members as static")]
		internal XmlQualifiedName GetContextPropertyXmlQualifiedName(MethodCallExpression methodCallExpression)
		{
			var propertyArgument = methodCallExpression.Arguments[1];
			if (!propertyArgument.Type.IsSubclassOfGenericType(typeof(MessageContextProperty<,>))) throw new InvalidOperationException();
			dynamic contextProperty = Expression.Lambda(propertyArgument).Compile().DynamicInvoke();
			var qualifiedName = new XmlQualifiedName(contextProperty.Name, contextProperty.Namespace);
			return qualifiedName;
		}

		internal Expression<Action<TMock>> RewriteExpression(MethodCallExpression methodCallExpression)
		{
			var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
			return methodCallExpression.Method.Name switch {
				nameof(BaseMessageContext.DeleteProperty) => RewriteExpression(_writeExpressionTemplate, qualifiedName, Expression.Constant(null)),
				nameof(BaseMessageContext.GetProperty) => context => context.Read(qualifiedName.Name, qualifiedName.Namespace),
				// TODO IsPromoted extension method ensures both that there is a value and that it is promoted
				nameof(BaseMessageContext.IsPromoted) => context => context.IsPromoted(qualifiedName.Name, qualifiedName.Namespace),
				nameof(BaseMessageContext.SetProperty) => RewriteExpression(_writeExpressionTemplate, qualifiedName, methodCallExpression.Arguments[2]),
				nameof(BaseMessageContext.Promote) => RewriteExpression(_promoteExpressionTemplate, qualifiedName, methodCallExpression.Arguments[2]),
				_ => throw new InvalidOperationException($"Unexpected call of extension method: '{methodCallExpression.Method.Name}'.")
			};
		}

		[SuppressMessage("Performance", "CA1822:Mark members as static")]
		private Expression<Action<TMock>> RewriteExpression(Expression<Action<TMock>> expressionTemplate, XmlQualifiedName qualifiedName, Expression valueExpression)
		{
			var mce = (MethodCallExpression) expressionTemplate.Body;
			var ec = Expression.Call(
				mce.Object,
				mce.Method,
				Expression.Constant(qualifiedName.Name),
				Expression.Constant(qualifiedName.Namespace),
				Expression.Convert(valueExpression, typeof(object)));
			return Expression.Lambda<Action<TMock>>(ec, expressionTemplate.Parameters);
		}

		private static readonly Expression<Action<TMock>> _promoteExpressionTemplate =
			context => context.Promote(BizTalkFactoryProperties.EnvironmentTag.Name, BizTalkFactoryProperties.EnvironmentTag.Namespace, null);

		private static readonly Expression<Action<TMock>> _writeExpressionTemplate =
			context => context.Write(BizTalkFactoryProperties.EnvironmentTag.Name, BizTalkFactoryProperties.EnvironmentTag.Namespace, null);
	}
}
