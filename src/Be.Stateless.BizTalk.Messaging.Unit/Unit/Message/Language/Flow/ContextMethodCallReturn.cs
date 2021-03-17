﻿#region Copyright & License

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
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Moq.Language;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message.Language.Flow
{
	internal class ContextMethodCallReturn<TMock> : IReturnsResult<TMock>
		where TMock : class, IBaseMessage
	{
		public ContextMethodCallReturn(IReturnsResult<IBaseMessageContext> contextMethodCallReturnImplementation)
		{
			_contextMethodCallReturnImplementation = contextMethodCallReturnImplementation ?? throw new ArgumentNullException(nameof(contextMethodCallReturnImplementation));
		}

		#region IReturnsResult<TMock> Members

		public ICallbackResult Callback(InvocationAction action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback(Delegate callback)
		{
			return _contextMethodCallReturnImplementation.Callback(callback);
		}

		public ICallbackResult Callback(Action action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1>(Action<T1> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2>(Action<T1, T2> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3>(Action<T1, T2, T3> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action)
		{
			return _contextMethodCallReturnImplementation.Callback(action);
		}

		[Obsolete("To verify this condition, use the overload to Verify that receives Times.AtMostOnce().")]
		public IVerifies AtMostOnce()
		{
			return _contextMethodCallReturnImplementation.AtMostOnce();
		}

		[Obsolete("To verify this condition, use the overload to Verify that receives Times.AtMost(callCount).")]
		public IVerifies AtMost(int callCount)
		{
			return _contextMethodCallReturnImplementation.AtMost(callCount);
		}

		public void Verifiable()
		{
			_contextMethodCallReturnImplementation.Verifiable();
		}

		public void Verifiable(string failMessage)
		{
			_contextMethodCallReturnImplementation.Verifiable(failMessage);
		}

		public IVerifies Raises(Action<TMock> eventExpression, EventArgs args)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises(Action<TMock> eventExpression, Func<EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises(Action<TMock> eventExpression, params object[] args)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1>(Action<TMock> eventExpression, Func<T1, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2>(Action<TMock> eventExpression, Func<T1, T2, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3>(Action<TMock> eventExpression, Func<T1, T2, T3, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<TMock> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
			Action<TMock> eventExpression,
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			Action<TMock> eventExpression,
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			Action<TMock> eventExpression,
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			Action<TMock> eventExpression,
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			Action<TMock> eventExpression,
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, EventArgs> func)
		{
			throw new NotImplementedException();
		}

		#endregion

		private readonly IReturnsResult<IBaseMessageContext> _contextMethodCallReturnImplementation;
	}
}
