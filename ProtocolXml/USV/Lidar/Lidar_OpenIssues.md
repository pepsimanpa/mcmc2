# Lidar Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 라이다 정보처리 CSC.
- 근거: `라이다 정보처리 CSC.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`.
- `TransferIntraNetworkStatusType` 등 라이다 정보처리 CSC가 다른 내부 장치로 보내는 연동은 원통 직접 접점이 아니므로 본 범위에서 제외한다.
- 공통 생성 `USVMessageBase`, `DestinationType`, `commandID`, CommandStatus 처리 ACK, PBIT/IBIT 전용 결과 분리 원칙을 적용한다.

## 2. 이번 감사에서 확정/반영

- 라이다 정보처리 대상 식별자는 `SurfaceDetection=0x03 / LidarProcessorCard=0x02 / subEquipment=0x00`이다.
- 원통 직접 Control은 `IntegrationControlType`, `SystemRebootControlType`, `RCUPBITControlType`, `RCUIBITControlType` 4종이다.
- 공용 Control destination은 `System.Target.Lidar`로 OM/Adapter가 준비한다.
- `LidarContactType`의 primitive type을 공용 구조체 기준으로 반영하였다.
  - `id`: UInt32
  - `boxHeight/boxWidth/boxLength`: Float64
  - `relativePositionX/Y/Z`: Float64
  - `relativeAzimuth`: Float64
  - `contactNum`: Int32
- CIPE 운용상태 및 처리카드 상태 raw code를 Semantic에서 제거하고 Binding `ValueMap`으로 이동하였다.
- PBIT/IBIT는 CommandStatus 처리 ACK와 별도의 CIPE 결과 Reply를 유지하였다.
- IBIT Processor 결과에 `cpuTemperature=Float64, 20~100 degC`, `cpuLoad=Float64, 0~100 percent`를 반영하였다.
- 기존 `BuildUSVMessageBase`, `UInt16` converter를 제거하였다.
- 원문 근거가 확인된 primitive `dataType` 누락은 0건으로 정리하였다.

## 3. Remaining TBD

1. **`cipeOperationalStatus` 실제 발행 subset**
   - 공용 CIPE 구조체는 Radar/Lidar/NearContact/EOIR 연결상태 code를 공통 정의한다.
   - 라이다 처리카드가 실제로 Lidar `20/23`만 발행하는지, 공용 전체 연결상태를 발행할 수 있는지 추가 확인이 필요하다.
   - 현재 Semantic은 라이다 카드의 직접 의미가 확인되는 Lidar 연결/비연결 상태만 노출한다.

2. **공용 CIPE Topic source routing**
   - 여러 CIPE 처리카드가 `CIPEPBITReportType`, `CIPECBITReportType`, `CIPEIBITReportType`, `CommandStatusReportType`을 공유한다.
   - 런타임에서 `usvHeader.equipmentID` 등으로 라이다 처리카드 보고를 식별하는 Adapter 규칙 확인이 필요하다.

3. **PBIT/IBIT Result correlation**
   - PBIT/IBIT 요청에는 commandID가 있으나 전용 CIPE 결과 Report에는 commandID가 없다.
   - CommandStatus ACK 이후 전용 결과를 어느 요청과 연결하는지 런타임 correlation 규칙 확인이 필요하다.

4. **`contactNum`과 sequence 길이 일치 규칙**
   - 접촉물 sequence 최대 길이는 256이며 별도 `contactNum`이 존재한다.
   - 두 값이 불일치할 경우 수신측의 우선순위/오류 처리 규칙은 원문에서 확인되지 않는다.

## 4. 현재 상태

- 원통 직접 연동 범위의 Semantic/Binding 구조 정리는 완료하였다.
- 남은 4건은 사용자 정책 선택사항이 아니라 추가 CSCI/IDL/Adapter 근거가 필요한 비차단 TBD이다.
