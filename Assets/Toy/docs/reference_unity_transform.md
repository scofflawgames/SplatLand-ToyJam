# Transform

This is an overview of the unity Transform via Toy. If you want to learn how to work with or extend the plugin yourself, please see [this tutorial here](tutorial_unity.md).

Please note that in Toy, the transform only handles parent-child relationships. To manage the GameObject's position and rotation, see [Rigidbody2D](reference_unity_rigidbody2d.md).

## SetParent(otherTransform, stay)

This is Unity's [SetParent method](https://docs.unity3d.com/ScriptReference/Transform.SetParent.html). It takes another Transform as the first argument, and a boolean as the second. if "stay" is true, the parent-relative position, rotation and scale are modified such that the GameObject keeps the same world space position, rotation and scale as before.

## GetParent()

This returns the parent's transform.

## IsChildOf(other)

This is Unity's [Transform.IsChildOf()](https://docs.unity3d.com/ScriptReference/Transform.IsChildOf.html).

This takes a Transform as it's argument.

## SetSiblingIndex(index)

This is Unity's [Transform.SetSiblingIndex()](https://docs.unity3d.com/ScriptReference/Transform.SetSiblingIndex.html).

## GetSiblingIndex()

This is Unity's [Transform.GetSiblingIndex()](https://docs.unity3d.com/ScriptReference/Transform.GetSiblingIndex.html). It returns a number.

## GetChild(index)

This is Unity's [Transform.GetChild()](https://docs.unity3d.com/ScriptReference/Transform.GetChild.html). It returns a Transform.

## GetChildCount()

This returns the number of children that this transform has.

## GameObject

This is the [Unity GameObject](reference_unity_gameobject.md).
