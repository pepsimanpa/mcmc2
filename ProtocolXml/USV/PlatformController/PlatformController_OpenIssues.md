# PlatformController Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 선체제어장치 CSCI(SPCE).
- 근거: `선체제어장치 CSCI.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`.
- 선체제어장치 내부의 엔진/워터젯/전력/환경 하위 장치 간 내부 연동은 본 Binding 범위가 아니다.
- 공용 생성 header/destination/commandID, CommandStatus 처리 ACK, Semantic raw-code 제거, declarative Binding 원칙을 적용한다.

## 2. 이번 감사에서 확정/반영

- 공용 식별자에 따라 선체제어 일반 대상은 `PlatformManagement=0x05 / HullManagementCard=0x01 / PlatformController subEquipment=0x01`로 정리하였다.
- 공용 명령 destination은 `System.Target.PlatformController`로 OM/Adapter가 준비하고 Binding은 물리 타입만 연결한다.
- `USVControlType`은 원격통제장치 CSCI에서 `RCU → SPCE`, `10 Hz`, `ACK 없음`으로 직접 정의되어 있으므로 `controlPropulsion` Control/ControlBinding으로 포함하였다.
- `USVControlStatusType`의 `targetControlState` 0~3 의미는 Semantic 논리값과 Binding ValueMap으로 분리하였다.
- 공용 구조체 기준 추진제어 primitive type을 반영하였다: targetControlState=UInt8, targetCourse/targetVelocity=Float32, steering/throttle/joystick=Int16, bowthrustTargetThrottle=UInt16.
- `integratedTargetSteering`, `integratedTargetThrottle`은 공용 구조체에서 현 프로젝트 미사용으로 표기되어 Semantic 입력에서는 제외하고 송신 Binding은 0 고정값으로 유지하였다.
- `PowerHeartBeatReportType.portONStatus`의 CSCI 직접 bit 50~0 장비 매핑은 `PackedField/BitMember`로 선언하여 기존 `BitNToBoolean` converter를 제거하였다.
- `DistressReportType.distress`의 0x01~0x20 의미는 `PackedField/BitMember`로 선언하여 mask converter를 제거하였다.
- 공용 header, DestinationType, commandID의 `BuildUSVMessageBase`, `BuildDestinationType`, `UInt16` converter를 제거하였다.
- Power/CCTV 제어 mask는 미확정 bit 규칙을 Binding converter에 숨기지 않고 OM/Adapter prepared value(`System.Prepared.*Mask`)로 이동하였다.
- ENV 자동소화 command boolean→octet은 `SourceValueMap(false→0, true→1)`으로 선언하였다.
- OperationMode/CBIT/전원상태 등 Semantic raw 숫자를 논리값으로 이동하고 확인 가능한 raw code는 Binding ValueMap/FixedField에 배치하였다.
- 공통 XSD 참조 경로를 `../../XSD/...`로 정리하였다.

## 3. Remaining TBD / Resolved

1. **PowerControlType 장비별 64-bit 제어 mask 매핑**
   - `powerControlDestination`, `stateControl`, `reset`은 `unsigned long long`이다.
   - 상태 보고 `portONStatus`의 bit 50~0 장비 의미는 확인되지만 PowerControlType의 세 mask가 같은 bit 위치 체계를 사용한다는 직접 문구는 부족하다.
   - 현재 OM/Adapter가 완성된 mask를 준비하고 Binding은 UInt64 wire 값만 연결한다.

2. **Power reset 시 `stateControl` 동시 의미**
   - Reset 제어에서 reset mask와 stateControl을 동시에 어떤 값으로 보내야 하는지 원문 규칙이 명확하지 않다.
   - 현재 `System.Prepared.PlatformPowerStateMask`로 남기며 임의 0/1 정책을 강제하지 않는다.

3. **CamPowerControlType.cctvMask relay bit 정의**
   - `cctvMask`는 octet이지만 relay별 bit 위치와 전체상태 설정/부분변경 의미가 명확하지 않다.
   - 현재 `System.Prepared.CameraPowerMask`를 OM/Adapter가 준비한다.

4. **ENVCommandType.seaWaterAirConditionerControl 값 의미**
   - 자료형은 octet이나 실제 논리 enum/범위가 직접 정의되어 있지 않아 Semantic에 일반 Profile로 유지한다.

5. **ENVCommandType ACK 표시와 commandID 부재**
   - 원 CSCI는 ACK=O로 표시하지만 메시지 구조에는 commandID가 없다.
   - 공용 `CommandStatusReportType`과 어떤 규칙으로 correlation하는지 확인 전 Reply를 강제로 연결하지 않는다.

6. **TowingDeviceControlType ACK 표시와 commandID 부재**
   - 원 CSCI는 ACK=O로 표시하지만 메시지 구조에는 commandID가 없다.
   - 실제 잠금 상태는 Monitor로 확인 가능하나 CommandStatus 처리 ACK correlation 규칙은 별도 확인이 필요하다.

7. **PBIT/IBIT Result correlation**
   - 공용 PBIT/IBIT 요청에는 commandID가 있으나 선체제어의 상세 결과 메시지들에는 commandID가 없다.
   - 동시에 여러 시험 요청이 가능한 경우 어떤 런타임 규칙으로 연계하는지 확인이 필요하다.

8. **선체제어 PBIT/IBIT의 다중 결과 메시지 모델**
   - 선체제어 CSCI는 Engine/WaterJet/BowThruster/Power 등 서로 다른 물리 Result Report Type을 각각 정의한다.
   - 따라서 '내부 필드를 여러 Reply로 쪼개지 않는다'는 공통 원칙을 유지하면서도, 물리 메시지 타입별 Reply는 각각 유지한다.
   - 향후 원작자가 하나의 상위 시험 완료 집계 메시지를 정의할 경우 재검토한다.

9. **[RESOLVED] PlatformController Boolean wire 재감사**
   - `선체제어장치 CSCI.csv`를 직접 재확인한 결과 PlatformController 원문에는 primitive `boolean` 필드가 없다.
   - 기존에 `Boolean`으로 선언했던 `fireDetected`, `leakDetected`, `flowAnomalyDetected`, `fireSensor1~3Alarm`, `autoExtinguisher1~4Active` 10개는 모두 원문 `octet`이므로 `UInt8`로 정정하였다.
   - 이 필드들의 Semantic Boolean 의미는 유지하되, wire primitive type은 원문 `octet`을 따른다.

10. **[RESOLVED] 선체 상태/PBIT/IBIT primitive dataType 전수 감사**
   - `선체제어장치 CSCI.csv`의 각 DDS message/type별 필드 정의를 기준으로 기존 미지정 primitive field의 `dataType`을 전수 보강하였다.
   - Engine/WaterJet/BowThruster/Power/Battery/Generator/Environment/Wind/Fire/Leak/Flow/Stabilizer/Interceptor/SeaWaterConditioner/TowingDevice/SPCE IBIT와 CBIT/PBIT, Propulsion/ENV/Power HeartBeat, Distress/Wind/SeaWaterConditioner 상태를 원문 타입대로 반영하였다.
   - `vcuProcessorIBIT`, `vcuTmpProcessorIBIT`는 원문 `ProcessorIBIT` 복합 구조체이므로 primitive `dataType`을 부여하지 않는다.
   - 최종 감사에서 composite/system field를 제외한 primitive `dataType` 미지정은 0건이며 XSD 검증을 통과하였다.

11. **CBIT 내부 boolean/bitfield 세부 의미**
   - `powerStatus`, 일부 Environment/IBIT 상태는 bitfield 또는 장치고유 raw 상태로 보인다.
   - 직접 bit 의미가 확인되지 않은 필드는 임의 Semantic 분해하지 않는다.

12. **USVControlType 운용모드별 유효 파라미터 조건**
   - targetControlState가 CourseVelocity/RpmSteering/Joystick일 때 각각 어떤 필드만 유효한지 구조적으로 추정 가능하지만 CSCI의 명시적 조건문이 부족하다.
   - Semantic은 전체 물리 구조를 노출하고 런타임 운용 제약은 OM/execution 책임으로 둔다.

## 4. 현재 상태

- 원통 직접 RCU 연동 범위에서 근거가 있는 설계 수정은 반영하였다.
- 남은 항목은 사용자 정책 선택보다는 추가 CSCI/IDL/Adapter/원작자 근거가 필요한 비차단 TBD이다.
