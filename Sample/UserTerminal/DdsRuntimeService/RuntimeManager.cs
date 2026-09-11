using System;
using System.Reflection;
using System.Reflection.PortableExecutable;
using Omg.Types.Dynamic;
using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;
using Rti.RequestReply;
using Rti.Types.Dynamic;
using Types;

namespace DdsServiceRuntime
{
    /// <summary>
    /// 외부 API는 Async로 통일
    /// 
    /// </summary>
    public class RuntimeManager
    {
        public RuntimeManager(string qosXmlUri, string participantName)
        {
            _provider = new Rti.Dds.Core.QosProvider(qosXmlUri);
            _participant = _provider.CreateParticipantFromConfig(participantName);

            // LoadTypeStringFromXml
            var structs = TypeLoader.LoadFromFile(qosXmlUri);
            foreach(var @struct in structs)
            {
                string typeName = $"{@struct.moduleName}::{@struct.name}";
                _typeSupportRegistry.Add(typeName, _provider.GetType(typeName));
            }
        }
        
        public Requester<DynamicData, DynamicData> RegisterRequester(RequestReplyChannelInfo channelInfo)
        {
            string requestTypeName = channelInfo.RequestTypeName;
            string replyTypeName = channelInfo.ReplyTypeName;

            _requesterRegistry.TryAdd(requestTypeName, new());
            _requesterRegistry[requestTypeName].TryAdd(replyTypeName, null);
            
            return _requesterRegistry[requestTypeName][replyTypeName] = 
                _participant.BuildRequester<DynamicData, DynamicData>()
                .WithTopicNames(requestTypeName, replyTypeName)
                .WithRequestDynamicType(_provider.GetType(requestTypeName))
                .WithReplyDynamicType(_provider.GetType(replyTypeName))
                .Create();
        }

        public Requester<DynamicData, DynamicData>? GetRequester(RequestReplyChannelInfo channelInfo)
        {
            if (_requesterRegistry[channelInfo.RequestTypeName].TryGetValue(channelInfo.ReplyTypeName, out var requester))
                return requester!;
            else 
                return null;
        }

        public Replier<DynamicData, DynamicData> RegisterReplier(RequestReplyChannelInfo channelInfo)
        {
            string requestTypeName = channelInfo.RequestTypeName;
            string replyTypeName = channelInfo.ReplyTypeName;

            _replierRegistry.TryAdd(requestTypeName, new());
            _replierRegistry[requestTypeName].TryAdd(replyTypeName, null);

            return _replierRegistry[requestTypeName][replyTypeName] = 
                _participant.BuildReplier<DynamicData, DynamicData>()
                .WithTopicNames(requestTypeName, replyTypeName)
                .WithRequestDynamicType(_provider.GetType(requestTypeName))
                .WithReplyDynamicType(_provider.GetType(replyTypeName))
                .Create();
        }

        public Replier<DynamicData, DynamicData>? GetReplier(RequestReplyChannelInfo channelInfo)
        {
            if (_replierRegistry[channelInfo.RequestTypeName].TryGetValue(channelInfo.ReplyTypeName, out var replier))
                return replier!;
            else
                return null;
        }

        public DynamicType GetDynamicType(string typeName)
        {
            return _typeSupportRegistry[typeName];
        }

        public DynamicData CreateSample(string typeName, List<(string, object)> @params)
        {
            var sampleType = (StructType)_typeSupportRegistry[typeName];
            var sample = new DynamicData(sampleType);

            foreach (var param in @params)
            {
                sample.SetAnyValue(param.Item1, param.Item2);
            }

            return sample;
        }

        private DomainParticipant _participant;
        private QosProvider _provider;
        private Dictionary<string, DynamicType> _typeSupportRegistry = new();
        private Dictionary<string, Dictionary<string, Requester<DynamicData, DynamicData>?>> _requesterRegistry = [];
        private Dictionary<string, Dictionary<string, Replier<DynamicData, DynamicData>?>> _replierRegistry = [];
    }
    public struct RequestReplyChannelInfo
    {
        public string RequestTypeName;
        public string ReplyTypeName;
    }
}
