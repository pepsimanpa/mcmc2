using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Topics;

namespace MessageAdapter
{
    public class TransportAdapterRegistry
    {
        public TransportAdapterRegistry()
        { }

        public bool Register(string topicName, ITransportAdapter adapter)
        {
            return _adapters.TryAdd(topicName, adapter);
        }

        public ITransportAdapter GetAdapter(string topicName)
        {
            if (_adapters.TryGetValue(topicName, out ITransportAdapter value))
                return value;
            else
                return null;
        }

        private readonly Dictionary<string, ITransportAdapter> _adapters = new();
    }

    public interface ITransportAdapter
    {
        public object CreateSample();
        public void SetField(object sample, string fieldName, object value);
        public void Write(object sample);
        public Type TopicType { get; }
    }

    public class TowedSonarArrayStartControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarArrayStartControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarArrayStartControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarArrayStartControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarArrayStartControlType;

            switch(fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };  
                    break;
                case "start":
                    msg.start = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarArrayStartControlType);
        }

        public Type TopicType { get => typeof(TowedSonarArrayStartControlType); }
        private readonly DataWriter<TowedSonarArrayStartControlType> _dataWriter;
    }

    public class TowedSonarAssemblyAutoLaunchControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyAutoLaunchControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyAutoLaunchControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyAutoLaunchControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyAutoLaunchControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "launch":
                    msg.launch = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyAutoLaunchControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyAutoLaunchControlType); }
        private readonly DataWriter<TowedSonarAssemblyAutoLaunchControlType> _dataWriter;
    }

    public class TowedSonarAssemblyPlatformPowerControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyPlatformPowerControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyPlatformPowerControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyPlatformPowerControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyPlatformPowerControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "targetDevice":
                    msg.targetDevice = Convert.ToByte(value);
                    break;
                case "powerOn":
                    msg.powerOn = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyPlatformPowerControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyPlatformPowerControlType); }
        private readonly DataWriter<TowedSonarAssemblyPlatformPowerControlType> _dataWriter;
    }

    public class TowedSonarAssemblyModeControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyModeControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyModeControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyModeControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyModeControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "mode":
                    msg.mode = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyModeControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyModeControlType); }
        private readonly DataWriter<TowedSonarAssemblyModeControlType> _dataWriter;
    }

    public class TowedSonarAssemblyRestartControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyRestartControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyRestartControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyRestartControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyRestartControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "restart":
                    msg.restart = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyRestartControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyRestartControlType); }
        private readonly DataWriter<TowedSonarAssemblyRestartControlType> _dataWriter;
    }

    public class TowedSonarAssemblyEmergencyStopControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyEmergencyStopControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyEmergencyStopControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyEmergencyStopControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyEmergencyStopControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "emergencyStop":
                    msg.emergencyStop = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyEmergencyStopControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyEmergencyStopControlType); }
        private readonly DataWriter<TowedSonarAssemblyEmergencyStopControlType> _dataWriter;
    }

    public class TowedSonarAssemblyScreanChangeConfigTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyScreanChangeConfigTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyScreanChangeConfigType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyScreanChangeConfigType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyScreanChangeConfigType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "screenChangeMode":
                    msg.screenChangeMode = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyScreanChangeConfigType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyScreanChangeConfigType); }
        private readonly DataWriter<TowedSonarAssemblyScreanChangeConfigType> _dataWriter;
    }

    public class TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyManualUltraShortBaseLineMotorControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyManualUltraShortBaseLineMotorControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyManualUltraShortBaseLineMotorControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyManualUltraShortBaseLineMotorControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "launch":
                    msg.launch = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyManualUltraShortBaseLineMotorControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyManualUltraShortBaseLineMotorControlType); }
        private readonly DataWriter<TowedSonarAssemblyManualUltraShortBaseLineMotorControlType> _dataWriter;
    }

    public class TowedSonarAssemblyUltraShortBaseLineStartControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyUltraShortBaseLineStartControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyUltraShortBaseLineStartControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyUltraShortBaseLineStartControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyUltraShortBaseLineStartControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "start":
                    msg.start = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyUltraShortBaseLineStartControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyUltraShortBaseLineStartControlType); }
        private readonly DataWriter<TowedSonarAssemblyUltraShortBaseLineStartControlType> _dataWriter;
    }

    public class TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyLaunchAndRecoveryWinchStartControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "start":
                    msg.start = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType); }
        private readonly DataWriter<TowedSonarAssemblyLaunchAndRecoveryWinchStartControlType> _dataWriter;
    }

    public class TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeAdapter : ITransportAdapter
    {
        public TowedSonarAssemblyLaunchAndRecoverySlideStartControlTypeAdapter(DomainParticipant participant, string topicName)
        {
            var publisher = participant.CreatePublisher();
            var topic = participant.CreateTopic<TowedSonarAssemblyLaunchAndRecoverySlideStartControlType>(topicName);
            _dataWriter = publisher.CreateDataWriter(topic);
        }

        public object CreateSample()
        {
            return new TowedSonarAssemblyLaunchAndRecoverySlideStartControlType();
        }

        public void SetField(object sample, string fieldName, object value)
        {
            var msg = sample as TowedSonarAssemblyLaunchAndRecoverySlideStartControlType;

            switch (fieldName)
            {
                case "platformIDKey":
                    msg.platformIDKey = Convert.ToByte(value);
                    break;
                case "equipmentIDKey":
                    msg.equipmentIDKey = Convert.ToByte(value);
                    break;
                case "timeStamp":
                    var dateTime = Convert.ToDateTime(value);
                    msg.timeStamp = new DateTime()
                    {
                        year = Convert.ToInt16(dateTime.Year),
                        month = Convert.ToInt16(dateTime.Month),
                        day = Convert.ToInt16(dateTime.Day),
                        hour = Convert.ToInt16(dateTime.Hour),
                    };
                    break;
                case "start":
                    msg.start = Convert.ToByte(value);
                    break;
                case "commandID":
                    msg.commandID = Convert.ToByte(value);
                    break;
                default:
                    Debug.WriteLine("No Matched Field");
                    break;
            }
        }

        public void Write(object sample)
        {
            _dataWriter.Write(sample as TowedSonarAssemblyLaunchAndRecoverySlideStartControlType);
        }

        public Type TopicType { get => typeof(TowedSonarAssemblyLaunchAndRecoverySlideStartControlType); }
        private readonly DataWriter<TowedSonarAssemblyLaunchAndRecoverySlideStartControlType> _dataWriter;
    }
}