# AUV Semantic / Binding Design Decisions

- Date: 2026-08-18
- Branch: `feature/auv-reply-bit-binding`
- Scope: AUV Platform / RF Communication Semantic & Binding
- Common rules: `ProtocolXml/Docs/SemanticBindingRules_20260818.md`

본 문서는 2026-08-18까지 검토한 AUV Semantic/Binding 주요 이슈 13건의 결정사항을 고정하기 위한 기록이다. 이후 동일 이슈를 재검토할 때는 원 ICD의 새 직접 근거가 없는 한 본 결정을 우선한다.

## Issue 1. `startMission` INFORMATION2 / INFORMATION3

**Status: RESOLVED**

- `MISSION_TRANS`는 실제 임무계획 전송이므로 INFORMATION2를 사용한다.
- `MISSION_START`는 이미 전송된 임무를 시작하는 기능으로 INFORMATION3를 사용한다.
- 원문 설명: INFORMATION1/2는 초반 RF 설정, 암호키 설정 또는 임무계획 시 사용하고 대부분 INFORMATION3를 사용한다.
- `startMission`에 별도 MissionPlan Parameter를 만들지 않는다.
- 과거 Semantic 주석 중 MISSION_START가 INFORMATION2라고 되어 있는 표현은 수정 대상이다.

## Issue 2. `requestFileList` DATA_TYPE Parameter

**Status: RESOLVED**

- File_List_Req는 현재 저장된 주행/센서 데이터의 File_list를 요청하는 기능이다.
- Semantic에서 DATA_TYPE Parameter를 제거한다.
- Reply는 AUV/SSS/OC/AC/FLS 전체 파일 개수를 반환한다.
- INFORMATION3의 DATA TYPE / File_First / File_Final은 Data_Req 데이터 전송에 사용한다.

## Issue 3. `requestData` Semantic ↔ Binding CDM

**Status: RESOLVED, CDM naming pending Issue 6**

Semantic 입력은 다음 논리 선택값을 유지한다.
- `RecordedData.Selection.Platform`
- `RecordedData.Selection.SideScanSonar`
- `RecordedData.Selection.OpticalCamera`
- `RecordedData.Selection.AcousticCamera`
- `RecordedData.Selection.ForwardLookingSonar`
- `RecordedData.FileRange.First`
- `RecordedData.FileRange.Final`

Binding의 기존 `RecordedData.Transfer.*` 매핑은 위 Semantic 의미에 맞게 수정한다.

DATA TYPE은 독립 bit 선택으로 정의하며, 원문 근거 없이 정확히 하나만 선택하도록 제한하지 않는다.

## Issue 4. RF-3 물리 구조

**Status: RESOLVED**

RF-3는 다음 구조를 사용한다.
- COUNT: 4 bytes
- ACK: 4 bytes
- Length: 2 bytes
- INFORMATION DATA: `Length` bytes
- Total: `10 + Length`
- checksum 없음
- stop 없음

RF-3 ACK는 일반 명령 ACK가 아니라 DATA TYPE 및 Start/End frame flag 영역이다.

ACK bit:
- bit11 AUV_DATA
- bit12 SSS_DATA
- bit13 OC_DATA
- bit14 AC_DATA
- bit15 FLS_DATA
- bit16 Start
- bit17 End

## Issue 5. `requestData` Reply vs SensorProduct

**Status: RESOLVED**

- `requestData`는 RF-1의 데이터 전송 요청 Control이다.
- 실제 RF-3 파일 데이터는 SensorProduct / ProductBinding으로 표현한다.
- `requestData` Semantic에는 일반 RF-2/RF-3 Reply를 두지 않는다.
- Binding에 존재하는 `requestDataReply`는 제거 대상이다.

## Issue 6. CDM system-wide consistency

**Status: DEFERRED**

AUV Semantic/Binding 내부 문자열 일치만으로 CDM을 확정하지 않는다.

다음 시스템 범위에서 동일 의미 CDM을 먼저 검색한다.
- USV
- MDV
- EMDW
- AUV

원칙:
1. 동일 의미 기존 CDM 재사용
2. 같은 도메인이라면 기존 계층에 맞춤
3. 장치 고유 의미만 신규 장치/도메인 CDM 사용

특히 다음 항목은 보류 상태다.
- Integrated Navigation alignment/position
- AUV/Platform recorded-data file count
- Flash/Light brightness
- 개별 BIT/상태 Result CDM

## Issue 7. `MissionPlan.OperationMode` Range

**Status: RESOLVED**

물리 코드 영역은 `1~8`이다.

현재 확정 의미:
- `1 = 광역탐색모드`
- `2 = 정밀탐색모드`
- `3~8 = TBD`

따라서 현재 HMI/Semantic 선택 범위는 `1~2`를 유지한다.
`3~8`을 유효 운용 입력으로 노출하지 않는다.

## Issue 8. Semantic Result ↔ Binding BitMember 연결

**Status: RESOLVED structurally / CDM values pending Issue 6**

- Result와 Field/BitMember는 동일 CDM을 식별키로 사용한다.
- 이름 또는 XML 선언 순서에 의존하지 않는다.
- Reserved bit는 Semantic Result/CDM을 정의하지 않는다.

## Issue 9. Semantic Result의 상태 의미

**Status: RESOLVED**

Semantic에서는 사용자가 이해하는 논리 상태를 보여준다.

예:
- 정상 / 고장
- 발생 / 미발생
- 활성 / 비활성

실제 `0/1`, bit offset, packing 및 논리 상태로의 변환은 Binding에서 정의한다.

AUV 주요 bit 의미:
- AUV_CHECK_RESULT 계열: 0=PASS, 1=FAIL
- EOR: 0=미발생, 1=발생
- MODE: 0=비활성, 1=활성
- STAT_CODE bit0~4: 0=정상, 1=비정상
- STAT_CODE bit5/6: DATA_LOCK/SYNC_LOCK flag

## Issue 10. RF_CONF_MODE / RF_CONF_PWR converter-only 표현

**Status: RESOLVED direction / implementation required**

Semantic은 사용자 선택 항목을 논리적으로 노출한다.

RF_CONF_MODE 관련:
- 운용모드: 1:1 / 1:2
- Time Slot: 2 / 4
- UHF Channel: 1 / 2 / 3
- L-Band Channel: 1 / 2 / 3

RF_CONF_PWR 관련:
- UHF Power: OFF / LOW / HIGH
- L-Band Power: OFF / LOW / HIGH

Binding에서는 전체 byte 구조를 `PackRfConfMode`/`PackRfConfPower` converter 하나에 숨기지 않고 PackedField/BitMember로 표현한다.

물리 구조:
- RF_CONF_MODE bit0: 1:1
- bit1: 1:2
- bit2: Time Slot 2
- bit3: Time Slot 4
- bit4~5: UHF Channel
- bit6~7: L-Band Channel
- RF_CONF_PWR: UHF/L-Band 각각 OFF/LOW/HIGH one-hot

선택형 Control Parameter를 명시적으로 표현하기 위한 XSD 확장이 필요할 수 있다.

## Issue 11. PackedField validation

**Status: RESOLVED as validator rule**

XSD 1.0에 복잡한 산술/형제 비교 제약을 억지로 넣지 않는다.

별도 Validator에서 확인:
- `offset + width <= PackedField.width`
- BitMember overlap 없음
- expectedValue / expectedMask width 초과 없음
- fixedValue width 초과 없음

## Issue 12. 미사용 INFORMATION Field

**Status: RESOLVED**

프로젝트 규칙으로 **미사용 Field 및 Reserved 영역은 0으로 전송한다.**

따라서 Binding의 `FixedField value="0"`을 유지할 수 있다.
원 ICD가 특정 값을 별도로 요구하는 경우 원문 값을 우선한다.

## Issue 13. RF-2 ACK / RF_CMD_COMPLETE / Body

**Status: PARTIALLY RESOLVED / simultaneity TBD**

원문 의미:
- `RF_CMD_COMPLETE`는 RF통신장치에 보낸 명령이 완료되었음을 알리는 bit이다.
- RF통신장치가 이 bit를 1로 설정하여 휴대용콘솔에 전송하면 휴대용콘솔은 RF통신장치에 보낸 명령이 완료되었음을 확인한다.

따라서:
- ACK: 어떤 원 명령에 대한 응답인지 식별
- RF_CMD_COMPLETE: RF통신장치 측 명령 처리 완료 확인
- INFORMATION Body: 해당 명령에서 정의된 실제 결과/상태

`RF_CMD_COMPLETE=1`은 AUV 본체의 실제 임무/동작 성공 또는 완료를 의미하지 않는다.

ACK + RF_CMD_COMPLETE + Body 결과가 항상 동일 RF-2 Telegram에서 동시에 유효한지는 직접 근거가 없어 TBD로 유지한다.

## 확정된 별도 bit 규칙

### COMMAND
- bit0 AUV_CHECK_Req
- bit1 MISSION_TRANS
- bit2 RC_MODE
- bit3 Status_Chk
- bit4 MISSION_START
- bit5 EMER_RETURN
- bit6 File_List_Req
- bit7 Data_Req
- bit8 MISSION_STOP
- bit9 Record_Del
- bit10 MTE_MODE
- bit15 FLASH
- bit16 PLC_Req
- bit24 RF_STATUS
- bit25 RF_CONFIG
- bit26 RF_CH_SCAN
- bit27 RF_GAIN
- bit28 KEY_CMD
- bit30 RF_SEND
- bit31 RF_EXEC

### AUV_CHECK_RESULT_L
- bit0 reserved
- bit1 MSS_TEST
- bit2 CCS_TEST
- bit3 NS_TEST
- bit4 BS_TEST
- bit5 PRS_TEST
- bit6 MCS_TEST
- bit7 reserved

### EOR
- bit0 LEAK
- bit1 ROLL
- bit2 PITCH
- bit3 VEL
- bit4 COM
- bit5 PCD
- bit6 GPS
- bit7 RF/UCD
- bit8 Ceiling Depth
- bit9 BT_SOC
- bit10 BT_TEMP
- bit11 BT_CO
- bit12 BT_V
- bit13 MC
- bit14 DIS
- bit15 EOM

### MODE
- bit0 PLC
- bit1 MS
- bit2 RC
- bit3 ALIGN
- bit4 MS_ST
- bit5 EMER
- bit6 FILE
- bit7 DATA
- bit8 MS_STOP
- bit9 REC_DEL
- bit10 RF_CONF
- bit11 EOR
- bit12~15 reserved

## 현재 남은 작업

1. Issue 6 전체 CDM 정합성 감사
2. 위 결정에 따른 Semantic/Binding/XSD 최소 수정
3. RF-3 ProductBinding 구조 반영
4. RF_CONF_MODE/PWR PackedField 반영
5. Result ↔ BitMember CDM 연결 반영
6. Validator 항목 구현 또는 검증 스크립트 마련
7. XML/XSD 전체 검증
8. RF-2 ACK/RF_CMD_COMPLETE/Body 동시 유효성 원문 추가 확인
