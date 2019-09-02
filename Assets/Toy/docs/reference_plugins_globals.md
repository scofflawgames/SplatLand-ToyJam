# Globals

"Globals" is a collection of key-value pairs (with strings as keys) which can be accessed by multiple scripts being run by the Unity Game Engine, as well as allowing access to these values from C#.

Please ensure that Globals is in the global environment of the ToyBehaviour or ToyInterface that uses it, otherwise it won't be available to any callbacks in that behaviour or interface.

## Usage

```
//player_character.toy
import "Globals";

this.Behaviour.FixedUpdate = () => {
	//...
	if (attack) {
		Globals["score"] = Globals["score"] + 1;
	}
	//...
};
```

```
//score_text.toy
import "Standard";
import "Globals";

Globals["score"] = 0;

this.Behaviour.Update = () => {
	this.TextMesh.SetText("Score: " + ToString( Globals["score"] ));
};
```

## Indexing

Globals can be indexed using traditional bracket notation. Existing elements can be accessed or overwritten, or new ones inserted if they don't already exist this way:

```
Globals["foo"] = "bar";
print Globals["foo"];
```

# C-Sharp

## Usage

```
void Update() {
	if (Input.GetKey("p")) {
		Toy.Plugin.Globals globals = new Toy.Plugin.Globals();

		Debug.Log(globals.Get("score")); //print the score to the unity console
	}
}
```

Two utility functions are provided for accessing the static container of a 

## public object Get(string key);

This accesses the value with the associated key "key" if it exists, otherwise it returns null.

## public void Set(string key, object val);

This creates or overwrites the key-value pair with they key "key". The object "val" will be set as it's new value.

WARNING

This function lacks internal type-checking. Please ensure that the value of "val" is a valid object in Toy (most notably, toy only uses doubles for numbers - it never uses floats. Also, GameObjects and certain components can only be processed within wrappers).

