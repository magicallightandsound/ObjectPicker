using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR.MagicLeap;

public class InputControllerTestScript {

    [Test]
    public void HasACursor() {
        // Use the Assert class to test conditions.

        var inputController = new GameObject("InputController");
        var inputControllerScript = inputController.AddComponent<InputController>();
        Assert.IsNotNull(inputControllerScript);

        var cursor = new GameObject("Cursor");
        var cursorScript = cursor.AddComponent<Cursor>();
        Assert.IsNotNull(inputControllerScript);
        
        // cursorScript.inputController = inputControllerScript;
        inputControllerScript.cursor = cursorScript;

        Assert.IsNotNull(inputControllerScript.cursor);
    }

    [Test]
    public void HasTriggerDownEventQueue() {
        InputController.OnTriggerDown += OnTriggerDown;
        InputController.OnTriggerDown -= OnTriggerDown;
    }

    [Test]
    public void HasTriggerUpEventQueue()
    {
        InputController.OnTriggerUp += OnTriggerUp;
        InputController.OnTriggerUp -= OnTriggerUp;
    }

    [Test]
    public void HasTouchpadGestureStartQueue()
    {
        InputController.OnTouchpadGestureStart += TouchpadGestureStart;
        InputController.OnTouchpadGestureStart -= TouchpadGestureStart;
    }

    [Test]
    public void HasTouchpadGestureEndQueue()
    {
        InputController.OnTouchpadGestureEnd += TouchpadGestureEnd;
        InputController.OnTouchpadGestureEnd -= TouchpadGestureEnd;
    }



  



    public void OnTriggerDown(byte controllerId, float value, GameObject gameObject, Transform cursorTransform)    {    }

    public void OnTriggerUp(byte controllerId, float value, GameObject gameObject, Transform cursorTransform)    {    }

    public void TouchpadGestureStart(byte controllerId,        MLInputControllerTouchpadGesture gesture,        Cursor cursor)
    {

    }

    public void TouchpadGestureEnd(byte controllerId,        MLInputControllerTouchpadGesture gesture,        Cursor cursor)
    {

    }
    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
