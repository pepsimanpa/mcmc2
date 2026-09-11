
/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from side_scan_sonar_pub.idl
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

public class TowedSonarAssemblyModeStatusType :  IEquatable<TowedSonarAssemblyModeStatusType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timeStamp { get; set; }
    public byte towedSonarAssemblyMode { get; set; }
    public byte screenChangeMode { get; set; }

    public TowedSonarAssemblyModeStatusType()
    {
        timeStamp = new global::DateTime();
    }

    public TowedSonarAssemblyModeStatusType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timeStamp, byte  towedSonarAssemblyMode, byte  screenChangeMode)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timeStamp = timeStamp;
        this.towedSonarAssemblyMode = towedSonarAssemblyMode;
        this.screenChangeMode = screenChangeMode;
    }

    public TowedSonarAssemblyModeStatusType(TowedSonarAssemblyModeStatusType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timeStamp = new global::DateTime(other.timeStamp);
        this.towedSonarAssemblyMode = other.towedSonarAssemblyMode;
        this.screenChangeMode = other.screenChangeMode;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timeStamp);
        hash.Add(this.towedSonarAssemblyMode);
        hash.Add(this.screenChangeMode);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblyModeStatusType other)
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
        this.towedSonarAssemblyMode.Equals(other.towedSonarAssemblyMode) && 
        this.screenChangeMode.Equals(other.screenChangeMode);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblyModeStatusType);

    public override string ToString() => TowedSonarAssemblyModeStatusTypeSupport.Instance.ToString(this);
}

public class RemoteControlledWeaponSystemCBITReportType :  IEquatable<RemoteControlledWeaponSystemCBITReportType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timestamp { get; set; }
    public byte towedSonarAssemblyTotalCBIT { get; set; }
    public byte launchAndRecoveryTotalCBIT { get; set; }
    public byte towedSonarArrayTotalCBIT { get; set; }
    public byte sonarSignalProcessingEquipmentTotalCBIT { get; set; }
    public byte sonarImageProcessingEquipmentTotalCBIT { get; set; }

    public RemoteControlledWeaponSystemCBITReportType()
    {
        timestamp = new global::DateTime();
    }

    public RemoteControlledWeaponSystemCBITReportType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timestamp, byte  towedSonarAssemblyTotalCBIT, byte  launchAndRecoveryTotalCBIT, byte  towedSonarArrayTotalCBIT, byte  sonarSignalProcessingEquipmentTotalCBIT, byte  sonarImageProcessingEquipmentTotalCBIT)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timestamp = timestamp;
        this.towedSonarAssemblyTotalCBIT = towedSonarAssemblyTotalCBIT;
        this.launchAndRecoveryTotalCBIT = launchAndRecoveryTotalCBIT;
        this.towedSonarArrayTotalCBIT = towedSonarArrayTotalCBIT;
        this.sonarSignalProcessingEquipmentTotalCBIT = sonarSignalProcessingEquipmentTotalCBIT;
        this.sonarImageProcessingEquipmentTotalCBIT = sonarImageProcessingEquipmentTotalCBIT;
    }

    public RemoteControlledWeaponSystemCBITReportType(RemoteControlledWeaponSystemCBITReportType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timestamp = new global::DateTime(other.timestamp);
        this.towedSonarAssemblyTotalCBIT = other.towedSonarAssemblyTotalCBIT;
        this.launchAndRecoveryTotalCBIT = other.launchAndRecoveryTotalCBIT;
        this.towedSonarArrayTotalCBIT = other.towedSonarArrayTotalCBIT;
        this.sonarSignalProcessingEquipmentTotalCBIT = other.sonarSignalProcessingEquipmentTotalCBIT;
        this.sonarImageProcessingEquipmentTotalCBIT = other.sonarImageProcessingEquipmentTotalCBIT;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timestamp);
        hash.Add(this.towedSonarAssemblyTotalCBIT);
        hash.Add(this.launchAndRecoveryTotalCBIT);
        hash.Add(this.towedSonarArrayTotalCBIT);
        hash.Add(this.sonarSignalProcessingEquipmentTotalCBIT);
        hash.Add(this.sonarImageProcessingEquipmentTotalCBIT);

        return hash.ToHashCode();
    }

    public bool Equals(RemoteControlledWeaponSystemCBITReportType other)
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
        this.timestamp.Equals(other.timestamp) && 
        this.towedSonarAssemblyTotalCBIT.Equals(other.towedSonarAssemblyTotalCBIT) && 
        this.launchAndRecoveryTotalCBIT.Equals(other.launchAndRecoveryTotalCBIT) && 
        this.towedSonarArrayTotalCBIT.Equals(other.towedSonarArrayTotalCBIT) && 
        this.sonarSignalProcessingEquipmentTotalCBIT.Equals(other.sonarSignalProcessingEquipmentTotalCBIT) && 
        this.sonarImageProcessingEquipmentTotalCBIT.Equals(other.sonarImageProcessingEquipmentTotalCBIT);
    }

    public override bool Equals(object obj) => this.Equals(obj as RemoteControlledWeaponSystemCBITReportType);

    public override string ToString() => RemoteControlledWeaponSystemCBITReportTypeSupport.Instance.ToString(this);
}

public class TowedSonarAssemblySystemStatusType :  IEquatable<TowedSonarAssemblySystemStatusType>
{
    public byte platformIDKey { get; set; }
    public byte equipmentIDKey { get; set; }
    public global::DateTime timestamp { get; set; }
    public byte towedSonarArrayPowerStatus { get; set; }
    public byte ultraShortBaseLinePowerStatus { get; set; }
    public byte winchPowerStatus { get; set; }
    public byte recoveryUnitPowerStatus { get; set; }
    public byte cameraPowerStatus { get; set; }
    public byte towedSonarArrayDepthInfo { get; set; }
    public int gpsDateUtc { get; set; }
    public int gpsTimeUtc { get; set; }
    public double gpsLatitude { get; set; }
    public double gpsLongitude { get; set; }
    public float gpsVelocity { get; set; }
    public float gpsCourse { get; set; }
    public float towedSonarArrayHeading { get; set; }
    public float towedSonarArrayDepth { get; set; }
    public float towedSonarArrayAltitude { get; set; }
    public float towedSonarArrayPitch { get; set; }
    public float towedSonarArrayRoll { get; set; }
    public float towedSonarArrayTemperature { get; set; }
    public float towedSonarArrayHumidity { get; set; }
    public float towedSonarArrayVoltage { get; set; }
    public float towedSonarArrayCurrent { get; set; }
    public float recoveryUnitMode { get; set; }
    public float ultraShortBaseLineMode { get; set; }
    public float winchMotorBrakeStatus { get; set; }
    public float slideMotorBrakeStatus { get; set; }
    public float ultraShortBaseLineMotorBrakeStatus { get; set; }
    public float cableDeployLength { get; set; }
    public float cableDeploySpeed { get; set; }
    public float slidePosition { get; set; }
    public float ultraShortBaseLineLinkPosition { get; set; }
    public float ultraShortBaseLineVerticalPosition { get; set; }
    public float winchCurrent { get; set; }
    public float recoveryUnitCurrent { get; set; }
    public float ultraShortBaseLineLinkCurrent { get; set; }
    public float ultraShortBaseLineVerticalCurrent { get; set; }
    public float cableTension { get; set; }
    public float limitSwitchStatus { get; set; }
    public byte recoveryUnitEmergencyStopStatus { get; set; }

    public TowedSonarAssemblySystemStatusType()
    {
        timestamp = new global::DateTime();
    }

    public TowedSonarAssemblySystemStatusType(byte  platformIDKey, byte  equipmentIDKey, global::DateTime  timestamp, byte  towedSonarArrayPowerStatus, byte  ultraShortBaseLinePowerStatus, byte  winchPowerStatus, byte  recoveryUnitPowerStatus, byte  cameraPowerStatus, byte  towedSonarArrayDepthInfo, int  gpsDateUtc, int  gpsTimeUtc, double  gpsLatitude, double  gpsLongitude, float  gpsVelocity, float  gpsCourse, float  towedSonarArrayHeading, float  towedSonarArrayDepth, float  towedSonarArrayAltitude, float  towedSonarArrayPitch, float  towedSonarArrayRoll, float  towedSonarArrayTemperature, float  towedSonarArrayHumidity, float  towedSonarArrayVoltage, float  towedSonarArrayCurrent, float  recoveryUnitMode, float  ultraShortBaseLineMode, float  winchMotorBrakeStatus, float  slideMotorBrakeStatus, float  ultraShortBaseLineMotorBrakeStatus, float  cableDeployLength, float  cableDeploySpeed, float  slidePosition, float  ultraShortBaseLineLinkPosition, float  ultraShortBaseLineVerticalPosition, float  winchCurrent, float  recoveryUnitCurrent, float  ultraShortBaseLineLinkCurrent, float  ultraShortBaseLineVerticalCurrent, float  cableTension, float  limitSwitchStatus, byte  recoveryUnitEmergencyStopStatus)
    {
        this.platformIDKey = platformIDKey;
        this.equipmentIDKey = equipmentIDKey;
        this.timestamp = timestamp;
        this.towedSonarArrayPowerStatus = towedSonarArrayPowerStatus;
        this.ultraShortBaseLinePowerStatus = ultraShortBaseLinePowerStatus;
        this.winchPowerStatus = winchPowerStatus;
        this.recoveryUnitPowerStatus = recoveryUnitPowerStatus;
        this.cameraPowerStatus = cameraPowerStatus;
        this.towedSonarArrayDepthInfo = towedSonarArrayDepthInfo;
        this.gpsDateUtc = gpsDateUtc;
        this.gpsTimeUtc = gpsTimeUtc;
        this.gpsLatitude = gpsLatitude;
        this.gpsLongitude = gpsLongitude;
        this.gpsVelocity = gpsVelocity;
        this.gpsCourse = gpsCourse;
        this.towedSonarArrayHeading = towedSonarArrayHeading;
        this.towedSonarArrayDepth = towedSonarArrayDepth;
        this.towedSonarArrayAltitude = towedSonarArrayAltitude;
        this.towedSonarArrayPitch = towedSonarArrayPitch;
        this.towedSonarArrayRoll = towedSonarArrayRoll;
        this.towedSonarArrayTemperature = towedSonarArrayTemperature;
        this.towedSonarArrayHumidity = towedSonarArrayHumidity;
        this.towedSonarArrayVoltage = towedSonarArrayVoltage;
        this.towedSonarArrayCurrent = towedSonarArrayCurrent;
        this.recoveryUnitMode = recoveryUnitMode;
        this.ultraShortBaseLineMode = ultraShortBaseLineMode;
        this.winchMotorBrakeStatus = winchMotorBrakeStatus;
        this.slideMotorBrakeStatus = slideMotorBrakeStatus;
        this.ultraShortBaseLineMotorBrakeStatus = ultraShortBaseLineMotorBrakeStatus;
        this.cableDeployLength = cableDeployLength;
        this.cableDeploySpeed = cableDeploySpeed;
        this.slidePosition = slidePosition;
        this.ultraShortBaseLineLinkPosition = ultraShortBaseLineLinkPosition;
        this.ultraShortBaseLineVerticalPosition = ultraShortBaseLineVerticalPosition;
        this.winchCurrent = winchCurrent;
        this.recoveryUnitCurrent = recoveryUnitCurrent;
        this.ultraShortBaseLineLinkCurrent = ultraShortBaseLineLinkCurrent;
        this.ultraShortBaseLineVerticalCurrent = ultraShortBaseLineVerticalCurrent;
        this.cableTension = cableTension;
        this.limitSwitchStatus = limitSwitchStatus;
        this.recoveryUnitEmergencyStopStatus = recoveryUnitEmergencyStopStatus;
    }

    public TowedSonarAssemblySystemStatusType(TowedSonarAssemblySystemStatusType other)
    {
        if (other == null)
        {
            return;
        }

        this.platformIDKey = other.platformIDKey;
        this.equipmentIDKey = other.equipmentIDKey;
        this.timestamp = new global::DateTime(other.timestamp);
        this.towedSonarArrayPowerStatus = other.towedSonarArrayPowerStatus;
        this.ultraShortBaseLinePowerStatus = other.ultraShortBaseLinePowerStatus;
        this.winchPowerStatus = other.winchPowerStatus;
        this.recoveryUnitPowerStatus = other.recoveryUnitPowerStatus;
        this.cameraPowerStatus = other.cameraPowerStatus;
        this.towedSonarArrayDepthInfo = other.towedSonarArrayDepthInfo;
        this.gpsDateUtc = other.gpsDateUtc;
        this.gpsTimeUtc = other.gpsTimeUtc;
        this.gpsLatitude = other.gpsLatitude;
        this.gpsLongitude = other.gpsLongitude;
        this.gpsVelocity = other.gpsVelocity;
        this.gpsCourse = other.gpsCourse;
        this.towedSonarArrayHeading = other.towedSonarArrayHeading;
        this.towedSonarArrayDepth = other.towedSonarArrayDepth;
        this.towedSonarArrayAltitude = other.towedSonarArrayAltitude;
        this.towedSonarArrayPitch = other.towedSonarArrayPitch;
        this.towedSonarArrayRoll = other.towedSonarArrayRoll;
        this.towedSonarArrayTemperature = other.towedSonarArrayTemperature;
        this.towedSonarArrayHumidity = other.towedSonarArrayHumidity;
        this.towedSonarArrayVoltage = other.towedSonarArrayVoltage;
        this.towedSonarArrayCurrent = other.towedSonarArrayCurrent;
        this.recoveryUnitMode = other.recoveryUnitMode;
        this.ultraShortBaseLineMode = other.ultraShortBaseLineMode;
        this.winchMotorBrakeStatus = other.winchMotorBrakeStatus;
        this.slideMotorBrakeStatus = other.slideMotorBrakeStatus;
        this.ultraShortBaseLineMotorBrakeStatus = other.ultraShortBaseLineMotorBrakeStatus;
        this.cableDeployLength = other.cableDeployLength;
        this.cableDeploySpeed = other.cableDeploySpeed;
        this.slidePosition = other.slidePosition;
        this.ultraShortBaseLineLinkPosition = other.ultraShortBaseLineLinkPosition;
        this.ultraShortBaseLineVerticalPosition = other.ultraShortBaseLineVerticalPosition;
        this.winchCurrent = other.winchCurrent;
        this.recoveryUnitCurrent = other.recoveryUnitCurrent;
        this.ultraShortBaseLineLinkCurrent = other.ultraShortBaseLineLinkCurrent;
        this.ultraShortBaseLineVerticalCurrent = other.ultraShortBaseLineVerticalCurrent;
        this.cableTension = other.cableTension;
        this.limitSwitchStatus = other.limitSwitchStatus;
        this.recoveryUnitEmergencyStopStatus = other.recoveryUnitEmergencyStopStatus;

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.platformIDKey);
        hash.Add(this.equipmentIDKey);
        hash.Add(this.timestamp);
        hash.Add(this.towedSonarArrayPowerStatus);
        hash.Add(this.ultraShortBaseLinePowerStatus);
        hash.Add(this.winchPowerStatus);
        hash.Add(this.recoveryUnitPowerStatus);
        hash.Add(this.cameraPowerStatus);
        hash.Add(this.towedSonarArrayDepthInfo);
        hash.Add(this.gpsDateUtc);
        hash.Add(this.gpsTimeUtc);
        hash.Add(this.gpsLatitude);
        hash.Add(this.gpsLongitude);
        hash.Add(this.gpsVelocity);
        hash.Add(this.gpsCourse);
        hash.Add(this.towedSonarArrayHeading);
        hash.Add(this.towedSonarArrayDepth);
        hash.Add(this.towedSonarArrayAltitude);
        hash.Add(this.towedSonarArrayPitch);
        hash.Add(this.towedSonarArrayRoll);
        hash.Add(this.towedSonarArrayTemperature);
        hash.Add(this.towedSonarArrayHumidity);
        hash.Add(this.towedSonarArrayVoltage);
        hash.Add(this.towedSonarArrayCurrent);
        hash.Add(this.recoveryUnitMode);
        hash.Add(this.ultraShortBaseLineMode);
        hash.Add(this.winchMotorBrakeStatus);
        hash.Add(this.slideMotorBrakeStatus);
        hash.Add(this.ultraShortBaseLineMotorBrakeStatus);
        hash.Add(this.cableDeployLength);
        hash.Add(this.cableDeploySpeed);
        hash.Add(this.slidePosition);
        hash.Add(this.ultraShortBaseLineLinkPosition);
        hash.Add(this.ultraShortBaseLineVerticalPosition);
        hash.Add(this.winchCurrent);
        hash.Add(this.recoveryUnitCurrent);
        hash.Add(this.ultraShortBaseLineLinkCurrent);
        hash.Add(this.ultraShortBaseLineVerticalCurrent);
        hash.Add(this.cableTension);
        hash.Add(this.limitSwitchStatus);
        hash.Add(this.recoveryUnitEmergencyStopStatus);

        return hash.ToHashCode();
    }

    public bool Equals(TowedSonarAssemblySystemStatusType other)
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
        this.timestamp.Equals(other.timestamp) && 
        this.towedSonarArrayPowerStatus.Equals(other.towedSonarArrayPowerStatus) && 
        this.ultraShortBaseLinePowerStatus.Equals(other.ultraShortBaseLinePowerStatus) && 
        this.winchPowerStatus.Equals(other.winchPowerStatus) && 
        this.recoveryUnitPowerStatus.Equals(other.recoveryUnitPowerStatus) && 
        this.cameraPowerStatus.Equals(other.cameraPowerStatus) && 
        this.towedSonarArrayDepthInfo.Equals(other.towedSonarArrayDepthInfo) && 
        this.gpsDateUtc.Equals(other.gpsDateUtc) && 
        this.gpsTimeUtc.Equals(other.gpsTimeUtc) && 
        this.gpsLatitude.Equals(other.gpsLatitude) && 
        this.gpsLongitude.Equals(other.gpsLongitude) && 
        this.gpsVelocity.Equals(other.gpsVelocity) && 
        this.gpsCourse.Equals(other.gpsCourse) && 
        this.towedSonarArrayHeading.Equals(other.towedSonarArrayHeading) && 
        this.towedSonarArrayDepth.Equals(other.towedSonarArrayDepth) && 
        this.towedSonarArrayAltitude.Equals(other.towedSonarArrayAltitude) && 
        this.towedSonarArrayPitch.Equals(other.towedSonarArrayPitch) && 
        this.towedSonarArrayRoll.Equals(other.towedSonarArrayRoll) && 
        this.towedSonarArrayTemperature.Equals(other.towedSonarArrayTemperature) && 
        this.towedSonarArrayHumidity.Equals(other.towedSonarArrayHumidity) && 
        this.towedSonarArrayVoltage.Equals(other.towedSonarArrayVoltage) && 
        this.towedSonarArrayCurrent.Equals(other.towedSonarArrayCurrent) && 
        this.recoveryUnitMode.Equals(other.recoveryUnitMode) && 
        this.ultraShortBaseLineMode.Equals(other.ultraShortBaseLineMode) && 
        this.winchMotorBrakeStatus.Equals(other.winchMotorBrakeStatus) && 
        this.slideMotorBrakeStatus.Equals(other.slideMotorBrakeStatus) && 
        this.ultraShortBaseLineMotorBrakeStatus.Equals(other.ultraShortBaseLineMotorBrakeStatus) && 
        this.cableDeployLength.Equals(other.cableDeployLength) && 
        this.cableDeploySpeed.Equals(other.cableDeploySpeed) && 
        this.slidePosition.Equals(other.slidePosition) && 
        this.ultraShortBaseLineLinkPosition.Equals(other.ultraShortBaseLineLinkPosition) && 
        this.ultraShortBaseLineVerticalPosition.Equals(other.ultraShortBaseLineVerticalPosition) && 
        this.winchCurrent.Equals(other.winchCurrent) && 
        this.recoveryUnitCurrent.Equals(other.recoveryUnitCurrent) && 
        this.ultraShortBaseLineLinkCurrent.Equals(other.ultraShortBaseLineLinkCurrent) && 
        this.ultraShortBaseLineVerticalCurrent.Equals(other.ultraShortBaseLineVerticalCurrent) && 
        this.cableTension.Equals(other.cableTension) && 
        this.limitSwitchStatus.Equals(other.limitSwitchStatus) && 
        this.recoveryUnitEmergencyStopStatus.Equals(other.recoveryUnitEmergencyStopStatus);
    }

    public override bool Equals(object obj) => this.Equals(obj as TowedSonarAssemblySystemStatusType);

    public override string ToString() => TowedSonarAssemblySystemStatusTypeSupport.Instance.ToString(this);
}

public class TsaCompressedMergeSignalDataType :  IEquatable<TsaCompressedMergeSignalDataType>
{
    public int pingNumber { get; set; }
    public short range { get; set; }
    public byte gain { get; set; }
    public byte tvg { get; set; }
    public byte pulseWidth { get; set; }
    public byte frequency { get; set; }
    public byte pulseType { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public float gpsVelocity { get; set; }
    public float gpsCourse { get; set; }
    public float towCourse { get; set; }
    public float towDepth { get; set; }
    public float towAltitude { get; set; }
    public float towPitch { get; set; }
    public float towRoll { get; set; }
    public float towTemp { get; set; }
    public float towHumidity { get; set; }
    public float towVoltage { get; set; }
    public float towCurrent { get; set; }
    public short dataSideScanSonarCount { get; set; }
    public short dataGapFillerCount { get; set; }
    public short beamCount { get; set; }
    public short channelCount { get; set; }
    [Bound(40000)]
    public ISequence<short> data { get; }

    public TsaCompressedMergeSignalDataType()
    {
        data = new Rti.Types.Sequence<short>();
    }

    public TsaCompressedMergeSignalDataType(int  pingNumber, short  range, byte  gain, byte  tvg, byte  pulseWidth, byte  frequency, byte  pulseType, double  latitude, double  longitude, float  gpsVelocity, float  gpsCourse, float  towCourse, float  towDepth, float  towAltitude, float  towPitch, float  towRoll, float  towTemp, float  towHumidity, float  towVoltage, float  towCurrent, short  dataSideScanSonarCount, short  dataGapFillerCount, short  beamCount, short  channelCount, ISequence<short>data)
    {
        this.pingNumber = pingNumber;
        this.range = range;
        this.gain = gain;
        this.tvg = tvg;
        this.pulseWidth = pulseWidth;
        this.frequency = frequency;
        this.pulseType = pulseType;
        this.latitude = latitude;
        this.longitude = longitude;
        this.gpsVelocity = gpsVelocity;
        this.gpsCourse = gpsCourse;
        this.towCourse = towCourse;
        this.towDepth = towDepth;
        this.towAltitude = towAltitude;
        this.towPitch = towPitch;
        this.towRoll = towRoll;
        this.towTemp = towTemp;
        this.towHumidity = towHumidity;
        this.towVoltage = towVoltage;
        this.towCurrent = towCurrent;
        this.dataSideScanSonarCount = dataSideScanSonarCount;
        this.dataGapFillerCount = dataGapFillerCount;
        this.beamCount = beamCount;
        this.channelCount = channelCount;
        this.data = data;
    }

    public TsaCompressedMergeSignalDataType(TsaCompressedMergeSignalDataType other)
    {
        if (other == null)
        {
            return;
        }

        this.pingNumber = other.pingNumber;
        this.range = other.range;
        this.gain = other.gain;
        this.tvg = other.tvg;
        this.pulseWidth = other.pulseWidth;
        this.frequency = other.frequency;
        this.pulseType = other.pulseType;
        this.latitude = other.latitude;
        this.longitude = other.longitude;
        this.gpsVelocity = other.gpsVelocity;
        this.gpsCourse = other.gpsCourse;
        this.towCourse = other.towCourse;
        this.towDepth = other.towDepth;
        this.towAltitude = other.towAltitude;
        this.towPitch = other.towPitch;
        this.towRoll = other.towRoll;
        this.towTemp = other.towTemp;
        this.towHumidity = other.towHumidity;
        this.towVoltage = other.towVoltage;
        this.towCurrent = other.towCurrent;
        this.dataSideScanSonarCount = other.dataSideScanSonarCount;
        this.dataGapFillerCount = other.dataGapFillerCount;
        this.beamCount = other.beamCount;
        this.channelCount = other.channelCount;
        this.data = new Rti.Types.Sequence<short>(other.data);

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.pingNumber);
        hash.Add(this.range);
        hash.Add(this.gain);
        hash.Add(this.tvg);
        hash.Add(this.pulseWidth);
        hash.Add(this.frequency);
        hash.Add(this.pulseType);
        hash.Add(this.latitude);
        hash.Add(this.longitude);
        hash.Add(this.gpsVelocity);
        hash.Add(this.gpsCourse);
        hash.Add(this.towCourse);
        hash.Add(this.towDepth);
        hash.Add(this.towAltitude);
        hash.Add(this.towPitch);
        hash.Add(this.towRoll);
        hash.Add(this.towTemp);
        hash.Add(this.towHumidity);
        hash.Add(this.towVoltage);
        hash.Add(this.towCurrent);
        hash.Add(this.dataSideScanSonarCount);
        hash.Add(this.dataGapFillerCount);
        hash.Add(this.beamCount);
        hash.Add(this.channelCount);
        hash.Add(this.data.Count);

        return hash.ToHashCode();
    }

    public bool Equals(TsaCompressedMergeSignalDataType other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.pingNumber.Equals(other.pingNumber) && 
        this.range.Equals(other.range) && 
        this.gain.Equals(other.gain) && 
        this.tvg.Equals(other.tvg) && 
        this.pulseWidth.Equals(other.pulseWidth) && 
        this.frequency.Equals(other.frequency) && 
        this.pulseType.Equals(other.pulseType) && 
        this.latitude.Equals(other.latitude) && 
        this.longitude.Equals(other.longitude) && 
        this.gpsVelocity.Equals(other.gpsVelocity) && 
        this.gpsCourse.Equals(other.gpsCourse) && 
        this.towCourse.Equals(other.towCourse) && 
        this.towDepth.Equals(other.towDepth) && 
        this.towAltitude.Equals(other.towAltitude) && 
        this.towPitch.Equals(other.towPitch) && 
        this.towRoll.Equals(other.towRoll) && 
        this.towTemp.Equals(other.towTemp) && 
        this.towHumidity.Equals(other.towHumidity) && 
        this.towVoltage.Equals(other.towVoltage) && 
        this.towCurrent.Equals(other.towCurrent) && 
        this.dataSideScanSonarCount.Equals(other.dataSideScanSonarCount) && 
        this.dataGapFillerCount.Equals(other.dataGapFillerCount) && 
        this.beamCount.Equals(other.beamCount) && 
        this.channelCount.Equals(other.channelCount) && 
        this.data.SequenceEqual(other.data);
    }

    public override bool Equals(object obj) => this.Equals(obj as TsaCompressedMergeSignalDataType);

    public override string ToString() => TsaCompressedMergeSignalDataTypeSupport.Instance.ToString(this);
}

