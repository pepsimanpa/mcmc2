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
- 자율운항장치 CSCI + 원격통제장치 CSCI + 공용 구조체를 재심층 교차감사하였다. `timeHoverAcheived`는 원문 비고에서 "실제 처음 달성된 절대 시간"으로 직접 확인되었고, `timeHoverCompleted`는 필드 설명 "유지 시간"과 비고 "완료될 것으로 추정되는 절대 시간"이 충돌함을 재확인하였다.
- `fuelQuantityPercent`는 ANU CSCI에서 0~100 범위와 잔여연료 비율 의미를 직접 확인했지만 Unit 열은 비어 있으며, `alertTime`은 "년, 월, 일, 시, 분, 초"라고만 기재되어 UInt64 패킹/epoch은 확인되지 않았다.
- 공용 구조체 `UnderwaterMissionPlanRecordCommand.timeLimit`은 UInt32 계열(`unsigned long`) / Range 0~14400까지는 확정되지만 Unit 열이 `-`이므로 기존 Semantic의 `s` 단위 추정을 제거하였다.
- AutonomousDocking `distanceRemaining`의 Unit 열도 실제로 `-`임을 재확인하여 단위를 미지정으로 유지한다. Achieved 3개 code, TargetTracking 1~49, targetTrackingID X.Y, RestrictArea 형상, EmergencyReturn action=3은 관련 CSV 전체에서도 추가 정의를 찾지 못했다.

## 3. Remaining TBD

1. **RecordCommand Size 불일치**: RCU/ANU CSCI의 Size 열과 동일 타입의 공용 구조체 실제 필드/sequence가 일치하지 않는다. 현재 공용 구조체를 우선한다.
2. **PBIT/IBIT Result correlation**: 전용 ANUPBIT/ANUIBIT 결과에 commandID가 없어 처리 ACK 이후 매칭 규칙 확인이 필요하다.
3. **Achieved 상태 code**: GlobalHover `headingAcheived`/`hoverRadiusAcheived`, TargetTracking `targetTrackingAcheived`는 octet이지만 raw 값 의미가 원문에 없다.
4. **GlobalHover 시간 의미/인코딩 — 부분 해결**: `timeHoverAcheived`는 ANU CSCI 비고에서 "실제 처음 달성된 절대 시간"으로 직접 확인된다. 반면 `timeHoverCompleted`는 필드 설명이 "유지 시간"이고 비고는 "완료될 것으로 추정되는 절대 시간 → 앞으로 계속 그 자리에서 유지해야 될 시간"으로 서로 충돌한다. 두 필드 모두 UInt64의 실제 epoch/packing 규칙은 별도 정의가 없어 후자는 의미와 인코딩을 계속 TBD로 유지한다.
5. **상태 거리/각도 Unit**: distancePointRemaining, errorYawAngle, distanceTargetRemaining의 Unit이 원문에 없다.
6. **AutonomousDocking distance Unit**: 동일 이름의 다른 상태 메시지와 달리 원문 Unit이 미기재되어 단위를 추정하지 않았다.
7. **Fuel 필드 Unit/시간 인코딩 — 부분 해결**: `fuelQuantityPercent`는 ANU CSCI에서 의미가 "잔여연료 비율"이고 Range 0~100으로 직접 확인되지만 Unit 열은 비어 있어 `%` 단위를 계약값으로 추가하지 않는다. `fuelQuantityRemain` 역시 Unit 미기재다. `alertTime`은 UInt64이고 비고에 "년, 월, 일, 시, 분, 초"만 있어 달력시각 성격은 확인되지만 실제 bit packing/epoch/format은 여전히 TBD이다.
8. **Underwater timeLimit Unit**: 공용 구조체에서 `timeLimit`은 `unsigned long`, Range 0~14400, 설명 "제한시간"까지는 직접 확인되지만 Unit 열은 `-`이다. 따라서 기존 Semantic의 `s` 단위는 원문보다 앞선 추정으로 확인되어 제거했으며, 실제 시간 단위는 계속 TBD이다.
9. **TargetTracking range 1~49**: 0은 충돌회피만 수행, 50~2000은 이격거리 유지로 정의되지만 1~49의 의미는 없다.
10. **targetTrackingID X.Y 식별 체계**: `0xX.Y.0001~0xX.Y.FFFE`의 X/Y 의미는 원문에 정의되지 않았다.
11. **RestrictArea 형상 규칙**: 두 좌표점만 제공되며 이를 사각형/다각형으로 구성하는 규칙이 없다.
12. **EmergencyReturn action=3**: Range에는 포함되지만 0~2만 의미가 정의되어 3은 Semantic에서 제외하였다.
13. **Record echo correlation**: 전용 RecordCommandAckReport는 commandID가 없어 CommandStatus ACK와 echo의 런타임 상관관계 확인이 필요하다.
14. **Aided record/time conventions**: 레코드의 endTime/arrivalTime 등 UInt64 시간값의 공통 epoch/format이 원문에 없다.

## 4. 현재 상태

- RCU 직접 연동 범위의 Semantic/Binding 구조 정리와 ANU/RCU/공용구조체 CSV 재심층 교차감사까지 완료하였다.
- 재심층 감사 후에도 미해결 이슈 수는 14건이며, 일부는 근거 범위를 좁히고 잘못 추정된 `timeLimit` 단위를 철회하였다. 남은 내용은 추가 IDL/Adapter/운용 규칙 근거가 필요한 비차단 TBD이다.
