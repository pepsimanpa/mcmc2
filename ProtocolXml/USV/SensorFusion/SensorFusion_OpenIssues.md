# SensorFusion Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 센서융합 정보처리 CSC.
- 근거: `센서융합 정보처리 CSC.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`.
- 타 장비 간 내부 연동은 본 Binding 범위에서 제외한다.
- 공용 header/destination/commandID, CommandStatus ACK, PBIT/IBIT 단일 결과 메시지, declarative Binding 원칙은 공통 설계 규칙을 따른다.

## 2. 이번 감사에서 확정/반영

- 센서융합 대상 식별자는 `SurfaceDetection=0x03 / SensorFusionProcessorCard=0x01 / subEquipment=0x00`이다.
- 공용 Control의 `destination`은 OM/Adapter가 `System.Target.SensorFusion`으로 준비하고 Binding은 연결만 선언한다.
- `FusionContactReportType.contactNum`과 `FusionWaveReportType.waveNum`은 `long` → `Int32`.
- `FusionContactType` primitive 자료형을 원 공용 구조체대로 선언하였다.
  - `long` 계열 → `Int32`
  - `double` → `Float64`
  - `float` → `Float32`
- `sensorFusionContactIdentifier` 0/1과 `sensorFusionContactAttribute` 0/1은 Semantic 논리값 + Binding `ValueMap`으로 분리하였다.
- `WaveType` 전체 primitive 자료형을 원 공용 구조체대로 선언하였다.
- `CIPEPBITReport` / `CIPECBITReport` / `CIPEIBITReport`의 상태 raw code는 Semantic에서 제거하고 Binding `ValueMap`으로 이동하였다.
- `ProcessorIBIT`: `cpuTemperature=Float64, 20~100 degC`, `cpuLoad=Float64, 0~100 percent`를 반영하였다.
- PBIT/IBIT는 각각 CommandStatus ACK와 별개의 단일 Result Reply로 유지하였다.
- 기존 `BuildUSVMessageBase`, `UInt16` converter를 제거하였다.
- 공통 XSD 경로를 `../../XSD/...`로 정리하였다.
- π 기호로 정의된 각도 Range는 XSD decimal에 임의 근사값을 넣지 않고 Semantic 주석으로 원문 범위를 보존하였다.

## 3. Remaining TBD

1. **공용 CIPE Topic의 송신 카드 식별 방식**
   - 센서융합/라이다/항해레이더/전자광학 영상처리카드가 동일한 `CIPEPBITReportType`, `CIPECBITReportType`, `CIPEIBITReportType` Topic/Type을 사용한다.
   - 실제 송신 카드는 `usvHeader.equipmentID`로 구분 가능하지만 현재 Binding XSD에는 Monitor/Reply용 source match 조건이 없다.
   - Adapter/runtime에서 source header를 기준으로 해당 장치 Binding에 라우팅하는 공식 규칙이 필요하다.

2. **PBIT/IBIT Result correlation**
   - `RCUPBITControlType` / `RCUIBITControlType`에는 `commandID`가 있으나 `CIPEPBITReportType` / `CIPEIBITReportType` 결과에는 `commandID`가 없다.
   - CommandStatus ACK 이후 전용 결과를 어떤 기준으로 요청과 연결하는지 런타임 규칙 확인이 필요하다.

3. **`cipeOperationalStatus`의 센서융합 카드 보고 방식**
   - 공용 구조체 한 octet에 Radar/Lidar/NearContact/EOIR 각각 Connected/Disconnected 총 8개 code가 정의되어 있다.
   - 센서융합 처리카드가 4개 센서 연결상태를 한 번에 하나씩 순환 보고하는지, 특정 우선순위/이상상태만 보고하는지 원문에 없다.
   - 현재는 원문에 정의된 8개 논리값을 모두 허용한다.

4. **`contactNum` / `waveNum`과 sequence 길이의 일치 규칙**
   - 두 리스트는 `sequence<..., 256>`으로 최대 256개이지만 count 필드의 명시적 Range와 count/list 불일치 처리 규칙은 없다.
   - 현재 Semantic은 Collection 최대 256을 선언하고 count 자체 Range는 강제하지 않는다.

5. **Wave 부가 물리량 단위**
   - `WaveType.waveSpectrum`, `integratedWaveEnergy`는 원 공용 구조체에 Unit/Range가 없다.
   - 임의 단위를 부여하지 않는다.

## 4. 현재 상태

- 원문 직접 근거가 있는 Semantic/Binding 정합성 보강은 완료 가능 상태이다.
- 남은 항목은 사용자가 정책을 선택할 사항이 아니라 추가 IDL/Adapter/원작자 근거가 필요한 비차단 TBD이다.
