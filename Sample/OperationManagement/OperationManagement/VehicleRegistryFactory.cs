using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Specification.Semantic.DTO;
using Specification.Node;
using Specification.Binding.DTO;

namespace Specification
{
    /// <summary>
    /// 특정 장비의 의미 기반 제어 명세를 정의한다.
    /// </summary>
    /// <remarks>
    /// ControlSpec은 운용 가능한 제어 행위와 입력 파라미터를 기술하며,
    /// 실제 메시지 포맷 및 전송 방식은 BindingSpec에서 정의된다.
    /// </remarks>
    public class VehicleRegistry
    {
        /// <summary>
        /// 제어 명령을 실행한다.
        /// </summary>
        /// <param name="vehicleSpecXmlPath">
        /// 사용자 입력으로 구성된 파라미터 집합.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// parameterSet 또는 context가 null인 경우.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// 현재 장비 상태에서 제어 수행이 불가능한 경우.
        /// </exception>
        public VehicleRegistry(string vehicleSpecXmlPath)
        {
            if (string.IsNullOrWhiteSpace(vehicleSpecXmlPath))
                throw new ArgumentException("xmlPath is required.", nameof(vehicleSpecXmlPath));

            if (File.Exists(vehicleSpecXmlPath) is false)
                throw new FileNotFoundException("XML is not found.", vehicleSpecXmlPath);

            using var specFile = File.OpenRead(vehicleSpecXmlPath);
            var serializer = new XmlSerializer(typeof(VehicleSpecDTO), "http://lignex1.com/mcmc2/umsSchema");
            var vehicleSpecDto = serializer.Deserialize(specFile) as VehicleSpecDTO;
            string dirPath = Path.GetDirectoryName(vehicleSpecXmlPath);

            foreach (var sensorSpecRef in vehicleSpecDto.SensorSpecRefs)
            {
                Sensors.Add(sensorSpecRef.id, new(sensorSpecRef, dirPath));
            }

            foreach (var platformSpecRef in vehicleSpecDto.PlatformSpecRefs)
            {
                Platform.Add(platformSpecRef.id, new(platformSpecRef, dirPath));
            }

            foreach (var payloadSpecRef in vehicleSpecDto.PayloadSpecRefs)
            {
                Payloads.Add(payloadSpecRef.id, new(payloadSpecRef, dirPath));
            }

            foreach (var commSpecRef in vehicleSpecDto.CommSpecRefs)
            {
                Comms.Add(commSpecRef.id, new(commSpecRef, dirPath));
            }
        }

        /// <summary>
        /// test test
        /// </summary>
        public Dictionary<string, SensorSpec> Sensors { get; init; } = new();

        /// <summary>
        /// test test
        /// </summary>
        public Dictionary<string, PlatformSpec> Platform { get; init; } = new();

        /// <summary>
        /// test test
        /// </summary>
        public Dictionary<string, PayloadSpec> Payloads { get; init; } = new();

        /// <summary>
        /// test test
        /// </summary>
        public Dictionary<string, CommSpec> Comms { get; init; } = new();
    }

    public class SensorSpec
    {
        public string ID { get; init; }

        public SensorSpec(SpecRefDTO specRef, string dirPath)
        {
            string semanticPath = $"{dirPath}\\{specRef.SemanticPath}";
            string bindingPath = $"{dirPath}\\{specRef.BindingPath}";

            if (string.IsNullOrWhiteSpace(semanticPath))
                throw new ArgumentException("xmlPath is required.", nameof(semanticPath));

            if (File.Exists(semanticPath) is false)
                throw new FileNotFoundException("XML is not found.", semanticPath);

            if (string.IsNullOrWhiteSpace(bindingPath))
                throw new ArgumentException("xmlPath is required.", nameof(bindingPath));

            if (File.Exists(bindingPath) is false)
                throw new FileNotFoundException("XML is not found.", bindingPath);

            ID = specRef.id;
            using var specFile = File.OpenRead(semanticPath);
            var specSerializer = new XmlSerializer(typeof(SensorSpecDTO), "http://lignex1.com/mcmc2/umsSchema");
            var sensorSpecDto = specSerializer.Deserialize(specFile) as SensorSpecDTO;

            using var bindingFile = File.OpenRead(bindingPath);
            var bindingSerializer = new XmlSerializer(typeof(SensorBindingDTO), "http://lignex1.com/mcmc2/umsSchema");
            var sensorBindingDto = bindingSerializer.Deserialize(bindingFile) as SensorBindingDTO;

            // 순서 중요
            ResolveSemantic(sensorSpecDto);
            ResolveBinding(sensorBindingDto);
            _semanticSpecs = _dictionaryBuilder.ToImmutable();
        }

        private void ResolveSemantic(SensorSpecDTO sensorSpecDTO)
        {
            var constantGroupNode = SpecNodeFactory.Instance.Create(sensorSpecDTO.ConstantSpecs, ID);
            var monitorGroupNode = SpecNodeFactory.Instance.Create(sensorSpecDTO.MonitorSpecs, ID);
            var controlGroupNode = SpecNodeFactory.Instance.Create(sensorSpecDTO.ControlSpecs, ID);
            var sensorProductGroupNode = SpecNodeFactory.Instance.Create(sensorSpecDTO.SensorProductSpecs, ID);
            
            _dictionaryBuilder.Add(constantGroupNode.FullId, constantGroupNode);
            foreach (var constantSpecDto in sensorSpecDTO.ConstantSpecs.Specs)
                RecursivelyResolveSemantic(constantSpecDto, constantGroupNode.FullId);

            _dictionaryBuilder.Add(monitorGroupNode.FullId, monitorGroupNode);
            foreach (var monitorSpecDto in sensorSpecDTO.MonitorSpecs.Specs)
                RecursivelyResolveSemantic(monitorSpecDto, monitorGroupNode.FullId);

            _dictionaryBuilder.Add(controlGroupNode.FullId, controlGroupNode);
            foreach (var controlSpecDto in sensorSpecDTO.ControlSpecs.Specs)
                RecursivelyResolveSemantic(controlSpecDto, controlGroupNode.FullId);

            _dictionaryBuilder.Add(sensorProductGroupNode.FullId, sensorProductGroupNode);
            foreach (var sensorProductSpecDto in sensorSpecDTO.SensorProductSpecs.Specs)
                RecursivelyResolveSemantic(sensorProductSpecDto, sensorProductGroupNode.FullId);
        }

        private void RecursivelyResolveSemantic(SemanticSpecBaseDTO spec, string fullParentId)
        {
            string fullId = $"{fullParentId}.{spec.id}";

            var node = SpecNodeFactory.Instance.Create(spec, fullParentId);

            _dictionaryBuilder.Add(fullId, node);

            var children = spec.GetChildren();
            if (children is null)
                return;

            foreach(var child in children)
            {
                RecursivelyResolveSemantic(child, fullId);
            }
        }

        private void ResolveBinding(SensorBindingDTO sensorBindingDTO)
        {
            string fullSemanticParentId = $"{ID}.control";
            foreach (var binding in sensorBindingDTO.Controls.Bindings)
            {
                string fullSemanticId = $"{fullSemanticParentId}.{binding.semanticId}";

                if (_dictionaryBuilder.TryGetValue(fullSemanticId, out SpecNode specNode))
                {
                    ControlSpecNode controlSpecNode = specNode as ControlSpecNode;
                    controlSpecNode.Binding = binding;
                }
            }
        }

        public SpecNode FindSpec(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            SpecNode node = null;
            _semanticSpecs.TryGetValue(id, out node);

            return node;
        }

        public void PrintAll(string filter = "")
        {
            var filterdSpecs = _semanticSpecs.ToImmutableSortedDictionary();
            if(filter != "")
            {
                filterdSpecs = _semanticSpecs.Where((k) => k.Value.ParentId == filter)
                    .OrderBy(k => k.Value.Id).ToImmutableSortedDictionary();
            }

            const int col1w = 40;
            const int col2w = 40;
            const int col3w = 40;
            const int col4w = 40;
            Console.WriteLine($"filter: {filter}");
            Console.WriteLine($"{"ID",-col1w} {"Parent", -col2w} {"CDM", -col3w} {"TYPE", -col4w}");
            foreach(var spec in filterdSpecs)
            {
                Console.WriteLine($"{spec.Value.Id,-col1w} {spec.Value.ParentId, -col2w} {spec.Value.Cdm,-col3w} {spec.Value.GetType(),-col4w}");
            }
        }

        public void PrintHierarchyAll()
        {
            Stack<string> parents = new();
            parents.Push(ID);
            Console.WriteLine(ID);

            foreach(var spec in _semanticSpecs.OrderBy(x => x.Key))
            {
                string fullParentId = spec.Value.ParentId;
                string onlyParentId = fullParentId[(fullParentId.LastIndexOf('.') + 1)..];
                string specId = spec.Value.Id;

                if(parents.Peek() != onlyParentId && !parents.Contains(onlyParentId))
                {
                    parents.Push(onlyParentId);
                }
                else if(parents.Peek() != onlyParentId && parents.Contains(onlyParentId))
                {
                    parents.Pop();
                }

                Console.Write(new string('\t', spec.Value.Level));
                Console.Write(specId);

                if (spec.Value.Cdm is not null)
                    Console.Write($" [{spec.Value.Cdm}]");
                Console.WriteLine();
            }
        }

        public IReadOnlyDictionary<string, SpecNode> SemanticSpecs { get => _semanticSpecs; }

        private ImmutableDictionary<string, SpecNode> _semanticSpecs = ImmutableDictionary<string, SpecNode>.Empty;
        ImmutableDictionary<string, SpecNode>.Builder _dictionaryBuilder = ImmutableDictionary.CreateBuilder<string, SpecNode>();
    }

    public class PlatformSpec
    {
        public PlatformSpec(SpecRefDTO specRef, string dirPath)
        {
            string refPath = $"{dirPath}\\{specRef.SemanticPath}";

            if (string.IsNullOrWhiteSpace(refPath))
                throw new ArgumentException("xmlPath is required.", nameof(refPath));

            if (File.Exists(refPath) is false)
                throw new FileNotFoundException("XML is not found.", refPath);

            using var specFile = File.OpenRead(refPath);
            var serializer = new XmlSerializer(typeof(PlatformSpecDTO), "http://lignex1.com/mcmc2/umsSchema");
            var spec = serializer.Deserialize(specFile) as PlatformSpecDTO;
        }

        public SpecNode FindSpec(string id)
        {
            return _specs[id];
        }

        private Dictionary<string, SpecNode> _specs;
    }

    public class PayloadSpec
    {
        public PayloadSpec(SpecRefDTO specRef, string dirPath)
        {
            string refPath = $"{dirPath}\\{specRef.SemanticPath}";

            if (string.IsNullOrWhiteSpace(refPath))
                throw new ArgumentException("xmlPath is required.", nameof(refPath));

            if (File.Exists(refPath) is false)
                throw new FileNotFoundException("XML is not found.", refPath);

            using var specFile = File.OpenRead(refPath);
            var serializer = new XmlSerializer(typeof(PayloadSpecDTO), "http://lignex1.com/mcmc2/umsSchema");
            var spec = serializer.Deserialize(specFile) as PayloadSpecDTO;
        }

        public SpecNode FindSpec(string id)
        {
            return _specs[id];
        }

        private Dictionary<string, SpecNode> _specs;
    }

    public class CommSpec
    {
        public CommSpec(SpecRefDTO specRef, string dirPath)
        {
            string refPath = $"{dirPath}\\{specRef.SemanticPath}";

            if (string.IsNullOrWhiteSpace(refPath))
                throw new ArgumentException("xmlPath is required.", nameof(refPath));

            if (File.Exists(refPath) is false)
                throw new FileNotFoundException("XML is not found.", refPath);

            using var specFile = File.OpenRead(refPath);
            var serializer = new XmlSerializer(typeof(CommSpecDTO), "http://lignex1.com/mcmc2/umsSchema");
            var spec = serializer.Deserialize(specFile) as CommSpecDTO;
        }

        public SpecNode FindSpec(string id)
        {
            return _specs[id];
        }

        private Dictionary<string, SpecNode> _specs;
    }

    
}
