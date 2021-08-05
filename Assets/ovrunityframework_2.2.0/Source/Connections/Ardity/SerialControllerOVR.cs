/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System.Threading;
using System.Text;
using System.IO.Ports;
using OVR.API;


public class SerialControllerOVR : MonoBehaviour
{
    int baudRate = 115200;
    public GameObject messageListener;
    int reconnectionDelay = 1000;
    public int maxUnreadMessages = 5;
    public byte separator = 90;
    // Internal reference to the Thread and the object that runs in it.
    protected Thread thread;
    protected SerialThreadOVR serialThread;

    public bool isConnected;

    // Constants used to mark the start and end of a connection. There is no
    // way you can generate clashing messages from your serial device, as I
    // compare the references of these strings, no their contents. So if you
    // send these same strings from the serial device, upon reconstruction they
    // will have different reference ids.
    public const string SERIAL_DEVICE_CONNECTED = "__Connected__";
    public const string SERIAL_DEVICE_DISCONNECTED = "__Disconnected__";

    public static string[] GetPortNames()
    {
        return SerialPort.GetPortNames();
    }

    

    public bool Connect(string port)
    {
        //Debug.Log("Connect: " + port);
        //Debug.Log("SerialControllerOVR: Connect: " + port);
        serialThread = new SerialThreadOVR(port, baudRate, reconnectionDelay, maxUnreadMessages, separator);
        thread = new Thread(new ThreadStart(serialThread.RunForever));
        thread.Start();
        isConnected = true;
        return true;
    }

    public void OnEnable()
    {
        messageListener = gameObject;
    }

    public void OnDisable()
    {
        // If there is a user-defined tear-down function, execute it before
        // closing the underlying COM port.
        if (userDefinedTearDownFunction != null)
            userDefinedTearDownFunction();

        // The serialThread reference should never be null at this point,
        // unless an Exception happened in the OnEnable(), in which case I've
        // no idea what face Unity will make.
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }

        // This reference shouldn't be null at this point anyway.
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
        isConnected = false;
    }

    void Update()
    {
        if (isConnected)
        {
            if (messageListener == null)
                return;
            byte[] message = ReadSerialMessage();
            if (message == null)
                return;
            // Check if the message is plain data or a connect/disconnect event.
            if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
                messageListener.SendMessage("OnConnectionEvent", true);
            else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
                messageListener.SendMessage("OnConnectionEvent", false);
            else
                messageListener.SendMessage("OnMessageArrived", message);
        }
    }

    public byte[] ReadSerialMessage()
    {
        // Read the next message from the queue
        return (byte[])serialThread.ReadMessage();
    }

    public void SendSerialMessage(byte[] message)
    {
        if(serialThread!=null)
            serialThread.SendMessage(message);
    }
    public delegate void TearDownFunction();
    private TearDownFunction userDefinedTearDownFunction;
    public void SetTearDownFunction(TearDownFunction userFunction)
    {
        this.userDefinedTearDownFunction = userFunction;
    }
    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(byte[] msg)
    {
        OVRMessage.ReceiveMessage(msg);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

}
