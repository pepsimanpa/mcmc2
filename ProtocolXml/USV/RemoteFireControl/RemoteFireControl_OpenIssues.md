# RemoteFireControl Semantic/Binding 감사 및 Open Issues

## 1. 범위

- 대상 경계: 원격통제장치(RCU) ↔ 원격사격통제장치 CSCI(RCWS).
- 근거: `원격사격통제장치 CSCI.csv`, `원격통제장치 CSCI.csv`, `공용 구조체.csv`, `공용 규칙.csv`, `공용 식별자 규칙.csv`.
- 중앙통제장치 전용 상태/녹화 RTP 등 원통 직접 접점이 아닌 연동은 제외한다.
- RCU 직접 RTP 영상은 SensorProduct로 유지한다.

## 2. 이번 감사에서 확정/반영

- 공용 식별자는 `Weapon=0x07 / RemoteFireControlSystem=0x01 / subEquipment=0x00`이다.
- 공용 Control destination은 `System.Target.RemoteFireControl`로 OM/Adapter가 준비한다.
- 원통 직접 Control 14개, Reply 20개, Monitor 3개, SensorProduct 1개를 대조하였다.
- RCWS CBIT/PBIT/상세 상태의 raw enum 숫자를 Semantic에서 제거하고 Binding `ValueMap`으로 이동하였다.
- 구동 자동지향, 카메라 종류, 수동 선보상 방향, Trigger mode, 사격/장전 mode, 메뉴 속도단계/SystemAction을 논리 `ValueSetProfile` + Binding `ValueMap`으로 정리하였다.
- 이벤트/Boolean 제어는 `SourceValueMap`으로 declarative 변환하였다.
- MenuConfig의 `displayIRW/displayIRT/patrolMode/calTrajectory`는 원문 송신 의미가 상태값과 반대이므로 `false→1 / true→0` SourceValueMap으로 기존 반전 converter를 대체하였다.
- `EOIRAimConfigType.commandID`는 원문 특수 규칙에 따라 `System.Communication.LastReceivedCommandID`를 그대로 계승한다.
- 원문 근거가 확인되는 RCWS primitive `dataType`을 보강하였으며 의도적 composite를 제외한 누락은 0건이다.
- 기존 `BuildUSVMessageBase`, `UInt16`, `BooleanToInverted01` converter를 제거하였다.
- PBIT/IBIT 및 CIPE/EOIR 지향·거리측정·메뉴 결과는 각 실제 물리 결과 메시지별 단일 Reply 구조를 유지하였다.
- 동일 RCWS CSCI 내부 교차검증으로 `firingMode` 유효 raw를 0/1로 확정하고 `turningAngle` 범위를 -180~180 deg로 확정하였다.
- RCWS IBIT detail 4개 octet은 CSCI의 bit 위치/대상 정의를 따라 PackedField로 분해하되, 0/1 polarity가 없어 bit별 Raw 의미로 유지하였다.
- 원격통제장치 CSCI의 `MenuConfigType`을 재대조하여 `stabilization(0=초기,1=토글)`과 `initZeroing(0=초기,1=영점초기화)`을 논리 Boolean 입력 + Binding `SourceValueMap(false→0,true→1)`으로 정리하였다. `stabilization`은 원문에 1 이후 0 초기화가 명시되지만 `initZeroing`에는 동일 문구가 없어 후자의 one-shot reset 정책은 TBD로 유지한다.

## 3. Remaining TBD — 9개

1. **ControlPanel joystick raw 값 ↔ 각도 변환**
   - `height`/`turn`은 물리 raw long 0~65535로 정의되어 있고 비고에는 조이스틱 가동범위 -20~+20 deg가 기재되어 있다.
   - raw 값과 각도의 변환 공식/중립값이 정의되지 않아 Semantic은 raw 입력 계약을 유지한다.

2. **IBIT detail bit polarity / 감시카메라 명칭 충돌**
   - 이번 RCWS CSCI 재검토로 `drivingIBIT_Detail`, `surveillanceIBIT_Detail`, `strikeIBIT_Detail`, `controlIBIT_Detail`의 bit 위치와 대상은 직접 확인되어 `PackedField/BitMember`와 Semantic bit별 Raw Result로 분해하였다.
   - 다만 각 bit의 `0/1` 중 어느 값이 정상/이상인지 직접 정의되어 있지 않아 `BooleanResult`/`ValueMap`은 적용하지 않는다.
   - `surveillanceIBIT_Detail` Bit1/2는 원문 symbolic name(`nightWideAngleCameraIBIT`/`nighttelephotoCameraIBIT`)과 한글 설명(협각/광각)이 서로 뒤바뀐 형태여서 원작자/IDL 확인 전 의미를 교정하지 않는다.

3. **PBIT/IBIT Result correlation**
   - 공용 요청에는 commandID가 있으나 전용 PBIT/IBIT 결과 Report에는 동일 commandID가 없다.
   - CommandStatus ACK 이후 전용 결과를 어느 요청과 연결하는지 런타임 규칙 확인이 필요하다.

4. **전용 결과 Report correlation**
   - `CIPEAimStatusType`, `EOIRAimStatusType`, `DistanceMeasureReportType`, `MenuStatusType`에는 요청 commandID와 직접 연결할 식별자가 없다.
   - 동시/연속 요청 시 결과 correlation 규칙 확인이 필요하다.

5. **거리측정 특수값**
   - `targetDistance`: 0=Unknown, 1~4000=m 유효값이다.
   - UInt16의 4001~65535 값은 의미가 정의되지 않았다.
   - Sentinel/Invalid를 최종 Semantic 공통 모델에서 어떻게 노출할지는 전역 후속 감사 대상이다.

6. **No-driving zone -30 특수값 적용 범위**
   - 원통 측 제어 원문은 `noDrivingZone1`에 -30 deg 설정 시 해당 구동제한구역을 설정하지 않는다고 명시하지만, 이번 RCWS CSCI의 `MenuStatusType`은 zone1~12 모두 단순 -30~30 deg 상태값으로만 기재한다.
   - 동일 특수 규칙이 zone2~12 제어에도 적용되는지는 여전히 직접 근거가 없어 확대 적용하지 않는다.

7. **`initZeroing` one-shot reset 규칙**
   - 원격통제장치 CSCI에서 wire 값 `0=초기, 1=영점 초기화`는 직접 확인되어 Binding `SourceValueMap(false→0,true→1)`으로 반영하였다.
   - 바로 앞 `stabilization` 등 다른 이벤트 필드는 `1 이후 다시 0으로 초기화`가 명시되어 있지만 `initZeroing`에는 그 문구가 없다. 따라서 wire 인코딩은 확정하되, 송신 후 OM/UI가 자동으로 논리값을 false로 되돌려야 하는지는 TBD로 유지한다.

8. **Menu SystemAction과 공용 Restart 중복**
   - `SystemAction`: 0=None, 1=Shutdown, 2=Reboot가 원통 측 제어 원문에 존재하며 `2=Reboot`는 공용 `SystemRebootControlType`과 기능적으로 중복된다.
   - 이번 RCWS CSCI는 MenuStatus 결과만 제공하고 우선순위/사용 조건을 정의하지 않아 둘 다 보존한다.

9. **물리 오탈자 / 실제 IDL 철자**
   - 이번 RCWS CSCI 자체에서 `DetailStatusType.cipeAming` 오탈자가 재확인되어 Binding의 해당 물리 이름 보존은 CSCI 기준으로 확정한다.
   - `ControlPanalConfigType` 등 원통 측 제어 타입의 실제 DDS IDL에서도 동일 철자를 사용하는지는 IDL 확보 시 재확인한다.

### 이번 CSV 재검토로 해결된 항목

- **`firingMode` raw 2/3**: `DetailStatusType`의 Range `0~3`과 달리 동일 RCWS CSCI의 `RCWSHeartBeatReportType.firingMode`는 Range `0~1`이고 양쪽 모두 `0=안전 / 1=사격`만 정의한다. 동일 의미 필드 교차검증에 따라 유효 코드는 0/1로 확정하고 2/3은 Range 표기 오류로 정리한다.
- **`turningAngle` Range**: `DetailStatusType`의 오염된 `0~360-180~180` 표기와 달리 동일 RCWS CSCI의 `StatusType.turningAngle`은 `-180~180 deg`로 명확하다. 동일 포탑구동 선회각 필드 교차검증에 따라 Semantic Range를 `-180~180 deg`로 확정한다.

## 4. 현재 상태

- 원통 직접 연동 범위의 Semantic/Binding 구조 정리와 선언형 변환은 완료하였다.
- RCWS/원격통제장치 CSCI 교차감사로 `firingMode`/`turningAngle` 2건은 해결하고 IBIT detail 및 Menu event wire 매핑을 정밀화하였다. 남은 9건은 추가 IDL/Adapter/운용 규칙 근거가 필요한 비차단 TBD이다.
