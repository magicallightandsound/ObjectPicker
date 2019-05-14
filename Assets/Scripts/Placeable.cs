/* *Copyright 2018 Rodney DegraciaMIT License:Permission is hereby granted, free of charge, to any person obtaining a copyof this software and associated documentation files (the "Software"), to dealin the Software without restriction, including without limitation the rightsto use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the Software, and to permit persons to whom the Software isfurnished to do so, subject to the following conditions:The above copyright notice and this permission notice shall be included inall copies or substantial portions of the Software.THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS ORIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THEAUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHERLIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS INTHE SOFTWARE.**/


















































using System;
using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.XR.MagicLeap;














/* * Add this script to any GameObject that is to be pickable and placeable. *  * Note: This script should be used with GameObjects that do not have a Rigidbody,  * since this script only modifies the GameObject transform.position *  */

public class Placeable : MonoBehaviour{    public enum PlaceableState    {        READY,        NOSELECTED,        ELIGIBLE_FOR_SELECTION,        NOT_ELIGIBLE_FOR_SELECTION,        SELECTED    }    [SerializeField]    public Material onHoverMaterial;    [SerializeField]    public Material onSelectedMaterial;    private Material saveMaterial;    private Ray controllerRay;    private float clampDistance;    private bool selectForTouchPadRotation = false;    protected PlaceableState placeableState;    protected Ray GetControllerRay()    {        return controllerRay;    }    protected float GetClampDistance()    {        return clampDistance;    }    private void Awake()    {        clampDistance = 0.0F;        placeableState = PlaceableState.READY;    }    void Start()    {        saveMaterial = this.gameObject.GetComponent<Renderer>().material;    }    // Update is called once per frame    void Update()    {    }    protected void OnEnable()    {        Cursor.OnCursorMove += OnCursorMove;        Cursor.OnCursorHover += OnCursorHover;        Cursor.OnCursorStopHover += OnCursorStopHover;        InputController.OnTriggerDown += OnTriggerDown;        InputController.OnTriggerUp += OnTriggerUp;        InputController.OnButtonDown += OnButtonDown;        InputController.OnButtonUp += OnButtonUp;        InputController.OnTouchpadGestureStart += OnTouchpadGestureStart;        InputController.OnTouchpadGestureEnd += OnTouchpadGestureEnd;        InputController.OnTouchpadGestureState += OnTouchpadGestureState;    }

    protected void OnDisable()    {        InputController.OnTouchpadGestureState -= OnTouchpadGestureState;        InputController.OnTouchpadGestureEnd -= OnTouchpadGestureEnd;        InputController.OnTouchpadGestureEnd -= OnTouchpadGestureEnd;        InputController.OnButtonDown -= OnButtonDown;        InputController.OnButtonUp -= OnButtonUp;        InputController.OnTriggerDown -= OnTriggerDown;        InputController.OnTriggerUp -= OnTriggerUp;        Cursor.OnCursorMove -= OnCursorMove;        Cursor.OnCursorHover -= OnCursorHover;        Cursor.OnCursorStopHover -= OnCursorStopHover;    }    /*     * We use a statemachine, since events may occur asynchronously,     * to help maintain state.     *      */    protected void ExecuteStateMachine(PlaceableState sm)    {        switch (sm)        {            case PlaceableState.NOSELECTED:                {                    if (placeableState != PlaceableState.SELECTED)                    {                        return;                    }                    clampDistance = 0.0F;   // reset the clamp, because the Gameobject is no longer selected                    placeableState = PlaceableState.READY;                    this.gameObject.GetComponent<Renderer>().material = saveMaterial;

                    selectForTouchPadRotation = false;                    break;                }            case PlaceableState.NOT_ELIGIBLE_FOR_SELECTION:                {                    if (placeableState != PlaceableState.ELIGIBLE_FOR_SELECTION)                    {                        return;                    }                    placeableState = PlaceableState.READY;

                    selectForTouchPadRotation = false;                    break;                }            case PlaceableState.ELIGIBLE_FOR_SELECTION:                {                    if (placeableState != PlaceableState.READY)                    {                        return;                    }                    placeableState = PlaceableState.ELIGIBLE_FOR_SELECTION;                    selectForTouchPadRotation = true;                    break;                }            case PlaceableState.SELECTED:                {                    if (placeableState != PlaceableState.ELIGIBLE_FOR_SELECTION)                    {                        return;                    }                    placeableState = PlaceableState.SELECTED;                    this.gameObject.GetComponent<Renderer>().material = onSelectedMaterial;
                    selectForTouchPadRotation = true;                    break;                }            case PlaceableState.READY:                {                    break;                }            default:                break;        }    }    public void OnCursorMove(Ray controllerRay, Transform cursorTransform, Transform controllerTransform, RaycastHit? raycast)    {        if (placeableState == PlaceableState.SELECTED)        {            ///            /// Calculate the distance of the original controller ray, when the game object was            /// first selected            ///            var heading = transform.position - controllerRay.origin;            var distance = heading.magnitude;            // Clamp the distance so that the distance from the InputController to the GameObject            // does not change while the GameObject is selected.            if (Mathf.Abs(clampDistance - 0) < float.Epsilon)            {                clampDistance = distance;            }            // Move the game Object to a position on the Ray, at the clamped distance            Vector3 position = controllerRay.GetPoint(clampDistance);            this.transform.position = position;        }    }    public void  OnCursorHover(GameObject gameObject, Transform cursorTransform, RaycastHit raycastHit)    {        ///        /// Return if we are not the gameObject that is being hovered by the Cursor        ///        if (this.gameObject.GetInstanceID() != gameObject.GetInstanceID())        {            return;        }               this.gameObject.GetComponent<Renderer>().material = onHoverMaterial;        ExecuteStateMachine(PlaceableState.ELIGIBLE_FOR_SELECTION);            }    public void OnCursorStopHover(GameObject gameObject)    {        ExecuteStateMachine(PlaceableState.NOT_ELIGIBLE_FOR_SELECTION);        this.gameObject.GetComponent<Renderer>().material = saveMaterial;    }    public void OnTriggerDown(byte controllerId, float value, GameObject gameObject, Transform cursorTransform)    {        ExecuteStateMachine(PlaceableState.SELECTED);    }    public void OnTriggerUp(byte controllerId, float value, GameObject gameObject, Transform cursorTransform)    {        ExecuteStateMachine(PlaceableState.NOSELECTED);    }



    public void OnButtonDown(byte controllerId, MLInputControllerButton button, GameObject gameObject, Transform cursorTransform)    {            }    public void OnButtonUp(byte controllerId, MLInputControllerButton button, GameObject gameObject, Transform cursorTransform)    {
        ;    }

    private void OnTouchpadGestureEnd(byte controllerId, MLInputControllerTouchpadGesture gesture, Cursor cursor)
    {
        

    }

    private void OnTouchpadGestureStart(byte controllerId, MLInputControllerTouchpadGesture gesture, Cursor cursor)
    {
        if (!selectForTouchPadRotation)
        {
            return;
        }

        bool axial = false;
 
        switch (gesture.Type)
        {
            case MLInputControllerTouchpadGestureType.None:
                break;
            case MLInputControllerTouchpadGestureType.Tap:
                break;
            case MLInputControllerTouchpadGestureType.ForceTapDown:
                break;
            case MLInputControllerTouchpadGestureType.ForceTapUp:
                break;
            case MLInputControllerTouchpadGestureType.ForceDwell:
                break;
            case MLInputControllerTouchpadGestureType.SecondForceDown:
                break;
            case MLInputControllerTouchpadGestureType.LongHold:
                break;
            case MLInputControllerTouchpadGestureType.RadialScroll:
                break;
            case MLInputControllerTouchpadGestureType.Swipe:
                axial = true;
                break;
            case MLInputControllerTouchpadGestureType.Scroll:
                axial = true;
                break;
            case MLInputControllerTouchpadGestureType.Pinch:
                break;
            default:
                break;
        }

        switch (gesture.Direction)
        {
            case MLInputControllerTouchpadGestureDirection.None:
                break;
            case MLInputControllerTouchpadGestureDirection.Up:
                if (axial)
                {
                    transform.Rotate(22, 0, 0, Space.World);
                }
                break;
            case MLInputControllerTouchpadGestureDirection.Down:
                if (axial)
                {
                    transform.Rotate(-22, 0, 0, Space.World);
                }
                break;
            case MLInputControllerTouchpadGestureDirection.Left:
                if (axial)
                {
                    transform.Rotate(0, 22, 0, Space.World);
                }
                break;
            case MLInputControllerTouchpadGestureDirection.Right:
                if (axial)
                {
                    transform.Rotate(0, -22, 0, Space.World);
                }
                break;
            case MLInputControllerTouchpadGestureDirection.In:
                break;
            case MLInputControllerTouchpadGestureDirection.Out:
                break;
            case MLInputControllerTouchpadGestureDirection.Clockwise:

                break;
            case MLInputControllerTouchpadGestureDirection.CounterClockwise:

                break;
            default:
                break;
        }

    }


    private void OnTouchpadGestureState(MLInputControllerTouchpadGesture gesture)
    {
        if (!selectForTouchPadRotation)
        {
            return;
        }
        bool radial = false;


        switch (gesture.Type)
        {
            case MLInputControllerTouchpadGestureType.None:
                break;
            case MLInputControllerTouchpadGestureType.Tap:
                break;
            case MLInputControllerTouchpadGestureType.ForceTapDown:
                break;
            case MLInputControllerTouchpadGestureType.ForceTapUp:
                break;
            case MLInputControllerTouchpadGestureType.ForceDwell:
                break;
            case MLInputControllerTouchpadGestureType.SecondForceDown:
                break;
            case MLInputControllerTouchpadGestureType.LongHold:
                break;
            case MLInputControllerTouchpadGestureType.RadialScroll:
                radial = true;
                break;
            case MLInputControllerTouchpadGestureType.Swipe:
                break;
            case MLInputControllerTouchpadGestureType.Scroll:
                break;
            case MLInputControllerTouchpadGestureType.Pinch:
                break;
            default:
                break;
        }

        switch (gesture.Direction)
        {
            case MLInputControllerTouchpadGestureDirection.None:
                break;
            case MLInputControllerTouchpadGestureDirection.Up:

                break;
            case MLInputControllerTouchpadGestureDirection.Down:

                break;
            case MLInputControllerTouchpadGestureDirection.Left:

                break;
            case MLInputControllerTouchpadGestureDirection.Right:

                break;
            case MLInputControllerTouchpadGestureDirection.In:
                break;
            case MLInputControllerTouchpadGestureDirection.Out:
                break;
            case MLInputControllerTouchpadGestureDirection.Clockwise:
                if (radial)
                {
                    transform.Rotate(0, 0, 5, Space.World);
                }

                break;
            case MLInputControllerTouchpadGestureDirection.CounterClockwise:
                if (radial)
                {
                    transform.Rotate(0, 0, -5, Space.World);
                }
                break;
            default:
                break;
        }
    }
}