//////////////////////////////////////////////////////////////////////////////////
//
//  Architecture of Scent(TM) Software Suite - OVR Unity Framework
//  Copyright (C) 2018-2019 OVR Tech LLC
//
//    OVR Unity Framework is proprietary software: you can modify it under the
//    terms of the Development Kit Agreement signed before receiving the
//    software. This Software is owned by OVR Tech LLC and any modifications,
//    changes, or alterations are subject to the terms of the Agreement.
//
//    OVR Unity Framework is distributed in the hope that it will be useful,for
//    testing purposes, and for feedback, but WITHOUT ANY WARRANTY; without even
//    the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//    See the Agreement for more details.
//
//    You should have received a copy of the Development Kit Agreement before
//    receiving the OVR Unity Framework. If not, this software may be in violation
//    of the Agreement. Please contact info@ovrtechnology.com or click the link
//    below to request the document.
//
//  https://ovrtechnology.com/contact/
//
//////////////////////////////////////////////////////////////////////////////////

using OVR.API;
using OVR.Data;
using OVR.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OVR.Components
{
  /// <summary>
  /// This component represents the nose of the player and should be placed on the HMD observant
  /// game-object in the scene hierarchy.
  /// </summary>
  public class OlfactoryEpithelium : MonoBehaviour
  {
        public bool shouldAutoConnect = true;
        public AutoConnectType autoConnectType;
        AutoConnect autoConnect;

        public static bool IsActive = true;
    //public string OvrDeviceHostname = "OVRv0_2_0";
    public Vector3 Position { get { return transform.TransformPoint(_offset); } }
    public static bool WaitForLoadBalance { get { return OdorantsProcessingThisFrame > 10; } }
    public static int OdorantsProcessingThisFrame = 0;
    public float BurstUpdateInterval { get; private set; }
    [SerializeField] private Vector3 _offset = new Vector3(0.0f, -0.05f, 0.08f);
    private static OlfactoryEpithelium Instance = null;
    private static List<OdorantCommand> _odorantCommands = new List<OdorantCommand>();
    private List<string> _odorantLogMessages = new List<string>();
    //private IGatewayBlacklist _blacklist = null;
    private int _maxNumOdorants = 9;
    private float _maintainConnectionTimer = 0.5f;
    private float _maintainConnectionTimerInterval = 1.0f;
    /*private static IDeviceGateway _gateway = null;
    private static IDeviceGateway _usb = null;
    private static IDeviceGateway _wifi = null;
    */

    private TextMesh _textMesh = null;
    private float _textTimer = 0.1f;



        void OnEnable()
        {
            //add needed components
            gameObject.AddComponent<ConnectionManager>();
            autoConnect = gameObject.AddComponent<AutoConnect>();
            autoConnect.SetAutoConnectionType(autoConnectType);

            BurstUpdateInterval = 0.05f;
            if (Instance == this)
                return;

            if (!Instance)

            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                throw new MultipleOlfactoryException("More than one OlfactoryEpithelium constructed.");
            }
        }

        void OnDisable()

        {
            _odorantCommands.Clear();
        }

        void Start()
        {
            if(shouldAutoConnect)
                AutoConnect.instance.Init();
        }
        private void OnDestroy()
        {
        }

        void Update()
        {
            if (!maintainDeviceConnection())
                _odorantCommands.Clear();

            // reset coroutine counter for load balancing
            OdorantsProcessingThisFrame = 0;

            //are ther any scents we are not using
            //if (_blacklist != null)
              //  _blacklist.Update();

            //need to send message
            if (ConnectionManager.instance.isConnected)
            {
                OdorantManager.instance.AddCommands(_odorantCommands);
                if (OdorantManager.instance.HasCommandsToSend())
                {
                    ConnectionManager.instance.SendData(MessageTypes.ODORANT_COMMANDS);
                    OdorantManager.instance.ClearCommands();
                }
            }
            _odorantCommands.Clear();
        }
        /// <summary>
        /// Adds a command only if the registry is active, the odorant command is not in the blacklist and
        /// there is not already another odorant destine for the same slot.
        /// </summary>
        /// <param name="command">Incoming odorant commands from Odorant components in the scene.</param>
        public void AddOdorantCommand(OdorantCommand command)
        {
            // is the epithelium active?
            if (!IsActive)
                return;
            // if the name or slit is in the blacklist, stop
            /*if (Get()._blacklist != null)
            {
                if (Get()._blacklist.Slots().Contains(command.Slot) || Get()._blacklist.Names().Contains(command.Name))
                    return;
            }*/

            // not already full of commands (max) and command is none zero
            if (_odorantCommands.Count >= Get()._maxNumOdorants || command.Intensity == 0)
            {
                return;
            }
            //Don't fully understand the logic of why he is doing it this way
            // if there is already that command
            //it still adds it, but it averages the intensity for the one it found
            var indexOfDuplicate = _odorantCommands.FindIndex(c => c.Slot == command.Slot);
            if (indexOfDuplicate >= 0)
            {
                _odorantCommands[indexOfDuplicate].Intensity = (byte)((command.Intensity + _odorantCommands[indexOfDuplicate].Intensity) / 2);
            }
            //add the odorant command
            _odorantCommands.Add(command);
        }
        public static bool Instanced ()
        {
            return Instance != null;
        }
        /// <summary>
        /// A temporary solution for getting an instance of this object in Odorant Components. This
        /// strategy will work fine in standalone systems or where there can only be one VR user.
        /// </summary>
        /// <returns>The one and only instance of the object.</returns>
        public static OlfactoryEpithelium Get()
        {
            if (!Instance)
            {
                Debug.LogWarningFormat("<b>[OVR]</b> Olfactory could not be found. Please place one on the player's HMD observant game-object.");
            }
            return Instance;
        }

        /// <summary>
        /// Used to connect and maintain device connectivity. Defaults to USB, if that fails WiFi connection
        /// is attempted. OVR Gateway, a standalone desktop service, will eventually replace this functionality.
        /// </summary>
        /// <returns>Only returns false if connection fails or no device is connected</returns>
        private bool maintainDeviceConnection()
        {
            return true;
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, Position);
            Gizmos.DrawRay(Position, transform.rotation * Quaternion.Euler(Vector3.right * 30.0f) * _offset * -0.3f);
        }
    }
}
