# NavigationRadar Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 항해레이더 정보처리 CSC.
- 근거: `항해레이더 정보처리 CSC.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`.
- `RadarAISFusionContactReportType` 등 타 CSC 전용 내부 연동은 본 범위에서 제외한다.
- 공통 생성 header/destination/commandID, CommandStatus 처리 ACK, PBIT/IBIT 결과 분리, declarative Binding 원칙을 적용한다.

## 2. 이번 감사에서 확정/반영

- 항해레이더 정보처리 대상 식별자는 `SurfaceDetection=0x03 / NavRadarProcessorCard=0x03 / subEquipment=0x00`이다.
- 공용 Control의 destination은 `System.Target.NavigationRadar`로 OM/Adapter가 준비한다.
- `RadarContactType`은 공용 구조체의 `Int32 / Float32 / Float64` 자료형을 Binding에 반영하였다.
- `AISContactType`은 공용 구조체에서 직접 확인되는 primitive type과 `shipName` 길이 21을 반영하였다. 원본 표의 일부 Range/Unavailable 열 이상값은 임의 보정하지 않는다.
- 레이더 송출, 영상 송출, AIS 메시지 출력 상태 enum은 Semantic 논리값과 Binding ValueMap/FixedField로 분리하였다.
- CIPE PBIT/CBIT/IBIT 상태 raw code를 Semantic에서 제거하고 Binding ValueMap으로 이동하였다.
- `ProcessorIBIT`의 `cpuTemperature=Float64, 20~100 degC`, `cpuLoad=Float64, 0~100 percent`를 IBIT 결과에 반영하였다.
- PBIT/IBIT는 CommandStatus 처리 ACK와 별도의 CIPE 단일 Result Reply로 유지하였다.
- 기존 `BuildUSVMessageBase`, `UInt16` converter를 제거하고 공통 XSD 경로를 `../../XSD/...`로 정리하였다.
- 레이더 영상은 기존대로 SensorProduct/RTP로 유지한다.
- `RadarAISStatusReportType.frameWidth`와 `frameHeight`는 `항해레이더 정보처리 CSC.csv`에서 각각 `long`, 4 Byte로 확인되어 Binding `Int32`로 확정하였다.

## 3. Remaining TBD / Resolved — 미해결 6개

1. **Radar/AIS 탐지거리 제어 octet 값 체계**
   - `RadarRangeControlType.setTargetRange`와 `AISRangeFilterControlType.setAISRange`는 모두 `octet`으로만 정의되어 있다.
   - 원문에 단위, 허용 범위, 프리셋 번호와 실제 거리의 대응표가 없다.
   - 상태 보고의 `radarRange`/`aisRange`가 m 단위라는 이유만으로 제어 octet을 거리(m)로 해석하지 않는다.

2. **`radarMessageTransmitStatus`의 업무 의미**
   - 상태 필드는 '레이더 메시지 출력 상태'로 정의되어 있으나 어떤 DDS/외부 메시지를 가리키는지 직접 명시되지 않았다.
   - 대응하는 별도 RCU Control도 확인되지 않아 Monitor 의미만 보존한다.

3. **공용 CIPE Topic의 송신 카드 식별 방식**
   - 여러 CIPE 처리카드가 `CIPEPBITReportType`, `CIPECBITReportType`, `CIPEIBITReportType`을 공유한다.
   - `usvHeader.equipmentID` 기반 Adapter/runtime source routing 규칙 확인이 필요하다.

4. **PBIT/IBIT Result correlation**
   - 요청에는 commandID가 있으나 CIPE 전용 결과에는 commandID가 없다.
   - CommandStatus ACK 이후 결과를 어떤 런타임 규칙으로 해당 요청과 연결하는지 확인이 필요하다.

5. **`cipeOperationalStatus` 허용 subset**
   - 공용 구조체는 Radar/Lidar/NearContact/EOIR 8개 연결상태 code를 정의한다.
   - 항해레이더 처리카드가 이 전체 code를 발행하는지 Radar 10/13만 발행하는지 원문에서 명확하지 않아 현재 공용 8개 값을 허용한다.

6. **contact count와 sequence 길이 일치 규칙**
   - Radar/AIS sequence 최대 길이는 256이지만 `contactNum`과 실제 sequence length가 불일치할 때의 처리 규칙은 없다.

7. **[RESOLVED] AISContactType 일부 원본 표 이상값**
   - 공용 구조체의 `AISContactType` Range 열은 `longitude=10000~`, `course=0~1`, `speed=0~12000`, `shipName=-180~180`, `shipType=0~360`, `shipLength=0~45`처럼 인접 의미와 맞지 않는 값이 연속되어 행/셀 정렬이 밀린 export 결함으로 판단된다.
   - 동일 `공용 구조체.csv`의 `RadarAISFusionContactType`에 동일 필드명/자료형/단위가 재정의되어 있어 `latitude=-90~90`, `longitude=-180~180`, `course=0~360`, `speed=0~45`, `detectionRange=0~12000`, `collisionRisk=1~100`, `shipType=0~255`를 교차확인하였다. AIS 자체 행에서 독립적으로 명확한 `contactID>=20000`, `heading=0~360`, `rateOfTurn=-90~90`도 반영하였다.
   - `shipLength/shipWidth`처럼 교차 구조에도 Range가 없는 항목은 임의 범위를 만들지 않고 Unit/Unavailable 의미만 유지한다. 이 기준으로 표 이상값 이슈를 종료한다.


## 4. 현재 상태

- 원문 직접 근거가 있는 Semantic/Binding 정합성 보강과 항해레이더/공용구조체 CSV 재심층 감사까지 완료하였다.
- AISContactType 표 정렬 이상 1건은 동일 공용 구조체의 RadarAISFusionContactType 교차근거로 해소하였다. 미해결 6건은 추가 CSCI/IDL/Adapter 근거가 필요한 비차단 TBD이다.
