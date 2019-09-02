using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toy {
	namespace Plugin {
		public class Unity : IPlugin, IBundle {
			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Unity();
					}
					return singleton;
				}
			}
			Unity singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Unity" : alias, this, true);
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					//access members
					case "FetchGameObject": return new FetchGameObject(this);
					case "LoadGameObject": return new LoadGameObject(this);
					case "LoadGameObjectAt": return new LoadGameObjectAt(this);
					case "IsSameGameObject": return new IsSameGameObject(this);

					//virtual input members
					case "GetAxis": return new GetAxis(this);
					case "GetButton": return new GetButton(this);
					case "GetButtonDown": return new GetButtonDown(this);
					case "GetButtonUp": return new GetButtonUp(this);

					//time members
					case "Time": return (double)Time.time;
					case "DeltaTime": return (double)Time.deltaTime;

					case "FixedTime": return (double)Time.fixedTime;
					case "FixedDeltaTime": return (double)Time.fixedDeltaTime;

					case "UnscaledTime": return (double)Time.unscaledTime;
					case "UnscaledDeltaTime": return (double)Time.unscaledDeltaTime;

					case "FixedUnscaledTime": return (double)Time.fixedUnscaledTime;
					case "FixedUnscaledDeltaTime": return (double)Time.fixedUnscaledDeltaTime;

					case "TimeScale": return new AssignableProperty((val) => Time.timeScale = (float)(double)val, (x) => (double)Time.timeScale);

					//scene management
					case "LoadScene": return new LoadScene(this);

					//TODO: real input members

					//TODO: separate UI behaviour

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

			//access members
			public class FetchGameObject : ICallable {
				Unity self = null;

				public FetchGameObject(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					GameObject go = GameObject.Find((string)arguments[0]);

					if (go == null) {
						throw new ErrorHandler.RuntimeError(token, "No GameObject named '" + (string)arguments[0] + "' could be found");
					}

					return new GameObjectWrapper(go);
				}
			}

			public class LoadGameObject : ICallable {
				Unity self = null;

				public LoadGameObject(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					GameObject go = Resources.Load((string)arguments[0]) as GameObject;

					if (go == null) {
						throw new ErrorHandler.RuntimeError(token, "Failed to load that resource");
					}

					return new GameObjectWrapper(GameObject.Instantiate(go));
				}
			}

			public class LoadGameObjectAt : ICallable {
				Unity self = null;

				public LoadGameObjectAt(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 1 + 3 + 3;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					Vector3 position;
					position.x = (float)(double)arguments[1];
					position.y = (float)(double)arguments[2];
					position.z = (float)(double)arguments[3];

					Vector3 rotation;
					rotation.x = (float)(double)arguments[4];
					rotation.y = (float)(double)arguments[5];
					rotation.z = (float)(double)arguments[6];

					GameObject go = Resources.Load((string)arguments[0]) as GameObject;

					if (go == null) {
						throw new ErrorHandler.RuntimeError(token, "Failed to load that resource");
					}

					return new GameObjectWrapper(GameObject.Instantiate(go, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z)));
				}
			}

			public class IsSameGameObject : ICallable {
				Unity self = null;

				public IsSameGameObject(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 2;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					GameObjectWrapper goLeft = (GameObjectWrapper)arguments[0];
					GameObjectWrapper goRight = (GameObjectWrapper)arguments[1];

					return System.Object.ReferenceEquals(goLeft.GetSelf(), goRight.GetSelf());
				}
			}

			//virtual input members
			public class GetAxis : ICallable {
				Unity self = null;

				public GetAxis(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return (double)Input.GetAxis((string)arguments[0]);
				}

				public override string ToString() { return "<Unity function>"; }
			}

			public class GetButton : ICallable {
				Unity self = null;

				public GetButton(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return Input.GetButton((string)arguments[0]);
				}

				public override string ToString() { return "<Unity function>"; }
			}

			public class GetButtonDown : ICallable {
				Unity self = null;

				public GetButtonDown(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return Input.GetButtonDown((string)arguments[0]);
				}

				public override string ToString() { return "<Unity function>"; }
			}

			public class GetButtonUp : ICallable {
				Unity self = null;

				public GetButtonUp(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return Input.GetButtonUp((string)arguments[0]);
				}

				public override string ToString() { return "<Unity function>"; }
			}

			public class LoadScene : ICallable {
				Unity self = null;

				public LoadScene(Unity self) {
					this.self = self;
				}

				public int Arity() {
					return 2;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					string sceneName = (string)arguments[0];
					string sceneMode = (string)arguments[1];

					LoadSceneMode mode;

					switch(sceneMode) {
						case "Single":
							mode = LoadSceneMode.Single;
							break;

						case "Additive":
							mode = LoadSceneMode.Additive;
							break;

						default:
							throw new ErrorHandler.RuntimeError(token, "Unexpected scene mode " + sceneMode);
					}

					SceneManager.LoadScene(sceneName, mode);
					return null;
				}

				public override string ToString() { return "<Unity function>"; }
			}

			public override string ToString() { return "<Unity plugin>"; }
		}
	}
}

