using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	public class RigidbodyWrapper : IBundle {
		Rigidbody self = null;

		public RigidbodyWrapper(Rigidbody self) {
			this.self = self;
		}

		public object Property(Interpreter interpreter, Token token, object argument) {
			string propertyName = (string)argument;

			switch(propertyName) { //NOTE: All in local-space
				//simple members
				case "PositionX": return new AssignableProperty((val) => { self.MovePosition( self.position + self.gameObject.transform.TransformDirection(new Vector3((float)(double)val - self.position.x, 0, 0)) ); return null; }, (x) => (double)self.position.x);
				case "PositionY": return new AssignableProperty((val) => { self.MovePosition( self.position + self.gameObject.transform.TransformDirection(new Vector3(0, (float)(double)val - self.position.y, 0)) ); return null; }, (x) => (double)self.position.y);
				case "PositionZ": return new AssignableProperty((val) => { self.MovePosition( self.position + self.gameObject.transform.TransformDirection(new Vector3(0, 0, (float)(double)val - self.position.z)) ); return null; }, (x) => (double)self.position.z);

				case "VelocityX": return new AssignableProperty((val) => { self.velocity = new Vector3((float)(double)val, self.velocity.y, self.velocity.z); return null; }, (x) => (double)self.velocity.x);
				case "VelocityY": return new AssignableProperty((val) => { self.velocity = new Vector3(self.velocity.x, (float)(double)val, self.velocity.z); return null; }, (x) => (double)self.velocity.y);
				case "VelocityZ": return new AssignableProperty((val) => { self.velocity = new Vector3(self.velocity.x, self.velocity.y, (float)(double)val); return null; }, (x) => (double)self.velocity.z);

				case "AngularVelocityX": return new AssignableProperty((val) => { self.angularVelocity += new Vector3((float)(double)val - self.angularVelocity.x, 0, 0); return null; }, (x) => (double)self.angularVelocity.x);
				case "AngularVelocityY": return new AssignableProperty((val) => { self.angularVelocity += new Vector3(0, (float)(double)val - self.angularVelocity.y, 0); return null; }, (x) => (double)self.angularVelocity.y);
				case "AngularVelocityZ": return new AssignableProperty((val) => { self.angularVelocity += new Vector3(0, 0, (float)(double)val - self.angularVelocity.z); return null; }, (x) => (double)self.angularVelocity.z);

				case "RotationX": return new AssignableProperty((val) => { self.MoveRotation(self.rotation * Quaternion.Euler((float)(double)val - self.rotation.x, 0, 0)); return null; }, (x) => (double)self.rotation.x);
				case "RotationY": return new AssignableProperty((val) => { self.MoveRotation(self.rotation * Quaternion.Euler(0, (float)(double)val - self.rotation.y, 0)); return null; }, (x) => (double)self.rotation.y);
				case "RotationZ": return new AssignableProperty((val) => { self.MoveRotation(self.rotation * Quaternion.Euler(0, 0, (float)(double)val - self.rotation.z)); return null; }, (x) => (double)self.rotation.z);

				//callable members
				case "AddForce": return new AddForce(this);
				case "AddTorque": return new AddTorque(this);

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
			RigidbodyWrapper self = null;

			public AddForce(RigidbodyWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 4; //x, y, z, mode
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				ForceMode mode;

				switch((string)arguments[3]) {
					case "force":
						mode = ForceMode.Force;
						break;

					case "acceleration":
						mode = ForceMode.Acceleration;
						break;

					case "impulse":
						mode = ForceMode.Impulse;
						break;

					case "velocity":
						mode = ForceMode.VelocityChange;
						break;

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown force mode: " + (string)arguments[3]);
				}

				self.self.AddForce(new Vector3((float)(double)arguments[0], (float)(double)arguments[1], (float)(double)arguments[2]), mode);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public class AddTorque : ICallable {
			RigidbodyWrapper self = null;

			public AddTorque(RigidbodyWrapper self) {
				this.self = self;
			}

			public int Arity() {
				return 4; //x, y, z, mode
			}

			public object Call(Interpreter interpreter, Token token, List<object> arguments) {
				ForceMode mode;

				switch((string)arguments[3]) {
					case "force":
						mode = ForceMode.Force;
						break;

					case "acceleration":
						mode = ForceMode.Acceleration;
						break;

					case "impulse":
						mode = ForceMode.Impulse;
						break;

					case "velocity":
						mode = ForceMode.VelocityChange;
						break;

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown force mode: " + (string)arguments[3]);
				}

				self.self.AddTorque(new Vector3((float)(double)arguments[0], (float)(double)arguments[1], (float)(double)arguments[2]), mode);

				return null;
			}

			public override string ToString() { return "<Unity function>"; }
		}

		public override string ToString() { return "<Unity Rigidbody>"; }
	}
}