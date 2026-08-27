# ElectroOpticalProcessor Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 전자광학장치 영상처리 CSC.
- 원천 파일명은 `전자광학추적장치 CSCI.csv`이나 CSV 내부 Source/Destination 명칭은 `전자광학장치 영상처리 CSC`이므로 내부 명칭을 기준으로 한다.
- 근거: `전자광학추적장치 CSCI.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`.
- 타 장비 간 내부 연동은 본 Binding 범위에서 제외한다.
- 공용 header/destination/commandID, CommandStatus ACK, PBIT/IBIT 단일 결과 메시지, declarative Binding 원칙은 공통 설계 규칙을 따른다.

## 2. 이번 감사에서 확정/반영

- 전자광학 영상처리 대상 식별자는 `SurfaceDetection=0x03 / EoSystemProcessor=0x06 / subEquipment=0x00`이다.
- 공용 Control의 `destination`은 OM/Adapter가 `System.Target.ElectroOpticalProcessor`로 준비하고 Binding은 연결만 선언한다.
- `ImageObjectDetection2DSeqReportType` primitive 자료형을 원문대로 보강하였다.
  - `imageWidth`, `imageHeight` → `UInt16`
  - `imageObjectDetectionCnt` → `UInt8`
  - `ImageObjectDetection2D.classID`, `confidence` → `UInt8`
  - Bounding Box 좌표/크기 → `UInt16`
  - `relativeAngle` → `Float32`
- 객체 classID 0~7은 Semantic 논리값 + Binding `ValueMap`으로 분리하였다.
- 공용 CIPE PBIT/CBIT/IBIT 상태 raw code는 Semantic에서 제거하고 Binding `ValueMap`으로 이동하였다.
- `ProcessorIBIT`: `cpuTemperature=Float64, 20~100 degC`, `cpuLoad=Float64, 0~100 percent`를 반영하였다.
- PBIT/IBIT는 각각 CommandStatus ACK와 별개의 단일 Result Reply로 유지하였다.
- `EOIRImageSaveControlType.controlImageSave`는 0=no change, 1=이미지 저장이며 Semantic에는 실제 저장 동작만 노출한다.
- 기존 `BuildUSVMessageBase`, `UInt16` converter를 제거하였다.
- 공통 XSD 경로를 `../../XSD/...`로 정리하였다.
- `relativeAngle`의 원문 Range `-PI~+PI`는 XSD decimal에 임의 근사값을 넣지 않고 Semantic 주석으로 보존하였다.

## 3. Remaining TBD

1. **공용 CIPE Topic의 송신 카드 식별 방식**
   - 센서융합/라이다/항해레이더/전자광학 영상처리카드가 동일한 `CIPEPBITReportType`, `CIPECBITReportType`, `CIPEIBITReportType` Topic/Type을 사용한다.
   - 실제 송신 카드는 `usvHeader.equipmentID`로 구분 가능하지만 현재 Binding XSD에는 Monitor/Reply용 source match 조건이 없다.
   - Adapter/runtime에서 source header를 기준으로 해당 장치 Binding에 라우팅하는 공식 규칙이 필요하다.

2. **PBIT/IBIT Result correlation**
   - `RCUPBITControlType` / `RCUIBITControlType`에는 `commandID`가 있으나 `CIPEPBITReportType` / `CIPEIBITReportType` 결과에는 `commandID`가 없다.
   - CommandStatus ACK 이후 전용 결과를 어떤 기준으로 요청과 연결하는지 런타임 규칙 확인이 필요하다.

3. **`cipeOperationalStatus`의 허용 subset**
   - 공용 구조체에는 Radar/Lidar/NearContact/EOIR 각각 Connected/Disconnected 총 8개 code가 정의되어 있다.
   - 현재 전자광학 영상처리카드 Semantic/Binding은 장치 기능상 EOIR에 해당하는 `40=Connected`, `43=Disconnected`만 노출한다.
   - 실제 이 카드가 40/43 외 공용 code도 발행할 수 있는지는 원문에 직접 명시되지 않았다.

4. **`imageObjectDetectionCnt`와 sequence 길이 일치 규칙**
   - count 값은 0~255이고 객체 sequence 최대 길이도 255개로 정의되어 있다.
   - count와 실제 sequence length가 불일치할 때의 처리 규칙은 원문에 없다.

5. **`relativeAngle` Range의 선언 방식**
   - 원문은 `-PI~+PI rad`를 사용한다.
   - 현재 Semantic XSD `Range`는 decimal이므로 PI 상수를 직접 표현할 수 없으며 임의 근사값을 넣지 않았다.
   - 향후 상수/수식 Range 표현을 XSD에서 지원할지 여부는 전역 스키마 개선 사항이다.

6. **`controlImageSave`의 boolean wire type 표현**
   - 원 CSCI/IDL의 `controlImageSave`는 `boolean`이다.
   - 현재 `CommonBindingSchema.xsd`의 `WireDataType`에는 Boolean 항목이 없어 `dataType="UInt8"`로 임의 치환하지 않고 `FixedField value="1"`만 유지한다.
   - 향후 Binding XSD에 Boolean wire type을 추가할지, DDS boolean은 별도 규칙으로 처리할지 전역 결정이 필요하다.

## 4. 현재 상태

- 원문 직접 근거가 있는 Semantic/Binding 정합성 보강은 완료하였다.
- 남은 항목은 사용자가 정책을 선택할 사항이 아니라 추가 IDL/Adapter/XSD/원작자 근거가 필요한 비차단 TBD이다.
