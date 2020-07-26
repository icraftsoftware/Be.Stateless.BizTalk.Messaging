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
using System.IO;
using System.Linq.Expressions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Extensions;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.Message.Language;
using Be.Stateless.BizTalk.Unit.Message.Language.Flow;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message
{
	/// <summary>
	/// <see cref="Mock"/> overloads to support the direct setup of the <see cref="BaseMessage"/>'s extension methods to read,
	/// write and promote <see cref="IBaseMessageContext"/> properties in a shorter and <b>type-safe</b> way.
	/// </summary>
	/// <typeparam name="TMock">
	/// Type to mock, which can be an interface or a class; in this case, <see cref="IBaseMessage"/>.
	/// </typeparam>
	/// <seealso cref="BaseMessage.GetProperty{T}(IBaseMessage,MessageContextProperty{T,string})"/>
	/// <seealso cref="BaseMessage.GetProperty{T,TResult}(IBaseMessage,MessageContextProperty{T,TResult})"/>
	/// <seealso cref="BaseMessage.SetProperty{T}(IBaseMessage,MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.SetProperty{T,TV}(IBaseMessage,MessageContextProperty{T,TV},TV)"/>
	/// <seealso cref="BaseMessage.Promote{T}(IBaseMessage,MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.Promote{T,TV}(IBaseMessage,MessageContextProperty{T,TV},TV)"/>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public class Mock<TMock> : Moq.Mock<TMock> where TMock : class, IBaseMessage
	{
		public Mock() : this(MockBehavior.Default) { }

		[SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
		public Mock(MockBehavior behavior) : base(behavior)
		{
			// setup a default context so that .GetProperty() extension on IBaseMessage mock can be called for property without setup
			_contextMock = new Context.Mock<IBaseMessageContext>(behavior);
			base.Setup(msg => msg.Context).Returns(_contextMock.Object);

			SetupProperty(msg => msg.BodyPart.ContentType);
			// hook GetOriginalDataStream() onto BodyPart.Data, so that it does not fail when BodyPart has a Data stream
			SetupProperty(msg => msg.BodyPart.Data);
			base.Setup(msg => msg.BodyPart.GetOriginalDataStream()).Returns(() => Object.BodyPart.Data);
		}

		public ISetup<TMock> Setup(Expression<Action<IBaseMessage>> expression)
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContext)))
			{
				// rewrite setup against context mock
				return new ContextMethodCallSetup<TMock>(_contextMock.SetupCoreMethodCallExpression(methodCallExpression));
			}
			// let base class handle all other setup
			return Setup((Expression<Action<TMock>>) (Expression) expression);
		}

		public ISetup<TMock, string> Setup(Expression<Func<IBaseMessage, string>> expression)
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContext))
				&& methodCallExpression.Method.Name == nameof(BaseMessageContext.GetProperty))
			{
				// rewrite setup against context mock
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return new ContextFunctionCallSetup<TMock, string>(_contextMock.SetupContextReadPropertyString(qualifiedName.Name, qualifiedName.Namespace));
			}
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, string>>) (Expression) expression);
		}

		public ISetup<TMock, bool> Setup(Expression<Func<IBaseMessage, bool>> expression)
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContext))
				&& methodCallExpression.Method.Name == nameof(BaseMessageContext.IsPromoted))
			{
				// rewrite setup against context mock
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return new ContextFunctionCallSetup<TMock, bool>(_contextMock.Setup(context => context.IsPromoted(qualifiedName.Name, qualifiedName.Namespace)));
			}
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, bool>>) (Expression) expression);
		}

		public ISetup<TMock, object> Setup<TResult>(Expression<Func<IBaseMessage, TResult?>> expression) where TResult : struct
		{
			// intercept and rewrite IBaseMessage setup against IBaseMessageContext
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContext))
				&& methodCallExpression.Method.Name == nameof(BaseMessageContext.GetProperty))
			{
				// rewrite setup against context mock
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return new ContextFunctionCallSetup<TMock, object>(_contextMock.Setup(context => context.Read(qualifiedName.Name, qualifiedName.Namespace)));
			}
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, object>>) (Expression) expression);
		}

		public ISetup<TMock, TResult> Setup<TResult>(Expression<Func<IBaseMessage, TResult>> expression)
		{
			// intercept setup
			if (expression.Body is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContext)))
				throw new InvalidOperationException($"Unexpected call of extension method: '{methodCallExpression.Method.Name}'.");
			// let base class handle all other setup
			return Setup((Expression<Func<TMock, TResult>>) (Expression) expression);
		}

		public new void Verify()
		{
			_contextMock.Verify();
			base.Verify();
		}

		public new void VerifyAll()
		{
			_contextMock.VerifyAll();
			// HACK: ensure explicit setups are called at least once not to fail VerifyAll() against current IBaseMessage mock
			Object.BodyPart.ContentType ??= "test/mock";
			Object.BodyPart.Data ??= new MemoryStream();
			Object.BodyPart.GetOriginalDataStream();
			base.VerifyAll();
		}

		public void Verify(Expression<Action<IBaseMessage>> expression)
		{
			Verify(expression, Times.AtLeastOnce());
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, Func<Times> times)
		{
			Verify(expression, times());
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, string failMessage)
		{
			Verify(expression, Times.AtLeastOnce(), failMessage);
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, Func<Times> times, string failMessage)
		{
			Verify(expression, times(), failMessage);
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public void Verify(Expression<Action<IBaseMessage>> expression, Times times, string failMessage = null)
		{
			// intercept and rewrite IBaseMessage Verify calls against IBaseMessageContext
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContext)))
			{
				// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
				var rewrittenExpression = _contextMock.RewriteExpression(methodCallExpression);
				_contextMock.Verify(rewrittenExpression, times, failMessage);
			}
			else if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(IBaseMessageContext))
			{
				var parameter = Expression.Parameter(typeof(IBaseMessageContext), "context");
				var ec = Expression.Call(parameter, methodCallExpression.Method, methodCallExpression.Arguments);
				var rewrittenExpression = Expression.Lambda<Action<IBaseMessageContext>>(ec, parameter);
				_contextMock.Verify(rewrittenExpression, times, failMessage);
			}
			else
			{
				// let base class handle all other Verify calls
				Verify((Expression<Action<TMock>>) (Expression) expression, times, failMessage);
			}
		}

		private readonly Context.Mock<IBaseMessageContext> _contextMock;
	}
}
