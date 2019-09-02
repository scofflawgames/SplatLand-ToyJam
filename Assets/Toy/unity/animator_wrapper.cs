using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	public class AnimatorWrapper : IBundle {
		Animator self = null;

		public AnimatorWrapper(Animator self) {
			this.self = self;
		}

		public object Property(Interpreter interpreter, Token token, object argument) {
			string propertyName = (string)argument;

			switch(propertyName) {
				//callable members
				case "Play": return new Play(this);
				case "SetTrigger": return new SetTrigger(this);
				case "ResetTrigger": return new ResetTrigger(this);
				case "SetBool": return new SetBool(this);
				case "GetBool": return new GetBool(this);
				case "SetInteger": return new SetInteger(this);
				case "GetInteger": return new GetInteger(this);
				case "SetNumber": return new SetNumber(this);
				case "GetNumber": return new GetNumber(this);

				//game obeject references
				case "GameObject": return new GameObjectWrapper(self.gameObject);

				default:
					throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
			}
		}

		public class Play : ICallable {
			AnimatorWrapper self;

			public Play(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string stateName = (string)arguments[0];

				self.self.Play(stateName);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class SetTrigger : ICallable {
			AnimatorWrapper self;

			public SetTrigger(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];

				self.self.SetTrigger(paramName);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class ResetTrigger : ICallable {
			AnimatorWrapper self;

			public ResetTrigger(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];

				self.self.ResetTrigger(paramName);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class SetBool : ICallable {
			AnimatorWrapper self;

			public SetBool(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 2;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];
				bool b = (bool)arguments[1];

				self.self.SetBool(paramName, b);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class GetBool : ICallable {
			AnimatorWrapper self;

			public GetBool(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];

				return self.self.GetBool(paramName);
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class SetInteger : ICallable {
			AnimatorWrapper self;

			public SetInteger(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 2;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];
				int i = (int)(double)arguments[1];

				self.self.SetInteger(paramName, i);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class GetInteger : ICallable {
			AnimatorWrapper self;

			public GetInteger(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];

				return (double)self.self.GetInteger(paramName);
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class SetNumber : ICallable {
			AnimatorWrapper self;

			public SetNumber(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 2;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];
				float f = (float)(double)arguments[1];

				self.self.SetFloat(paramName, f);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class GetNumber : ICallable {
			AnimatorWrapper self;

			public GetNumber(AnimatorWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 1;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				string paramName = (string)arguments[0];

				return (double)self.self.GetFloat(paramName);
			}

			public override string ToString() { return "<Unity function>"; }
		}


		public override string ToString() { return "<Unity Animator>"; }
	}
}