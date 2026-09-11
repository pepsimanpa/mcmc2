
/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from side_scan_sonar_sub.idl
using RTI Code Generator (rtiddsgen) version 4.3.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using System;
using System.Reflection;
using System.Collections.Generic;
using Rti.Types;
using System.Linq;
using Omg.Types;

public class TowedSonarAssemblyStatusConfigType :  IEquatable<TowedSonarAssemblyStatusConfigType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public short range { get; set; }
    public byte gain { get; set; }
    public byte timeVariedGain { get; set; }
    public byte pulseWidth { get; set; }
    public byte frequency { get; set; }
    public byte pulseType { get; set; }
    public byte lineBalanceOffserAmplifier { get; set; }
    public byte lineAmplifier { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyStatusConfigType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyStatusConfigType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, short  range, byte  gain, byte  timeVariedGain, byte  pulseWidth, byte  frequency, byte  pulseType, byte  lineBalanceOffserAmplifier, byte  lineAmplifier, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.range = range;
        this.gain = gain;
        this.timeVariedGain = timeVariedGain;
        this.pulseWidth = pulseWidth;
        this.frequency = frequency;
        this.pulseType = pulseType;
        this.lineBalanceOffserAmplifier = lineBalanceOffserAmplifier;
        this.lineAmplifier = lineAmplifier;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyStatusConfigType(TowedSonarAssemblyStatusConfigType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.range = other.range;
        this.gain = other.gain;
        this.timeVariedGain = other.timeVariedGain;
        this.pulseWidth = other.pulseWidth;
        this.frequency = other.frequency;
        this.pulseType = other.pulseType;
        this.lineBalanceOffserAmplifier = other.lineBalanceOffserAmplifier;
        this.lineAmplifier = other.lineAmplifier;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.range);
        hash.Add(this.gain);
        hash.Add(this.timeVariedGain);
        hash.Add(this.pulseWidth);
        hash.Add(this.frequency);
        hash.Add(this.pulseType);
        hash.Add(this.lineBalanceOffserAmplifier);
        hash.Add(this.lineAmplifier);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyStatusConfigType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.range.Equals(other.range) && 
        this.gain.Equals(other.gain) && 
        this.timeVariedGain.Equals(other.timeVariedGain) && 
        this.pulseWidth.Equals(other.pulseWidth) && 
        this.frequency.Equals(other.frequency) && 
        this.pulseType.Equals(other.pulseType) && 
        this.lineBalanceOffserAmplifier.Equals(other.lineBalanceOffserAmplifier) && 
        this.lineAmplifier.Equals(other.lineAmplifier) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyStatusConfigType);

    public override string ToString() => TowedSonarAssemblyStatusConfigTypeSupport.Instance.ToString(this);
}

public class TowedSonarArrayStartControlType :  IEquatable<TowedSonarArrayStartControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte start { get; set; }
    public short commandID { get; set; }

    public TowedSonarArrayStartControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarArrayStartControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  start, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.start = start;
        this.commandID = commandID;
    }

    public TowedSonarArrayStartControlType(TowedSonarArrayStartControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.start = other.start;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.start);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarArrayStartControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.start.Equals(other.start) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarArrayStartControlType);

    public override string ToString() => TowedSonarArrayStartControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyPlatformPowerControlType :  IEquatable<TowedSonarAssemblyPlatformPowerControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte targetDevice { get; set; }
    public byte powerOn { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyPlatformPowerControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyPlatformPowerControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  targetDevice, byte  powerOn, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.targetDevice = targetDevice;
        this.powerOn = powerOn;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyPlatformPowerControlType(TowedSonarAssemblyPlatformPowerControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.targetDevice = other.targetDevice;
        this.powerOn = other.powerOn;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.targetDevice);
        hash.Add(this.powerOn);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyPlatformPowerControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.targetDevice.Equals(other.targetDevice) && 
        this.powerOn.Equals(other.powerOn) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyPlatformPowerControlType);

    public override string ToString() => TowedSonarAssemblyPlatformPowerControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyModeControlType :  IEquatable<TowedSonarAssemblyModeControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte mode { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyModeControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyModeControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  mode, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.mode = mode;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyModeControlType(TowedSonarAssemblyModeControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.mode = other.mode;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.mode);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyModeControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.mode.Equals(other.mode) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyModeControlType);

    public override string ToString() => TowedSonarAssemblyModeControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyAutoLaunchControlType :  IEquatable<TowedSonarAssemblyAutoLaunchControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte launch { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyAutoLaunchControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyAutoLaunchControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  launch, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.launch = launch;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyAutoLaunchControlType(TowedSonarAssemblyAutoLaunchControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.launch = other.launch;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.launch);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyAutoLaunchControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.launch.Equals(other.launch) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyAutoLaunchControlType);

    public override string ToString() => TowedSonarAssemblyAutoLaunchControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyRestartControlType :  IEquatable<TowedSonarAssemblyRestartControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte restart { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyRestartControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyRestartControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  restart, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.restart = restart;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyRestartControlType(TowedSonarAssemblyRestartControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.restart = other.restart;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.restart);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyRestartControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.restart.Equals(other.restart) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyRestartControlType);

    public override string ToString() => TowedSonarAssemblyRestartControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyEmergencyStopControlType :  IEquatable<TowedSonarAssemblyEmergencyStopControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte emergencyStop { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyEmergencyStopControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyEmergencyStopControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  emergencyStop, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.emergencyStop = emergencyStop;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyEmergencyStopControlType(TowedSonarAssemblyEmergencyStopControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.emergencyStop = other.emergencyStop;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.emergencyStop);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyEmergencyStopControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.emergencyStop.Equals(other.emergencyStop) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyEmergencyStopControlType);

    public override string ToString() => TowedSonarAssemblyEmergencyStopControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyCableLengthControlType :  IEquatable<TowedSonarAssemblyCableLengthControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public float lengthCommandWinch { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyCableLengthControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyCableLengthControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, float  lengthCommandWinch, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.lengthCommandWinch = lengthCommandWinch;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyCableLengthControlType(TowedSonarAssemblyCableLengthControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.lengthCommandWinch = other.lengthCommandWinch;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.lengthCommandWinch);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyCableLengthControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.lengthCommandWinch.Equals(other.lengthCommandWinch) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyCableLengthControlType);

    public override string ToString() => TowedSonarAssemblyCableLengthControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyTargetLengthConfigType :  IEquatable<TowedSonarAssemblyTargetLengthConfigType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public float targetDepth { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyTargetLengthConfigType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyTargetLengthConfigType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, float  targetDepth, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.targetDepth = targetDepth;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyTargetLengthConfigType(TowedSonarAssemblyTargetLengthConfigType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.targetDepth = other.targetDepth;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.targetDepth);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyTargetLengthConfigType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.targetDepth.Equals(other.targetDepth) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyTargetLengthConfigType);

    public override string ToString() => TowedSonarAssemblyTargetLengthConfigTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyScreanChangeConfigType :  IEquatable<TowedSonarAssemblyScreanChangeConfigType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte screenChangeMode { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyScreanChangeConfigType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyScreanChangeConfigType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  screenChangeMode, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.screenChangeMode = screenChangeMode;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyScreanChangeConfigType(TowedSonarAssemblyScreanChangeConfigType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.screenChangeMode = other.screenChangeMode;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.screenChangeMode);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyScreanChangeConfigType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.screenChangeMode.Equals(other.screenChangeMode) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyScreanChangeConfigType);

    public override string ToString() => TowedSonarAssemblyScreanChangeConfigTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyManualUltraShortBaseLineMotorControlType :  IEquatable<TowedSonarAssemblyManualUltraShortBaseLineMotorControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte launch { get; set; }
    public float motorSpeed { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyManualUltraShortBaseLineMotorControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyManualUltraShortBaseLineMotorControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  launch, float  motorSpeed, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.launch = launch;
        this.motorSpeed = motorSpeed;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyManualUltraShortBaseLineMotorControlType(TowedSonarAssemblyManualUltraShortBaseLineMotorControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.launch = other.launch;
        this.motorSpeed = other.motorSpeed;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.launch);
        hash.Add(this.motorSpeed);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyManualUltraShortBaseLineMotorControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.launch.Equals(other.launch) && 
        this.motorSpeed.Equals(other.motorSpeed) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyManualUltraShortBaseLineMotorControlType);

    public override string ToString() => TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyUltraShortBaseLineStartControlType :  IEquatable<TowedSonarAssemblyUltraShortBaseLineStartControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte start { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyUltraShortBaseLineStartControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyUltraShortBaseLineStartControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  start, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.start = start;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyUltraShortBaseLineStartControlType(TowedSonarAssemblyUltraShortBaseLineStartControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.start = other.start;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.start);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyUltraShortBaseLineStartControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.start.Equals(other.start) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyUltraShortBaseLineStartControlType);

    public override string ToString() => TowedSonarAssemblyUltraShortBaseLineStartControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyLaunchAndRecoverySlideStartControlType :  IEquatable<TowedSonarAssemblyLaunchAndRecoverySlideStartControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte start { get; set; }
    public float slideDeploy { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyLaunchAndRecoverySlideStartControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyLaunchAndRecoverySlideStartControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  start, float  slideDeploy, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.start = start;
        this.slideDeploy = slideDeploy;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyLaunchAndRecoverySlideStartControlType(TowedSonarAssemblyLaunchAndRecoverySlideStartControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.start = other.start;
        this.slideDeploy = other.slideDeploy;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.start);
        hash.Add(this.slideDeploy);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyLaunchAndRecoverySlideStartControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.start.Equals(other.start) && 
        this.slideDeploy.Equals(other.slideDeploy) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyLaunchAndRecoverySlideStartControlType);

    public override string ToString() => TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType :  IEquatable<TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte start { get; set; }
    public float winchDeploy { get; set; }
    public short commandID { get; set; }

    public TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  start, float  winchDeploy, short  commandID)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.start = start;
        this.winchDeploy = winchDeploy;
        this.commandID = commandID;
    }

    public TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType(TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.start = other.start;
        this.winchDeploy = other.winchDeploy;
        this.commandID = other.commandID;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.start);
        hash.Add(this.winchDeploy);
        hash.Add(this.commandID);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.platformIDKey.Equals(other.platformIDKey) && 
        this.equipmentIDKey.Equals(other.equipmentIDKey) && 
        this.timeStamp.Equals(other.timeStamp) && 
        this.start.Equals(other.start) && 
        this.winchDeploy.Equals(other.winchDeploy) && 
        this.commandID.Equals(other.commandID);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType);

    public override string ToString() => TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeSupport.Instance.ToString(this);
}

