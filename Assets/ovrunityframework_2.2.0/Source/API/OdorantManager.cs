using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System;

using UnityEngine;
using OVR.API;
//using OVR.Components;
using OVR.Data;
//using OVR.DataCollection;

namespace OVR.API
{
    public class OdorantManager : MonoBehaviour
    {
        public List<OdorantCommand> _commands = new List<OdorantCommand>();
        private byte[] _packet;
        private int _maxCommandsPerPacket=9;

        public static OdorantManager instance;
        public void Awake(){ instance = this;}

        public void AddCommand(OdorantCommand command)
        {
            if (_commands.Count >= _maxCommandsPerPacket)
                return;

            _commands.Add(command);
        }

        public void AddCommands(IEnumerable<OdorantCommand> commands)
        //public void AddCommands(List<OdorantCommand> commands)
        {
            if (commands.Count() + _commands.Count > _maxCommandsPerPacket)
                return;
            _commands.AddRange(commands);
        }

        public bool HasCommandsToSend()
        {
            return _commands.Any();
        }
        public void ClearCommands()
        {
            _commands.Clear();
        }

    }
}
