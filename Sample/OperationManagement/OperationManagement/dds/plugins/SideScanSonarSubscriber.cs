/*
* (c) Copyright, Real-Time Innovations, 2012.  All rights reserved.
* RTI grants Licensee a license to use, modify, compile, and create derivative
* works of the software solely for use with RTI Connext DDS. Licensee may
* redistribute copies of the software provided that all such copies are subject
* to this license. The software is provided "as is", with no warranty of any
* type, including any warranty for fitness for any purpose. RTI is under no
* obligation to maintain or support the software. RTI shall not be liable for
* any incidental or consequential damages arising out of the use or inability
* to use the software.
*/

using System;
using System.Threading.Tasks;
using Omg.Dds.Core;
using Rti.Dds.Core;
using Rti.Dds.Core.Status;
using Rti.Dds.Domain;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;

/// <summary>
/// Example application that subscribes to global::HelloWorld.
/// </summary>
public static class SideScanSonarSubscriber
{
    private static void ProcessData<T>(DataReader<T> reader)
    {
        // Take all samples. Samples are loaned to application; loan is
        // returned when the samples collection is Disposed.
        using (var samples = reader.Take())
        {
            foreach (var sample in samples)
            {
                if (sample.Info.ValidData)
                {
                    Console.WriteLine(sample.Data);
                }
                else
                {
                    Console.WriteLine($"Received instance update: {sample.Info.State.Instance}");
                }
            }
        }
    }

    /// <summary>
    /// Runs the subscriber example.
    /// </summary>
    public static async Task RunSubscriber(int domainId = 0, int sampleCount = int.MaxValue)
    {
        // A DomainParticipant allows an application to begin communicating in
        // a DDS domain. Typically there is one DomainParticipant per application.
        // DomainParticipant QoS is configured in USER_QOS_PROFILES.xml
        //
        // A participant needs to be Disposed to release middleware resources.
        // The 'using' keyword indicates that it will be Disposed when this
        // scope ends.
        using DomainParticipant participant = DomainParticipantFactory.Instance.CreateParticipant(domainId);

        // A Topic has a name and a datatype.
        var topic1 = participant.CreateTopic<TowedSonarAssemblyStatusConfigType>("TowedSonarAssemblyStatusConfigType");
        var topic2 = participant.CreateTopic<TowedSonarArrayStartControlType>("TowedSonarArrayStartControlType");
        var topic3 = participant.CreateTopic<TowedSonarAssemblyPlatformPowerControlType>("TowedSonarAssemblyPlatformPowerControlType");
        var topic4 = participant.CreateTopic<TowedSonarAssemblyModeControlType>("TowedSonarAssemblyModeControlType");
        var topic5 = participant.CreateTopic<TowedSonarAssemblyAutoLaunchControlType>("TowedSonarAssemblyAutoLaunchControlType");
        var topic6 = participant.CreateTopic<TowedSonarAssemblyRestartControlType>("TowedSonarAssemblyRestartControlType");
        var topic7 = participant.CreateTopic<TowedSonarAssemblyEmergencyStopControlType>("TowedSonarAssemblyEmergencyStopControlType");
        var topic8 = participant.CreateTopic<TowedSonarAssemblyCableLengthControlType>("TowedSonarAssemblyCableLengthControlType");
        var topic9 = participant.CreateTopic<TowedSonarAssemblyTargetLengthConfigType>("TowedSonarAssemblyTargetLengthConfigType");
        var topic10 = participant.CreateTopic<TowedSonarAssemblyScreanChangeConfigType>("TowedSonarAssemblyScreanChangeConfigType");
        var topic11 = participant.CreateTopic<TowedSonarAssemblyManualUltraShortBaseLineMotorControlType>("TowedSonarAssemblyManualUltraShortBaseLineMotorControlType");
        var topic12 = participant.CreateTopic<TowedSonarAssemblyUltraShortBaseLineStartControlType>("TowedSonarAssemblyUltraShortBaseLineStartControlType");
        var topic13 = participant.CreateTopic<TowedSonarAssemblyLaunchAndRecoverySlideStartControlType>("TowedSonarAssemblyLaunchAndRecoverySlideStartControlType");
        var topic14 = participant.CreateTopic<TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType>("TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType");

        // A Subscriber allows an application to create one or more DataReaders
        // Subscriber QoS is configured in USER_QOS_PROFILES.xml
        Subscriber subscriber = participant.CreateSubscriber();

        // This DataReader reads data on Topic "Example HelloWorld".
        // DataReader QoS is configured in USER_QOS_PROFILES.xml
        var reader1 = subscriber.CreateDataReader(topic1);
        var reader2 = subscriber.CreateDataReader(topic2);
        var reader3 = subscriber.CreateDataReader(topic3);
        var reader4 = subscriber.CreateDataReader(topic4);
        var reader5 = subscriber.CreateDataReader(topic5);
        var reader6 = subscriber.CreateDataReader(topic6);
        var reader7 = subscriber.CreateDataReader(topic7);
        var reader8 = subscriber.CreateDataReader(topic8);
        var reader9 = subscriber.CreateDataReader(topic9);
        var reader10 = subscriber.CreateDataReader(topic10);
        var reader11 = subscriber.CreateDataReader(topic11);
        var reader12 = subscriber.CreateDataReader(topic12);
        var reader13 = subscriber.CreateDataReader(topic13);
        var reader14 = subscriber.CreateDataReader(topic14);

        // Obtain the DataReader's Status Condition
        StatusCondition statusCondition1 = reader1.StatusCondition;
        StatusCondition statusCondition2 = reader2.StatusCondition;
        StatusCondition statusCondition3 = reader3.StatusCondition;
        StatusCondition statusCondition4 = reader4.StatusCondition;
        StatusCondition statusCondition5 = reader5.StatusCondition;
        StatusCondition statusCondition6 = reader6.StatusCondition;
        StatusCondition statusCondition7 = reader7.StatusCondition;
        StatusCondition statusCondition8 = reader8.StatusCondition;
        StatusCondition statusCondition9 = reader9.StatusCondition;
        StatusCondition statusCondition10 = reader10.StatusCondition;
        StatusCondition statusCondition11 = reader11.StatusCondition;
        StatusCondition statusCondition12 = reader12.StatusCondition;
        StatusCondition statusCondition13 = reader13.StatusCondition;
        StatusCondition statusCondition14 = reader14.StatusCondition;

        // Enable the 'data available' status.
        statusCondition1.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition2.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition3.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition4.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition5.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition6.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition7.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition8.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition9.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition10.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition11.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition12.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition13.EnabledStatuses = StatusMask.DataAvailable;
        statusCondition14.EnabledStatuses = StatusMask.DataAvailable;

        // Associate an event handler with the status condition.
        // This will run when the condition is triggered, in the context of
        // the dispatch call (see below)
        statusCondition1.Triggered += _ => ProcessData<TowedSonarAssemblyStatusConfigType>(reader1);
        statusCondition2.Triggered += _ => ProcessData<TowedSonarArrayStartControlType>(reader2);
        statusCondition3.Triggered += _ => ProcessData<TowedSonarAssemblyPlatformPowerControlType>(reader3);
        statusCondition4.Triggered += _ => ProcessData<TowedSonarAssemblyModeControlType>(reader4);
        statusCondition5.Triggered += _ => ProcessData<TowedSonarAssemblyAutoLaunchControlType>(reader5);
        statusCondition6.Triggered += _ => ProcessData<TowedSonarAssemblyRestartControlType>(reader6);
        statusCondition7.Triggered += _ => ProcessData<TowedSonarAssemblyEmergencyStopControlType>(reader7);
        statusCondition8.Triggered += _ => ProcessData<TowedSonarAssemblyCableLengthControlType>(reader8);
        statusCondition9.Triggered += _ => ProcessData<TowedSonarAssemblyTargetLengthConfigType>(reader9);
        statusCondition10.Triggered += _ => ProcessData<TowedSonarAssemblyScreanChangeConfigType>(reader10);
        statusCondition11.Triggered += _ => ProcessData<TowedSonarAssemblyManualUltraShortBaseLineMotorControlType>(reader11);
        statusCondition12.Triggered += _ => ProcessData<TowedSonarAssemblyUltraShortBaseLineStartControlType>(reader12);
        statusCondition13.Triggered += _ => ProcessData<TowedSonarAssemblyLaunchAndRecoverySlideStartControlType>(reader13);
        statusCondition14.Triggered += _ => ProcessData<TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType>(reader14);

        // Create a WaitSet and attach the StatusCondition
        var waitset = new WaitSet();
        waitset.AttachCondition(statusCondition1);
        waitset.AttachCondition(statusCondition2);
        waitset.AttachCondition(statusCondition3);
        waitset.AttachCondition(statusCondition4);
        waitset.AttachCondition(statusCondition5);
        waitset.AttachCondition(statusCondition6);
        waitset.AttachCondition(statusCondition7);
        waitset.AttachCondition(statusCondition8);
        waitset.AttachCondition(statusCondition9);
        waitset.AttachCondition(statusCondition10);
        waitset.AttachCondition(statusCondition11);
        waitset.AttachCondition(statusCondition12);
        waitset.AttachCondition(statusCondition13);
        waitset.AttachCondition(statusCondition14);

        while (true)
        {
            // Dispatch will call the handlers associated with the WaitSet
            // conditions when they activate
            Console.WriteLine("SideScanSonar subscriber sleeping for 4 sec...");
            waitset.Dispatch(Duration.FromSeconds(4));
        }
    }
}
