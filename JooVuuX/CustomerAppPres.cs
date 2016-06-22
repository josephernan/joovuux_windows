using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JooVuuX
{
    class CustomerAppPres
    {
        public static int CustomerPresDateFormat = 0;    //combobox14
        public static int CustomerPresBeepNoise = 1;	//button15
        public static int CustomerPresRecordLED = 2;		//button16
        public static int CustomerPresDefaultMode = 3;		//combobox11
        public static int CustomerPresPowerOnRecord = 4;	//combobox10
        public static int CustomerPresStandbyTime = 5;		//combobox13
        public static int CustomerPresPowerOffDisconnect = 6;	//combobox12
        public static int CustomerPresMotionDet = 7;		//button17
        public static int CustomerPresMotionDetSenti = 8;	//combobox15
        public static int CustomerPresTVType = 9;			//combobox18
        public static int CustomerPresGsensor = 10;			//button18
        public static int CustomerPresGsensorSenti = 11;
        public static int CustomerPresCarNumberStamp = 12;		//button19
        public static int CustomerPresCarSpeedStamp = 13;	//button20
        public static int CustomerPresCarSpeedUnit = 14;
        public static int CustomerPresTimeMode = 15;		//button22
        public static int CustomerPresTimeModeStart = 16;		//
        public static int CustomerPresTimeModeEnd = 17;		//

        public static int CustomerPresResolutionType = 18;	//combobox24
        public static int CustomerPresAudio = 19;			//button31
        public static int CustomerPresTimeStamp = 20;		//button33
        public static int CustomerPresAutoRotation = 21;		//button30
        public static int CustomerPresLoopRecord = 22;		//button29
        public static int CustomerPresVideoLength = 23;		//combobox22
        public static int CustomerPresVideoBitRate = 24;		//combobox21
        public static int CustomerPresWDR = 25;			//button27
        public static int CustomerPresFieldOfView = 26;		//combobox19
        public static int CustomerPresTimeLapseVideo = 27;     //combobox17

        public static int CustomerPresResolutionTypeMode2 = 28;		//combobox16
        public static int CustomerPresAudioMode2 = 29;			//button35
        public static int CustomerPresTimeStampMode2 = 30;		//button25
        public static int CustomerPresAutoRotationMode2 = 31;	//button34
        public static int CustomerPresLoopRecordMode2 = 32;		//button28
        public static int CustomerPresVideoLengthMode2 = 33;		//combobox26
        public static int CustomerPresVideoBitRateMode2 = 34;		//combobox25
        public static int CustomerPresWDRMode2 = 35;			//button26
        public static int CustomerPresFieldOfViewMode2 = 36;		//combobox23
        public static int CustomerPresTimeLapseVideoMode2 = 37;        //combobox20

        public static int CustomerPresWhiteBalance = 38;                //white balance
        public static int CustomerPresSharpness = 39;                    //sharpness
        public static int CustomerPresContrast = 40;                    //contrast
        public static int CustomerPresExposure = 41;                    //exposure

        public static int CustomerPresMaxMember = 43;

        public byte[] customer_press = new byte[CustomerPresMaxMember];
        
        public byte[] wifiPassword = new byte[11] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', 
            (byte)'6', (byte)'7', (byte)'8', (byte)'9', 0 };
        public byte[] carNumber = new byte[11] { (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0', 
            (byte)'0', (byte)'0', (byte)'0', (byte)'0', 0 };
        public byte[] cameraDateBytes = new byte[8] { (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0',
            (byte)'0', (byte)'0'};
        public byte[] cameraTimeBytes = new byte[6] { (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0'};
    }
}
