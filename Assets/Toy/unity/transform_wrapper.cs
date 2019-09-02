using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	class TransformWrapper : IBundle {
		Transform self = null;
		TransformWrapper parentWrapper = null;

		public TransformWrapper(Transform self) {
			this.self = self;
		}

		//IBundle
		public object Property(Interpreter interpreter, Token token, object argument) {
			string propertyName = (string)argument;

			if (self == null) {
				throw new ErrorHandler.RuntimeError(token, "Can't access property of null Unity Transform");
			}

			switch(propertyName) {
				//parent-child stuff
				case "SetParent": return new SetParent(this);
				case "GetParent": return new GetParent(this);
				case "IsChildOf": return new IsChildOf(this);

				case "SetSiblingIndex": return new SetSiblingIndex(this);
				case "GetSiblingIndex": return new GetSiblingIndex(this);

				case "GetChild": return new GetChild(this);
				case "GetChildCount": return new GetChildCount(this);

				//game obeject references
				case "GameObject": return new GameObjectWrapper(self.gameObject);

				default:
					throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
			}
		}

		public class SetParent : ICallable {
			TransformWrapper self = null;

			public SetParent(TransformWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 2;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				if (arguments[0] == null) {
					self.self.parent = null;
					return null;
				}

				if (!(arguments[0] is TransformWrapper)) {
					throw new ErrorHandler.RuntimeError(token, "First argument must be a unity transform");
				}

				TransformWrapper other = (TransformWrapper)arguments[0];

				self.self.SetParent(other.self, (bool)arguments[1]);
				self.parentWrapper = other;

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class GetParent : ICallable {
			TransformWrapper self = null;

			public GetParent(TransformWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 0;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				return self.parentWrapper;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class IsChildOf : ICallable {
			TransformWrapper self = null;

			public IsChildOf(TransformWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				if (!(arguments[0] is TransformWrapper)) {
					throw new ErrorHandler.RuntimeError(token, "First argument must be a unity transform");
				}

				TransformWrapper other = (TransformWrapper)arguments[0];

				return self.self.IsChildOf(other.self);
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class SetSiblingIndex : ICallable {
			TransformWrapper self = null;

			public SetSiblingIndex(TransformWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				self.self.SetSiblingIndex((int)(double)arguments[0]);
				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class GetSiblingIndex : ICallable {
			TransformWrapper self = null;

			public GetSiblingIndex(TransformWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 0;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				return (double)self.self.GetSiblingIndex();
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class GetChild : ICallable {
			TransformWrapper self = null;

			public GetChild(TransformWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				return new TransformWrapper( self.self.GetChild((int)(double)arguments[0]) );
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class GetChildCount : ICallable {
			TransformWrapper self = null;

			public GetChildCount(TransformWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 0;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				return (double)self.self.childCount;
			}

			public override string ToString() { return "<Unity function>"; }
		}


		public override string ToString() { return "<Unity Transform>"; }
	}
}