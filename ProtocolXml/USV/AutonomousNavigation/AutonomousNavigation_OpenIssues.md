# AutonomousNavigation Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 자율운항장치(ANU).
- RCU 직접 대상인 임무수행 상태, 레코드 ACK, PBIT/CBIT/IBIT, 충돌회피 불가, 연료경고, 전체 임무경로점 및 17개 Control을 포함한다.
- ANU→선체제어장치 `USVControlType`은 RCU 직접 접점이 아니므로 제외한다.

## 2. 이번 감사에서 확정/반영

- 공용 Control destination은 `System.Target.AutonomousNavigation`으로 준비하며 공용 식별자는 AutonomousControl 0x02 / AutoNavControlCard 0x01이다.
- Semantic의 raw 숫자 enum을 제거하고 운용모드, 임무종류, 임무상태, 수행가능여부, BIT, 충돌회피, 연료경고, EndAction을 논리값으로 정의하였다.
- `missionStateID` wire 1~8은 converter 없이 Binding ValueMap으로 8개 Mission.Type 논리값에 연결하였다.
- 레코드 명령/ACK의 공용 구조체 primitive type을 반영하고 CommandStatus ACK와 RecordCommand echo Reply를 분리 유지하였다.
- Underwater `searchingRange`는 UInt16 raw와 Semantic meter 간 `scale=100`으로 선언하여 `Scale0.01` converter를 제거하였다.
- PBIT/IBIT는 처리 ACK와 전용 결과 Reply를 분리하고 ProcessorIBIT 온도/CPU Load 결과를 반영하였다.
- 원문에 값 의미가 없는 `headingAcheived`, `hoverRadiusAcheived`, `targetTrackingAcheived`는 임의 0/1 enum 가정을 제거하였다.
- AutonomousDocking `distanceRemaining`은 원문 Unit 미기재이므로 기존 m 추정을 제거하였다.

## 3. Remaining TBD

1. **RecordCommand Size 불일치**: RCU/ANU CSCI의 Size 열과 동일 타입의 공용 구조체 실제 필드/sequence가 일치하지 않는다. 현재 공용 구조체를 우선한다.
2. **PBIT/IBIT Result correlation**: 전용 ANUPBIT/ANUIBIT 결과에 commandID가 없어 처리 ACK 이후 매칭 규칙 확인이 필요하다.
3. **Achieved 상태 code**: GlobalHover `headingAcheived`/`hoverRadiusAcheived`, TargetTracking `targetTrackingAcheived`는 octet이지만 raw 값 의미가 원문에 없다.
4. **GlobalHover 시간 의미/인코딩**: timeHoverAcheived/timeHoverCompleted의 절대시각·지속시간 의미와 UInt64 인코딩 기준이 불명확하다.
5. **상태 거리/각도 Unit**: distancePointRemaining, errorYawAngle, distanceTargetRemaining의 Unit이 원문에 없다.
6. **AutonomousDocking distance Unit**: 동일 이름의 다른 상태 메시지와 달리 원문 Unit이 미기재되어 단위를 추정하지 않았다.
7. **Fuel 필드 Unit/시간 인코딩**: fuelQuantityRemain, fuelQuantityPercent의 명시적 Unit과 alertTime UInt64 인코딩 규칙 확인이 필요하다.
8. **Underwater timeLimit Unit**: 시간 제한이라는 의미와 범위는 있으나 wire Unit이 원문에 명확히 기재되지 않았다.
9. **TargetTracking range 1~49**: 0은 충돌회피만 수행, 50~2000은 이격거리 유지로 정의되지만 1~49의 의미는 없다.
10. **targetTrackingID X.Y 식별 체계**: `0xX.Y.0001~0xX.Y.FFFE`의 X/Y 의미는 원문에 정의되지 않았다.
11. **RestrictArea 형상 규칙**: 두 좌표점만 제공되며 이를 사각형/다각형으로 구성하는 규칙이 없다.
12. **EmergencyReturn action=3**: Range에는 포함되지만 0~2만 의미가 정의되어 3은 Semantic에서 제외하였다.
13. **Record echo correlation**: 전용 RecordCommandAckReport는 commandID가 없어 CommandStatus ACK와 echo의 런타임 상관관계 확인이 필요하다.
14. **Aided record/time conventions**: 레코드의 endTime/arrivalTime 등 UInt64 시간값의 공통 epoch/format이 원문에 없다.

## 4. 현재 상태

- RCU 직접 연동 범위의 Semantic/Binding 구조 정리는 완료하였다.
- 남은 14건은 사용자 정책 선택사항이 아니라 추가 CSCI/IDL/Adapter 근거가 필요한 비차단 TBD이다.
