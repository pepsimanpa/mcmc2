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


## Primitive dataType migration

- `예인형수중탐색장치 CSCI.csv`, `원격통제장치 CSCI.csv`, `공용 규칙.csv`, `공용 구조체.csv`에서 자료형이 직접 확인되는 primitive 필드만 Binding `dataType`으로 반영한다.
- IDL 매핑 기준: `octet/unsigned char -> UInt8`, `short -> Int16`, `unsigned short -> UInt16`, `long -> Int32`, `unsigned long -> UInt32`, `float -> Float32`, `double -> Float64`.
- `USVMessageBase`, `DestinationType` 등 composite 구조체에는 primitive `dataType`을 억지로 부여하지 않는다.
- 동일 이름이라도 메시지별 타입이 달라질 수 있는 필드는 DDS `typeName` 문맥으로 재확인한다. 특히 원신호의 `pulseType`과 제어 `StatusConfigType.pulseType`처럼 이름 재사용이 있는 경우 현재 Binding에 실제 사용되는 메시지 정의를 우선한다.
- `boolean`, 배열, 문자열/char, 사용자 정의 구조체처럼 현재 `WireDataType`으로 직접 표현하기 애매한 항목은 후속 검토로 남긴다.


## Enum / ValueMap migration

- Semantic 논리 상태의 raw 숫자 코드를 제거하고 실제 DDS 코드는 Binding `ValueMap`으로 이동한다.
- 반영: 운용모드, SSS/GF PulseType, SSS/GF TVG, 진회수 상태, USBL 모드, 3개 Brake, 2개 Limit Switch, CBIT 4상태, 제어 `powerOn`, 제어 `pulseType`.
- 5개 `DetailSystemStatusType` powerStatus는 원문에서 0/1 의미가 직접 확인되지 않아 raw 0/1을 임시 유지한다.
- `SideScanSonar.SonarType`, `ReceiveProcessing.SoftwareGain.Mode`, `ReceiveProcessing.LowPassFilter.Mode`, `SideScanSonar.Subsystem(targetDevice)`은 최종 논리 값/CDM 재확인 후 이동한다.
- 최종 CDM 감사 전에는 기존 CDM을 재사용하고 새 CDM 생성을 최소화한다.


## Reply semantic result contract

- PBIT와 IBIT은 각각 하나의 점검 결과 DDS 메시지이며, Semantic에서도 각각 `requestPbitResult`, `requestIbitResult` 한 개의 Reply를 유지한다. 메시지 내부 결과 필드는 하나의 `GroupResult` 아래 표현한다.
- 38개 `CommandStatusReportType` ACK는 라우팅/상관관계 필드가 아니라 `commandStatusReport.status`만 Semantic Result로 노출한다. `dstEquipmentType`, `dstEquipmentID`, `commandID`는 Binding/Adapter 메타데이터로 유지한다.
- `CommandStatusReport.status`의 원문 값(Executing/Pending/Failed/Reject/Canceled)은 확정되어 있으나, 세부 상태 CDM 명칭은 최종 CDM 감사까지 새로 만들지 않는다. 따라서 현재 Semantic은 `Control.Response.Status` 결과 존재만 선언한다.
- `TSAPBITReportType`, `TSAIBITReportType` 내부 필드는 현재 Binding에 이미 존재하는 CDM을 그대로 Semantic Result에 재사용하며, Total 상태코드 및 IBIT detail bit의 raw 의미는 추가 추론하지 않는다.
- `LaunchReportType`, `LanchAndRecoveryBackStopReportType`, `CommunicationLevelReportType`도 하나의 결과 Report Reply를 유지하고 실제 payload 필드를 Semantic Result로 노출한다. raw 0/1/2 등의 의미 매핑은 원문 재확인 전까지 보류한다.
- commandID가 없는 결과 Report의 요청-결과 correlation 방식은 기존 TBD를 유지한다.


## Final parameter / source audit

- `StatusConfigType.sonarType`: `0=예인형 SSS`, `1=예인형 GapFiller`, `2=선체부착형 SSS`를 Semantic 논리 선택 + Binding ValueMap으로 반영한다.
- `StartControlType.sonarType`: 별도 코드 체계 `0=예인형`, `1=선체부착형`을 사용한다. 동일 필드명이어도 메시지별 wire code를 Binding에서 구분한다.
- `PlatformPowerControlType.targetDevice`: `1=SSS`, `2=USBL`, `3=WINCH`, `4=CAMERA`, `5=HMS`로 반영한다.
- `CommunicationLevelCommandType/ReportType.videoQualityType`: 모두 `0~7=MCS Level1~Level8`로 정의되어 제어/결과를 ValueSet으로 표현한다. Report가 요청값을 단순 echo한다고 추가 가정하지 않는다.
- `LaunchReportType.launch`: 결과 의미 `0=정지상태`, `1=진수완료`, `2=회수완료`로 반영하며, `AutoLaunchControlType.launch` 명령의 `0=정지/1=진수/2=회수`와 구분한다.
- `StatusConfigType.frequency`: 예인형 SSS(`sonarType=0`)에 대해 100/600 kHz만 원문에 명시된다. 현재 CommonSpecSchema에 제어용 `QuantityValueSetProfile`이 없어 `Profile`을 유지하고 XML 주석으로 제한을 보존한다. 선체부착형 frequency 허용값은 TBD이다.
- GapFiller는 별도 range 설정이 없고 예인형 SSS range와 동기화된다. 현재 평면 Parameter 모델에서 조건부 적용은 직접 표현하지 않고 후속 스키마/OM 검토로 남긴다.
- `ReceiveProcessing.TVG.Mode(0~1)`, `ReceiveProcessing.SoftwareGain.Mode(0~2)`, `ReceiveProcessing.LowPassFilter.Mode(0~1)`은 정확한 논리 명칭을 이번 근거에서 확정하지 못해 QuantityProfile을 임시 유지한다.
- `LanchAndRecoveryBackStopReportType.slideBackStop`, PBIT Total, IBIT Total/detail의 raw 의미는 기존 TBD를 유지한다.
- 새 SonarType/Subsystem/MCS/LaunchReport 하위 CDM 경로는 연결용 임시 키이며 최종 USV CDM 감사에서 명칭/계층을 재검토한다.


## Final structural audit

- Semantic/Binding Control은 `38↔38`, Reply는 `47↔47`이며 ID 연결도 일치한다.
- 수동 USBL 명령은 `launch=0/1/2/3`(정지/진수/회수/원점복귀), USBL 작동은 `start=0/1`, 슬라이드는 `start=0/1/2/3`, 윈치는 `start=0/1/2`의 분리 Control + `FixedField` 구조를 유지한다. 이 값들은 사용자 선택 enum이 아니라 이미 행위별 Control로 분리되었으므로 FixedField가 적절하다.
- USBL `motorSpeed(0~0.011 m/s)`, 슬라이드 `slideDeploy(0~2.15 m)`, 윈치 `winchDeploy(0~100 m)` 보조 필드는 정지/원점복귀를 포함한 각 원문 메시지 구조에서 유지한다. 원문에 무시/고정값 규칙이 없으므로 임의 제거하지 않는다.
- 케이블 분리 `0/1`, 화면모드 `0/1`, 자동 진회수 명령 `0/1/2`, BackStop 현재 `0/1/2` FixedField 구조를 확인했다. BackStop 결과 Report의 raw 의미는 별도 TBD이다.
- PBIT/IBIT은 각각 하나의 결과 Reply + 하나의 GroupResult이며 내부 필드는 각각 6개/62개이다.
- SensorProduct 5종은 Semantic/Binding `5↔5`: SSS 압축, GapFiller 압축, 병합 압축, ObjectDetection2D, 진회수 카메라 RTP이다. ProductBinding은 스트림 채널/메시지 선택 구조이므로 `ContactTypeList.sonarType` 같은 payload 내부 분류 필드를 별도 Binding Field로 누락된 것으로 보지 않는다.
- converter는 0건이고 primitive dataType 미지정은 0건이다. `usvHeader`, `destination` composite만 의도적으로 dataType이 없다.
- 설계를 막는 미해결 항목은 아니지만 원문 재확인이 필요한 핵심 TBD는 전원상태 5개 0/1 의미, TVG/SW Gain/LPF mode 논리명, PBIT/IBIT raw 상세 의미, BackStop 결과값 의미, commandID 없는 Result Report correlation, LastReceivedCommandID 동시성, dotted-path Adapter 규칙이다.
