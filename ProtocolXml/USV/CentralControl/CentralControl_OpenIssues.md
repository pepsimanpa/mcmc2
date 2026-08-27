# CentralControl Semantic/Binding 최종 감사

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 중앙통제장치(MCE).
- 중앙통제가 자율운항/선체제어/네트워크/영상장치로 재배포하는 내부 DDS는 본 Binding 범위에서 제외한다.
- 원통→중통 `RCUHeartBeatReportType` 5 Hz는 원격통제장치 생존정보이므로 중앙통제 HMI 기능으로 노출하지 않는다.
- SSS에서 확정한 공통 규칙(생성 header/commandID, Derived destination, dotted-path struct 접근, CommandStatus 처리상태, converter 금지, PBIT/IBIT 요청-결과 분리)은 중앙통제에도 동일 적용하며 별도 Open Issue로 중복 관리하지 않는다.

## 2. 이번 원문 재검토에서 확정/정리

업로드된 `중앙통제장치 CSCI.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`를 현재 XML과 다시 대조하였다.

- 공용 식별자 규칙에 따라 중앙통제 일반 대상은 `dstEquipmentType=0x06(CoreProcessing)`, `dstEquipmentID=0x01(CentralManagementControlCard)`, `dstSubEquipmentID=0x00`으로 유지한다.
- `USVHeartBeatReportType`의 primitive 자료형을 원문대로 보강한다: `latitude/longitude/course/groundSpeed/roll/pitch = double(Float64)`.
- 이전에 미확정으로 남겼던 HeartBeat 하위 enum raw code는 중앙통제 CSCI에 직접 기재되어 있어 해소한다.
  - `missionModeAC`: 1=경로점운항, 2=목표해점고정, 3=표적추종, 4=Point&Go, 5=자동운항, 6=수상감시정찰, 7=수중감시정찰, 8=자동접안.
  - `operationalModeEOIR`: 0~8 = 대기/수동추적/자동추적/RADAR종속/선수지향/SWING/RCWS연동/STOW/BIT.
  - `operationalModeTSA`: 0~6 = 자동/대기/수동진수/수동회수/수동조출/비상정지/케이블분리.
  - `operationalModeRCWS`: 0~3 = 안전/사격/급탄/보호.
- 위 enum은 Semantic에는 논리 값/CDM만 두고 Binding `ValueMap`에 실제 raw code를 선언한다.
- `MCEIBITReportType` 원문 필드 철자는 `operationalStaus`로 유지하되, Semantic 논리 결과명은 `operationalStatus`로 정상화한다.
- 공용 `ProcessorIBIT` 직접 근거에 따라 `cpuTemperature(Float64, 20~100 ℃)`와 `cpuLoad(Float64, 0~100 %)`를 IBIT Semantic 결과에 반영한다.
- `AuthorityControl.commandTarget`은 공용 구조체에서 `command=0`일 때만 사용한다고 직접 명시되어 있다. `0/1/2`의 의미는 확정한다. `255=이외의 경우`도 원문에 있으나 `command!=0`일 때 반드시 255를 넣으라는 규칙은 없으므로 강제하지 않는다.
- `AuthorityControl.destination`이 3-byte `DestinationType`인 것은 확정되지만 각 command별 실제 값 결정 규칙은 확인되지 않아 BindingOnly/TBD로 유지한다.
- `voiceRxControl`은 필드명/설명이 '음성수신명령'인데 비고의 raw 1 설명은 '송신'으로 적혀 있다. 현재는 raw `UInt8`만 보존하고 논리 `ValueMap`은 만들지 않는다.
- 기존 `MonitorBindingDDS/ControlBindingDDS/Channel` → 공통 `MonitorBinding/ControlBinding/DDSChannel`, CommandStatus 처리상태, OperationMode 0~7 독립 Control, Authority command 0~4 독립 Control, Wireless Preset PackedField, VHF/Volume/Squelch/영상분배 구조는 원문 재대조 후 그대로 유지한다.

## 3. 중앙통제 고유 Remaining TBD — 8개

1. **AuthorityControl destination / 비사용 필드 초기화 규칙**
   - `destination`의 물리 구조가 `DestinationType(3 bytes)`인 것은 확정이다.
   - 다만 `command=0~4` 각각에서 실제 어떤 DestinationType 값을 넣어야 하는지는 원문에 명시되지 않았다.
   - `commandTarget`은 `command=0`에서 사용한다고 명시되어 있으나 `command!=0`일 때 해당 byte를 0/255/기존값 중 무엇으로 초기화하는지도 직접 규칙이 없다.
   - 따라서 임의 FixedField로 강제하지 않고 현재 raw 필드를 유지한다.

2. **VHF `rxChannel` 값 체계**
   - `long` 4-byte 및 '채널/채널 및 주파수' 설명만 확인된다.
   - 허용 범위, 주파수 단위, 채널표는 원문만으로 확정할 수 없다.

3. **`MCEPBITReportType` 1 Hz 표기의 의미**
   - 중앙통제 CSCI는 `MCEPBITReportType`을 '중앙통제 PBIT 응답'으로 명명하면서 주기를 명시적으로 `1 Hz`로 표기한다.
   - 원격통제장치에는 별도 `RCUPBITControlType` 비주기 PBIT 요청이 존재한다.
   - 현재는 요청 Control의 전용 Result Reply로 유지하지만, 실제로 요청 직후 1회 결과인지 / 1 Hz로 최신 결과를 지속 송신하는지 / 두 역할을 겸하는지 확인이 필요하다.

4. **PBIT/IBIT Result correlation**
   - 요청에는 commandID가 있으나 `MCEPBITReportType`/`MCEIBITReportType` 결과에는 commandID가 없다.
   - CommandStatus ACK는 commandID로 연결되지만 전용 결과의 런타임 매칭 규칙은 확인이 필요하다.

5. **`MCEIBITReportType.operationalStaus` 실제 IDL 철자**
   - 중앙통제 CSCI 원문은 `operationalStaus`로 적혀 있어 Binding은 그대로 보존한다.
   - 실제 DDS IDL도 동일 오탈자인지, 구현에서는 `operationalStatus`로 수정되었는지는 확인 필요하다.
   - Semantic 논리명은 물리 철자와 분리해 `operationalStatus`를 사용한다.

6. **`VoiceRxTxControlType.voiceRxControl` 설명 불일치**
   - 필드명/설명은 '음성수신명령'이나 비고는 `0=중지, 1=송신`이다.
   - raw 0/1은 보존하되 raw 1의 논리 의미를 `Receive/Enable/Transmit` 중 하나로 추정하지 않는다.

7. **`ControlCommandReportType` 원본 `#REF!`**
   - `controlCommand` 필드와 값 의미(0=USV, 1=고정형, 2=이동형, 3=출입항, 4=정비용 노트북)는 식별 가능하다.
   - 주변 source/type 셀의 `#REF!`는 원본 결함으로 남는다.

8. **`operationalStateUSV` 2~7 의미**
   - 원문 Range는 0~7이나 직접 확인된 의미는 0=정상, 1=비정상 경고뿐이다.
   - 2~7 의미를 추정하지 않고 Semantic/Binding에서도 현재 제외한다.

## 4. 전역 후속 항목

- CDM 명칭/계층은 다른 USV 장치 완료 후 최종 USV CDM 감사에서 일괄 재검토한다. 이는 중앙통제 고유 Open Issue로 세지 않는다.
- 공통 dotted-path Adapter 규칙, commandID 없는 결과 Report correlation, 로컬/중복 XSD 정리 여부 역시 USV 전체 전역 감사에서 재확인한다.

## 5. 현재 상태

- 중앙통제 Semantic/Binding의 구조 방향은 원문과 정합한다.
- HeartBeat 하위 4개 enum raw code 미확정 이슈는 이번 원문 재검토로 **해소**하였다.
- 원문 직접 근거가 있는 primitive `dataType`, ValueMap, ProcessorIBIT 범위, Semantic 논리명 보강을 반영하였다.
- 남은 8개 항목은 설계자가 임의 결정할 사항이 아니라 추가 원문/IDL/구현 근거가 필요한 비차단 TBD이다.
