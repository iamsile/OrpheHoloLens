﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System;

#if WINDOWS_UWP
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Orphe;
#endif



namespace Orpe
{
    public class OrpheTest : MonoBehaviour
    {
        public GameObject UITextPrefab;
        bool isInit = false;

        private Transform model;
        private TextMesh logmsg;

        private Dictionary<AxisBar.AxisType, AxisBar> axis;


        private void Start()
        {
            axis = new Dictionary<AxisBar.AxisType, AxisBar>();


            axis.Add(AxisBar.AxisType.X, GameObject.Find("axisX").GetComponent<AxisBar>());
            axis.Add(AxisBar.AxisType.Y, GameObject.Find("axisY").GetComponent<AxisBar>());
            axis.Add(AxisBar.AxisType.Z, GameObject.Find("axisZ").GetComponent<AxisBar>());

            model = transform.Find("Capsule");
            logmsg = transform.Find("Message").GetComponent<TextMesh>();
            logmsg.text = "Step 1. Select a device to connect.";


#if WINDOWS_UWP
            StartOrphe();
#endif
        }


#if WINDOWS_UWP
        private void StartOrphe()
        {
            //initialize and setup callback
            OrpheDeviceManager.Instance.Init("orphe6R");

            OrpheDeviceManager.Instance.OnDeviceAdded += IdAddedtoUI;
            OrpheDeviceManager.Instance.OnDeviceRemoved += IdRemovedtoUI;
            OrpheDeviceManager.Instance.OnConnected += OnConnected;
            OrpheDeviceManager.Instance.OnConnectFailed += OnConnectFailed;
            OrpheDeviceManager.Instance.OnValueChanged += ValueChanged;
        }

        private void OnConnectFailed(string msg)
        {
            if (OrpheDeviceManager.Instance.isConnected)
            {
                logmsg.text = "Alrealdy connected.";
            }
            else
            {
                logmsg.text = "Failed to connect. Please change the orphe to Pairing-Mode and connect again.";
            }
            

        }

        private void OnConnected(string msg)
        {
            logmsg.text = "Success to connect:" + msg;
            // throw new NotImplementedException();
        }

        private void ValueChanged(OrpheValueChangedEventArgs e)
        {
            //  model.localRotation = OrpheDeviceManager.Instance.deviceQ;
            model.localRotation = new Quaternion((float)e.Quaternion.x, (float)e.Quaternion.y, (float)e.Quaternion.z, (float)e.Quaternion.w);
            model.localEulerAngles += new Vector3(-90, 0, 0);
            //  logmsg.text = model.localRotation.ToString();

            float speed = 5;
            axis[AxisBar.AxisType.X].val = 5 + (float)e.Acceleration.x * speed;
            axis[AxisBar.AxisType.Y].val = 5 + (float)e.Acceleration.y * speed;
            axis[AxisBar.AxisType.Z].val = 5 + (float)e.Acceleration.z * speed;

            logmsg.text = "Shock:" + e.Shock;
            logmsg.text += "\r\nGestureDirection:" + e.GestureDirection.ToString();
            logmsg.text += "\r\nGestureName:" + e.GestureName.ToString();
            


        }

        private void OnDestroy()
        {
            OrpheDeviceManager.Instance.Init("orphe6R");
            OrpheDeviceManager.Instance.OnDeviceAdded -= IdAddedtoUI;
            OrpheDeviceManager.Instance.OnDeviceRemoved -= IdRemovedtoUI;
            OrpheDeviceManager.Instance.OnConnected -= OnConnected;
            OrpheDeviceManager.Instance.OnConnectFailed -= OnConnectFailed;
            OrpheDeviceManager.Instance.OnValueChanged -= ValueChanged;
        }


#endif

        // Update is called once per frame
        void Update()
        {
            // Debug.Log("Update");
#if !WINDOWS_UWP
            if (Input.GetKeyUp(KeyCode.Space))
            {
                //   Debug.Log("isPrepared");
                //isPrepare = true;
                IdAddedtoUI("hoge");
            }
#endif
        }
        public void IdAddedtoUI(string text)
        {
            Debug.Log("IdAddedtoUI" + text);
            if (UITextPrefab == null)
            {
                Debug.Log("UITextPrefab not found");
                return;
            }

            GameObject g = GameObject.Instantiate(UITextPrefab);
            g.transform.SetParent(transform);
            g.name = text;
            g.transform.localPosition = new Vector3(0.2f, transform.childCount * -0.3f, 0);
            g.GetComponent<TextMesh>().text = text;

        }
        public void IdRemovedtoUI(string text)
        {
            Debug.Log("IdRemovedtoUI" + text);
            Destroy(transform.Find(text).gameObject);

        }

        public void IdClicked(string id)
        {
            logmsg.text = "connecting to :" + id + "...";
          //  Debug.Log("IdClicked:" + id);
#if WINDOWS_UWP
            OrpheDeviceManager.Instance.btnConnect_Click(id);
#endif
        }

    }
}
