'use strict';

const state = {
  files: [],
  devices: [],
  selectedDevice: null,
  selectedAction: null,
  inspectIndex: new Map(),
  rootLabel: 'LOCAL',
};

const ids = [
  'designFolderInput','landing','sourceState','workspace','deviceSearch','deviceList','railSource',
  'deviceEyebrow','deviceTitle','deviceMeta','deviceInlineStats','functionSearch','functionMap',
  'traceEmpty','tracePanel','traceKind','traceCode','traceTitle','traceDescription','inspectSemantic',
  'specNode','semanticNode','bindingNodes','wireNodes','replySection','replyLane','fieldTrace','issueStrip',
  'inspector','inspectorTitle','inspectorEmpty','inspectorBody','closeInspector',
];
const dom = Object.fromEntries(ids.map((id) => [id, document.getElementById(id)]));

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
function basename(path) { return normalizePath(path).split('/').pop() || ''; }
function extension(path) {
  const name = basename(path);
  const dot = name.lastIndexOf('.');
  return dot >= 0 ? name.slice(dot + 1).toLowerCase() : '';
}
function direct(node, name = '') {
  if (!node) return [];
  return [...node.children].filter((child) => !name || child.localName === name);
}
function first(node, name) { return direct(node, name)[0] || null; }
function all(node, name) {
  if (!node) return [];
  return [...node.getElementsByTagNameNS('*', name)];
}
function attr(node, name) { return node ? node.getAttribute(name) || '' : ''; }
function value(node) { return node ? node.textContent.trim() : ''; }
function semanticRefId(text) {
  const raw = String(text || '').trim();
  const hash = raw.lastIndexOf('#');
  return hash >= 0 ? raw.slice(hash + 1) : raw;
}
function commentBefore(node) {
  let previous = node?.previousSibling || null;
  while (previous && previous.nodeType === Node.TEXT_NODE && !previous.nodeValue.trim()) previous = previous.previousSibling;
  return previous?.nodeType === Node.COMMENT_NODE ? previous.nodeValue.trim().replace(/\s+/g, ' ') : '';
}
function make(tag, className = '', text = '') {
  const node = document.createElement(tag);
  if (className) node.className = className;
  if (text !== '') node.textContent = String(text);
  return node;
}
function clear(node) { if (node) node.replaceChildren(); }
function attrs(node) { return node ? Object.fromEntries([...node.attributes].map((item) => [item.name, item.value])) : {}; }
function attrsSummary(node, excluded = []) {
  const values = [];
  for (const item of [...(node?.attributes || [])]) {
    if (!excluded.includes(item.name)) values.push(`${item.name}=${item.value}`);
  }
  return values.join(' · ');
}
function uniqueBy(items, keyFn) {
  const seen = new Set();
  return items.filter((item) => {
    const key = keyFn(item);
    if (seen.has(key)) return false;
    seen.add(key);
    return true;
  });
}

function setSource(status, text) {
  dom.sourceState.className = `source-state ${status || ''}`.trim();
  dom.sourceState.textContent = text;
}

async function readFolder(fileList) {
  const files = [...fileList];
  if (!files.length) return;
  const root = files[0].webkitRelativePath.split('/')[0] || '선택 폴더';
  setSource('loading', `${root} · XML 관계 분석 중`);
  const entries = [];
  for (const file of files) {
    const ext = extension(file.name);
    if (!['xml','xsd','md','idl'].includes(ext)) continue;
    entries.push({
      name: file.name,
      path: normalizePath(file.webkitRelativePath || file.name),
      ext,
      text: await file.text(),
    });
  }
  try {
    ingest(entries, root);
  } catch (error) {
    console.error(error);
    setSource('error', error.message || '폴더를 분석하지 못했습니다.');
  }
}

function parseFiles(entries) {
  return entries.map((entry) => {
    const file = { ...entry, doc: null, root: '' };
    if (['xml','xsd'].includes(entry.ext)) {
      file.doc = new DOMParser().parseFromString(entry.text, 'application/xml');
      const parserError = file.doc.getElementsByTagName('parsererror')[0];
      if (parserError) file.error = parserError.textContent.trim();
      else file.root = file.doc.documentElement.localName;
    }
    return file;
  });
}

function resolveFile(baseFile, reference) {
  if (!reference) return null;
  const expected = normalizePath(`${dirname(baseFile.path)}/${reference}`).toLowerCase();
  let found = state.files.find((file) => file.path.toLowerCase() === expected);
  if (found) return found;
  const base = basename(reference).toLowerCase();
  const candidates = state.files.filter((file) => file.name.toLowerCase() === base);
  if (candidates.length === 1) return candidates[0];
  found = candidates.find((file) => dirname(file.path) === dirname(baseFile.path));
  return found || candidates[0] || null;
}

function parseProfile(node) {
  const range = all(node, 'Range')[0];
  const unit = all(node, 'Unit')[0];
  const resolution = all(node, 'Resolution')[0];
  const values = [];
  for (const candidate of [...node.getElementsByTagName('*')]) {
    if (!['Value','Option','Enum','Map'].includes(candidate.localName)) continue;
    const summary = attrsSummary(candidate);
    const text = value(candidate);
    if (summary || text) values.push(summary || text);
  }
  return {
    kind: node.localName,
    cdm: attr(node, 'cdm'),
    required: attr(node, 'required') ? attr(node, 'required') !== 'false' : attr(node, 'minOccurs') !== '0',
    defaultValue: attr(node, 'default'),
    unit: value(unit),
    min: range ? attr(range, 'min') : '',
    max: range ? attr(range, 'max') : '',
    resolution: value(resolution),
    description: commentBefore(node),
    values: uniqueBy(values, (item) => item),
    node,
  };
}

function parseResultNode(node) {
  return {
    kind: node.localName,
    cdm: attr(node, 'cdm'),
    id: attr(node, 'id'),
    required: attr(node, 'required') !== 'false',
    description: commentBefore(node),
    profiles: direct(node).filter((child) => /Result$|Spec$|Profile$/.test(child.localName)).map(parseProfile),
    node,
  };
}

function parseSemanticActions(ref) {
  const file = ref.semanticFile;
  if (!file?.doc) return [];
  const actions = [];
  const controls = all(file.doc, 'ControlSpecs')[0];
  if (controls) {
    for (const node of direct(controls).filter((item) => ['ControlSpec','SetPointSpec'].includes(item.localName))) {
      const parameters = first(node, 'Parameters');
      const targetNode = first(node, 'Target');
      const replies = direct(node, 'Reply').map((reply) => ({
        cdm: attr(reply, 'cdm'),
        bindRef: semanticRefId(attr(reply, 'bindRef')),
        required: attr(reply, 'required') !== 'false',
        timeout: attr(reply, 'timeout'),
        description: commentBefore(reply),
        results: direct(reply).filter((child) => /Result$/.test(child.localName) || child.localName === 'Results').map(parseResultNode),
        node: reply,
      }));
      actions.push({
        kind: 'Control', ref, groupId: attr(controls, 'id'), semanticId: attr(node, 'id'),
        name: attr(node, 'name') || attr(node, 'id'), cdm: attr(node, 'cdm'), description: commentBefore(node),
        target: targetNode ? { cdm: attr(targetNode, 'cdm'), description: commentBefore(targetNode), node: targetNode } : null,
        profiles: parameters ? direct(parameters).map(parseProfile) : [], replies, node,
      });
    }
  }
  const monitors = all(file.doc, 'MonitorSpecs')[0];
  if (monitors) {
    for (const node of direct(monitors, 'GroupSpec')) {
      actions.push({
        kind: 'Monitor', ref, groupId: attr(monitors, 'id'), semanticId: attr(node, 'id'),
        name: attr(node, 'name') || attr(node, 'id'), cdm: attr(node, 'cdm'), description: commentBefore(node),
        target: null, profiles: direct(node).filter((child) => /Spec$|Profile$/.test(child.localName)).map(parseProfile), replies: [], node,
      });
    }
  }
  const products = all(file.doc, 'SensorProductSpecs')[0];
  if (products) {
    for (const node of direct(products).filter((item) => ['ProductStreamSpec','ProductFileSpec','ProductFrameSpec'].includes(item.localName))) {
      actions.push({
        kind: 'Product', ref, groupId: attr(products, 'id'), semanticId: attr(node, 'id'),
        name: attr(node, 'name') || attr(node, 'id'), cdm: attr(node, 'cdm'), description: commentBefore(node),
        target: null, profiles: direct(node).filter((child) => /Spec$|Profile$/.test(child.localName)).map(parseProfile), replies: [], node,
      });
    }
  }
  return actions.map((action) => ({ ...action, publicId: `${ref.id}.${action.groupId}.${action.semanticId}` }));
}

const FIELD_NAMES = new Set(['Field','FixedField','DerivedField','ArrayField','PackedField']);

function parseMaps(node) {
  const maps = [];
  for (const holder of direct(node).filter((child) => ['ValueMap','SourceValueMap'].includes(child.localName))) {
    for (const map of direct(holder, 'Map')) maps.push({ kind: holder.localName, text: attrsSummary(map) || value(map), node: map });
  }
  return maps;
}

function parseField(node, depth = 0) {
  const field = {
    kind: node.localName,
    name: attr(node, 'name'), cdm: attr(node, 'cdm'), dataType: attr(node, 'dataType'),
    value: attr(node, 'value'), converter: attr(node, 'converter'), sourceField: attr(node, 'sourceField'),
    description: commentBefore(node), depth, maps: parseMaps(node), bitMembers: [], children: [], node,
  };
  if (node.localName === 'PackedField') {
    field.bitMembers = direct(node, 'BitMember').map((member) => ({
      name: attr(member, 'name'), cdm: attr(member, 'cdm'), offset: attr(member, 'offset'), width: attr(member, 'width') || '1',
      value: attr(member, 'value'), description: commentBefore(member), maps: parseMaps(member), node: member,
    }));
  }
  const element = first(node, 'Element');
  if (element) field.children = direct(element).filter((child) => FIELD_NAMES.has(child.localName)).map((child) => parseField(child, depth + 1));
  return field;
}

function channelInfo(node) {
  const channel = direct(node).find((child) => child.localName.endsWith('Channel'));
  if (!channel) return { protocol: 'UNKNOWN', identity: 'Channel 없음', attrs: {}, node: null };
  const protocol = channel.localName.replace('Channel','') || 'UNKNOWN';
  const channelAttrs = attrs(channel);
  let identity = protocol;
  if (protocol === 'DDS') identity = `${channelAttrs.topicName || 'Topic ?'} / ${channelAttrs.typeName || 'Type ?'}`;
  else if (protocol === 'RTP') identity = 'RTP Stream';
  else identity = `${channelAttrs.infoCode || channelAttrs.topicName || 'Code ?'} / ${channelAttrs.messageType || channelAttrs.typeName || 'Type ?'}`;
  return { protocol, identity, attrs: channelAttrs, node: channel };
}

function parseBindingMessage(node) {
  return {
    channel: channelInfo(node),
    fields: direct(node).filter((child) => FIELD_NAMES.has(child.localName)).map((field) => parseField(field)),
    node,
  };
}
function bindingGroupName(kind) { return kind === 'Control' ? 'Controls' : kind === 'Monitor' ? 'Monitors' : 'SensorProducts'; }
function bindingNodeNames(kind) {
  if (kind === 'Control') return new Set(['ControlBinding','ControlBindingDDS']);
  if (kind === 'Monitor') return new Set(['MonitorBinding','MonitorBindingDDS']);
  return new Set(['ProductBinding']);
}

function parseBindingVariants(action) {
  const variants = [];
  for (const file of action.ref.bindingFiles) {
    if (!file?.doc) continue;
    const groups = all(file.doc, bindingGroupName(action.kind));
    const group = groups.find((item) => semanticRefId(attr(item, 'semantic_id')) === action.groupId) || groups[0];
    if (!group) continue;
    const names = bindingNodeNames(action.kind);
    for (const node of direct(group).filter((item) => names.has(item.localName) && semanticRefId(attr(item, 'semantic_id')) === action.semanticId)) {
      const message = parseBindingMessage(node);
      const replies = action.kind === 'Control' ? direct(node, 'Reply').map((reply) => ({
        semanticId: semanticRefId(attr(reply, 'semantic_id')),
        required: attr(reply, 'required') !== 'false', timeout: attr(reply, 'timeout'),
        ...parseBindingMessage(reply),
      })) : [];
      variants.push({ file, ...message, replies });
    }
  }
  return variants;
}

function findIssueFile(ref) {
  if (!ref.semanticFile) return null;
  const expected = ref.semanticFile.name.replace(/Semantic\.xml$/i, '_OpenIssues.md').toLowerCase();
  let file = state.files.find((item) => item.name.toLowerCase() === expected);
  if (file) return file;
  const folder = dirname(ref.semanticFile.path);
  file = state.files.find((item) => dirname(item.path) === folder && /_openissues\.md$/i.test(item.name));
  return file || null;
}

function extractIssues(file) {
  if (!file?.text) return [];
  const lines = file.text.replace(/\r/g, '').split('\n');
  const issues = [];
  let section = '';
  for (const raw of lines) {
    const line = raw.trim();
    if (/^#{1,6}\s+/.test(line)) {
      section = line.replace(/^#+\s+/, '');
      continue;
    }
    const match = line.match(/^(?:\d+[.)]|[-*])\s+(.+)/);
    if (!match) continue;
    const text = match[1].trim();
    if (/resolved|closed|완료|해결 완료|superseded?/i.test(`${section} ${text}`)) continue;
    issues.push({ section, text });
  }
  return uniqueBy(issues, (item) => `${item.section}|${item.text}`).slice(0, 80);
}

function parseVehicleFile(file) {
  const identification = all(file.doc, 'Identification')[0];
  const identity = {
    id: value(first(identification, 'ID')) || basename(file.path),
    name: value(first(identification, 'Name')) || basename(file.path),
  };
  const refNodes = [...file.doc.documentElement.getElementsByTagName('*')].filter((node) => node.localName.endsWith('SpecRef'));
  return refNodes.map((node) => {
    const semanticPath = value(first(node, 'SemanticPath'));
    const bindingPaths = direct(node, 'BindingPath').map(value);
    const ref = {
      id: attr(node, 'id'), name: attr(node, 'name') || attr(node, 'id'), cdm: attr(node, 'cdm'),
      kind: node.localName, vehicleFile: file, identity, semanticPath, bindingPaths,
      semanticFile: resolveFile(file, semanticPath),
      bindingFiles: bindingPaths.map((path) => resolveFile(file, path)).filter(Boolean),
      node,
    };
    ref.actions = parseSemanticActions(ref);
    ref.actions.forEach((action) => { action.bindings = parseBindingVariants(action); });
    ref.issueFile = findIssueFile(ref);
    ref.issues = extractIssues(ref.issueFile);
    return ref;
  });
}

function ingest(entries, root = 'LOCAL') {
  state.files = parseFiles(entries);
  const vehicleFiles = state.files.filter((file) => file.doc && file.root === 'VehicleSpec');
  state.devices = uniqueBy(vehicleFiles.flatMap(parseVehicleFile), (ref) => `${ref.vehicleFile.path}|${ref.id}|${ref.semanticFile?.path || ''}`);
  if (!state.devices.length) throw new Error('VehicleSpec에서 장비 SpecRef를 찾지 못했습니다.');
  state.selectedDevice = state.devices[0];
  state.selectedAction = null;
  state.rootLabel = root;
  dom.functionSearch.value = '';
  dom.landing.hidden = true;
  dom.workspace.hidden = false;
  dom.railSource.textContent = root;
  setSource('', `${state.devices.length}개 장비 참조 색인 완료`);
  closeInspector();
  renderAll();
}

function deviceKey(ref) { return `${ref.vehicleFile.path}|${ref.id}`; }
function actionCounts(ref) {
  return {
    Control: ref.actions.filter((action) => action.kind === 'Control').length,
    Monitor: ref.actions.filter((action) => action.kind === 'Monitor').length,
    Product: ref.actions.filter((action) => action.kind === 'Product').length,
    Issue: ref.issues.length,
  };
}
function protocols(action) {
  return [...new Set(action.bindings.map((variant) => variant.channel.protocol).filter(Boolean))];
}
function exactIssueMentions(action) {
  const tokens = [action.publicId, action.semanticId, action.cdm].filter(Boolean).map((token) => token.toLowerCase());
  return action.ref.issues.filter((issue) => tokens.some((token) => issue.text.toLowerCase().includes(token)));
}

function renderDeviceList() {
  clear(dom.deviceList);
  const q = dom.deviceSearch.value.trim().toLowerCase();
  for (const ref of state.devices.filter((item) => {
    const hay = `${item.name} ${item.id} ${item.identity.name} ${item.identity.id}`.toLowerCase();
    return !q || hay.includes(q);
  })) {
    const counts = actionCounts(ref);
    const button = make('button', `device-item${ref === state.selectedDevice ? ' is-active' : ''}`);
    button.type = 'button';
    button.dataset.deviceKey = deviceKey(ref);
    button.append(make('strong', '', ref.name || ref.id), make('small', '', `${ref.id} · C${counts.Control} M${counts.Monitor} P${counts.Product}`));
    dom.deviceList.append(button);
  }
}

function inlineStat(label, number, issue = false) {
  const node = make('span', `inline-stat${issue ? ' issue' : ''}`);
  node.append(make('span', '', label), make('b', '', number));
  return node;
}

function renderDeviceHeader() {
  const ref = state.selectedDevice;
  if (!ref) return;
  dom.deviceEyebrow.textContent = 'EQUIPMENT MAP';
  dom.deviceTitle.textContent = ref.name || ref.identity.name;
  dom.deviceMeta.textContent = `${ref.identity.name} · ${ref.kind} · ${ref.id} · ${ref.semanticFile?.path || ref.semanticPath || 'Semantic 미확인'}`;
  clear(dom.deviceInlineStats);
  const counts = actionCounts(ref);
  dom.deviceInlineStats.append(
    inlineStat('CONTROL', counts.Control),
    inlineStat('MONITOR', counts.Monitor),
    inlineStat('PRODUCT', counts.Product),
    inlineStat('OPEN ISSUE', counts.Issue, counts.Issue > 0),
  );
}

function actionVisible(action, q) {
  if (!q) return true;
  const hay = `${action.name} ${action.semanticId} ${action.cdm} ${action.publicId}`.toLowerCase();
  return hay.includes(q);
}

function renderFunctionMap() {
  clear(dom.functionMap);
  const ref = state.selectedDevice;
  if (!ref) return;
  const q = dom.functionSearch.value.trim().toLowerCase();
  for (const kind of ['Control','Monitor','Product']) {
    const actions = ref.actions.filter((action) => action.kind === kind && actionVisible(action, q));
    const group = make('section', `function-group ${kind.toLowerCase()}`);
    const header = make('header');
    header.append(make('b', '', kind.toUpperCase()), make('span', '', String(actions.length)));
    const items = make('div', 'function-items');
    if (!actions.length) items.append(make('p', 'function-empty', q ? '검색 결과 없음' : '정의 없음'));
    for (const action of actions) {
      const button = make('button', `function-item${action === state.selectedAction ? ' is-active' : ''}`);
      button.type = 'button';
      button.dataset.actionId = action.publicId;
      const text = make('span');
      const title = make('strong', '', action.name || action.semanticId);
      if (exactIssueMentions(action).length) title.append(make('span', 'issue-mark', '●'));
      text.append(title, make('small', '', action.cdm || action.semanticId));
      const wire = protocols(action).join('/') || '—';
      button.append(text, make('code', '', wire));
      items.append(button);
    }
    group.append(header, items);
    dom.functionMap.append(group);
  }
}

function nodeBox(label, title, code, meta, path) {
  const box = make('div', 'node-box');
  box.append(make('span', 'node-label', label), make('strong', '', title || '—'));
  if (code) box.append(make('code', '', code));
  if (meta) box.append(make('div', 'node-meta', meta));
  if (path) box.append(make('div', 'node-path', path));
  return box;
}

function renderSpecNode(action) {
  clear(dom.specNode);
  const ref = action.ref;
  dom.specNode.append(nodeBox(
    'SPEC REF',
    ref.name || ref.id,
    ref.id,
    `${ref.kind}${ref.cdm ? ` · ${ref.cdm}` : ''}`,
    ref.vehicleFile.path,
  ));
}

function profileSummary(profile) {
  return [
    profile.cdm,
    profile.min || profile.max ? `${profile.min || '…'}~${profile.max || '…'}` : '',
    profile.unit,
  ].filter(Boolean).join(' · ');
}

function renderSemanticNode(action) {
  clear(dom.semanticNode);
  const box = nodeBox(
    'SEMANTIC ACTION',
    action.name || action.semanticId,
    action.cdm || action.semanticId,
    action.target?.cdm ? `Target · ${action.target.cdm}` : `${action.profiles.length} profile`,
    action.ref.semanticFile?.path || action.ref.semanticPath,
  );
  const chips = make('div', 'semantic-profiles');
  action.profiles.slice(0, 8).forEach((profile) => chips.append(make('span', '', profileSummary(profile) || profile.kind)));
  if (action.profiles.length > 8) chips.append(make('span', '', `+${action.profiles.length - 8}`));
  if (action.profiles.length) box.append(chips);
  dom.semanticNode.append(box);
}

function renderChannelGrid(channel) {
  const grid = make('div', 'channel-grid');
  for (const [key, val] of Object.entries(channel.attrs)) {
    const row = make('div');
    row.append(make('span', '', key), make('code', '', val));
    grid.append(row);
  }
  return grid;
}

function renderBindingAndWire(action) {
  clear(dom.bindingNodes);
  clear(dom.wireNodes);
  if (!action.bindings.length) {
    const binding = make('div', 'binding-card');
    binding.append(make('span', 'node-label', 'BINDING'), make('strong', '', '연결된 Binding 없음'), make('div', 'node-meta', '선택 폴더에서 semantic_id가 일치하는 Binding을 찾지 못했습니다.'));
    const wire = make('div', 'wire-card');
    wire.append(make('span', 'node-label', 'WIRE'), make('strong', '', '—'));
    dom.bindingNodes.append(binding);
    dom.wireNodes.append(wire);
    return;
  }
  action.bindings.forEach((variant, index) => {
    const binding = make('article', 'binding-card interactive');
    binding.dataset.bindingIndex = String(index);
    binding.tabIndex = 0;
    binding.setAttribute('role', 'button');
    binding.append(
      make('span', 'node-label', `BINDING ${index + 1}`),
      make('strong', '', variant.file.name),
      make('code', '', semanticRefId(attr(variant.node, 'semantic_id')) || action.semanticId),
      make('div', 'field-count', `${flattenFields(variant.fields).length} field · ${variant.replies.length} reply`),
    );
    dom.bindingNodes.append(binding);

    const wire = make('article', 'wire-card');
    wire.append(make('span', 'node-label', 'CHANNEL'), make('strong', '', variant.channel.identity), make('span', 'protocol', variant.channel.protocol));
    if (Object.keys(variant.channel.attrs).length) wire.append(renderChannelGrid(variant.channel));
    dom.wireNodes.append(wire);
  });
}

function semanticReplySummary(reply) {
  return [reply.required ? 'required' : 'optional', reply.timeout ? `timeout=${reply.timeout}` : '', reply.description].filter(Boolean).join(' · ');
}

function renderReplyLane(action) {
  clear(dom.replyLane);
  const physical = action.bindings.flatMap((variant, variantIndex) => variant.replies.map((reply) => ({ variant, variantIndex, reply })));
  dom.replySection.hidden = action.kind !== 'Control' || (!action.replies.length && !physical.length);
  if (dom.replySection.hidden) return;

  const max = Math.max(action.replies.length, physical.length, 1);
  for (let i = 0; i < max; i += 1) {
    const semantic = action.replies[i] || null;
    const physicalItem = physical[i] || null;
    const row = make('div', 'reply-row');

    const wire = make('div', 'reply-box');
    wire.append(make('b', '', 'WIRE / REPLY'));
    if (physicalItem) {
      wire.append(make('code', '', physicalItem.reply.channel.identity), make('small', '', `${physicalItem.reply.channel.protocol} · ${flattenFields(physicalItem.reply.fields).length} field`));
    } else wire.append(make('small', '', 'Binding Reply 없음'));

    const binding = make('div', 'reply-box');
    binding.append(make('b', '', 'BINDING REPLY'));
    if (physicalItem) {
      binding.append(make('code', '', physicalItem.variant.file.name), make('small', '', physicalItem.reply.semanticId || 'semantic_id 미지정'));
    } else binding.append(make('small', '', '연결 없음'));

    const semanticBox = make('div', 'reply-box');
    semanticBox.append(make('b', '', 'SEMANTIC REPLY'));
    if (semantic) {
      semanticBox.append(make('code', '', semantic.bindRef || semantic.cdm || 'Reply'), make('small', '', semanticReplySummary(semantic) || 'Reply 정의'));
    } else semanticBox.append(make('small', '', 'Semantic Reply 없음'));

    row.append(wire, make('div', 'reply-arrow', '←'), binding, make('div', 'reply-arrow', '←'), semanticBox);
    dom.replyLane.append(row);
  }
}

function flattenFields(fields) {
  const out = [];
  function visit(field) {
    out.push(field);
    field.children.forEach(visit);
  }
  fields.forEach(visit);
  return out;
}

function semanticMatches(action, field) {
  const matches = [];
  if (field.cdm) {
    action.profiles.filter((profile) => profile.cdm && profile.cdm === field.cdm).forEach((profile) => matches.push({ type: 'profile', value: profile }));
    if (action.cdm && action.cdm === field.cdm) matches.push({ type: 'action', value: action });
    if (action.target?.cdm && action.target.cdm === field.cdm) matches.push({ type: 'target', value: action.target });
  }
  return matches;
}

function transformChips(field) {
  const items = [];
  if (field.sourceField) items.push({ className: '', text: `sourceField=${field.sourceField}` });
  if (field.converter) items.push({ className: '', text: `converter=${field.converter}` });
  field.maps.forEach((map) => items.push({ className: 'map', text: `${map.kind}: ${map.text}` }));
  field.bitMembers.forEach((member) => {
    items.push({ className: 'packed', text: `bit ${member.offset}/${member.width} ${member.name || member.cdm || ''}`.trim() });
    member.maps.forEach((map) => items.push({ className: 'map', text: `${member.name || 'bit'} ${map.kind}: ${map.text}` }));
  });
  if (field.kind === 'FixedField' && field.value) items.push({ className: '', text: `fixed=${field.value}` });
  return items;
}

function semanticCellText(match) {
  if (!match) return { title: '직접 일치 없음', code: '', detail: '동일 CDM 근거 없음' };
  if (match.type === 'profile') return { title: match.value.kind, code: match.value.cdm, detail: profileSummary(match.value) };
  if (match.type === 'target') return { title: 'Target', code: match.value.cdm, detail: match.value.description || '' };
  return { title: 'Action', code: match.value.cdm, detail: match.value.semanticId };
}

function fieldCell(label, title, code, detail, className = '') {
  const cell = make('div', `field-cell ${className}`.trim());
  cell.append(make('label', '', label), make('strong', className, title));
  if (code) cell.append(make('code', '', code));
  if (detail && detail !== code) cell.append(make('small', '', detail));
  return cell;
}

function renderFieldTrace(action) {
  clear(dom.fieldTrace);
  state.inspectIndex.clear();
  let inspectCounter = 0;
  if (!action.bindings.length) {
    dom.fieldTrace.append(make('div', 'field-empty', 'Binding이 없어 Field Trace를 구성할 수 없습니다.'));
    return;
  }

  action.bindings.forEach((variant, variantIndex) => {
    const group = make('section', 'field-group');
    const header = make('header');
    header.append(make('b', '', `${variant.channel.protocol} · ${variant.file.name}`), make('code', '', variant.channel.identity));
    group.append(header);
    const fields = flattenFields(variant.fields);
    if (!fields.length) group.append(make('div', 'field-empty', '직접 정의된 Binding Field가 없습니다.'));

    fields.forEach((field) => {
      const matches = semanticMatches(action, field);
      const primary = semanticCellText(matches[0]);
      const chips = transformChips(field);
      const key = `${action.publicId}:${variantIndex}:${inspectCounter++}`;
      state.inspectIndex.set(key, { action, variant, field, matches });

      const row = make('div', 'field-row');
      row.tabIndex = 0;
      row.setAttribute('role', 'button');
      row.dataset.inspectKey = key;
      row.append(fieldCell('SEMANTIC EVIDENCE', primary.title, primary.code, primary.detail, matches.length ? '' : 'no-evidence'));
      row.append(fieldCell('BINDING FIELD', field.name || '(unnamed)', field.cdm, `${field.kind}${field.description ? ` · ${field.description}` : ''}`));

      const transform = make('div', 'field-cell');
      transform.append(make('label', '', 'EXPLICIT TRANSFORM'));
      if (chips.length) {
        const chipWrap = make('div', 'transform-chips');
        chips.forEach((chip) => chipWrap.append(make('span', chip.className, chip.text)));
        transform.append(chipWrap);
      } else transform.append(make('small', 'no-evidence', '변환 속성 없음'));
      row.append(transform);

      row.append(fieldCell('WIRE REPRESENTATION', field.dataType || field.kind, field.value ? `value=${field.value}` : '', field.depth ? `nested depth ${field.depth}` : ''));
      group.append(row);
    });
    dom.fieldTrace.append(group);
  });
}

function renderIssues(action) {
  clear(dom.issueStrip);
  const mentions = exactIssueMentions(action);
  const allIssues = action.ref.issues;
  dom.issueStrip.className = `issue-strip${allIssues.length ? '' : ' clean'}`;
  const header = make('header');
  header.append(make('b', '', allIssues.length ? `OpenIssues ${allIssues.length}` : 'OpenIssues 없음'));
  header.append(make('small', '', action.ref.issueFile?.path || '연결된 _OpenIssues.md 없음'));
  dom.issueStrip.append(header);

  if (!allIssues.length) {
    dom.issueStrip.append(make('div', 'issue-note', '선택한 장비 폴더에서 미해결 항목 문서를 찾지 못했습니다.'));
    return;
  }
  if (mentions.length) {
    dom.issueStrip.append(make('div', 'issue-note', `아래 ${mentions.length}개 항목은 문서 본문에 현재 Semantic ID/CDM이 문자 그대로 등장합니다. 그 외 연계는 추정하지 않습니다.`));
    mentions.slice(0, 8).forEach((issue) => {
      const item = make('div', 'issue-item');
      item.append(make('strong', '', issue.section || 'ISSUE'), document.createTextNode(issue.text));
      dom.issueStrip.append(item);
    });
  } else {
    dom.issueStrip.append(make('div', 'issue-note', `장비 수준 미해결 항목 ${allIssues.length}개가 있으나 현재 기능과의 연계는 XML/문서에 명시된 근거가 없어 표시하지 않습니다.`));
  }
}

function renderTrace() {
  const action = state.selectedAction;
  dom.traceEmpty.hidden = Boolean(action);
  dom.tracePanel.hidden = !action;
  if (!action) return;

  dom.traceKind.textContent = action.kind;
  dom.traceKind.className = `kind-badge ${action.kind.toLowerCase()}`;
  dom.traceCode.textContent = action.publicId;
  dom.traceTitle.textContent = action.name || action.semanticId;
  dom.traceDescription.textContent = action.description || 'Semantic XML에 별도 설명 주석이 없습니다.';
  renderSpecNode(action);
  renderSemanticNode(action);
  renderBindingAndWire(action);
  renderReplyLane(action);
  renderFieldTrace(action);
  renderIssues(action);
}

function renderAll() {
  renderDeviceList();
  renderDeviceHeader();
  renderFunctionMap();
  renderTrace();
}

function selectDevice(key) {
  state.selectedDevice = state.devices.find((ref) => deviceKey(ref) === key) || null;
  state.selectedAction = null;
  dom.functionSearch.value = '';
  closeInspector();
  renderAll();
}

function selectAction(id) {
  state.selectedAction = state.selectedDevice?.actions.find((action) => action.publicId === id) || null;
  closeInspector();
  renderFunctionMap();
  renderTrace();
  if (window.innerWidth < 780 && state.selectedAction) dom.tracePanel.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

function inspectSection(title, content) {
  const section = make('section', 'inspect-section');
  section.append(make('h3', '', title), content);
  return section;
}
function inspectKv(rows) {
  const dl = make('dl', 'inspect-kv');
  rows.forEach(([label, raw]) => {
    dl.append(make('dt', '', label));
    const dd = make('dd');
    const text = raw === undefined || raw === null || raw === '' ? '—' : String(raw);
    if (/[.#:=/]/.test(text)) dd.append(make('code', '', text));
    else dd.textContent = text;
    dl.append(dd);
  });
  return dl;
}
function xmlBlock(path, xml) {
  const wrap = make('div');
  wrap.append(make('p', 'inspect-path', path || '경로 미확인'), make('pre', 'inspect-xml', xml || '원문 없음'));
  return wrap;
}
function openInspector(title, sections) {
  dom.inspectorTitle.textContent = title;
  dom.inspectorEmpty.hidden = true;
  dom.inspectorBody.hidden = false;
  clear(dom.inspectorBody);
  sections.forEach((section) => dom.inspectorBody.append(section));
  dom.inspector.classList.add('is-open');
}
function closeInspector() {
  dom.inspectorTitle.textContent = 'Source';
  dom.inspectorEmpty.hidden = false;
  dom.inspectorBody.hidden = true;
  clear(dom.inspectorBody);
  dom.inspector.classList.remove('is-open');
}

function openSemanticInspector() {
  const action = state.selectedAction;
  if (!action) return;
  const profileList = make('div', 'inspect-chip-list');
  if (action.profiles.length) {
    action.profiles.forEach((profile) => profileList.append(make('div', 'inspect-chip', `${profile.kind} · ${profile.cdm || 'CDM 없음'}${profileSummary(profile) ? ` · ${profileSummary(profile)}` : ''}`)));
  } else profileList.append(make('div', 'inspect-chip', '직접 정의된 Profile 없음'));
  openInspector(action.semanticId, [
    inspectSection('SEMANTIC', inspectKv([
      ['kind', action.kind], ['id', action.semanticId], ['cdm', action.cdm], ['target', action.target?.cdm], ['reply', action.replies.map((reply) => reply.bindRef || reply.cdm).join(', ')],
    ])),
    inspectSection('PROFILES', profileList),
    inspectSection('SOURCE XML', xmlBlock(action.ref.semanticFile?.path, action.node?.outerHTML)),
  ]);
}

function openBindingInspector(index) {
  const action = state.selectedAction;
  const variant = action?.bindings[index];
  if (!variant) return;
  openInspector(`${variant.channel.protocol} Binding`, [
    inspectSection('BINDING', inspectKv([
      ['file', variant.file.name], ['semantic_id', semanticRefId(attr(variant.node, 'semantic_id'))], ['fields', flattenFields(variant.fields).length], ['replies', variant.replies.length],
    ])),
    inspectSection('CHANNEL', inspectKv(Object.entries(variant.channel.attrs))),
    inspectSection('SOURCE XML', xmlBlock(variant.file.path, variant.node?.outerHTML)),
  ]);
}

function openFieldInspector(key) {
  const item = state.inspectIndex.get(key);
  if (!item) return;
  const { action, variant, field, matches } = item;
  const mapList = make('div', 'inspect-chip-list');
  const explicit = transformChips(field);
  if (explicit.length) explicit.forEach((entry) => mapList.append(make('div', 'inspect-chip', entry.text)));
  else mapList.append(make('div', 'inspect-chip', '명시된 변환 속성 없음'));

  const semanticList = make('div', 'inspect-chip-list');
  if (matches.length) {
    matches.forEach((match) => {
      const summary = semanticCellText(match);
      semanticList.append(make('div', 'inspect-chip', `${summary.title} · ${summary.code || 'CDM 없음'}${summary.detail ? ` · ${summary.detail}` : ''}`));
    });
  } else semanticList.append(make('div', 'inspect-chip', '동일 CDM으로 직접 일치하는 Semantic 요소 없음'));

  const sections = [
    inspectSection('BINDING FIELD', inspectKv([
      ['kind', field.kind], ['name', field.name], ['cdm', field.cdm], ['dataType', field.dataType], ['value', field.value], ['sourceField', field.sourceField], ['converter', field.converter],
    ])),
    inspectSection('EXPLICIT EVIDENCE', mapList),
    inspectSection('SAME CDM SEMANTIC', semanticList),
  ];
  if (field.bitMembers.length) {
    const bits = make('div', 'inspect-chip-list');
    field.bitMembers.forEach((member) => bits.append(make('div', 'inspect-chip', `offset=${member.offset} · width=${member.width} · ${member.name || 'unnamed'}${member.cdm ? ` · ${member.cdm}` : ''}`)));
    sections.push(inspectSection('BIT MEMBERS', bits));
  }
  sections.push(inspectSection('BINDING SOURCE XML', xmlBlock(variant.file.path, field.node?.outerHTML)));
  if (matches[0]?.value?.node) sections.push(inspectSection('SEMANTIC SOURCE XML', xmlBlock(action.ref.semanticFile?.path, matches[0].value.node.outerHTML)));
  openInspector(field.name || field.cdm || 'Binding Field', sections);
}

function activateKeyboard(event, callback) {
  if (event.key === 'Enter' || event.key === ' ') {
    event.preventDefault();
    callback();
  }
}

dom.designFolderInput.addEventListener('change', () => readFolder(dom.designFolderInput.files));
dom.deviceSearch.addEventListener('input', renderDeviceList);
dom.functionSearch.addEventListener('input', renderFunctionMap);
dom.deviceList.addEventListener('click', (event) => {
  const button = event.target.closest('[data-device-key]');
  if (button) selectDevice(button.dataset.deviceKey);
});
dom.functionMap.addEventListener('click', (event) => {
  const button = event.target.closest('[data-action-id]');
  if (button) selectAction(button.dataset.actionId);
});
dom.inspectSemantic.addEventListener('click', openSemanticInspector);
dom.semanticNode.addEventListener('click', openSemanticInspector);
dom.semanticNode.addEventListener('keydown', (event) => activateKeyboard(event, openSemanticInspector));
dom.bindingNodes.addEventListener('click', (event) => {
  const item = event.target.closest('[data-binding-index]');
  if (item) openBindingInspector(Number(item.dataset.bindingIndex));
});
dom.bindingNodes.addEventListener('keydown', (event) => {
  const item = event.target.closest('[data-binding-index]');
  if (item) activateKeyboard(event, () => openBindingInspector(Number(item.dataset.bindingIndex)));
});
dom.fieldTrace.addEventListener('click', (event) => {
  const row = event.target.closest('[data-inspect-key]');
  if (row) openFieldInspector(row.dataset.inspectKey);
});
dom.fieldTrace.addEventListener('keydown', (event) => {
  const row = event.target.closest('[data-inspect-key]');
  if (row) activateKeyboard(event, () => openFieldInspector(row.dataset.inspectKey));
});
dom.closeInspector.addEventListener('click', closeInspector);

window.XmlDesignTrace = {
  ingest,
  getState: () => state,
};
