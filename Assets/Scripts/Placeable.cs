/* *Copyright 2018 Rodney DegraciaMIT License:Permission is hereby granted, free of charge, to any person obtaining a copyof this software and associated documentation files (the "Software"), to dealin the Software without restriction, including without limitation the rightsto use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the Software, and to permit persons to whom the Software isfurnished to do so, subject to the following conditions:The above copyright notice and this permission notice shall be included inall copies or substantial portions of the Software.THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS ORIMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THEAUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHERLIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS INTHE SOFTWARE.**/


















































using System;
using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.XR.MagicLeap;














/* * Add this script to any GameObject that is to be pickable and placeable. *  * Note: This script should be used with GameObjects that do not have a Rigidbody,  * since this script only modifies the GameObject transform.position *  */

public class Placeable : MonoBehaviour{    public enum PlaceableState    {        READY,        NOSELECTED,        ELIGIBLE_FOR_SELECTION,        NOT_ELIGIBLE_FOR_SELECTION,        SELECTED    }    [SerializeField]    public Material onHoverMaterial;    [SerializeField]    public Material onSelectedMaterial;    private Material saveMaterial;    private Ray controllerRay;    private float clampDistance;    protected PlaceableState placeableState;    private MLInputController controller;    protected Ray GetControllerRay()    {        return controllerRay;    }    protected float GetClampDistance()    {        return clampDistance;    }    private void Awake()    {        clampDistance = 0.0F;        placeableState = PlaceableState.READY;    }    void Start()    {        saveMaterial = this.gameObject.GetComponent<Renderer>().material;    }    // Update is called once per frame    void Update()    {    }    protected void OnEnable()    {        Cursor.OnCursorMove += OnCursorMove;        Cursor.OnCursorHover += OnCursorHover;        Cursor.OnCursorStopHover += OnCursorStopHover;        InputController.OnTriggerDown += OnTriggerDown;        InputController.OnTriggerUp += OnTriggerUp;        InputController.OnTouchpadGestureStart += OnTouchpadGestureStart;        InputController.OnTouchpadGestureEnd += OnTouchpadGestureEnd;    }

    // UpdateLED - Constants
    private const float HALF_HOUR_IN_DEGREES = 15.0f;
    private const float DEGREES_PER_HOUR = 12.0f / 360.0f;

    private const int MIN_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock12);
    private const int MAX_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock6And12);
    private const int LED_INDEX_DELTA = MAX_LED_INDEX - MIN_LED_INDEX;

    private void OnTouchpadGestureStart(MLInputControllerTouchpadGesture gesture, Cursor cursor, InputController inputController)
    {
        if (placeableState != PlaceableState.ELIGIBLE_FOR_SELECTION)
        {
            return;
        }

        Prestige.InputController pic = inputController.GetPrestigeInputController();

        MLInputController controller = pic.GetMLInputController();
        if (controller.Touch1Active)
        {

            // Get angle of touchpad position.
            float angle = -Vector2.SignedAngle(Vector2.up, controller.Touch1PosAndForce);
            if (angle < 0.0f)
            {
                angle += 360.0f;
            }

            float cooked_angle = angle / 30.0f;

            int index = (int)cooked_angle;
             switch (index)
            {
                case 0:  // 12 oclock
                    {
                        // Increase X rot
                        transform.Rotate(12.0f, 0, 0, Space.World);
                    }
                    break;
                case 1:
                    {
                        // Increase X rot
                        transform.Rotate(12.0f, 0, 0, Space.World);
                    }
                    break;
                case 2:
                    {
                        // Increase y rot
                        transform.Rotate(0, 12f, 0, Space.World);
                    }
                    break;
                case 3:
                    {
                        // Increase y rot
                        transform.Rotate(0, 12f, 0, Space.World);
                    }
                    break;
                case 4:
                    {
                        // Increase y rot
                        transform.Rotate(0, 12f, 0, Space.World);
                    }
                    break;
                case 5:
                    {
                        // Decrease X rot
                        transform.Rotate(-12.0f, 0, 0, Space.World);
                    }
                    break;
                case 6:
                    {
                        // Decrease X rot
                        transform.Rotate(-12.0f, 0, 0, Space.World);
                    }
                    break;

                case 7:  // 12 oclock
                    {
                        // Decrease X rot
                        transform.Rotate(-12.0f, 0, 0, Space.World);
                    }
                    break;
                case 8:
                    {
                        // Decrease y rot
                        transform.Rotate(0, -12f, 0, Space.World);
                    }
                    break;
                case 9:
                    {
                        // Decrease y rot
                        transform.Rotate(0, -12f, 0, Space.World);
                    }
                    break;
                case 10:
                    {
                        // Decrease y rot
                        transform.Rotate(0, -12f, 0, Space.World);
                    }
                    break;
                case 11:
                    {
                        // Increase X rot
                        transform.Rotate(12.0f, 0, 0, Space.World);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTouchpadGestureEnd(MLInputControllerTouchpadGesture gesture, Cursor cursor, InputController inputController)
    {
    }



    protected void OnDisable()    {
        InputController.OnTouchpadGestureStart += OnTouchpadGestureStart;        InputController.OnTouchpadGestureEnd += OnTouchpadGestureEnd;        InputController.OnTriggerDown -= OnTriggerDown;        InputController.OnTriggerUp -= OnTriggerUp;        Cursor.OnCursorMove -= OnCursorMove;        Cursor.OnCursorHover -= OnCursorHover;        Cursor.OnCursorStopHover -= OnCursorStopHover;    }    /*     * We use a statemachine, since events may occur asynchronously,     * to help maintain state.     *      */    protected void ExecuteStateMachine(PlaceableState sm)    {        switch (sm)        {            case PlaceableState.NOSELECTED:                {                    if (placeableState != PlaceableState.SELECTED)                    {                        return;                    }                    clampDistance = 0.0F;   // reset the clamp, because the Gameobject is no longer selected                    placeableState = PlaceableState.READY;                    this.gameObject.GetComponent<Renderer>().material = saveMaterial;                    break;                }            case PlaceableState.NOT_ELIGIBLE_FOR_SELECTION:                {                    if (placeableState != PlaceableState.ELIGIBLE_FOR_SELECTION)                    {                        return;                    }                    placeableState = PlaceableState.READY;                    break;                }            case PlaceableState.ELIGIBLE_FOR_SELECTION:                {                    if (placeableState != PlaceableState.READY)                    {                        return;                    }                    placeableState = PlaceableState.ELIGIBLE_FOR_SELECTION;                    break;                }            case PlaceableState.SELECTED:                {                    if (placeableState != PlaceableState.ELIGIBLE_FOR_SELECTION)                    {                        return;                    }                    placeableState = PlaceableState.SELECTED;                    this.gameObject.GetComponent<Renderer>().material = onSelectedMaterial;                    break;                }            case PlaceableState.READY:                {                    break;                }            default:                break;        }    }    public void OnCursorMove(Ray controllerRay, Transform cursorTransform, RaycastHit? raycast, InputController inputController)    {        if (placeableState == PlaceableState.SELECTED)        {            ///            /// Calculate the distance of the original controller ray, when the game object was            /// first selected            ///            var heading = GetComponent<Renderer>().bounds.center - this.controllerRay.origin;            var distance = heading.magnitude;            // Clamp the distance so that the distance from the InputController to the GameObject            // does not change while the GameObject is selected.            if (Mathf.Abs(clampDistance - 0) < float.Epsilon)            {                clampDistance = distance;            }            // Move the game Object to a position on the Ray, at the clamped distance            Vector3 position = controllerRay.GetPoint(clampDistance);            this.transform.position = position;            this.transform.rotation = inputController.transform.rotation;                    }        if (placeableState == PlaceableState.NOSELECTED)        {            this.controllerRay = controllerRay;        }    }    public void OnCursorHover(GameObject gameObject, Transform cursorTransform, RaycastHit raycastHit, InputController inputController)    {        ///        /// Return if we are not the gameObject that is being hovered by the Cursor        ///        if (this.gameObject.GetInstanceID() != gameObject.GetInstanceID())        {            return;        }               this.gameObject.GetComponent<Renderer>().material = onHoverMaterial;        ExecuteStateMachine(PlaceableState.ELIGIBLE_FOR_SELECTION);    }    public void OnCursorStopHover(GameObject gameObject, InputController inputController)    {        ExecuteStateMachine(PlaceableState.NOT_ELIGIBLE_FOR_SELECTION);        this.gameObject.GetComponent<Renderer>().material = saveMaterial;    }    public void OnTriggerDown(float value, GameObject gameObject, Transform cursorTransform, InputController inputController)    {        ExecuteStateMachine(PlaceableState.SELECTED);    }    public void OnTriggerUp(float value, GameObject gameObject, Transform cursorTransform, InputController inputController)    {        ExecuteStateMachine(PlaceableState.NOSELECTED);    }}