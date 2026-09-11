/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from common.idl
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

    public struct DateTimeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::DateTime>
    {

        private short year;
        private short month;
        private short day;
        private short hour;

        public void Destroy(bool optionalsOnly)
        {
        }

        public void FromNative(global::DateTime sample, bool keysOnly = false)
        {

            sample.year = year;
            sample.month = month;
            sample.day = day;
            sample.hour = hour;
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            year = (short) (0);
            month = (short) (0);
            day = (short) (0);
            hour = (short) (0);
        }

        public void ToNative(global::DateTime sample, bool keysOnly = false)
        {
            year = sample.year;
            month = sample.month;
            day = sample.day;
            hour = sample.hour;
        }
    }

    internal class DateTimePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::DateTime, DateTimeUnmanaged>
    {

        internal DateTimePlugin() : base("global::DateTime", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // DateTime struct
            var DateTimeStructMembers = new StructMember[]
            {
                new StructMember("year", dtf.GetPrimitiveType<short>(), id: 0),
                new StructMember("month", dtf.GetPrimitiveType<short>(), id: 1),
                new StructMember("day", dtf.GetPrimitiveType<short>(), id: 2),
                new StructMember("hour", dtf.GetPrimitiveType<short>(), id: 3)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<DateTimeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("DateTime")
                .AddMembers(DateTimeStructMembers));

            return result;
        }
    }
}
public class DateTimeSupport : Rti.Dds.Topics.TypeSupport<global::DateTime>
{
    public DateTimeSupport() : base(
        new Implementation.DateTimePlugin(),
        new Lazy<DynamicType>(() =>Implementation.DateTimePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static DateTimeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<DateTimeSupport, global::DateTime>();

}

