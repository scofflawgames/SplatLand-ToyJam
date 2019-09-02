# Unity Plugin

This is an overview of the unity plugin via Toy. If you want to learn how to work with or extend the plugin yourself, please see [this tutorial here](tutorial_unity.md).

## FetchGameObject(name)

This function takes a case sensitive string as an argument. The argument must be the name of an object in the unity scene. If the object can't be found, then it causes an error.

## LoadGameObject(fname)

This function takes a filepath as an argument, and returns a newly instantiated GameObject. This uses Unity's resource system, so the prefab must be in a folder named "Resources".

## LoadGameObjectAt(fname, x, y, z, rx, ry, rz)

This function takes a filepath as an argument, and returns a newly instantiated GameObject. This uses Unity's resource system, so the prefab must be in a folder named "Resources".

The parameters "x", "y" and "z" indicate the location to instantiate the GameObject, and "rx", "ry" and "rz" are the rotations to use.

## IsSameGameObject(first, second)

This function is a replacement for Standard's IsSame() function, as the latter doesn't work with unity types. It returns true if "first" and "second" refer to the same GameObject.

# Virtual Input Members

## GetAxis(name)

This function takes a virtual axis name as an argument, and returns it's value. Virtual axes can be set up in unity under File -> Build Settings -> Player Settings -> Input.

## GetButton(name)

This function takes a virtual button name as an argument, and returns true if it is pressed. Virtual axes can be set up in unity under File -> Build Settings -> Player Settings -> Input.

## GetButtonDown(name)

This function takes a virtual button name as an argument, and returns true it was pressed this frame. Virtual axes can be set up in unity under File -> Build Settings -> Player Settings -> Input.

## GetButtonUp(name)

This function takes a virtual button name as an argument, and returns true it was released this frame. Virtual axes can be set up in unity under File -> Build Settings -> Player Settings -> Input.

# Time Members

## Time

This is Unity's [Time.time](https://docs.unity3d.com/ScriptReference/Time-time.html) as a number.

## DeltaTime

This is Unity's [Time.deltaTime](https://docs.unity3d.com/ScriptReference/Time-deltaTime.html) as a number.

## FixedTime

This is Unity's [Time.fixedTime](https://docs.unity3d.com/ScriptReference/Time-fixedTime.html) as a number.

## FixedDeltaTime

This is Unity's [Time.fixedDeltaTime](https://docs.unity3d.com/ScriptReference/Time-fixedDeltaTime.html) as a number.

## UnscaledTime

This is Unity's [Time.unscaledTime](https://docs.unity3d.com/ScriptReference/Time-unscaledTime.html) as a number.

## UnscaledDeltaTime

This is Unity's [Time.unscaledDeltaTime](https://docs.unity3d.com/ScriptReference/Time-unscaledDeltaTime.html) as a number.

## FixedUnscaledTime

This is Unity's [Time.fixedUnscaledTime](https://docs.unity3d.com/ScriptReference/Time-fixedUnscaledTime.html) as a number.

## FixedUnscaledDeltaTime

This is Unity's [Time.fixedUnscaledDeltaTime](https://docs.unity3d.com/ScriptReference/Time-fixedUnscaledDeltaTime.html) as a number.

## TimeScale

This is Unity's [Time.timeScale](https://docs.unity3d.com/ScriptReference/Time-timeScale.html) as a number. It can be changed.

# Scene Management

## LoadScene(name, mode)

This function takes two arguments. The first one is the name of a unity scene to load, and the second is either "Single" or "Additive". Passing "Single" will unload the current scene first, while passing "Additive" will not.

