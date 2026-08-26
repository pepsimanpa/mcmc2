# MDV / EMDW Semantic & Binding Design Decisions

- Date: 2026-08-25
- Last implementation update: 2026-08-26
- Branch: `feature/auv-reply-bit-binding`
- Sources: `rov_mdv.csv`, `rov_emdw.csv`, `rov_common.csv`
- Common rules: `ProtocolXml/Docs/SemanticBindingRules_20260818.md`
- Status: Source-backed implemented baseline

본 문서는 MDV/EMDW 원본 ICD를 재검토하여 확정한 Semantic/Binding 설계결정을 기록한다. 새로운 직접 원문 근거가 없는 한 동일 이슈를 반복 재해석하지 않는다.

## 1. Control 기능 분리

**Status: RESOLVED / KEEP**

원 ICD의 물리 enum을 운용 기능 단위 Control로 분리한 현재 구조를 유지한다.

MDV/EMDW `missionCtrlCmd`: 정지 / 시작 / 일시정지 / 재개

MDV 운용모드: 레거시모드 / 자율운항모드

EMDW 운용모드: 관제터미널모드 / 통합통제모드

비상정지 유형: 즉시정지 / 임무중지 후 대기

EMDW 처리 명령: 처리 취소 / 처리 준비 / 처리 실행

실제 wire enum code는 Binding에서 고정/변환한다.

## 2. 공통 제어응답 처리결과와 사유

**Status: RESOLVED / APPLIED**

`T_MDV_CTRL_RESP`, `T_EMDW_CTRL_RESP`의 `result`는 원 ICD 기준 성공/실패이다.

Semantic Reply는 처리결과와 사유를 논리 상태로 제공한다. 실제 `0=성공`, `1=실패` 및 reasonCode 숫자값은 Binding에서 변환한다.

기존 Binding 주석의 `result=0은 요청 검증 완료 및 명령 수락` 표현은 원문보다 강한 해석이므로 제거하였다.

MDV reasonCode 의미:
- 없음
- 요청값 오류
- 임무계획 오류
- 모드전환 불가
- 모드 불일치
- MDV 미연동
- 운용상태 비정상
- 기타 오류

EMDW는 위 의미에 다음을 추가한다.
- 처리(Disposal)조건 불충족

Control 응답 성공은 전체 임무 수행 완료와 동일하지 않다. 임무 진행/완료는 별도 Mission Progress 상태정보로 확인한다.

## 3. MissionPlan 선택형 Parameter

**Status: RESOLVED / APPLIED**

### Waypoint Action
Semantic 선택 의미: 통과 / 대기

wire code: `0=통과`, `1=대기`

Binding converter: `MissionWaypointActionToCode`

### Mission Type
Semantic 선택 의미: 이동 / 탐색 / 식별

wire code: `0=이동`, `1=탐색`, `2=식별`

Binding converter: `MissionTypeToCode`

## 4. 마지막 Waypoint 규칙

**Status: RESOLVED / VALIDATOR RULE**

원 ICD에 `마지막 경로점은 대기(1)로 설정`이 직접 명시되어 있다.

Waypoint 목록이 존재할 때 Validator/OM은 마지막 Waypoint Action이 대기인지 확인한다.

## 5. Emergency Stop Reason

**Status: RESOLVED / APPLIED**

Semantic 선택 의미와 wire code:
- `0` 운용자 명령
- `1` 체계 판단
- `2` 통신 이상
- `3` 장애 발생
- `4` 기타

Semantic에는 의미를 노출하고 실제 숫자값은 Binding의 `EmergencyStopReasonToCode`로 변환한다.

## 6. Platform Status Reason Code

**Status: RESOLVED**

MDV와 EMDW 원 ICD는 동일한 의미 구조를 사용한다.
- 없음
- 콘솔 이상
- 플랫폼 미연동
- 플랫폼 이상
- 유도 이상
- 기타

플랫폼 명칭만 MDV/EMDW로 달라지며 공통 `Platform.Status.Reason.*` 의미를 재사용한다.

## 7. Current Waypoint Index / Availability

**Status: RESOLVED / APPLIED**

정상 경유점 인덱스 범위:
- `0~65534`
- 0-based index

특수 wire 값:
- `0xFFFF` = 현재 추종 경유점 없음

Semantic은 물리 sentinel 값을 직접 노출하지 않고 다음 두 항목으로 표현한다.
- `currentWaypointAvailable` / `MissionPlan.CurrentWaypoint.Available`: 현재 추종 경유점 존재 여부
- `currentWaypointIndex` / `MissionPlan.CurrentWaypointIndex`: 현재 추종 경유점 인덱스 (`0~65534`)

Binding은 단일 물리 `currentWaypointIndex` Field를 중복 선언하지 않고 `DerivedSemantic`으로 두 의미를 파생한다.
- raw `0~65534` → `currentWaypointAvailable=true`, `currentWaypointIndex=raw`
- raw `0xFFFF` → `currentWaypointAvailable=false`, `currentWaypointIndex`는 의미값 없음

## 8. setTargetInformation

**Status: KEEP CURRENT STRUCTURE**

MDV/EMDW 원 ICD의 Target Information 메시지는 별도 공통 제어응답을 갖지 않는다.
Body에도 플랫폼 ID를 별도 제어 Target 필드로 두는 근거가 없으므로 현재처럼 Parameters 중심 Control로 유지하고 Reply를 임의로 추가하지 않는다.

Semantic Target을 별도로 강제하지 않는다.

## 9. Heartbeat

**Status: PHYSICAL DEFINITION CONFIRMED / SEMANTIC DIRECTION TBD**

`rov_common.csv` 기준 `T_HEARTBEAT`:
- 주기 1초
- 양방향 송수신
- `srcDevID`
- `dstDevID`
- `heartBeatCnt`
- 송신 시 `heartBeatCnt` 1씩 증가

물리 Heartbeat의 양방향 전송 자체는 확정이다.

그러나 **자체 송신 Heartbeat를 Semantic Monitor 대상으로 정의하는 것이 장기 구조상 적절한지는 아직 확정하지 않는다.**
현재 MDV/EMDW의 `transmitHeartbeat` / `receiveHeartbeat` 구조는 당장 유지하고, 향후 Monitor 방향성 또는 통신관리 전용 모델을 검토한다.

송신 Heartbeat의 Header Reserved는 공통 zero-fill 규칙에 따라 `FixedField value="0"`으로 반영하였다.

## 10. Reserved / Unused Field

**Status: RESOLVED BY COMMON RULE / APPLIED WHERE MODIFIED**

송신 시 미사용/Reserved Field는 프로젝트 공통 규칙에 따라 0으로 설정한다.
원 ICD가 별도 값을 직접 지정하면 원문을 우선한다.

## 11. CDM 적용 주의

- MDV/EMDW에서 직접 동일 의미가 확인된 상태는 공통 CDM을 재사용한다.
- USV의 `Degraded/Unavailable/NoResponse` 상태와 MDV/EMDW의 `Warning/Abnormal` 상태는 의미 체계가 다르므로 이름이 비슷하다는 이유로 강제 통합하지 않는다.
- 신규 세부 CDM 명칭은 전체 시스템 CDM audit에서 최종 확인한다.

## 12. Binding Converter Mapping

**Status: RESOLVED / APPLIED / VERIFIED**

### `CommandResultCodeToState`
- raw `0` → 성공 (`Command.Result.Success`)
- raw `1` → 실패 (`Command.Result.Failure`)

적용 수량:
- MDV Reply 9개
- EMDW Reply 12개

### `UInt16LEToMdvCommandReasonState`
little-endian uint16 decode 후:
- `0` 없음
- `1` 요청값 오류
- `2` 임무계획 오류
- `3` 모드전환 불가
- `4` 모드 불일치
- `5` MDV 미연동
- `6` 운용상태 비정상
- `7` 기타 오류

MDV Reply 9개에 적용하였다.

### `UInt16LEToEmdwCommandReasonState`
little-endian uint16 decode 후:
- `0` 없음
- `1` 요청값 오류
- `2` 임무계획 오류
- `3` 모드전환 불가
- `4` 모드 불일치
- `5` EMDW 미연동
- `6` 운용상태 비정상
- `7` 처리(Disposal)조건 불충족
- `8` 기타 오류

EMDW Reply 12개에 적용하였다.

### `MissionWaypointActionToCode`
- 통과 → `0`
- 대기 → `1`

MDV/EMDW `transferMissionPlan`의 Waypoint Action에 적용하였다.

### `MissionTypeToCode`
- 이동 → `0`
- 탐색 → `1`
- 식별 → `2`

MDV/EMDW `transferMissionPlan`에 적용하였다.

### `EmergencyStopReasonToCode`
- 운용자 명령 → `0`
- 체계 판단 → `1`
- 통신 이상 → `2`
- 장애 발생 → `3`
- 기타 → `4`

MDV/EMDW의 두 비상정지 Control에 적용하였다.

### `WaypointIndexToAvailable` / `WaypointIndexToIndexWhenAvailable`
`Field converter="UInt16LE"`로 raw 값을 decode한 뒤 동일 물리값에서 두 Semantic 값을 파생한다.
- `WaypointIndexToAvailable`: `0~65534 → true`, `0xFFFF → false`
- `WaypointIndexToIndexWhenAvailable`: `0~65534 → 동일 index`, `0xFFFF → 의미값 없음`

MDV/EMDW Mission Progress Binding에 적용하였다.

## 13. 구현/검증 상태

**Binding patch commit:** `1772faf5feed15f8e796c8f5bcf0008ca8b38625`
**Current Waypoint availability commit:** `35bc0a89cd22af0679905184c9671971d16325cd`

반영 및 확인 완료:
- `ValueSetProfile` XSD 추가
- `ValueSetResult` XSD 추가
- MDV/EMDW Reply Result 의미 추가
- MDV Reply 9개 / EMDW Reply 12개 result/reason converter 연결
- 기존 `result=0=명령 수락` 과해석 주석 제거
- Mission Type / Waypoint Action / Emergency Stop Reason 의미형 Parameter 및 Binding converter 연결
- MDV statusCode 의미 보강
- `currentWaypointAvailable` Boolean 상태 추가
- currentWaypointIndex 정상 범위 `0~65534` 유지
- 단일 물리 currentWaypointIndex Field → `currentWaypointAvailable` + `currentWaypointIndex` 복수 `DerivedSemantic` 매핑 적용
- 송신 Heartbeat Header Reserved zero-fill
- Heartbeat Semantic 방향 문제 TBD 유지
- 패치 후 MDV/EMDW Binding XML parse 성공

별도 후속 구현/검증:
- converter 식별자에 대응하는 Adapter 실제 변환 구현/등록 여부 확인
- 마지막 Waypoint Action Validator/OM 구현
- 전체 시스템 CDM audit
- 기존 Monitor `ValueSetSpec`의 raw 숫자값을 Binding으로 이전할지 공통 XSD 차원에서 검토
- Heartbeat Semantic 방향 모델은 별도 결정 전까지 구조 변경 금지
