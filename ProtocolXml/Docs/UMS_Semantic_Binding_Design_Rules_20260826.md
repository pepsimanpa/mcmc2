# UMS Semantic / Binding / Specification 통합 설계 규칙

- 기준일: 2026-08-26
- Scope: MCMC2 전체 UMS(AUV / MDV / EMDW / USV 및 이후 추가 UMS)의 Semantic / Binding / Specification 설계
- Out of Scope: OM 실행 알고리즘, Validator 코드, 런타임 구현 및 실제 소프트웨어 구현 검증
- Status: Working baseline / common rule set
- 적용 우선순위: 새로운 직접 원문 근거가 없는 한 본 문서를 전체 UMS에 동일하게 적용한다.

본 문서는 기존 `SemanticBindingRules_20260818.md`와 `MdvEmdwDesignDecisions_20260825.md`를 통합하고, 2026-08-26까지 확정된 선언형 Binding 구조와 XML 작업 원칙을 반영한 단일 기준 문서다.

UMS 종류가 달라도 Semantic / Binding / Specification의 역할과 표현 규칙은 동일하다. 장치별 차이는 원 ICD가 정의한 실제 의미, 메시지 구조, 코드값 및 예외사항에 한정한다.

---

## 1. 근거 우선순위

1. 원 ICD / CSCI / 원작자 XML / XSD의 직접 명시
2. 본 문서에 기록된 확정 설계 규칙
3. 현재 Semantic / Binding / Specification 구현
4. 추론

과거 대화, 변환 CSV의 해석성 컬럼, 과거 임시 설계는 직접 원문과 동일한 권위를 갖지 않는다.

원문과 기존 XML이 충돌하면 원문을 우선하고, 이유를 본 문서에 기록한다.

---

## 2. 계층별 책임

### 2.1 Semantic

Semantic은 **전송매체와 무관한 원격 운용 의미 계약**이다.

Semantic에 정의한다.
- 운용 가능한 Control의 의미
- Control Target
- 사용자가 실제 입력하거나 선택하는 Parameter
- 단위 / 범위 / 해상도
- Reply 존재 여부와 논리적 결과
- Monitor의 의미
- 파일 / 영상 / 스트림 등 SensorProduct의 논리 의미
- 사용자가 바로 이해할 수 있는 상태 의미

Semantic에 정의하지 않는다.
- TCP / UDP / DDS / RS422 / RF 등 전송 방식
- wire 자료형
- endian
- scale에 따른 실제 정수 인코딩
- enum 숫자 code
- bit offset / mask / packed raw value
- Reserved / framing 값
- wire sentinel 값

예:
- Semantic: `이동 / 탐색 / 식별`
- Binding: `이동=0 / 탐색=1 / 식별=2`

운용관리 내부 상태나 시스템이 자동 생성하는 값은 HMI 입력 Parameter로 만들지 않는다.

### 2.2 OM / 실행 로직과의 책임 경계

OM은 **값 생성, 상태 판단, 업무 규칙 검증**을 담당한다.

예:
- 현재 시각 생성 및 Year / Month / Date / Hour / Minute / Second / Millisecond 값 준비
- Command UUID 생성
- Heartbeat Sequence 증가
- 임무계획 유효성 검사
- 마지막 Waypoint Action이 `대기`인지 검사
- 필요한 내부 상태 생성

Binding이 OM 실행 알고리즘을 대신하지 않는다.

본 설계의 범위는 **OM이 어떤 값을 준비해야 하는지, 어떤 운용 제약을 실행 계층에서 검증해야 하는지에 대한 책임 경계를 정의하는 것까지**다.
OM 내부 알고리즘, Validator 코드 작성, 구현 방식 및 실제 구현 여부 확인은 본 Semantic / Binding 설계 범위에 포함하지 않는다.

### 2.3 Binding

Binding은 **Semantic 또는 OM이 준비한 값을 실제 wire 구조에 어떻게 표현하는지** 선언한다.

Binding에서 정의한다.
- Protocol / Channel / Message / Topic / Type
- Field / FixedField / DerivedField / PackedField / BitMember / ArrayField
- 실제 wire `dataType`
- `scale`
- 배열 `length`
- 표현 `format`
- 의미 ↔ raw code의 `ValueMap`
- 특수 wire 값의 `Sentinel / Normal`
- bit packing
- 상수값 / Reserved zero-fill
- OM이 준비한 값의 `sourceField`
- Reply의 실제 Telegram과 Field / Bit 구조
- expectedValue / expectedMask 등 수신 식별 조건

Binding은 값을 생성하거나 증가시키지 않는다.

### 2.4 Specification

Specification은 Semantic과 Binding을 묶고 장치/시스템 수준의 공통 표현 규칙을 제공한다.

현재 MDV / EMDW 기준 byte order는 Specification에서 다음처럼 1회 선언한다.

```xml
<DataEncoding byteOrder="little"/>
```

Field마다 `little` / `big`을 반복 선언하지 않는다.

---

## 3. 선언형 Binding 원칙

### 3.1 Converter 제거 원칙

신규/리팩터링 Binding은 **converter 식별자에 의존하지 않는 선언형 구조**를 기본으로 한다.

기존 converter가 담당하던 역할은 다음처럼 표현한다.

| 기존 역할 | 선언형 Binding |
|---|---|
| UInt16LE / UInt32LE / Float32LE | `dataType` + 상위 `DataEncoding` |
| ×10 / ×10000000 | `scale` |
| UUID 16 bytes | `dataType="UInt8" length="16" format="uuid"` |
| enum code | `ValueMap` |
| 특수 raw sentinel | `Sentinel / Normal` |
| 반복 배열 | `ArrayField` / `length` |

공통 XSD에는 기존 AUV / USV 호환성을 위해 converter 속성이 임시로 남아 있을 수 있으나, MDV / EMDW Binding에서는 converter를 사용하지 않는다.

### 3.2 WireDataType / dataType

물리 자료형은 `dataType`으로 명시한다.

예:

```xml
<Field name="missionId"
       cdm="Mission.Identifier"
       dataType="UInt32"/>
```

사용 가능한 기본 WireDataType은 XSD 정의를 따른다.

### 3.3 scale

Semantic 실제값과 wire 정수 표현의 고정 배율을 선언한다.

예:

```xml
<Field name="speed"
       cdm="MissionPlan.Waypoint.Speed"
       dataType="UInt16"
       scale="10"/>
```

의미:
- Semantic → wire: `value × 10`
- wire → Semantic: `raw ÷ 10`

송수신 방향별 converter를 별도로 만들지 않는다.

### 3.4 length / format

고정 길이 배열은 `length`로 표현한다.

예:

```xml
<DerivedField name="commandId"
              cdm="Command.Identifier.UUID"
              sourceField="System.Generated.CommandIdentifier"
              dataType="UInt8"
              length="16"
              format="uuid"/>
```

`format`은 동일 wire 배열이 어떤 상위 형식인지 나타낼 때 사용한다.

### 3.5 ValueMap

Semantic 의미와 wire code의 대응 관계는 Binding에서 직접 선언한다.

예:

```xml
<Field name="missionType"
       cdm="Mission.Type"
       dataType="UInt8">
    <ValueMap>
        <Map cdm="Mission.Type.Move" value="0"/>
        <Map cdm="Mission.Type.Search" value="1"/>
        <Map cdm="Mission.Type.Identify" value="2"/>
    </ValueMap>
</Field>
```

Semantic에는 `0 / 1 / 2`를 넣지 않는다.

ValueMap 적용 대상 예:
- Mission Type
- Waypoint Action
- Emergency Stop Reason
- Reply 성공 / 실패
- Reply reasonCode
- Health / Link / Mission state 등 상태 enum

### 3.6 Sentinel / Normal

특정 raw 값이 일반 수치가 아닌 특수 의미를 갖는 경우 `Sentinel / Normal`로 표현한다.

MDV / EMDW `currentWaypointIndex`:

```xml
<Field name="currentWaypointIndex" dataType="UInt16">
    <Sentinel value="0xFFFF">
        <Set cdm="MissionPlan.CurrentWaypoint.Available" value="false"/>
    </Sentinel>
    <Normal>
        <Set cdm="MissionPlan.CurrentWaypoint.Available" value="true"/>
        <Bind cdm="MissionPlan.CurrentWaypointIndex"/>
    </Normal>
</Field>
```

- raw `0~65534` → Available=true, Index=raw
- raw `0xFFFF` → Available=false, Index는 의미값 없음

물리 Field를 두 번 선언하지 않는다.

---

## 4. 자동생성/파생값 규칙

Binding의 `DerivedField`는 **OM이 이미 준비한 source 값**을 wire Field에 연결하는 용도다.

예:

```xml
<DerivedField name="systemTimeYear"
              sourceField="System.Time.Year"
              dataType="UInt16"/>
```

```xml
<DerivedField name="heartbeatCount"
              cdm="Communication.Heartbeat.Sequence"
              sourceField="System.Generated.HeartbeatSequence"
              dataType="UInt32"/>
```

다음은 Binding에 두지 않는다.
- `generator="incrementing"`
- `sourceMember="year"`와 같은 객체 분해 로직
- UUID 생성 알고리즘
- 시간 생성 알고리즘

즉 OM이 `System.Time.Year`, `System.Generated.HeartbeatSequence` 등 최종 값을 준비한다.

---

## 5. Control / Reply / Monitor

- Control은 송신 방향 자체가 아니라 원격 실행 가능한 기능이다.
- Control 내부 Reply는 해당 Control에 대한 응답이다.
- Monitor는 주기적/지속적으로 관찰해야 하는 논리 대상이다.
- Control / Monitor를 송신 / 수신의 동의어로 사용하지 않는다.
- ACK와 실제 상태 결과가 별도 데이터라면 하나로 합치지 않는다.
- 명확한 원문 근거가 없는 Monitor를 임의 생성하지 않는다.

### Heartbeat 방향 모델링 TBD

`rov_common.csv`의 `T_HEARTBEAT`는 다음이 직접 확인됐다.
- 1초 주기
- 양방향 송수신
- `srcDevID / dstDevID / heartBeatCnt`
- 송신 시 `heartBeatCnt` 1씩 증가

물리 Heartbeat 양방향 전송은 확정이다.

그러나 **자체 송신 Heartbeat를 Semantic Monitor 대상으로 보는 것이 적절한지는 아직 TBD**다.
현재 MDV / EMDW의 `transmitHeartbeat / receiveHeartbeat` 구조는 당장 유지하고, Monitor 방향성 또는 통신관리 전용 모델 검토 후 결정한다.

---

## 6. Semantic 선택값 / Result 표현

Semantic은 논리 의미만 제공한다.

예:
- 정상 / 고장
- 연결 / 미연결
- 성공 / 실패
- 이동 / 탐색 / 식별

신규 구조:
- HMI 선택형 Parameter: `ValueSetProfile`
- Reply 논리 결과: `ValueSetResult`
- Monitor 상태: `ValueSetSpec`

`ValueSetProfile`, `ValueSetResult`, `ValueSetSpec`의 Semantic `<Value>`에는 raw 숫자 code를 기록하지 않는다.

실제 code 대응은 Binding `ValueMap`으로 이동한다.

Semantic Result와 Binding Field / BitMember는 **동일 CDM을 연결키**로 사용한다.

Reserved bit/field는 Semantic Result를 만들지 않는다.

---

## 7. CDM 공통 규칙

- 동일 의미가 다른 UMS에 이미 있으면 기존 공통 CDM을 우선 재사용한다.
- 의미가 다른 상태를 이름 유사성만으로 강제 통합하지 않는다.
- 장치 내부 고유 의미만 도메인 특화 CDM을 사용한다.
- 한 XML에서 문자열을 맞추기 위한 목적으로 신규 CDM을 만들지 않는다.
- CDM 정합성은 장치별이 아니라 전체 시스템 관점에서 검토한다.

예:
- MDV/EMDW `Warning / Abnormal`
- 다른 USV의 `Degraded / Unavailable / NoResponse`

직접 동일 의미라는 원문 근거가 없으면 같은 상태로 취급하지 않는다.

---

## 8. PackedField / BitMember

- byte order 적용 후 정규화된 정수의 LSB를 `offset=0`으로 한다.
- 원 표가 bit0 기준으로 직접 정의되어 있으면 MSB/LSB 표기만 보고 순서를 뒤집지 않는다.
- packed 구조는 converter에 숨기지 않고 `PackedField / BitMember`로 가시화한다.

Validator 확인 항목:
- `offset + width <= PackedField.width`
- BitMember overlap 없음
- expectedValue / expectedMask가 PackedField width를 초과하지 않음
- fixedValue가 BitMember width를 초과하지 않음

---

## 9. Reserved / 미사용 Field

고정 길이 Telegram의 미사용 INFORMATION / Reserved 영역은 원문 별도 지정이 없으면 **0으로 송신**한다.

송신 예:

```xml
<FixedField name="reserved"
            value="0"
            dataType="UInt8"
            length="5"/>
```

수신 예:

```xml
<Field name="reserved"
       dataType="UInt8"
       length="3"/>
```

Reserved도 wire 구조의 일부이므로 `dataType`과 배열 크기 `length`를 생략하지 않는다.

MDV / EMDW의 Field audit 기준:
- 모든 `Field`
- 모든 `FixedField`
- 모든 `DerivedField`

은 물리 wire Field라면 `dataType`을 가져야 한다.

---

## 10. 운용 제약과 실행 계층 책임 경계

XSD 또는 Binding이 표현하기 부적절한 상호조건은 Semantic / Binding에서 임의로 보정하거나 실행하지 않는다.
본 문서에는 **실행 계층에서 검증되어야 하는 운용 제약 자체만 설계 요구사항으로 기록**하며, 실제 Validator/OM 구현은 범위 밖으로 둔다.

대표 확정 사례:

### 마지막 Waypoint Action

MDV / EMDW 원 ICD에 **마지막 경로점은 대기(1)로 설정**이 직접 명시되어 있다.

Semantic은 Waypoint Action 의미를 `통과 / 대기`로 제공하고, Binding은 `0 / 1` ValueMap을 정의한다.

운용 제약은 다음과 같다.

- Waypoint 목록이 존재하는 경우 마지막 Waypoint Action == `대기`
- Binding은 이를 임의로 `대기`로 수정하거나 강제하지 않는다.
- 실제 실행 계층은 이 제약을 만족하지 않는 임무계획을 정상 전송 대상으로 취급해서는 안 된다.
- 단, 이를 검사하는 Validator/OM의 실제 코드와 구현 여부는 본 설계 범위에서 검토하지 않는다.

---

## 11. SensorProduct

파일 / 영상 / 스트림 등 대형 산출물을 일반 Control Reply에 억지로 포함하지 않는다.

- 요청 행위 → Control
- 실제 파일 / 프레임 / 스트림 → SensorProduct / ProductBinding

---

## 12. RF_CMD_COMPLETE

AUV RF ICD 기준:
- `RF_CMD_COMPLETE`는 RF통신장치에 보낸 명령이 완료되었음을 알리는 bit다.
- RF통신장치가 1로 설정해 휴대용콘솔에 전송하면 해당 RF 명령 완료를 확인한다.
- AUV 본체의 실제 임무 성공/완료와 동일한 의미가 아니다.

ACK / RF_CMD_COMPLETE / INFORMATION Body가 항상 동일 RF-2 Telegram에서 동시에 유효한지는 직접 근거 확인 전까지 TBD로 유지한다.

---

## 13. MDV / EMDW 확정 적용 사례

아래 항목은 MDV / EMDW 원 ICD (`rov_mdv.csv`, `rov_emdw.csv`, `rov_common.csv`)를 기준으로 확정됐다.

### Control 분리

- Mission Control: 정지 / 시작 / 일시정지 / 재개
- MDV 운용모드: 레거시 / 자율운항
- EMDW 운용모드: 관제터미널 / 통합통제
- Emergency Stop: 즉시정지 / 임무중지 후 대기
- EMDW Disposal: 취소 / 준비 / 실행

물리 enum을 하나의 거대한 Control로 두지 않고 운용 기능 단위로 분리한다.

### Control Reply

`T_MDV_CTRL_RESP`, `T_EMDW_CTRL_RESP`의 `result`:
- `0 = 성공`
- `1 = 실패`

Semantic에는 성공 / 실패만 제공하고 Binding ValueMap에서 숫자를 매핑한다.

MDV reasonCode:
- 0 없음
- 1 요청값 오류
- 2 임무계획 오류
- 3 모드전환 불가
- 4 모드 불일치
- 5 MDV 미연동
- 6 운용상태 비정상
- 7 기타 오류

EMDW는 추가로:
- 7 처리(Disposal)조건 불충족
- 8 기타 오류

Control Reply 성공은 전체 임무 수행 완료를 의미하지 않는다. 실제 진행/완료는 Mission Progress 상태로 확인한다.

### Mission Type

- 이동 → 0
- 탐색 → 1
- 식별 → 2

Semantic 의미 + Binding ValueMap 구조를 사용한다.

### Waypoint Action

- 통과 → 0
- 대기 → 1

마지막 Waypoint는 반드시 대기.

### Emergency Stop Reason

- 운용자 명령 → 0
- 체계 판단 → 1
- 통신 이상 → 2
- 장애 발생 → 3
- 기타 → 4

### Platform Status Reason

MDV / EMDW 공통 의미:
- 없음
- 콘솔 이상
- 플랫폼 미연동
- 플랫폼 이상
- 유도 이상
- 기타

### Current Waypoint

- 정상 index: 0~65534
- `0xFFFF`: 현재 추종 경유점 없음
- Semantic: `currentWaypointAvailable` + `currentWaypointIndex`
- Binding: `Sentinel / Normal`

### setTargetInformation

원 ICD에 별도 공통 제어응답이 없으므로 Reply를 임의 추가하지 않는다.

### Heartbeat

물리 양방향 / 1초 주기 / 증가 count는 확정.
Semantic 방향 모델은 TBD.

---

## 14. XML 수정 작업 원칙

이 항목은 이후 XML 자동 작업 시 반드시 적용한다.

### 14.1 주석/서식 보존 우선

기존 XML 주석은 단순 장식이 아니라 다음 정보를 포함할 수 있다.
- 원 ICD 해석 근거
- 설계 이유
- 특수값 설명
- 송수신 주의사항
- Adapter/OM 검증 조건
- TBD / 미확정 원문 사항

따라서 **Semantic / Binding / Specification XML 수정·리팩터링 시 기존의 유효한 주석은 원칙적으로 보존한다.**

주석 처리 기준:
- 현재 설계와 의미가 동일한 주석: 그대로 유지
- XML 구조만 변경된 주석: 삭제하지 않고 현재 구조(`dataType / scale / ValueMap / Sentinel` 등)에 맞춰 문구 갱신
- 원 ICD의 미확정 사항, 패킷 구조, 검증 조건, 설계 근거를 설명하는 주석: 반드시 유지
- 폐기된 설계(`converter`, `generator`, `sourceMember`, 과거 DerivedSemantic 방식 등)를 전제로 하는 주석: 현재 확정 구조에 맞게 수정하거나 더 이상 유효하지 않을 때만 제거
- 원문보다 강한 추론이나 폐기된 해석을 담은 주석: 그대로 복원하지 않음

주석 삭제는 기능 변경의 부수효과로 허용하지 않으며, 삭제가 필요한 경우 현재 규칙 또는 직접 원문과 충돌하는지 먼저 확인한다.

### 14.2 기본 작업 방식

**수정은 텍스트 기반 최소 패치, 검증은 XML Parser / XSD Validator 방식**을 기본으로 한다.

권장 흐름:

1. 기존 XML 원문 읽기
2. 정확한 block / attribute 단위 최소 치환
3. 기존 주석 / 들여쓰기 / 줄바꿈 유지
4. XML well-formed parse 검사
5. XSD 검사
6. Semantic ↔ Binding 정합성 검사
7. git diff로 의도하지 않은 대량 formatting 변경 확인

### 14.3 XML 전체 재직렬화 금지 원칙

`xml.etree.ElementTree` 등의 일반 파서로 XML 전체를 읽고 다시 저장하면 기본 설정에서 주석이 유실되거나 formatting이 크게 바뀔 수 있다.

따라서 단순 패치 작업에서는 전체 재직렬화를 피한다.

부득이하게 구조적 파싱/재저장이 필요할 경우:
- comment-preserving parser 사용
- 기존 주석 보존 여부 확인
- 저장 전후 comment count / diff 검사
- 대량 formatting diff가 생기면 적용하지 않음

예:

```python
parser = ET.XMLParser(
    target=ET.TreeBuilder(insert_comments=True)
)
```

다만 이 방식도 attribute/indentation formatting을 변경할 수 있으므로 **최소 텍스트 패치가 우선**이다.

### 14.4 MDV / EMDW 주석 복구 상태

2026-08-26 선언형 Binding 리팩터링 과정에서 `ElementTree` 전체 재직렬화로 MDV / EMDW XML의 기존 주석 일부가 유실되었으나, 이는 의도된 설계 변경이 아니므로 리팩터링 직전 XML을 기준으로 복구하였다.

**Status: RESOLVED / RESTORED**

복구 결과:
- `MdvPlatformSemantic.xml`: 2개
- `MdvTcpBinding.xml`: 75개
- `EmdwPlatformSemantic.xml`: 2개
- `EmdwTcpBinding.xml`: 82개
- 총 161개 주석 복구
- 복구 commit: `b65b55d` (`Restore valid MDV EMDW XML comments`)

복구 시 현재 `dataType / scale / ValueMap / Sentinel / converter=0` 구조는 유지하였다. 현재 설계와 충돌하는 과거 converter/DerivedSemantic 기반 표현이나 원문보다 강한 과거 해석은 그대로 되살리지 않고 현재 확정 규칙에 맞게 갱신하였다.

---

## 15. 변경 후 공통 검증 체크리스트

1. XML well-formed
2. XSD 검증
3. Semantic Control ID ↔ Binding `semantic_id`
4. Semantic Reply `bindRef` ↔ Binding Reply `semantic_id`
5. Parameter CDM ↔ Binding CDM/sourceField
6. Result CDM ↔ Binding Field/BitMember CDM
7. Semantic ValueSet에 raw 숫자 code가 남아 있지 않은지
8. Binding ValueMap이 원 ICD raw code와 일치하는지
9. 모든 물리 `Field / FixedField / DerivedField`의 `dataType`
10. 배열 크기 `length`
11. scale / format
12. Specification `DataEncoding`
13. converter 신규 사용 여부
14. PackedField width / overlap / mask
15. Telegram Field 순서 및 packetLength
16. Reserved / unused zero-fill
17. Sentinel / Normal 규칙
18. 반복/상호조건 Validator 규칙
19. 기존 공통 CDM 재사용 여부
20. TBD를 추정으로 확정하지 않았는지
21. 기존 XML 주석이 의도치 않게 삭제되지 않았는지
22. 불필요한 대량 formatting diff가 발생하지 않았는지

---

## 16. 현재 남은 주요 TBD / 후속작업

- Heartbeat 자체 송신을 Semantic Monitor로 유지할지 여부
- 전체 시스템 CDM audit
- AUV / USV를 동일 선언형 Binding 규칙으로 마이그레이션할지 단계별 검토
- 공통 XSD의 legacy `converter` 속성 최종 제거 시점 결정

본 문서는 위 항목이 확정될 때마다 갱신하며, 별도의 UMS별 규칙 문서를 새로 분리하지 않는 것을 기본 원칙으로 한다.
