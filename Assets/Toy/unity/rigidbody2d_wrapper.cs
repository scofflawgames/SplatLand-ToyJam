using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	public class Rigidbody2DWrapper : IBundle {
		Rigidbody2D self = null;

		public Rigidbody2DWrapper(Rigidbody2D self) {
			this.self = self;
		}

		public object Property(Interpreter interpreter, Token token, object argument) {
			string propertyName = (string)argument;

			switch(propertyName) {
				//simple members
				case "PositionX": return new AssignableProperty((val) => self.position = new Vector2((float)(double)val, self.position.y), (x) => (double)self.position.x);
				case "PositionY": return new AssignableProperty((val) => self.position = new Vector2(self.position.x, (float)(double)val), (x) => (double)self.position.y);
				case "VelocityX": return new AssignableProperty((val) => self.velocity = new Vector2((float)(double)val, self.velocity.y), (x) => (double)self.velocity.x);
				case "VelocityY": return new AssignableProperty((val) => self.velocity = new Vector2(self.velocity.x, (float)(double)val), (x) => (double)self.velocity.y);
				case "Rotation": return new AssignableProperty((val) => self.rotation = (float)(double)val, (x) => (double)self.rotation);

				//callable members
				case "AddForce": return new AddForce(this);

				//game obeject references
				case "GameObject": return new GameObjectWrapper(self.gameObject);

				default:
					throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
			}
		}

		//assignable properties
		public class AssignableProperty : AssignableIndex {
			Func<object, object> Set = null;
			Func<object, object> Get = null;

			public AssignableProperty(Func<object, object> Set, Func<object, object> Get) {
				this.Set = Set;
				this.Get = Get;
			}

			public override object Value {
				set {
					Set(value);
				}
				get {
					return Get(null);
				}
			}
		}

		public class AddForce : ICallable {
			Rigidbody2DWrapper self = null;

			public AddForce(Rigidbody2DWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 3;
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				ForceMode2D mode;

				if ((string)arguments[2] == "force") {
					mode = ForceMode2D.Force;
				} else if ((string)arguments[2] == "impulse") {
					mode = ForceMode2D.Impulse;
				} else {
					throw new ErrorHandler.RuntimeError(token, "Unknown force mode: " + (string)arguments[2]);
				}

				self.self.AddForce(new Vector2((float)(double)arguments[0], (float)(double)arguments[1]), mode);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public override string ToString() { return "<Unity Rigidbody2D>"; }
	}
}