/*
 *
Copyright 2018 Rodney Degracia

MIT License:

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*
*/


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Assertions;

/*
 *  Prestige is a namespace that wraps the Magic Leap SDK objects
 *  for easier programming by keeping as much of the Magic Leap SDK objects
 *  within the Prestige namespace, thereby keeping the Magic Leap specific code
 *  from polluting the Application code. By keeping the Application code as 
 *  Unity-specific as possible, Unity developers should have an easier time
 *  porting Unity specific codebases to work with Magic Leap controller input via Prestige.
 *  
 */


namespace Prestige
{
    /*
     * Possible devices that may be used as Controllers
     * 
     * Interestingly, the left hand controller is considered by Magic Leap to be the first controller
     * 
     */
    [Flags]
    public enum DeviceType : int
    {
        MobileApp = 1 << 0,
        ControllerFirst = 1 << 1,   // Left
        ControllerSecond = 1 << 2,  // Right
    }

    /*
     * InputControllerManager manages the possible devices that may be
     * used as Magic Leap Controllers.
     * 
     * This class manages the connection and disconnection of Magic Leap controllers, via
     * a list of MLInputControllers.
     * 
     */
    public class InputControllerManager
    {


        private DeviceType deviceTypesAllowed = (DeviceType)~0;

        private List<MLInputController> inputControllers = new List<MLInputController>();

        private MLInputConfiguration inputConfiguration;

        protected MLInputConfiguration GetMLInputConfiguration()
        {
            return inputConfiguration;
        }

        protected List<MLInputController> GetInputControllers()
        {
            return inputControllers;
        }

        public InputControllerManager()
        {
            inputConfiguration = new MLInputConfiguration(MLInputConfiguration.DEFAULT_TRIGGER_DOWN_THRESHOLD,
                                                        MLInputConfiguration.DEFAULT_TRIGGER_UP_THRESHOLD,
                                                        true);
        }

        public InputControllerManager(MLInputConfiguration inputConfiguration)
        {
            this.inputConfiguration = inputConfiguration;
        }


        virtual public void Start()
        {
            if (!MLInput.IsStarted)
            {
                if (MagicLeapDevice.IsReady())
                {
                    MLResult result = MLInput.Start(inputConfiguration);
                    if (result.IsOk == true)
                    {
                        MLInput.OnControllerConnected += HandleOnControllerConnected;
                        MLInput.OnControllerDisconnected += HandleOnControllerDisconnected;

                        MLInputController leftController = MLInput.GetController(MLInput.Hand.Left);
                        AddInputController(leftController);

                        MLInputController rightController = MLInput.GetController(MLInput.Hand.Right);
                        AddInputController(rightController);

                        MLInputController mobileController = MLInput.GetController(0);  // Mobile
                        AddInputController(mobileController);
                    }
                }

            }

        }

        virtual public void AddInputController(MLInputController newController)
        {
            if (IsDeviceAllowed(newController))
            {
                if (inputControllers.Exists((device) => device.Id == newController.Id))
                {
                    Debug.LogWarning(string.Format("Connected controller with id {0} already connected.", newController));
                    return;
                }

                inputControllers.Add(newController);
            }
        }

        virtual public void Stop()
        {
            if (MLInput.IsStarted)
            {
                MLInput.Stop();

                MLInput.OnControllerConnected += HandleOnControllerConnected;
                MLInput.OnControllerDisconnected += HandleOnControllerDisconnected;
            }

        }

        public MLInputController GetFirstController()
        {
            Assert.IsNotNull(inputControllers);
            foreach (var inputController in inputControllers)
            {

                bool isLeftHandController = (inputController.Type == MLInputControllerType.Control && inputController.Hand == MLInput.Hand.Left);
                if (isLeftHandController)
                {
                    return inputController;
                }

            }
            return null;
        }

        public MLInputController GetSecondController()
        {
            Assert.IsNotNull(inputControllers);
            foreach (var inputController in inputControllers)
            {
                bool isRightHandController = (inputController.Type == MLInputControllerType.Control && inputController.Hand == MLInput.Hand.Right);
                if (isRightHandController)
                {
                    return inputController;
                }

            }
            return null;
        }

        public MLInputController GetMobileAppController()
        {

            foreach (var inputController in inputControllers)
            {
                bool isMobileApp = (inputController.Type == MLInputControllerType.MobileApp);
                if (isMobileApp)
                {
                    return inputController;
                }

            }
            return null;
        }

        protected bool IsDeviceAllowed(MLInputController inputController)
        {
            if (inputController == null || !inputController.Connected)
            {
                return false;
            }

            bool isMobileApp = ((deviceTypesAllowed & DeviceType.MobileApp) != 0 && inputController.Type == MLInputControllerType.MobileApp);
            bool isLeftHandController = ((deviceTypesAllowed & DeviceType.ControllerFirst) != 0 && inputController.Type == MLInputControllerType.Control && inputController.Hand == MLInput.Hand.Left);
            bool isRightHandController = ((deviceTypesAllowed & DeviceType.ControllerSecond) != 0 && inputController.Type == MLInputControllerType.Control && inputController.Hand == MLInput.Hand.Right);

            return (isMobileApp || isLeftHandController || isRightHandController);
        }

        virtual protected void HandleOnControllerConnected(byte controllerId)
        {
            MLInputController newController = MLInput.GetController(controllerId);
            Assert.IsNotNull(newController);

            if (IsDeviceAllowed(newController))
            {
                if (inputControllers.Exists((device) => device.Id == controllerId))
                {
                    Debug.LogWarning(string.Format("Connected controller with id {0} already connected.", controllerId));
                    return;
                }

                inputControllers.Add(newController);
            }
        }


        virtual protected void HandleOnControllerDisconnected(byte controllerId)
        {
            inputControllers.RemoveAll((device) => device.Id == controllerId);
        }
    }

    /*
     * InputController wraps the MLInput of an MLInputController
     * and provides access to the associated MLInputController, if desired.
     */
    public class InputController
    {

        private DeviceType deviceType;
        private static InputControllerManager controllerManager;

        protected DeviceType GetDeviceType()
        {
            return deviceType;
        }

        protected static InputControllerManager GetControllerManager()
        {
            return controllerManager;
        }

        public InputController()
        {
            if (controllerManager == null)
            {
                controllerManager = new InputControllerManager();
            }

        }

        public InputController(MLInputConfiguration inputConfiguration)
        {
            if (controllerManager == null)
            {
                controllerManager = new InputControllerManager(inputConfiguration);
            }

        }

        public InputController(DeviceType deviceType)
        {
            if (controllerManager == null)
            {
                controllerManager = new InputControllerManager();
            }

            this.deviceType = deviceType;
        }

        public void Start()
        {
            Assert.IsTrue(controllerManager != null);
            controllerManager.Start();

        }

        public void Stop()
        {

            controllerManager.Stop();
        }


        public void RegisterTouchpadGestureStartHandler(MLInput.ControllerTouchpadGestureDelegate callback)
        {
            MLInput.OnControllerTouchpadGestureStart += callback;
        }

        private void MLInput_OnControllerTouchpadGestureStart(byte controllerId, MLInputControllerTouchpadGesture touchpadGesture)
        {
            throw new NotImplementedException();
        }

        public void RegisterTouchpadGestureEndHandler(MLInput.ControllerTouchpadGestureDelegate callback)
        {
            MLInput.OnControllerTouchpadGestureStart += callback;
        }

        public void UnregisterTouchpadGestureStartHandler(MLInput.ControllerTouchpadGestureDelegate callback)
        {
            MLInput.OnControllerTouchpadGestureStart -= callback;
        }

        public void UnregisterTouchpadGestureEndHandler(MLInput.ControllerTouchpadGestureDelegate callback)
        {
            MLInput.OnControllerTouchpadGestureStart -= callback;
        }

        public void RegisterTriggerDownHandler(MLInput.TriggerDelegate callback)
        {
            MLInput.OnTriggerDown += callback;

        }

        public void RegisterTriggerUpHandler(MLInput.TriggerDelegate callback)
        {
            MLInput.OnTriggerUp += callback;
        }

        public void UnregisterTriggerDownHandler(MLInput.TriggerDelegate callback)
        {
            MLInput.OnTriggerDown += callback;

        }

        public void UnregisterTriggerUpHandler(MLInput.TriggerDelegate callback)
        {
            MLInput.OnTriggerUp += callback;
        }

        public void RegisterButtonDownHandler(MLInput.ControllerButtonDelegate callback)
        {
            MLInput.OnControllerButtonDown += callback;
        }

        public void RegisterButtonUpHandler(MLInput.ControllerButtonDelegate callback)
        {
            MLInput.OnControllerButtonUp += callback;
        }

        public void UnregisterButtonDownHandler(MLInput.ControllerButtonDelegate callback)
        {
            MLInput.OnControllerButtonDown -= callback;

        }

        public void UnregisterButtonUpHandler(MLInput.ControllerButtonDelegate callback)
        {
            MLInput.OnControllerButtonUp -= callback;
        }

        public MLInputController GetMLInputController()
        {
            Assert.IsNotNull(controllerManager);

            switch (deviceType)
            {
                case DeviceType.MobileApp:
                    {
                        return controllerManager.GetMobileAppController();
                    }
                case DeviceType.ControllerFirst:
                    {
                        return controllerManager.GetFirstController();
                    }
                case DeviceType.ControllerSecond:
                    {
                        return controllerManager.GetSecondController();
                    }
                default:
                    Assert.IsTrue(false);   // Should not assert
                    return null;
            }
        }

    }

    /*
     * Wraps MLWorldRays.QueryParams() behavior and calls back to the
     * Application code via worldRaysCallback. 
     * 
     */
    public class WorldRaysManager
    {

        public delegate void WorldRaysCallback(MLWorldRays.MLWorldRaycastResultState state, RaycastHit result, float confidence);

        private WorldRaysCallback worldRaysCallback;

        private MLWorldRays.QueryParams queryParams = new MLWorldRays.QueryParams();

        private Func<MLWorldRays.MLWorldRaycastResultState, RaycastHit, float> callback;

        protected MLWorldRays.QueryParams QueryParams()
        {
            return queryParams;
        }

        public WorldRaysManager()
        {
            queryParams = new MLWorldRays.QueryParams();

        }

        public WorldRaysManager(Hashtable options)
        {
            queryParams = new MLWorldRays.QueryParams();

            queryParams.Width = (uint)options["Width"];
            queryParams.Height = (uint)options["Height"];
            queryParams.HorizontalFovDegrees = (float)options["HorizontalFovDegrees"];
            queryParams.CollideWithUnobserved = (bool)options["CollideWithUnobserved"];
        }

        virtual public void Start(WorldRaysCallback worldRaysCallback)
        {
            if (MagicLeapDevice.IsReady())
            {
                MLResult result = MLWorldRays.Start();

                this.worldRaysCallback = worldRaysCallback;
            }

        }

        virtual public void Update(Vector3 position, Vector3 direction, Vector3 up)
        {
            if (MLWorldRays.IsStarted)
            {
                queryParams.Position = position;
                queryParams.Direction = direction;
                queryParams.UpVector = up;

                MLWorldRays.GetWorldRays(queryParams, HandleOnReceiveRaycast);
            }
        }

        virtual public void Stop()
        {
            if (MLWorldRays.IsStarted)
            {
                MLWorldRays.Stop();
            }
        }

        virtual protected void HandleOnReceiveRaycast(MLWorldRays.MLWorldRaycastResultState state, Vector3 point, Vector3 normal, float confidence)
        {
            bool hasRequestFailed = (state == MLWorldRays.MLWorldRaycastResultState.RequestFailed);
            bool noCollision = (state == MLWorldRays.MLWorldRaycastResultState.NoCollision);

            if (!hasRequestFailed && !noCollision)
            {
                RaycastHit result = new RaycastHit();

                result.point = point;
                result.normal = normal;
                result.distance = Vector3.Distance(queryParams.Position, point);

                worldRaysCallback(state, result, confidence);
            }
        }

    }

}
