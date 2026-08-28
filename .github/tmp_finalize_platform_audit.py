from pathlib import Path

p = Path('ProtocolXml/USV/PlatformController/PlatformController_OpenIssues.md')
s = p.read_text(encoding='utf-8')
old = '''9. **[RESOLVED] DDS boolean wire type**
   - 공통 `WireDataType`에 `Boolean`을 추가하였다.
   - 원 CSCI/IDL에서 실제 DDS boolean으로 확인된 CBIT/환경상태 직접 Field 10개를 `dataType="Boolean"`으로 선언하였다.
   - octet으로 정의된 ENV command와 bow-thruster enable 등은 기존 UInt8/SourceValueMap을 유지한다.

10. **일부 선체 상태/IBIT 상세 primitive dataType**
   - 선체제어 CSCI의 대형 상태/IBIT 구조체 중 현재 확보 근거에서 primitive type을 직접 재확인하지 못한 필드는 BindingOnly 또는 dataType 미지정으로 유지한다.
   - 전역 primitive dataType 감사 시 원 선체제어 CSCI/IDL을 기준으로 추가 보강해야 한다.'''
new = '''9. **[RESOLVED] PlatformController Boolean wire 재감사**
   - `선체제어장치 CSCI.csv`를 직접 재확인한 결과 PlatformController 원문에는 primitive `boolean` 필드가 없다.
   - 기존에 `Boolean`으로 선언했던 `fireDetected`, `leakDetected`, `flowAnomalyDetected`, `fireSensor1~3Alarm`, `autoExtinguisher1~4Active` 10개는 모두 원문 `octet`이므로 `UInt8`로 정정하였다.
   - 이 필드들의 Semantic Boolean 의미는 유지하되, wire primitive type은 원문 `octet`을 따른다.

10. **[RESOLVED] 선체 상태/PBIT/IBIT primitive dataType 전수 감사**
   - `선체제어장치 CSCI.csv`의 각 DDS message/type별 필드 정의를 기준으로 기존 미지정 primitive field의 `dataType`을 전수 보강하였다.
   - Engine/WaterJet/BowThruster/Power/Battery/Generator/Environment/Wind/Fire/Leak/Flow/Stabilizer/Interceptor/SeaWaterConditioner/TowingDevice/SPCE IBIT와 CBIT/PBIT, Propulsion/ENV/Power HeartBeat, Distress/Wind/SeaWaterConditioner 상태를 원문 타입대로 반영하였다.
   - `vcuProcessorIBIT`, `vcuTmpProcessorIBIT`는 원문 `ProcessorIBIT` 복합 구조체이므로 primitive `dataType`을 부여하지 않는다.
   - 최종 감사에서 composite/system field를 제외한 primitive `dataType` 미지정은 0건이며 XSD 검증을 통과하였다.'''
if old in s:
    s = s.replace(old, new)
elif new not in s:
    raise SystemExit('Platform OpenIssues target block not found')
p.write_text(s, encoding='utf-8')

p = Path('ProtocolXml/Docs/USV_Semantic_Binding_Integrated_Audit_20260828.md')
d = p.read_text(encoding='utf-8')
reps = [
    ('실제 DDS/IDL boolean 직접 필드 21개 반영: ElectroOptical 8, ElectroOpticalProcessor 1, PlatformController 10, NetworkAbstraction 2',
     '실제 DDS/IDL boolean 직접 필드 11개 반영: ElectroOptical 8, ElectroOpticalProcessor 1, NetworkAbstraction 2. PlatformController는 원 CSCI 재감사에서 직접 boolean 0개로 확인되어 기존 10개를 UInt8(octet)로 정정'),
    ('최종 감사에서 `usvHeader` / `destination` composite를 제외하고 남은 primitive `dataType` 미지정은 총 **419건**이다.',
     '최종 감사에서 `usvHeader` / `destination` 및 `ProcessorIBIT` 같은 composite를 제외하고 남은 primitive `dataType` 미지정은 총 **0건**이다.'),
    ('| PlatformController | 419 | 실제 DDS boolean 10개 해결; 나머지 legacy 상태/PBIT/IBIT primitive type 근거 부족 |',
     '| PlatformController | 0 | `선체제어장치 CSCI.csv` 전수 재확인으로 상태/PBIT/IBIT/Heartbeat primitive type 반영; 기존 Boolean 10개는 원문 octet에 따라 UInt8로 정정 |'),
    ('| **합계** | **419** |  |', '| **합계** | **0** |  |'),
    ('이 419건은 XSD 오류가 아니다. 현재 XSD에서 `dataType` 속성 자체는 선택적이다.\n\n다만 프로젝트 설계 규칙상 확인 가능한 primitive type은 선언해야 하므로, **추가 CSCI / IDL 근거 확보 후 보강할 공통 후속항목**으로 관리한다.',
     '현재 14개 USV 장치의 source-confirmed primitive field는 모두 `dataType`이 선언되었다. `usvHeader`, `destination`, `ProcessorIBIT` 등 복합 구조체는 primitive `WireDataType` 대상이 아니므로 예외로 유지한다.'),
    ('### USV-COMMON-04 — 원문 부족 primitive wire dataType\n\n- 총 미지정: 419건\n- Boolean schema gap은 해결되었고, 남은 항목은 CSCI/IDL primitive 근거 미확보 항목\n- 원문 없이 추정하여 채우지 않는다.',
     '### USV-COMMON-04 — 원문 부족 primitive wire dataType — **RESOLVED (2026-08-28)**\n\n- NavigationRadar 및 NetworkAbstraction의 잔여 type은 장치 CSCI/공용 구조체에서 확인하여 해결하였다.\n- PlatformController의 기존 미지정 항목은 `선체제어장치 CSCI.csv`를 message/type별로 전수 재감사하여 primitive type을 확정하였다.\n- 최종 primitive `dataType` 미지정: 0건. 복합 구조체에는 primitive type을 강제하지 않는다.')
]
for a, b in reps:
    if a in d:
        d = d.replace(a, b)
    elif b not in d:
        raise SystemExit('Integrated audit target not found: ' + a[:100])
if '419' in d:
    raise SystemExit('stale 419 remains in integrated audit')
p.write_text(d, encoding='utf-8')
print('DOC_UPDATE_OK')
