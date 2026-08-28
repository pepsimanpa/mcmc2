# ElectroOptical Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 전자광학장치 CSCI(EOIR 본체, equipmentID 0x05).
- `전자광학추적장치 CSCI.csv`로 명명된 영상처리 CSC(EoSystemProcessor 0x06)는 별도 `ElectroOpticalProcessor` 범위이며 본 문서에서 제외한다.
- 공통 생성 header/destination/commandID, CommandStatus 처리 ACK, PBIT/IBIT 전용 결과, SensorProduct 원칙을 적용한다.

## 2. 이번 감사에서 확정/반영

- 공용 식별자: `SurfaceDetection=0x03 / EOIR=0x05 / subEquipment=0x00`.
- 공용 Control destination은 `System.Target.ElectroOptical`로 OM/Adapter가 준비한다.
- 원통과 직접 연결된 20개 Control과 Tracking/LRF/Heartbeat/CBIT/PBIT/IBIT, EO/IR RTP를 범위로 유지한다.
- Semantic의 운용모드/추적/Zoom/Focus/EO/MWIR/SWIR/LRF/Wiper/Power raw enum 숫자를 제거하고 논리 `ValueSetProfile`/`ValueSetSpec`으로 변경하였다.
- 실제 raw code는 Binding `ValueMap`/`FixedField`로 이동하였다.
- `EOIRTrackingBboxType`: targetNumber=UInt8, center/size=UInt16을 공용 구조체대로 반영하였다.
- 원통 Control primitive: joystick=Int16, pan/tilt/swing=Float32, targetId/Zoom·Focus position=UInt16, enum=UInt8, commandID=UInt16을 반영하였다.
- `statusLrfLife`는 원문 raw 0~10000을 /100하여 percent로 사용하는 규칙을 `dataType=UInt16, scale=100`으로 선언하였다.
- Zoom/Focus 위치값은 Semantic percent → wire raw 변환이 x2이므로 `scale=2`로 선언하였다.
- PBIT/IBIT는 CommandStatus 처리 ACK와 별개의 단일 EOIR Result Reply로 유지하고 내부 결과를 한 Reply 안에 보존하였다.
- `BuildUSVMessageBase`, `UInt16`, `DivideBy100`, `MultiplyBy2` converter를 선언형 Binding으로 대체하였다.
- 공통 XSD 경로를 `../../XSD/...`로 정리하였다.
- 전자광학장치 CSCI 재심층 감사에서 `statusPowerSwir/statusControlSwir`의 4상태 코드를 직접 확인하여 Semantic ValueSetResult + Binding ValueMap으로 반영하였다.
- IBIT detail 5개 octet은 CSCI에 명시된 bit 위치/점검 대상까지 PackedField/BitMember 및 Semantic GroupResult로 분해하되, 0/1 polarity는 근거가 없어 bit별 Raw로 유지하였다.
- Boolean WireDataType 추가 전의 오래된 Binding 주석을 현재 공용 XSD 정책에 맞게 정리하였다.

## 3. Remaining TBD / Resolved

1. **[RESOLVED] DDS boolean의 Binding wire type 표현**
   - 원 CSCI에 `boolean`인 필드가 다수 존재한다: referenceModeResult, autoTrackingSetting, swingFlagResult, irSwitching, eoirFlagResult, laserIlluminatorSetting 등.
   - `CommonBindingSchema.xsd/WireDataType`에 `Boolean`을 추가하였다.
   - 원 CSCI/IDL primitive가 실제 `boolean`인 8개 직접 Field를 `dataType="Boolean"`으로 선언하였다.
   - Semantic Boolean이라도 원 wire가 octet/bit/sentinel인 항목은 Boolean으로 강제 변경하지 않는다.

2. **Zoom/Focus `0x5555` No-change sentinel**
   - `positionZoomControl` / `positionFocusControl`은 유효 raw 0~1000 외에 `0x5555=No change`가 존재한다.
   - 동시에 별도 `zoomControl/focusControl`에 No-change 상태가 있어 sentinel이 어느 조건에서 우선되는지 원문 실행 규칙이 불충분하다.
   - scale=2는 확정 반영하되 0x5555 생성 조건은 임의 구현하지 않는다.

3. **Swing 속도 Unit 충돌**
   - 필드명/설명은 `swingSpeedResult`, '스윙 속도/각속도'이나 원문 Unit은 `deg`이다.
   - 기존 XML의 `deg/s` 해석은 추론이므로 철회하였다.
   - Semantic에는 원문 Unit `deg`를 보존하며 실제 속도 단위가 deg/s인지 원작자 확인이 필요하다.

4. **SWIR 추적 sensor code**
   - Tracking의 sensorSelection/trackingSensor는 EO/MWIR까지만 원문 code가 정의되어 있다.
   - SWIR 영상 전환/세부 설정은 존재하지만 SWIR 추적 code는 없다. 임의 추가하지 않는다.

5. **EOIRPowerControlType의 SWIR 전원 필드 부재**
   - SWIR 설정/영상전환/IBIT 상태는 존재하지만 Power Control에는 EO/MWIR/LRF/Illuminator만 있고 `swirPower`가 없다.
   - 별도 SWIR 전원 제어가 필요한지 원문 추가 확인이 필요하다.

6. **IR RTP 물리 채널**
   - 원본 RCU 직접 RTP는 MWIR 영상으로 명명되어 있고 별도 SWIR RTP 인터페이스가 확인되지 않는다.
   - `EOIRSwitchingControlType`은 MWIR/SWIR 전환을 제공하므로 Semantic은 공용 `irVideo`로 유지한다.
   - 동일 물리 RTP 포트를 공유하는지 여부는 ICD 확인 대상이다.

7. **Wiper 정의되지 않은 값**
   - 원문 Range는 0~9지만 유효 정의는 0~3이다.
   - 4/5는 삭제 요청, 6~9는 의미 미정의이므로 Semantic/Binding ValueMap에서 제외하였다.

8. **LRF 특수 거리값**
   - `statusDistance`: 0=Unknown, 65535=Fail, 1~50000=유효 m 거리.
   - 현재 Semantic은 유효 거리만 노출한다. Unknown/Fail을 별도 논리 상태로 노출할지 여부는 전역 Sentinel/상태 모델 감사에서 재검토한다.

9. **PBIT/IBIT Result correlation**
   - 공용 요청에는 commandID가 있으나 EOIR 전용 PBIT/IBIT 결과에는 commandID가 없다.
   - CommandStatus ACK 이후 결과의 런타임 매칭 규칙 확인이 필요하다.

10. **IBIT 내부 bit polarity — 부분 해결**
   - 전자광학장치 CSCI 재심층 감사로 `statusInternalGimbal`, `statusSensorAssembly1/2`, `statusExternalGimbal`, `statusStabilizationControlBoard`의 bit 위치와 각 점검 대상은 직접 확인하였다. 따라서 Binding `PackedField/BitMember`와 Semantic `GroupResult`로 bit별 항목을 분해하였다.
   - 다만 원문은 각 bit의 `0/1` 중 어느 값이 정상/이상인지 직접 정의하지 않는다. 따라서 `BooleanResult`/ValueMap으로 확대 해석하지 않고 각 bit를 Raw 의미로 유지한다.

## 4. 현재 상태

- 직접 RCU 연동 범위의 원문 근거가 있는 Semantic/Binding 정합성 보강과 전자광학장치 CSCI 재심층 감사까지 완료하였다.
- 남은 항목은 사용자가 임의로 정책을 선택할 사항이 아니라 추가 IDL/XSD/ICD/원작자 근거가 필요한 비차단 TBD이다.
