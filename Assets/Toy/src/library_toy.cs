using System;
using CSString = System.String;
using System.Collections.Generic;

namespace Toy {
	namespace Library {
		public class Toy : IPlugin, IBundle {
			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Toy();
					}
					return singleton;
				}
			}
			static Toy singleton = null;

			//version data
			double major = 0;
			double minor = 1;
			double patch = 1;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(CSString.IsNullOrEmpty(alias) ? "Toy" : alias, this, true);
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					case "version": return $"{major}.{minor}.{patch}";
					case "major": return major;
					case "minor": return minor;
					case "patch": return patch;

					case "VersionGreater": return new VersionGreater(this);
					case "VersionEqual": return new VersionEqual(this);
					case "VersionLess": return new VersionLess(this);

					case "author": return "Kayne Ruse";

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
				}
			}

			public class VersionGreater : ICallable {
				Toy self;

				public VersionGreater(Toy self) {
					this.self = self;
				}

				public int Arity() {
					return 3;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					if (!(arguments[0] is double && arguments[1] is double && arguments[2] is double)) {
						throw new ErrorHandler.RuntimeError(token, "Version info must consist of numbers");
					}

					if ((double)arguments[0] > self.major) return false;
					if ((double)arguments[1] > self.minor) return false;
					if ((double)arguments[2] > self.patch) return false;
					return true;
				}
			}

			public class VersionEqual : ICallable {
				Toy self;

				public VersionEqual(Toy self) {
					this.self = self;
				}

				public int Arity() {
					return 3;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					if (!(arguments[0] is double && arguments[1] is double && arguments[2] is double)) {
						throw new ErrorHandler.RuntimeError(token, "Version info must consist of numbers");
					}

					if ((double)arguments[0] != self.major) return false;
					if ((double)arguments[1] != self.minor) return false;
					if ((double)arguments[2] != self.patch) return false;
					return true;
				}
			}

			public class VersionLess : ICallable {
				Toy self;

				public VersionLess(Toy self) {
					this.self = self;
				}

				public int Arity() {
					return 3;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					if (!(arguments[0] is double && arguments[1] is double && arguments[2] is double)) {
						throw new ErrorHandler.RuntimeError(token, "Version info must consist of numbers");
					}

					if ((double)arguments[0] < self.major) return false;
					if ((double)arguments[1] < self.minor) return false;
					if ((double)arguments[2] < self.patch) return false;
					return true;
				}
			}
		}
	}
}