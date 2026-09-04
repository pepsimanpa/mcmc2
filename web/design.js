'use strict';

const $ = (id) => document.getElementById(id);
const dom = Object.fromEntries([
  'folderInput','landing','landingStatus','workspace','deviceSearch','deviceList','deviceTitle','deviceSubtitle','deviceStats',
  'sourceSummary','sourceFiles','issueSummary','featureSearch','featureGrid','emptyDetail','featureDetail','featureKind','featureId',
  'featureTitle','featureCdm','openHmiButton','semanticSummary','semanticParams','bindingSummary','bindingVariants','replyPanel','replyList','evidenceList'
].map((id)=>[id,$(id)]));

const state={bundle:null,device:null,feature:null,filter:'all'};
const FIELD_NAMES=new Set(['Field','FixedField','DerivedField','ArrayField','PackedField']);
const CHANNEL_NAMES=new Set(['DDSChannel','TCPChannel','UDPChannel','RTPChannel','RS422Channel','Channel']);

function el(tag,cls='',text=''){const n=document.createElement(tag);if(cls)n.className=cls;if(text!==undefined)n.textContent=text;return n;}
function clear(n){if(n)n.replaceChildren();}
function direct(n,name){return n?[...n.children].filter((c)=>!name||c.localName===name):[];}
function first(n,name){return direct(n,name)[0]||null;}
function all(n,name){return n?[...n.getElementsByTagNameNS('*',name)]:[];}
function attr(n,name){return n?.getAttribute(name)||'';}
function text(n){return n?.textContent?.trim()||'';}
function base(path){return String(path||'').replace(/\\/g,'/').split('/').pop();}
function norm(path){const out=[];for(const p of String(path||'').replace(/\\/g,'/').split('/')){if(!p||p==='.')continue;if(p==='..')out.pop();else out.push(p);}return out.join('/');}
function dir(path){const p=norm(path);const i=p.lastIndexOf('/');return i<0?'':p.slice(0,i);}
function rel(from,ref){return norm([dir(from),ref].filter(Boolean).join('/'));}
function refId(raw){const s=String(raw||'');return s.includes('#')?s.slice(s.lastIndexOf('#')+1):s;}
function commentBefore(node){let p=node?.previousSibling;while(p&&p.nodeType===Node.TEXT_NODE)p=p.previousSibling;return p?.nodeType===Node.COMMENT_NODE?p.nodeValue.trim().replace(/\s+/g,' '):'';}

async function readFolder(files){
  const selected=[...files]; if(!selected.length)return;
  dom.landingStatus.textContent='XML · XSD · OpenIssue 문서를 읽는 중...';
  const entries=[];
  for(const f of selected){const ext=base(f.name).toLowerCase().split('.').pop();if(!['xml','xsd','md','idl'].includes(ext))continue;entries.push({name:f.name,path:norm(f.webkitRelativePath||f.name),text:await f.text(),ext});}
  state.bundle=buildBundle(entries);
  dom.landing.hidden=true; dom.workspace.hidden=false;
  renderDevices();
  const firstDevice=state.bundle.devices[0]; if(firstDevice)selectDevice(firstDevice.key);
}

function parseDocs(entries){return entries.map((entry)=>{
  const file={...entry,doc:null};
  if(['xml','xsd'].includes(entry.ext)){
    file.doc=new DOMParser().parseFromString(entry.text,'application/xml');
    file.root=file.doc.documentElement?.localName||'';
    file.bad=Boolean(file.doc.getElementsByTagName('parsererror')[0]);
  }
  return file;
});}

function resolve(files,owner,reference){
  const target=rel(owner.path,reference).toLowerCase();
  return files.find((f)=>f.path.toLowerCase()===target)||files.find((f)=>f.name.toLowerCase()===base(reference).toLowerCase())||null;
}

function parseProfile(node){
  const range=all(node,'Range')[0],unit=all(node,'Unit')[0],res=all(node,'Resolution')[0];
  return {kind:node.localName,cdm:attr(node,'cdm'),required:attr(node,'required')!=='false'&&attr(node,'minOccurs')!=='0',defaultValue:attr(node,'default'),unit:text(unit),min:range?attr(range,'min'):'',max:range?attr(range,'max'):'',resolution:text(res),description:commentBefore(node)};
}

function parseFeatures(ref,semantic){
  const features=[];
  const controls=all(semantic.doc,'ControlSpecs')[0];
  if(controls)for(const n of direct(controls).filter((x)=>['ControlSpec','SetPointSpec'].includes(x.localName))){
    const params=first(n,'Parameters');
    features.push({kind:'Control',id:attr(n,'id'),name:attr(n,'name')||attr(n,'id'),cdm:attr(n,'cdm'),description:commentBefore(n),params:params?direct(params).filter((x)=>x.localName.endsWith('Profile')||x.localName.endsWith('Spec')).map(parseProfile):[],replies:direct(n,'Reply').map((r)=>({cdm:attr(r,'cdm'),bindRef:attr(r,'bindRef'),required:attr(r,'required')!=='false',timeout:attr(r,'timeout')})),bindings:[],semanticFile:semantic,ref});
  }
  const monitors=all(semantic.doc,'MonitorSpecs')[0];
  if(monitors)for(const n of direct(monitors,'GroupSpec'))features.push({kind:'Monitor',id:attr(n,'id'),name:attr(n,'name')||attr(n,'id'),cdm:attr(n,'cdm'),description:commentBefore(n),params:direct(n).filter((x)=>x.localName.endsWith('Spec')).map(parseProfile),replies:[],bindings:[],semanticFile:semantic,ref});
  const products=all(semantic.doc,'SensorProductSpecs')[0];
  if(products)for(const n of direct(products).filter((x)=>['ProductStreamSpec','ProductFileSpec','ProductFrameSpec'].includes(x.localName)))features.push({kind:'Product',id:attr(n,'id'),name:attr(n,'name')||attr(n,'id'),cdm:attr(n,'cdm'),description:commentBefore(n),params:[],replies:[],bindings:[],semanticFile:semantic,ref});
  return features;
}

function channel(node,fileName){
  const c=direct(node).find((x)=>CHANNEL_NAMES.has(x.localName)); if(!c)return null;
  let protocol=c.localName.replace('Channel','').toUpperCase(); if(c.localName==='Channel')protocol=/tcp|rf/i.test(fileName)?'TCP':/udp/i.test(fileName)?'UDP':'DDS';
  return {protocol,topic:attr(c,'topicName'),type:attr(c,'typeName'),infoCode:attr(c,'infoCode')||attr(c,'topicName'),messageType:attr(c,'messageType')||attr(c,'typeName')};
}

function parseField(n){
  const children=[]; const element=first(n,'Element'); if(element)for(const c of direct(element).filter((x)=>FIELD_NAMES.has(x.localName)||x.localName==='BitMember'))children.push(parseField(c));
  if(n.localName==='PackedField')for(const c of direct(n,'BitMember'))children.push(parseField(c));
  const maps=all(n,'Map').map((m)=>({cdm:attr(m,'cdm'),value:attr(m,'value')}));
  return {kind:n.localName,name:attr(n,'name'),cdm:attr(n,'cdm'),dataType:attr(n,'dataType'),value:attr(n,'value'),sourceField:attr(n,'sourceField'),converter:attr(n,'converter'),offset:attr(n,'offset'),width:attr(n,'width'),maps,children,description:commentBefore(n)};
}

function parseBindingNode(n,file){
  return {semanticId:refId(attr(n,'semantic_id')),channel:channel(n,file.name),fields:direct(n).filter((x)=>FIELD_NAMES.has(x.localName)).map(parseField),replies:direct(n,'Reply').map((r)=>({semanticId:refId(attr(r,'semantic_id')),channel:channel(r,file.name),fields:direct(r).filter((x)=>FIELD_NAMES.has(x.localName)).map(parseField)})),file};
}

function attachBindings(features,files){
  for(const file of files){
    const groups=[['Controls',['ControlBinding','ControlBindingDDS']],['Monitors',['MonitorBinding','MonitorBindingDDS']],['SensorProducts',['ProductBinding']]];
    for(const [groupName,names] of groups){const g=all(file.doc,groupName)[0];if(!g)continue;for(const n of direct(g).filter((x)=>names.includes(x.localName))){const b=parseBindingNode(n,file);const f=features.find((x)=>x.id===b.semanticId);if(f)f.bindings.push(b);}}
  }
}

function issueForDevice(files,semantic){
  const folder=dir(semantic.path).toLowerCase();
  const md=files.find((f)=>f.ext==='md'&&dir(f.path).toLowerCase()===folder&&/_openissues\.md$/i.test(f.name));
  if(!md)return {file:null,count:0,items:[]};
  const items=md.text.split(/\r?\n/).map((s)=>s.trim()).filter((s)=>/^\d+[.)]\s+/.test(s)||/^[-*]\s+.*(?:TBD|OPEN|미해결)/i.test(s));
  return {file:md,count:items.length,items};
}

function buildBundle(entries){
  const files=parseDocs(entries); const specs=files.filter((f)=>f.doc&&!f.bad&&f.root==='VehicleSpec'); const devices=[];
  for(const spec of specs){
    const refs=[...spec.doc.documentElement.getElementsByTagName('*')].filter((n)=>n.localName.endsWith('SpecRef'));
    for(const n of refs){const sp=text(first(n,'SemanticPath'));const semantic=resolve(files,spec,sp);if(!semantic?.doc)continue;const bindingFiles=direct(n,'BindingPath').map((x)=>resolve(files,spec,text(x))).filter((x)=>x?.doc);const ref={id:attr(n,'id'),name:attr(n,'name')||attr(n,'id'),cdm:attr(n,'cdm'),spec,semantic,bindingFiles};const features=parseFeatures(ref,semantic);attachBindings(features,bindingFiles);const issues=issueForDevice(files,semantic);devices.push({key:`${spec.path}::${ref.id}`,ref,features,issues});}
  }
  return {files,devices};
}

function renderDevices(filter=''){
  clear(dom.deviceList); if(!state.bundle)return; const q=filter.toLowerCase();
  for(const d of state.bundle.devices.filter((x)=>!q||x.ref.name.toLowerCase().includes(q)||x.ref.id.toLowerCase().includes(q))){const b=el('button','device-button'+(state.device?.key===d.key?' is-active':''));b.type='button';b.dataset.device=d.key;b.append(el('strong','',d.ref.name),el('small','',`${d.features.filter((f)=>f.kind==='Control').length} Control · ${d.features.filter((f)=>f.kind==='Monitor').length} Monitor`));dom.deviceList.append(b);}
}

function stat(label,value){const a=el('div','stat');a.append(el('span','',label),el('strong','',value));return a;}
function selectDevice(key){state.device=state.bundle.devices.find((d)=>d.key===key);state.feature=null;if(!state.device)return;renderDevices(dom.deviceSearch.value);dom.deviceTitle.textContent=state.device.ref.name;dom.deviceSubtitle.textContent=state.device.ref.cdm||`${state.device.ref.id} 설계`;clear(dom.deviceStats);dom.deviceStats.append(stat('CONTROL',state.device.features.filter((f)=>f.kind==='Control').length),stat('MONITOR',state.device.features.filter((f)=>f.kind==='Monitor').length),stat('PRODUCT',state.device.features.filter((f)=>f.kind==='Product').length));dom.sourceSummary.textContent='Specification → Semantic → Binding';dom.sourceFiles.textContent=[base(state.device.ref.spec.path),base(state.device.ref.semantic.path),...state.device.ref.bindingFiles.map((f)=>base(f.path))].join(' · ');dom.issueSummary.textContent=state.device.issues.file?`${state.device.issues.count}개 항목 추적 중`:'OpenIssue 문서 없음';renderFeatures();dom.featureDetail.hidden=true;dom.emptyDetail.hidden=false;}

function renderFeatures(){clear(dom.featureGrid);if(!state.device)return;const q=dom.featureSearch.value.toLowerCase();const list=state.device.features.filter((f)=>(state.filter==='all'||f.kind===state.filter)&&(!q||f.name.toLowerCase().includes(q)||f.id.toLowerCase().includes(q)||f.cdm.toLowerCase().includes(q)));for(const f of list){const b=el('button','feature-card'+(state.feature===f?' is-active':''));b.type='button';b.dataset.feature=f.id;const top=el('div','topline');top.append(el('span',`kind-pill ${f.kind}`,f.kind.toUpperCase()),el('small','',`${f.bindings.length} Binding`));b.append(top,el('strong','',f.name),el('code','',f.cdm||f.id));dom.featureGrid.append(b);}if(!list.length)dom.featureGrid.append(el('div','empty-detail','조건에 맞는 기능이 없습니다.'));}

function summaryItem(label,value){const n=el('div','summary-item');n.append(el('span','',label),el('strong','',value||'—'));return n;}
function flatFields(fields){const out=[];for(const f of fields){out.push(f);out.push(...flatFields(f.children||[]));}return out;}
function fieldMeaning(f){if(f.kind==='FixedField')return `고정값 ${f.value}`;if(f.kind==='DerivedField')return [f.sourceField,f.converter].filter(Boolean).join(' → ')||'Derived';if(f.kind==='PackedField')return `Packed ${f.children.length} bit member`;if(f.maps?.length)return f.maps.map((m)=>`${m.cdm}=${m.value}`).join(', ');return f.cdm||f.description||'직접 필드';}

function selectFeature(id){state.feature=state.device.features.find((f)=>f.id===id);if(!state.feature)return;renderFeatures();dom.emptyDetail.hidden=true;dom.featureDetail.hidden=false;dom.featureKind.textContent=state.feature.kind.toUpperCase();dom.featureKind.className=`kind-pill ${state.feature.kind}`;dom.featureId.textContent=state.feature.id;dom.featureTitle.textContent=state.feature.name;dom.featureCdm.textContent=state.feature.cdm||'CDM 미지정';dom.openHmiButton.hidden=state.feature.kind!=='Control';renderSemantic();renderBinding();renderReplies();renderEvidence();}

function renderSemantic(){clear(dom.semanticSummary);clear(dom.semanticParams);const f=state.feature;dom.semanticSummary.append(summaryItem('Semantic XML',base(f.semanticFile.path)),summaryItem('CDM',f.cdm||'—'),summaryItem('Local ID',f.id),summaryItem('입력/결과 항목',String(f.params.length)));if(f.description)dom.semanticSummary.append(summaryItem('설명',f.description));const list=el('div','param-list');if(!f.params.length)list.append(el('div','param-row','직접 정의된 Parameter/Profile이 없습니다.'));for(const p of f.params){const row=el('div','param-row');row.append(el('code','',p.cdm||p.kind),el('span','',[p.min||p.max?`${p.min||'…'} ~ ${p.max||'…'}`:'',p.unit].filter(Boolean).join(' · ')||p.description||'의미 프로필'),el('small','',p.required?'필수':'선택'));list.append(row);}dom.semanticParams.append(list);}

function renderBinding(){clear(dom.bindingSummary);clear(dom.bindingVariants);const f=state.feature;dom.bindingSummary.append(summaryItem('Binding 수',String(f.bindings.length)),summaryItem('전송 방식',[...new Set(f.bindings.map((b)=>b.channel?.protocol||'UNKNOWN'))].join(' / ')||'—'));if(!f.bindings.length){dom.bindingVariants.append(el('div','empty-detail','연결된 Binding이 없습니다.'));return;}for(const b of f.bindings){const card=el('article','binding-card');const h=el('header');h.append(el('strong','',`${b.channel?.protocol||'UNKNOWN'} · ${base(b.file.path)}`),el('small','',`${flatFields(b.fields).length} fields`));card.append(h);const meta=el('div','wire-meta');if(b.channel?.protocol==='DDS'){meta.append(summaryWire('Topic',b.channel.topic),summaryWire('Type',b.channel.type));}else{meta.append(summaryWire('Info Code',b.channel?.infoCode),summaryWire('Message Type',b.channel?.messageType));}card.append(meta);const table=el('div','field-table');for(const field of flatFields(b.fields)){const r=el('div','field-row');r.append(el('code','',field.name||'(unnamed)'),el('span','',field.dataType||field.kind),el('span','',fieldMeaning(field)));table.append(r);}card.append(table);dom.bindingVariants.append(card);}}
function summaryWire(label,value){const n=el('div');n.append(el('span','',label),el('b','',value||'—'));return n;}

function renderReplies(){clear(dom.replyList);const f=state.feature;const physical=f.bindings.flatMap((b)=>b.replies.map((r)=>({...r,binding:b})));const replies=[...f.replies];dom.replyPanel.hidden=f.kind!=='Control'||(!replies.length&&!physical.length);if(dom.replyPanel.hidden)return;if(!replies.length)dom.replyList.append(replyItem('Semantic Reply 없음','Binding에만 물리 Reply가 정의되어 있는지 확인'));for(const r of replies){const matches=physical.filter((p)=>p.semanticId===r.bindRef);dom.replyList.append(replyItem(r.bindRef||r.cdm,`${r.cdm||'—'} · ${r.required?'필수':'선택'}${matches.length?` · ${matches.map((m)=>m.channel?.topic||m.channel?.messageType||m.channel?.protocol).join(' / ')}`:''}`));}}
function replyItem(title,detail){const n=el('article','reply-item');n.append(el('strong','',title),el('small','',detail));return n;}

function renderEvidence(){clear(dom.evidenceList);const f=state.feature;dom.evidenceList.append(evidence('confirmed','Semantic source confirmed',base(f.semanticFile.path)));if(f.bindings.length)dom.evidenceList.append(evidence('confirmed','Binding source confirmed',f.bindings.map((b)=>base(b.file.path)).join(', ')));else dom.evidenceList.append(evidence('tbd','Binding TBD','연결된 물리 Binding이 없음'));const fields=f.bindings.flatMap((b)=>flatFields(b.fields));const derived=fields.filter((x)=>x.kind==='DerivedField').length,mapped=fields.filter((x)=>x.maps?.length).length,packed=fields.filter((x)=>x.kind==='PackedField').length;if(derived||mapped||packed)dom.evidenceList.append(evidence('runtime','변환 규칙 존재',[derived?`Derived ${derived}`:'',mapped?`ValueMap ${mapped}`:'',packed?`Packed ${packed}`:''].filter(Boolean).join(' · ')));if(state.device.issues.file)dom.evidenceList.append(evidence('tbd',`${state.device.issues.count}개 OpenIssue 추적`,base(state.device.issues.file.path)));else dom.evidenceList.append(evidence('confirmed','추가 OpenIssue 문서 미검출','선택한 폴더 범위 기준'));}
function evidence(type,title,detail){const n=el('article',`evidence-item ${type}`);n.append(el('span','evidence-dot'),(()=>{const d=el('div');d.append(el('strong','',title),el('small','',detail));return d;})());return n;}

dom.folderInput.addEventListener('change',()=>readFolder(dom.folderInput.files));
dom.deviceSearch.addEventListener('input',()=>renderDevices(dom.deviceSearch.value));
dom.featureSearch.addEventListener('input',renderFeatures);
document.addEventListener('click',(e)=>{const d=e.target.closest('[data-device]');if(d)selectDevice(d.dataset.device);const f=e.target.closest('[data-feature]');if(f)selectFeature(f.dataset.feature);const filter=e.target.closest('[data-filter]');if(filter){state.filter=filter.dataset.filter;document.querySelectorAll('[data-filter]').forEach((b)=>b.classList.toggle('is-active',b===filter));renderFeatures();}});
dom.openHmiButton.addEventListener('click',()=>{location.href='index.html#hmi';});
