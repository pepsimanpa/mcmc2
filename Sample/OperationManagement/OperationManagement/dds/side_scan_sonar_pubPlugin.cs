/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from side_scan_sonar_pub.idl
using RTI Code Generator (rtiddsgen) version 4.3.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using System;
using System.Runtime.InteropServices;
using Omg.Types;
using Omg.Types.Dynamic;
using Rti.Types;
using Rti.Dds.Core;
using Rti.Types.Dynamic;
using Rti.Dds.NativeInterface.TypePlugin;

namespace Implementation
{

    public struct TowedSonarAssemblyModeStatusTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyModeStatusType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte towedSonarAssemblyMode;
        private byte screenChangeMode;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyModeStatusType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.towedSonarAssemblyMode = towedSonarAssemblyMode;
            sample.screenChangeMode = screenChangeMode;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            towedSonarAssemblyMode = (byte) (0);
            screenChangeMode = (byte) (0);
        }

        public void ToNative(global::TowedSonarAssemblyModeStatusType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            towedSonarAssemblyMode = sample.towedSonarAssemblyMode;
            screenChangeMode = sample.screenChangeMode;
        }
    }

    internal class TowedSonarAssemblyModeStatusTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyModeStatusType, TowedSonarAssemblyModeStatusTypeUnmanaged>
    {

        internal TowedSonarAssemblyModeStatusTypePlugin() : base("global::TowedSonarAssemblyModeStatusType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyModeStatusType struct
            var TowedSonarAssemblyModeStatusTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("towedSonarAssemblyMode", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("screenChangeMode", dtf.GetPrimitiveType<byte>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyModeStatusTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyModeStatusType")
                .AddMembers(TowedSonarAssemblyModeStatusTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyModeStatusTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyModeStatusType>
{
    public TowedSonarAssemblyModeStatusTypeSupport() : base(
        new Implementation.TowedSonarAssemblyModeStatusTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyModeStatusTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyModeStatusTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyModeStatusTypeSupport, global::TowedSonarAssemblyModeStatusType>();

}

namespace Implementation
{

    public struct RemoteControlledWeaponSystemCBITReportTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::RemoteControlledWeaponSystemCBITReportType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timestamp;
        private byte towedSonarAssemblyTotalCBIT;
        private byte launchAndRecoveryTotalCBIT;
        private byte towedSonarArrayTotalCBIT;
        private byte sonarSignalProcessingEquipmentTotalCBIT;
        private byte sonarImageProcessingEquipmentTotalCBIT;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timestamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::RemoteControlledWeaponSystemCBITReportType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timestamp.FromNative(sample.timestamp, keysOnly: false);
            sample.towedSonarAssemblyTotalCBIT = towedSonarAssemblyTotalCBIT;
            sample.launchAndRecoveryTotalCBIT = launchAndRecoveryTotalCBIT;
            sample.towedSonarArrayTotalCBIT = towedSonarArrayTotalCBIT;
            sample.sonarSignalProcessingEquipmentTotalCBIT = sonarSignalProcessingEquipmentTotalCBIT;
            sample.sonarImageProcessingEquipmentTotalCBIT = sonarImageProcessingEquipmentTotalCBIT;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timestamp.Initialize(allocatePointers, allocateMemory);
            towedSonarAssemblyTotalCBIT = (byte) (0);
            launchAndRecoveryTotalCBIT = (byte) (0);
            towedSonarArrayTotalCBIT = (byte) (0);
            sonarSignalProcessingEquipmentTotalCBIT = (byte) (0);
            sonarImageProcessingEquipmentTotalCBIT = (byte) (0);
        }

        public void ToNative(global::RemoteControlledWeaponSystemCBITReportType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timestamp.ToNative(sample.timestamp, keysOnly: false);
            towedSonarAssemblyTotalCBIT = sample.towedSonarAssemblyTotalCBIT;
            launchAndRecoveryTotalCBIT = sample.launchAndRecoveryTotalCBIT;
            towedSonarArrayTotalCBIT = sample.towedSonarArrayTotalCBIT;
            sonarSignalProcessingEquipmentTotalCBIT = sample.sonarSignalProcessingEquipmentTotalCBIT;
            sonarImageProcessingEquipmentTotalCBIT = sample.sonarImageProcessingEquipmentTotalCBIT;
        }
    }

    internal class RemoteControlledWeaponSystemCBITReportTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::RemoteControlledWeaponSystemCBITReportType, RemoteControlledWeaponSystemCBITReportTypeUnmanaged>
    {

        internal RemoteControlledWeaponSystemCBITReportTypePlugin() : base("global::RemoteControlledWeaponSystemCBITReportType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // RemoteControlledWeaponSystemCBITReportType struct
            var RemoteControlledWeaponSystemCBITReportTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timestamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("towedSonarAssemblyTotalCBIT", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("launchAndRecoveryTotalCBIT", dtf.GetPrimitiveType<byte>(), id: 4),
                new StructMember("towedSonarArrayTotalCBIT", dtf.GetPrimitiveType<byte>(), id: 5),
                new StructMember("sonarSignalProcessingEquipmentTotalCBIT", dtf.GetPrimitiveType<byte>(), id: 6),
                new StructMember("sonarImageProcessingEquipmentTotalCBIT", dtf.GetPrimitiveType<byte>(), id: 7)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<RemoteControlledWeaponSystemCBITReportTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("RemoteControlledWeaponSystemCBITReportType")
                .AddMembers(RemoteControlledWeaponSystemCBITReportTypeStructMembers));

            return result;
        }
    }
}
public class RemoteControlledWeaponSystemCBITReportTypeSupport : Rti.Dds.Topics.TypeSupport<global::RemoteControlledWeaponSystemCBITReportType>
{
    public RemoteControlledWeaponSystemCBITReportTypeSupport() : base(
        new Implementation.RemoteControlledWeaponSystemCBITReportTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.RemoteControlledWeaponSystemCBITReportTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static RemoteControlledWeaponSystemCBITReportTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<RemoteControlledWeaponSystemCBITReportTypeSupport, global::RemoteControlledWeaponSystemCBITReportType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblySystemStatusTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblySystemStatusType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timestamp;
        private byte towedSonarArrayPowerStatus;
        private byte ultraShortBaseLinePowerStatus;
        private byte winchPowerStatus;
        private byte recoveryUnitPowerStatus;
        private byte cameraPowerStatus;
        private byte towedSonarArrayDepthInfo;
        private int gpsDateUtc;
        private int gpsTimeUtc;
        private double gpsLatitude;
        private double gpsLongitude;
        private float gpsVelocity;
        private float gpsCourse;
        private float towedSonarArrayHeading;
        private float towedSonarArrayDepth;
        private float towedSonarArrayAltitude;
        private float towedSonarArrayPitch;
        private float towedSonarArrayRoll;
        private float towedSonarArrayTemperature;
        private float towedSonarArrayHumidity;
        private float towedSonarArrayVoltage;
        private float towedSonarArrayCurrent;
        private float recoveryUnitMode;
        private float ultraShortBaseLineMode;
        private float winchMotorBrakeStatus;
        private float slideMotorBrakeStatus;
        private float ultraShortBaseLineMotorBrakeStatus;
        private float cableDeployLength;
        private float cableDeploySpeed;
        private float slidePosition;
        private float ultraShortBaseLineLinkPosition;
        private float ultraShortBaseLineVerticalPosition;
        private float winchCurrent;
        private float recoveryUnitCurrent;
        private float ultraShortBaseLineLinkCurrent;
        private float ultraShortBaseLineVerticalCurrent;
        private float cableTension;
        private float limitSwitchStatus;
        private byte recoveryUnitEmergencyStopStatus;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timestamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblySystemStatusType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timestamp.FromNative(sample.timestamp, keysOnly: false);
            sample.towedSonarArrayPowerStatus = towedSonarArrayPowerStatus;
            sample.ultraShortBaseLinePowerStatus = ultraShortBaseLinePowerStatus;
            sample.winchPowerStatus = winchPowerStatus;
            sample.recoveryUnitPowerStatus = recoveryUnitPowerStatus;
            sample.cameraPowerStatus = cameraPowerStatus;
            sample.towedSonarArrayDepthInfo = towedSonarArrayDepthInfo;
            sample.gpsDateUtc = gpsDateUtc;
            sample.gpsTimeUtc = gpsTimeUtc;
            sample.gpsLatitude = gpsLatitude;
            sample.gpsLongitude = gpsLongitude;
            sample.gpsVelocity = gpsVelocity;
            sample.gpsCourse = gpsCourse;
            sample.towedSonarArrayHeading = towedSonarArrayHeading;
            sample.towedSonarArrayDepth = towedSonarArrayDepth;
            sample.towedSonarArrayAltitude = towedSonarArrayAltitude;
            sample.towedSonarArrayPitch = towedSonarArrayPitch;
            sample.towedSonarArrayRoll = towedSonarArrayRoll;
            sample.towedSonarArrayTemperature = towedSonarArrayTemperature;
            sample.towedSonarArrayHumidity = towedSonarArrayHumidity;
            sample.towedSonarArrayVoltage = towedSonarArrayVoltage;
            sample.towedSonarArrayCurrent = towedSonarArrayCurrent;
            sample.recoveryUnitMode = recoveryUnitMode;
            sample.ultraShortBaseLineMode = ultraShortBaseLineMode;
            sample.winchMotorBrakeStatus = winchMotorBrakeStatus;
            sample.slideMotorBrakeStatus = slideMotorBrakeStatus;
            sample.ultraShortBaseLineMotorBrakeStatus = ultraShortBaseLineMotorBrakeStatus;
            sample.cableDeployLength = cableDeployLength;
            sample.cableDeploySpeed = cableDeploySpeed;
            sample.slidePosition = slidePosition;
            sample.ultraShortBaseLineLinkPosition = ultraShortBaseLineLinkPosition;
            sample.ultraShortBaseLineVerticalPosition = ultraShortBaseLineVerticalPosition;
            sample.winchCurrent = winchCurrent;
            sample.recoveryUnitCurrent = recoveryUnitCurrent;
            sample.ultraShortBaseLineLinkCurrent = ultraShortBaseLineLinkCurrent;
            sample.ultraShortBaseLineVerticalCurrent = ultraShortBaseLineVerticalCurrent;
            sample.cableTension = cableTension;
            sample.limitSwitchStatus = limitSwitchStatus;
            sample.recoveryUnitEmergencyStopStatus = recoveryUnitEmergencyStopStatus;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timestamp.Initialize(allocatePointers, allocateMemory);
            towedSonarArrayPowerStatus = (byte) (0);
            ultraShortBaseLinePowerStatus = (byte) (0);
            winchPowerStatus = (byte) (0);
            recoveryUnitPowerStatus = (byte) (0);
            cameraPowerStatus = (byte) (0);
            towedSonarArrayDepthInfo = (byte) (0);
            gpsDateUtc = (int) (0);
            gpsTimeUtc = (int) (0);
            gpsLatitude = (double) (0.0);
            gpsLongitude = (double) (0.0);
            gpsVelocity = (float) (0.0f);
            gpsCourse = (float) (0.0f);
            towedSonarArrayHeading = (float) (0.0f);
            towedSonarArrayDepth = (float) (0.0f);
            towedSonarArrayAltitude = (float) (0.0f);
            towedSonarArrayPitch = (float) (0.0f);
            towedSonarArrayRoll = (float) (0.0f);
            towedSonarArrayTemperature = (float) (0.0f);
            towedSonarArrayHumidity = (float) (0.0f);
            towedSonarArrayVoltage = (float) (0.0f);
            towedSonarArrayCurrent = (float) (0.0f);
            recoveryUnitMode = (float) (0.0f);
            ultraShortBaseLineMode = (float) (0.0f);
            winchMotorBrakeStatus = (float) (0.0f);
            slideMotorBrakeStatus = (float) (0.0f);
            ultraShortBaseLineMotorBrakeStatus = (float) (0.0f);
            cableDeployLength = (float) (0.0f);
            cableDeploySpeed = (float) (0.0f);
            slidePosition = (float) (0.0f);
            ultraShortBaseLineLinkPosition = (float) (0.0f);
            ultraShortBaseLineVerticalPosition = (float) (0.0f);
            winchCurrent = (float) (0.0f);
            recoveryUnitCurrent = (float) (0.0f);
            ultraShortBaseLineLinkCurrent = (float) (0.0f);
            ultraShortBaseLineVerticalCurrent = (float) (0.0f);
            cableTension = (float) (0.0f);
            limitSwitchStatus = (float) (0.0f);
            recoveryUnitEmergencyStopStatus = (byte) (0);
        }

        public void ToNative(global::TowedSonarAssemblySystemStatusType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timestamp.ToNative(sample.timestamp, keysOnly: false);
            towedSonarArrayPowerStatus = sample.towedSonarArrayPowerStatus;
            ultraShortBaseLinePowerStatus = sample.ultraShortBaseLinePowerStatus;
            winchPowerStatus = sample.winchPowerStatus;
            recoveryUnitPowerStatus = sample.recoveryUnitPowerStatus;
            cameraPowerStatus = sample.cameraPowerStatus;
            towedSonarArrayDepthInfo = sample.towedSonarArrayDepthInfo;
            gpsDateUtc = sample.gpsDateUtc;
            gpsTimeUtc = sample.gpsTimeUtc;
            gpsLatitude = sample.gpsLatitude;
            gpsLongitude = sample.gpsLongitude;
            gpsVelocity = sample.gpsVelocity;
            gpsCourse = sample.gpsCourse;
            towedSonarArrayHeading = sample.towedSonarArrayHeading;
            towedSonarArrayDepth = sample.towedSonarArrayDepth;
            towedSonarArrayAltitude = sample.towedSonarArrayAltitude;
            towedSonarArrayPitch = sample.towedSonarArrayPitch;
            towedSonarArrayRoll = sample.towedSonarArrayRoll;
            towedSonarArrayTemperature = sample.towedSonarArrayTemperature;
            towedSonarArrayHumidity = sample.towedSonarArrayHumidity;
            towedSonarArrayVoltage = sample.towedSonarArrayVoltage;
            towedSonarArrayCurrent = sample.towedSonarArrayCurrent;
            recoveryUnitMode = sample.recoveryUnitMode;
            ultraShortBaseLineMode = sample.ultraShortBaseLineMode;
            winchMotorBrakeStatus = sample.winchMotorBrakeStatus;
            slideMotorBrakeStatus = sample.slideMotorBrakeStatus;
            ultraShortBaseLineMotorBrakeStatus = sample.ultraShortBaseLineMotorBrakeStatus;
            cableDeployLength = sample.cableDeployLength;
            cableDeploySpeed = sample.cableDeploySpeed;
            slidePosition = sample.slidePosition;
            ultraShortBaseLineLinkPosition = sample.ultraShortBaseLineLinkPosition;
            ultraShortBaseLineVerticalPosition = sample.ultraShortBaseLineVerticalPosition;
            winchCurrent = sample.winchCurrent;
            recoveryUnitCurrent = sample.recoveryUnitCurrent;
            ultraShortBaseLineLinkCurrent = sample.ultraShortBaseLineLinkCurrent;
            ultraShortBaseLineVerticalCurrent = sample.ultraShortBaseLineVerticalCurrent;
            cableTension = sample.cableTension;
            limitSwitchStatus = sample.limitSwitchStatus;
            recoveryUnitEmergencyStopStatus = sample.recoveryUnitEmergencyStopStatus;
        }
    }

    internal class TowedSonarAssemblySystemStatusTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblySystemStatusType, TowedSonarAssemblySystemStatusTypeUnmanaged>
    {

        internal TowedSonarAssemblySystemStatusTypePlugin() : base("global::TowedSonarAssemblySystemStatusType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblySystemStatusType struct
            var TowedSonarAssemblySystemStatusTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timestamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("towedSonarArrayPowerStatus", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("ultraShortBaseLinePowerStatus", dtf.GetPrimitiveType<byte>(), id: 4),
                new StructMember("winchPowerStatus", dtf.GetPrimitiveType<byte>(), id: 5),
                new StructMember("recoveryUnitPowerStatus", dtf.GetPrimitiveType<byte>(), id: 6),
                new StructMember("cameraPowerStatus", dtf.GetPrimitiveType<byte>(), id: 7),
                new StructMember("towedSonarArrayDepthInfo", dtf.GetPrimitiveType<byte>(), id: 8),
                new StructMember("gpsDateUtc", dtf.GetPrimitiveType<int>(), id: 9),
                new StructMember("gpsTimeUtc", dtf.GetPrimitiveType<int>(), id: 10),
                new StructMember("gpsLatitude", dtf.GetPrimitiveType<double>(), id: 11),
                new StructMember("gpsLongitude", dtf.GetPrimitiveType<double>(), id: 12),
                new StructMember("gpsVelocity", dtf.GetPrimitiveType<float>(), id: 13),
                new StructMember("gpsCourse", dtf.GetPrimitiveType<float>(), id: 14),
                new StructMember("towedSonarArrayHeading", dtf.GetPrimitiveType<float>(), id: 15),
                new StructMember("towedSonarArrayDepth", dtf.GetPrimitiveType<float>(), id: 16),
                new StructMember("towedSonarArrayAltitude", dtf.GetPrimitiveType<float>(), id: 17),
                new StructMember("towedSonarArrayPitch", dtf.GetPrimitiveType<float>(), id: 18),
                new StructMember("towedSonarArrayRoll", dtf.GetPrimitiveType<float>(), id: 19),
                new StructMember("towedSonarArrayTemperature", dtf.GetPrimitiveType<float>(), id: 20),
                new StructMember("towedSonarArrayHumidity", dtf.GetPrimitiveType<float>(), id: 21),
                new StructMember("towedSonarArrayVoltage", dtf.GetPrimitiveType<float>(), id: 22),
                new StructMember("towedSonarArrayCurrent", dtf.GetPrimitiveType<float>(), id: 23),
                new StructMember("recoveryUnitMode", dtf.GetPrimitiveType<float>(), id: 24),
                new StructMember("ultraShortBaseLineMode", dtf.GetPrimitiveType<float>(), id: 25),
                new StructMember("winchMotorBrakeStatus", dtf.GetPrimitiveType<float>(), id: 26),
                new StructMember("slideMotorBrakeStatus", dtf.GetPrimitiveType<float>(), id: 27),
                new StructMember("ultraShortBaseLineMotorBrakeStatus", dtf.GetPrimitiveType<float>(), id: 28),
                new StructMember("cableDeployLength", dtf.GetPrimitiveType<float>(), id: 29),
                new StructMember("cableDeploySpeed", dtf.GetPrimitiveType<float>(), id: 30),
                new StructMember("slidePosition", dtf.GetPrimitiveType<float>(), id: 31),
                new StructMember("ultraShortBaseLineLinkPosition", dtf.GetPrimitiveType<float>(), id: 32),
                new StructMember("ultraShortBaseLineVerticalPosition", dtf.GetPrimitiveType<float>(), id: 33),
                new StructMember("winchCurrent", dtf.GetPrimitiveType<float>(), id: 34),
                new StructMember("recoveryUnitCurrent", dtf.GetPrimitiveType<float>(), id: 35),
                new StructMember("ultraShortBaseLineLinkCurrent", dtf.GetPrimitiveType<float>(), id: 36),
                new StructMember("ultraShortBaseLineVerticalCurrent", dtf.GetPrimitiveType<float>(), id: 37),
                new StructMember("cableTension", dtf.GetPrimitiveType<float>(), id: 38),
                new StructMember("limitSwitchStatus", dtf.GetPrimitiveType<float>(), id: 39),
                new StructMember("recoveryUnitEmergencyStopStatus", dtf.GetPrimitiveType<byte>(), id: 40)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblySystemStatusTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblySystemStatusType")
                .AddMembers(TowedSonarAssemblySystemStatusTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblySystemStatusTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblySystemStatusType>
{
    public TowedSonarAssemblySystemStatusTypeSupport() : base(
        new Implementation.TowedSonarAssemblySystemStatusTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblySystemStatusTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblySystemStatusTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblySystemStatusTypeSupport, global::TowedSonarAssemblySystemStatusType>();

}

namespace Implementation
{

    public struct TsaCompressedMergeSignalDataTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TsaCompressedMergeSignalDataType>
    {

        private int pingNumber;
        private short range;
        private byte gain;
        private byte tvg;
        private byte pulseWidth;
        private byte frequency;
        private byte pulseType;
        private double latitude;
        private double longitude;
        private float gpsVelocity;
        private float gpsCourse;
        private float towCourse;
        private float towDepth;
        private float towAltitude;
        private float towPitch;
        private float towRoll;
        private float towTemp;
        private float towHumidity;
        private float towVoltage;
        private float towCurrent;
        private short dataSideScanSonarCount;
        private short dataGapFillerCount;
        private short beamCount;
        private short channelCount;
        private NativeSeq data;

        public void Destroy(bool optionalsOnly)
        {
            data.Destroy(optionalsOnly);
        }

        public void FromNative(global::TsaCompressedMergeSignalDataType sample, bool keysOnly = false)
        {

            sample.pingNumber = pingNumber;
            sample.range = range;
            sample.gain = gain;
            sample.tvg = tvg;
            sample.pulseWidth = pulseWidth;
            sample.frequency = frequency;
            sample.pulseType = pulseType;
            sample.latitude = latitude;
            sample.longitude = longitude;
            sample.gpsVelocity = gpsVelocity;
            sample.gpsCourse = gpsCourse;
            sample.towCourse = towCourse;
            sample.towDepth = towDepth;
            sample.towAltitude = towAltitude;
            sample.towPitch = towPitch;
            sample.towRoll = towRoll;
            sample.towTemp = towTemp;
            sample.towHumidity = towHumidity;
            sample.towVoltage = towVoltage;
            sample.towCurrent = towCurrent;
            sample.dataSideScanSonarCount = dataSideScanSonarCount;
            sample.dataGapFillerCount = dataGapFillerCount;
            sample.beamCount = beamCount;
            sample.channelCount = channelCount;
            data.FromNative((Sequence<short>) sample.data);
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            pingNumber = (int) (0);
            range = (short) (0);
            gain = (byte) (0);
            tvg = (byte) (0);
            pulseWidth = (byte) (0);
            frequency = (byte) (0);
            pulseType = (byte) (0);
            latitude = (double) (0.0);
            longitude = (double) (0.0);
            gpsVelocity = (float) (0.0f);
            gpsCourse = (float) (0.0f);
            towCourse = (float) (0.0f);
            towDepth = (float) (0.0f);
            towAltitude = (float) (0.0f);
            towPitch = (float) (0.0f);
            towRoll = (float) (0.0f);
            towTemp = (float) (0.0f);
            towHumidity = (float) (0.0f);
            towVoltage = (float) (0.0f);
            towCurrent = (float) (0.0f);
            dataSideScanSonarCount = (short) (0);
            dataGapFillerCount = (short) (0);
            beamCount = (short) (0);
            channelCount = (short) (0);
            data.Initialize<short >(max: ((int)40000), absoluteMax: ((int)40000), allocateMemory: allocateMemory);
        }

        public void ToNative(global::TsaCompressedMergeSignalDataType sample, bool keysOnly = false)
        {
            pingNumber = sample.pingNumber;
            range = sample.range;
            gain = sample.gain;
            tvg = sample.tvg;
            pulseWidth = sample.pulseWidth;
            frequency = sample.frequency;
            pulseType = sample.pulseType;
            latitude = sample.latitude;
            longitude = sample.longitude;
            gpsVelocity = sample.gpsVelocity;
            gpsCourse = sample.gpsCourse;
            towCourse = sample.towCourse;
            towDepth = sample.towDepth;
            towAltitude = sample.towAltitude;
            towPitch = sample.towPitch;
            towRoll = sample.towRoll;
            towTemp = sample.towTemp;
            towHumidity = sample.towHumidity;
            towVoltage = sample.towVoltage;
            towCurrent = sample.towCurrent;
            dataSideScanSonarCount = sample.dataSideScanSonarCount;
            dataGapFillerCount = sample.dataGapFillerCount;
            beamCount = sample.beamCount;
            channelCount = sample.channelCount;
            data.ToNative((Sequence<short>) sample.data);
        }
    }

    internal class TsaCompressedMergeSignalDataTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TsaCompressedMergeSignalDataType, TsaCompressedMergeSignalDataTypeUnmanaged>
    {

        internal TsaCompressedMergeSignalDataTypePlugin() : base("global::TsaCompressedMergeSignalDataType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TsaCompressedMergeSignalDataType struct
            var TsaCompressedMergeSignalDataTypeStructMembers = new StructMember[]
            {
                new StructMember("pingNumber", dtf.GetPrimitiveType<int>(), id: 0),
                new StructMember("range", dtf.GetPrimitiveType<short>(), id: 1),
                new StructMember("gain", dtf.GetPrimitiveType<byte>(), id: 2),
                new StructMember("tvg", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("pulseWidth", dtf.GetPrimitiveType<byte>(), id: 4),
                new StructMember("frequency", dtf.GetPrimitiveType<byte>(), id: 5),
                new StructMember("pulseType", dtf.GetPrimitiveType<byte>(), id: 6),
                new StructMember("latitude", dtf.GetPrimitiveType<double>(), id: 7),
                new StructMember("longitude", dtf.GetPrimitiveType<double>(), id: 8),
                new StructMember("gpsVelocity", dtf.GetPrimitiveType<float>(), id: 9),
                new StructMember("gpsCourse", dtf.GetPrimitiveType<float>(), id: 10),
                new StructMember("towCourse", dtf.GetPrimitiveType<float>(), id: 11),
                new StructMember("towDepth", dtf.GetPrimitiveType<float>(), id: 12),
                new StructMember("towAltitude", dtf.GetPrimitiveType<float>(), id: 13),
                new StructMember("towPitch", dtf.GetPrimitiveType<float>(), id: 14),
                new StructMember("towRoll", dtf.GetPrimitiveType<float>(), id: 15),
                new StructMember("towTemp", dtf.GetPrimitiveType<float>(), id: 16),
                new StructMember("towHumidity", dtf.GetPrimitiveType<float>(), id: 17),
                new StructMember("towVoltage", dtf.GetPrimitiveType<float>(), id: 18),
                new StructMember("towCurrent", dtf.GetPrimitiveType<float>(), id: 19),
                new StructMember("dataSideScanSonarCount", dtf.GetPrimitiveType<short>(), id: 20),
                new StructMember("dataGapFillerCount", dtf.GetPrimitiveType<short>(), id: 21),
                new StructMember("beamCount", dtf.GetPrimitiveType<short>(), id: 22),
                new StructMember("channelCount", dtf.GetPrimitiveType<short>(), id: 23),
                new StructMember("data", tsf.CreateSequenceWithAccessInfo(dtf, dtf.GetPrimitiveType<short>(), ((int)40000)), id: 24)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TsaCompressedMergeSignalDataTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TsaCompressedMergeSignalDataType")
                .AddMembers(TsaCompressedMergeSignalDataTypeStructMembers));

            return result;
        }
    }
}
public class TsaCompressedMergeSignalDataTypeSupport : Rti.Dds.Topics.TypeSupport<global::TsaCompressedMergeSignalDataType>
{
    public TsaCompressedMergeSignalDataTypeSupport() : base(
        new Implementation.TsaCompressedMergeSignalDataTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TsaCompressedMergeSignalDataTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TsaCompressedMergeSignalDataTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TsaCompressedMergeSignalDataTypeSupport, global::TsaCompressedMergeSignalDataType>();

}

