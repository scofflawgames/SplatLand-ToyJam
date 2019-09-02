# Animator

This is an overview of the unity Animator via Toy. If you want to learn how to work with or extend the plugin yourself, please see [this tutorial here](tutorial_unity.md).

## Play(name)

This is Unity's [Animator.Play()](https://docs.unity3d.com/ScriptReference/Animator.Play.html).

It takes the name of an animation state to play.

## SetTrigger(name)

This is Unity's [Animator.SetTrigger()](https://docs.unity3d.com/ScriptReference/Animator.SetTrigger.html).

It takes the name of an animation trigger to set for this frame.

## ResetTrigger(name)

This is Unity's [Animator.ResetTrigger()](https://docs.unity3d.com/ScriptReference/Animator.ResetTrigger.html).

It takes the name of an animation trigger that has been set this frame, which is then unset.

## SetBool(name, b)

This is Unity's [Animator.SetBool()](https://docs.unity3d.com/ScriptReference/Animator.SetBool.html).

It takes the name of a boolean animation parameter and sets it to the value of "b" if it exists. Otherwise it raises an error.

## GetBool(name)

This is Unity's [Animator.GetBool()](https://docs.unity3d.com/ScriptReference/Animator.GetBool.html).

It takes the name of a boolean animation parameter and returns it's value, if it exists. Otherwise it raises an error.

## SetInteger(name, n)

This is Unity's [Animator.SetInteger()](https://docs.unity3d.com/ScriptReference/Animator.SetInteger.html).

It takes the name of an integer animation parameter, and sets it to the value of "n" (truncating the decimals) if it exists. Otherwise it raises an error.

## GetInteger(name)

This is Unity's [Animator.GetInteger()](https://docs.unity3d.com/ScriptReference/Animator.GetInteger.html).

It takes the name of an integer animation parameter, and returns the value if it exists. Otherwise it returns an error.

## SetNumber(name, n)

This is Unity's [Animator.SetFloat()](https://docs.unity3d.com/ScriptReference/Animator.SetFloat.html).

It takes the name of a float animation parameter, and sets it to the value of "n" (truncating extra decimals) if it exists. Otherwise it raises an error.

## GetNumber(name)

This is Unity's [Animator.GetFloat()](https://docs.unity3d.com/ScriptReference/Animator.GetFloat.html).

It takes the name of a float animation parameter, and returns the value if it exists. Otherwise it returns an error.

## GameObject

This is the [Unity GameObject](reference_unity_gameobject.md).

