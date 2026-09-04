'use strict';

const OM_DDS_TYPES = {
  controlSpecListRequest: 'Messages::OperationManagement::ControlSpecListRequest',
  controlSpecListReply: 'Messages::OperationManagement::ControlSpecListReply',
  controlExecutionRequest: 'Messages::OperationManagement::ControlExecutionRequest',
  controlExecutionReply: 'Messages::OperationManagement::ControlExecutionReply',
  monitorSpecListRequest: 'Messages::OperationManagement::MonitorSpecListRequest',
  monitorSpecListReply: 'Messages::OperationManagement::MonitorSpecListReply',
};

const OPERATION_STATES = {
  finished: 'Finished',
  processing: 'Processing',
  failed: 'Failed',
};

const FIELD_NAMES = ['Field', 'FixedField', 'DerivedField', 'ArrayField', 'PackedField'];
const CHANNEL_NAMES = ['DDSChannel', 'TCPChannel', 'UDPChannel', 'RTPChannel', 'RS422Channel', 'Channel'];

const SUPPORTED_DERIVED = new Set([
  'ByteSumModulo65536LE',
  'ArrayLengthUInt8',
  'ArrayLengthUInt16',
  'Bit16',
  'Bit17',
  'DateToUInt8',
  'DecimalToInt32Scale10000000LE',
  'DecimalToUInt16Scale10LE',
  'Float32LE',
  'HourToUInt8',
  'IncrementingUInt32LE',
  'Int8',
  'Int32Scale10000000ToDecimalLE',
  'MillisecondToUInt16LE',
  'MinuteToUInt8',
  'MonthToUInt8',
  'PackDataTypeBits',
  'PackRfConfMode',
  'PackRfConfPower',
  'Rf3DataTypeFromBits11To15',
  'SecondToUInt8',
  'TargetAuvToInformation1',
  'TargetAuvToInformation2',
  'TargetAuvToInformation3',
  'UInt16LE',
  'UInt16Scale10ToDecimalLE',
  'UInt32LE',
  'UInt32Scale10ToDecimalLE',
  'UInt8Array16',
  'UnpackRfConfMode',
  'UnpackRfConfPower',
  'UuidToUInt8Array16',
  'YearToUInt16LE',
]);

const state = {
  bundle: null,
  loadToken: 0,
  cdmView: 'principles',
  activeDeviceRef: null,
  selectedFeature: null,
  selectedFile: null,
  selectedFiles: new Set(),
  fileSelectionAnchor: null,
  selectedRefs: new Set(),
  selectedRef: null,
  refSelectionAnchor: null,
  selectedFeatures: new Set(),
  featureSelectionAnchor: null,
  demo: {},
  hmiKind: 'Control',
  hmiDeviceKey: '',
  hmiActionId: '',
  hmiBindingKey: '',
  hmiSourceKey: 'semantic',
};

const ids = [
  'sourceDot', 'sourceFolderName', 'sourceStatusText',
  'xmlFileCount', 'xsdFileCount', 'idlFileCount', 'controlCount', 'monitorCount', 'productCount', 'diagnosticCount',
  'folderInput',
  'modelEmptyState', 'modelWorkspace', 'relationshipMap', 'schemaRail',
  'schemaSummary', 'schemaList', 'devicePicker', 'devicePickerButton', 'devicePickerName', 'devicePickerMeta',
  'devicePopover', 'deviceSearch', 'deviceList', 'deviceDetailName',
  'selectionFileCount', 'selectionControlCount', 'selectionMonitorCount', 'selectionProductCount',
  'loadedFileCount', 'fileList', 'controlSearch', 'controlList', 'modelDetail', 'bindingDetail', 'diagnosticList',
  'cdmDocument', 'cdmFileButtons', 'cdmReferenceHint',
  'hmiEmptyState', 'hmiWorkspace', 'presetPicker', 'presetSearch', 'presetSelection', 'presetList', 'hmiState', 'terminalLog', 'terminalForm',
  'terminalInput', 'draftControlId', 'sendDraftButton', 'argumentChips', 'omState',
  'routePolicy', 'pipeline', 'bindingSummary', 'resultBanner', 'payloadTableBody',
  'copyPayloadButton', 'busTimeline', 'resetDemoButton', 'hmiCatalogSearch', 'hmiCatalog',
];
const dom = Object.fromEntries(ids.map((id) => [id, document.getElementById(id)]));

function el(tag, className = '', text) {
  const node = document.createElement(tag);
  if (className) node.className = className;
  if (text !== undefined) node.textContent = String(text);
  return node;
}

function clear(node) {
  if (node) node.replaceChildren();
}

function direct(node, name) {
  if (!node) return [];
  return [...node.children].filter((child) => !name || child.localName === name);
}

function first(node, name) {
  return direct(node, name)[0] || null;
}

function all(node, name) {
  if (!node) return [];
  return [...node.getElementsByTagNameNS('*', name)];
}

function value(node) {
  return node ? node.textContent.trim() : '';
}

function attr(node, name) {
  return node ? node.getAttribute(name) || '' : '';
}

function basename(path) {
  return String(path || '').replace(/\\/g, '/').split('/').pop();
}

function extension(name) {
  return basename(name).toLowerCase().split('.').pop();
}

function normalizePath(path) {
  const parts = [];
  for (const part of String(path || '').replace(/\\/g, '/').split('/')) {
    if (!part || part === '.') continue;
    if (part === '..') parts.pop();
    else parts.push(part);
  }
  return parts.join('/');
}

function dirname(path) {
  const normalized = normalizePath(path);
  const index = normalized.lastIndexOf('/');
  return index >= 0 ? normalized.slice(0, index) : '';
}

function relativePath(basePath, reference) {
  return normalizePath([dirname(basePath), reference].filter(Boolean).join('/'));
}

function delay(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function commentBefore(node) {
  let previous = node ? node.previousSibling : null;
  while (previous && previous.nodeType === Node.TEXT_NODE) previous = previous.previousSibling;
  return previous && previous.nodeType === Node.COMMENT_NODE
    ? previous.nodeValue.trim().replace(/\s+/g, ' ')
    : '';
}

function makeDl(rows) {
  const dl = el('dl');
  for (const [key, rawValue] of rows) {
    const wrap = el('div');
    wrap.append(el('dt', '', key), el('dd', '', rawValue === '' || rawValue == null ? '—' : rawValue));
    dl.append(wrap);
  }
  return dl;
}

function setSource(status, title, detail) {
  if (dom.sourceFolderName) dom.sourceFolderName.textContent = title;
  if (dom.sourceStatusText) dom.sourceStatusText.textContent = detail;
  if (dom.sourceDot) {
    dom.sourceDot.style.background = status === 'ok'
      ? 'var(--green)'
      : status === 'error'
        ? 'var(--red)'
        : 'var(--amber)';
  }
}

function setState(node, text, type = '') {
  if (!node) return;
  node.textContent = text;
  node.dataset.state = type;
}

function parserError(doc) {
  return doc.getElementsByTagName('parsererror')[0] || null;
}

function profileType(localName) {
  if (['QuantityProfile', 'QuantitySpec', 'QuantityValueSetSpec', 'QuantityResult'].includes(localName)) return 'number';
  if (['BooleanProfile', 'BooleanSpec', 'BooleanResult'].includes(localName)) return 'boolean';
  if (['CollectionProfile', 'CollectionSpec'].includes(localName)) return 'collection';
  if (['TextProfile', 'TextSpec'].includes(localName)) return 'text';
  if (['ValueSetProfile', 'ValueSetSpec', 'ValueSetResult', 'HealthStatusSetSpec'].includes(localName)) return 'valueSet';
  if (localName === 'Result') return 'result';
  return 'unknown';
}

function lowerFirst(text) {
  return text ? text[0].toLowerCase() + text.slice(1) : 'value';
}

function semanticRefId(raw) {
  const text = String(raw || '').trim();
  const hash = text.lastIndexOf('#');
  return hash >= 0 ? text.slice(hash + 1) : text;
}

// Binding의 semantic_id가 "control" 또는 "SideScanSonarSemantic.xml#control" 어느 쪽이어도
// 동일한 Semantic group ID로 해석한다.
function flattenFields(fields) {
  const output = [];
  for (const field of fields || []) {
    output.push(field);
    output.push(...flattenFields(field.children));
  }
  return output;
}

function channelProtocol(channelNode, fileName = '') {
  if (!channelNode) return 'UNKNOWN';
  const local = channelNode.localName;
  if (local === 'DDSChannel') return 'DDS';
  if (local === 'TCPChannel') return 'TCP';
  if (local === 'UDPChannel') return 'UDP';
  if (local === 'RTPChannel') return 'RTP';
  if (local === 'RS422Channel') return 'RS422';

  const file = fileName.toLowerCase();
  if (file.includes('ucd') || file.includes('rs422')) return 'RS422';
  if (file.includes('tcp') || file.includes('rf')) return 'TCP';
  if (file.includes('udp')) return 'UDP';
  return 'DDS';
}

function channelIdentity(channel) {
  if (!channel) return '미정';
  if (channel.protocol === 'DDS') return channel.topicName || 'Topic 미정';
  if (channel.protocol === 'RTP') return 'RTP';
  return channel.infoCode || channel.messageType || 'Info Code 없음';
}

function channelRows(channel) {
  if (!channel) return [['Protocol', 'UNKNOWN']];
  if (channel.protocol === 'DDS') {
    return [
      ['Protocol', 'DDS'],
      ['Topic', channel.topicName],
      ['Type', channel.typeName],
    ];
  }
  if (channel.protocol === 'RTP') return [['Protocol', 'RTP']];
  return [
    ['Protocol', channel.protocol],
    ['Info Code', channel.infoCode || '없음'],
    ['Message Type', channel.messageType],
  ];
}

function channelSummary(channel) {
  if (!channel) return 'UNKNOWN';
  if (channel.protocol === 'DDS') return `${channel.protocol} · ${channel.topicName || '—'} · ${channel.typeName || '—'}`;
  if (channel.protocol === 'RTP') return 'RTP';
  return `${channel.protocol} · ${channel.infoCode || 'Info Code 없음'} · ${channel.messageType || '—'}`;
}

const tabButtons = [...document.querySelectorAll('[role=tab]')];
const panels = [...document.querySelectorAll('[role=tabpanel]')];

function activateTab(name, focus = false) {
  for (const button of tabButtons) {
    const active = button.dataset.tab === name;
    button.classList.toggle('is-active', active);
    button.setAttribute('aria-selected', String(active));
    button.tabIndex = active ? 0 : -1;
    if (active && focus) button.focus();
  }
  for (const panel of panels) panel.hidden = panel.dataset.panel !== name;
  history.replaceState(null, '', `#${name}`);
}

function handleTabKey(event) {
  const current = tabButtons.indexOf(event.currentTarget);
  let next = current;
  if (event.key === 'ArrowRight') next = (current + 1) % tabButtons.length;
  else if (event.key === 'ArrowLeft') next = (current - 1 + tabButtons.length) % tabButtons.length;
  else if (event.key === 'Home') next = 0;
  else if (event.key === 'End') next = tabButtons.length - 1;
  else return;
  event.preventDefault();
  activateTab(tabButtons[next].dataset.tab, true);
}

function appendInline(node, text) {
  const parts = String(text).split(/(`[^`]+`)/g);
  for (const part of parts) {
    if (part.startsWith('`') && part.endsWith('`')) node.append(el('code', '', part.slice(1, -1)));
    else node.append(document.createTextNode(part));
  }
}

function renderPrinciples() {
  clear(dom.cdmDocument);
  const head = el('div', 'document-header');
  head.append(el('span', '', 'CDM RULES'), el('h2', '', 'CDM 적용 기준'));
  const grid = el('div', 'principle-grid');
  const sections = [
    {
      title: 'Semantic',
      rules: [
        'Control · 원격 제어와 요청 기능',
        'Monitor · 지속적으로 감시하는 정보',
        'Parameters · 전송방식과 독립된 입력 의미',
        'Reply · 요청 검증 및 명령 수락 결과',
      ],
    },
    {
      title: 'Binding',
      rules: [
        'ControlBinding / MonitorBinding · 기능과 메시지 연결',
        'TCP / UDP / RS422 · infoCode와 messageType',
        'DDS · topicName과 typeName',
        'Header·ACK·checksum·reserved · Binding 전용',
      ],
    },
  ];
  for (const item of sections) {
    const section = el('section');
    section.append(el('h3', '', item.title));
    const list = el('ul');
    item.rules.forEach((rule) => list.append(el('li', '', rule)));
    section.append(list);
    grid.append(section);
  }
  dom.cdmDocument.append(head, grid);
}

function markdownTable(lines, start) {
  const table = el('table');
  const rows = [];
  let index = start;
  while (index < lines.length && /^\s*\|/.test(lines[index])) {
    rows.push(lines[index].trim().replace(/^\||\|$/g, '').split('|').map((cell) => cell.trim()));
    index += 1;
  }
  if (rows.length < 2 || !rows[1].every((cell) => /^:?-+:?$/.test(cell))) return null;

  const thead = el('thead');
  const headerRow = el('tr');
  rows[0].forEach((cell) => {
    const th = el('th');
    appendInline(th, cell);
    headerRow.append(th);
  });
  thead.append(headerRow);

  const tbody = el('tbody');
  for (const row of rows.slice(2)) {
    const bodyRow = el('tr');
    row.forEach((cell) => {
      const td = el('td');
      appendInline(td, cell);
      bodyRow.append(td);
    });
    tbody.append(bodyRow);
  }
  table.append(thead, tbody);
  return { node: table, next: index };
}

function renderMarkdown(text) {
  clear(dom.cdmDocument);
  const article = el('div', 'markdown-body');
  const lines = String(text).replace(/\r/g, '').split('\n');
  let index = 0;
  while (index < lines.length) {
    const line = lines[index];
    if (/^~~~|^```/.test(line)) {
      const fence = line.slice(0, 3);
      const code = [];
      index += 1;
      while (index < lines.length && !lines[index].startsWith(fence)) code.push(lines[index++]);
      index += 1;
      const pre = el('pre');
      pre.append(el('code', '', code.join('\n')));
      article.append(pre);
      continue;
    }

    const table = markdownTable(lines, index);
    if (table) {
      article.append(table.node);
      index = table.next;
      continue;
    }

    const heading = line.match(/^(#{1,4})\s+(.+)/);
    if (heading) {
      const node = el(`h${heading[1].length}`);
      appendInline(node, heading[2]);
      article.append(node);
      index += 1;
      continue;
    }

    if (/^[-*]\s+/.test(line)) {
      const list = el('ul');
      while (index < lines.length && /^[-*]\s+/.test(lines[index])) {
        const item = el('li');
        appendInline(item, lines[index].replace(/^[-*]\s+/, ''));
        list.append(item);
        index += 1;
      }
      article.append(list);
      continue;
    }

    if (/^\d+\.\s+/.test(line)) {
      const list = el('ol');
      while (index < lines.length && /^\d+\.\s+/.test(lines[index])) {
        const item = el('li');
        appendInline(item, lines[index].replace(/^\d+\.\s+/, ''));
        list.append(item);
        index += 1;
      }
      article.append(list);
      continue;
    }

    if (!line.trim()) {
      index += 1;
      continue;
    }

    const parts = [line.trim()];
    index += 1;
    while (
      index < lines.length
      && lines[index].trim()
      && !/^(#|[-*]\s|\d+\.\s|\||~~~|```)/.test(lines[index])
    ) parts.push(lines[index++].trim());
    const paragraph = el('p');
    appendInline(paragraph, parts.join(' '));
    article.append(paragraph);
  }
  dom.cdmDocument.append(article);
}

async function readFolder(fileList) {
  const token = ++state.loadToken;
  const files = [...fileList];
  if (!files.length) return;
  const folder = files[0].webkitRelativePath.split('/')[0] || '선택 폴더';
  setSource('loading', folder, '파일 읽기·XML 파싱 중');

  const entries = [];
  for (const file of files) {
    if (token !== state.loadToken) return;
    const ext = extension(file.name);
    if (!['xml', 'xsd', 'md', 'idl'].includes(ext)) continue;
    entries.push({
      name: file.name,
      path: normalizePath(file.webkitRelativePath || file.name),
      text: await file.text(),
    });
  }
  if (token === state.loadToken) ingest(entries, folder);
}

function parseFiles(entries, diagnostics) {
  return entries.map((entry, index) => {
    const path = normalizePath(entry.path || entry.name || `file-${index}`);
    const ext = extension(entry.name || path);
    const file = {
      ...entry,
      name: entry.name || basename(path),
      path,
      key: `${path.toLowerCase()}#${index}`,
      format: ext.toUpperCase(),
      kind: ext === 'md' ? 'Markdown' : ext === 'idl' ? 'IDL' : 'Unknown',
      doc: null,
    };

    if (ext === 'xml' || ext === 'xsd') {
      file.doc = new DOMParser().parseFromString(entry.text, 'application/xml');
      const bad = parserError(file.doc);
      if (bad) {
        file.error = bad.textContent.trim();
        diagnostics.push({ level: 'error', code: 'XML_PARSE', message: `${path} 문법 오류` });
        return file;
      }
      const root = file.doc.documentElement.localName;
      if (ext === 'xsd') file.kind = 'XSD';
      else if (root === 'VehicleSpec') file.kind = 'Specification';
      else if (root.endsWith('Binding')) file.kind = 'Binding';
      else if (root.endsWith('Spec')) file.kind = 'Semantic';
      else file.kind = 'XML';
      file.root = root;
    }
    return file;
  });
}

function parseSchema(file) {
  const root = file.doc.documentElement;
  return {
    file: file.name,
    path: file.path,
    namespace: attr(root, 'targetNamespace'),
    includes: direct(root)
      .filter((node) => ['include', 'import'].includes(node.localName))
      .map((node) => attr(node, 'schemaLocation')),
    roots: direct(root, 'element').map((node) => attr(node, 'name')),
    complexTypes: direct(root, 'complexType').length,
    simpleTypes: direct(root, 'simpleType').length,
  };
}

function resolveReferencedFile(files, vehicleFile, reference) {
  const exactPath = relativePath(vehicleFile.path, reference).toLowerCase();
  const exact = files.find((file) => file.path.toLowerCase() === exactPath);
  if (exact) return exact;

  const sameBase = files.filter((file) => file.name.toLowerCase() === basename(reference).toLowerCase());
  if (sameBase.length === 1) return sameBase[0];
  if (sameBase.length > 1) {
    const sameFolder = sameBase.find((file) => dirname(file.path) === dirname(vehicleFile.path));
    return sameFolder || sameBase[0];
  }
  return null;
}

function parseProfile(node) {
  const range = direct(node, 'Range')[0];
  const unit = direct(node, 'Unit')[0];
  const resolution = direct(node, 'Resolution')[0];
  const valuesNode = direct(node, 'Values')[0];
  const required = attr(node, 'required');
  return {
    kind: node.localName,
    name: attr(node, 'name'),
    type: profileType(node.localName),
    cdm: attr(node, 'cdm'),
    required: required ? required !== 'false' : attr(node, 'minOccurs') !== '0',
    defaultValue: attr(node, 'default'),
    unit: value(unit),
    min: range && attr(range, 'min') !== '' ? Number(attr(range, 'min')) : null,
    max: range && attr(range, 'max') !== '' ? Number(attr(range, 'max')) : null,
    resolution: resolution ? Number(value(resolution)) : null,
    minOccurs: attr(node, 'minOccurs'),
    maxOccurs: attr(node, 'maxOccurs'),
    description: commentBefore(node),
    values: valuesNode
      ? direct(valuesNode, 'Value').map((item) => ({
        value: attr(item, 'value') || value(item),
        name: attr(item, 'name'),
        cdm: attr(item, 'cdm'),
      }))
      : [],
    children: direct(node)
      .filter((child) => /(?:Profile|Spec|Result)$/.test(child.localName) || child.localName === 'Results')
      .map(parseProfile),
  };
}

function parseControlGroup(ref, file) {
  const group = all(file.doc, 'ControlSpecs')[0];
  if (!group) return [];
  return direct(group).filter((node) => ['ControlSpec', 'SetPointSpec'].includes(node.localName)).map((node) => {
      const targetNode = first(node, 'Target');
      const parametersNode = first(node, 'Parameters');
      const replies = direct(node, 'Reply').map((reply) => ({
        cdm: attr(reply, 'cdm'),
        bindRef: semanticRefId(attr(reply, 'bindRef')),
        required: attr(reply, 'required') !== 'false',
        timeout: attr(reply, 'timeout'),
        results: direct(first(reply, 'Results')).map(parseProfile),
      }));
      const target = targetNode
        ? { kind: 'Target', type: 'unknown', cdm: attr(targetNode, 'cdm'), required: true, defaultValue: '', unit: '', min: null, max: null, resolution: null, description: commentBefore(targetNode) }
        : null;
      return {
        kind: 'Control',
        ref,
        groupId: attr(group, 'id'),
        semanticId: attr(node, 'id'),
        publicId: `${ref.id}.${attr(group, 'id')}.${attr(node, 'id')}`,
        name: attr(node, 'name') || attr(node, 'id'),
        cdm: attr(node, 'cdm'),
        description: commentBefore(node),
        target,
        parameters: parametersNode ? direct(parametersNode).map(parseProfile) : [],
        preconditions: direct(node, 'Precondition').map((item) => ({
          cdm: attr(item, 'cdm'),
          description: commentBefore(item),
        })),
        outputs: [],
        replies,
        bindings: [],
        inputs: [],
        sourceFile: file.name,
        sourcePath: file.path,
      };
    });
}

function parseMonitorGroup(ref, file) {
  const group = all(file.doc, 'MonitorSpecs')[0];
  if (!group) return [];
  return direct(group, 'GroupSpec').map((node) => ({
    kind: 'Monitor',
    ref,
    groupId: attr(group, 'id'),
    semanticId: attr(node, 'id'),
    publicId: `${ref.id}.${attr(group, 'id')}.${attr(node, 'id')}`,
    name: attr(node, 'name') || attr(node, 'id'),
    cdm: attr(node, 'cdm'),
    description: commentBefore(node),
    target: null,
    parameters: [],
    outputs: direct(node).filter((child) => child.localName.endsWith('Spec')).map(parseProfile),
    replies: [],
    bindings: [],
    inputs: [],
    sourceFile: file.name,
    sourcePath: file.path,
  }));
}

function parseProductGroup(ref, file) {
  const group = all(file.doc, 'SensorProductSpecs')[0];
  if (!group) return [];
  const productNames = new Set(['ProductStreamSpec', 'ProductFileSpec', 'ProductFrameSpec']);
  return direct(group)
    .filter((node) => productNames.has(node.localName))
    .map((node) => {
      const productKind = first(node, 'ProductKind');
      const processingState = first(node, 'ProcessingState');
      return {
        kind: 'Product',
        ref,
        groupId: attr(group, 'id'),
        semanticId: attr(node, 'id'),
        publicId: `${ref.id}.${attr(group, 'id')}.${attr(node, 'id')}`,
        name: attr(node, 'name') || attr(node, 'id'),
        cdm: attr(node, 'cdm'),
        description: commentBefore(node),
        productType: node.localName,
        productKind: attr(productKind, 'cdm') || value(productKind),
        processingState: attr(processingState, 'cdm') || value(processingState),
        target: null,
        parameters: [],
        outputs: [],
        replies: [],
        bindings: [],
        inputs: [],
        sourceFile: file.name,
        sourcePath: file.path,
      };
    });
}

function parseMaps(node) {
  return direct(node)
    .filter((child) => ['ValueMap', 'SourceValueMap'].includes(child.localName))
    .flatMap((holder) => direct(holder, 'Map').map((item) => ({
      kind: holder.localName,
      cdm: attr(item, 'cdm'),
      sourceValue: attr(item, 'sourceValue'),
      value: attr(item, 'value'),
    })));
}

function parseField(node) {
  const element = first(node, 'Element');
  const bitMembers = node.localName === 'PackedField'
    ? direct(node, 'BitMember').map((member) => ({
      kind: 'BitMember',
      name: attr(member, 'name'),
      cdm: attr(member, 'cdm'),
      value: attr(member, 'fixedValue'),
      fixedValue: attr(member, 'fixedValue'),
      sourceFields: attr(member, 'sourceField').split(',').map((item) => item.trim()).filter(Boolean),
      converter: attr(member, 'converter'),
      offset: attr(member, 'offset'),
      width: attr(member, 'width') || '1',
      maps: parseMaps(member),
      description: commentBefore(member),
      children: [],
    }))
    : [];
  return {
    kind: node.localName,
    name: attr(node, 'name'),
    cdm: attr(node, 'cdm'),
    value: attr(node, 'value'),
    fixedValue: attr(node, 'fixedValue'),
    defaultValue: attr(node, 'defaultValue'),
    converter: attr(node, 'converter'),
    sourceFields: attr(node, 'sourceField').split(',').map((item) => item.trim()).filter(Boolean),
    dataType: attr(node, 'dataType'),
    scale: attr(node, 'scale'),
    length: attr(node, 'length'),
    format: attr(node, 'format'),
    width: attr(node, 'width'),
    byteOrder: attr(node, 'byteOrder'),
    expectedMask: attr(node, 'expectedMask'),
    expectedValue: attr(node, 'expectedValue'),
    inputUnit: attr(node, 'inputUnit'),
    outputUnit: attr(node, 'outputUnit'),
    minOccurs: attr(node, 'minOccurs'),
    maxOccurs: attr(node, 'maxOccurs'),
    maps: parseMaps(node),
    description: commentBefore(node),
    children: bitMembers.length
      ? bitMembers
      : element
      ? direct(element).filter((child) => FIELD_NAMES.includes(child.localName)).map(parseField)
      : [],
  };
}

function parseChannel(node, fileName) {
  const channelNode = direct(node).find((child) => CHANNEL_NAMES.includes(child.localName));
  if (!channelNode) return null;
  const protocol = channelProtocol(channelNode, fileName);
  const legacy = channelNode.localName === 'Channel';
  return {
    protocol,
    elementName: channelNode.localName,
    legacy,
    infoCode: attr(channelNode, 'infoCode') || (legacy && protocol !== 'DDS' ? attr(channelNode, 'topicName') : ''),
    messageType: attr(channelNode, 'messageType') || (legacy && protocol !== 'DDS' ? attr(channelNode, 'typeName') : ''),
    topicName: attr(channelNode, 'topicName'),
    typeName: attr(channelNode, 'typeName'),
  };
}

function parseMessage(node, fileName) {
  return {
    channel: parseChannel(node, fileName),
    fields: direct(node).filter((child) => FIELD_NAMES.includes(child.localName)).map(parseField),
  };
}

function parseBindingGroup(ref, file, kind) {
  const groupName = kind === 'Control' ? 'Controls' : 'Monitors';
  const nodeNames = kind === 'Control'
    ? ['ControlBinding', 'ControlBindingDDS']
    : ['MonitorBinding', 'MonitorBindingDDS'];
  const group = all(file.doc, groupName)[0];
  if (!group) return [];

  return direct(group)
    .filter((node) => nodeNames.includes(node.localName))
    .map((node) => {
      const base = parseMessage(node, file.name);
      const replies = kind === 'Control'
        ? direct(node, 'Reply').map((reply) => ({
          ...parseMessage(reply, file.name),
          semanticId: semanticRefId(attr(reply, 'semantic_id')),
          required: attr(reply, 'required') !== 'false',
          timeout: attr(reply, 'timeout'),
        }))
        : [];
      return {
        ...base,
        kind,
        groupId: semanticRefId(attr(group, 'semantic_id')),
        semanticId: semanticRefId(attr(node, 'semantic_id')),
        sourceFile: file.name,
        sourcePath: file.path,
        transport: base.channel ? base.channel.protocol : 'UNKNOWN',
        replies,
        key: `${ref.id}|${kind}|${semanticRefId(attr(group, 'semantic_id'))}|${semanticRefId(attr(node, 'semantic_id'))}|${file.path}`,
      };
    });
}

function parseProductBindingGroup(ref, file) {
  const group = all(file.doc, 'SensorProducts')[0];
  if (!group) return [];

  return direct(group, 'ProductBinding').map((node) => {
    const base = parseMessage(node, file.name);
    const matchFields = direct(node, 'MatchField').map((match) => ({
      name: attr(match, 'name'),
      mask: attr(match, 'mask'),
      value: attr(match, 'value'),
      description: commentBefore(match),
    }));
    const groupId = semanticRefId(attr(group, 'semantic_id'));
    const semanticId = semanticRefId(attr(node, 'semantic_id'));
    return {
      ...base,
      kind: 'Product',
      groupId,
      semanticId,
      sourceFile: file.name,
      sourcePath: file.path,
      transport: base.channel ? base.channel.protocol : 'UNKNOWN',
      replies: [],
      matchFields,
      key: `${ref.id}|Product|${groupId}|${semanticId}|${file.path}`,
    };
  });
}

function aliasFor(input, fields, used) {
  let key = '';
  const exactNames = [...new Set(fields
    .filter((field) => field.cdm === input.cdm && ['Field', 'ArrayField', 'BitMember'].includes(field.kind))
    .map((field) => field.name)
    .filter(Boolean))];
  if (exactNames.length === 1) key = exactNames[0];

  if (!key) {
    const leaf = input.cdm.split('.').pop();
    const suffixNames = [...new Set(fields
      .filter((field) => ['Field', 'ArrayField', 'BitMember'].includes(field.kind)
        && field.name.toLowerCase().endsWith(leaf.toLowerCase()))
      .map((field) => field.name))];
    if (suffixNames.length === 1) key = suffixNames[0];
  }

  if (!key) {
    const fallback = {
      'Platform.Motion.Speed': 'remoteSpeed',
      'Platform.Motion.Direction': 'remoteDirection',
      'Autonomy.EmergencyReturn.Position.Latitude': 'emergencyLatitude',
      'Autonomy.EmergencyReturn.Position.Longitude': 'emergencyLongitude',
      'Equipment.Light.Brightness': 'brightness',
      'MissionPlan.Waypoints': 'waypoints',
      'RecordedData.FileRange.First': 'fileFirst',
      'RecordedData.FileRange.Final': 'fileFinal',
    };
    key = fallback[input.cdm] || lowerFirst(input.cdm.split('.').pop());
  }

  if (used.has(key)) {
    const parts = input.cdm.split('.');
    key = lowerFirst(parts.slice(-2).join(''));
  }
  if (used.has(key)) key = input.cdm;
  used.add(key);
  return key;
}

function attachBindings(ref, diagnostics) {
  const variants = ref.bindingFiles.flatMap((file) => [
    ...parseBindingGroup(ref, file, 'Control'),
    ...parseBindingGroup(ref, file, 'Monitor'),
    ...parseProductBindingGroup(ref, file),
  ]);

  for (const variant of variants) {
    const action = ref.actions.find((item) => (
      item.kind === variant.kind
      && item.groupId === variant.groupId
      && item.semanticId === variant.semanticId
    ));
    if (action) action.bindings.push(variant);
    else diagnostics.push({
      level: 'warning',
      code: 'ORPHAN_BINDING',
      message: `${variant.sourceFile}의 ${variant.kind} ${variant.semanticId}가 Semantic에 없습니다.`,
    });
  }

  for (const action of ref.actions) {
    if (action.kind === 'Control') {
      const fields = action.bindings.flatMap((binding) => flattenFields(binding.fields));
      const used = new Set();
      if (action.target) {
        const targetField = fields.find((field) => field.cdm === action.target.cdm && ['Field', 'ArrayField'].includes(field.kind));
        action.target = {
          ...action.target,
          key: targetField?.name || 'targetId',
          type: targetField && /UInt|Int|Float|Scale|Bit/i.test(targetField.converter) ? 'number' : action.target.type,
          autoSupplied: true,
        };
        used.add(action.target.key);
      }
      action.inputs = action.parameters.map((input) => {
        const exactField = fields.find((field) => field.cdm === input.cdm && ['Field', 'ArrayField'].includes(field.kind));
        let type = input.type;
        if (type === 'unknown') {
          if (/Communication\.RF\.Configuration\.(?:OperationMode|.*\.Power)$/.test(input.cdm)) type = 'text';
          else if (/Identifier\.Numeric|Latitude|Longitude|Depth|Speed|Bearing|Range|Heading|Pitch|Command|Reason|Type|Brightness|Interval|Channel|TimeSlot/i.test(input.cdm)) type = 'number';
          else if (exactField && /UInt|Int|Float|Scale|Bit/i.test(exactField.converter)) type = 'number';
        }
        return { ...input, type, key: aliasFor(input, fields, used) };
      });
    }

    if (!action.bindings.length) diagnostics.push({
      level: 'warning',
      code: 'NO_BINDING',
      message: `${action.publicId} Binding이 없습니다.`,
    });
  }
}

function vehicleIdentity(vehicleFile) {
  const identification = all(vehicleFile?.doc, 'Identification')[0];
  return {
    targetId: value(first(identification, 'ID')) || basename(vehicleFile?.path || 'UNKNOWN'),
    targetName: value(first(identification, 'Name')) || basename(vehicleFile?.path || 'UNKNOWN'),
    sourcePath: vehicleFile?.path || '',
  };
}

function actionTarget(action) {
  return vehicleIdentity(action?.ref?.vehicleFile);
}

function controlsForVehicle(vehicleFile) {
  if (!state.bundle) return [];
  return state.bundle.controls.filter((control) => control.ref.vehicleFile.path === vehicleFile.path);
}

function serialiseIdlValue(value) {
  if (value === undefined || value === null) return '';
  return typeof value === 'string' ? value : JSON.stringify(value);
}

function idlText(value, maxLength = 0) {
  const text = value === undefined || value === null ? '' : String(value);
  return maxLength > 0 ? text.slice(0, maxLength) : text;
}

function controlParamPayload(input, fieldValue = '') {
  return {
    fieldName: idlText(input.key, 100),
    cdm: idlText(input.cdm, 100),
    dataType: idlText(input.type || 'unknown', 40),
    required: Boolean(input.required),
    fieldValue: idlText(serialiseIdlValue(fieldValue), 200),
    defaultValue: idlText(input.defaultValue, 200),
    unit: idlText(input.unit, 40),
    minValue: idlText(input.min === null || input.min === undefined ? '' : input.min, 40),
    maxValue: idlText(input.max === null || input.max === undefined ? '' : input.max, 40),
    resolution: idlText(input.resolution === null || input.resolution === undefined ? '' : input.resolution, 40),
    description: idlText(input.description || `${input.kind || 'Parameter'} · ${input.cdm || input.key}`, 200),
  };
}

function controlSpecPayload(control, values = null, catalog = false) {
  const params = catalog
    ? control.inputs.map((input) => controlParamPayload(input, ''))
    : control.inputs
      .filter((input) => values && Object.hasOwn(values, input.key))
      .map((input) => controlParamPayload(input, values[input.key]));
  return {
    id: control.publicId,
    name: control.name,
    cdm: control.cdm,
    targetCdm: control.target?.cdm || '',
    numsOfParams: params.length,
    params,
  };
}

function parseReferences(vehicleFile, files, diagnostics) {
  const nodes = [...vehicleFile.doc.documentElement.getElementsByTagName('*')]
    .filter((node) => node.localName.endsWith('SpecRef'));

  return nodes.map((node) => {
    const semanticPath = value(first(node, 'SemanticPath'));
    const bindingPaths = direct(node, 'BindingPath').map(value);
    const semanticFile = resolveReferencedFile(files, vehicleFile, semanticPath);
    const bindingFiles = bindingPaths
      .map((path) => resolveReferencedFile(files, vehicleFile, path))
      .filter(Boolean);

    if (!semanticFile) diagnostics.push({
      level: 'error',
      code: 'MISSING_SEMANTIC',
      message: `${vehicleFile.path}: ${semanticPath} 참조 파일 누락`,
    });
    for (const path of bindingPaths) {
      if (!resolveReferencedFile(files, vehicleFile, path)) diagnostics.push({
        level: 'error',
        code: 'MISSING_BINDING',
        message: `${vehicleFile.path}: ${path} 참조 파일 누락`,
      });
    }

    const ref = {
      id: attr(node, 'id'),
      name: attr(node, 'name'),
      cdm: attr(node, 'cdm'),
      kind: node.localName,
      vehicleFile,
      semanticPath,
      bindingPaths,
      semanticFile,
      bindingFiles,
      controls: [],
      monitors: [],
      products: [],
      actions: [],
    };

    if (semanticFile) {
      ref.controls = parseControlGroup(ref, semanticFile);
      ref.monitors = parseMonitorGroup(ref, semanticFile);
      ref.products = parseProductGroup(ref, semanticFile);
      ref.actions = [...ref.controls, ...ref.monitors, ...ref.products];
    }
    attachBindings(ref, diagnostics);
    return ref;
  });
}

function dedupeEquivalentRefs(refs, diagnostics) {
  const seen = new Map();
  const unique = [];
  for (const ref of refs) {
    const resolvable = ref.id && ref.semanticFile && ref.bindingFiles.length === ref.bindingPaths.length;
    const key = resolvable ? [
      ref.kind.toLowerCase(),
      ref.id.toLowerCase(),
      ref.semanticFile.path.toLowerCase(),
      ref.bindingFiles.map((file) => file.path.toLowerCase()).sort().join('|'),
    ].join('::') : '';
    if (!key || !seen.has(key)) {
      if (key) seen.set(key, ref);
      unique.push(ref);
      continue;
    }
    const first = seen.get(key);
    diagnostics.push({
      level: 'warning',
      code: 'DUPLICATE_SPEC_REF_SKIPPED',
      message: ref.id + ': ' + ref.vehicleFile.path + ' 중복 참조 제외 · 기준 ' + first.vehicleFile.path,
    });
  }
  return unique;
}

function auditBundle(bundle) {
  const idsSeen = new Set();
  const unsupported = new Set();

  for (const action of bundle.actions) {
    if (idsSeen.has(action.publicId)) bundle.diagnostics.push({
      level: 'error',
      code: 'DUPLICATE_ID',
      message: `${action.publicId} 중복`,
    });
    idsSeen.add(action.publicId);

    for (const binding of action.bindings) {
      if (!binding.channel) bundle.diagnostics.push({
        level: 'error',
        code: 'NO_CHANNEL',
        message: `${binding.sourceFile} / ${binding.semanticId} Channel 누락`,
      });
      if (binding.channel && ['TCP', 'UDP', 'RS422'].includes(binding.channel.protocol)) {
        if (!binding.channel.messageType) bundle.diagnostics.push({
          level: 'error',
          code: 'NO_MESSAGE_TYPE',
          message: `${binding.sourceFile} / ${binding.semanticId} messageType 누락`,
        });
        if (binding.channel.infoCode) {
          const code = Number(binding.channel.infoCode);
          if (!Number.isInteger(code) || code < 0 || code > 65535) bundle.diagnostics.push({
            level: 'error',
            code: 'INFO_CODE_RANGE',
            message: `${binding.sourceFile} / ${binding.semanticId} infoCode 범위 오류`,
          });
        }
      }

      if (action.kind === 'Control') {
        for (const reply of action.replies) {
          if (reply.required && !binding.replies.some((item) => item.semanticId === reply.bindRef)) {
            bundle.diagnostics.push({
              level: 'error',
              code: 'REPLY_LINK',
              message: `${action.publicId} / ${binding.sourceFile} Reply 연결 실패`,
            });
          }
        }
      }

      for (const field of flattenFields(binding.fields)) {
        if (field.kind === 'DerivedField' && field.converter && !SUPPORTED_DERIVED.has(field.converter)) unsupported.add(field.converter);
      }
    }
  }

  if (unsupported.size) bundle.diagnostics.push({
    level: 'warning',
    code: 'CONVERTER_TBD',
    message: `실행식 없는 converter: ${[...unsupported].sort().join(', ')}`,
  });

  bundle.diagnostics.push({
    level: 'ok',
    code: 'INDEXED',
    message: `${bundle.controls.length} Control / ${bundle.monitors.length} Monitor / ${bundle.products.length} Sensor Product / ${bundle.actions.reduce((sum, item) => sum + item.bindings.length, 0)} Binding variant 색인`,
  });

  bundle.fatal = !bundle.vehicleFiles.length
    || bundle.diagnostics.some((item) => item.level === 'error' && ['MISSING_SEMANTIC'].includes(item.code));
}

function buildBundle(entries) {
  const diagnostics = [];
  const files = parseFiles(entries, diagnostics);
  const schemas = files.filter((file) => file.kind === 'XSD').map(parseSchema);
  const vehicleFiles = files.filter((file) => file.kind === 'Specification');
  if (!vehicleFiles.length) diagnostics.push({
    level: 'error',
    code: 'NO_SPEC',
    message: 'Vehicle Specification XML이 없습니다.',
  });

  const parsedRefs = vehicleFiles.flatMap((vehicleFile) => parseReferences(vehicleFile, files, diagnostics));
  const refs = dedupeEquivalentRefs(parsedRefs, diagnostics);
  const controls = refs.flatMap((ref) => ref.controls);
  const monitors = refs.flatMap((ref) => ref.monitors);
  const products = refs.flatMap((ref) => ref.products);
  const actions = [...controls, ...monitors, ...products];
  const bundle = {
    files,
    schemas,
    vehicleFiles,
    vehicleFile: vehicleFiles[0] || null,
    targets: vehicleFiles.map((vehicleFile) => vehicleIdentity(vehicleFile)),
    operationManagementIdl: files.find((file) => file.name.toLowerCase() === 'operationmanagement.idl') || null,
    refs,
    controls,
    monitors,
    products,
    actions,
    diagnostics,
    fatal: !vehicleFiles.length,
  };
  auditBundle(bundle);
  bundle.diagnostics.push({
    level: bundle.operationManagementIdl ? 'ok' : 'warning',
    code: 'OM_IDL',
    message: bundle.operationManagementIdl
      ? 'OperationManagement.idl을 읽어 DDS 데모 계약으로 표시합니다.'
      : 'OperationManagement.idl 미포함: 웹 내장 계약으로 DDS 데모를 표시합니다.',
  });
  return bundle;
}

function ingest(entries, folder) {
  try {
    state.activeDeviceRef = null;
    state.selectedFiles.clear();
    state.selectedFile = null;
    state.fileSelectionAnchor = null;
    state.selectedRefs.clear();
    state.selectedRef = null;
    state.refSelectionAnchor = null;
    state.selectedFeatures.clear();
    state.selectedFeature = null;
    state.featureSelectionAnchor = null;
    state.bundle = buildBundle(entries);
    state.bundle.folder = folder;
    const status = state.bundle.fatal ? 'error' : 'ok';
    const detail = state.bundle.fatal
      ? '필수 참조를 확인하세요.'
      : `${state.bundle.controls.length} Control · ${state.bundle.monitors.length} Monitor · ${state.bundle.products.length} Sensor Product 분석 완료`;
    setSource(status, folder, detail);
    renderBundle();
    resetDemo();
  } catch (error) {
    console.error(error);
    state.bundle = null;
    setSource('error', folder, `분석 실패: ${error.message}`);
    if (dom.diagnosticCount) dom.diagnosticCount.textContent = '1';
  }
}

function renderBundle() {
  const bundle = state.bundle;
  if (dom.xmlFileCount) dom.xmlFileCount.textContent = bundle.files.filter((file) => file.format === 'XML').length;
  if (dom.xsdFileCount) dom.xsdFileCount.textContent = bundle.schemas.length;
  if (dom.idlFileCount) dom.idlFileCount.textContent = bundle.files.filter((file) => file.kind === 'IDL').length;
  if (dom.controlCount) dom.controlCount.textContent = bundle.controls.length;
  if (dom.monitorCount) dom.monitorCount.textContent = bundle.monitors.length;
  if (dom.productCount) dom.productCount.textContent = bundle.products.length;
  if (dom.diagnosticCount) dom.diagnosticCount.textContent = bundle.diagnostics.length;
  if (dom.modelEmptyState) dom.modelEmptyState.hidden = true;
  if (dom.modelWorkspace) dom.modelWorkspace.hidden = false;
  if (dom.hmiEmptyState) dom.hmiEmptyState.hidden = true;
  if (dom.hmiWorkspace) dom.hmiWorkspace.hidden = false;

  renderRelationships();
  renderFiles();
  updateSelectionSummary();
  renderFeatureList();
  renderDiagnostics();
  renderCatalog();
  renderPresets();
  renderHmiWorkspace();
  renderCdmFileButtons();
  if (state.cdmView !== 'principles') showCdmView(state.cdmView);
}

function renderCdmFileButtons() {
  clear(dom.cdmFileButtons);
  if (!dom.cdmFileButtons || !state.bundle) return;
  const documents = [
    ['classification', 'CDM_Classification.md'],
    ['mapping', 'AUV_CDM_Mapping.md'],
  ].filter(([, name]) => getUniqueFileByName(name));
  for (const [view, name] of documents) {
    const button = el('button', 'reference-button', name);
    button.type = 'button';
    button.dataset.cdmView = view;
    dom.cdmFileButtons.append(button);
  }
  if (dom.cdmReferenceHint) {
    dom.cdmReferenceHint.textContent = documents.length
      ? '선택 폴더 Markdown 문서'
      : '선택 폴더에 CDM Markdown 없음';
  }
}

function relationButton(type, title, meta, filePath = '', refKey = '') {
  const button = el('button', `relation-node ${type}`);
  button.type = 'button';
  if (filePath) button.dataset.file = filePath;
  if (refKey) button.dataset.refKey = refKey;
  button.setAttribute('aria-selected', 'false');
  button.append(el('strong', '', title), el('small', '', meta));
  return button;
}

function referenceKey(ref) {
  return `${ref.vehicleFile.path}::${ref.id}`;
}

function referenceFiles(ref) {
  return [
    ref.vehicleFile?.path,
    ref.semanticFile?.path,
    ...ref.bindingFiles.map((file) => file.path),
  ].filter(Boolean);
}

function findReference(key) {
  return state.bundle?.refs.find((ref) => referenceKey(ref) === key) || null;
}

function renderRelationshipsLegacy() {
  clear(dom.relationshipMap);
  for (const ref of state.bundle.refs) {
    const chain = el('div', 'relation-chain');
    const bindings = el('div', 'binding-stack');
    chain.append(
      relationButton(
        'vehicle',
        ref.name || ref.id,
        `SpecRef · ${ref.id} · ${ref.controls.length} Control · ${ref.monitors.length} Monitor · ${ref.products.length} Product`,
        '',
        referenceKey(ref),
      ),
      el('div', 'relation-arrow', '→'),
      relationButton(
        'semantic',
        ref.semanticPath,
        `${ref.controls.length} Control · ${ref.monitors.length} Monitor · ${ref.products.length} Product`,
        ref.semanticFile ? ref.semanticFile.path : ref.semanticPath,
      ),
      el('div', 'relation-arrow', '→'),
    );
    for (const file of ref.bindingFiles) {
      const variants = ref.actions.flatMap((action) => action.bindings.filter((binding) => binding.sourcePath === file.path));
      const protocols = [...new Set(variants.map((binding) => binding.transport))].join('/') || 'UNKNOWN';
      bindings.append(relationButton('binding', file.name, `${protocols} · ${variants.length} variant`, file.path));
    }
    chain.append(bindings);
    dom.relationshipMap.append(chain);
  }

  clear(dom.schemaRail);
  for (const schema of state.bundle.schemas) {
    const button = el('button', 'schema-node', `${schema.file} · ${schema.roots.length} root · include ${schema.includes.length}`);
    button.dataset.file = schema.path;
    button.setAttribute('aria-selected', 'false');
    dom.schemaRail.append(button);
  }
}

function deviceMeta(ref) {
  return 'SpecRef · ' + ref.id + ' · ' + ref.controls.length + ' Control · '
    + ref.monitors.length + ' Monitor · ' + ref.products.length + ' Product';
}

function renderDeviceList(filter = '') {
  clear(dom.deviceList);
  if (!dom.deviceList || !state.bundle) return;
  const query = filter.trim().toLowerCase();
  const refs = state.bundle.refs.filter((ref) => (
    !query
    || (ref.name || '').toLowerCase().includes(query)
    || ref.id.toLowerCase().includes(query)
  ));
  for (const ref of refs) {
    const key = referenceKey(ref);
    const button = el('button', 'device-option');
    button.type = 'button';
    button.dataset.deviceKey = key;
    button.classList.toggle('is-active', key === state.activeDeviceRef);
    button.append(el('strong', '', ref.name || ref.id), el('small', '', deviceMeta(ref)));
    dom.deviceList.append(button);
  }
  if (!refs.length) dom.deviceList.append(el('p', 'empty-text', '일치하는 장치 없음'));
}

function renderSchemaSummary() {
  if (!state.bundle) return;
  const schemas = [...new Map(state.bundle.schemas.map((schema) => [schema.file.toLowerCase(), schema])).values()];
  const roots = schemas.reduce((sum, schema) => sum + schema.roots.length, 0);
  const includes = schemas.reduce((sum, schema) => sum + schema.includes.length, 0);
  if (dom.schemaSummary) dom.schemaSummary.textContent = '공통 XSD ' + schemas.length + '종 · Root ' + roots + ' · Include ' + includes;
  clear(dom.schemaList);
  if (!dom.schemaList) return;
  for (const schema of schemas) {
    const button = el('button', 'schema-node', schema.file + ' · ' + schema.roots.length + ' root · include ' + schema.includes.length);
    button.type = 'button';
    button.dataset.file = schema.path;
    button.setAttribute('aria-selected', 'false');
    dom.schemaList.append(button);
  }
}

function renderRelationships() {
  clear(dom.relationshipMap);
  renderDeviceList(dom.deviceSearch ? dom.deviceSearch.value : '');
  renderSchemaSummary();
  const ref = state.activeDeviceRef ? findReference(state.activeDeviceRef) : null;
  if (dom.devicePickerName) dom.devicePickerName.textContent = ref ? (ref.name || ref.id) : '장치를 선택하세요';
  if (dom.devicePickerMeta) dom.devicePickerMeta.textContent = ref ? deviceMeta(ref) : state.bundle.refs.length + '개 장치';
  if (dom.deviceDetailName) dom.deviceDetailName.textContent = ref ? (ref.name || ref.id) : '장치를 선택하세요';
  if (dom.devicePickerButton) dom.devicePickerButton.textContent = ref ? '장치 변경' : '장치 선택';

  if (!ref) {
    dom.relationshipMap.append(el('p', 'device-empty', '상단 장치 선택 필요'));
    return;
  }

  const chain = el('div', 'relation-chain');
  const bindings = el('div', 'binding-stack');
  chain.append(
    relationButton('vehicle', ref.name || ref.id, deviceMeta(ref), '', referenceKey(ref)),
    el('div', 'relation-arrow', '→'),
    relationButton(
      'semantic',
      ref.semanticPath,
      ref.controls.length + ' Control · ' + ref.monitors.length + ' Monitor · ' + ref.products.length + ' Product',
      ref.semanticFile ? ref.semanticFile.path : ref.semanticPath,
    ),
    el('div', 'relation-arrow', '→'),
  );
  for (const file of ref.bindingFiles) {
    const variants = ref.actions.flatMap((action) => action.bindings.filter((binding) => binding.sourcePath === file.path));
    const protocols = [...new Set(variants.map((binding) => binding.transport))].join('/') || 'UNKNOWN';
    bindings.append(relationButton('binding', file.name, protocols + ' · ' + variants.length + ' variant', file.path));
  }
  chain.append(bindings);
  dom.relationshipMap.append(chain);
  renderFileSelection();
}

function renderFiles() {
  if (!dom.fileList || !dom.loadedFileCount) return;
  clear(dom.fileList);
  dom.loadedFileCount.textContent = state.bundle.files.length;
  const order = { Specification: 0, Semantic: 1, Binding: 2, XSD: 3, Markdown: 4, IDL: 5 };
  const files = [...state.bundle.files].sort((a, b) => (
    (order[a.kind] ?? 9) - (order[b.kind] ?? 9)
    || a.path.localeCompare(b.path)
  ));
  for (const file of files) {
    const button = el('button', 'file-item');
    button.dataset.file = file.path;
    button.setAttribute('aria-selected', 'false');
    button.append(
      el('strong', '', file.name),
      el('small', '', `${file.kind}${file.root ? ` · ${file.root}` : ''}${file.path !== file.name ? ` · ${file.path}` : ''}`),
    );
    dom.fileList.append(button);
  }
}

function makeDetail(title, lines) {
  const block = el('section', 'detail-block');
  block.append(el('h3', '', title));
  const list = el('ul');
  lines.forEach((line) => list.append(el('li', '', line)));
  block.append(list);
  return block;
}

function uniqueSelectionOrder(nodes, dataKey) {
  const seen = new Set();
  const order = [];
  for (const node of nodes) {
    const itemValue = node.dataset[dataKey];
    if (itemValue && !seen.has(itemValue)) {
      seen.add(itemValue);
      order.push(itemValue);
    }
  }
  return order;
}

function fileSelectionOrder(sourceNode) {
  if (sourceNode?.closest('#fileList')) return uniqueSelectionOrder(dom.fileList.querySelectorAll('[data-file]'), 'file');
  return uniqueSelectionOrder(document.querySelectorAll('#relationshipMap [data-file], #schemaRail [data-file]'), 'file');
}

function applyWindowsSelection({ set, value: selectedValue, anchor, order, shiftKey, toggleKey }) {
  if (shiftKey) {
    const startValue = anchor && order.includes(anchor) ? anchor : selectedValue;
    const startIndex = order.indexOf(startValue);
    const endIndex = order.indexOf(selectedValue);
    if (!toggleKey) set.clear();
    if (startIndex >= 0 && endIndex >= 0) {
      const [from, to] = startIndex <= endIndex ? [startIndex, endIndex] : [endIndex, startIndex];
      order.slice(from, to + 1).forEach((item) => set.add(item));
    } else set.add(selectedValue);
    return anchor || selectedValue;
  }

  if (toggleKey) {
    if (set.has(selectedValue)) set.delete(selectedValue);
    else set.add(selectedValue);
    return selectedValue;
  }

  set.clear();
  set.add(selectedValue);
  return selectedValue;
}

function referenceSelectionOrder() {
  return uniqueSelectionOrder(dom.relationshipMap.querySelectorAll('[data-ref-key]'), 'refKey');
}

function selectedRelatedActions() {
  if (!state.bundle || !state.activeDeviceRef) return [];
  return state.bundle.actions.filter((action) => featureMatchesFiles(action));
}

function updateSelectionSummary() {
  const actions = selectedRelatedActions();
  const controls = actions.filter((action) => action.kind === 'Control').length;
  const monitors = actions.filter((action) => action.kind === 'Monitor').length;
  const products = actions.filter((action) => action.kind === 'Product').length;
  if (dom.selectionFileCount) dom.selectionFileCount.textContent = String(state.selectedFiles.size);
  if (dom.selectionControlCount) dom.selectionControlCount.textContent = String(controls);
  if (dom.selectionMonitorCount) dom.selectionMonitorCount.textContent = String(monitors);
  if (dom.selectionProductCount) dom.selectionProductCount.textContent = String(products);
}

function rebuildFilesFromSelectedReferences() {
  state.selectedFiles.clear();
  for (const key of state.selectedRefs) {
    const ref = findReference(key);
    if (ref) referenceFiles(ref).forEach((path) => state.selectedFiles.add(path));
  }
}

function showReferenceLegacy(key, options = {}) {
  const ref = findReference(key);
  if (!ref) return;

  state.refSelectionAnchor = applyWindowsSelection({
    set: state.selectedRefs,
    value: key,
    anchor: state.refSelectionAnchor,
    order: referenceSelectionOrder(),
    shiftKey: Boolean(options.shiftKey),
    toggleKey: Boolean(options.ctrlKey || options.metaKey),
  });
  state.selectedRef = state.selectedRefs.has(key)
    ? key
    : [...state.selectedRefs][state.selectedRefs.size - 1] || null;
  rebuildFilesFromSelectedReferences();

  const primaryRef = state.selectedRef ? findReference(state.selectedRef) : null;
  state.selectedFile = primaryRef?.vehicleFile?.path || [...state.selectedFiles][state.selectedFiles.size - 1] || null;
  state.fileSelectionAnchor = state.selectedFile;

  renderFeatureList(dom.controlSearch.value);
  clear(dom.modelDetail);
  if (!primaryRef) {
    dom.modelDetail.append(el('p', 'empty-text', '파일 또는 Control을 선택하세요.'));
    renderFileSelection();
    updateSelectionSummary();
    return;
  }

  const actions = selectedRelatedActions();
  const controlCount = actions.filter((action) => action.kind === 'Control').length;
  const monitorCount = actions.filter((action) => action.kind === 'Monitor').length;
  const productCount = actions.filter((action) => action.kind === 'Product').length;
  dom.modelDetail.append(
    el('h3', '', primaryRef.name || primaryRef.id),
    el('code', '', primaryRef.id),
    makeDl([
      ['종류', primaryRef.kind],
      ['Specification', primaryRef.vehicleFile.name],
      ['Semantic', primaryRef.semanticFile?.name || primaryRef.semanticPath],
      ['Binding', primaryRef.bindingFiles.map((file) => file.name).join(', ') || '없음'],
      ['선택 파일', `${state.selectedFiles.size}개`],
      ['관련 Control', `${controlCount}개`],
      ['관련 Monitor', `${monitorCount}개`],
      ['관련 Sensor Product', `${productCount}개`],
    ]),
  );
  renderFileSelection();
  updateSelectionSummary();
}

function showReference(key) {
  const ref = findReference(key);
  if (!ref) return;
  state.activeDeviceRef = key;
  state.selectedRefs.clear();
  state.selectedRefs.add(key);
  state.selectedRef = key;
  state.selectedFiles.clear();
  state.selectedFile = null;
  state.fileSelectionAnchor = null;
  state.selectedFeatures.clear();
  state.selectedFeature = null;
  state.featureSelectionAnchor = null;
  if (dom.devicePopover) dom.devicePopover.hidden = true;
  if (dom.devicePickerButton) dom.devicePickerButton.setAttribute('aria-expanded', 'false');
  if (dom.deviceSearch) dom.deviceSearch.value = '';

  renderRelationships();
  renderFeatureList(dom.controlSearch ? dom.controlSearch.value : '');
  clear(dom.modelDetail);
  clear(dom.bindingDetail);
  dom.modelDetail.append(
    el('h3', '', ref.name || ref.id),
    el('code', '', ref.id),
    makeDl([
      ['종류', ref.kind],
      ['Specification', ref.vehicleFile.name],
      ['Semantic', ref.semanticFile ? ref.semanticFile.name : ref.semanticPath],
      ['Binding', ref.bindingFiles.map((file) => file.name).join(', ') || '없음'],
      ['Control', ref.controls.length + '개'],
      ['Monitor', ref.monitors.length + '개'],
      ['Sensor Product', ref.products.length + '개'],
    ]),
  );
  if (dom.bindingDetail) dom.bindingDetail.append(el('p', 'empty-text', '기능 선택 후 Binding 정보 표시'));
  updateSelectionSummary();
}

function renderFileSelection() {
  for (const item of document.querySelectorAll('[data-file]')) {
    const selected = state.selectedFiles.has(item.dataset.file);
    const primary = selected && item.dataset.file === state.selectedFile;
    item.classList.toggle('is-active', selected);
    item.classList.toggle('is-selected', selected);
    item.classList.toggle('is-primary', primary);
    item.setAttribute('aria-selected', String(selected));
  }
  for (const item of document.querySelectorAll('[data-ref-key]')) {
    const selected = state.selectedRefs.has(item.dataset.refKey);
    const primary = selected && item.dataset.refKey === state.selectedRef;
    item.classList.toggle('is-active', selected);
    item.classList.toggle('is-selected', selected);
    item.classList.toggle('is-primary', primary);
    item.setAttribute('aria-selected', String(selected));
  }
}

function semanticRelatedFiles(path) {
  if (!state.bundle) return [path];
  const related = new Set([path]);
  for (const ref of state.bundle.refs) {
    if (ref.semanticFile?.path !== path) continue;
    ref.bindingFiles.forEach((file) => related.add(file.path));
  }
  return [...related];
}

function expandSelectedSemanticFiles() {
  if (!state.bundle) return;
  const selected = [...state.selectedFiles];
  for (const path of selected) {
    const file = state.bundle.files.find((item) => item.path === path);
    if (file?.kind === 'Semantic') semanticRelatedFiles(path).forEach((item) => state.selectedFiles.add(item));
  }
}

function showFile(path, options = {}) {
  const file = state.bundle.files.find((item) => item.path === path);
  if (!file) return;

  state.selectedRefs.clear();
  state.selectedRef = null;
  state.refSelectionAnchor = null;

  const sourceNode = options.sourceNode || null;
  const order = fileSelectionOrder(sourceNode);
  const toggleKey = Boolean(options.ctrlKey || options.metaKey);
  const relatedFiles = file.kind === 'Semantic' ? semanticRelatedFiles(path) : [path];

  if (options.shiftKey) {
    state.fileSelectionAnchor = applyWindowsSelection({
      set: state.selectedFiles,
      value: path,
      anchor: state.fileSelectionAnchor,
      order,
      shiftKey: true,
      toggleKey,
    });
    expandSelectedSemanticFiles();
  } else if (toggleKey && relatedFiles.length > 1) {
    const removeGroup = relatedFiles.every((item) => state.selectedFiles.has(item));
    relatedFiles.forEach((item) => {
      if (removeGroup) state.selectedFiles.delete(item);
      else state.selectedFiles.add(item);
    });
    state.fileSelectionAnchor = path;
  } else if (!toggleKey && relatedFiles.length > 1) {
    state.selectedFiles.clear();
    relatedFiles.forEach((item) => state.selectedFiles.add(item));
    state.fileSelectionAnchor = path;
  } else {
    state.fileSelectionAnchor = applyWindowsSelection({
      set: state.selectedFiles,
      value: path,
      anchor: state.fileSelectionAnchor,
      order,
      shiftKey: false,
      toggleKey,
    });
  }

  state.selectedFile = state.selectedFiles.has(path)
    ? path
    : [...state.selectedFiles][state.selectedFiles.size - 1] || null;
  renderFeatureList(dom.controlSearch.value);
  clear(dom.modelDetail);
  clear(dom.bindingDetail);
  if (dom.bindingDetail) dom.bindingDetail.append(el('p', 'empty-text', '기능 선택 후 Binding 정보 표시'));

  const detailFile = state.selectedFile
    ? state.bundle.files.find((item) => item.path === state.selectedFile)
    : null;
  if (!detailFile) {
    dom.modelDetail.append(el('p', 'empty-text', '파일 또는 Control을 선택하세요.'));
    renderFileSelection();
    updateSelectionSummary();
    return;
  }

  dom.modelDetail.append(
    el('h3', '', detailFile.name),
    makeDl([
      ['종류', detailFile.kind],
      ['Root', detailFile.root || '없음'],
      ['상대경로', detailFile.path],
      ['크기', `${detailFile.text.length.toLocaleString()} chars`],
      ['선택', `${state.selectedFiles.size}개 파일`],
      ['관련 Control', `${selectedRelatedActions().filter((action) => action.kind === 'Control').length}개`],
      ['관련 Monitor', `${selectedRelatedActions().filter((action) => action.kind === 'Monitor').length}개`],
      ['관련 Sensor Product', `${selectedRelatedActions().filter((action) => action.kind === 'Product').length}개`],
    ]),
  );

  if (detailFile.kind === 'XSD') {
    const schema = state.bundle.schemas.find((item) => item.path === detailFile.path);
    if (schema) dom.modelDetail.append(makeDetail('XSD 구성', [
      `namespace: ${schema.namespace}`,
      `root element: ${schema.roots.join(', ') || '없음'}`,
      `include/import: ${schema.includes.join(', ') || '없음'}`,
      `complex/simple type: ${schema.complexTypes} / ${schema.simpleTypes}`,
    ]));
  }

  renderFileSelection();
  updateSelectionSummary();
}

function featureMatchesFiles(action) {
  const activeRef = state.activeDeviceRef ? findReference(state.activeDeviceRef) : null;
  if (!activeRef || action.ref !== activeRef) return false;
  const selected = state.selectedFiles;
  return !selected.size
    || selected.has(action.sourcePath)
    || action.bindings.some((binding) => selected.has(binding.sourcePath));
}

function renderFeatureList(filter = '') {
  clear(dom.controlList);
  if (!state.bundle) return;
  const query = filter.trim().toLowerCase();
  const actions = state.bundle.actions.filter((action) => (
    featureMatchesFiles(action)
    && (!query
      || action.publicId.toLowerCase().includes(query)
      || action.cdm.toLowerCase().includes(query)
      || action.name.toLowerCase().includes(query)
      || action.kind.toLowerCase().includes(query))
  ));

  actions.sort((a, b) => a.kind.localeCompare(b.kind) || a.publicId.localeCompare(b.publicId));
  for (const action of actions) {
    const button = el('button', `control-item ${action.kind.toLowerCase()}`);
    button.dataset.controlId = action.publicId;
    button.setAttribute('aria-selected', 'false');
    const header = el('span', 'feature-item-head');
    header.append(el('strong', '', action.name), el('b', `feature-badge ${action.kind.toLowerCase()}`, action.kind));
    button.append(
      header,
      el('small', '', action.publicId),
      el('small', '', `${action.cdm} · ${action.bindings.length} variant`),
    );
    dom.controlList.append(button);
  }
  if (!dom.controlList.children.length) {
    dom.controlList.append(el('p', 'empty-text', state.activeDeviceRef ? '선택 범위에 해당하는 기능 없음' : '장치를 먼저 선택하세요'));
  }
  const visibleIds = new Set(actions.map((action) => action.publicId));
  for (const id of [...state.selectedFeatures]) if (!visibleIds.has(id)) state.selectedFeatures.delete(id);
  if (state.selectedFeature && !visibleIds.has(state.selectedFeature)) state.selectedFeature = [...state.selectedFeatures][state.selectedFeatures.size - 1] || null;
  renderFeatureSelection();
}

function inputLabel(input) {
  const range = input.min !== null || input.max !== null ? ` · ${input.min ?? '…'}..${input.max ?? '…'}` : '';
  return `${input.key} ← ${input.cdm} · ${input.type}${input.unit ? ` · ${input.unit}` : ''}${range}`;
}

function outputLabel(output) {
  const range = output.min !== null || output.max !== null ? ` · ${output.min ?? '…'}..${output.max ?? '…'}` : '';
  return `${output.cdm} · ${output.type}${output.unit ? ` · ${output.unit}` : ''}${range}`;
}

function profileTypeLabel(type) {
  return {
    number: '숫자 (number)',
    boolean: '참/거짓 (boolean)',
    collection: '목록 (collection)',
    text: '문자열 (text)',
    valueSet: '값 목록 (valueSet)',
    result: '결과값 (result)',
  }[type] || '미정';
}

function profileRangeLabel(profile) {
  const hasRange = profile.min !== null || profile.max !== null;
  const range = hasRange ? (profile.min ?? '…') + '..' + (profile.max ?? '…') : '';
  return [range, profile.unit || ''].filter(Boolean).join(' · ') || '—';
}

function profileTable(title, profiles, inputMode = false) {
  const block = el('section', 'detail-block semantic-profile-block');
  block.append(el('h3', '', title));
  if (!profiles.length) {
    block.append(el('p', 'semantic-empty', inputMode ? '입력 없음' : '직접 Profile 없음'));
    return block;
  }
  const tableWrap = el('div', 'semantic-table-wrap');
  const table = el('table', 'semantic-table');
  const head = el('thead');
  const headRow = el('tr');
  const headings = inputMode
    ? ['입력 Key', 'CDM 의미', '형식', '범위 / 단위']
    : ['CDM 의미', '형식', '범위 / 단위'];
  headings.forEach((heading) => headRow.append(el('th', '', heading)));
  head.append(headRow);
  const body = el('tbody');
  for (const profile of profiles) {
    const row = el('tr');
    if (inputMode) {
      const keyCell = el('td');
      keyCell.dataset.label = '입력 Key';
      keyCell.append(el('code', '', profile.key));
      if (!profile.required) keyCell.append(el('small', 'optional-mark', '선택'));
      row.append(keyCell);
    }
    const cdmCell = el('td', '', profile.cdm || '미정');
    cdmCell.dataset.label = 'CDM 의미';
    const typeCell = el('td', '', profileTypeLabel(profile.type));
    typeCell.dataset.label = '형식';
    const rangeCell = el('td', '', profileRangeLabel(profile));
    rangeCell.dataset.label = '범위 / 단위';
    row.append(cdmCell, typeCell, rangeCell);
    body.append(row);
  }
  table.append(head, body);
  tableWrap.append(table);
  block.append(tableWrap);
  return block;
}

function fieldDisplay(field) {
  return [
    field.value ? `value=${field.value}` : '',
    field.fixedValue ? `fixedValue=${field.fixedValue}` : '',
    field.cdm || '',
    field.dataType || '',
    field.scale ? `scale=${field.scale}` : '',
    field.offset !== undefined && field.offset !== '' ? `offset=${field.offset}` : '',
    field.width ? `width=${field.width}` : '',
    field.expectedValue ? `expected=${field.expectedValue}` : '',
    field.expectedMask ? `mask=${field.expectedMask}` : '',
    field.maps?.length ? `${field.maps.length} map` : '',
    field.converter || '',
  ].filter(Boolean).join(' · ') || '—';
}

function appendFieldRows(container, fields, depth = 0) {
  for (const field of fields) {
    const row = el('div', 'field-row-large');
    row.style.setProperty('--field-depth', String(depth));
    row.append(
      el('code', 'field-row-name', field.name || '(unnamed)'),
      el('span', 'field-row-kind', field.kind),
      el('span', 'field-row-meta', fieldDisplay(field)),
    );
    container.append(row);
    if (field.children.length) appendFieldRows(container, field.children, depth + 1);
  }
}

function messageSectionLegacy(kind, id, channel, fields, reply) {
  const section = el('section', `message-section ${kind.toLowerCase()}`);
  section.append(
    el('h3', 'message-section-title', `${kind}${id ? ` · ${id}` : ''}`),
    makeDl([
      ...channelRows(channel),
      ['Field', `${flattenFields(fields).length}개`],
      ...(reply ? [['Required', reply.required ? 'true' : 'false']] : []),
    ]),
  );
  section.append(el('h4', 'message-field-title', `${kind} Field`));
  const list = el('div', `field-list-large ${kind === 'Reply' ? 'reply-fields' : ''}`);
  const columns = el('div', 'field-row-header');
  columns.append(el('span', '', 'Field'), el('span', '', 'Kind'), el('span', '', 'Value / CDM / Converter'));
  list.append(columns);
  if (fields.length) appendFieldRows(list, fields);
  else list.append(el('p', 'empty-text', '정의된 Field 없음'));
  section.append(list);
  return section;
}

function messageSection(kind, id, channel, fields, reply) {
  const section = el('section', 'message-section ' + kind.toLowerCase());
  section.append(
    el('h3', 'message-section-title', kind + (id ? ' · ' + id : '')),
    makeDl([
      ...channelRows(channel),
      ['Field', flattenFields(fields).length + '개'],
      ...(reply ? [['Required', reply.required ? 'true' : 'false']] : []),
    ]),
  );

  const details = el('details', 'field-details');
  const summary = el('summary');
  summary.append(
    el('span', '', kind + ' Field' + (id ? ' · ' + id : '') + ' · ' + flattenFields(fields).length + '개'),
    el('small', '', '펼치기'),
  );
  details.append(summary);
  const list = el('div', 'field-list-large ' + (kind === 'Reply' ? 'reply-fields' : ''));
  const columns = el('div', 'field-row-header');
  columns.append(el('span', '', 'FIELD'), el('span', '', 'KIND'), el('span', '', 'VALUE / CDM / CONVERTER'));
  list.append(columns);
  if (fields.length) appendFieldRows(list, fields);
  else list.append(el('p', 'semantic-empty', '정의된 Field 없음'));
  details.append(list);
  section.append(details);
  return section;
}

function bindingDetail(action, binding) {
  const block = el('section', 'detail-block binding-detail-large');
  block.append(el('h3', 'binding-file-title', `${binding.transport} · ${binding.sourceFile}`));
  const sectionKind = action.kind === 'Control' ? 'Request' : action.kind === 'Product' ? 'Product' : 'Monitor';
  block.append(messageSection(sectionKind, '', binding.channel, binding.fields));
  if (binding.matchFields?.length) {
    block.append(makeDetail('Product 판별 조건', binding.matchFields.map((match) => (
      `${match.name}${match.mask ? ` & ${match.mask}` : ''} = ${match.value}`
    ))));
  }
  for (const reply of binding.replies) block.append(messageSection('Reply', reply.semanticId, reply.channel, reply.fields, reply));
  return block;
}

function renderReplyOverview(control) {
  const block = el('section', 'detail-block reply-overview');
  block.append(el('h3', '', 'Reply 항목'));
  if (!control.replies.length) {
    block.append(el('p', 'empty-text', 'Reply 없음'));
    return block;
  }

  const list = el('div', 'reply-list');
  for (const reply of control.replies) {
    const item = el('article', 'reply-item');
    item.append(
      el('strong', '', reply.bindRef),
      el('small', '', `${reply.cdm} · ${reply.required ? '필수' : '선택'}${reply.timeout ? ` · timeout ${reply.timeout}` : ''}`),
    );
    const variants = control.bindings.flatMap((binding) => (
      binding.replies
        .filter((candidate) => candidate.semanticId === reply.bindRef)
        .map((candidate) => ({ binding, candidate }))
    ));
    for (const entry of variants) {
      const row = el('div', 'reply-binding');
      row.append(
        el('b', '', `${entry.binding.transport} · ${entry.binding.sourceFile}`),
        el('span', '', channelSummary(entry.candidate.channel)),
        el('small', '', `${flattenFields(entry.candidate.fields).length}개 Field`),
      );
      item.append(row);
    }
    list.append(item);
  }
  block.append(list);
  return block;
}

function featureSelectionOrder() {
  return uniqueSelectionOrder(dom.controlList.querySelectorAll('[data-control-id]'), 'controlId');
}

function renderFeatureSelection() {
  for (const item of document.querySelectorAll('[data-control-id]')) {
    const selected = state.selectedFeatures.has(item.dataset.controlId);
    const primary = selected && item.dataset.controlId === state.selectedFeature;
    item.classList.toggle('is-active', selected);
    item.classList.toggle('is-selected', selected);
    item.classList.toggle('is-primary', primary);
    item.setAttribute('aria-selected', String(selected));
  }
}

function clearFeatureDetail() {
  clear(dom.modelDetail);
  clear(dom.bindingDetail);
  dom.modelDetail.append(el('p', 'empty-text', 'Control, Monitor 또는 Sensor Product를 선택하세요.'));
  if (dom.bindingDetail) dom.bindingDetail.append(el('p', 'empty-text', '기능 선택 후 Binding 정보 표시'));
}

function showFeature(id, options = {}) {
  const action = state.bundle.actions.find((item) => item.publicId === id);
  if (!action) return;

  state.featureSelectionAnchor = applyWindowsSelection({
    set: state.selectedFeatures,
    value: id,
    anchor: state.featureSelectionAnchor,
    order: featureSelectionOrder(),
    shiftKey: Boolean(options.shiftKey),
    toggleKey: Boolean(options.ctrlKey || options.metaKey),
  });

  state.selectedFeature = state.selectedFeatures.has(id)
    ? id
    : [...state.selectedFeatures][state.selectedFeatures.size - 1] || null;

  if (!state.selectedFeature) {
    clearFeatureDetail();
    renderFeatureSelection();
    return;
  }

  const primaryAction = state.bundle.actions.find((item) => item.publicId === state.selectedFeature);
  if (!primaryAction) return;
  clear(dom.modelDetail);
  clear(dom.bindingDetail);
  dom.modelDetail.append(
    el('h3', '', primaryAction.name),
    el('code', '', primaryAction.publicId),
    makeDl([
      ['종류', primaryAction.kind],
      ['CDM', primaryAction.cdm],
      ['Semantic', primaryAction.sourceFile],
      ['Local ID', primaryAction.semanticId],
      ['Reply', primaryAction.kind === 'Control' ? primaryAction.replies.map((item) => item.bindRef).join(', ') || 'No Reply' : '해당 없음'],
      ['선택', `${state.selectedFeatures.size}개 항목`],
    ]),
  );

  if (primaryAction.kind === 'Control') {
    dom.modelDetail.append(profileTable('HMI 입력', primaryAction.inputs, true));
    dom.modelDetail.append(renderReplyOverview(primaryAction));
  } else if (primaryAction.kind === 'Product') {
    dom.modelDetail.append(makeDl([
      ['Product Type', primaryAction.productType],
      ['Product Kind', primaryAction.productKind || '미정'],
      ['Processing State', primaryAction.processingState || '없음'],
    ]));
  } else {
    dom.modelDetail.append(profileTable('Monitor 의미 항목', primaryAction.outputs));
  }

  if (primaryAction.bindings.length) {
    for (const binding of primaryAction.bindings) dom.bindingDetail.append(bindingDetail(primaryAction, binding));
  } else if (dom.bindingDetail) {
    dom.bindingDetail.append(el('p', 'empty-text', '연결된 Binding 없음'));
  }
  renderFeatureSelection();
}

function renderDiagnostics() {
  clear(dom.diagnosticList);
  for (const item of state.bundle.diagnostics) {
    const row = el('div', `diagnostic-item ${item.level}`);
    row.append(el('b', '', item.level.toUpperCase()), el('span', '', `${item.code} · ${item.message}`));
    dom.diagnosticList.append(row);
  }
}

function getUniqueFileByName(name) {
  if (!state.bundle) return null;
  return state.bundle.files.find((file) => file.name.toLowerCase() === name.toLowerCase()) || null;
}

function showCdmView(view) {
  state.cdmView = view;
  for (const button of document.querySelectorAll('[data-cdm-view]')) button.classList.toggle('is-active', button.dataset.cdmView === view);
  if (view === 'principles') {
    renderPrinciples();
    return;
  }
  const name = view === 'classification' ? 'CDM_Classification.md' : 'AUV_CDM_Mapping.md';
  const file = getUniqueFileByName(name);
  if (file) renderMarkdown(file.text);
  else {
    clear(dom.cdmDocument);
    dom.cdmDocument.append(el('p', 'empty-text', `선택 폴더에서 ${name} 파일을 찾지 못했습니다.`));
  }
}

function appendTerminal(text, type = '', prefix = '') {
  const line = el('div', `terminal-line ${type}`, `${prefix ? `${prefix} ` : ''}${text}`);
  dom.terminalLog.append(line);
  dom.terminalLog.scrollTop = dom.terminalLog.scrollHeight;
}

function appendControlIdList(controls) {
  const wrapper = el('div', 'terminal-control-list');
  controls.forEach((control, index) => {
    const command = `control -i ${control.publicId}`;
    const button = el('button', 'terminal-control-id');
    button.type = 'button';
    button.title = `${command} 입력`;
    button.append(
      el('span', 'terminal-control-index', String(index + 1).padStart(2, '0')),
      el('code', '', control.publicId),
      el('small', '', control.name || ''),
    );
    button.addEventListener('click', () => {
      dom.terminalInput.value = command;
      dom.terminalInput.focus();
      dom.terminalInput.setSelectionRange(command.length, command.length);
    });
    wrapper.append(button);
  });
  dom.terminalLog.append(wrapper);
  dom.terminalLog.scrollTop = dom.terminalLog.scrollHeight;
}

function addBus(channel, direction, payload, type = '', metadata = {}) {
  if (!dom.busTimeline) return;
  const empty = dom.busTimeline.querySelector('.timeline-empty');
  if (empty) clear(dom.busTimeline);
  const row = el('article', `bus-message ${type}`);
  const meta = el('div', 'bus-meta');
  meta.append(el('strong', '', direction), el('small', '', channel));
  for (const [key, itemValue] of Object.entries(metadata)) {
    if (itemValue !== '' && itemValue !== null && itemValue !== undefined) meta.append(el('small', 'bus-metadata', `${key}: ${itemValue}`));
  }
  row.append(meta, el('code', '', JSON.stringify(payload, null, 2)));
  dom.busTimeline.append(row);
  dom.busTimeline.scrollTop = dom.busTimeline.scrollHeight;
}

function resetPipeline() {
  if (!dom.pipeline) return;
  for (const item of dom.pipeline.children) item.className = '';
}

function markStep(name, status) {
  const item = dom.pipeline?.querySelector(`[data-step=${name}]`);
  if (item) item.className = status;
}

function payloadEmpty(message) {
  const row = el('tr');
  const cell = el('td', 'table-empty', message);
  cell.colSpan = 3;
  row.append(cell);
  return row;
}

function resetDemo() {
  state.demo = {
    specLoaded: false,
    controlId: null,
    args: new Map(),
    history: [],
    historyIndex: 0,
    sequence: 1,
    requestId: 1,
    ddsSequence: 1,
    currentDds: null,
    latestPayload: null,
    processing: false,
    commandUuid: null,
    lastCompletionCommand: 0,
    now: null,
  };
  clear(dom.terminalLog);
  if (dom.busTimeline) {
    clear(dom.busTimeline);
    dom.busTimeline.append(el('p', 'timeline-empty', 'DDS/장치 메시지가 없습니다. request -rcc부터 실행하세요.'));
  }
  setState(dom.hmiState, 'SPEC NOT LOADED');
  setState(dom.omState, 'IDLE');
  resetPipeline();
  if (dom.bindingSummary) dom.bindingSummary.replaceChildren(el('p', 'empty-text', '송신 후 선택 Binding이 표시됩니다.'));
  if (dom.resultBanner) {
    dom.resultBanner.className = 'result-banner';
    dom.resultBanner.replaceChildren(el('strong', '', '처리 대기'), el('small', '', '실제 장비 상태가 아닙니다.'));
  }
  if (dom.payloadTableBody) dom.payloadTableBody.replaceChildren(payloadEmpty('아직 생성된 논리 메시지가 없습니다.'));
  if (dom.copyPayloadButton) dom.copyPayloadButton.disabled = true;
  if (dom.presetSearch) dom.presetSearch.value = '';
  if (dom.presetSelection) dom.presetSelection.textContent = 'Control 검색 또는 선택';
  if (dom.presetPicker) dom.presetPicker.open = false;
  renderDraft();
  if (dom.terminalLog) {
    appendTerminal('XML 기반 Protocol Lab 준비 완료.', 'success');
    appendTerminal('request -rcc로 시작하세요.', 'info');
  }
}

function targetDeviceValue(control) {
  const target = actionTarget(control);
  const raw = String(target.targetId || '').trim();
  if (/^\d+$/.test(raw)) return Number(raw);
  const suffix = raw.match(/(\d+)\s*$/);
  return suffix ? Number(suffix[1]) : raw;
}

function renderDraft() {
  if (!dom.draftControlId || !dom.argumentChips) return;
  const draft = state.demo || {};
  dom.draftControlId.textContent = draft.controlId || '선택되지 않음';
  clear(dom.argumentChips);
  const control = draft.controlId && state.bundle ? state.bundle.controls.find((item) => item.publicId === draft.controlId) : null;
  if (control) {
    const target = actionTarget(control);
    dom.argumentChips.append(el('span', 'argument-chip auto', `targetId = ${target.targetId} (자동)`));
    if (control.target) dom.argumentChips.append(el('span', 'argument-chip auto', `${control.target.key} = ${targetDeviceValue(control)} (자동 매핑)`));
  }
  if (!draft.args || !draft.args.size) {
    dom.argumentChips.append(el('span', 'empty-text', control ? '추가 Parameter가 없습니다.' : '입력된 key-value가 없습니다.'));
  } else {
    for (const [key, rawValue] of draft.args) dom.argumentChips.append(el('span', 'argument-chip', `${key} = ${rawValue}`));
  }
}

function updateRouteOptions(control) {
  clear(dom.routePolicy);
  if (!dom.routePolicy) return;
  const auto = el('option', '', 'DEFAULT · 첫 번째 Binding');
  auto.value = 'AUTO';
  dom.routePolicy.append(auto);
  for (const binding of control.bindings) {
    const option = el('option', '', `${channelSummary(binding.channel)} · ${binding.sourceFile}`);
    option.value = binding.key;
    dom.routePolicy.append(option);
  }
}

function sampleObjectFromCollection(input) {
  const sample = {};
  const element = (input.children || []).find((child) => child.kind === 'ElementProfile');
  const children = element?.children?.length ? element.children : (input.children || []);
  for (const child of children) {
    const key = lowerFirst(child.cdm.split('.').pop());
    sample[key] = sampleValue(child);
  }
  return sample;
}

function sampleValue(input) {
  if (input.values?.length) {
    const firstValue = input.values[0];
    return firstValue.value || firstValue.cdm || firstValue.name || 'VALUE';
  }
  if (input.type === 'boolean') return true;
  if (input.type === 'collection') return [sampleObjectFromCollection(input)];
  if (/Communication\.RF\.Configuration\.OperationMode/.test(input.cdm)) return '1:1';
  if (/Communication\.RF\.Configuration\.TimeSlot/.test(input.cdm)) return 2;
  if (/Communication\.RF\.Configuration\..*\.Channel/.test(input.cdm)) return 1;
  if (/Communication\.RF\.Configuration\..*\.Power/.test(input.cdm)) return 'LOW';
  if (input.type === 'number') {
    if (Number.isFinite(input.min) && Number.isFinite(input.max)) {
      if (input.min <= 1 && input.max >= 1) return 1;
      const middle = input.min + ((input.max - input.min) / 2);
      return Number(middle.toPrecision(8));
    }
    if (Number.isFinite(input.min)) return input.min;
    if (Number.isFinite(input.max)) return input.max;
    if (/Latitude/.test(input.cdm)) return 37.1234567;
    if (/Longitude/.test(input.cdm)) return 127.1234567;
    return 1;
  }
  return 'DEMO';
}

function selectControl(id, withSample = false, preserveArgs = false) {
  const control = state.bundle.controls.find((item) => item.publicId === id);
  if (!control) return false;
  state.demo.controlId = id;
  if (!preserveArgs) state.demo.args.clear();
  if (withSample) {
    for (const input of control.inputs) {
      const sample = sampleValue(input);
      state.demo.args.set(input.key, typeof sample === 'string' ? sample : JSON.stringify(sample));
    }
  }
  updateRouteOptions(control);
  setState(dom.hmiState, 'DRAFTING');
  renderDraft();
  if (dom.presetSelection) dom.presetSelection.textContent = `${control.name} · ${control.publicId}`;
  resetPipeline();
  appendTerminal(`Control 선택: ${id}`, 'success');
  const target = actionTarget(control);
  appendTerminal(`자동 Target: targetId=${target.targetId}${control.target ? ` → ${control.target.key}=${targetDeviceValue(control)}` : ''}`, 'info');
  appendTerminal('입력 가능한 key:', 'info');
  if (control.inputs.length) control.inputs.forEach((input) => appendTerminal(`  ${input.key} · ${input.cdm}`, 'info'));
  else appendTerminal('  입력 없음', 'info');
  return true;
}

function renderPresets(filter = '') {
  clear(dom.presetList);
  if (!state.bundle || !dom.presetList) return;
  const query = filter.trim().toLowerCase();
  const controls = state.bundle.controls.filter((control) => (
    !query
    || control.publicId.toLowerCase().includes(query)
    || control.name.toLowerCase().includes(query)
    || control.cdm.toLowerCase().includes(query)
  ));
  const groups = new Map();
  for (const control of controls) {
    const label = control.ref.name || control.ref.id || 'UMS';
    if (!groups.has(label)) groups.set(label, []);
    groups.get(label).push(control);
  }
  for (const [label, items] of groups) {
    const group = el('section', 'preset-group');
    group.append(el('h4', '', `${label} · ${items.length}`));
    for (const control of items) {
      const button = el('button');
      button.type = 'button';
      button.dataset.presetControl = control.publicId;
      button.append(el('strong', '', control.name), el('code', '', control.publicId));
      group.append(button);
    }
    dom.presetList.append(group);
  }
  if (!controls.length) dom.presetList.append(el('p', 'empty-text', '일치하는 Control이 없습니다.'));
}

function renderCatalog(filter = '') {
  clear(dom.hmiCatalog);
  if (!state.bundle || !dom.hmiCatalog) return;
  const query = filter.trim().toLowerCase();
  for (const control of state.bundle.controls.filter((item) => (
    !query
    || item.publicId.toLowerCase().includes(query)
    || item.cdm.toLowerCase().includes(query)
    || item.name.toLowerCase().includes(query)
  ))) {
    const card = el('article', 'catalog-card');
    card.append(el('h3', '', control.name), el('code', '', control.publicId), el('p', '', control.cdm));
    const protocols = [...new Set(control.bindings.map((binding) => binding.transport))].join('/') || 'No Binding';
    card.append(el('p', '', `${control.inputs.length} input · ${control.bindings.length} variant · ${protocols} · ${control.replies.length ? 'Reply' : 'No Reply'}`));
    const button = el('button', 'button ghost', 'Draft로 선택');
    button.dataset.selectControl = control.publicId;
    card.append(button);
    dom.hmiCatalog.append(card);
  }
  if (!dom.hmiCatalog.children.length) dom.hmiCatalog.append(el('p', 'empty-text', '검색 결과가 없습니다.'));
}

function setupHmiContractUi() {
  if (!dom.hmiWorkspace || dom.hmiContractLayout) return;

  document.title = 'UMS XML Reference';
  const brandTitle = document.querySelector('.brand strong');
  const brandSub = document.querySelector('.brand small');
  const brandLink = document.querySelector('.brand');
  if (brandTitle) brandTitle.textContent = 'UMS XML Reference';
  if (brandSub) brandSub.textContent = 'Semantic · Binding · HMI Contract';
  if (brandLink) brandLink.href = '#hmi';
  const tabLabels = {
    hmi: ['01', '기능 문서', 'Semantic · Binding Reference'],
    model: ['02', '관계 진단', 'Specification · XSD 관계'],
    cdm: ['03', 'CDM 참고', '공통 모델과 Extension'],
  };
  for (const [name, labels] of Object.entries(tabLabels)) {
    const button = document.querySelector(`[data-tab=${name}]`);
    if (!button) continue;
    const number = button.querySelector('b');
    const title = button.querySelector('strong');
    const note = button.querySelector('small');
    if (number) number.textContent = labels[0];
    if (title) title.textContent = labels[1];
    if (note) note.textContent = labels[2];
  }
  const footerLabels = document.querySelectorAll('footer span');
  if (footerLabels[0]) footerLabels[0].textContent = 'UMS XML Reference · static offline documentation';
  if (footerLabels[1]) footerLabels[1].textContent = 'Semantic · Binding 설명 · 논리 메시지 Mock · 실제 장비 송수신 아님';
  const emptyTitle = dom.hmiEmptyState?.querySelector('strong');
  const emptyNote = dom.hmiEmptyState?.querySelector('small');
  if (emptyTitle) emptyTitle.textContent = 'XML 설계 폴더를 선택하세요';
  if (emptyNote) emptyNote.textContent = 'Specification · Semantic · Binding을 연결해 기능 문서를 생성합니다.';
  if (dom.sourceStatusText && !state.bundle) dom.sourceStatusText.textContent = 'XML 폴더 선택 · 기능 문서 생성';

  const oldFlow = dom.hmiWorkspace.querySelector('.flow-strip');
  const oldCatalog = dom.hmiWorkspace.querySelector('.catalog-section');
  if (oldFlow) oldFlow.hidden = true;
  if (oldCatalog) oldCatalog.hidden = true;

  const layout = el('section', 'protocol-docs-layout');
  layout.id = 'hmiContractLayout';

  const navigator = el('aside', 'docs-navigation');
  const navHead = el('header', 'docs-pane-head');
  navHead.append(el('p', '', 'UMS XML REFERENCE'), el('h2', '', '기능 목차'));
  const deviceLabel = el('label', 'hmi-device-label');
  deviceLabel.append(el('span', '', '장치 문서'));
  const deviceSelect = el('select');
  deviceSelect.id = 'hmiDeviceSelect';
  deviceLabel.append(deviceSelect);
  const kindTabs = el('div', 'hmi-kind-tabs');
  for (const kind of ['Control', 'Monitor', 'Product']) {
    const button = el('button', kind === 'Control' ? 'is-active' : '', kind);
    button.type = 'button';
    button.dataset.hmiKind = kind;
    kindTabs.append(button);
  }
  const search = el('input');
  search.id = 'hmiFunctionSearch';
  search.type = 'search';
  search.placeholder = '기능명, ID, CDM 검색';
  const list = el('div', 'hmi-function-list');
  list.id = 'hmiFunctionList';
  navigator.append(navHead, deviceLabel, kindTabs, search, list);

  const contract = el('article', 'docs-article');
  const contractHead = el('header', 'docs-pane-head docs-article-head');
  const contractTitle = el('div');
  contractTitle.append(el('p', '', 'SEMANTIC DOCUMENT'), el('h2', '', '기능 설명서'));
  const catalogCommand = el('div', 'hmi-catalog-command');
  catalogCommand.append(el('code', '', 'request -rcc'));
  const catalogCopy = el('button', 'button ghost', '복사');
  catalogCopy.type = 'button';
  catalogCopy.dataset.copyText = 'request -rcc';
  catalogCommand.append(catalogCopy);
  contractHead.append(contractTitle, catalogCommand);
  const contractBody = el('div', 'hmi-contract-body');
  contractBody.id = 'hmiContractBody';
  contract.append(contractHead, contractBody);

  const response = el('aside', 'docs-source');
  const responseHead = el('header', 'docs-pane-head');
  responseHead.append(el('p', '', 'BINDING & SOURCE'), el('h2', '', '전송 정의·XML 근거'));
  const responseBody = el('div', 'hmi-response-body');
  responseBody.id = 'hmiResponseBody';
  response.append(responseHead, responseBody);

  layout.append(navigator, contract, response);
  dom.hmiWorkspace.prepend(layout);

  const legacyLayout = dom.hmiWorkspace.querySelector('.hmi-layout');
  if (legacyLayout) {
    const demo = el('details', 'hmi-demo-drawer');
    demo.id = 'hmiDemoDrawer';
    const summary = el('summary');
    summary.append(el('span', '', '터미널·Mock 실행 데모'), el('small', '', '필요할 때 펼치기'));
    demo.append(summary, legacyLayout);
    dom.hmiWorkspace.append(demo);
    dom.hmiDemoDrawer = demo;
  }

  dom.hmiContractLayout = layout;
  dom.hmiDeviceSelect = deviceSelect;
  dom.hmiKindTabs = kindTabs;
  dom.hmiFunctionSearch = search;
  dom.hmiFunctionList = list;
  dom.hmiContractBody = contractBody;
  dom.hmiResponseBody = responseBody;
}

function flattenSemanticProfiles(profiles) {
  const output = [];
  for (const profile of profiles || []) {
    if (profile.cdm || profile.name) output.push(profile);
    output.push(...flattenSemanticProfiles(profile.children));
  }
  return output;
}

function profileAllowedLabel(profile) {
  if (profile.values?.length) {
    return profile.values.map((item) => item.name || item.value || item.cdm).filter(Boolean).join(', ');
  }
  const range = profileRangeLabel(profile);
  if (range !== '—') return range;
  if (profile.defaultValue !== '') return `기본값 ${profile.defaultValue}`;
  return '—';
}

function hmiExampleValue(profile) {
  if (profile.values?.length) {
    const firstValue = profile.values[0];
    return firstValue.value || firstValue.cdm || firstValue.name || 'VALUE';
  }
  const sample = sampleValue(profile);
  return typeof sample === 'string' ? sample : JSON.stringify(sample);
}

function hmiProfileTable(title, profiles, inputMode = false) {
  const section = el('section', 'hmi-contract-section');
  section.append(el('h3', '', title));
  if (!profiles.length) {
    section.append(el('p', 'semantic-empty', inputMode ? 'HMI 입력 없음' : '표시할 의미 항목 없음'));
    return section;
  }
  const wrap = el('div', 'hmi-contract-table-wrap');
  const table = el('table', 'hmi-contract-table');
  const head = el('thead');
  const headRow = el('tr');
  const headings = inputMode
    ? ['입력 Key', 'CDM 의미', '형식', '범위·선택값', '명령']
    : ['출력 항목', 'CDM 의미', '형식', '범위·선택값'];
  headings.forEach((heading) => headRow.append(el('th', '', heading)));
  head.append(headRow);
  const body = el('tbody');
  for (const profile of profiles) {
    const row = el('tr');
    const key = inputMode ? profile.key : (profile.name || lowerFirst(profile.cdm.split('.').pop()));
    const keyCell = el('td');
    keyCell.append(el('code', '', key || '미정'));
    if (inputMode && !profile.required) keyCell.append(el('small', 'optional-mark', '선택'));
    row.append(
      keyCell,
      el('td', '', profile.cdm || '미정'),
      el('td', '', profileTypeLabel(profile.type)),
      el('td', '', profileAllowedLabel(profile)),
    );
    if (inputMode) {
      const command = `control -a ${key},${hmiExampleValue(profile)}`;
      const commandCell = el('td', 'hmi-command-cell');
      commandCell.append(el('code', '', command));
      const copy = el('button', 'icon-copy', '복사');
      copy.type = 'button';
      copy.dataset.copyText = command;
      commandCell.append(copy);
      row.append(commandCell);
    }
    body.append(row);
  }
  table.append(head, body);
  wrap.append(table);
  section.append(wrap);
  return section;
}

function fieldRuleLabel(field) {
  const values = [];
  if (field.fixedValue !== '') values.push(`고정값 ${field.fixedValue}`);
  else if (field.value !== '') values.push(`값 ${field.value}`);
  if (field.cdm) values.push(field.cdm);
  if (field.sourceFields?.length) values.push(`source: ${field.sourceFields.join(', ')}`);
  if (field.converter) values.push(`converter: ${field.converter}`);
  if (field.dataType) values.push(field.dataType);
  if (field.offset !== undefined && field.offset !== '') values.push(`offset ${field.offset}`);
  if (field.width) values.push(`width ${field.width}`);
  if (field.maps?.length) values.push(`map ${field.maps.length}개`);
  return values.join(' · ') || '별도 매핑 규칙 없음';
}

function docsFieldTable(title, fields) {
  const details = el('details', 'docs-field-details');
  const flat = flattenFields(fields);
  const summary = el('summary');
  summary.append(el('span', '', title), el('small', '', `${flat.length} fields · 펼치기`));
  details.append(summary);
  if (!flat.length) {
    details.append(el('p', 'semantic-empty', '정의된 Field 없음'));
    return details;
  }
  const wrap = el('div', 'docs-field-table-wrap');
  const table = el('table', 'docs-field-table');
  const head = el('thead');
  const headRow = el('tr');
  ['Field', 'Kind', 'Value / CDM / Converter'].forEach((label) => headRow.append(el('th', '', label)));
  head.append(headRow);
  const body = el('tbody');
  for (const field of flat) {
    const row = el('tr');
    row.append(el('td', '', field.name || '이름 없음'), el('td', '', field.kind), el('td', '', fieldRuleLabel(field)));
    body.append(row);
  }
  table.append(head, body);
  wrap.append(table);
  details.append(wrap);
  return details;
}

function docsChannelCard(title, channel, fields, reply = null) {
  const section = el('section', 'docs-binding-message');
  const heading = el('header');
  heading.append(el('h3', '', title));
  if (reply && (!reply.required || reply.timeout)) {
    heading.append(el('small', '', `${reply.required ? '' : '선택 응답'}${reply.timeout ? ` timeout ${reply.timeout}` : ''}`.trim()));
  }
  section.append(heading);
  const meta = el('dl', 'docs-meta-grid compact');
  for (const [label, raw] of [...channelRows(channel), ['Field', `${flattenFields(fields).length}개`]]) {
    const item = el('div');
    item.append(el('dt', '', label), el('dd', '', raw || '—'));
    meta.append(item);
  }
  section.append(meta, docsFieldTable(`${title} Field`, fields));
  return section;
}

function xmlExcerpt(file, action, binding = null) {
  const lines = String(file?.text || '').split(/\r?\n/);
  if (!lines.length) return { start: 1, lines: ['XML 원문 없음'] };
  const id = action.semanticId;
  const attribute = binding ? 'semantic_id' : 'id';
  let hit = lines.findIndex((line) => line.includes(attribute) && line.includes(id));
  if (hit < 0) hit = lines.findIndex((line) => line.includes(id));
  if (hit < 0) return { start: 1, lines: lines.slice(0, 80) };

  let start = hit;
  while (start > Math.max(0, hit - 10) && !/<[A-Za-z_:][\w:.-]*/.test(lines[start])) start -= 1;
  const opening = lines[start].match(/<([A-Za-z_:][\w:.-]*)/);
  let end = Math.min(lines.length - 1, start + 100);
  if (opening && !/\/>\s*$/.test(lines[start])) {
    const tag = opening[1].replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const openPattern = new RegExp(`<${tag}(?:\\s|>|/)`, 'g');
    const closePattern = new RegExp(`</${tag}>`, 'g');
    let depth = 0;
    for (let index = start; index < Math.min(lines.length, start + 160); index += 1) {
      depth += (lines[index].match(openPattern) || []).length;
      depth -= (lines[index].match(closePattern) || []).length;
      if (depth === 0 && index >= hit) {
        end = index;
        break;
      }
    }
  }
  return { start: start + 1, lines: lines.slice(start, end + 1) };
}

function sourceFileFor(action, binding) {
  if (!state.bundle) return null;
  if (!binding) return action.ref?.semanticFile || state.bundle.files.find((file) => file.path === action.sourcePath);
  return state.bundle.files.find((file) => file.path === binding.sourcePath)
    || action.ref?.bindingFiles.find((file) => file.name === binding.sourceFile)
    || null;
}

function docsXmlSource(action, binding) {
  const section = el('section', 'docs-xml-source');
  const tabs = el('div', 'docs-source-tabs');
  const semanticTab = el('button', state.hmiSourceKey === 'semantic' ? 'is-active' : '', 'Semantic');
  semanticTab.type = 'button';
  semanticTab.dataset.hmiSource = 'semantic';
  tabs.append(semanticTab);
  for (const variant of action.bindings) {
    const button = el('button', state.hmiSourceKey === variant.key ? 'is-active' : '', variant.transport);
    button.type = 'button';
    button.dataset.hmiSource = variant.key;
    button.title = variant.sourceFile;
    tabs.append(button);
  }

  const sourceBinding = action.bindings.find((item) => item.key === state.hmiSourceKey) || null;
  const file = sourceFileFor(action, sourceBinding);
  const head = el('header', 'docs-source-file');
  head.append(el('strong', '', file?.name || '원문 파일 없음'), el('small', '', file?.path || '참조 경로를 확인하세요.'));
  section.append(tabs, head);
  if (!file) {
    section.append(el('p', 'semantic-empty', '표시할 XML 원문이 없습니다.'));
    return section;
  }

  const excerpt = xmlExcerpt(file, action, sourceBinding);
  const pre = el('pre', 'xml-source-code');
  excerpt.lines.forEach((line, index) => {
    const row = el('span', line.includes(action.semanticId) ? 'is-hit' : '');
    row.append(el('i', '', excerpt.start + index), el('code', '', line || ' '));
    pre.append(row);
  });
  section.append(pre);
  return section;
}

function renderHmiResponse(action) {
  clear(dom.hmiResponseBody);
  if (!action) {
    dom.hmiResponseBody.append(el('p', 'semantic-empty', '메시지를 선택하면 Binding과 XML 원문을 표시합니다.'));
    return;
  }

  let binding = action.bindings.find((item) => item.key === state.hmiBindingKey);
  if (!binding) {
    binding = action.bindings[0] || null;
    state.hmiBindingKey = binding?.key || '';
  }
  if (state.hmiSourceKey !== 'semantic' && !action.bindings.some((item) => item.key === state.hmiSourceKey)) {
    state.hmiSourceKey = 'semantic';
  }

  const overview = el('section', 'docs-binding-overview');
  const heading = el('div', 'docs-section-title');
  heading.append(el('div', '', 'Binding variant'), el('strong', '', `${action.bindings.length}개`));
  overview.append(heading);
  if (action.bindings.length > 1) {
    const selector = el('div', 'docs-binding-tabs');
    for (const variant of action.bindings) {
      const button = el('button', variant.key === state.hmiBindingKey ? 'is-active' : '');
      button.type = 'button';
      button.dataset.hmiBinding = variant.key;
      button.append(el('strong', '', variant.transport), el('small', '', variant.sourceFile));
      selector.append(button);
    }
    overview.append(selector);
  }

  if (!binding) {
    overview.append(el('p', 'docs-warning', '연결된 Binding이 없습니다. Semantic 정의와 Specification 참조를 확인하세요.'));
  } else {
    overview.append(el('p', 'docs-file-label', `${binding.transport} · ${binding.sourceFile}`));
    const requestName = action.kind === 'Control' ? 'Request' : action.kind;
    overview.append(docsChannelCard(requestName, binding.channel, binding.fields));
    for (const reply of binding.replies) {
      overview.append(docsChannelCard(`Reply · ${reply.semanticId}`, reply.channel, reply.fields, reply));
    }
  }
  dom.hmiResponseBody.append(overview, docsXmlSource(action, binding));
}

function renderHmiContract(action) {
  clear(dom.hmiContractBody);
  if (!action) {
    dom.hmiContractBody.append(el('p', 'semantic-empty', '왼쪽 목차에서 기능을 선택하세요.'));
    renderHmiResponse(null);
    return;
  }

  const articleHead = el('section', 'docs-action-head');
  articleHead.append(el('p', 'docs-breadcrumb', `${action.ref?.name || action.ref?.id || '장치'} / ${action.kind} / ${action.semanticId}`));
  const trace = el('div', 'docs-trace-path');
  [
    ['Specification', action.ref?.vehicleFile?.name || '미정'],
    ['Semantic', action.sourceFile || '미정'],
    ['Binding', `${action.bindings.length} variant`],
  ].forEach(([label, raw], index) => {
    if (index) trace.append(el('i', '', '→'));
    const node = el('span');
    node.append(el('small', '', label), el('strong', '', raw));
    trace.append(node);
  });
  articleHead.append(trace);
  const titleRow = el('div');
  const title = el('div');
  title.append(el('span', `feature-badge ${action.kind.toLowerCase()}`, action.kind), el('h1', '', action.name));
  const idCopy = el('button', 'button ghost', 'ID 복사');
  idCopy.type = 'button';
  idCopy.dataset.copyText = action.publicId;
  titleRow.append(title, idCopy);
  articleHead.append(titleRow, el('code', 'hmi-full-id', action.publicId));
  articleHead.append(el('p', 'docs-lead', action.description || (
    action.kind === 'Control'
      ? 'HMI에서 요청하여 장치 동작 또는 설정을 변경하는 Control 메시지입니다.'
      : action.kind === 'Monitor'
        ? '장치가 송신하며 HMI가 지속적으로 수신·표시하는 Monitor 메시지입니다.'
        : '장치가 생성하는 파일·프레임·스트림 형태의 Product 메시지입니다.'
  )));

  const meta = el('dl', 'docs-meta-grid');
  [
    ['CDM', action.cdm || '미정'],
    ['SpecRef', action.ref?.id || '미정'],
    ['Semantic', action.sourceFile],
    ['Local ID', action.semanticId],
    ['Binding', `${action.bindings.length} variant`],
  ].forEach(([label, raw]) => {
    const item = el('div');
    item.append(el('dt', '', label), el('dd', '', raw));
    meta.append(item);
  });
  articleHead.append(meta);
  dom.hmiContractBody.append(articleHead);

  if (action.kind === 'Control') {
    const commands = [
      'request -rcc',
      `control -i ${action.publicId}`,
      ...action.inputs.map((input) => `control -a ${input.key},${hmiExampleValue(input)}`),
      'control -r',
    ];
    const commandSection = el('section', 'hmi-command-sequence docs-section');
    const commandHead = el('header');
    commandHead.append(el('div', '', '01'), el('h2', '', 'HMI 호출 순서'));
    const copyAll = el('button', 'button primary', '명령 전체 복사');
    copyAll.type = 'button';
    copyAll.dataset.copyText = commands.join('\n');
    commandHead.append(copyAll);
    const pre = el('pre');
    pre.append(el('code', '', commands.join('\n')));
    commandSection.append(commandHead, pre);
    dom.hmiContractBody.append(commandSection, hmiProfileTable('02 · HMI 입력', action.inputs, true));

    const replySection = el('section', 'docs-section docs-replies');
    replySection.append(el('h2', '', `03 · Reply 출력 (${action.replies.length})`));
    if (!action.replies.length) replySection.append(el('p', 'semantic-empty', 'Semantic에 Reply가 정의되어 있지 않습니다.'));
    for (const reply of action.replies) {
      const card = el('article', 'hmi-reply-card');
      const head = el('header');
      head.append(el('code', '', reply.bindRef || 'bindRef 미정'));
      if (!reply.required || reply.timeout) {
        head.append(el('small', '', `${reply.required ? '' : '선택 응답'}${reply.timeout ? ` timeout ${reply.timeout}` : ''}`.trim()));
      }
      card.append(head, el('p', '', reply.cdm || 'CDM 미정'));
      card.append(hmiProfileTable('HMI 출력 항목', flattenSemanticProfiles(reply.results), false));
      replySection.append(card);
    }
    dom.hmiContractBody.append(replySection);

    if (action.preconditions?.length) {
      const conditions = el('section', 'docs-section docs-conditions');
      conditions.append(el('h2', '', '실행 전제조건'));
      const list = el('ul');
      action.preconditions.forEach((item) => list.append(el('li', '', item.cdm)));
      conditions.append(list);
      dom.hmiContractBody.append(conditions);
    }
    const tryIt = el('details', 'docs-try-it');
    const trySummary = el('summary');
    trySummary.append(el('strong', '', 'Try it · 터미널과 Mock으로 확인'), el('small', '', '보조 기능 펼치기'));
    const demoButton = el('button', 'button ghost hmi-open-demo', '이 Control로 터미널 데모 열기');
    demoButton.type = 'button';
    demoButton.dataset.hmiOpenDemo = action.publicId;
    tryIt.append(trySummary, demoButton);
    dom.hmiContractBody.append(tryIt);
  } else if (action.kind === 'Monitor') {
    dom.hmiContractBody.append(el('p', 'docs-info', 'Monitor는 control -i로 호출하지 않습니다. HMI가 수신하여 아래 의미 항목을 표시합니다.'));
    dom.hmiContractBody.append(hmiProfileTable('HMI 표시 항목', flattenSemanticProfiles(action.outputs), false));
  } else {
    const product = el('section', 'docs-section');
    product.append(el('h2', '', 'Product 정의'));
    product.append(makeDl([
      ['형태', action.productType || '미정'],
      ['종류', action.productKind || '미정'],
      ['처리 상태', action.processingState || '미정'],
    ]));
    dom.hmiContractBody.append(product);
  }
  renderHmiResponse(action);
}

function renderHmiWorkspace() {
  if (!state.bundle || !dom.hmiDeviceSelect) return;
  const refs = state.bundle.refs;
  if (!refs.length) return;
  const previousDevice = state.hmiDeviceKey;
  clear(dom.hmiDeviceSelect);
  for (const ref of refs) {
    const option = el('option', '', `${ref.name || ref.id} · ${ref.actions.length}개 메시지`);
    option.value = referenceKey(ref);
    dom.hmiDeviceSelect.append(option);
  }
  if (!refs.some((ref) => referenceKey(ref) === previousDevice)) state.hmiDeviceKey = referenceKey(refs[0]);
  dom.hmiDeviceSelect.value = state.hmiDeviceKey;

  const activeRef = refs.find((ref) => referenceKey(ref) === state.hmiDeviceKey) || refs[0];
  const query = (dom.hmiFunctionSearch?.value || '').trim().toLowerCase();
  const actions = activeRef.actions.filter((action) => (
    action.kind === state.hmiKind
    && (!query
      || action.publicId.toLowerCase().includes(query)
      || action.name.toLowerCase().includes(query)
      || action.cdm.toLowerCase().includes(query))
  ));

  for (const button of dom.hmiKindTabs.querySelectorAll('[data-hmi-kind]')) {
    const count = activeRef.actions.filter((action) => action.kind === button.dataset.hmiKind).length;
    button.textContent = `${button.dataset.hmiKind} ${count}`;
    button.classList.toggle('is-active', button.dataset.hmiKind === state.hmiKind);
  }

  let selectedAction = actions.find((item) => item.publicId === state.hmiActionId);
  if (!selectedAction) {
    selectedAction = actions[0] || null;
    state.hmiActionId = selectedAction?.publicId || '';
  }

  clear(dom.hmiFunctionList);
  for (const action of actions) {
    const summary = action.kind === 'Control'
      ? `${action.cdm || 'CDM 미정'} · 입력 ${action.inputs.length} · Reply ${action.replies.length}`
      : action.kind === 'Monitor'
        ? `${action.cdm || 'CDM 미정'} · 출력 ${flattenSemanticProfiles(action.outputs).length} · Binding ${action.bindings.length}`
        : `${action.cdm || 'CDM 미정'} · Product · Binding ${action.bindings.length}`;
    const button = el('button', action.publicId === state.hmiActionId ? 'is-active' : '');
    button.type = 'button';
    button.dataset.hmiAction = action.publicId;
    button.append(
      el('strong', '', action.name),
      el('code', '', action.publicId),
      el('small', '', summary),
    );
    dom.hmiFunctionList.append(button);
  }
  if (!actions.length) dom.hmiFunctionList.append(el('p', 'semantic-empty', '해당 메시지 없음'));

  renderHmiContract(selectedAction);
}

function makeRequestId(prefix) {
  return `${prefix}-${String(state.demo.requestId++).padStart(6, '0')}`;
}

function makeDdsIdentity() {
  return `DDS-SAMPLE-${String(state.demo.ddsSequence++).padStart(6, '0')}`;
}

async function requestSpecs() {
  if (!state.bundle) {
    appendTerminal('먼저 XML 폴더를 분석하세요.', 'error');
    return;
  }
  const controls = state.bundle.controls;
  const registeredSpecRefs = [...new Set(state.bundle.refs.map((ref) => ref.id).filter(Boolean))];
  const sampleIdentity = makeDdsIdentity();
  const requestPayload = { targetId: 'ALL', targetName: 'ALL' };

  setState(dom.omState, 'CATALOG BUILD');
  addBus(OM_DDS_TYPES.controlSpecListRequest, 'HMI CSC → OPERATION MANAGEMENT CSC', requestPayload, 'dds', {
    transport: 'DDS',
    sampleIdentity,
    scope: 'CURRENT_LOADED_UMS_CONTROLS',
    source: state.bundle.operationManagementIdl ? 'OperationManagement.idl' : 'built-in contract',
    targetRole: 'Catalog scope sentinel (ALL)',
  });
  await delay(70);

  const replyPayload = {
    targetId: 'ALL',
    targetName: 'ALL',
    numsOfControlSpecs: controls.length,
    controlSpecList: controls.map((control) => controlSpecPayload(control, null, true)),
  };
  addBus(OM_DDS_TYPES.controlSpecListReply, 'OPERATION MANAGEMENT CSC → HMI CSC', replyPayload, 'dds', {
    transport: 'DDS',
    relatedSampleIdentity: sampleIdentity,
    registeredSpecRefs: registeredSpecRefs.join(', ') || 'none',
    meaning: '현재 로드된 UMS에서 OPERATION MANAGEMENT CSC가 수행 가능한 전체 Control Draft 카탈로그',
  });

  state.demo.specLoaded = true;
  setState(dom.hmiState, 'SPEC READY');
  setState(dom.omState, 'IDLE');
  appendTerminal(`현재 로드된 UMS의 Control Draft ${controls.length}개를 단일 Reply로 수신했습니다.`, 'success');
  if (registeredSpecRefs.length) appendTerminal(`등록 대상: ${registeredSpecRefs.join(', ')}`, 'info');
  appendTerminal('control -i <full-id>로 Draft를 시작하세요.', 'info');
  appendTerminal('사용 가능한 Control Full ID:', 'info');
  appendControlIdList(controls);
}

function parseArgument(input, raw) {
  if (input.type === 'boolean') {
    if (/^(true|1|on)$/i.test(raw)) return { ok: true, value: true };
    if (/^(false|0|off)$/i.test(raw)) return { ok: true, value: false };
    return { ok: false, message: `${input.key}는 boolean이어야 합니다.` };
  }
  if (input.type === 'collection') {
    if (Array.isArray(raw)) return { ok: true, value: raw };
    try {
      const parsed = JSON.parse(raw);
      return Array.isArray(parsed)
        ? { ok: true, value: parsed }
        : { ok: false, message: `${input.key}는 JSON 배열이어야 합니다.` };
    } catch {
      return { ok: false, message: `${input.key} JSON 배열 문법 오류` };
    }
  }
  if (input.type === 'number') {
    const parsed = Number(raw);
    if (!Number.isFinite(parsed)) return { ok: false, message: `${input.key}는 숫자여야 합니다.` };
    if ((input.min !== null && parsed < input.min) || (input.max !== null && parsed > input.max)) {
      const range = `${input.min ?? '…'}..${input.max ?? '…'}${input.unit ? ` ${input.unit}` : ''}`;
      return { ok: false, message: `${input.key} 범위 오류: ${range}만 입력할 수 있습니다.` };
    }
    return { ok: true, value: parsed };
  }
  if (input.type === 'valueSet' && input.values?.length) {
    const text = String(raw).trim();
    const normalized = text.toLowerCase();
    const selected = input.values.find((item) => [
      item.value,
      item.name,
      item.cdm,
      item.cdm?.split('.').pop(),
    ].filter(Boolean).some((candidate) => String(candidate).toLowerCase() === normalized));
    if (!selected) {
      const allowed = input.values.map((item) => item.name || item.value || item.cdm?.split('.').pop()).filter(Boolean);
      return { ok: false, message: `${input.key} 허용값: ${allowed.join(', ')}` };
    }
    return { ok: true, value: selected.value || selected.cdm || selected.name };
  }
  return { ok: true, value: raw };
}

function validateControl(control) {
  const definitions = new Map(control.inputs.map((input) => [input.key, input]));
  const values = {};
  for (const input of control.inputs) {
    if (input.defaultValue === '') continue;
    const parsed = parseArgument(input, input.defaultValue);
    if (parsed.ok) values[input.key] = parsed.value;
  }
  for (const [key, raw] of state.demo.args) {
    const input = definitions.get(key);
    if (!input) return { ok: false, code: 'UNKNOWN_ARGUMENT', message: `알 수 없는 key ${key}. 허용: ${[...definitions.keys()].join(', ')}` };
    const parsed = parseArgument(input, raw);
    if (!parsed.ok) return { ok: false, code: 'TYPE_OR_RANGE', message: parsed.message };
    values[key] = parsed.value;
  }
  for (const input of control.inputs) {
    if (input.required && !Object.hasOwn(values, input.key)) return { ok: false, code: 'MISSING_ARGUMENT', message: `필수 key ${input.key}가 없습니다.` };
  }
  return { ok: true, values };
}

function resolveBinding(control) {
  return dom.routePolicy.value === 'AUTO'
    ? control.bindings[0]
    : control.bindings.find((item) => item.key === dom.routePolicy.value);
}

function sourceValue(source, control, values, physical) {
  if (control.target && (source === control.target.cdm || source === control.target.key || source === 'System.Target.DeviceId')) return targetDeviceValue(control);
  if (source.startsWith('System.Target.')) return targetDeviceValue(control);
  const input = control.inputs.find((item) => item.cdm === source || item.key === source);
  if (input && Object.hasOwn(values, input.key)) return values[input.key];
  if (Object.hasOwn(physical, source)) return physical[source];
  if (source === 'System.Time.Now') return state.demo.now;
  if (source === 'System.Generated.HeartbeatSequence') return state.demo.sequence;
  if (source === 'System.Generated.Sequence') return state.demo.sequence;
  if (source === 'System.Generated.CommandSequence') return state.demo.sequence;
  if (source === 'System.Generated.USVMessageBase') return `USV_HEADER_${state.demo.sequence}`;
  if (/^System\.Generated\./.test(source)) return state.demo.sequence;
  if (source === 'System.Context.CommandID') return state.demo.sequence;
  if (source === 'System.Frame.Checksum') return 'MOCK_CHECKSUM';
  if (source === 'System.Communication.LastReceivedCompletionCommand') return state.demo.lastCompletionCommand || 0;
  return undefined;
}

function numericValue(raw) {
  if (typeof raw === 'number' && Number.isFinite(raw)) return raw;
  if (typeof raw === 'boolean') return raw ? 1 : 0;
  if (typeof raw === 'string') {
    const text = raw.trim();
    if (/^[-+]?0x[0-9a-f]+$/i.test(text)) return Number.parseInt(text, 16);
    const parsed = Number(text);
    if (Number.isFinite(parsed)) return parsed;
  }
  return null;
}

function declaredMapValue(field, raw) {
  if (raw === undefined) return undefined;
  const text = String(raw);
  const sourceMap = field.maps?.find((item) => item.kind === 'SourceValueMap' && item.sourceValue === text);
  if (sourceMap) return sourceMap.value;
  const valueMap = field.maps?.find((item) => (
    item.kind === 'ValueMap'
    && (item.cdm === text || item.cdm?.split('.').pop() === text || item.value === text)
  ));
  return valueMap ? valueMap.value : raw;
}

function packedFieldValue(field, control, values, physical) {
  let packed = BigInt(numericValue(field.defaultValue) ?? 0);
  for (const member of field.children || []) {
    let raw;
    if (member.fixedValue !== '') raw = member.fixedValue;
    else if (member.sourceFields.length) raw = sourceValue(member.sourceFields[0], control, values, physical);
    else raw = fieldInput(member, control, values)?.value;
    if (raw === undefined) {
      if (!member.cdm && !member.sourceFields.length) raw = 0;
      else return { ok: false, value: `TBD bit ${member.name}` };
    }
    raw = declaredMapValue(member, raw);
    const numeric = numericValue(raw);
    if (numeric === null || !Number.isInteger(numeric)) return { ok: false, value: `TBD bit ${member.name}` };
    const offset = BigInt(Number(member.offset || 0));
    const width = BigInt(Number(member.width || 1));
    const mask = ((1n << width) - 1n) << offset;
    packed = (packed & ~mask) | ((BigInt(numeric) << offset) & mask);
  }
  const totalWidth = Number(field.width || 32);
  return { ok: true, value: totalWidth > 32 ? `0x${packed.toString(16).toUpperCase()}` : Number(packed) };
}

function derivedValue(field, control, values, physical) {
  const sources = field.sourceFields.map((source) => sourceValue(source, control, values, physical));
  const converter = field.converter;
  const now = state.demo.now || new Date();

  if (!converter && sources.length && sources[0] !== undefined) {
    return { ok: true, value: declaredMapValue(field, sources[0]) };
  }

  if (/^TargetAuvToInformation[123]$/.test(converter)) {
    const kind = Number(converter.slice(-1));
    const target = Number(sources[0]);
    if ([1, 2].includes(target)) return { ok: true, value: target === 2 ? kind + 3 : kind };
  }
  if ((converter === 'ArrayLengthUInt8' || converter === 'ArrayLengthUInt16') && Array.isArray(sources[0])) return { ok: true, value: sources[0].length };
  if (converter === 'IncrementingUInt32LE' || converter === 'UInt16' || converter === 'UInt16LE' || converter === 'UInt32LE' || converter === 'Float32LE' || converter === 'Int8') {
    return { ok: true, value: sources[0] ?? state.demo.sequence };
  }
  if (converter === 'YearToUInt16LE') return { ok: true, value: now.getFullYear() };
  if (converter === 'MonthToUInt8') return { ok: true, value: now.getMonth() + 1 };
  if (converter === 'DateToUInt8') return { ok: true, value: now.getDate() };
  if (converter === 'HourToUInt8') return { ok: true, value: now.getHours() };
  if (converter === 'MinuteToUInt8') return { ok: true, value: now.getMinutes() };
  if (converter === 'SecondToUInt8') return { ok: true, value: now.getSeconds() };
  if (converter === 'MillisecondToUInt16LE') return { ok: true, value: now.getMilliseconds() };
  if (converter === 'DecimalToInt32Scale10000000LE' && Number.isFinite(Number(sources[0]))) return { ok: true, value: Math.round(Number(sources[0]) * 10000000) };
  if (converter === 'DecimalToUInt16Scale10LE' && Number.isFinite(Number(sources[0]))) return { ok: true, value: Math.round(Number(sources[0]) * 10) };
  if (converter === 'PackDataTypeBits' && sources.every((item) => typeof item === 'boolean')) return { ok: true, value: sources.reduce((mask, item, index) => mask | (item ? (1 << index) : 0), 0) };
  if (converter === 'Bit16' && Number.isFinite(Number(sources[0]))) return { ok: true, value: (Number(sources[0]) >> 16) & 1 };
  if (converter === 'Bit17' && Number.isFinite(Number(sources[0]))) return { ok: true, value: (Number(sources[0]) >> 17) & 1 };
  return { ok: false, value: `TBD ${converter || 'converter'}(${field.sourceFields.join(', ')})` };
}

function fieldInput(field, control, values) {
  if (control.target && field.cdm && field.cdm === control.target.cdm) return { input: control.target, value: targetDeviceValue(control), automaticTarget: true };
  let input = field.cdm && control.inputs.find((item) => item.cdm === field.cdm);
  if (!input) input = control.inputs.find((item) => item.key === field.name);
  return input && Object.hasOwn(values, input.key) ? { input, value: values[input.key], automaticTarget: false } : null;
}

function mapBinding(control, binding, values) {
  state.demo.now = new Date();
  const rows = [];
  const physical = {};

  for (const field of binding.fields) {
    let row;
    if (field.kind === 'FixedField') {
      row = { field, name: field.name, value: field.value, owner: 'fixed', note: 'Binding FixedField', unresolved: false };
    } else if (field.kind === 'PackedField') {
      const result = packedFieldValue(field, control, values, physical);
      row = result.ok
        ? { field, name: field.name, value: result.value, owner: 'derived', note: 'Binding PackedField', unresolved: false }
        : { field, name: field.name, value: result.value, owner: 'converter', note: 'PACKED FIELD SOURCE REQUIRED', unresolved: true };
    } else if (field.kind === 'DerivedField') {
      const result = derivedValue(field, control, values, physical);
      row = result.ok
        ? { field, name: field.name, value: result.value, owner: 'derived', note: field.converter, unresolved: false }
        : { field, name: field.name, value: result.value, owner: 'converter', note: `CONVERTER UNSUPPORTED · ${field.converter || 'converter'}`, unresolved: true };
    } else {
      const found = fieldInput(field, control, values);
      if (found) row = { field, name: field.name, value: declaredMapValue(field, found.value), owner: found.automaticTarget ? 'target' : 'input', note: found.automaticTarget ? '자동 Target' : `HMI Parameter · ${found.input.key}`, unresolved: false };
      else if (!field.cdm && (field.name.toLowerCase().includes('reserve') || field.name.toLowerCase().includes('reserved'))) row = { field, name: field.name, value: 0, owner: 'fallback', note: 'Web Demo fallback · zero-fill', unresolved: false };
      else row = { field, name: field.name, value: 'TBD', owner: 'xml', note: 'XML DEFINITION REQUIRED · 값 공급 규칙 없음', unresolved: true };
    }
    physical[field.name] = row.value;
    if (field.cdm) physical[field.cdm] = row.value;
    rows.push(row);
  }

  return {
    rows,
    unresolved: rows.filter((row) => row.unresolved),
    fallbacks: rows.filter((row) => row.owner === 'fallback'),
  };
}

function ownerLabel(owner) {
  return {
    input: 'HMI INPUT',
    target: 'AUTO TARGET',
    fixed: 'BINDING FIXED',
    derived: 'DERIVED',
    fallback: 'DEMO FALLBACK',
    converter: 'CONVERTER UNSUPPORTED',
    xml: 'XML DEFINITION REQUIRED',
    unresolved: 'VALUE NOT RESOLVED',
  }[owner] || owner;
}

function renderBindingSummary(control, binding) {
  clear(dom.bindingSummary);
  dom.bindingSummary.append(
    el('h3', '', `${binding.transport} · ${binding.sourceFile}`),
    makeDl([
      ...channelRows(binding.channel),
      ['Semantic ID', binding.semanticId],
      ['Reply', binding.replies.map((item) => item.semanticId).join(', ') || 'No Reply'],
    ]),
  );
}

function displayValue(rawValue) {
  return typeof rawValue === 'object' ? JSON.stringify(rawValue) : String(rawValue);
}

function renderPayload(binding, mapped, requestId) {
  clear(dom.payloadTableBody);
  for (const row of mapped.rows) {
    const tr = el('tr');
    const field = el('td');
    field.append(el('code', '', row.name), el('small', '', row.note));
    const valueCell = el('td');
    valueCell.append(el('code', '', displayValue(row.value)));
    const owner = el('td');
    owner.append(el('span', `owner-badge ${row.owner}`, ownerLabel(row.owner)));
    tr.append(field, valueCell, owner);
    dom.payloadTableBody.append(tr);
  }
  state.demo.latestPayload = {
    requestId,
    protocol: binding.channel?.protocol,
    infoCode: binding.channel?.infoCode || null,
    messageType: binding.channel?.messageType || binding.channel?.typeName,
    topicName: binding.channel?.topicName || null,
    payload: Object.fromEntries(mapped.rows.map((row) => [row.name, row.value])),
    mock: true,
  };
  dom.copyPayloadButton.disabled = false;
}

function setResult(type, title, detail) {
  dom.resultBanner.className = `result-banner ${type}`;
  dom.resultBanner.replaceChildren(el('strong', '', title), el('small', '', detail));
}

function publishExecutionState(executionReport, detail = '') {
  const context = state.demo.currentDds;
  if (!context) return;
  addBus(
    OM_DDS_TYPES.controlExecutionReply,
    'OPERATION MANAGEMENT CSC → HMI CSC',
    { targetId: context.target.targetId, executionReport },
    executionReport === OPERATION_STATES.failed ? 'error dds' : 'dds',
    { transport: 'DDS', relatedSampleIdentity: context.sampleIdentity, meaning: detail },
  );
}

function rejectRequest(requestId, code, message) {
  publishExecutionState(OPERATION_STATES.failed, `${code}: ${message}`);
  setState(dom.omState, 'REJECTED');
  setState(dom.hmiState, 'REJECTED');
  setResult('blocked', `REJECTED · ${code}`, message);
  appendTerminal(`${code}: ${message}`, 'error');
  state.demo.processing = false;
}

async function publishControl() {
  const draft = state.demo;
  if (!draft.specLoaded) {
    appendTerminal('먼저 request -rcc를 실행하세요.', 'error');
    return;
  }
  if (!draft.controlId) {
    appendTerminal('control -i로 Control을 선택하세요.', 'error');
    return;
  }
  if (draft.processing) return;

  draft.processing = true;
  const requestId = makeRequestId('TRACE');
  const control = state.bundle.controls.find((item) => item.publicId === draft.controlId);
  const target = control ? actionTarget(control) : { targetId: 'UNKNOWN', targetName: 'UNKNOWN' };
  const sampleIdentity = makeDdsIdentity();
  state.demo.currentDds = { target, sampleIdentity, requestId };
  resetPipeline();
  const rawValues = Object.fromEntries(draft.args);
  addBus(
    OM_DDS_TYPES.controlExecutionRequest,
    'HMI CSC → OPERATION MANAGEMENT CSC',
    { targetId: target.targetId, controlSpec: control ? controlSpecPayload(control, rawValues, false) : null },
    'dds',
    { transport: 'DDS', sampleIdentity, traceId: requestId, targetRole: 'DDS Target Instance' },
  );

  markStep('lookup', 'active');
  setState(dom.omState, 'LOOKUP');
  await delay(60);
  if (!control) {
    markStep('lookup', 'error');
    rejectRequest(requestId, 'UNKNOWN_CONTROL', 'Control을 찾을 수 없습니다.');
    return;
  }
  markStep('lookup', 'done');

  markStep('validate', 'active');
  const validation = validateControl(control);
  if (!validation.ok) {
    markStep('validate', 'error');
    rejectRequest(requestId, validation.code, validation.message);
    return;
  }
  markStep('validate', 'done');

  markStep('binding', 'active');
  const binding = resolveBinding(control);
  if (!binding) {
    markStep('binding', 'error');
    rejectRequest(requestId, 'NO_BINDING', '선택 가능한 Binding이 없습니다.');
    return;
  }
  markStep('binding', 'done');
  renderBindingSummary(control, binding);

  markStep('mapping', 'active');
  setState(dom.omState, 'MAPPING');
  await delay(60);
  const mapped = mapBinding(control, binding, validation.values);
  renderPayload(binding, mapped, requestId);
  markStep('mapping', 'done');

  if (mapped.unresolved.length) {
    markStep('publish', 'error');
    setState(dom.omState, 'PAYLOAD PREVIEW');
    setState(dom.hmiState, 'TBD BLOCKED');
    setResult('blocked', 'PAYLOAD_PREVIEW_WITH_TBD', `${mapped.unresolved.length}개 Field가 미해결되어 Mock 송신을 보류했습니다.`);
    publishExecutionState(OPERATION_STATES.failed, `Logical Message unresolved: ${mapped.unresolved.map((row) => row.name).join(', ')}`);
    state.demo.processing = false;
    return;
  }

  markStep('publish', 'active');
  setState(dom.omState, 'MOCK PUBLISH');
  await delay(60);
  addBus(
    channelSummary(binding.channel),
    'OPERATION MANAGEMENT CSC → Device',
    Object.fromEntries(mapped.rows.map((row) => [row.name, row.value])),
    'device',
    { protocol: binding.channel.protocol, messageType: binding.channel.messageType || binding.channel.typeName, traceId: requestId, mock: true },
  );
  markStep('publish', 'done');
  state.demo.sequence += 1;

  if (!binding.replies.length) {
    setResult('success', 'MOCK_PUBLISHED_NO_REPLY', 'Reply가 없는 단방향 메시지입니다.');
    setState(dom.omState, 'MOCK PUBLISHED');
  } else {
    const reply = binding.replies[0];
    setResult('success', 'MOCK_REPLY_ACCEPTED', `${reply.semanticId} Mock Reply를 수락했습니다. 실제 장비 동작 완료를 의미하지 않습니다.`);
    setState(dom.omState, 'MOCK REPLY');
  }
  publishExecutionState(OPERATION_STATES.finished, 'OperationManagement Mock 처리 완료');
  setState(dom.hmiState, 'MOCK COMPLETE');
  state.demo.processing = false;
}

function printHelp() {
  [
    '-v',
    'request -rcc',
    'control -i <full-id>',
    'control -a <fieldName>,<value>',
    'control -cl',
    'control -r',
    'control -sh',
    '-h',
    'clear  (웹 터미널 화면 지우기)',
  ].forEach((line) => appendTerminal(line, 'info'));
}

function handleSelect(id) {
  if (!state.demo.specLoaded) {
    appendTerminal('먼저 request -rcc를 실행하세요.', 'error');
    return false;
  }
  const previousId = state.demo.controlId;
  const hadArguments = state.demo.args.size > 0;
  const preserveArguments = previousId === id;
  if (!selectControl(id, false, preserveArguments)) {
    const near = state.bundle.controls.filter((item) => item.publicId.toLowerCase().includes(id.toLowerCase())).map((item) => item.publicId);
    appendTerminal(`알 수 없는 Control ID${near.length ? `. 후보: ${near.join(', ')}` : `: ${id}`}`, 'error');
    return false;
  }
  if (previousId && previousId !== id && hadArguments) {
    appendTerminal('Control이 변경되어 이전 Draft 입력값을 초기화했습니다.', 'info');
  }
  return true;
}

function handleArgument(key, raw) {
  const control = state.demo.controlId ? state.bundle.controls.find((item) => item.publicId === state.demo.controlId) : null;
  if (!control) {
    appendTerminal('먼저 control -i <full-id>로 Control을 선택하세요.', 'error');
    return false;
  }
  if (control?.target && [control.target.key, control.target.cdm, 'targetId'].includes(key)) {
    appendTerminal(`${key}는 ControlExecutionRequest.targetId에서 자동 공급되므로 HMI가 입력하지 않습니다.`, 'error');
    return false;
  }
  const managed = new Set((control?.bindings || []).flatMap((binding) => (
    flattenFields(binding.fields)
      .filter((field) => ['FixedField', 'DerivedField'].includes(field.kind))
      .map((field) => field.name)
  )));
  if (managed.has(key)) {
    appendTerminal(`${key}는 Binding 관리 Field이므로 HMI가 입력할 수 없습니다.`, 'error');
    return false;
  }
  const input = control.inputs.find((item) => item.key === key);
  if (!input) {
    appendTerminal(`UNKNOWN_ARGUMENT: 알 수 없는 key ${key}. 허용: ${control.inputs.map((item) => item.key).join(', ') || '입력 없음'}`, 'error');
    return false;
  }
  const validation = parseArgument(input, raw);
  if (!validation.ok) {
    appendTerminal(`TYPE_OR_RANGE: ${validation.message}`, 'error');
    return false;
  }
  const before = state.demo.args.get(key);
  state.demo.args.set(key, raw);
  renderDraft();
  appendTerminal(`${key} = ${raw}${before !== undefined ? ' 갱신' : ''}`, 'success');
  return true;
}

function clearControlSettings() {
  state.demo.controlId = null;
  state.demo.args.clear();
  state.demo.currentDds = null;
  if (dom.presetSelection) dom.presetSelection.textContent = 'Control 검색 또는 선택';
  renderDraft();
  setState(dom.hmiState, state.demo.specLoaded ? 'SPEC READY' : 'SPEC NOT LOADED');
  appendTerminal('Cleared All Control Settings', 'success');
}

function showControlSettings() {
  appendTerminal(`control id: ${state.demo.controlId || ''}`, 'info');
  const control = state.demo.controlId && state.bundle ? state.bundle.controls.find((item) => item.publicId === state.demo.controlId) : null;
  if (control) {
    const target = actionTarget(control);
    appendTerminal(`targetId: ${target.targetId} (자동)`, 'info');
    if (control.target) appendTerminal(`${control.target.key}: ${targetDeviceValue(control)} (자동 매핑)`, 'info');
  }
  appendTerminal('control params:', 'info');
  const params = [...state.demo.args].map(([fieldName, fieldValue]) => ({ fieldName, fieldValue }));
  appendTerminal(JSON.stringify(params, null, 2), 'info');
}

function commandTokens(command) {
  const matches = String(command).match(/"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\S+/g) || [];
  return matches.map((token) => {
    if ((token.startsWith('"') && token.endsWith('"')) || (token.startsWith("'") && token.endsWith("'"))) return token.slice(1, -1);
    return token;
  });
}

function parseControlOptions(tokens) {
  const result = { add: null, clear: false, id: null, run: false, show: false, error: '' };
  for (let index = 1; index < tokens.length; index += 1) {
    const option = tokens[index].toLowerCase();
    if (['-a', '--add'].includes(option)) {
      if (index + 1 >= tokens.length) { result.error = `${tokens[index]} 뒤에 fieldName,value가 필요합니다.`; break; }
      result.add = tokens[++index];
    } else if (['-cl', '--clear', '-c'].includes(option)) result.clear = true;
    else if (['-i', '--id'].includes(option)) {
      if (index + 1 >= tokens.length) { result.error = `${tokens[index]} 뒤에 Control ID가 필요합니다.`; break; }
      result.id = tokens[++index];
    } else if (['-r', '--run'].includes(option)) result.run = true;
    else if (['-sh', '--show', '-p'].includes(option)) result.show = true;
    else { result.error = `알 수 없는 control 옵션: ${tokens[index]}`; break; }
  }
  return result;
}

async function controlCommand(command) {
  const tokens = commandTokens(command);
  const parsed = parseControlOptions(tokens);
  if (parsed.error) {
    appendTerminal(parsed.error, 'error');
    return;
  }
  if (parsed.add !== null) {
    const comma = parsed.add.indexOf(',');
    if (comma <= 0 || comma === parsed.add.length - 1) {
      appendTerminal('please write {id,value}', 'error');
      return;
    }
    const key = parsed.add.slice(0, comma).trim();
    const raw = parsed.add.slice(comma + 1).trim();
    if (!handleArgument(key, raw)) return;
  }
  if (parsed.clear) clearControlSettings();
  if (parsed.id !== null && !handleSelect(parsed.id)) return;
  if (parsed.run) await publishControl();
  if (parsed.show) showControlSettings();
  if (tokens.length === 1) appendTerminal('control 옵션이 없습니다. -h 또는 help를 확인하세요.', 'error');
}

async function executeCommand(raw) {
  const command = String(raw).replace(/[–—−]/g, '-').replace(/\s+/g, ' ').trim();
  if (!command) return;
  state.demo.history.push(command);
  state.demo.historyIndex = state.demo.history.length;
  appendTerminal(command, 'input', '>');
  if (/^(help|-h|--help)$/i.test(command)) printHelp();
  else if (command === 'clear') clear(dom.terminalLog);
  else if (/^(-v|--version)$/i.test(command)) appendTerminal('v0.1', 'info');
  else if (/^request\s+(-rcc|--request-control-specs)$/i.test(command)) await requestSpecs();
  else if (/^control(?:\s|$)/i.test(command)) await controlCommand(command);
  else appendTerminal('알 수 없는 명령입니다. -h 또는 help를 입력하세요.', 'error');
}

const tabbar = document.querySelector('.tabbar');
if (tabbar) {
  for (const name of ['hmi', 'model', 'cdm']) {
    const button = document.querySelector(`[data-tab=${name}]`);
    if (button) tabbar.append(button);
  }
}
setupHmiContractUi();

for (const button of tabButtons) {
  button.addEventListener('click', () => activateTab(button.dataset.tab));
  button.addEventListener('keydown', handleTabKey);
}

if (dom.folderInput) dom.folderInput.addEventListener('change', () => readFolder(dom.folderInput.files));
if (dom.controlSearch) dom.controlSearch.addEventListener('input', () => renderFeatureList(dom.controlSearch.value));
if (dom.deviceSearch) dom.deviceSearch.addEventListener('input', () => renderDeviceList(dom.deviceSearch.value));
if (dom.devicePickerButton) dom.devicePickerButton.addEventListener('click', (event) => {
  event.stopPropagation();
  const nextOpen = dom.devicePopover.hidden;
  dom.devicePopover.hidden = !nextOpen;
  dom.devicePickerButton.setAttribute('aria-expanded', String(nextOpen));
  if (nextOpen) {
    renderDeviceList(dom.deviceSearch ? dom.deviceSearch.value : '');
    dom.deviceSearch?.focus();
  }
});
if (dom.schemaRail) dom.schemaRail.addEventListener('toggle', () => {
  const label = dom.schemaRail.querySelector('summary small');
  if (label) label.textContent = dom.schemaRail.open ? '접기' : '펼치기';
});
if (dom.hmiCatalogSearch) dom.hmiCatalogSearch.addEventListener('input', () => renderCatalog(dom.hmiCatalogSearch.value));
if (dom.presetSearch) dom.presetSearch.addEventListener('input', () => renderPresets(dom.presetSearch.value));
if (dom.hmiDeviceSelect) dom.hmiDeviceSelect.addEventListener('change', () => {
  state.hmiDeviceKey = dom.hmiDeviceSelect.value;
  state.hmiActionId = '';
  state.hmiBindingKey = '';
  state.hmiSourceKey = 'semantic';
  renderHmiWorkspace();
});
if (dom.hmiFunctionSearch) dom.hmiFunctionSearch.addEventListener('input', renderHmiWorkspace);
if (dom.sendDraftButton) dom.sendDraftButton.addEventListener('click', publishControl);
if (dom.resetDemoButton) dom.resetDemoButton.addEventListener('click', resetDemo);

if (dom.terminalForm) dom.terminalForm.addEventListener('submit', async (event) => {
  event.preventDefault();
  const command = dom.terminalInput.value;
  dom.terminalInput.value = '';
  await executeCommand(command);
});

if (dom.terminalInput) dom.terminalInput.addEventListener('keydown', (event) => {
  if (!['ArrowUp', 'ArrowDown'].includes(event.key)) return;
  event.preventDefault();
  if (event.key === 'ArrowUp' && state.demo.historyIndex > 0) state.demo.historyIndex -= 1;
  if (event.key === 'ArrowDown' && state.demo.historyIndex < state.demo.history.length) state.demo.historyIndex += 1;
  dom.terminalInput.value = state.demo.history[state.demo.historyIndex] || '';
});

if (dom.copyPayloadButton) dom.copyPayloadButton.addEventListener('click', async () => {
  if (!state.demo.latestPayload) return;
  try {
    await navigator.clipboard.writeText(JSON.stringify(state.demo.latestPayload, null, 2));
    appendTerminal('논리 메시지를 클립보드에 복사했습니다.', 'success');
  } catch {
    appendTerminal('브라우저 권한 때문에 복사하지 못했습니다.', 'warning');
  }
});

document.addEventListener('click', (event) => {
  const copy = event.target.closest('[data-copy-text]');
  if (copy) {
    navigator.clipboard.writeText(copy.dataset.copyText || '').catch(() => {});
    const original = copy.textContent;
    copy.textContent = '복사됨';
    window.setTimeout(() => { copy.textContent = original; }, 900);
    return;
  }

  const hmiKind = event.target.closest('[data-hmi-kind]');
  if (hmiKind) {
    state.hmiKind = hmiKind.dataset.hmiKind;
    state.hmiActionId = '';
    state.hmiBindingKey = '';
    state.hmiSourceKey = 'semantic';
    renderHmiWorkspace();
    return;
  }

  const hmiAction = event.target.closest('[data-hmi-action]');
  if (hmiAction) {
    state.hmiActionId = hmiAction.dataset.hmiAction;
    state.hmiBindingKey = '';
    state.hmiSourceKey = 'semantic';
    renderHmiWorkspace();
    return;
  }

  const hmiBinding = event.target.closest('[data-hmi-binding]');
  if (hmiBinding) {
    state.hmiBindingKey = hmiBinding.dataset.hmiBinding;
    if (state.hmiSourceKey !== 'semantic') state.hmiSourceKey = state.hmiBindingKey;
    const action = state.bundle?.actions.find((item) => item.publicId === state.hmiActionId) || null;
    renderHmiResponse(action);
    return;
  }

  const hmiSource = event.target.closest('[data-hmi-source]');
  if (hmiSource) {
    state.hmiSourceKey = hmiSource.dataset.hmiSource;
    const action = state.bundle?.actions.find((item) => item.publicId === state.hmiActionId) || null;
    renderHmiResponse(action);
    return;
  }

  const openDemo = event.target.closest('[data-hmi-open-demo]');
  if (openDemo) {
    state.demo.specLoaded = true;
    selectControl(openDemo.dataset.hmiOpenDemo, true);
    if (dom.hmiDemoDrawer) dom.hmiDemoDrawer.open = true;
    dom.hmiDemoDrawer?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    return;
  }

  const device = event.target.closest('[data-device-key]');
  if (device && state.bundle) {
    showReference(device.dataset.deviceKey);
    return;
  }
  if (dom.devicePopover && !dom.devicePopover.hidden && !event.target.closest('#devicePicker')) {
    dom.devicePopover.hidden = true;
    dom.devicePickerButton?.setAttribute('aria-expanded', 'false');
  }

  const cdm = event.target.closest('[data-cdm-view]');
  if (cdm) showCdmView(cdm.dataset.cdmView);

  const refNode = event.target.closest('[data-ref-key]');
  if (refNode && state.bundle) showReference(refNode.dataset.refKey, {
    shiftKey: event.shiftKey,
    ctrlKey: event.ctrlKey,
    metaKey: event.metaKey,
  });

  const file = event.target.closest('[data-file]');
  if (file && state.bundle) showFile(file.dataset.file, {
    sourceNode: file,
    shiftKey: event.shiftKey,
    ctrlKey: event.ctrlKey,
    metaKey: event.metaKey,
  });

  const feature = event.target.closest('[data-control-id]');
  if (feature && state.bundle) showFeature(feature.dataset.controlId, {
    shiftKey: event.shiftKey,
    ctrlKey: event.ctrlKey,
    metaKey: event.metaKey,
  });

  const preset = event.target.closest('[data-preset-control]');
  if (preset) {
    if (!state.demo.specLoaded) state.demo.specLoaded = true;
    selectControl(preset.dataset.presetControl, true);
    if (dom.presetPicker) dom.presetPicker.open = false;
    activateTab('hmi');
  }

  const select = event.target.closest('[data-select-control]');
  if (select) {
    if (!state.demo.specLoaded) state.demo.specLoaded = true;
    selectControl(select.dataset.selectControl, false);
    activateTab('hmi');
    dom.terminalInput.focus();
  }
});

const initialTab = ['cdm', 'model', 'hmi'].includes(location.hash.slice(1)) ? location.hash.slice(1) : 'hmi';
activateTab(initialTab);
renderPrinciples();
resetDemo();

window.ProtocolExplorer = {
  loadEntries(entries, folder = '테스트 폴더') { ingest(entries, folder); },
  getBundle() { return state.bundle; },
  buildBundle,
};
