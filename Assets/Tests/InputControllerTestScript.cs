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
        var inputControllerScript = inputController.AddComponent<ActsAsInputController>();
        Assert.IsNotNull(inputControllerScript);

        var cursor = new GameObject("Cursor");
        var cursorScript = cursor.AddComponent<ActsAsCursor>();
        Assert.IsNotNull(inputControllerScript);
        
        // cursorScript.inputController = inputControllerScript;
        inputControllerScript.cursor = cursorScript;

        Assert.IsNotNull(inputControllerScript.cursor);
    }

    [Test]
    public void HasTriggerDownEventQueue() {
        ActsAsInputController.OnTriggerDown += OnTriggerDown;
        ActsAsInputController.OnTriggerDown -= OnTriggerDown;
    }

    [Test]
    public void HasTriggerUpEventQueue()
    {
        ActsAsInputController.OnTriggerUp += OnTriggerUp;
        ActsAsInputController.OnTriggerUp -= OnTriggerUp;
    }

    [Test]
    public void HasTouchpadGestureStartQueue()
    {
        ActsAsInputController.OnTouchpadGestureStart += TouchpadGestureStart;
        ActsAsInputController.OnTouchpadGestureStart -= TouchpadGestureStart;
    }

    [Test]
    public void HasTouchpadGestureEndQueue()
    {
        ActsAsInputController.OnTouchpadGestureEnd += TouchpadGestureEnd;
        ActsAsInputController.OnTouchpadGestureEnd -= TouchpadGestureEnd;
    }



  



    public void OnTriggerDown(byte controllerId, float value, GameObject gameObject, Transform cursorTransform)
    {
    }

    public void OnTriggerUp(byte controllerId, float value, GameObject gameObject, Transform cursorTransform)
    {
    }

    public void TouchpadGestureStart(byte controllerId,
        MLInputControllerTouchpadGesture gesture,
        ActsAsCursor cursor)
    {

    }

    public void TouchpadGestureEnd(byte controllerId,
        MLInputControllerTouchpadGesture gesture,
        ActsAsCursor cursor)
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
