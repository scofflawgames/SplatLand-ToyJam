using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	namespace Plugin {
		public class Globals : IPlugin, ICollection {
			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Globals();
					}
					return singleton;
				}
			}
			Globals singleton = null;

			//members
			static Dictionary<string, object> container;

			//constructor
			public Globals() {
				if (container == null) {
					container = new Dictionary<string, object>();
				}
			}

			//Accessor and mutator for C#
			public object Get(string key) {
				return container.ContainsKey(key) ? container[key] : null;
			}

			public void Set(string key, object val) {
				if (container.ContainsKey(key)) {
					container.Remove(key);
				}
				container.Add(key, val);
			}

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Globals" : alias, this, true);
			}

			//the index assign helper class
			public class GlobalsAssignableIndex : AssignableIndex {
				Dictionary<string, object> container;
				string index;

				public GlobalsAssignableIndex(Dictionary<string, object> container, string index) {
					this.container = container;
					this.index = index;
				}

				public override object Value {
					get {
						return this.container.ContainsKey(index) ? this.container[this.index] : null;
					}
					set {
						if (this.container.ContainsKey(this.index)) {
							this.container.Remove(this.index);
						}
						this.container.Add(this.index, value);
					}
				}
			}

			//ICollection
			public object Access(Interpreter interpreter, Token token, object first, object second, object third) {
				if (second != null || third != null) {
					throw new ErrorHandler.RuntimeError(token, "Globals only takes one slice index");
				}

				if (!(first is string)) {
					throw new ErrorHandler.RuntimeError(token, "Globals only takes a string as a key");
				}

				return new GlobalsAssignableIndex(container, (string)first);
			}
		}
	}
}