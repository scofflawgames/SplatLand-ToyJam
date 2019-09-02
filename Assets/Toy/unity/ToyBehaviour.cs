using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	public class ToyBehaviour : MonoBehaviour, IBundle {
		//Toy members
		[SerializeField]
		string toyScript = "";

		Environment environment;
		public Dictionary<string, ScriptFunction> propertyMethods = new Dictionary<string, ScriptFunction>();

		ScriptFunction GetPropertyMethod(string propertyName, int argCount) {
			//this function not overwritten
			if (!propertyMethods.ContainsKey(propertyName)) {
				//dummy args
				List<Expr> args = new List<Expr>();
				for (int i = 0; i < argCount; i++) {
					args.Add(new Variable( new Token(TokenType.IDENTIFIER, "param" + i.ToString(), null, -1) ));
				}

				//NO-OP
				return new ScriptFunction(new Function(args, new List<Stmt>() { new Pass(new Token(TokenType.PASS, "", null, -1)) }), null);
			}
			return propertyMethods[propertyName];
		}

		//assignable properties
		public class AssignableProperty : AssignableIndex {
			ToyBehaviour self = null;
			string propertyName;
			int argCount = -1;

			public AssignableProperty(ToyBehaviour self, string propertyName, int argCount) {
				this.self = self;
				this.propertyName = propertyName;
				this.argCount = argCount;
			}

			public override object Value {
				set {
					self.propertyMethods[propertyName] = (ScriptFunction)value;
				}
				get {
					return self.GetPropertyMethod(propertyName, argCount);
				}
			}
		}

		//IBundle
		public object Property(Interpreter interpreter, Token token, object argument) {
			string propertyName = (string)argument;

			switch(propertyName) {
				case "Awake":
				case "Start":
				case "OnDestroy":
				case "OnEnable":
				case "OnDisable":
				case "FixedUpdate":
				case "Update":
				case "LateUpdate":

				case "OnMouseEnter":
				case "OnMouseOver":
				case "OnMouseExit":

				return new AssignableProperty(this, propertyName, 0);

				case "OnCollisionEnter":
				case "OnCollisionStay":
				case "OnCollisionExit":

				case "OnTriggerEnter":
				case "OnTriggerStay":
				case "OnTriggerExit":

				case "OnCollisionEnter2D":
				case "OnCollisionStay2D":
				case "OnCollisionExit2D":

				case "OnTriggerEnter2D":
				case "OnTriggerStay2D":
				case "OnTriggerExit2D":

					return new AssignableProperty(this, propertyName, 1);

				//game obeject references
				case "GameObject": return new GameObjectWrapper(gameObject); //TODO: using new here will break IsSame()

				default:
					throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
			}
		}

		public override string ToString() { return "<Unity ToyBehaviour>"; }

		//public script files
		public void RunFile(string fileName) {
			environment = Runner.RunFile(environment, Application.streamingAssetsPath + "/" + fileName + ".toy");

			if (environment == null) {
				throw new NullReferenceException("Environment is null in ToyBehaviour.RunFile()");
			}
		}

		public void Run(string script) {
			environment = Runner.Run(environment, script);

			if (environment == null) {
				throw new NullReferenceException("Environment is null in ToyBehaviour.Run()");
			}
		}

		//creation/destruction methods (unity glue functions)
		void Awake() {
			environment = new Environment();

			environment.Define("this", new GameObjectWrapper(this.gameObject), true);

			if (!String.IsNullOrEmpty(toyScript)) {
				RunFile(toyScript);
			}

			Runner.Run(environment, GetPropertyMethod("Awake", 0), new List<object>());
		}

		void Start() {
			Runner.Run(environment, GetPropertyMethod("Start", 0), new List<object>());
		}

		void OnDestroy() {
			Runner.Run(environment, GetPropertyMethod("OnDestroy", 0), new List<object>());
		}

		void OnEnable() {
			Runner.Run(environment, GetPropertyMethod("OnEnable", 0), new List<object>());
		}

		void OnDisable() {
			Runner.Run(environment, GetPropertyMethod("OnDisable", 0), new List<object>());
		}

		//loop methods
		void FixedUpdate() {
			Runner.Run(environment, GetPropertyMethod("FixedUpdate", 0), new List<object>());
		}

		void Update() {
			Runner.Run(environment, GetPropertyMethod("Update", 0), new List<object>());
		}

		void LateUpdate() {
			Runner.Run(environment, GetPropertyMethod("LateUpdate", 0), new List<object>());
		}

		//physics methods
		void OnCollisionEnter(Collision collision) {
			Runner.Run(environment, GetPropertyMethod("OnCollisionEnter", 1), new List<object>() { new GameObjectWrapper(collision.gameObject) });
		}

		void OnCollisiionStay(Collision collision) {
			Runner.Run(environment, GetPropertyMethod("OnCollisionStay", 1), new List<object>() { new GameObjectWrapper(collision.gameObject) });
		}

		void OnCollisionExit(Collision collision) {
			Runner.Run(environment, GetPropertyMethod("OnCollisionExit", 1), new List<object>() { new GameObjectWrapper(collision.gameObject) });
		}

		void OnTriggerEnter(Collider collider) {
			Runner.Run(environment, GetPropertyMethod("OnTriggerEnter", 1), new List<object>() { new GameObjectWrapper(collider.gameObject) });
		}

		void OnTriggerStay(Collider collider) {
			Runner.Run(environment, GetPropertyMethod("OnTriggerStay", 1), new List<object>() { new GameObjectWrapper(collider.gameObject) });
		}

		void OnTriggerExit(Collider collider) {
			Runner.Run(environment, GetPropertyMethod("OnTriggerExit", 1), new List<object>() { new GameObjectWrapper(collider.gameObject) });
		}

		void OnCollisionEnter2D(Collision2D collision) {
			Runner.Run(environment, GetPropertyMethod("OnCollisionEnter2D", 1), new List<object>() { new GameObjectWrapper(collision.gameObject) });
		}

		void OnCollisionStay2D(Collision2D collision) {
			Runner.Run(environment, GetPropertyMethod("OnCollisionStay2D", 1), new List<object>() { new GameObjectWrapper(collision.gameObject) });
		}

		void OnCollisionExit2D(Collision2D collision) {
			Runner.Run(environment, GetPropertyMethod("OnCollisionExit2D", 1), new List<object>() { new GameObjectWrapper(collision.gameObject) });
		}

		void OnTriggerEnter2D(Collider2D collider) {
			Runner.Run(environment, GetPropertyMethod("OnTriggerEnter2D", 1), new List<object>() { new GameObjectWrapper(collider.gameObject) });
		}

		void OnTriggerStay2D(Collider2D collider) {
			Runner.Run(environment, GetPropertyMethod("OnTriggerStay2D", 1), new List<object>() { new GameObjectWrapper(collider.gameObject) });
		}

		void OnTriggerExit2D(Collider2D collider) {
			Runner.Run(environment, GetPropertyMethod("OnTriggerExit2D", 1), new List<object>() { new GameObjectWrapper(collider.gameObject) });
		}

		//input methods
		void OnMouseEnter() {
			Runner.Run(environment, GetPropertyMethod("OnMouseEnter", 0), new List<object>());
		}

		void OnMouseOver() {
			Runner.Run(environment, GetPropertyMethod("OnMouseOver", 0), new List<object>());
		}

		void OnMouseExit() {
			Runner.Run(environment, GetPropertyMethod("OnMouseExit", 0), new List<object>());
		}
	}
}