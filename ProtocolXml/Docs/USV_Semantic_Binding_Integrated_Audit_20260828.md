# USV Semantic / Binding 최종 통합 감사

- 작성일: 2026-08-28
- 대상 저장소: `pepsimanpa/mcmc2`
- 대상 브랜치: `feature/usv-semantic-binding-audit`
- 대상 범위: 원격통제장치(RCU)와 직접 연동하는 USV 장치 14종의 Semantic / Binding
- 공통 설계 기준: `ProtocolXml/Docs/UMS_Semantic_Binding_Design_Rules_20260826.md`

---

## 1. 목적

장치별 Source Audit 완료 후 14개 USV Semantic / Binding을 하나의 시스템 관점에서 다시 비교하여 다음 항목의 일관성을 최종 점검하였다.

1. 동일 의미의 CDM 명명 일관성
2. 공통 Control인 연동개시 / 시스템재시작 / PBIT / IBIT의 동일 개념 적용 여부
3. `CommandStatusReportType` 처리 ACK 계약의 일관성
4. PBIT / IBIT 처리 ACK와 실제 점검 결과 Reply의 분리 여부
5. 공통 Control의 Target / 물리 Destination 표현 일관성
6. Semantic `ValueSet*`과 Binding `ValueMap`의 대응 여부
7. `PackedField / BitMember`의 구조 정합성
8. 장치별 OpenIssue 중 USV 공통 이슈의 중복 여부
9. 원문 근거가 부족한 primitive `dataType` 잔여 현황

본 감사에서도 기존 원칙대로 **원 CSCI / ICD / 공용 구조체 / 공용 식별자 규칙에 없는 값을 통합 일관성만을 이유로 새로 추정하지 않는다.**

---

## 2. 대상 장치

| No. | 장치 | 폴더 |
|---:|---|---|
| 1 | 자율운항장치 | `AutonomousNavigation` |
| 2 | 중앙통제장치 | `CentralControl` |
| 3 | 전자광학장치 | `ElectroOptical` |
| 4 | 전자광학장치 영상처리 CSC | `ElectroOpticalProcessor` |
| 5 | 복합항법장치 | `IntegratedNavigation` |
| 6 | 라이다 정보처리 | `Lidar` |
| 7 | 운항용카메라장치 | `NavigationCamera` |
| 8 | 항해레이더 정보처리 | `NavigationRadar` |
| 9 | 근거리접촉물탐지장치 | `NearContactDetection` |
| 10 | 네트워크추상화장치 | `NetworkAbstraction` |
| 11 | 선체제어장치 | `PlatformController` |
| 12 | 원격사격통제장치 | `RemoteFireControl` |
| 13 | 센서융합 정보처리 | `SensorFusion` |
| 14 | 예인형수중탐색장치 | `SideScanSonar` |

---

## 3. 최종 결론

### 3.1 공통 Control은 동일 개념으로 통일

원격통제장치가 각 장치에 보내는 아래 4개 공통 행위는 장치 고유 기능이 아니라 **동일한 시스템 공통 개념**으로 최종 통일하였다.

| 공통 행위 | Semantic CDM | 물리 DDS Topic / Type | 공통 의미 |
|---|---|---|---|
| 연동개시 | `Control.Integration.Start` | `RUSV::C2::RCU::IntegrationControlType` / `IntegrationControlType` | RCU와 대상 장치 간 연동 개시 |
| 시스템재시작 | `Control.System.Restart` | `RUSV::C2::RCU::SystemRebootControlType` / `SystemRebootControlType` | 대상 장치 시스템 재시작 |
| PBIT 요청 | `Request.Health.PBIT` | `RUSV::SO::RCU::RCUPBITControlType` / `RCUPBITControlType` | 대상 장치 PBIT 수행 요청 |
| IBIT 요청 | `Request.Health.IBIT` | `RUSV::SO::RCU::RCUIBITControlType` / `RCUIBITControlType` | 대상 장치 IBIT 수행 요청 |

표준 장치의 Semantic Target은 모두 다음 공통 CDM을 사용한다.

```text
Platform.Identifier.Numeric
```

대상 장치의 실제 물리 식별자 조립은 Semantic 입력이 아니라 OM / Adapter가 준비하며 Binding은 다음 형태를 사용한다.

```text
System.Target.<Device>
```

따라서 **행위 자체는 동일 CDM을 재사용하고, 어느 장치에 명령하는지는 Target / Destination에서 구분한다.**

### 3.2 공통 commandID 처리도 동일

공통 Control의 `commandID`는 장치별 HMI 입력값이 아니라 시스템이 생성하는 상관관계 식별자이며 Binding에서는 공통적으로 다음 방식으로 선언한다.

```text
sourceField = System.Generated.CommandSequence
wire type   = UInt16
```

`USVMessageBase` 역시 장치 Semantic에서 사용자가 구성하지 않고 OM / Adapter가 준비한다.

---

## 4. CommandStatus ACK 공통 계약

`CommandStatusReportType`은 장치별로 다른 의미의 결과 메시지가 아니라 **명령 접수/처리 상태 ACK라는 동일 개념**으로 통일하였다.

### 4.1 Semantic

모든 `CommandStatusReportType` Reply는 다음 공통 CDM을 사용한다.

```text
Command.ProcessingStatus.Response
```

Semantic Result는 다음 상태 결과를 공통적으로 노출한다.

```text
Control.Response.Status
```

그리고 처리 ACK는 `required="true"`로 맞추었다.

### 4.2 Binding

물리 Reply에서는 다음 필드를 보존한다.

- `usvHeader`
- `commandStatusReport.dstEquipmentType`
- `commandStatusReport.dstEquipmentID`
- `commandStatusReport.status`
- `commandID`

여기서 Destination과 commandID는 라우팅 / correlation용 물리 메타데이터이고, Semantic 결과값으로는 `commandStatusReport.status`만 노출한다.

### 4.3 의미상 주의사항

`CommandStatusReportType`은 **물리 장비의 실제 동작 완료 보고가 아니다.**

즉 아래처럼 해석하지 않는다.

```text
CommandStatus ACK 수신 = 실제 장비 동작 완료
```

처리 ACK와 장비 고유 상태 / 결과 Report는 별도 계약이다.

---

## 5. PBIT / IBIT 공통 구조

PBIT / IBIT은 전 장치에서 다음 2단계 구조로 정리한다.

### 5.1 처리 ACK

```text
PBIT/IBIT Control
    -> CommandStatusReportType
       cdm = Command.ProcessingStatus.Response
```

### 5.2 실제 점검 결과

```text
PBIT Result Reply
    cdm = Request.Health.PBIT.Response

IBIT Result Reply
    cdm = Request.Health.IBIT.Response
```

장치별 실제 결과 메시지 Type과 내부 결과 필드는 다르므로 Binding과 하위 Result CDM에서 장치 고유성을 유지한다.

예를 들어 선체제어장치는 하나의 IBIT 요청에 대해 Engine / WaterJet / BowThruster / Power / Battery 등 여러 물리 결과 메시지를 갖지만, **각 결과 메시지 자체가 별도 물리 Report이기 때문에 개별 Reply를 유지하는 것이 올바르다.**

근거리접촉물탐지장치도 SCDE 결과와 내부 Lidar 결과가 별도 물리 Report로 존재하므로 각각 Reply를 유지한다.

---

## 6. NetworkAbstraction 예외

네트워크추상화장치는 원 CSCI 자체가 ControlStation / USV / L-Band / GeoSat / Aux 등 여러 대상 장비를 분리한다.

따라서 다른 장치처럼 하나의 `startIntegration`, `requestPbit`, `requestIbit` ID로 합치지 않는다.

예:

- `startControlStationIntegration`
- `startUsvIntegration`
- `requestNetworkIntegrationControlStationPbit`
- `requestLBandTerminalPbit`
- `requestGeoSatUsvIbit`

그러나 **행위의 Semantic CDM은 공통 개념을 그대로 재사용한다.**

- Integration → `Control.Integration.Start`
- PBIT → `Request.Health.PBIT`
- IBIT → `Request.Health.IBIT`

Semantic Target도 장비 특성상 다음과 같이 구체적인 식별자 CDM을 유지한다.

- `Communication.NetworkAbstraction.Identifier.Numeric`
- `Communication.Equipment.Identifier.Numeric`

Binding Destination은 각 물리 대상에 따라 다음처럼 분리된다.

```text
System.Target.NetworkAbstraction.ControlStation
System.Target.NetworkAbstraction.USV
System.Target.NetworkAbstraction.LBandControlStationGroup
System.Target.NetworkAbstraction.LBandTerminal
System.Target.NetworkAbstraction.AuxControl
System.Target.NetworkAbstraction.GeoSatControlStation
System.Target.NetworkAbstraction.GeoSatUSV
```

이는 통합 규칙 위반이 아니라 **원 CSCI의 실제 다중 대상 구조를 보존한 의도적 예외**이다.

또한 NetworkAbstraction에는 표준 장치의 `restartSystem`에 대응하는 직접 RCU 메시지가 원문에서 확인되지 않아 임의 추가하지 않는다.

---

## 7. 공통 Health 상태 CDM 정규화

장치별 XML에 동일한 4상태가 다음 세 계열로 혼재되어 있었다.

- `Health.*`
- `Health.Status.*`
- `OperationalState.*`

다음 **정확한 4상태 집합**에 한해 공통 CDM으로 통일하였다.

| 논리 상태 | 최종 공통 CDM |
|---|---|
| 정상 | `Health.Normal` |
| 기능저하 | `Health.Degraded` |
| 비가용 | `Health.Unavailable` |
| 미응답 | `Health.NoResponse` |

다만 아래처럼 의미 체계 자체가 다른 상태는 공통 4상태로 강제 변경하지 않는다.

```text
Normal / Warning / Fault / Failure
```

예를 들어 선체제어 배터리 Fault 상태는 별도 상태 체계이므로 기존 의미를 보존한다.

---

## 8. 동일 ID라고 해서 무조건 동일 CDM으로 합치지 않음

통합 감사의 기준은 **문자열 ID 일치가 아니라 실제 의미 일치**이다.

대표 예시는 `emergencyStop`이다.

- CentralControl의 `emergencyStop`은 플랫폼 전체 긴급정지 성격의 `Control.Platform.EmergencyStop`
- SideScanSonar의 `emergencyStop`은 예인형수중탐색장치 자체의 `Control.EmergencyStop`

동일한 로컬 ID를 사용하지만 의미 범위가 다르므로 하나의 CDM으로 강제 통합하지 않는다.

---

## 9. Destination 최종 원칙

표준 공통 Control의 Semantic에서는 장치 식별을 다음 공통 Target 의미로 취급한다.

```text
Platform.Identifier.Numeric
```

물리 `DestinationType`은 OM / Adapter가 공용 식별자 규칙에 따라 조립하고 Binding은 완성된 값을 다음 방식으로 연결한다.

```text
DerivedField name="destination"
sourceField="System.Target.<Device>"
```

따라서 Semantic에 `dstEquipmentType`, `dstEquipmentID`, `dstSubEquipmentID`의 raw 숫자값을 노출하지 않는다.

NetworkAbstraction처럼 원 CSCI가 여러 세부 목적지를 직접 구분하는 경우에만 구체적인 target/destination 경로를 사용한다.

---

## 10. ValueSet / ValueMap 최종 감사

### 10.1 확정 결과

원문에서 raw 코드가 확인되는 논리 enum은 다음 원칙으로 정리하였다.

```text
Semantic ValueSet*
    = 논리 이름/CDM

Binding ValueMap
    = 실제 wire 숫자 코드
```

통합 감사 후 **Semantic과 Binding의 ValueMap CDM 불일치 0건**을 확인하였다.

PlatformController에서 기존에 Semantic ValueSet은 존재하지만 Binding ValueMap이 빠져 있던 CBIT 상태와 전원 상태 일부도, 과거 원본 XML에 실제 raw code 근거가 남아 있는 항목에 한해서 ValueMap을 보강하였다.

예:

- 4상태 CBIT: 0 / 1 / 2 / 3
- `Platform.Power.Source`: Battery=0, PDU=1
- `Platform.Power.PDU.Source`: Generator=0, Shore=1
- Battery Charge: Standby=0, Charging=1, Discharging=2
- Battery Level: 0~5
- Battery Fault: Normal=0, Warning=1, Fault=2, Failure=3

### 10.2 의도적으로 ValueMap을 만들지 않은 SSS 5개 상태

SideScanSonar의 다음 상태 필드는 Semantic 논리 상태가 존재하지만 현재 Binding ValueMap을 **의도적으로 확정하지 않는다.**

- `towedSonarArrayPowerStatus`
- `ultraShortBaseLinePowerStatus`
- `winchPowerStatus`
- `recoveryUnitPowerStatus`
- `cameraPowerStatus`

원 CSCI의 **제어값** `powerOn`은 `0=OFF / 1=ON`으로 확인되지만, `DetailSystemStatusType`의 **상태값** 5개가 동일 polarity라는 직접 근거가 없다.

따라서 “제어와 상태가 같을 것”이라는 추론으로 ValueMap을 넣지 않고 기존 OpenIssue를 유지한다.

최종 ValueMap 감사 상태는 다음과 같다.

- 확인 가능한 ValueMap의 Semantic 대응 불일치: **0건**
- 원문 근거 부족으로 의도적으로 미확정: **SSS 5건**

---

## 11. PackedField 최종 감사

14개 장치 Binding 전체의 `PackedField`는 총 **65개**이다.

검사 결과:

- BitMember overlap: **0건**
- PackedField width 범위 초과: **0건**

따라서 현재 선언된 bit 구조 자체의 기계적 정합성 문제는 없다.

원문에서 bit 의미가 불완전한 필드는 반복 구조를 추정하여 임의로 분해하지 않고 Raw 상태로 유지한다.

---

## 12. PBIT / IBIT Result correlation — USV 공통 이슈

최종 XML을 전수 분석한 결과, 실제 PBIT / IBIT 결과 Reply는 총 **61개**이다.

| 장치 | 실제 PBIT/IBIT Result Reply 수 | Result에 commandID 포함 |
|---|---:|---:|
| AutonomousNavigation | 2 | 0 |
| CentralControl | 2 | 0 |
| ElectroOptical | 2 | 0 |
| ElectroOpticalProcessor | 2 | 0 |
| IntegratedNavigation | 2 | 0 |
| Lidar | 2 | 0 |
| NavigationCamera | 2 | 0 |
| NavigationRadar | 2 | 0 |
| NearContactDetection | 4 | 0 |
| NetworkAbstraction | 14 | 0 |
| PlatformController | 21 | 0 |
| RemoteFireControl | 2 | 0 |
| SensorFusion | 2 | 0 |
| SideScanSonar | 2 | 0 |
| **합계** | **61** | **0** |

즉 이 문제는 특정 장치의 Binding 오류가 아니라 **USV 전체 공통 인터페이스 특성**이다.

공통 요청에는 `commandID`가 있으나 전용 결과 Report에는 이를 되돌려주는 필드가 없으므로, 다음의 런타임 규칙이 별도 정의되어야 한다.

- 요청 시점과 결과 Report의 temporal correlation
- 장치 / 세부 장치 source routing
- 동시에 여러 점검 요청이 존재할 때의 매칭 규칙

이를 아래 `USV-COMMON-01`로 통합 관리한다.

---

## 13. Primitive dataType 잔여 현황

원문 / IDL 근거가 직접 확인되는 primitive 필드는 Binding `dataType`을 선언하는 것이 원칙이다.

최종 감사에서 `usvHeader` / `destination` 및 `ProcessorIBIT` 같은 composite를 제외하고 남은 primitive `dataType` 미지정은 총 **0건**이다.

| 장치 | 미지정 수 | 사유 |
|---|---:|---|
| ElectroOptical | 0 | 실제 DDS boolean 8개를 `dataType="Boolean"`으로 해결 |
| ElectroOpticalProcessor | 0 | `controlImageSave`를 `dataType="Boolean"`으로 해결 |
| NavigationRadar | 0 | `항해레이더 정보처리 CSC.csv`에서 `frameWidth/frameHeight=long(4 Byte)` 확인 후 `Int32` 반영 |
| NetworkAbstraction | 0 | `공용 구조체.csv`에서 GeoSat 수치 8개 primitive type까지 확인 완료 |
| PlatformController | 0 | `선체제어장치 CSCI.csv` 전수 재확인으로 상태/PBIT/IBIT/Heartbeat primitive type 반영; 기존 Boolean 10개는 원문 octet에 따라 UInt8로 정정 |
| 기타 9개 장치 | 0 | 확인 가능한 primitive type 반영 완료 |
| **합계** | **0** |  |

현재 14개 USV 장치의 source-confirmed primitive field는 모두 `dataType`이 선언되었다. `usvHeader`, `destination`, `ProcessorIBIT` 등 복합 구조체는 primitive `WireDataType` 대상이 아니므로 예외로 유지한다.

DDS / IDL primitive `boolean`은 공통 `WireDataType.Boolean`으로 직접 표현한다. Semantic Boolean이 octet/packed bit/sentinel에서 파생되는 경우에는 실제 wire 타입을 유지한다.

---

## 14. 공통 OpenIssue 통합 ID

장치별 `*_OpenIssues.md`는 원문 근거와 장치 세부 맥락을 보존하기 위해 삭제하지 않는다.

다만 여러 장치에 반복되는 문제는 아래 공통 ID를 master tracking 항목으로 사용한다.

### USV-COMMON-01 — PBIT / IBIT 비동기 결과 correlation

- 적용 범위: 14개 전 장치
- 실제 결과 Reply: 61개
- 결과 Reply의 commandID: 0개
- 상태: 추가 Adapter / runtime correlation 규칙 필요

### USV-COMMON-02 — DDS Boolean wire type 표현 — **RESOLVED (2026-08-28)**

- 공통 `CommonBindingSchema.xsd/WireDataType`에 `Boolean` 추가
- 실제 DDS/IDL boolean 직접 필드 11개 반영: ElectroOptical 8, ElectroOpticalProcessor 1, NetworkAbstraction 2. PlatformController는 원 CSCI 재감사에서 직접 boolean 0개로 확인되어 기존 10개를 UInt8(octet)로 정정
- Semantic Boolean이라고 해서 자동으로 wire Boolean으로 변경하지 않는 규칙을 공통 설계문서에 명시
- AUV / MDV / EMDW 교차 감사 결과 직접 wire Boolean 후보는 0개였으며, 기존 Boolean 의미는 packed bit 또는 numeric sentinel/파생 의미이므로 실제 wire 타입을 유지

### USV-COMMON-03 — 공용 Result Topic source routing / fan-out

대표 적용 장치:

- ElectroOpticalProcessor
- Lidar
- NavigationRadar
- NearContactDetection
- NetworkAbstraction
- SensorFusion

같은 계열 Result Topic을 여러 처리카드 / 세부 장치가 공유하거나 하나의 요청에서 여러 결과가 fan-out되는 구조가 있으므로, `usvHeader` 및 장비 식별정보 기반 source discrimination 규칙을 Adapter 수준에서 명확히 해야 한다.

### USV-COMMON-04 — 원문 부족 primitive wire dataType — **RESOLVED (2026-08-28)**

- NavigationRadar 및 NetworkAbstraction의 잔여 type은 장치 CSCI/공용 구조체에서 확인하여 해결하였다.
- PlatformController의 기존 미지정 항목은 `선체제어장치 CSCI.csv`를 message/type별로 전수 재감사하여 primitive type을 확정하였다.
- 최종 primitive `dataType` 미지정: 0건. 복합 구조체에는 primitive type을 강제하지 않는다.

### 장치 고유로 유지하는 대표 이슈

다음은 공통 이슈로 올리지 않고 해당 장치 문서에 유지한다.

- SSS 전원 상태 5개 0/1 polarity
- Radar/AIS 탐지거리 octet 프리셋 규칙
- EOIR Zoom/Focus `0x5555` sentinel
- IntegratedNavigation 특정 BIT 예약비트/표기 오류
- AutonomousNavigation Achieved code 및 단위 미기재
- 각 장치 고유 range / unit / sentinel / 물리 Type 오탈자

---

## 15. 장치별 최종 구조 검증 결과

| 장치 | Control | Reply | Monitor | Product | XSD | ID 참조 | converter | Semantic raw enum |
|---|---:|---:|---:|---:|---|---|---:|---:|
| AutonomousNavigation | 17 | 26 | 10 | 0 | PASS | PASS | 0 | 0 |
| CentralControl | 24 | 26 | 3 | 0 | PASS | PASS | 0 | 0 |
| ElectroOptical | 20 | 22 | 4 | 2 | PASS | PASS | 0 | 0 |
| ElectroOpticalProcessor | 5 | 7 | 2 | 0 | PASS | PASS | 0 | 0 |
| IntegratedNavigation | 5 | 6 | 2 | 0 | PASS | PASS | 0 | 0 |
| Lidar | 4 | 6 | 2 | 0 | PASS | PASS | 0 | 0 |
| NavigationCamera | 5 | 6 | 2 | 1 | PASS | PASS | 0 | 0 |
| NavigationRadar | 12 | 14 | 6 | 1 | PASS | PASS | 0 | 0 |
| NearContactDetection | 4 | 8 | 2 | 1 | PASS | PASS | 0 | 0 |
| NetworkAbstraction | 19 | 32 | 9 | 0 | PASS | PASS | 0 | 0 |
| PlatformController | 20 | 37 | 14 | 0 | PASS | PASS | 0 | 0 |
| RemoteFireControl | 14 | 20 | 3 | 1 | PASS | PASS | 0 | 0 |
| SensorFusion | 4 | 6 | 3 | 0 | PASS | PASS | 0 | 0 |
| SideScanSonar | 38 | 47 | 8 | 5 | PASS | PASS | 0 | 0 |

`Control` 수는 `ControlSpecs` 하위의 `ControlSpec`, `SetPointSpec`, `TriggerSpec` 등 실제 Binding 대상 제어 subtype 전체를 포함한 수치이다.

---

## 16. 최종 통합 검증 체크리스트

| 항목 | 결과 |
|---|---|
| 28개 Semantic / Binding XML XSD 검증 | PASS |
| Control ↔ ControlBinding ID | PASS |
| Reply bindRef ↔ Binding Reply | PASS |
| Monitor ↔ MonitorBinding ID | PASS |
| Product ↔ ProductBinding ID | PASS |
| converter 잔존 | 0 |
| Semantic 논리 ValueSet raw numeric enum | 0 |
| generic 4-state Health CDM 분산 | 정규화 완료 |
| 확인 가능한 ValueSet ↔ ValueMap 불일치 | 0 |
| 원문 근거 부족 intentional no-map | SSS 5건 |
| PackedField overlap | 0 |
| PackedField width 초과 | 0 |
| 공통 Control Semantic Target | 표준 장치 통일 완료 |
| PBIT/IBIT 실제 Result의 commandID | 61개 중 0개 — `USV-COMMON-01` |
| 기존 CSCI 근거 주석 보존 | PASS |

---

## 17. 이번 통합 감사에서 해소된 기존 후속항목

장치별 초기 감사에서 “USV 전체 장치 완료 후 실시”로 미뤘던 다음 항목은 본 문서 작성으로 해소된 것으로 본다.

- 공통 Control CDM 명명 감사
- 공통 Reply / ACK 의미 감사
- 공통 Destination / Target 표현 감사
- generic Health 4상태 CDM 일관성 감사
- ValueSet / ValueMap 전수 대응 감사
- PackedField 구조 전수 감사
- 중복 OpenIssue 공통 분류

장치별 OpenIssues 문서의 과거 “최종 CDM 감사 예정” 문구는 **본 통합 감사 문서가 supersede**한다.

---

## 18. 현재 사용자 결정 필요사항

**현재 사용자 정책 결정이 필요한 항목은 없다.**

남은 공통 / 장치별 TBD는 다음 중 하나의 추가 근거가 필요한 사항이다.

- 원 CSCI / ICD
- DDS IDL
- 공용 XSD 개선
- OM / Adapter runtime routing / correlation 규칙
- 원작자 확인

따라서 근거가 확보되기 전에는 통합 일관성을 위해 값을 임의로 만드는 방식으로 닫지 않는다.

---

## 19. 최종 설계 해석

USV 14개 장치의 공통 메시지는 최종적으로 다음과 같이 해석한다.

```text
[Semantic]
같은 행위 = 같은 CDM
  - Integration
  - Restart
  - PBIT
  - IBIT
  - Command processing ACK
  - PBIT Result
  - IBIT Result

장치 차이 = Target / 하위 Result 의미

[Binding]
같은 공통 Control = 같은 DDS Topic / Type
장치 차이 = System.Target.* Destination
실제 enum / bit / scale = Binding에서 선언

[Runtime]
USVMessageBase / Destination / commandID = OM / Adapter 생성
CommandStatusReport = 처리 ACK
실제 PBIT/IBIT 결과 = 별도 Report
결과 Report의 commandID 부재 = USV-COMMON-01로 관리
```

이 기준을 이후 USV XML 변경 시 공통 회귀검증 기준으로 사용한다.
