/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from side_scan_sonar_sub.idl
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

    public struct TowedSonarAssemblyStatusConfigTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyStatusConfigType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private short range;
        private byte gain;
        private byte timeVariedGain;
        private byte pulseWidth;
        private byte frequency;
        private byte pulseType;
        private byte lineBalanceOffserAmplifier;
        private byte lineAmplifier;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyStatusConfigType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.range = range;
            sample.gain = gain;
            sample.timeVariedGain = timeVariedGain;
            sample.pulseWidth = pulseWidth;
            sample.frequency = frequency;
            sample.pulseType = pulseType;
            sample.lineBalanceOffserAmplifier = lineBalanceOffserAmplifier;
            sample.lineAmplifier = lineAmplifier;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            range = (short) (0);
            gain = (byte) (0);
            timeVariedGain = (byte) (0);
            pulseWidth = (byte) (0);
            frequency = (byte) (0);
            pulseType = (byte) (0);
            lineBalanceOffserAmplifier = (byte) (0);
            lineAmplifier = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyStatusConfigType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            range = sample.range;
            gain = sample.gain;
            timeVariedGain = sample.timeVariedGain;
            pulseWidth = sample.pulseWidth;
            frequency = sample.frequency;
            pulseType = sample.pulseType;
            lineBalanceOffserAmplifier = sample.lineBalanceOffserAmplifier;
            lineAmplifier = sample.lineAmplifier;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyStatusConfigTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyStatusConfigType, TowedSonarAssemblyStatusConfigTypeUnmanaged>
    {

        internal TowedSonarAssemblyStatusConfigTypePlugin() : base("global::TowedSonarAssemblyStatusConfigType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyStatusConfigType struct
            var TowedSonarAssemblyStatusConfigTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("range", dtf.GetPrimitiveType<short>(), id: 3),
                new StructMember("gain", dtf.GetPrimitiveType<byte>(), id: 4),
                new StructMember("timeVariedGain", dtf.GetPrimitiveType<byte>(), id: 5),
                new StructMember("pulseWidth", dtf.GetPrimitiveType<byte>(), id: 6),
                new StructMember("frequency", dtf.GetPrimitiveType<byte>(), id: 7),
                new StructMember("pulseType", dtf.GetPrimitiveType<byte>(), id: 8),
                new StructMember("lineBalanceOffserAmplifier", dtf.GetPrimitiveType<byte>(), id: 9),
                new StructMember("lineAmplifier", dtf.GetPrimitiveType<byte>(), id: 10),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 11)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyStatusConfigTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyStatusConfigType")
                .AddMembers(TowedSonarAssemblyStatusConfigTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyStatusConfigTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyStatusConfigType>
{
    public TowedSonarAssemblyStatusConfigTypeSupport() : base(
        new Implementation.TowedSonarAssemblyStatusConfigTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyStatusConfigTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyStatusConfigTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyStatusConfigTypeSupport, global::TowedSonarAssemblyStatusConfigType>();

}

namespace Implementation
{

    public struct TowedSonarArrayStartControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarArrayStartControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte start;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarArrayStartControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.start = start;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            start = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarArrayStartControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            start = sample.start;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarArrayStartControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarArrayStartControlType, TowedSonarArrayStartControlTypeUnmanaged>
    {

        internal TowedSonarArrayStartControlTypePlugin() : base("global::TowedSonarArrayStartControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarArrayStartControlType struct
            var TowedSonarArrayStartControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("start", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarArrayStartControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarArrayStartControlType")
                .AddMembers(TowedSonarArrayStartControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarArrayStartControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarArrayStartControlType>
{
    public TowedSonarArrayStartControlTypeSupport() : base(
        new Implementation.TowedSonarArrayStartControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarArrayStartControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarArrayStartControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarArrayStartControlTypeSupport, global::TowedSonarArrayStartControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyPlatformPowerControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyPlatformPowerControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte targetDevice;
        private byte powerOn;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyPlatformPowerControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.targetDevice = targetDevice;
            sample.powerOn = powerOn;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            targetDevice = (byte) (0);
            powerOn = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyPlatformPowerControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            targetDevice = sample.targetDevice;
            powerOn = sample.powerOn;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyPlatformPowerControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyPlatformPowerControlType, TowedSonarAssemblyPlatformPowerControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyPlatformPowerControlTypePlugin() : base("global::TowedSonarAssemblyPlatformPowerControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyPlatformPowerControlType struct
            var TowedSonarAssemblyPlatformPowerControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("targetDevice", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("powerOn", dtf.GetPrimitiveType<byte>(), id: 4),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 5)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyPlatformPowerControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyPlatformPowerControlType")
                .AddMembers(TowedSonarAssemblyPlatformPowerControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyPlatformPowerControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyPlatformPowerControlType>
{
    public TowedSonarAssemblyPlatformPowerControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyPlatformPowerControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyPlatformPowerControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyPlatformPowerControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyPlatformPowerControlTypeSupport, global::TowedSonarAssemblyPlatformPowerControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyModeControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyModeControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte mode;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyModeControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.mode = mode;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            mode = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyModeControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            mode = sample.mode;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyModeControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyModeControlType, TowedSonarAssemblyModeControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyModeControlTypePlugin() : base("global::TowedSonarAssemblyModeControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyModeControlType struct
            var TowedSonarAssemblyModeControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("mode", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyModeControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyModeControlType")
                .AddMembers(TowedSonarAssemblyModeControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyModeControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyModeControlType>
{
    public TowedSonarAssemblyModeControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyModeControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyModeControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyModeControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyModeControlTypeSupport, global::TowedSonarAssemblyModeControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyAutoLaunchControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyAutoLaunchControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte launch;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyAutoLaunchControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.launch = launch;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            launch = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyAutoLaunchControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            launch = sample.launch;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyAutoLaunchControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyAutoLaunchControlType, TowedSonarAssemblyAutoLaunchControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyAutoLaunchControlTypePlugin() : base("global::TowedSonarAssemblyAutoLaunchControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyAutoLaunchControlType struct
            var TowedSonarAssemblyAutoLaunchControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("launch", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyAutoLaunchControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyAutoLaunchControlType")
                .AddMembers(TowedSonarAssemblyAutoLaunchControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyAutoLaunchControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyAutoLaunchControlType>
{
    public TowedSonarAssemblyAutoLaunchControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyAutoLaunchControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyAutoLaunchControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyAutoLaunchControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyAutoLaunchControlTypeSupport, global::TowedSonarAssemblyAutoLaunchControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyRestartControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyRestartControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte restart;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyRestartControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.restart = restart;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            restart = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyRestartControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            restart = sample.restart;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyRestartControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyRestartControlType, TowedSonarAssemblyRestartControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyRestartControlTypePlugin() : base("global::TowedSonarAssemblyRestartControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyRestartControlType struct
            var TowedSonarAssemblyRestartControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("restart", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyRestartControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyRestartControlType")
                .AddMembers(TowedSonarAssemblyRestartControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyRestartControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyRestartControlType>
{
    public TowedSonarAssemblyRestartControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyRestartControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyRestartControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyRestartControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyRestartControlTypeSupport, global::TowedSonarAssemblyRestartControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyEmergencyStopControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyEmergencyStopControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte emergencyStop;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyEmergencyStopControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.emergencyStop = emergencyStop;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            emergencyStop = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyEmergencyStopControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            emergencyStop = sample.emergencyStop;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyEmergencyStopControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyEmergencyStopControlType, TowedSonarAssemblyEmergencyStopControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyEmergencyStopControlTypePlugin() : base("global::TowedSonarAssemblyEmergencyStopControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyEmergencyStopControlType struct
            var TowedSonarAssemblyEmergencyStopControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("emergencyStop", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyEmergencyStopControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyEmergencyStopControlType")
                .AddMembers(TowedSonarAssemblyEmergencyStopControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyEmergencyStopControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyEmergencyStopControlType>
{
    public TowedSonarAssemblyEmergencyStopControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyEmergencyStopControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyEmergencyStopControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyEmergencyStopControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyEmergencyStopControlTypeSupport, global::TowedSonarAssemblyEmergencyStopControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyCableLengthControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyCableLengthControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private float lengthCommandWinch;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyCableLengthControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.lengthCommandWinch = lengthCommandWinch;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            lengthCommandWinch = (float) (0.0f);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyCableLengthControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            lengthCommandWinch = sample.lengthCommandWinch;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyCableLengthControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyCableLengthControlType, TowedSonarAssemblyCableLengthControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyCableLengthControlTypePlugin() : base("global::TowedSonarAssemblyCableLengthControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyCableLengthControlType struct
            var TowedSonarAssemblyCableLengthControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("lengthCommandWinch", dtf.GetPrimitiveType<float>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyCableLengthControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyCableLengthControlType")
                .AddMembers(TowedSonarAssemblyCableLengthControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyCableLengthControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyCableLengthControlType>
{
    public TowedSonarAssemblyCableLengthControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyCableLengthControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyCableLengthControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyCableLengthControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyCableLengthControlTypeSupport, global::TowedSonarAssemblyCableLengthControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyTargetLengthConfigTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyTargetLengthConfigType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private float targetDepth;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyTargetLengthConfigType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.targetDepth = targetDepth;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            targetDepth = (float) (0.0f);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyTargetLengthConfigType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            targetDepth = sample.targetDepth;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyTargetLengthConfigTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyTargetLengthConfigType, TowedSonarAssemblyTargetLengthConfigTypeUnmanaged>
    {

        internal TowedSonarAssemblyTargetLengthConfigTypePlugin() : base("global::TowedSonarAssemblyTargetLengthConfigType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyTargetLengthConfigType struct
            var TowedSonarAssemblyTargetLengthConfigTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("targetDepth", dtf.GetPrimitiveType<float>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyTargetLengthConfigTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyTargetLengthConfigType")
                .AddMembers(TowedSonarAssemblyTargetLengthConfigTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyTargetLengthConfigTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyTargetLengthConfigType>
{
    public TowedSonarAssemblyTargetLengthConfigTypeSupport() : base(
        new Implementation.TowedSonarAssemblyTargetLengthConfigTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyTargetLengthConfigTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyTargetLengthConfigTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyTargetLengthConfigTypeSupport, global::TowedSonarAssemblyTargetLengthConfigType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyScreanChangeConfigTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyScreanChangeConfigType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte screenChangeMode;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyScreanChangeConfigType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.screenChangeMode = screenChangeMode;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            screenChangeMode = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyScreanChangeConfigType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            screenChangeMode = sample.screenChangeMode;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyScreanChangeConfigTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyScreanChangeConfigType, TowedSonarAssemblyScreanChangeConfigTypeUnmanaged>
    {

        internal TowedSonarAssemblyScreanChangeConfigTypePlugin() : base("global::TowedSonarAssemblyScreanChangeConfigType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyScreanChangeConfigType struct
            var TowedSonarAssemblyScreanChangeConfigTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("screenChangeMode", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyScreanChangeConfigTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyScreanChangeConfigType")
                .AddMembers(TowedSonarAssemblyScreanChangeConfigTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyScreanChangeConfigTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyScreanChangeConfigType>
{
    public TowedSonarAssemblyScreanChangeConfigTypeSupport() : base(
        new Implementation.TowedSonarAssemblyScreanChangeConfigTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyScreanChangeConfigTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyScreanChangeConfigTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyScreanChangeConfigTypeSupport, global::TowedSonarAssemblyScreanChangeConfigType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyManualUltraShortBaseLineMotorControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte launch;
        private float motorSpeed;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyManualUltraShortBaseLineMotorControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.launch = launch;
            sample.motorSpeed = motorSpeed;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            launch = (byte) (0);
            motorSpeed = (float) (0.0f);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyManualUltraShortBaseLineMotorControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            launch = sample.launch;
            motorSpeed = sample.motorSpeed;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyManualUltraShortBaseLineMotorControlType, TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypePlugin() : base("global::TowedSonarAssemblyManualUltraShortBaseLineMotorControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyManualUltraShortBaseLineMotorControlType struct
            var TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("launch", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("motorSpeed", dtf.GetPrimitiveType<float>(), id: 4),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 5)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyManualUltraShortBaseLineMotorControlType")
                .AddMembers(TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyManualUltraShortBaseLineMotorControlType>
{
    public TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeSupport, global::TowedSonarAssemblyManualUltraShortBaseLineMotorControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyUltraShortBaseLineStartControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyUltraShortBaseLineStartControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte start;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyUltraShortBaseLineStartControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.start = start;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            start = (byte) (0);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyUltraShortBaseLineStartControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            start = sample.start;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyUltraShortBaseLineStartControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyUltraShortBaseLineStartControlType, TowedSonarAssemblyUltraShortBaseLineStartControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyUltraShortBaseLineStartControlTypePlugin() : base("global::TowedSonarAssemblyUltraShortBaseLineStartControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyUltraShortBaseLineStartControlType struct
            var TowedSonarAssemblyUltraShortBaseLineStartControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("start", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 4)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyUltraShortBaseLineStartControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyUltraShortBaseLineStartControlType")
                .AddMembers(TowedSonarAssemblyUltraShortBaseLineStartControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyUltraShortBaseLineStartControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyUltraShortBaseLineStartControlType>
{
    public TowedSonarAssemblyUltraShortBaseLineStartControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyUltraShortBaseLineStartControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyUltraShortBaseLineStartControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyUltraShortBaseLineStartControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyUltraShortBaseLineStartControlTypeSupport, global::TowedSonarAssemblyUltraShortBaseLineStartControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyLaunchAndRecoverySlideStartControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte start;
        private float slideDeploy;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyLaunchAndRecoverySlideStartControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.start = start;
            sample.slideDeploy = slideDeploy;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            start = (byte) (0);
            slideDeploy = (float) (0.0f);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyLaunchAndRecoverySlideStartControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            start = sample.start;
            slideDeploy = sample.slideDeploy;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyLaunchAndRecoverySlideStartControlType, TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypePlugin() : base("global::TowedSonarAssemblyLaunchAndRecoverySlideStartControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyLaunchAndRecoverySlideStartControlType struct
            var TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("start", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("slideDeploy", dtf.GetPrimitiveType<float>(), id: 4),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 5)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyLaunchAndRecoverySlideStartControlType")
                .AddMembers(TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyLaunchAndRecoverySlideStartControlType>
{
    public TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeSupport, global::TowedSonarAssemblyLaunchAndRecoverySlideStartControlType>();

}

namespace Implementation
{

    public struct TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType>
    {

        private byte platformIDKey;
        private byte equipmentIDKey;
        private global::Implementation.DateTimeUnmanaged timeStamp;
        private byte start;
        private float winchDeploy;
        private short commandID;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            timeStamp.Destroy(optionalsOnly);
        }

        public void FromNative(global::TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType sample, bool keysOnly = false)
        {

            sample.platformIDKey = platformIDKey;
            sample.equipmentIDKey = equipmentIDKey;
            timeStamp.FromNative(sample.timeStamp, keysOnly: false);
            sample.start = start;
            sample.winchDeploy = winchDeploy;
            sample.commandID = commandID;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            platformIDKey = (byte) (0);
            equipmentIDKey = (byte) (0);
            timeStamp.Initialize(allocatePointers, allocateMemory);
            start = (byte) (0);
            winchDeploy = (float) (0.0f);
            commandID = (short) (0);
        }

        public void ToNative(global::TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType sample, bool keysOnly = false)
        {
            platformIDKey = sample.platformIDKey;
            equipmentIDKey = sample.equipmentIDKey;
            timeStamp.ToNative(sample.timeStamp, keysOnly: false);
            start = sample.start;
            winchDeploy = sample.winchDeploy;
            commandID = sample.commandID;
        }
    }

    internal class TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType, TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeUnmanaged>
    {

        internal TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypePlugin() : base("global::TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType struct
            var TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeStructMembers = new StructMember[]
            {
                new StructMember("platformIDKey", dtf.GetPrimitiveType<byte>(), id: 0),
                new StructMember("equipmentIDKey", dtf.GetPrimitiveType<byte>(), id: 1),
                new StructMember("timeStamp", global::DateTimeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("start", dtf.GetPrimitiveType<byte>(), id: 3),
                new StructMember("winchDeploy", dtf.GetPrimitiveType<float>(), id: 4),
                new StructMember("commandID", dtf.GetPrimitiveType<short>(), id: 5)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType")
                .AddMembers(TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeStructMembers));

            return result;
        }
    }
}
public class TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeSupport : Rti.Dds.Topics.TypeSupport<global::TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType>
{
    public TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeSupport() : base(
        new Implementation.TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeSupport, global::TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType>();

}

