# CentralControl Semantic/Binding 미확정 사항

## 1. 작업 범위

- 대상: **원격통제장치(RCU) ↔ 중앙통제장치(MCE)**
- 기준 입력: `원격통제장치 CSCI.csv`, `중앙통제장치 CSCI.csv`
- 산출물: `CentralControlSemantic.xml`, `CentralControlBinding.xml`
- 중앙통제가 자율운항/선체제어/네트워크/영상장치로 다시 송신하는 내부 DDS는 이번 Binding 범위에서 제외한다.
- 원통→중통 5 Hz `RCUHeartBeatReportType`은 원격통제장치 생존정보이며 중앙통제의 운용자 기능이 아니므로 Semantic/Binding 논리 뷰에서 제외한다.

## 2. 확정된 설계 결정

| 항목 | 결정 | 근거 유형 |
|---|---|---|
| 운용모드 | ES/ER/RC/LC/AC/N/MT/EA 8개를 독립 Semantic Control로 노출 | User-Decided + ICD-Explicit |
| EA 비상조치 | `executeEmergencyAction`으로 노출 | User-Decided |
| 통제권 | command 0~4를 5개 Semantic Control로 분리 | User-Decided + 공용 구조체 의미 |
| 통제권 전환대기 | `commandTarget`만 운용자 Parameter로 노출 | Derived |
| PBIT/IBIT | 요청 + 전용 결과 Reply로 표현 | Derived + ICD-Explicit |
| CBIT | 1 Hz Monitor | ICD-Explicit |
| USV HeartBeat | 5 Hz 플랫폼 종합상태 Monitor | ICD-Explicit |
| 통제권 소유정보 | 1 Hz Monitor | ICD-Explicit |
| 비주기 ACK | `CommandStatusReportType`을 명령 처리상태로 표현. 실제 동작 완료로 해석하지 않음 | ICD-Explicit |
| 볼륨/스퀄치 | selector 값 1/2를 `setVoiceVolume`, `setVhfSquelch`로 분리 | Design-Choice |
| 영상분배 | `videoTransmitType=0` 중지와 `1~4` 영상선택을 `stopVideoDistribution`, `selectVideoDistribution`으로 분리 | User-Decided + Design-Choice |
| 중앙통제 CBIT Monitor 명칭 | 물리 시험명(CBIT) 대신 운용자 관점의 `중앙통제장치 상태`로 표시 | User-Decided + Design-Choice |
| 중통 HeartBeat 내 타 장비 모드 | 중앙통제가 제공하는 USV 종합상태의 일부로 포함 | Design-Choice |

## 3. Open / TBD

### O-01. `AuthorityControl.destination`의 정확한 의미

- **상태:** Open
- **현상:** `AuthorityControl` 내부에 `command`, `commandTarget`, `destination : DestinationType`이 존재한다.
- **확정된 부분:** `commandTarget`은 command=0(통제권 전환 대기)에서 통제소 대상 선택에 사용한다.
- **미확정:** 내부 `destination`이 통제권 업무상 누구/무엇을 지칭하는지 명시 설명이 부족하다.
- **현재 처리:** Semantic/HMI 입력에서 제외하고 BindingOnly raw 필드로 보존.
- **추후 확인:** 공용 구조체 원본 설명, DDS IDL, 원통/중통 구현 코드 또는 통제권 시퀀스 확인.

### O-02. 일반 `DestinationType`의 중앙통제 대상 3-byte 식별값

- **상태:** Open
- **대상 메시지:** 연동 개시, 시스템 재시작, PBIT, IBIT.
- **현상:** 원통 CSV는 `destination : DestinationType`이 중앙통제를 선택한다는 의미만 제공한다.
- **현재 처리:** HMI 입력이 아닌 BindingOnly 필드로 보존.
- **추후 확인:** 식별자 규칙의 `dstEquipmentType/dstEquipmentID/dstSubEquipmentID` 조합과 중앙통제 제어카드/기록저장카드 대상 정책 확인 후 Fixed/Derived 규칙으로 변경.

### O-03. DDS 공용 struct 중첩 필드 접근 표기

- **상태:** Open
- **현상:** 현재 `CommonBindingSchema.xsd`에는 일반 struct를 계층적으로 기술하는 별도 Element가 없다.
- **영향:** `OperationModeControl.operationMode`, `AuthorityControl.command/commandTarget/destination`을 세부 매핑해야 한다.
- **현재 처리:** `operationModeControlStatus.operationMode`, `authorityControlStatus.command` 같은 **dotted path**를 잠정 사용.
- **추후 확인:** DDS Adapter/OperationManagement가 dotted path를 지원하는지 확인. 지원하지 않으면 Binding XSD/Adapter에 명시적 nested-struct 접근 규칙을 추가.

### O-04. Wireless Preset Bit 7~6 실제 wire encoding

- **상태:** Open
- **현상:** `topology`와 `mission`은 각각 octet인데 원문은 둘 다 `Bit 7~6`을 사용한다고 적는다.
- **의미:** topology `00=고송신, 01=저송신`; mission `00=탐지, 01=소나, 10=무장, 11=중계`.
- **미확정:** 실제 DDS 값이 논리값 0/1/2/3인지, `0x00/0x40/0x80/0xC0`처럼 shift된 값인지.
- **현재 처리:** Semantic에는 의미 Parameter만 정의하고 Binding에는 converter를 확정하지 않음.
- **추후 확인:** DDS IDL 생성 코드, producer 코드, 수신 측 bit masking, 패킷 캡처/시험값 확인.

### O-05. VHF `rxChannel` 범위/단위/채널표

- **상태:** Open
- **현상:** `rxChannel : long`, 설명은 `채널`, 비고는 `채널 및 주파수`뿐이다.
- **현재 처리:** Semantic Quantity Parameter는 정의하되 Unit/Range는 지정하지 않음.
- **추후 확인:** VHF 운용 채널표, 주파수 단위, 값이 채널 번호인지 주파수 수치인지 확인.

### O-06. 중앙통제 PBIT 응답의 `1 Hz` 표기

- **상태:** Open (문서 불일치)
- **현상:** `MCEPBITReportType`은 이름이 PBIT 응답인데 중앙통제 CSV에는 1 Hz로 표기된다.
- **비교:** 원통 PBIT 요청은 비주기이며, 다른 장치들의 PBIT 응답은 대부분 비주기 `X` 패턴이다.
- **현재 처리:** Semantic/Binding에서는 PBIT 요청에 대한 전용 결과 Reply로 해석하되, CSV의 1 Hz 표기를 주석으로 보존.
- **추후 확인:** 중앙통제 원본 IDL/설계서 또는 실행 주기 설정 확인.

### O-07. PBIT/IBIT 결과와 요청 `commandID` 상관관계

- **상태:** Open
- **현상:** 원통 PBIT/IBIT 요청에는 `commandID`가 있으나 `MCEPBITReportType`, `MCEIBITReportType`에는 `commandID`가 없다.
- **현재 처리:**
  - `CommandStatusReportType`은 commandID 기반 보조 ACK로 Binding에 유지.
  - PBIT/IBIT 전용 결과를 Semantic의 논리 Reply로 연결.
- **추후 확인:** 런타임에서 최근 요청 단일 outstanding 정책, Source/Topic 기반 매칭, 상태 머신 기반 매칭 등 실제 상관관계 규칙 확인.

### O-08. `MCEIBITReportType.operationalStaus` 오타 여부

- **상태:** Open (원문 필드명 검증)
- **현상:** 중앙통제 CSV에는 `operationalStaus`로 표기되어 있다.
- **현재 처리:** Binding에 CSV 표기를 그대로 사용.
- **추후 확인:** 실제 DDS IDL 필드명이 `operationalStaus`인지 `operationalStatus`인지 확인 후 필요 시 수정.

### O-09. `ProcessorIBIT` 16-byte 세부 구조 매핑

- **상태:** Open
- **현상:** 중앙통제 CSV에는 `mceProcessorIBIT : ProcessorIBIT`만 있고 내부 필드가 없다.
- **현재 처리:** IBIT Reply에서 raw BindingOnly 필드로 보존.
- **추후 확인:** 공용 구조체/IDL의 `ProcessorIBIT` 필드 정의를 확보한 뒤 의미가 명확한 항목만 CDM으로 승격.

### O-10. `VoiceRxTxControlType.voiceRxControl` 비고 불일치

- **상태:** Open (문서 오기 가능성)
- **현상:** 필드 설명은 `음성수신명령`인데 비고는 `0: 중지, 1: 송신`으로 적혀 있다.
- **현재 처리:** Semantic은 `Communication.Voice.Receive` 활성/중지 의미로 표현하며 `1=송신`이라는 문구 자체는 확정 의미로 사용하지 않음.
- **추후 확인:** 수신측 코드/IDL 주석/운용 시퀀스로 1의 의미 확인.

### O-11. `ControlCommandReportType` 원본의 `#REF!`

- **상태:** Source defect
- **현상:** 중앙통제 CSV의 `controlCommand` 행 주변에 `#REF!` 셀이 존재한다.
- **현재 처리:** Topic/Type과 `controlCommand` 필드 의미는 식별 가능하므로 Monitor 매핑은 유지.
- **추후 확인:** 원본 Excel 수식/참조 복구 후 CSV 재생성 권장.

### O-12. 운용모드 RCU Topic의 다중 Destination

- **상태:** Confirmed physical / intent review optional
- **현상:** `RUSV::C2::RCU::OperationModeControlType`은 중앙통제와 선체제어가 함께 수신한다. 중앙통제도 다시 자율운항/선체제어에 `MCE::OperationModeControlType`을 송신한다.
- **현재 처리:** 이번 Binding은 **원통↔중통 경계**만 표현하며, 병렬 수신 및 중통 downstream fan-out은 별도 내부 연동으로 본다.
- **추후 확인:** 선체제어 중복 수신의 설계 의도, ACK 주체/우선순위 확인이 필요할 경우 내부 시퀀스로 별도 정리.

### O-13. `CommandStatusReport`의 완료 의미

- **상태:** Confirmed limitation
- **공용 status:** Executing / Pending / Failed / Reject / Canceled.
- **주의:** Success/Completed 값이 없으므로 이 Reply를 `운용모드 전환 완료`, `통제권 전환 완료`로 표시하면 안 된다.
- **현재 처리:** Semantic Reply 명칭/의미를 `Command.ProcessingStatus.Response`으로 제한.
- **실제 상태 확인:** 운용모드는 `USVHeartBeatReportType.operationalModeUSV`, 통제권은 `ControlCommandReportType.controlCommand` Monitor를 사용.

### O-14. CDM 이름의 Registry 확정

- **상태:** Open / Design-Choice
- **현상:** 이번 XML의 `Platform.*`, `Control.Authority.*`, `Communication.*` 등은 기존 설계 규칙에 맞춘 프로젝트 Profile 후보이며 별도 승인된 CDM Registry와의 최종 대조는 아직 수행하지 않았다.
- **현재 처리:** ICD 물리명과 분리된 의미 키로 사용.
- **추후 확인:** 다른 USV 장치 Semantic까지 작성한 뒤 공통성 비교 후 CoreCandidate/Profile/Extension 분류를 정리.

## 4. 후속 작업 우선순위

1. **O-01 / O-02:** 공용 `AuthorityControl`, `DestinationType` 및 식별자 규칙 원본 확인.
2. **O-03:** OperationManagement/DDS Adapter의 nested struct 필드 접근 규칙 확인.
3. **O-04:** Preset Bit 7~6 실제 packing 코드 확인.
4. **O-05:** VHF 채널/주파수 정의 확보.
5. **O-06 / O-07:** PBIT/IBIT 응답 주기와 요청-결과 correlation 규칙 확인.
6. **O-08 / O-09 / O-10 / O-11:** IDL/원본 Excel로 문서 오기와 공용 구조체 세부 매핑 보정.
7. 다른 USV 내부 장치 Semantic 작성 후 **O-14 CDM 공통성 재검토**.

## 5. 변경 시 검증 체크리스트

- Semantic `ControlSpec/@id` ↔ Binding `ControlBindingDDS/@semantic_id` 일치
- Semantic `Reply/@bindRef`가 동일 Binding 파일의 `<Reply semantic_id>`에 존재
- Monitor `GroupSpec/@id` ↔ `MonitorBindingDDS/@semantic_id` 일치
- CSV의 Topic/Type/Field 원문 철자 보존 (`VHFchannelCommendType`, `operationalStaus` 등)
- `CommandStatusReport`를 실제 동작 완료로 승격하지 않음
- 미확정 bit/range/destination 값을 임의로 FixedValue/Range/converter로 확정하지 않음
- 원통→중통 범위 밖의 MCE downstream DDS를 `CentralControlBinding.xml`에 혼합하지 않음
