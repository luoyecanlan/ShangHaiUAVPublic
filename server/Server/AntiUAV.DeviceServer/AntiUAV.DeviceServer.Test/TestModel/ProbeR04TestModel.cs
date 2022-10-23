using AntiUAV.DevicePlugin.ProbeR04.Cmd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.DeviceServer.Test.TestModel
{
    public class ProbeR04TestModel : ITestModel
    {
        public byte[] 航迹数据通信协议()
        {
            R_ProbeR04_Track track = new R_ProbeR04_Track
            {
                CheckCode = 124,
                Head = 3428144588,
                Length = 120,
                RadarId = 29,
                Revs = 1,
                Revs2 = 1,
                SendModel = '1',
                StartFlag = 1,
                TargetAngle = 23000,
                TargetDistance = 1000,
                TargetElevation = 4000,
                TargetHeight = 1200,
                TargetLat = 120,
                TargetLng = 120,
                TargetNO = 210,
                TargetNorthPoint = 12000,
                TargetPointCount = 11000,
                TargetSailSpeed = 20,
                TargetSpeed = 500,
                TimeSpan = (ulong)DateTime.UtcNow.ToUTCLong()
            };
            return track.ToBytes();
        }

        public byte[] 过界数据通信协议()
        {
            R_ProbeR04_Cross cross = new R_ProbeR04_Cross
            {
                CheckCode = 125,
                Head = 1439485013,
                Lenght = 64,
                RadarCorner = 35000,
                RadarId = 29,
                Resv = new byte[42],
                TimeSpan = (ulong)DateTime.UtcNow.ToUTCLong()
            };
            return cross.ToBytes();
        }

        public byte[] 雷达状态数据通信协议()
        {
            //激励开关
            BitArray arr = new BitArray(new bool[] { 
                true,true,true,true,
                true,true,true,true
            });
            byte[] bytes = new byte[1];
            arr.CopyTo(bytes, 0);
            R_ProbeR04_Status status = new R_ProbeR04_Status
            {
                AngleCorrection = 1800,
                CheckCode = 12323,
                ClutterStatus = true,
                CompassPitch = 18000,
                CompassPosition = 10000,
                CompassTilt = -18000,
                CPI = 3,
                Duration = 65535,
                Factor = 80,
                FllowTargetId = 29,
                Head = 1440603477,
                Length = 64,
                LoseNumber = 0,
                ModulationData = 10,
                Params = new byte[2] { bytes[0], 0x00 },
                Params2 = new byte[4] { 0x00, 0x00, 0x00, 0x01 },
                RadarHeight = 2000,
                RadarId = 20,
                RadarLat = 120,
                RadarLng = 120,
                RadarType = 1,
                Resv2 = new byte[31],
                Threshold = 345,
                TimeSpan = (ulong)DateTime.UtcNow.ToUTCLong(),
                TurntableOrientation = 36000,
                TurntablePitch = 18000,
                TurretData = 20
            };
            return status.ToBytes();
        }
    }
}
