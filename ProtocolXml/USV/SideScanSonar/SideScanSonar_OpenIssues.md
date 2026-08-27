# SideScanSonar Open Issues

이 문서는 예인형수중탐색장치 CSCI 및 공용 CSV를 기준으로, 현재 단계에서 확정하지 않은 사항과 후속 검토 항목을 보존한다.

## Source basis

- `공용 식별자 규칙.csv`
- `공용 규칙.csv`
- `공용 구조체.csv`
- `원격통제장치 CSCI.csv`
- `예인형수중탐색장치 CSCI.csv`

## 이번 단계에서 확정한 기준

- SSS Semantic/Binding은 장치 폴더의 오래된 로컬 XSD 복사본이 아니라 `ProtocolXml/XSD/CommonSpecSchema.xsd`, `ProtocolXml/XSD/CommonBindingSchema.xsd`를 기준으로 검증한다.
- `USVMessageBase`는 공용 구조체의 발행주체 정보이며, 실제 값 조립은 OM/Adapter가 수행한다. Binding은 이미 준비된 구조체를 연결만 한다.
- SSS 명령 대상 `DestinationType`은 공용 식별자 규칙상 `dstEquipmentType=0x04`, `dstEquipmentID=0x01`, `dstSubEquipmentID=0x00`을 사용한다. 구조체 조립은 OM/Adapter 책임으로 둔다.
- 일반 `commandID`는 원격통제장치 CSCI 규칙에 따라 비주기 메시지 생성 시 증가시키되, 실제 생성은 OM/Adapter가 수행하고 Binding은 `UInt16` wire type만 선언한다.
- `CommandStatusReportType`은 명령 처리상태 ACK이다. 공용 구조체의 상태는 `Executing/Pending/Failed/Reject/Canceled`이므로 실제 물리 동작 완료/성공으로 해석하지 않는다.
- 기존 SSS Semantic/Binding XML의 CSCI 근거 주석과 물리 Type 이름은 삭제하거나 임의 수정하지 않는다.

## Deferred / TBD

### 1. LastReceivedCommandID correlation

- `StartControlType`, `AutoLaunchControlType` 등 일부 CSCI는 수신받은 메시지의 `commandID` 값을 읽어서 그대로 넣어 송신하도록 명시한다.
- 현재 Binding의 `System.Communication.LastReceivedCommandID` 표현은 유지한다.
- 여러 명령이 동시에 진행될 때 어느 수신 메시지의 `commandID`를 계승할지 정확한 OM/Adapter 상관관계 규칙은 TBD이다.

### 2. 결과 Report의 commandID 부재

- `TSAPBITReportType`, `TSAIBITReportType`, `LaunchReportType`, `LanchAndRecoveryBackStopReportType`, `CommunicationLevelReportType` 등 일부 결과 Report에는 `commandID`가 없다.
- 요청-결과 correlation 방법은 TBD이다. 현재 별도 Reply 구조는 유지한다.

### 3. DetailSystemStatusType 전원 상태의 0/1 의미

- `PlatformPowerControlType.powerOn`은 CSCI에서 `0=OFF`, `1=ON`이 명시되어 있다.
- 반면 `DetailSystemStatusType`의 `towedSonarArrayPowerStatus` 등 전원 상태 필드는 원문에 0/1 의미가 직접 명시되어 있지 않다.
- 제어값과 상태값이 동일할 가능성은 높지만 현재 단계에서는 확정하지 않는다.

### 4. IBIT 상세 bit 해석

- 다수 IBIT detail octet은 비정상 bit flag이다.
- 그러나 `sonarImagingProcessingEquipmentDetail`은 CSCI에서 보드/조립체 **장착 유무** bit flag로 정의되어 있다.
- 따라서 모든 IBIT detail bit를 일괄적으로 `0=PASS / 1=FAIL` 또는 `0=정상 / 1=비정상`으로 해석하지 않는다. 필드별 원문 기준으로 후속 분석한다.

### 5. DDS 중첩 구조체 접근 표기

- 현재 Binding은 `commandStatusReport.dstEquipmentType`처럼 dotted path를 사용한다.
- 공통 Adapter가 dotted path를 정식 구조체 접근 규칙으로 사용할지 최종 확정은 TBD이다.

### 6. 로컬 XSD 복사본 정리

- `SideScanSonar/CommonSpecSchema.xsd`, `SideScanSonar/CommonBindingSchema.xsd`는 현재 `ProtocolXml/XSD`의 공통 XSD보다 오래된 snapshot이다.
- 이번 단계에서는 외부 도구가 이 파일을 직접 참조할 가능성을 고려해 파일 자체는 삭제하지 않는다.
- SSS XML의 `schemaLocation`만 공통 XSD로 전환하고, USV 전체 migration 완료 후 외부 의존성을 확인한 다음 삭제 여부를 결정한다.

### 7. 최종 CDM 감사

- CDM 명칭과 계층은 USV 장치 전체를 동일 기준으로 정리한 뒤 별도 전수 감사한다.

### 8. CSCI 오탈자 보존

- `LanchAndRecoveryBackStopReportType`, `ScreanChangeConfigType` 등 물리 Type 이름의 오탈자는 CSCI 원문 및 DDS `typeName`과의 정합성을 위해 현재 그대로 보존한다.
