'use strict';

const state = {
  files: [],
  devices: [],
  selectedDevice: null,
  selectedAction: null,
  kindFilter: 'All',
};

const dom = Object.fromEntries([
  'designFolderInput','designSourceState','designStats','designEmpty','designWorkspace',
  'designDeviceSearch','designDeviceList','designDeviceHero','designReferencePath',
  'designFunctionFilters','designFunctionSearch','designFunctionList','designNoFunction','designFunctionDetail',
  'designFunctionHero','flowFeature','flowSemantic','flowBinding','flowWire','flowReply',
  'designSemanticSummary','designSemanticTable','designBindingVariants','designReplyDetail','designIssueDetail',
  'designRawSemantic','designRawBinding',
].map((id) => [id, document.getElementById(id)]));

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

function basename(path) {
  return normalizePath(path).split('/').pop() || '';
}

function extension(path) {
  const name = basename(path);
  const dot = name.lastIndexOf('.');
  return dot >= 0 ? name.slice(dot + 1).toLowerCase() : '';
}

function direct(node, name = '') {
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

function attr(node, name) {
  return node ? node.getAttribute(name) || '' : '';
}

function value(node) {
  return node ? node.textContent.trim() : '';
}

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

function clear(node) {
  if (node) node.replaceChildren();
}

function attrs(node) {
  return node ? Object.fromEntries([...node.attributes].map((item) => [item.name, item.value])) : {};
}

function attrsSummary(node, excluded = []) {
  const values = [];
  for (const item of [...(node?.attributes || [])]) {
    if (excluded.includes(item.name)) continue;
    values.push(`${item.name}=${item.value}`);
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

function setSource(status, title, detail) {
  dom.designSourceState.className = `design-source ${status}`;
  dom.designSourceState.replaceChildren(
    make('b', '', status === 'ok' ? 'READY' : status === 'error' ? 'ERROR' : 'LOADING'),
    make('strong', '', title),
    make('small', '', detail),
  );
}

async function readFolder(fileList) {
  const files = [...fileList];
  if (!files.length) return;
  const root = files[0].webkitRelativePath.split('/')[0] || '선택 폴더';
  setSource('loading', root, 'XML · Markdown 관계를 분석하고 있습니다.');
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
    setSource('error', root, error.message);
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
        target: targetNode ? { cdm: attr(targetNode, 'cdm'), description: commentBefore(targetNode) } : null,
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
    for (const map of direct(holder, 'Map')) {
      maps.push({ kind: holder.localName, text: attrsSummary(map) || value(map) });
    }
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
  if (!channel) return { protocol: 'UNKNOWN', identity: 'Channel 없음', attrs: {} };
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

function bindingGroupName(kind) {
  return kind === 'Control' ? 'Controls' : kind === 'Monitor' ? 'Monitors' : 'SensorProducts';
}

function bindingNodeNames(kind) {
  if (kind === 'Control') return new Set(['ControlBinding','ControlBindingDDS']);
  if (kind === 'Monitor') return new Set(['MonitorBinding','MonitorBindingDDS']);
  return new Set(['ProductBinding']);
}

function parseBindingVariants(action) {
  const variants = [];
  for (const file of action.ref.bindingFiles) {
    if (!file?.doc) continue;
    const group = all(file.doc, bindingGroupName(action.kind)).find((item) => semanticRefId(attr(item, 'semantic_id')) === action.groupId) || all(file.doc, bindingGroupName(action.kind))[0];
    if (!group) continue;
    const names = bindingNodeNames(action.kind);
    for (const node of direct(group).filter((item) => names.has(item.localName) && semanticRefId(attr(item, 'semantic_id')) === action.semanticId)) {
      const message = parseBindingMessage(node);
      const replies = action.kind === 'Control' ? direct(node, 'Reply').map((reply) => ({
        semanticId: semanticRefId(attr(reply, 'semantic_id')), required: attr(reply, 'required') !== 'false', timeout: attr(reply, 'timeout'),
        ...parseBindingMessage(reply),
      })) : [];
      variants.push({ file, ...message, replies });
    }
  }
  return variants;
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
    };
    ref.actions = parseSemanticActions(ref);
    ref.actions.forEach((action) => { action.bindings = parseBindingVariants(action); });
    ref.issueFile = findIssueFile(ref);
    ref.issues = extractIssues(ref.issueFile);
    return ref;
  });
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
  const candidates = [];
  let section = '';
  for (const raw of lines) {
    const line = raw.trim();
    if (/^#{1,5}\s+/.test(line)) {
      section = line.replace(/^#+\s+/, '');
      continue;
    }
    const numbered = line.match(/^\d+[.)]\s+(.+)/);
    const bullet = line.match(/^[-*]\s+(.+)/);
    const text = numbered?.[1] || bullet?.[1] || '';
    if (!text) continue;
    const context = `${section} ${text}`;
    if (/resolved|완료|해결 완료|supersede|closed/i.test(context)) continue;
    if (/TBD|OpenIssue|미해결|Deferred|unknown|확인|필요|여부|정의|충돌|미정|원문|polarity|correlation|routing|우선순위|규칙|의미|단위|Range|IDL|ICD|RTP/i.test(context)) {
      candidates.push({ section, text });
    }
  }
  return uniqueBy(candidates, (item) => item.text).slice(0, 40);
}

function ingest(entries, root) {
  state.files = parseFiles(entries);
  const vehicleFiles = state.files.filter((file) => file.doc && file.root === 'VehicleSpec');
  state.devices = uniqueBy(vehicleFiles.flatMap(parseVehicleFile), (ref) => `${ref.vehicleFile.path}|${ref.id}|${ref.semanticFile?.path || ''}`);
  state.selectedDevice = state.devices[0] || null;
  state.selectedAction = null;
  state.kindFilter = 'All';
  setSource('ok', root, `${state.devices.length}개 장비 참조 · XML 설계 색인 완료`);
  dom.designEmpty.hidden = true;
  dom.designWorkspace.hidden = false;
  renderAll();
}

function totalActions(kind) {
  return state.devices.reduce((sum, ref) => sum + ref.actions.filter((action) => !kind || action.kind === kind).length, 0);
}

function totalBindings() {
  return state.devices.reduce((sum, ref) => sum + ref.actions.reduce((sub, action) => sub + action.bindings.length, 0), 0);
}

function renderStats() {
  const values = [
    state.devices.length,
    totalActions('Control'),
    totalActions('Monitor'),
    totalActions('Product'),
    totalBindings(),
    new Set(state.devices.map((ref) => ref.issueFile?.path).filter(Boolean)).size,
  ];
  [...dom.designStats.querySelectorAll('strong')].forEach((node, index) => { node.textContent = values[index] ?? 0; });
}

function deviceMetrics(ref) {
  return {
    Control: ref.actions.filter((action) => action.kind === 'Control').length,
    Monitor: ref.actions.filter((action) => action.kind === 'Monitor').length,
    Product: ref.actions.filter((action) => action.kind === 'Product').length,
    Binding: ref.actions.reduce((sum, action) => sum + action.bindings.length, 0),
    TBD: ref.issues.length,
  };
}

function renderDeviceList() {
  clear(dom.designDeviceList);
  const query = dom.designDeviceSearch.value.trim().toLowerCase();
  for (const ref of state.devices.filter((item) => !query || item.name.toLowerCase().includes(query) || item.id.toLowerCase().includes(query) || item.identity.name.toLowerCase().includes(query))) {
    const metrics = deviceMetrics(ref);
    const button = make('button', `design-device-item${ref === state.selectedDevice ? ' is-active' : ''}`);
    button.type = 'button';
    button.dataset.deviceKey = `${ref.vehicleFile.path}|${ref.id}`;
    button.append(
      make('strong', '', ref.name || ref.identity.name),
      make('small', '', `${ref.id} · C ${metrics.Control} / M ${metrics.Monitor} / B ${metrics.Binding}${metrics.TBD ? ` · TBD ${metrics.TBD}` : ''}`),
    );
    dom.designDeviceList.append(button);
  }
}

function metricNode(label, number) {
  const node = make('div', 'design-metric');
  node.append(make('span', '', label), make('strong', '', number));
  return node;
}

function renderDeviceHero() {
  clear(dom.designDeviceHero);
  const ref = state.selectedDevice;
  if (!ref) return;
  const text = make('div');
  text.append(make('h2', '', ref.name || ref.identity.name), make('p', '', `${ref.kind} · ${ref.id} · ${ref.cdm || 'CDM 미지정'}`));
  const metrics = make('div', 'design-device-metrics');
  const counts = deviceMetrics(ref);
  Object.entries(counts).forEach(([label, number]) => metrics.append(metricNode(label, number)));
  dom.designDeviceHero.append(text, metrics);
}

function pathNode(label, text) {
  const node = make('div', 'design-path-node');
  node.append(make('b', '', label), make('small', '', text || '미확인'));
  return node;
}

function renderReferencePath() {
  clear(dom.designReferencePath);
  const ref = state.selectedDevice;
  if (!ref) return;
  dom.designReferencePath.append(pathNode('SPECIFICATION', ref.vehicleFile.path), make('i', 'design-path-arrow', '→'), pathNode('SEMANTIC', ref.semanticFile?.path || ref.semanticPath));
  for (const file of ref.bindingFiles) dom.designReferencePath.append(make('i', 'design-path-arrow', '→'), pathNode('BINDING', file.path));
}

function filteredActions() {
  const ref = state.selectedDevice;
  if (!ref) return [];
  const query = dom.designFunctionSearch.value.trim().toLowerCase();
  return ref.actions.filter((action) => (
    (state.kindFilter === 'All' || action.kind === state.kindFilter)
    && (!query || action.name.toLowerCase().includes(query) || action.semanticId.toLowerCase().includes(query) || action.cdm.toLowerCase().includes(query))
  ));
}

function renderFunctionList() {
  clear(dom.designFunctionList);
  const actions = filteredActions();
  actions.sort((a,b) => a.kind.localeCompare(b.kind) || a.name.localeCompare(b.name));
  for (const action of actions) {
    const button = make('button', `design-function-item ${action.kind.toLowerCase()}${action === state.selectedAction ? ' is-active' : ''}`);
    button.type = 'button';
    button.dataset.actionId = action.publicId;
    button.append(make('strong', '', action.name), make('code', '', action.semanticId), make('small', '', `${action.cdm || 'CDM 미지정'} · ${action.bindings.length} Binding`));
    dom.designFunctionList.append(button);
  }
  if (!actions.length) dom.designFunctionList.append(make('p', 'empty-text', '조건에 맞는 기능이 없습니다.'));
}

function dl(rows) {
  const node = make('dl', 'design-kv');
  for (const [label, raw] of rows) {
    node.append(make('dt', '', label));
    const dd = make('dd');
    if (String(raw || '').includes('.') || String(raw || '').includes('::')) dd.append(make('code', '', raw || '—'));
    else dd.textContent = raw || '—';
    node.append(dd);
  }
  return node;
}

function profileTable(profiles) {
  const wrap = make('div', 'design-table-wrap');
  const table = make('table', 'design-table');
  const head = make('thead');
  const hr = make('tr');
  ['항목','CDM','필수','범위 / 단위','값 정의'].forEach((text) => hr.append(make('th', '', text)));
  head.append(hr);
  const body = make('tbody');
  for (const profile of profiles) {
    const row = make('tr');
    const item = make('td');
    item.append(make('strong', '', profile.kind));
    if (profile.description) item.append(make('small', '', profile.description));
    row.append(
      item,
      make('td', '', profile.cdm || '—'),
      make('td', '', profile.required ? '필수' : '선택'),
      make('td', '', [profile.min || profile.max ? `${profile.min || '…'}~${profile.max || '…'}` : '', profile.unit, profile.resolution ? `res ${profile.resolution}` : ''].filter(Boolean).join(' · ') || '—'),
      make('td', '', profile.values.join(' / ') || profile.defaultValue || '—'),
    );
    body.append(row);
  }
  if (!profiles.length) {
    const row = make('tr');
    const cell = make('td', '', '직접 정의된 입력/출력 Profile이 없습니다.');
    cell.colSpan = 5;
    row.append(cell); body.append(row);
  }
  table.append(head, body); wrap.append(table); return wrap;
}

function fieldMeta(field) {
  return [field.cdm, field.value ? `value=${field.value}` : '', field.converter ? `converter=${field.converter}` : '', field.sourceField ? `source=${field.sourceField}` : '', field.description].filter(Boolean).join(' · ');
}

function appendFields(container, fields) {
  for (const field of fields) {
    const row = make('div', 'design-field');
    row.style.setProperty('--depth', field.depth);
    const name = make('div', 'design-field-name', field.name || '(unnamed)');
    const type = make('div', 'design-field-type', `${field.kind}${field.dataType ? ` · ${field.dataType}` : ''}`);
    const meta = make('div', 'design-field-meta', fieldMeta(field) || '—');
    const maps = [...field.maps];
    for (const member of field.bitMembers) {
      maps.push({ kind: 'BitMember', text: `bit ${member.offset}${member.width !== '1' ? ` width ${member.width}` : ''} · ${member.name}${member.cdm ? ` → ${member.cdm}` : ''}` });
    }
    if (maps.length) {
      const chips = make('div', 'design-map');
      maps.forEach((item) => chips.append(make('span', '', `${item.kind}: ${item.text}`)));
      meta.append(chips);
    }
    row.append(name, type, meta); container.append(row);
    if (field.children.length) appendFields(container, field.children);
  }
}

function variantNode(variant, index) {
  const node = make('article', 'design-variant');
  const header = make('header');
  const text = make('div');
  text.append(make('strong', '', `${variant.file.name} · Variant ${index + 1}`), make('small', '', variant.channel.identity));
  header.append(text, make('span', 'design-wire-badge', variant.channel.protocol));
  const body = make('div', 'design-variant-body');
  body.append(dl(Object.entries(variant.channel.attrs).map(([key,val]) => [key,val])));
  const list = make('div', 'design-field-list');
  appendFields(list, variant.fields);
  if (!variant.fields.length) list.append(make('p', 'empty-text', 'Request/Monitor Field 없음'));
  body.append(list); node.append(header, body); return node;
}

function actionIssueCandidates(action) {
  const issues = action.ref.issues;
  const tokens = [action.semanticId, action.name, action.cdm.split('.').pop()].filter((item) => item && item.length > 3).map((item) => item.toLowerCase());
  const matched = issues.filter((issue) => tokens.some((token) => issue.text.toLowerCase().includes(token)));
  return matched.length ? { scope: '선택 기능과 관련된 이슈', items: matched } : { scope: '장비 수준 OpenIssue', items: issues };
}

function renderActionDetail() {
  const action = state.selectedAction;
  dom.designNoFunction.hidden = Boolean(action);
  dom.designFunctionDetail.hidden = !action;
  if (!action) return;

  clear(dom.designFunctionHero);
  const title = make('div');
  title.append(make('h2', '', action.name), make('code', '', action.publicId));
  const meta = make('div', 'design-function-hero-meta');
  meta.append(make('span', `design-badge ${action.kind.toLowerCase()}`, action.kind));
  if (action.cdm) meta.append(make('span', 'design-badge', action.cdm));
  if (action.bindings.length) meta.append(make('span', 'design-badge', `${action.bindings.length} Binding`));
  const issueView = actionIssueCandidates(action);
  if (issueView.items.length) meta.append(make('span', 'design-badge tbd', `TBD ${issueView.items.length}`));
  title.append(meta);
  dom.designFunctionHero.append(title);

  dom.flowFeature.textContent = action.name;
  dom.flowSemantic.textContent = action.cdm || action.semanticId;
  dom.flowBinding.textContent = action.bindings.map((variant) => variant.file.name).join(', ') || 'Binding 없음';
  dom.flowWire.textContent = [...new Set(action.bindings.map((variant) => variant.channel.protocol))].join('/') || '—';
  dom.flowReply.textContent = action.replies.length ? action.replies.map((reply) => reply.bindRef).join(', ') : action.bindings.flatMap((variant) => variant.replies.map((reply) => reply.semanticId)).join(', ') || '없음';

  clear(dom.designSemanticSummary);
  dom.designSemanticSummary.append(dl([
    ['Local ID', action.semanticId], ['CDM', action.cdm], ['설명', action.description],
    ['Target', action.target?.cdm || '없음'], ['Semantic 파일', action.ref.semanticFile?.path || '미확인'],
  ]));
  clear(dom.designSemanticTable); dom.designSemanticTable.append(profileTable(action.profiles));

  clear(dom.designBindingVariants);
  if (action.bindings.length) action.bindings.forEach((variant,index) => dom.designBindingVariants.append(variantNode(variant,index)));
  else dom.designBindingVariants.append(make('p', 'design-clean', '연결된 Binding을 찾지 못했습니다.'));

  clear(dom.designReplyDetail);
  const semanticReplies = action.replies;
  const physicalReplies = action.bindings.flatMap((variant) => variant.replies.map((reply) => ({ variant, reply })));
  if (!semanticReplies.length && !physicalReplies.length) dom.designReplyDetail.append(make('div', 'design-clean', 'Reply가 없는 단방향 기능입니다.'));
  for (const reply of semanticReplies) {
    const item = make('div', 'design-reply-item');
    item.append(make('strong', '', reply.bindRef || 'Reply'), make('code', '', reply.cdm || 'CDM 미지정'), make('small', '', `${reply.required ? '필수' : '선택'}${reply.timeout ? ` · timeout ${reply.timeout}` : ''}${reply.description ? ` · ${reply.description}` : ''}`));
    dom.designReplyDetail.append(item);
  }
  for (const { variant, reply } of physicalReplies) {
    const item = make('div', 'design-reply-item');
    item.append(make('strong', '', `${reply.semanticId} · ${reply.channel.protocol}`), make('code', '', reply.channel.identity), make('small', '', `${variant.file.name} · ${reply.fields.length} Field`));
    dom.designReplyDetail.append(item);
  }

  clear(dom.designIssueDetail);
  if (!issueView.items.length) dom.designIssueDetail.append(make('div', 'design-clean', '현재 선택 폴더에서 연결되는 OpenIssue를 찾지 못했습니다.'));
  else {
    dom.designIssueDetail.append(make('small', 'empty-text', issueView.scope));
    issueView.items.slice(0,12).forEach((issue) => {
      const item = make('div', 'design-issue-item');
      item.append(make('strong', '', issue.section || 'OpenIssue'), make('small', '', issue.text));
      dom.designIssueDetail.append(item);
    });
  }

  dom.designRawSemantic.textContent = action.node?.outerHTML || '';
  dom.designRawBinding.textContent = action.bindings.map((variant) => `<!-- ${variant.file.path} -->\n${variant.node.outerHTML}`).join('\n\n') || '';
}

function renderAll() {
  renderStats();
  renderDeviceList();
  renderDeviceHero();
  renderReferencePath();
  [...dom.designFunctionFilters.querySelectorAll('button')].forEach((button) => button.classList.toggle('is-active', button.dataset.kind === state.kindFilter));
  renderFunctionList();
  renderActionDetail();
}

function selectDevice(key) {
  state.selectedDevice = state.devices.find((ref) => `${ref.vehicleFile.path}|${ref.id}` === key) || null;
  state.selectedAction = null;
  dom.designFunctionSearch.value = '';
  renderAll();
}

function selectAction(id) {
  state.selectedAction = state.selectedDevice?.actions.find((action) => action.publicId === id) || null;
  renderFunctionList();
  renderActionDetail();
  if (window.innerWidth < 1150) dom.designFunctionDetail.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

dom.designFolderInput.addEventListener('change', () => readFolder(dom.designFolderInput.files));
dom.designDeviceSearch.addEventListener('input', renderDeviceList);
dom.designFunctionSearch.addEventListener('input', renderFunctionList);
dom.designFunctionFilters.addEventListener('click', (event) => {
  const button = event.target.closest('[data-kind]');
  if (!button) return;
  state.kindFilter = button.dataset.kind;
  renderAll();
});
dom.designDeviceList.addEventListener('click', (event) => {
  const button = event.target.closest('[data-device-key]');
  if (button) selectDevice(button.dataset.deviceKey);
});
dom.designFunctionList.addEventListener('click', (event) => {
  const button = event.target.closest('[data-action-id]');
  if (button) selectAction(button.dataset.actionId);
});

window.XmlDesignExplorer = {
  ingest,
  getState: () => state,
};
