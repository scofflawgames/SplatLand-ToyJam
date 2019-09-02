# Interface

This is an overview of the unity Interface via Toy. If you want to learn how to work with or extend the plugin yourself, please see [this tutorial here](tutorial_unity.md).

## OnPointerEnter(x, y)

This is called from Unity's [IPointerEnterHandler.OnPointerEnter()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"x" and "y" are the position of the pointer when it entered the bounds of this UI element.

## OnPointerExit(x, y)

This is called from Unity's [IPointerExitHandler.OnPointerExit()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"x" and "y" are the position of the pointer when it exited the bounds of this UI element.

## OnPointerDown(x, y)

This is called from Unity's [IPointerDownHandler.OnPointerDown()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"x" and "y" are the position of the pointer when it was pressed.

## OnPointerUp(x, y)

This is called from Unity's [IPointerUpHandler.OnPointerUp()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"x" and "y" are the position of the pointer when it was released.

## OnPointerClick(x, y)

This is called from Unity's [IPointerClickHandler.OnPointerClick()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"x" and "y" are the position of the pointer when it was pressed and released.

## OnBeginDrag(x, y)

This is called from Unity's [IBeginDragHandler.OnBeginDrag()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"x" and "y" are the position of the pointer when the drag began.

## OnDrag(x, y)

This is called from Unity's [IDragHandler.OnDrag()](https://docs.unity3d.com/Manual/SupportedEvents.html).

This function is called on every tick of the drag. "x" and "y" are the position of the pointer.

## OnEndDrag(x, y)

This is called from Unity's [IEndDragHandler.OnEndDrag()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"x" and "y" are the position of the pointer when the drag ended.

## OnDrop(x, y)

This is called from Unity's [IDropHandler.OnDrop()](https://docs.unity3d.com/Manual/SupportedEvents.html).

This function is called on the object at the drop position, not on the dragged object itself. "x" and "y" are the position of the pointer when the drag ended.

## OnScroll(xDelta, yDelta)

This is called from Unity's [IScrollHandler.OnScroll()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"xDelta" and "yDelta" are the amount to scroll in this tick.

## OnUpdateSelected()

This is called from Unity's [IUpdateSelectedHandler.OnUpdateSelected()](https://docs.unity3d.com/Manual/SupportedEvents.html).

This function is called each tick in the selected object.

## OnSelect()

This is called from Unity's [ISelectHandler.OnSelect()](https://docs.unity3d.com/Manual/SupportedEvents.html).

This function is called when the UI element is selected.

## OnDeselect()

This is called from Unity's [IDeselectHandler.OnDeselect()](https://docs.unity3d.com/Manual/SupportedEvents.html).

This function is called when the UI element is deselected.

## OnMove(xMove, yMove)

This is called from Unity's [IMoveHandler.OnMove()](https://docs.unity3d.com/Manual/SupportedEvents.html).

"xMove" and "yMove" are the raw axis input, from -1 to 1.

## OnSubmit()

This is called from Unity's [ISubmitHandler.OnSubmit()](https://docs.unity3d.com/Manual/SupportedEvents.html).

This function is called in the selected object when the submit button is pressed.

## OnCancel()

This is called from Unity's [ICancelHandler.OnCancel()](https://docs.unity3d.com/Manual/SupportedEvents.html).

This function is called in the selected object when the cancel button is pressed.

## GameObject

This is the [Unity GameObject](reference_unity_gameobject.md).
