using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HSCF.Communication.Scs.Communication.EndPoints.Tcp;
using HSCF.Communication.ScsServices.Client;
using HomeSeerAPI;
using System.Diagnostics;
using HSCF.Communication.ScsServices.Service;

using HSCF;
using System.Collections.Specialized;
using System.Collections.Generic;

static string ROOT_ZWAVE_DEVICE = "Z-Wave Switch Root Device";

static int MANUFACTURER_INNOVELLI = 798;

// hs_connect 192.168.0.151:10400

public static void Main(string[] args)
{
    Scheduler.Classes.clsDeviceEnumeration deviceEnumerator
        = (Scheduler.Classes.clsDeviceEnumeration)hs.GetDeviceEnumerator();

    while (!deviceEnumerator.Finished)
    {
        Scheduler.Classes.DeviceClass device = deviceEnumerator.GetNext();                    

        Debug.WriteLine(device.get_Name(hs));

        if (device.get_Relationship(hs) == Enums.eRelationship.Child)
        {
            Scheduler.Classes.DeviceClass parent = (Scheduler.Classes.DeviceClass)
                hs.GetDeviceByRef(hs.GetDeviceParentRefByRef(device.get_Ref(hs)));

            string parentDeviceType = parent.get_Device_Type_String(hs);

            PlugExtraData.clsPlugExtraData ped = device.get_PlugExtraData_Get(hs);
            
            object parentManufacturerId = ped.GetNamedKeys() != null ? ped.GetNamed("manufacturer_id") : null;

            if (parentDeviceType == ROOT_ZWAVE_DEVICE && parentManufacturerId != null && (int)parentManufacturerId == MANUFACTURER_INNOVELLI)
            {
                string deviceTypeString = device.get_Device_Type_String(hs);
                if (deviceTypeString.StartsWith("Z-Wave"))
                {
                    deviceTypeString = deviceTypeString.Substring("Z-Wave ".Length);
                }
                string parentName = parent.get_Name(hs);

                Debug.WriteLine(deviceTypeString + ":" + device.get_Name(hs) + "<-" + parent.get_Device_Type_String(hs) + ":" + parentName);
                device.set_Name(hs, parentName + " " + deviceTypeString);
            }
        }
        else
        {
            string deviceType = device.get_Device_Type_String(hs);

            if (deviceType == "Z-Wave Switch Root Device")
            {
                string deviceName = device.get_Name(hs);
                if (deviceName.EndsWith(" Status"))
                {
                    deviceName = deviceName.Substring(0, deviceName.Length - " Status".Length);
                    device.set_Name(hs, deviceName);
                }
            }
        }
    }

    Debug.WriteLine("Done!");
}


