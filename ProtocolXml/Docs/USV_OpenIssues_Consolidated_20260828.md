# USV Semantic / Binding OpenIssue 최종 통합 목록

- 기준일: 2026-08-28
- 저장소: `pepsimanpa/mcmc2`
- 브랜치: `feature/usv-semantic-binding-audit`
- 범위: 원격통제장치(RCU)와 직접 연동하는 USV 장치 14종
- 상위 설계 기준: `ProtocolXml/Docs/UMS_Semantic_Binding_Design_Rules_20260826.md`
- 통합 감사: `ProtocolXml/Docs/USV_Semantic_Binding_Integrated_Audit_20260828.md`
- CSV 재심층 감사 최종 갱신: 2026-08-30

---

## 1. 목적과 관리 원칙

장치별 `*_OpenIssues.md`는 원 CSCI/IDL의 세부 맥락을 보존하기 위해 유지한다. 이 문서는 장치별 문서에 반복되는 동일 근본원인을 **공통 master issue**로 묶고, 그 외 항목을 **장치 고유 issue**로 최종 분리한 master tracking 문서다.

- 공통 이슈는 `USV-COMMON-*` ID를 master로 사용한다.
- 장치별 문서의 동일 항목은 삭제하지 않고 공통 ID의 장치별 근거/증상으로 본다.
- 원 CSCI/IDL/Adapter 근거가 없는 값을 통합 일관성만으로 추정하여 닫지 않는다.
- 현재 남은 항목은 Semantic/Binding XML의 XSD/참조 정합성을 깨는 blocker가 아니라, 추가 원문·IDL·Adapter·운용설정 근거가 필요한 비차단 TBD이다.

---

## 2. 최종 기계 검증 상태

2026-08-28 최종 재검증 결과:

| 항목 | 결과 |
|---|---:|
| Semantic XML | 14 / 14 XSD PASS |
| Binding XML | 14 / 14 XSD PASS |
| Control/Reply/Monitor/Product 참조 오류 | 0 |
| converter | 0 |
| primitive `dataType` 미지정 | 0 |
| PackedField | 111 |
| 실제 DDS/IDL `Boolean` Field | 11 |

실제 DDS/IDL Boolean 11개는 ElectroOptical 8, ElectroOpticalProcessor 1, NetworkAbstraction 2이며 PlatformController는 원 CSCI 재감사 결과 0개다.

---

## 3. 공통 OpenIssue master

### USV-COMMON-01 — PBIT / IBIT 비동기 결과 correlation

- 적용: 14개 전 장치.
- PBIT/IBIT 실제 Result Reply는 총 61개이고 결과 Report의 `commandID`는 0개다.
- 공용 요청에는 `commandID`가 있으나 전용 점검 결과에는 동일 식별자가 없어, CommandStatus 처리 ACK 이후 실제 결과를 어떤 요청과 연결할지 runtime 규칙이 필요하다.
- 필요 근거: Adapter correlation 규칙, 동시 요청 허용 정책, source/시간 기반 매칭 규칙.
- 상태: **OPEN / runtime integration**.

### USV-COMMON-02 — DDS Boolean wire type — RESOLVED

- `CommonBindingSchema.xsd`의 `WireDataType`에 `Boolean`을 반영하였다.
- 실제 DDS/IDL boolean만 `dataType="Boolean"`으로 사용하고, Semantic Boolean이 octet/packed bit/sentinel에서 파생되는 경우 원 wire type을 유지한다.
- 상태: **RESOLVED (2026-08-28)**.

### USV-COMMON-03 — Result source routing / one-request multi-result fan-out

- 주요 적용: ElectroOpticalProcessor, Lidar, NavigationRadar, NearContactDetection, NetworkAbstraction, PlatformController, SensorFusion.
- 공용 CIPE Topic을 여러 처리카드가 공유하거나, 하나의 PBIT/IBIT 요청에서 복수 물리 Result가 발생하는 구조가 있다.
- XML에서는 물리 Report별 Reply를 유지하는 것이 맞으며, runtime에서 `usvHeader`, equipment/subEquipment, Topic context를 이용한 source discrimination과 상위 완료 판정 규칙이 필요하다.
- 상태: **OPEN / runtime routing & aggregation**.

### USV-COMMON-04 — primitive wire `dataType` 부족 — RESOLVED

- 14개 USV Binding에서 source-confirmed primitive `dataType` 미지정은 0건이다.
- `usvHeader`, `destination`, `ProcessorIBIT` 등 composite는 primitive WireDataType 대상이 아니므로 예외다.
- 상태: **RESOLVED (2026-08-28)**.

### USV-COMMON-05 — count 필드와 sequence 실제 길이 일치 규칙

- 적용: ElectroOpticalProcessor, Lidar, NavigationCamera, NavigationRadar, SensorFusion.
- 원문에 `*Cnt`/`contactNum`/`waveNum`과 sequence가 동시에 존재하지만 값 불일치 시 우선순위/오류 처리 규칙이 없다.
- 현재 Semantic은 source-confirmed collection bound만 유지하고 불일치 처리 알고리즘은 만들지 않는다.
- 필요 근거: IDL bound, Adapter/수신측 validation 규칙.
- 상태: **OPEN / runtime validation**.

### USV-COMMON-06 — 기호 Range(`-PI~+PI`)의 XSD 표현

- 적용: ElectroOpticalProcessor, NavigationCamera, SensorFusion.
- 원문은 정확한 `-PI~+PI` 범위를 사용하지만 현 `Range`는 decimal만 허용한다.
- 임의 소수 근사값을 계약값으로 넣지 않고 Unit/주석으로 원문을 보존한다.
- 필요 근거: CommonSpecSchema의 상수/수식 Range 지원 여부 결정.
- 상태: **OPEN / common XSD enhancement**.

### USV-COMMON-07 — 공용 CIPE `cipeOperationalStatus` producer/subset 의미

- 적용: ElectroOpticalProcessor, Lidar, NavigationRadar, SensorFusion.
- 공용 구조체는 Radar/Lidar/NearContact/EOIR Connected/Disconnected 코드를 한 octet에 정의하지만 각 처리카드가 실제로 어느 subset을 발행하는지 일관된 명시가 없다.
- 현재 각 장치는 직접 근거가 있는 범위만 허용하거나, 원문 구조에 따라 공용 값을 유지한다.
- 필요 근거: CIPE 공용 상태 producer 계약/IDL/원작자 확인.
- 상태: **OPEN / shared source semantics**.

### USV-COMMON-08 — nested structure dotted-path Adapter 접근 규칙

- 대표 표기: `commandStatusReport.dstEquipmentType`, `commandStatusReport.status`.
- XSD/현재 XML에서는 문자열 경로로 유효하지만 runtime Adapter가 dotted path를 공식 nested member 접근 규칙으로 지원한다는 별도 계약이 필요하다.
- SideScanSonar에서 명시적으로 TBD로 남아 있고 CentralControl 등 다수 장치가 동일 표기를 사용한다.
- 상태: **OPEN / Adapter contract**.

### USV-COMMON-09 — PBIT/IBIT 외 비동기 Result/ACK correlation

- 적용 예: AutonomousNavigation Record echo, PlatformController ENV/Towing ACK 표기, RemoteFireControl CIPE/EOIR/Distance/Menu 결과, SideScanSonar Launch/BackStop/CommunicationLevel 및 `LastReceivedCommandID` 계승 메시지.
- 물리 메시지에는 처리/결과 관계가 있으나 동일 `commandID` 또는 명확한 transaction key가 없는 구간이 존재한다.
- PBIT/IBIT 전용 문제인 COMMON-01과 분리하여 일반 비동기 Control↔Result correlation 규칙으로 관리한다.
- 상태: **OPEN / runtime transaction model**.

### USV-COMMON-10 — RTP endpoint / 물리 채널 배치값

- 적용: ElectroOptical, NavigationCamera, NearContactDetection.
- 스트림 존재와 논리 Product는 확인되지만 일부 IP/Port가 `todo`/미기재이거나 MWIR/SWIR의 동일 물리 채널 공유 여부가 정의되지 않았다.
- Semantic/Binding은 존재하는 Product/RTPChannel을 유지하고 배치값은 임의 생성하지 않는다.
- 상태: **OPEN / ICD & deployment configuration**.

### USV-COMMON-11 — CSCI 물리 오탈자와 실제 DDS IDL 철자 확인

- 적용 예: CentralControl `operationalStaus`, RemoteFireControl `cipeAming`/`ControlPanalConfigType`, SideScanSonar `LanchAndRecoveryBackStopReportType`/`ScreanChangeConfigType`.
- Binding은 현재 원 CSCI 물리 이름을 보존한다.
- 실제 IDL에서 오탈자가 동일하게 존재하는지 확인될 때까지 이름을 정규화하지 않는다.
- 상태: **OPEN / IDL verification**.

### USV-COMMON-12 — 수치 sentinel/invalid 결과의 Semantic 노출 정책

- 적용: ElectroOptical LRF `statusDistance`, RemoteFireControl `targetDistance` 등.
- 유효 수치 외 `Unknown`/`Fail`/미정의 raw 영역이 있으나, 이를 별도 논리 상태로 노출할지 수치 Result에서 제외할지 공통 계약이 완전히 정리되지 않았다.
- 원문에 없는 raw 의미는 만들지 않으며 현재는 유효 수치 계약을 우선 유지한다.
- 상태: **OPEN / common result modeling**.

### 공통 이슈 요약

- Active common master issue: **10개** (`01`, `03`, `05`~`12`)
- Resolved master issue: **2개** (`02`, `04`)
- XML 설계 merge blocker: **0개**
- 실제 runtime 통합 전에 우선 확정할 항목: **01, 03, 08, 09**

---

## 4. 장치 고유 OpenIssue 최종 취합

아래 목록은 위 공통 ID로 승격한 중복 원인을 제외한 장치 고유 항목이다. 상세 원문 근거는 각 장치의 `*_OpenIssues.md`를 우선한다.

### 4.1 AutonomousNavigation

공통 참조: `USV-COMMON-01`, `USV-COMMON-09`.

장치 고유 TBD:
1. RecordCommand Size 열과 공용 구조체 실제 필드/sequence 불일치.
2. GlobalHover/TargetTracking Achieved 3개 octet raw code 의미.
3. `timeHoverCompleted` 의미 충돌 및 GlobalHover UInt64 시간 인코딩.
4. `distancePointRemaining`, `errorYawAngle`, `distanceTargetRemaining` Unit.
5. AutonomousDocking `distanceRemaining` Unit.
6. Fuel quantity Unit과 `alertTime` UInt64 packing/epoch.
7. Underwater `timeLimit` 실제 Unit.
8. TargetTracking range `1~49` 의미.
9. `targetTrackingID 0xX.Y.*`의 X/Y 식별 체계.
10. RestrictArea 두 점으로 형상을 구성하는 규칙.
11. EmergencyReturn `action=3` 의미.
12. aided record의 `endTime/arrivalTime` 등 공통 UInt64 시간 convention.

### 4.2 CentralControl

공통 참조: `USV-COMMON-01`, `USV-COMMON-11`.

장치 고유 TBD:
1. AuthorityControl `destination` 값 결정 및 비사용 `commandTarget` 초기화 규칙.
2. VHF `rxChannel` 범위/주파수/채널표.
3. `MCEPBITReportType`의 `1 Hz`와 비주기 PBIT 요청 관계.
4. `voiceRxControl`의 `음성수신명령` vs raw 1=`송신` 설명 충돌.
5. `operationalStateUSV` raw 2~7 의미.

### 4.3 ElectroOptical

공통 참조: `USV-COMMON-01`, `USV-COMMON-10`, `USV-COMMON-12`.

장치 고유 TBD:
1. Zoom/Focus `0x5555=No change`와 별도 No-change Control의 우선 적용 조건.
2. Swing 속도 필드의 Unit `deg` vs 의미상 각속도 충돌.
3. SWIR tracking sensor code 부재.
4. `EOIRPowerControlType`의 SWIR 전원 필드 부재.
5. Wiper Range 0~9 중 정의되지 않은 6~9 의미.
6. IBIT detail bit의 0/1 polarity.

### 4.4 ElectroOpticalProcessor

공통 참조만 남음: `USV-COMMON-01`, `03`, `05`, `06`, `07`.

장치 고유 미해결 master issue는 현재 없음.

### 4.5 IntegratedNavigation

공통 참조: `USV-COMMON-01`.

장치 고유 TBD:
1. `integratedNavigationAidedSensor` 0/1/2/4/8/16의 조합값 허용 여부.
2. `ajAsStatus` 정상 상태 및 항재밍+항기만 동시감지 표현.
3. IMU accelerometer temperature 8-bit signedness/유효범위.
4. IDC temperature/CPU Load의 Range 셀 `0` 의미.
5. RTCM `vaildRtcmDataSize` 원문 대범위와 256-byte buffer의 충돌.

### 4.6 Lidar

공통 참조만 남음: `USV-COMMON-01`, `03`, `05`, `07`.

장치 고유 미해결 master issue는 현재 없음.

### 4.7 NavigationCamera

공통 참조: `USV-COMMON-01`, `05`, `06`, `10`.

장치 고유 TBD:
1. IBIT detail 8개 octet의 0/1 polarity.
2. EO2~8 / IR2~8 상세 bit 의미.

### 4.8 NavigationRadar

공통 참조: `USV-COMMON-01`, `03`, `05`, `07`.

장치 고유 TBD:
1. Radar/AIS 탐지거리 제어 octet의 실제 거리/프리셋 값 체계.
2. `radarMessageTransmitStatus`가 가리키는 실제 메시지/업무 의미.

재심층 감사에서 `AISContactType`의 밀린 Range 셀은 동일 `공용 구조체.csv`의 `RadarAISFusionContactType` 동일 필드 교차근거로 해소하였다. 복원 근거가 없는 `shipLength/shipWidth`는 Range 미지정으로 유지한다.

### 4.9 NearContactDetection

공통 참조: `USV-COMMON-01`, `03`, `10`.

장치 고유 TBD:
1. Lidar `Level` / `SensorAction` 다중 bit 동시 설정 가능 여부와 우선순위.

### 4.10 NetworkAbstraction

공통 참조: `USV-COMMON-01`, `03`.

장치 고유 TBD:
1. `commsChannelID=2` 의미.
2. `IntraNetworkStatusType.commandID` 생산주체/업무 의미.
3. `configureLBandComms` packed control octet의 세부 polarity/표시명.
4. GeoSat `valid=false`일 때 하위 수치 표시/신뢰 처리 규칙.

L-Band 통제소/중계소 그룹 결과 식별과 공용 Result source routing은 `USV-COMMON-03`으로 통합 관리한다.

### 4.11 PlatformController

공통 참조: `USV-COMMON-01`, `03`, `09`.

장치 고유 TBD:
1. `PowerControlType` 64-bit 제어 mask의 장비별 bit mapping.
2. Power reset 시 `stateControl` 동시 값 의미.
3. `CamPowerControlType.cctvMask` relay별 bit 정의.
4. `ENVCommandType.seaWaterAirConditionerControl` 논리값 의미.
5. 직접 의미가 없는 CBIT/Environment 고유 bit/raw 상태.
6. `USVControlType` 운용모드별 유효 parameter 조건.

### 4.12 RemoteFireControl

공통 참조: `USV-COMMON-01`, `09`, `11`, `12`.

장치 고유 TBD:
1. joystick raw 0~65535 ↔ -20~+20 deg 변환식/중립값.
2. IBIT detail bit polarity 및 야간 광각/협각 symbolic name-한글 설명 충돌.
3. no-driving zone의 `-30=미설정` 규칙을 zone2~12에도 적용할지 여부.
4. `initZeroing` 송신 후 one-shot 자동 reset 정책.
5. Menu `SystemAction.Reboot`와 공용 `SystemRebootControlType`의 사용 우선순위.

### 4.13 SensorFusion

공통 참조: `USV-COMMON-01`, `03`, `05`, `06`, `07`.

장치 고유 TBD:
1. `WaveType.waveSpectrum`, `integratedWaveEnergy` Unit/Range.

### 4.14 SideScanSonar

공통 참조: `USV-COMMON-01`, `08`, `09`, `11`.

장치 고유 TBD:
1. `DetailSystemStatusType` 전원상태 5개 0/1 polarity.
2. 원문 code/polarity가 없는 일부 PBIT/IBIT detail 및 `sonarImagingProcessingEquipmentDetail` 장착유무 bit 해석.
3. 장치 폴더의 오래된 로컬 XSD snapshot 삭제 여부와 외부 의존성.
4. 선체부착형 SSS frequency 허용값 및 StatusConfig frequency 조건부 제약.
5. GapFiller range가 SSS range와 동기화되는 조건을 평면 Parameter 모델에서 표현하는 방법.
6. SoftwareGain Mode(0~2), LowPassFilter Mode(0~1)의 논리값 의미.

기존 문서의 "최종 CDM 감사" 항목은 `USV_Semantic_Binding_Integrated_Audit_20260828.md`가 완료/supersede하며 active issue로 세지 않는다.

---

## 5. 최종 merge / runtime readiness 판단

### Semantic / Binding 설계

- **Review/Merge Ready**.
- 28개 XML XSD PASS, 참조 오류 0, converter 0, source-confirmed primitive type 누락 0이다.
- 현재 OpenIssue는 원문에 없는 값을 임의 보완하지 않았기 때문에 남아 있는 것이며 XML 구조 오류가 아니다.

### Runtime/Adapter 통합 전 필수 확인

최소 다음 공통 이슈는 실제 연동 시험 전에 계약이 필요하다.

1. `USV-COMMON-01` PBIT/IBIT correlation.
2. `USV-COMMON-03` source routing/fan-out 및 multi-result 완료 판정.
3. `USV-COMMON-08` dotted-path nested member 접근.
4. `USV-COMMON-09` 일반 비동기 Result/ACK transaction correlation.

### 배치/ICD 확정 시 확인

- `USV-COMMON-10` RTP endpoint/channel.
- `USV-COMMON-11` 실제 DDS IDL 철자.

### 공통 XSD/모델 개선 후보

- `USV-COMMON-05` count/sequence validation contract.
- `USV-COMMON-06` symbolic Range expression.
- `USV-COMMON-12` sentinel/invalid result exposure.

---

## 6. 최종 결론

USV 14개 장치의 Semantic / Binding 설계 및 CSV 기반 source audit는 완료 상태다. 남은 문제는 크게 다음 세 종류로 한정된다.

1. **runtime correlation/routing** — 요청과 비동기 결과 연결, shared Topic source discrimination, multi-result completion.
2. **원문/IDL 의미 부족** — raw code, unit, bit polarity, typo의 실제 IDL 철자.
3. **배치/공통 스키마 보완** — RTP endpoint, count/sequence validation, symbolic Range, sentinel 표현.

따라서 현재 브랜치는 Semantic/Binding 설계 자체로는 merge 가능한 상태이며, 본 문서의 공통 master ID와 장치 고유 TBD를 이후 ICD/IDL/Adapter 확인 시 순차적으로 닫는다.
