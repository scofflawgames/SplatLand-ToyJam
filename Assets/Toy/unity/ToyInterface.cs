using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Toy {
	public class ToyInterface : MonoBehaviour,
		//toy interface
		IBundle,
		//enter/exit with the mouse
		IPointerEnterHandler,
		IPointerExitHandler,
		//press/release with the mouse
		IPointerDownHandler,
		IPointerUpHandler,
		IPointerClickHandler,
		//dragging with the mouse
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IDropHandler, //NOTE: called on the object at the drop location
		//mouse wheel
		IScrollHandler,
		//update
		IUpdateSelectedHandler,
		//select/deselect
		ISelectHandler,
		IDeselectHandler,
		//movement (up, down, left, right)
		IMoveHandler,
		//input
		ISubmitHandler,
		ICancelHandler
	{
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
			ToyInterface self = null;
			string propertyName;
			int argCount = -1;

			public AssignableProperty(ToyInterface self, string propertyName, int argCount) {
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
				case "OnUpdateSelected":
				case "OnSelect":
				case "OnDeselect":
				case "OnSubmit":
				case "OnCancel":

				return new AssignableProperty(this, propertyName, 0);

				case "OnPointerEnter":
				case "OnPointerExit":
				case "OnPointerDown":
				case "OnPointerUp":
				case "OnPointerClick":
				case "OnBeginDrag":
				case "OnDrag":
				case "OnEndDrag":
				case "OnDrop":

				case "OnScroll":
				case "OnMove":

				return new AssignableProperty(this, propertyName, 2);

				//game obeject references
				case "GameObject": return new GameObjectWrapper(gameObject); //TODO: using new here will break IsSame()

				default:
					throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
			}
		}

		public override string ToString() { return "<Unity ToyInterface>"; }

		//public script files
		public void RunFile(string fileName) {
			environment = Runner.RunFile(environment, Application.streamingAssetsPath + "/" + fileName + ".toy");

			if (environment == null) {
				throw new NullReferenceException("Environment is null in ToyInterface.RunFile()");
			}
		}

		public void Run(string script) {
			environment = Runner.Run(environment, script);

			if (environment == null) {
				throw new NullReferenceException("Environment is null in ToyInterface.Run()");
			}
		}

		//creation/destruction methods (unity glue functions)
		void Awake() {
			environment = new Environment();

			environment.Define("this", new GameObjectWrapper(this.gameObject), true);

			if (!String.IsNullOrEmpty(toyScript)) {
				RunFile(toyScript);
			}
		}

		//Unity UI members
		public void OnPointerEnter(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnPointerEnter", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnPointerExit(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnPointerExit", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnPointerDown(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnPointerDown", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnPointerUp(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnPointerUp", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnPointerClick(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnPointerClick", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnBeginDrag(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnBeginDrag", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnDrag(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnDrag", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnEndDrag(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnEndDrag", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnDrop(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnDrop", 2), new List<object>() { pointerEvent.position.x, pointerEvent.position.y });
		}

		public void OnScroll(PointerEventData pointerEvent) {
			Runner.Run(environment, GetPropertyMethod("OnScroll", 2), new List<object>() { pointerEvent.scrollDelta.x, pointerEvent.scrollDelta.y });
		}

		public void OnUpdateSelected(BaseEventData e) {
			Runner.Run(environment, GetPropertyMethod("OnUpdateSelected", 0), new List<object>());
		}

		public void OnSelect(BaseEventData e) {
			Runner.Run(environment, GetPropertyMethod("OnSelect", 0), new List<object>());
		}

		public void OnDeselect(BaseEventData e) {
			Runner.Run(environment, GetPropertyMethod("OnDeselect", 0), new List<object>());
		}

		public void OnMove(AxisEventData axisEvent) {
			Runner.Run(environment, GetPropertyMethod("OnMove", 2), new List<object>() { axisEvent.moveVector.x, axisEvent.moveVector.y });
		}

		public void OnSubmit(BaseEventData e) {
			Runner.Run(environment, GetPropertyMethod("OnSubmit", 0), new List<object>());
		}

		public void OnCancel(BaseEventData e) {
			Runner.Run(environment, GetPropertyMethod("OnCancel", 0), new List<object>());
		}
	}
}