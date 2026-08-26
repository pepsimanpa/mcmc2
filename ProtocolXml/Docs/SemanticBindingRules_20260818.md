# Semantic & Binding Design Rules

- Date: 2026-08-18
- Scope: MCMC2 Semantic / Binding / Specification 공통 설계 규칙
- Status: Working baseline
- Priority: 이후 장치 분석 시 동일 이슈를 반복 재해석하지 않고 본 문서를 우선 적용한다. 단, 원 ICD/CSCI에 더 직접적이고 명확한 근거가 있으면 원문을 우선한다.

## 1. 근거 우선순위

1. 원 ICD/CSCI/원작자 XML/XSD의 직접 명시
2. 본 문서에 기록된 확정 설계 규칙 및 장치별 Design Decisions
3. 현재 Semantic/Binding/Specification 구현
4. 추론

과거 대화나 변환 CSV의 해석성 컬럼은 직접 원문과 동일한 권위를 갖지 않는다.

## 2. Semantic의 역할

Semantic은 전송매체와 무관한 원격 운용 의미 계약이다.

Semantic에 정의하는 항목:
- 운용자가 수행하는 Control의 의미
- Control Target
- 운용자/HMI가 실제 입력해야 하는 Parameters
- 단위, 범위, 해상도
- Reply 존재 여부와 논리적 결과
- Monitor 의미
- 파일/영상/스트림 등 SensorProduct의 논리적 의미

Semantic에 정의하지 않는 항목:
- TCP/UDP/DDS/RS422/RF/UCD 등 전송 방식
- byte offset, endian, checksum
- 물리 bit 위치, packed HEX 값
- wire 전용 예약/프레이밍 값

운용관리 내부 상태나 시스템이 자동으로 채우는 값은 HMI 입력 Parameter로 만들지 않는다.

## 3. Binding의 역할

Binding은 Semantic 의미를 실제 통신 규격에 매핑한다.

Binding에서 정의하는 항목:
- Protocol / Channel / Message / Topic / Type
- Field / FixedField / DerivedField / PackedField / BitMember
- 상수, 자동 생성값, 시스템 유도값
- byte order, bit packing, converter
- Reply의 실제 Telegram 및 결과 Field/Bit
- 하나의 물리 Field가 sentinel/state code 등으로 복수의 논리 의미를 포함하면 `Field` 하위 `DerivedSemantic`으로 여러 Semantic CDM을 파생한다. 이때 wire Field 자체를 중복 선언하지 않는다.
- expectedValue / expectedMask 등 수신 식별 조건

## 4. Control / Reply / Monitor

- Control은 송신 방향 자체가 아니라 원격 실행 가능한 기능이다.
- Control 내부 Reply는 해당 Control에 대한 응답을 의미한다.
- Monitor는 주기적/지속적으로 감시되는 데이터다.
- ACK와 실제 BIT/상태 결과가 별도 데이터라면 하나로 합치지 않는다.
- 명확한 주기 메시지 근거가 없으면 Monitor를 임의로 만들지 않는다.

### Heartbeat 방향 모델링 TBD

`rov_common.csv`의 `T_HEARTBEAT`는 1초 주기 양방향 송수신이며 `heartBeatCnt`는 송신 시 1씩 증가하도록 직접 정의되어 있다.

다만 **자체 송신 Heartbeat를 Semantic Monitor 대상으로 보는 것이 장기 구조상 적절한지 여부는 확정하지 않는다.**
현재 MDV/EMDW의 `transmitHeartbeat` / `receiveHeartbeat` 표현은 기존 구조를 유지하고, 향후 Monitor의 방향성 및 통신관리 전용 Semantic/Binding 모델을 검토한 뒤 결정한다.

## 5. Semantic Result / 선택값 표현 원칙

Semantic Result와 HMI 선택형 Parameter는 사용자가 이해할 수 있는 논리 의미를 표현한다.

예:
- 정상 / 고장
- 발생 / 미발생
- 활성 / 비활성
- 성공 / 실패
- 이동 / 탐색 / 식별

물리값 `0/1`, enum code, bit offset, mask와 같은 wire 표현은 Binding에 둔다.

이를 위해 신규 설계에서는:
- HMI 선택형 Parameter: `ValueSetProfile`
- Reply 논리 상태 결과: `ValueSetResult`

를 사용한다. 두 형식의 `<Value>`에는 의미 이름과 CDM을 정의하며 wire 숫자값을 기록하지 않는다.

Semantic Result와 Binding의 Field/BitMember는 이름이나 선언 순서가 아니라 동일 CDM을 연결키로 사용한다.

Reserved bit는 Semantic Result를 만들지 않으며 CDM도 부여하지 않는다.

## 6. 공통 CDM 원칙

- 동일 의미가 USV/MDV/EMDW/AUV에 이미 존재하면 기존 공통 CDM을 우선 재사용한다.
- 같은 분야이지만 의미가 다르면 기존 계층에 맞춰 확장한다.
- 장치 내부 고유 의미만 장치/도메인 특화 CDM을 사용한다.
- 단순히 한 XML 내부의 Semantic/Binding 문자열을 맞추기 위해 새 CDM을 만들지 않는다.
- CDM 정합성은 장치 단위가 아니라 시스템 전반에서 검토한다.
- 원 ICD의 상태 의미가 다른 경우 명칭이 비슷하다는 이유만으로 하나의 CDM에 강제 통합하지 않는다. 예: `Warning/Abnormal`과 `Degraded/Unavailable`은 직접 동일 의미가 확인될 때만 통합한다.

## 7. PackedField / BitMember 규칙

- byteOrder 적용 후 정규화된 정수의 LSB를 `offset=0`으로 한다.
- 원 표의 윗행이 bit0으로 정의된 경우 MSB/LSB 표기만 보고 bit 순서를 뒤집지 않는다.
- packed 구조가 원문에 명확히 정의된 경우 converter 하나에 전체 구조를 숨기지 않고 PackedField/BitMember로 가시화한다.
- converter는 논리 선택값과 실제 bit code 사이 변환처럼 필요한 최소 변환에 사용한다.

XSD 1.0으로 직접 보장하기 어려운 다음 조건은 별도 Validator에서 확인한다.
- `offset + width <= PackedField.width`
- BitMember 간 overlap 없음
- expectedValue/expectedMask가 PackedField width를 초과하지 않음
- fixedValue가 BitMember width를 초과하지 않음

원 ICD가 반복 데이터 전체에 대한 조건을 지정하면 XSD에 억지로 넣지 않고 Validator/OM 규칙으로 관리할 수 있다. 예: MDV/EMDW 임무계획의 **마지막 Waypoint Action은 대기**.

## 8. 미사용 Field 처리 규칙

고정 길이 Telegram에서 현재 명령에 사용하지 않는 INFORMATION Field 및 Reserved 영역은 **0으로 전송한다**.

Binding의 `FixedField value="0"`은 본 프로젝트의 미사용 Field zero-fill 규칙을 나타낸다.

원 ICD가 특정 미사용값을 별도로 명시하는 경우 해당 원문 값을 우선한다.

## 9. SensorProduct 규칙

파일, 영상, 스트림 등 큰 데이터 산출물은 일반 Control Reply 필드에 억지로 포함하지 않는다.

- 요청 행위: Control
- 실제 파일/프레임/스트림: SensorProduct / ProductBinding

## 10. RF_CMD_COMPLETE 일반 규칙

AUV RF ICD 기준:
- `RF_CMD_COMPLETE`는 RF통신장치에 보낸 명령이 완료되었음을 알리는 bit이다.
- RF통신장치가 이 bit를 1로 설정하여 휴대용콘솔에 전송하면 휴대용콘솔은 RF통신장치에 보낸 명령이 완료되었음을 확인한다.
- 이는 AUV 본체의 실제 임무/동작 성공 또는 완료와 동일하지 않다.

ACK, RF_CMD_COMPLETE, INFORMATION Body가 항상 동일 RF-2 Telegram에서 동시에 유효한지는 직접 근거가 확인될 때까지 별도 TBD로 관리한다.

## 11. 변경 및 검증 체크리스트

Semantic/Binding 변경 후 최소 확인:
1. XSD 문법/검증
2. Semantic Control ID ↔ Binding semantic_id
3. Semantic Reply bindRef ↔ Binding Reply semantic_id
4. Parameter CDM ↔ Binding sourceField/CDM
5. Result CDM ↔ Binding Field/BitMember CDM
6. ValueSetProfile/ValueSetResult 논리 상태 ↔ Binding raw code 변환
7. PackedField bit width/overlap
8. Telegram 순서/길이/endian
9. Reserved/misused field zero-fill
10. 반복/상호조건 Validator 규칙
11. 기존 공통 CDM 재사용 여부
12. TBD를 추정으로 확정하지 않았는지 확인
